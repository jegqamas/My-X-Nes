//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyNes.Nes.Output.Video {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    public sealed partial class VideoModeSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static VideoModeSettings defaultInstance = ((VideoModeSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new VideoModeSettings())));
        
        public static VideoModeSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int SlimDX_ResMode {
            get {
                return ((int)(this["SlimDX_ResMode"]));
            }
            set {
                this["SlimDX_ResMode"] = value;
            }
        }
    }
}
