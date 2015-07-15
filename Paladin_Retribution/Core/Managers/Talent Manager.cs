using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Threading.Tasks;
using Styx;
using Styx.Common;
using Styx.Common.Helpers;
using Styx.CommonBot;
using Styx.CommonBot.Routines;
using Styx.CommonBot.CharacterManagement;
using Styx.WoWInternals;

using L = Paladin_Retribution.Core.Utilities.Log;

namespace Paladin_Retribution.Core.Managers
{
    class Talent_Manager
    {
        static TalentPlacementSet set;

        //public static List<Talent> Talents { get; private set; }

        private static bool[,] talents = new bool[7, 3];

        public static bool talentsRegistered { get; set; }

        #region [Talent Variables]
        // Tier 1
        public static bool t_SpeedOfLight { get; set; }
        public static bool t_LongArmOfTheLaw { get; set; }
        public static bool t_PursuitOfJustice { get; set; }
        // Tier 2
        public static bool t_FistOfJustice { get; set; }
        public static bool t_Repentence { get; set; }
        public static bool t_BlindingLight { get; set; }
        // Tier 3
        public static bool t_SelflessHealer { get; set; }
        public static bool t_EternalFlame { get; set; }
        public static bool t_SacredShield { get; set; }
        // Tier 4
        public static bool t_HandOfPurity { get; set; }
        public static bool t_UnbreakableSpirit { get; set; }
        public static bool t_Clemency { get; set; }
        // Tier 5
        public static bool t_HolyAvenger { get; set; }
        public static bool t_SanctifiedWrath { get; set; }
        public static bool t_DivinePurpose { get; set; }
        // Tier 6
        public static bool t_HolyPrism { get; set; }
        public static bool t_LightsHammer { get; set; }
        public static bool t_ExecutionSentence { get; set; }
        // Tier 7
        public static bool t_EmpoweredSeals { get; set; }
        public static bool t_Seraphim { get; set; }
        public static bool t_FinalVerdict { get; set; }
        #endregion

        #region [Method] Talent Initialization
        public static void registerTalents()
        {
            //Talents = new List<Talent>();

            Lua.Events.AttachEvent("PLAYER_TALENT_UPDATE", loadTalents);
            Lua.Events.AttachEvent("CONFIRM_TALENT_WIPE", loadTalents);
            Lua.Events.AttachEvent("ACTIVE_TALENT_GROUP_CHANGED", loadTalents);
            Lua.Events.AttachEvent("PLAYER_SPECIALIZATION_CHANGED", loadTalents);
            Lua.Events.AttachEvent("LEARNED_SPELL_IN_TAB", loadTalents);
            loadTalents();
        }
        #endregion

        #region [Method] Load Talents
        public static void loadTalents(object sender, LuaEventArgs args)
        {
            L.diagnosticLog("{0} Event Fired!", args.EventName);
            loadTalents();
        }

        public static void loadTalents()
        {
            set = Styx.StyxWoW.Me.GetLearnedTalents();
            Logging.Write(Colors.Aqua, "[Shineey] Talents loaded");
            resetTalents();
            foreach (TalentPlacement tp in set)
            {
                //talents[tp.Tier, tp.Index] = true;  // set new talents to true
                L.diagnosticLog("[Shineey] Tier {0} => {1}", tp.Tier, tp.Name);
            }
            setTalentVars();
        }

        public static void resetTalents()
        {
            for (int tier = 0; tier <= 6; tier++) {
                for (int index = 0; index <= 2; index++) {
                    talents[tier, index] = false;   // reset talents to false
                }
            }
            
            // Tier 1
            t_SpeedOfLight = false;
            t_LongArmOfTheLaw = false;
            t_PursuitOfJustice = false;
            // Tier 2
            t_FistOfJustice = false;
            t_Repentence = false;
            t_BlindingLight = false;
            // Tier 3
            t_SelflessHealer = false;
            t_EternalFlame = false;
            t_SacredShield = false;
            // Tier 4
            t_HandOfPurity = false;
            t_UnbreakableSpirit = false;
            t_Clemency = false;
            // Tier 5
            t_HolyAvenger = false;
            t_SanctifiedWrath = false;
            t_DivinePurpose = false;
            // Tier 6
            t_HolyPrism = false;
            t_LightsHammer = false;
            t_ExecutionSentence = false;
            // Tier 7
            t_EmpoweredSeals = false;
            t_Seraphim = false;
            t_FinalVerdict = false;
        }

        public static void setTalentVars()
        {
            foreach (TalentPlacement tp in set)
            {
                talents[tp.Tier, tp.Index] = true;  // set new talents to true
            }
            // Set talent variable to new values
            // Tier 1
            t_SpeedOfLight = talents[0, 0];
            t_LongArmOfTheLaw = talents[0, 1];
            t_PursuitOfJustice = talents[0, 2];
            // Tier 2
            t_FistOfJustice = talents[1, 0];
            t_Repentence = talents[1, 1];
            t_BlindingLight = talents[1, 2];
            // Tier 3
            t_SelflessHealer = talents[2, 0];
            t_EternalFlame = talents[2, 1];
            t_SacredShield = talents[2, 2];
            // Tier 4
            t_HandOfPurity = talents[3, 0];
            t_UnbreakableSpirit = talents[3, 1];
            t_Clemency = talents[3, 2];
            // Tier 5
            t_HolyAvenger = talents[4, 0];
            t_SanctifiedWrath = talents[4, 1];
            t_DivinePurpose = talents[4, 2];
            // Tier 6
            t_HolyPrism = talents[5, 0];
            t_LightsHammer = talents[5, 1];
            t_ExecutionSentence = talents[5, 2];
            // Tier 7
            t_EmpoweredSeals = talents[6, 0];
            t_Seraphim = talents[6, 1];
            t_FinalVerdict = talents[6, 2];
            /*
            for (int tier = 0; tier <= 6; tier++) {
                for (int index = 0; index <= 2; index++) {
                    if (talents[tier, index] == true) {
                        Logging.Write(Colors.OrangeRed, "[Shineey] Tier {0}, Index {1} = true", tier, index);
                    }
                }
            }*/
        }
        #endregion
    }
}
