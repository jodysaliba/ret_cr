//Created: 07/11/2015 by Shineey
//Professions:
//Talents:
//Glyphs:
//Revision: 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Styx;
using Styx.Common;
using Styx.CommonBot.Routines;
using Styx.WoWInternals.WoWObjects;
using Styx.TreeSharp;
using Paladin_Retribution.Rotation;

#region [Method] - Class Redundancy
using Paladin_Retribution.Interface.GUI;
using HKM = Paladin_Retribution.Core.Managers.Hotkey_Manager;
using TM = Paladin_Retribution.Core.Managers.Talent_Manager;
using R = Paladin_Retribution.Rotation.Rotation;
using U = Paladin_Retribution.Core.Unit;
using L = Paladin_Retribution.Core.Utilities.Log;
using Rot = Paladin_Retribution.Rotation.Rotation;
#endregion  

namespace Paladin_Retribution
{
    public class Main : CombatRoutine
    {
        private static readonly Version version = new Version(1, 1, 3);
        public override string Name { get { return "Shineey Paladin v" + version; } }
        public override WoWClass Class { get { return WoWClass.Paladin; } }
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region [Method] - Implementations
        private Composite _preCombatBehavior, _combatBuffBehavior, _combatBehavior;
        public override Composite PreCombatBuffBehavior{ get { return _preCombatBehavior ?? (_preCombatBehavior = R.preCombatBuffing()); } }
        public override Composite CombatBuffBehavior { get { return _combatBuffBehavior ?? (_combatBuffBehavior = R.combatBuffing()); } }
        public override Composite CombatBehavior { get { return _combatBehavior ?? (_combatBehavior = R.rotationSelector()); } }

            #region [Method] - Hidden Overrides
            public override void Initialize()
            {
                Logging.Write(Colors.OrangeRed, "[Shineey] ----------------------------------------");
                Logging.Write(Colors.OrangeRed, "[Shineey] Hello {0}!", Me.Name);
                Logging.Write(Colors.White, "");
                Logging.Write(Colors.OrangeRed, "[Shineey] For optimal performance please use: Enyo");
                Logging.Write(Colors.White, "");
                Logging.Write(Colors.OrangeRed, "[Shineey] Current Version:");
                Logging.Write(Colors.OrangeRed, "[Shineey] -- Retribution v" + version + " --");
                Logging.Write(Colors.OrangeRed, "[Shineey] -- by Shineey --");
                Logging.Write(Colors.OrangeRed, "[Shineey] -- A Paladin Combat Routine --");
                Logging.Write(Colors.OrangeRed, "[Shineey] ----------------------------------------");

                HKM.registerHotKeys();
                TM.registerTalents();

                //Logging.Write(Colors.White, "[Shineey] num_t17: " + Rot.t17_equipped);
                //Logging.Write(Colors.White, "[Shineey] num_t18: " + Rot.t18_equipped);
                //Logging.Write(Colors.White, "[Shineey] t_Seraphim: " + TM.t_Seraphim);
                //Logging.Write(Colors.White, "[Shineey] t_FinalVerdict: " + TM.t_FinalVerdict);
            }
            public override bool WantButton { get { return true; } }

            private ConfigForm _configForm;
            public override void OnButtonPress() 
            {
                Logging.Write(Colors.OrangeRed, "Coming soon!");
                if (_configForm == null || _configForm.IsDisposed || _configForm.Disposing)
                {
                    _configForm = new ConfigForm();
                }

                _configForm.Show();
            }

            public override void ShutDown() { HKM.removeHotkeys(); }
            #endregion

            #region [Method] - Pulse
            static int pulsePhase = 0;
            public override void Pulse()
            {
                pulsePhase++;
                if (!StyxWoW.IsInWorld || Me == null || !Me.IsValid || !Me.IsAlive)
                    return;
                if (!Me.Combat || Me.Mounted)
                    return;
                if (pulsePhase % 2 == 0)
                    U.enemyAnnex(8f);
                else
                    U.Cache();
                //Logging.Write(Colors.OrangeRed, "num_t17: " + Rot.num_t17);
                //Logging.Write(Colors.OrangeRed, "num_t18: " + Rot.num_t18);
                //Logging.Write(Colors.OrangeRed, "Rot.enemies: " + Rot.enemies);
                //Logging.Write(Colors.OrangeRed, "U.nearbyEnemies: " + U.activeEnemies(Me.Location, 40f).Count());
                //Logging.Write(Colors.OrangeRed, "U.myEnemies: " + U.myEnemies);
                //Logging.Write(Colors.OrangeRed, "U.singEnemyCount: " + U.singEnemyCount.Count());
                //Logging.Write(Colors.OrangeRed, "enemies: " + Rot.activeEn);
            }
            #endregion

        #endregion
    }
}


