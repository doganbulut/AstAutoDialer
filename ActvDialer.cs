// Type: AstAutoDialer.ActvDialer
// Assembly: AstAutoDialer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B1750608-7C33-45D3-A128-4749BC17D73C
// Assembly location: I:\TLCSilme\AstDialer\AstAutoDialer.exe

using AstAutoDialer.Properties;
using Asterisk.NET.Manager;
using Asterisk.NET.Manager.Action;
using Asterisk.NET.Manager.Event;
using Asterisk.NET.Manager.Response;
using System;
using System.Threading;
using System.Windows.Forms;

namespace AstAutoDialer
{
  public class ActvDialer
  {
    private string mQueue = "";
    private ActvDialer.TTrunk[] Trunks = new ActvDialer.TTrunk[1];
    private ActvDialer.TAgent[] Agents = new ActvDialer.TAgent[1];
    private static ManagerConnection manager;
    private int mAgentFirst;
    private int mAgentCount;

    public event ActvDialer.DlgHangup ProcessHangup;

    public ActvDialer(string Campaign, int ExtFirst, int ExtCount, string Queue)
    {
      this.mAgentFirst = ExtFirst;
      this.mAgentCount = ExtCount;
      this.mQueue = Queue;
      Array.Resize<ActvDialer.TAgent>(ref this.Agents, this.mAgentCount);
      Array.Resize<ActvDialer.TTrunk>(ref this.Trunks, this.mAgentCount);
    }

    public bool ConnectPBX()
    {
      try
      {
        this.InitAgent(this.mAgentFirst, this.mAgentCount);
        this.InitTrunk(this.mAgentCount);
        this.InitAsterisk();
        return true;
      }
      catch
      {
        return false;
      }
    }

    public bool DisconnectPBX()
    {
      try
      {
        for (int index = 0; index < this.mAgentCount; ++index)
          this.AgentRemoveQueue(this.Agents[index].Exten);
        ActvDialer.manager.Logoff();
        return true;
      }
      catch
      {
        return false;
      }
    }

    private bool InitAgent(int Exten, int Count)
    {
      try
      {
        for (int index = 0; index < this.mAgentCount; ++index)
        {
          int num = this.mAgentFirst + index;
          this.Agents[index].Exten = num.ToString();
          this.Agents[index].Name = "SIP/" + num.ToString();
          this.Agents[index].Status = this.AgentState(this.Agents[index].Exten);
          this.Agents[index].CampainQueue = this.mQueue;
          this.Agents[index].NemberQueue = "";
          this.Agents[index].BridgeUniqID = "";
          this.Agents[index].Speak = false;
        }
        for (int index = 0; index < this.mAgentCount; ++index)
          this.AgentRemoveQueue(this.Agents[index].Exten);
        for (int index = 0; index < this.mAgentCount; ++index)
          this.AgentAddQueue(this.Agents[index].Exten);
        return true;
      }
      catch
      {
        return false;
      }
    }

    private bool InitTrunk(int Count)
    {
      try
      {
        for (int i = 0; i < this.mAgentCount; ++i)
          this.ClearTrunk(i);
        return true;
      }
      catch
      {
        return false;
      }
    }

    private bool InitAsterisk()
    {
      try
      {
        ActvDialer.manager = new ManagerConnection(Settings.Default.astserver, 5038, Settings.Default.astuser, Settings.Default.astpass);
        ActvDialer.manager.NewCallerId += new NewCallerIdEventHandler(this.manager_NewCallerId);
        ActvDialer.manager.NewState += new NewStateEventHandler(this.manager_NewState);
        ActvDialer.manager.Hangup += new HangupEventHandler(this.manager_Hangup);
        ActvDialer.manager.UnhandledEvent += new ManagerEventHandler(this.manager_UnhandledEvent);
        ActvDialer.manager.QueueMember += new QueueMemberEventHandler(this.manager_QueueMember);
        ActvDialer.manager.QueueMemberAdded += new QueueMemberAddedEventHandler(this.manager_QueueMemberAdded);
        ActvDialer.manager.QueueMemberRemoved += new QueueMemberRemovedEventHandler(this.manager_QueueMemberRemoved);
        ActvDialer.manager.QueueMemberPaused += new QueueMemberPausedEventHandler(this.manager_QueueMemberPaused);
        ActvDialer.manager.Unlink += new UnlinkEventHandler(this.manager_Unlink);
        ActvDialer.manager.OriginateResponse += new OriginateResponseEventHandler(this.manager_OriginateResponse);
        ActvDialer.manager.FireAllEvents = true;
        ActvDialer.manager.Login();
        return true;
      }
      catch
      {
        return false;
      }
    }

