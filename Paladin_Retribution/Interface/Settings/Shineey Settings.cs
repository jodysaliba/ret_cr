using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Reflection;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using Styx.Helpers;
using Styx;

using DefaultValue = Styx.Helpers.DefaultValueAttribute;

namespace Paladin_Retribution.Interface.Settings
{
    class Shineey_Settings : Styx.Helpers.Settings
    {
        private static Shineey_Settings _instance;

        public static readonly Shineey_Settings Instance = new Shineey_Settings();
        public Shineey_Settings()
            : base(Path.Combine(CharacterSettingsPath, "ShineeyPaladinSettings.xml"))
        {
            _instance = this;
        }

        private Hotkey_Settings _hotkeySettings;
        internal Hotkey_Settings Hotkeys() { return _hotkeySettings ?? (_hotkeySettings = new Hotkey_Settings()); }

        public static string CharacterSettingsPath
        {
            get
            {
                string settingsDirectory = Path.Combine(Styx.Common.Utilities.AssemblyDirectory, "Settings");
                return Path.Combine(Path.Combine(settingsDirectory, StyxWoW.Me.RealmName), StyxWoW.Me.Name);
            }
        }

        public static void Initialize()
        {
            if (_instance == null)
                _instance = new Shineey_Settings();
        }

        
    }
}
