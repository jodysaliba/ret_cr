using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

#region [Method] - Class Redundancy
using Paladin_Retribution;
using Paladin_Retribution.Interface.Settings;
using HkM = Paladin_Retribution.Core.Managers.Hotkey_Manager;
#endregion

namespace Paladin_Retribution.Interface.GUI
{
    public partial class ConfigForm : Form
    {
        public ConfigForm()
        {
            InitializeComponent();
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            l_version.Text = "Version: " + Main.version.ToString();

            pg_Hotkeys.SelectedObject = Shineey_Settings.Instance.Hotkeys();
        }

        private void b_Save_Click(object sender, EventArgs e)
        {
            // save the settings
            ((Styx.Helpers.Settings)pg_Hotkeys.SelectedObject).Save();
            
            // update hotkeys
            HkM.update();

            Close();
        }
    }
}
