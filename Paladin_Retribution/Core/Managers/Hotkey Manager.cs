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

        #region [Method] - Hotkey Registration
        // AOE = X
        // CDs = Insert
        // Pause = Pause
        public static void registerHotKeys()
        {
            if (keysRegistered)
                return;
            HotkeysManager.Register("aoeOn", Keys.X, ModifierKeys.NoRepeat, ret =>
            {
                aoeOn = !aoeOn;
                //Lua.DoString(aoeOn ? @"print('AoE Mode: \124cFF15E61C Enabled!')" : @"print('AoE Mode: \124cFFE61515 Disabled!')");
                if (!aoeOn)
                {
                    //LuaDoString("print('Execution Paused!')");
                    StyxWoW.Overlay.AddToast(() =>
                    {
                        return string.Format("AOE Disabled!");
                    },
                    TimeSpan.FromSeconds(2),
                    System.Windows.Media.Colors.Lime,
                    System.Windows.Media.Colors.Blue,
                    new System.Windows.Media.FontFamily("Consolas"));
                }
                else
                {
                    //LuaDoString("print('Execution Resumed!')");
                    StyxWoW.Overlay.AddToast(() =>
                    {
                        return string.Format("AOE Enabled!");
                    },
                    TimeSpan.FromSeconds(2),
                    System.Windows.Media.Colors.Lime,
                    System.Windows.Media.Colors.Blue,
                    new System.Windows.Media.FontFamily("Consolas"));
                }
            });
            HotkeysManager.Register("cooldownsOn", Keys.Insert, ModifierKeys.NoRepeat, ret =>
            {
                cooldownsOn = !cooldownsOn;
                //Lua.DoString(cooldownsOn ? @"print('Cooldowns: \124cFF15E61C Enabled!')" : @"print('Cooldowns: \124cFFE61515 Disabled!')");
                if (!cooldownsOn)
                {
                    StyxWoW.Overlay.AddToast(() =>
                    {
                        return string.Format("CDs Disabled!");
                    },
                    TimeSpan.FromSeconds(2),
                    System.Windows.Media.Colors.Lime,
                    System.Windows.Media.Colors.Blue,
                    new System.Windows.Media.FontFamily("Consolas"));
                }
                else
                {
                    StyxWoW.Overlay.AddToast(() =>
                    {
                        return string.Format("CDs Enabled!");
                    },
                    TimeSpan.FromSeconds(2),
                    System.Windows.Media.Colors.Lime,
                    System.Windows.Media.Colors.Blue,
                    new System.Windows.Media.FontFamily("Consolas"));
                }
            });
            HotkeysManager.Register("manualOn", Keys.Pause, ModifierKeys.NoRepeat, ret =>
            {
                manualOn = !manualOn;
                //Lua.DoString(manualOn ? @"print('Manual Mode: \124cFF15E61C Enabled!')" : @"print('Manual Mode: \124cFFE61515 Disabled!')");
                if (manualOn)
                {
                    //LuaDoString("print('Execution Paused!')");
                    StyxWoW.Overlay.AddToast(() =>
                    {
                        return string.Format("Execution Paused!");
                    },
                    TimeSpan.FromSeconds(2),
                    System.Windows.Media.Colors.Lime,
                    System.Windows.Media.Colors.Blue,
                    new System.Windows.Media.FontFamily("Consolas"));

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
                    System.Windows.Media.Colors.Lime,
                    System.Windows.Media.Colors.Blue,
                    new System.Windows.Media.FontFamily("Consolas"));

                    // Kick it back into overdrive!
                    TreeRoot.TicksPerSecond = 8;
                }
            });
            keysRegistered = true;
            //Lua.DoString(@"print('Hotkeys: \124cFF15E61C Registered!')");
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
            //Lua.DoString(@"print('Hotkeys: \124cFFE61515 Removed!')");
            Logging.Write(Colors.OrangeRed, "[Shineey] Hotkeys: Removed!");
        }  
        #endregion
    }
}