    private void manager_OriginateResponse(object sender, OriginateResponseEvent e)
    {
      try
      {
        if (e.UniqueId == null)
          return;
        for (int index = 0; index < this.mAgentCount; ++index)
        {
          if (this.Trunks[index].AstID == e.UniqueId)
            this.Trunks[index].OriginateState = e.Reason.ToString();
        }
      }
      catch
      {
      }
    }

    private void manager_QueueMemberPaused(object sender, QueueMemberPausedEvent e)
    {
      try
      {
        if (e.Queue != this.mQueue)
          return;
        for (int index = 0; index < this.mAgentCount; ++index)
        {
          if (this.Agents[index].Exten == e.MemberName)
          {
            this.Agents[index].QueuePaused = e.Paused;
            break;
          }
        }
      }
      catch
      {
      }
    }

    private void manager_QueueMemberRemoved(object sender, QueueMemberRemovedEvent e)
    {
      try
      {
        if (e.Queue != this.mQueue)
          return;
        for (int index = 0; index < this.mAgentCount; ++index)
        {
          if (this.Agents[index].Exten == e.MemberName)
          {
            this.Agents[index].NemberQueue = "";
            this.Agents[index].QueuePaused = true;
            break;
          }
        }
      }
      catch
      {
      }
    }

    private void manager_QueueMemberAdded(object sender, QueueMemberAddedEvent e)
    {
      try
      {
        if (e.Queue != this.mQueue)
          return;
        for (int index = 0; index < this.mAgentCount; ++index)
        {
          if (this.Agents[index].Exten == e.MemberName)
          {
            this.Agents[index].NemberQueue = e.Queue;
            this.Agents[index].QueuePaused = e.Paused;
            break;
          }
        }
      }
      catch
      {
      }
    }

    private int GetAgent(string Agent)
    {
      try
      {
        for (int index = 0; index < this.mAgentCount; ++index)
        {
          if (this.Agents[index].Exten == Agent)
            return index;
        }
        return -1;
      }
      catch
      {
        return -1;
      }
    }

    private void manager_Unlink(object sender, UnlinkEvent e)
    {
      try
      {
        for (int index = 0; index < this.Trunks.Length; ++index)
        {
          if (this.Trunks[index].Activate && this.Trunks[index].AstID == e.UniqueId1)
          {
            string channel2 = e.Channel2;
            int startIndex = channel2.IndexOf("SIP/");
            string str1 = channel2.Remove(startIndex, 4);
            string str2 = str1.Substring(0, str1.IndexOf("-"));
            this.Trunks[index].Agent = str2;
            int agent = this.GetAgent(str2);
            if (agent != -1)
              this.Agents[agent].Speak = false;
            this.AgentLock(this.mQueue, str2, false);
            break;
          }
        }
      }
      catch
      {
      }
    }

    private void manager_QueueMember(object sender, QueueMemberEvent e)
    {
      try
      {
        if (e.Queue != this.mQueue || e.Name == null)
          return;
        for (int index = 0; index < this.mAgentCount; ++index)
        {
          if (this.Agents[index].Exten == e.Name)
          {
            this.Agents[index].NemberQueue = e.Queue;
            this.Agents[index].QueuePaused = e.Paused;
            break;
          }
        }
      }
      catch
      {
      }
    }

    private void manager_UnhandledEvent(object sender, ManagerEvent e)
    {
      if (!e.GetType().Name.StartsWith("Bridge"))
        return;
      BridgeEvent bridgeEvent = (BridgeEvent) e;
      if (bridgeEvent.UniqueId1 == null)
        return;
      for (int index = 0; index < this.Trunks.Length; ++index)
      {
        if (this.Trunks[index].Activate && this.Trunks[index].AstID == bridgeEvent.UniqueId1)
        {
          this.Trunks[index].BridgeState = ((object) bridgeEvent.BridgeState).ToString();
          this.Trunks[index].BridgeTime = DateTime.Now;
          string channel2 = bridgeEvent.Channel2;
          int startIndex = channel2.IndexOf("SIP/");
          string str1 = channel2.Remove(startIndex, 4);
          string str2 = str1.Substring(0, str1.IndexOf("-"));
          this.Trunks[index].Agent = str2;
          int agent = this.GetAgent(str2);
          if (agent != -1)
            this.Agents[agent].Speak = true;
          this.AgentLock(this.mQueue, str2, true);
          break;
        }
      }
    }

