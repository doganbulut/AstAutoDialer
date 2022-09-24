// Type: AstAutoDialer.Program
// Assembly: AstAutoDialer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B1750608-7C33-45D3-A128-4749BC17D73C
// Assembly location: I:\TLCSilme\AstDialer\AstAutoDialer.exe

using System;
using System.Windows.Forms;

namespace AstAutoDialer
{
  internal static class Program
  {
    [STAThread]
    private static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run((Form) new FrmMain());
    }
  }
}
