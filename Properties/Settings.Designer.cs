// Type: AstAutoDialer.Properties.Settings
// Assembly: AstAutoDialer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B1750608-7C33-45D3-A128-4749BC17D73C
// Assembly location: I:\TLCSilme\AstDialer\AstAutoDialer.exe

using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AstAutoDialer.Properties
{
  [CompilerGenerated]
  [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0")]
  internal sealed class Settings : ApplicationSettingsBase
  {
    private static Settings defaultInstance = (Settings) SettingsBase.Synchronized((SettingsBase) new Settings());

    public static Settings Default
    {
      get
      {
        return Settings.defaultInstance;
      }
    }

    [ApplicationScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("192.168.10.139")]
    public string astserver
    {
      get
      {
        return (string) this["astserver"];
      }
    }

    [DebuggerNonUserCode]
    [DefaultSettingValue("root")]
    [ApplicationScopedSetting]
    public string astuser
    {
      get
      {
        return (string) this["astuser"];
      }
    }

    [ApplicationScopedSetting]
    [DefaultSettingValue("Aa123456")]
    [DebuggerNonUserCode]
    public string astpass
    {
      get
      {
        return (string) this["astpass"];
      }
    }

    [DefaultSettingValue("Data Source=192.168.10.200; Initial Catalog=TLCOUT; Persist Security Info=True; User ID=sa;Password=Aa123456")]
    [ApplicationScopedSetting]
    [DebuggerNonUserCode]
    public string SQLConnectionString
    {
      get
      {
        return (string) this["SQLConnectionString"];
      }
    }

    static Settings()
    {
    }
  }
}