    private void manager_Hangup(object sender, HangupEvent e)
    {
      try
      {
        if (e.UniqueId == null)
          return;
        for (int index = 0; index < this.Trunks.Length; ++index)
        {
          if (this.Trunks[index].Activate && this.Trunks[index].AstID == e.UniqueId)
          {
            this.Trunks[index].HangupState = e.Cause.ToString();
            this.Trunks[index].EndTime = DateTime.Now;
            this.Trunks[index].Activate = false;
            this.ProcessHangup(this.Trunks[index].DBID, e.Cause);
            break;
          }
        }
      }
      catch
      {
      }
    }

    private void manager_NewState(object sender, NewStateEvent e)
    {
      try
      {
        if (e.UniqueId == null)
          return;
        for (int index = 0; index < this.Trunks.Length; ++index)
        {
          if (this.Trunks[index].Activate && this.Trunks[index].Phone == e.CallerIdNum && this.Trunks[index].AstID == e.UniqueId)
          {
            this.Trunks[index].ChanState = e.State;
            break;
          }
        }
      }
      catch
      {
      }
    }

    private void manager_NewCallerId(object sender, NewCallerIdEvent e)
    {
      try
      {
        if (e.CallerIdNum == null)
          return;
        for (int index = 0; index < this.Trunks.Length; ++index)
        {
          if (this.Trunks[index].Activate && this.Trunks[index].Phone == e.CallerIdNum)
          {
            this.Trunks[index].AstID = e.UniqueId;
            this.Trunks[index].ChanState = "0";
            this.Trunks[index].HangupState = "";
            this.Trunks[index].OriginateState = "";
            this.Trunks[index].BridgeState = "";
            break;
          }
        }
      }
      catch
      {
      }
    }

    protected void ReadFreeAgent()
    {
      try
      {
        int num1 = 0;
        for (int index = 0; index < this.mAgentCount; ++index)
        {
          this.Agents[index].Status = this.AgentState(this.Agents[index].Exten);
          this.AgentQueueStatus(this.Agents[index].Name);
          if (this.Agents[index].Status == "0" && this.Agents[index].NemberQueue == this.mQueue && (!this.Agents[index].QueuePaused && !this.Agents[index].Speak))
          {
            int num2;
            num1 = num2 = num1 + 1;
          }
        }
      }
      catch
      {
      }
    }

    public bool AgentLock(string Queue, string Exten, bool Lock)
    {
      try
      {
        QueuePauseAction queuePauseAction = new QueuePauseAction();
        queuePauseAction.ActionId = "11";
        queuePauseAction.Queue = Queue;
        queuePauseAction.Interface = "SIP/" + Exten;
        queuePauseAction.Paused = Lock;
        return ActvDialer.manager.SendAction((ManagerAction) queuePauseAction) != null;
      }
      catch
      {
        return false;
      }
    }

    public ActvDialer.TAgent[] AgentStatus()
    {
      try
      {
        this.ReadFreeAgent();
        return this.Agents;
      }
      catch
      {
        return (ActvDialer.TAgent[]) null;
      }
    }

    public ActvDialer.TTrunk[] TrunkStatus()
    {
      try
      {
        return this.Trunks;
      }
      catch
      {
        return (ActvDialer.TTrunk[]) null;
      }
    }

    protected string AgentState(string Exten)
    {
      try
      {
        ExtensionStateAction extensionStateAction = new ExtensionStateAction();
        extensionStateAction.ActionId = "9";
        extensionStateAction.Exten = Exten;
        extensionStateAction.Context = "from-internal";
        ManagerResponse managerResponse = ActvDialer.manager.SendAction((ManagerAction) extensionStateAction);
        Thread.Sleep(5);
        Application.DoEvents();
        if (managerResponse != null)
          return ((ExtensionStateResponse) managerResponse).Status.ToString();
        else
          return "";
      }
      catch
      {
        return "";
      }
    }

    protected void AgentQueueStatus(string Exten)
    {
      QueueStatusAction queueStatusAction = new QueueStatusAction();
      queueStatusAction.ActionId = "12";
      queueStatusAction.Member = Exten;
      queueStatusAction.Queue = this.mQueue;
      ActvDialer.manager.SendAction((ManagerAction) queueStatusAction);
    }

