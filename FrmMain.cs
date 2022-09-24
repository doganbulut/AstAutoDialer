// Type: AstAutoDialer.FrmMain
// Assembly: AstAutoDialer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B1750608-7C33-45D3-A128-4749BC17D73C
// Assembly location: I:\TLCSilme\AstDialer\AstAutoDialer.exe

using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace AstAutoDialer
{
  public class FrmMain : Form
  {
    private int AgentCount = 12;
    private ActvDialer AstDial;
    private ActvDialer.TAgent[] AstAgent;
    private ActvDialer.TTrunk[] AstTrunk;
    private int[] AgentFree;
    private IContainer components;
    private Timer tmrAgent;
    private Label lblAstState;
    private ListBox lstAgent;
    private Button button1;
    private Button button2;
    private ListBox lstTrunk;
    private TextBox txtExten;
    private TextBox txtQueue;
    private CheckBox chkstate;
    private Button button3;
    private Label label1;
    private ComboBox cmdCampains;
    private Button button4;
    private Label label2;
    private Label label3;
    private Button button5;

    public FrmMain()
    {
      this.InitializeComponent();
      this.LoadCampain();
    }

    private void AstDial_ProcessHangup(string DBID, int Cause)
    {
      int result = 0;
      if (!int.TryParse(DBID, out result))
        return;
      this.EndCall(result, Cause);
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.AstDial = new ActvDialer(this.cmdCampains.SelectedItem.ToString(), 201, this.AgentCount, "902");
      this.AstDial.ProcessHangup += new ActvDialer.DlgHangup(this.AstDial_ProcessHangup);
      if (this.AstDial.ConnectPBX())
      {
        this.lblAstState.Text = "Santrale bağlanıldı";
        this.AgentFree = new int[this.AgentCount];
        this.AstAgent = new ActvDialer.TAgent[this.AgentCount];
        this.AstTrunk = new ActvDialer.TTrunk[this.AgentCount];
        for (int index = 0; index < this.AgentCount; ++index)
          this.AgentFree[index] = 0;
        this.tmrAgent.Enabled = true;
      }
      else
      {
        this.lblAstState.Text = "Hata Santrale bağlanılmadı";
        this.tmrAgent.Enabled = false;
      }
    }

    private void button2_Click(object sender, EventArgs e)
    {
      if (!this.AstDial.DisconnectPBX())
        return;
      this.tmrAgent.Enabled = false;
      this.lblAstState.Text = "Santralle bağlantı kesildi";
    }

    private void tmrAgent_Tick(object sender, EventArgs e)
    {
      this.tmrAgent.Enabled = false;
      this.AstAgent = this.AstDial.AgentStatus();
      this.lstAgent.Items.Clear();
      this.lstTrunk.Items.Clear();
      for (int index = 0; index < this.AgentCount; ++index)
      {
        this.lstAgent.Items.Add((object) (this.AstAgent[index].Exten + (object) ":" + this.AstAgent[index].Status + ":" + this.AstAgent[index].NemberQueue + ":" + (this.AstAgent[index].QueuePaused ? 1 : 0) + ":" + this.AstAgent[index].BridgeUniqID));
        if (!this.AstAgent[index].QueuePaused && this.AstAgent[index].NemberQueue == "902" && !this.AstAgent[index].Speak)
        {
          if (this.AstAgent[index].Status == "0")
          {
            this.AgentFree[index] = this.AgentFree[index] + 1;
            if (this.AgentFree[index] == 4)
            {
              this.AgentFree[index] = 0;
              string Phone = "";
              int dbCall = this.GetDBCall(this.txtQueue.Text, this.cmdCampains.SelectedItem.ToString(), out Phone);
              if (dbCall != 0)
              {
                if (Phone.Length == 10)
                  Phone = "0" + Phone;
                this.AstDial.NewCallAgent(this.txtQueue.Text, Phone, dbCall, "sipone");
              }
            }
            else if (this.AgentFree[index] > 4)
              this.AgentFree[index] = 0;
          }
          else if (this.AstAgent[index].Status == "1")
            this.AgentFree[index] = 0;
        }
      }
      this.AstTrunk = this.AstDial.TrunkStatus();
      for (int index = 0; index < this.AstTrunk.Length; ++index)
        this.lstTrunk.Items.Add((object) this.AstTrunk[index].Activate.ToString());
      this.tmrAgent.Enabled = true;
    }

    protected int GetDBCall(string userid, string campain, out string Phone)
    {
      DataHelper dataHelper = new DataHelper();
      SqlParameter[] prmArray = new SqlParameter[4];
      string str1 = "";
      try
      {
        dataHelper.SetPrmArrayByPrm(prmArray, (short) 0, "@CAMPAIN_ID", SqlDbType.VarChar, campain, false);
        dataHelper.SetPrmArrayByPrm(prmArray, (short) 1, "@USER_ID", SqlDbType.Int, userid, false);
        dataHelper.SetPrmArrayByPrm(prmArray, (short) 2, "@PHONE", SqlDbType.VarChar, 20, (object) str1, true);
        dataHelper.SetPrmArrayByPrm(prmArray, (short) 3, "@RECORD_ID", SqlDbType.VarChar, 10, (object) str1, true);
        dataHelper.GetDataTable(prmArray, "GETRECORDDIALER");
        string str2 = prmArray[2].Value.ToString();
        int num = Convert.ToInt32(prmArray[3].Value.ToString());
        Phone = str2;
        return num;
      }
      catch (Exception ex)
      {
        Phone = "";
        return 0;
      }
    }

    protected bool EndCall(int DBID, int Cause)
    {
      DataHelper dataHelper = new DataHelper();
      SqlParameter[] prmArray = new SqlParameter[2];
      try
      {
        string prmValue;
        switch (Cause)
        {
          case 1:
            prmValue = "ULASILAMADI";
            break;
          case 2:
            prmValue = "no route to network";
            break;
          case 3:
            prmValue = "no route to destination";
            break;
          case 16:
            prmValue = "ARANDI";
            break;
          case 17:
            prmValue = "MESGUL";
            break;
          case 18:
            prmValue = "NOUSER";
            break;
          case 19:
            prmValue = "CEVAP_YOK";
            break;
          case 20:
            prmValue = "subscriber-absent";
            break;
          case 21:
            prmValue = "REDDEDILDI";
            break;
          case 22:
            prmValue = "number-changed";
            break;
          case 23:
            prmValue = "redirection";
            break;
          case 26:
            prmValue = "non-selected";
            break;
          case 27:
            prmValue = "destination-out";
            break;
          case 28:
            prmValue = "address incomplete";
            break;
          case 29:
            prmValue = "facility-rejected ";
            break;
          case 31:
            prmValue = "normal-unspecified";
            break;
          default:
            prmValue = "NOT-DEFINED";
            break;
        }
        dataHelper.SetPrmArrayByPrm(prmArray, (short) 0, "@ID", SqlDbType.Int, DBID.ToString(), false);
        dataHelper.SetPrmArrayByPrm(prmArray, (short) 1, "@CALL_STATUS", SqlDbType.VarChar, prmValue, false);
        dataHelper.ExecuteCommand("UPDATECALLSTATUS", prmArray);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    protected bool LoadCampain()
    {
      try
      {
        SqlDataReader campain = this.GetCampain();
        if (campain == null)
          return false;
        while (campain.Read())
          this.cmdCampains.Items.Add((object) campain.GetString(0));
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    protected SqlDataReader GetCampain()
    {
      DataHelper dataHelper = new DataHelper();
      try
      {
        return dataHelper.GetDataReader("GETCAMPAINS");
      }
      catch (Exception ex)
      {
        return (SqlDataReader) null;
      }
    }

    private void button3_Click(object sender, EventArgs e)
    {
      this.AstDial.AgentLock(this.txtQueue.Text, this.txtExten.Text, this.chkstate.Checked);
    }

    private void btnGetCamp_Click(object sender, EventArgs e)
    {
    }

    private void button4_Click(object sender, EventArgs e)
    {
    }

    private void FrmMain_Load(object sender, EventArgs e)
    {
    }

    private void button4_Click_1(object sender, EventArgs e)
    {
      this.AstDial.AgentAddQueue(this.txtExten.Text);
    }

    private void button5_Click(object sender, EventArgs e)
    {
      this.AstDial.AgentRemoveQueue(this.txtExten.Text);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      this.tmrAgent = new Timer(this.components);
      this.lblAstState = new Label();
      this.lstAgent = new ListBox();
      this.button1 = new Button();
      this.button2 = new Button();
      this.lstTrunk = new ListBox();
      this.txtExten = new TextBox();
      this.txtQueue = new TextBox();
      this.chkstate = new CheckBox();
      this.button3 = new Button();
      this.label1 = new Label();
      this.cmdCampains = new ComboBox();
      this.button4 = new Button();
      this.label2 = new Label();
      this.label3 = new Label();
      this.button5 = new Button();
      this.SuspendLayout();
      this.tmrAgent.Interval = 1000;
      this.tmrAgent.Tick += new EventHandler(this.tmrAgent_Tick);
      this.lblAstState.AutoSize = true;
      this.lblAstState.Font = new Font("Tahoma", 11.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 162);
      this.lblAstState.Location = new Point(13, 9);
      this.lblAstState.Margin = new Padding(4, 0, 4, 0);
      this.lblAstState.Name = "lblAstState";
      this.lblAstState.Size = new Size(76, 18);
      this.lblAstState.TabIndex = 1;
      this.lblAstState.Text = "Ast State";
      this.lstAgent.FormattingEnabled = true;
      this.lstAgent.Location = new Point(12, 56);
      this.lstAgent.Name = "lstAgent";
      this.lstAgent.Size = new Size(199, 186);
      this.lstAgent.TabIndex = 2;
      this.button1.Location = new Point(12, 27);
      this.button1.Name = "button1";
      this.button1.Size = new Size(75, 23);
      this.button1.TabIndex = 3;
      this.button1.Text = "Connect";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new EventHandler(this.button1_Click);
      this.button2.Location = new Point(93, 27);
      this.button2.Name = "button2";
      this.button2.Size = new Size(75, 23);
      this.button2.TabIndex = 4;
      this.button2.Text = "Disconnect";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new EventHandler(this.button2_Click);
      this.lstTrunk.FormattingEnabled = true;
      this.lstTrunk.Location = new Point(217, 56);
      this.lstTrunk.Name = "lstTrunk";
      this.lstTrunk.Size = new Size(550, 186);
      this.lstTrunk.TabIndex = 5;
      this.txtExten.Location = new Point(496, 6);
      this.txtExten.Name = "txtExten";
      this.txtExten.Size = new Size(43, 20);
      this.txtExten.TabIndex = 6;
      this.txtQueue.Location = new Point(496, 27);
      this.txtQueue.Name = "txtQueue";
      this.txtQueue.Size = new Size(43, 20);
      this.txtQueue.TabIndex = 7;
      this.txtQueue.Text = "902";
      this.chkstate.AutoSize = true;
      this.chkstate.Location = new Point(647, 6);
      this.chkstate.Name = "chkstate";
      this.chkstate.Size = new Size(81, 17);
      this.chkstate.TabIndex = 8;
      this.chkstate.Text = "Aktif / Pasif";
      this.chkstate.UseVisualStyleBackColor = true;
      this.button3.Location = new Point(647, 29);
      this.button3.Name = "button3";
      this.button3.Size = new Size(111, 23);
      this.button3.TabIndex = 9;
      this.button3.Text = "Durum Değiştir";
      this.button3.UseVisualStyleBackColor = true;
      this.button3.Click += new EventHandler(this.button3_Click);
      this.label1.AutoSize = true;
      this.label1.Location = new Point(188, 6);
      this.label1.Name = "label1";
      this.label1.Size = new Size(57, 13);
      this.label1.TabIndex = 11;
      this.label1.Text = "Kampanya";
      this.cmdCampains.Font = new Font("Tahoma", 9.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 162);
      this.cmdCampains.FormattingEnabled = true;
      this.cmdCampains.Location = new Point(191, 23);
      this.cmdCampains.Margin = new Padding(4);
      this.cmdCampains.Name = "cmdCampains";
      this.cmdCampains.Size = new Size(217, 24);
      this.cmdCampains.TabIndex = 12;
      this.button4.Location = new Point(545, 4);
      this.button4.Name = "button4";
      this.button4.Size = new Size(96, 23);
      this.button4.TabIndex = 13;
      this.button4.Text = "Kuyruğa Ekle";
      this.button4.UseVisualStyleBackColor = true;
      this.button4.Click += new EventHandler(this.button4_Click_1);
      this.label2.AutoSize = true;
      this.label2.Location = new Point(455, 14);
      this.label2.Name = "label2";
      this.label2.Size = new Size(35, 13);
      this.label2.TabIndex = 14;
      this.label2.Text = "Agent";
      this.label3.AutoSize = true;
      this.label3.Location = new Point(455, 34);
      this.label3.Name = "label3";
      this.label3.Size = new Size(40, 13);
      this.label3.TabIndex = 15;
      this.label3.Text = "Kuyruk";
      this.button5.Location = new Point(545, 27);
      this.button5.Name = "button5";
      this.button5.Size = new Size(96, 23);
      this.button5.TabIndex = 16;
      this.button5.Text = "Kuyruktan Çıkar";
      this.button5.UseVisualStyleBackColor = true;
      this.button5.Click += new EventHandler(this.button5_Click);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(770, 251);
      this.Controls.Add((Control) this.button5);
      this.Controls.Add((Control) this.label3);
      this.Controls.Add((Control) this.label2);
      this.Controls.Add((Control) this.button4);
      this.Controls.Add((Control) this.cmdCampains);
      this.Controls.Add((Control) this.label1);
      this.Controls.Add((Control) this.button3);
      this.Controls.Add((Control) this.chkstate);
      this.Controls.Add((Control) this.txtQueue);
      this.Controls.Add((Control) this.txtExten);
      this.Controls.Add((Control) this.lstTrunk);
      this.Controls.Add((Control) this.button2);
      this.Controls.Add((Control) this.button1);
      this.Controls.Add((Control) this.lstAgent);
      this.Controls.Add((Control) this.lblAstState);
      this.Name = "FrmMain";
      this.Text = "Dialer";
      this.Load += new EventHandler(this.FrmMain_Load);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
