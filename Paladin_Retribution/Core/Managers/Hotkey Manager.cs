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

#region [Method] - Class Redundancy
using Paladin_Retribution.Interface.Settings;
using L = Paladin_Retribution.Core.Utilities.Log;
#endregion

namespace Paladin_Retribution.Core.Managers
{
    class Hotkey_Manager
    {
        public static bool aoeOn { get; set; }
        public static bool cooldownsOn { get; set; }
        public static bool manualOn { get; set; }
        public static bool keysRegistered { get; set; }
        public static bool righteousness { get; set; }

        private static Hotkey_Settings Hotkey_Settings { get { return Shineey_Settings.Instance.Hotkeys(); } }

        #region [Method] - Hotkey Registration
        // AOE = X
        // CDs = Insert
        // Pause = Pause
        // Seals = Shift + C
        public static void registerHotKeys()
        {
            initKeyStates();

            if (keysRegistered)
                return;
            
            start(true);
            
            initKeyStates();
            Logging.Write(Colors.Aqua, "[Shineey] Hotkeys: Registered!");
        }

        private static void start(bool needReset = false)
        {
            if (needReset)
                initKeyStates();

            keysRegistered = true;

            // hook the hotkeys for the appropriate WoW window
            HotkeysManager.Initialize(StyxWoW.Memory.Process.MainWindowHandle);

            // define hotkeys
            RegisterHotkeyAssignment("manualOn", Hotkey_Settings.keyPause, (hk) => { Manual_Toggle(); });
            RegisterHotkeyAssignment("aoeOn", Hotkey_Settings.keyAOE, (hk) => { AoE_Toggle(); });
            RegisterHotkeyAssignment("cooldownsOn", Hotkey_Settings.keyCooldowns, (hk) => { CDs_Toggle(); });
            RegisterHotkeyAssignment("righteousness", Hotkey_Settings.keyRighteousness, (hk) => { Seal_Toggle(); });
            Logging.Write(Colors.Aqua, "[Shineey] Hotkeys: Updated!");
        }

        public static void update()
        {
            removeHotkeys();
            start();
        }
        
        private static void initKeyStates()
        {
            // reset these values so we begin at same state every Start
            aoeOn = true;
            cooldownsOn = true;
            manualOn = false;
            righteousness = true;
        }

        private static void RegisterHotkeyAssignment(string name, Keys key, Action<Hotkey> callback)
        {
            Keys keyCode = key & Keys.KeyCode;
            ModifierKeys mods = ModifierKeys.NoRepeat;

            if ((key & Keys.Shift) != 0)
                mods |= ModifierKeys.Shift;
            if ((key & Keys.Alt) != 0)
                mods |= ModifierKeys.Alt;
            if ((key & Keys.Control) != 0)
                mods |= ModifierKeys.Control;
            HotkeysManager.Register(name, keyCode, mods, callback);
        }

        private static bool AoE_Toggle()
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
            return aoeOn;
        }

        private static bool CDs_Toggle()
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
            return cooldownsOn;
        }

        private static bool Manual_Toggle()
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
            return manualOn;
        }

        private static bool Seal_Toggle()
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
            return righteousness;
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
            HotkeysManager.Unregister("righteousness");
            aoeOn = false;
            cooldownsOn = false;
            manualOn = false;
            righteousness = false;

            keysRegistered = false;
            Logging.Write(Colors.Aqua, "[Shineey] Hotkeys: Removed!");
        }  
        #endregion

        
    }
}