    public void AgentRemoveQueue(string Exten)
    {
      try
      {
        QueueRemoveAction queueRemoveAction = new QueueRemoveAction();
        queueRemoveAction.ActionId = "13";
        queueRemoveAction.Queue = this.mQueue;
        queueRemoveAction.Interface = "SIP/" + Exten;
        ActvDialer.manager.SendAction((ManagerAction) queueRemoveAction);
      }
      catch
      {
      }
    }

    public void AgentAddQueue(string Exten)
    {
      try
      {
        QueueAddAction queueAddAction = new QueueAddAction();
        queueAddAction.ActionId = "14";
        queueAddAction.Queue = this.mQueue;
        queueAddAction.Interface = "SIP/" + Exten;
        queueAddAction.MemberName = Exten;
        queueAddAction.Penalty = 0;
        queueAddAction.Paused = true;
        ActvDialer.manager.SendAction((ManagerAction) queueAddAction);
      }
      catch
      {
      }
    }

    public void ClearTrunk(int i)
    {
      try
      {
        this.Trunks[i].Activate = false;
        this.Trunks[i].Agent = "";
        this.Trunks[i].AstID = "";
        this.Trunks[i].BridgeState = "";
        this.Trunks[i].ChanState = "";
        this.Trunks[i].DBID = "";
        this.Trunks[i].HangupState = "";
        this.Trunks[i].OriginateState = "";
        this.Trunks[i].Phone = "";
        this.Trunks[i].StartTime = new DateTime(0L);
        this.Trunks[i].EndTime = new DateTime(0L);
        this.Trunks[i].CallerID = "";
        this.Trunks[i].BridgeTime = new DateTime(0L);
      }
      catch
      {
      }
    }

    public bool NewCallAgent(string Queue, string Phone, int DataID, string SipTrunk)
    {
      int index1 = -1;
      try
      {
        for (int index2 = 0; index2 < this.Trunks.Length; ++index2)
        {
          if (!this.Trunks[index2].Activate)
          {
            index1 = index2;
            break;
          }
        }
        if (index1 == -1)
        {
          int length = this.Trunks.Length;
          Array.Resize<ActvDialer.TTrunk>(ref this.Trunks, length + 10);
          for (int i = length; i < length + 10; ++i)
            this.ClearTrunk(i);
          index1 = length;
        }
        this.Trunks[index1].Activate = true;
        this.Trunks[index1].Agent = "";
        this.Trunks[index1].AstID = "";
        this.Trunks[index1].BridgeState = "";
        this.Trunks[index1].ChanState = "";
        this.Trunks[index1].DBID = DataID.ToString();
        this.Trunks[index1].HangupState = "";
        this.Trunks[index1].OriginateState = "";
        this.Trunks[index1].Phone = Phone;
        this.Trunks[index1].StartTime = DateTime.Now;
        this.Trunks[index1].EndTime = DateTime.Now;
        this.Trunks[index1].CallerID = this.Trunks[index1].DBID + "<" + this.Trunks[index1].Phone + ">";
        OriginateAction originateAction = new OriginateAction();
        originateAction.ActionId = "0";
        originateAction.Channel = "SIP/" + SipTrunk + "/" + this.Trunks[index1].Phone;
        originateAction.Context = "from-internal";
        originateAction.CallerId = this.Trunks[index1].CallerID;
        originateAction.Exten = Queue;
        originateAction.Priority = 1;
        originateAction.Async = true;
        originateAction.SetVariable("_SIPADDHEADER55", "Alert-Info: Ring Answer");
        originateAction.Timeout = 15000;
        ActvDialer.manager.SendAction((ManagerAction) originateAction, 15000);
        return true;
      }
      catch
      {
        return false;
      }
    }

    public delegate void DlgHangup(string DBID, int Cause);

    public struct TTrunk
    {
      public bool Activate;
      public string AstID;
      public string DBID;
      public string Phone;
      public string CallerID;
      public string Agent;
      public string ChanState;
      public string HangupState;
      public string OriginateState;
      public string BridgeState;
      public DateTime BridgeTime;
      public DateTime StartTime;
      public DateTime EndTime;
    }

    public struct TAgent
    {
      public string Exten;
      public string Name;
      public string Status;
      public string CampainQueue;
      public string NemberQueue;
      public bool QueuePaused;
      public string BridgeUniqID;
      public bool Speak;
    }
  }
}
