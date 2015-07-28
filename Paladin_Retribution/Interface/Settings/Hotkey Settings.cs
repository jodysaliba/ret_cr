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

using Styx.WoWInternals.WoWObjects;
using Styx.Helpers;
using Styx;

using DefaultValue = Styx.Helpers.DefaultValueAttribute;

namespace Paladin_Retribution.Interface.Settings
{
    class Hotkey_Settings : Styx.Helpers.Settings
    {
        public Hotkey_Settings()
            : base(Path.Combine(Shineey_Settings.CharacterSettingsPath, "ShineeyPaladinSettings.Hotkeys.xml"))
        {

        }

        [Setting]
        [Browsable(false)]
        [DefaultValue(false)]
        public bool SavedToFile { get; set; }

        [Setting]
        [DefaultValue(Keys.Pause)]
        [Category("Hotkeys")]
        [DisplayName("Key - Pause/Play")]
        [Description("Enables/Disables combat abilities")]
        public Keys keyPause { get; set; }

        [Setting]
        [DefaultValue(Keys.Insert)]
        [Category("Hotkeys")]
        [DisplayName("Key - AoE")]
        [Description("Enables/Disables AoE rotation")]
        public Keys keyAOE { get; set; }

        [Setting]
        [DefaultValue(Keys.X)]
        [Category("Hotkeys")]
        [DisplayName("Key - Cooldowns")]
        [Description("Enables/Disables cooldown abilities")]
        public Keys keyCooldowns { get; set; }

        [Setting]
        [DefaultValue(Keys.C | Keys.Shift)]
        [Category("Hotkeys")]
        [DisplayName("Key - Seal Switching")]
        [Description("Switches seals between Truth and Righteousness")]
        public Keys keyRighteousness { get; set; }
    }
}
