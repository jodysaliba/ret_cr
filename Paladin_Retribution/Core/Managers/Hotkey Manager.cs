using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.WoWInternals;
using Styx.TreeSharp;

namespace Paladin_Retribution.Core.Managers
{
    class Hotkey_Manager
    {
        public static bool aoeOn { get; set; }
        public static bool cooldownsOn { get; set; }
        public static bool manualOn { get; set; }
        public static bool keysRegistered { get; set; }
        public static bool righteousness { get; set; }

        #region [Method] - Hotkey Registration
        // AOE = X
        // CDs = Insert
        // Pause = Pause
        public static void registerHotKeys()
        {
            if (keysRegistered)
                return;
            HotkeysManager.Register("aoeOn", Keys.Insert, ModifierKeys.NoRepeat, ret =>
            {
                aoeOn = !aoeOn;
                if (!aoeOn)
                {
                    //LuaDoString("print('Execution Paused!')");
                    StyxWoW.Overlay.AddToast(() =>
                    {
                        return string.Format("AOE Disabled!");
                    },
                    TimeSpan.FromSeconds(2),
                    System.Windows.Media.Colors.OrangeRed,
                    System.Windows.Media.Colors.SeaShell,
                    new System.Windows.Media.FontFamily("Consolas"),
                    System.Windows.FontWeights.Normal,
                    32);
                }
                else
                {
                    //LuaDoString("print('Execution Resumed!')");
                    StyxWoW.Overlay.AddToast(() =>
                    {
                        return string.Format("AOE Enabled!");
                    },
                    TimeSpan.FromSeconds(2),
                    System.Windows.Media.Colors.DarkGreen,
                    System.Windows.Media.Colors.SeaShell,
                    new System.Windows.Media.FontFamily("Consolas"),
                    System.Windows.FontWeights.Normal,
                    32);
                }
            });
            HotkeysManager.Register("cooldownsOn", Keys.X, ModifierKeys.NoRepeat, ret =>
            {
                cooldownsOn = !cooldownsOn;
                if (!cooldownsOn)
                {
                    StyxWoW.Overlay.AddToast(() =>
                    {
                        return string.Format("CDs Disabled!");
                    },
                    TimeSpan.FromSeconds(2),
                    System.Windows.Media.Colors.OrangeRed,
                    System.Windows.Media.Colors.SeaShell,
                    new System.Windows.Media.FontFamily("Consolas"),
                    System.Windows.FontWeights.Normal,
                    32);
                }
                else
                {
                    StyxWoW.Overlay.AddToast(() =>
                    {
                        return string.Format("CDs Enabled!");
                    },
                    TimeSpan.FromSeconds(2),
                    System.Windows.Media.Colors.DarkGreen,
                    System.Windows.Media.Colors.SeaShell,
                    new System.Windows.Media.FontFamily("Consolas"),
                    System.Windows.FontWeights.Normal,
                    32);
                }
            });
            HotkeysManager.Register("manualOn", Keys.Pause, ModifierKeys.NoRepeat, ret =>
            {
                manualOn = !manualOn;
                if (manualOn)
                {
                    //LuaDoString("print('Execution Paused!')");
                    StyxWoW.Overlay.AddToast(() =>
                    {
                        return string.Format("Execution Paused!");
                    },
                    TimeSpan.FromSeconds(2),
                    System.Windows.Media.Colors.OrangeRed,
                    System.Windows.Media.Colors.SeaShell,
                    new System.Windows.Media.FontFamily("Consolas"),
                    System.Windows.FontWeights.Normal,
                    32);

                    // Make the bot use less resources while paused.
                    TreeRoot.TicksPerSecond = 5;
                }
                else
                {
                    //LuaDoString("print('Execution Resumed!')");
                    StyxWoW.Overlay.AddToast(() =>
                    {
                        return string.Format("Execution Resumed!");
                    },
                    TimeSpan.FromSeconds(2),
                    System.Windows.Media.Colors.DarkGreen,
                    System.Windows.Media.Colors.SeaShell,
                    new System.Windows.Media.FontFamily("Consolas"),
                    System.Windows.FontWeights.Normal,
                    32);

                    // Kick it back into overdrive!
                    TreeRoot.TicksPerSecond = 8;
                }
            });
            HotkeysManager.Register("righteousness", Keys.C, ModifierKeys.Shift, ret =>
            {
                righteousness = !righteousness;
                if (!righteousness)
                {
                    //LuaDoString("print('Execution Paused!')");
                    StyxWoW.Overlay.AddToast(() =>
                    {
                        return string.Format("Seal of Truth!");
                    },
                    TimeSpan.FromSeconds(2),
                    System.Windows.Media.Colors.DarkOrange,
                    System.Windows.Media.Colors.SeaShell,
                    new System.Windows.Media.FontFamily("Consolas"),
                    System.Windows.FontWeights.Normal,
                    32);
                }
                else
                {
                    //LuaDoString("print('Execution Resumed!')");
                    StyxWoW.Overlay.AddToast(() =>
                    {
                        return string.Format("Seal of Righteousness!");
                    },
                    TimeSpan.FromSeconds(2),
                    System.Windows.Media.Colors.Navy,
                    System.Windows.Media.Colors.SeaShell,
                    new System.Windows.Media.FontFamily("Consolas"),
                    System.Windows.FontWeights.Normal,
                    32);
                }
            });
            keysRegistered = true;
            Logging.Write(Colors.Aqua, "[Shineey] Hotkeys: Registered!");
        }  
        #endregion

        #region [Method] - Hotkey Removal
        public static void removeHotkeys()
        {
            if (!keysRegistered)
                return;
            HotkeysManager.Unregister("aoeOn");
            HotkeysManager.Unregister("cooldownsOn");
            HotkeysManager.Unregister("manualOn");
            aoeOn = false;
            cooldownsOn = false;
            manualOn = false;
            keysRegistered = false;
            Logging.Write(Colors.OrangeRed, "[Shineey] Hotkeys: Removed!");
        }  
        #endregion
    }
}
