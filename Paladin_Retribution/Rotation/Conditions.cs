using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonBehaviors.Actions;
using Styx;
using Styx.CommonBot;
using Styx.TreeSharp;
using CommonBehaviors.Actions;
using Styx.WoWInternals.WoWObjects;
using Styx.WoWInternals;
using Action = Styx.TreeSharp.Action;


#region [Method] - Class Redundancy
using C = Paladin_Retribution.Rotation.Conditions;
using HkM = Paladin_Retribution.Core.Managers.Hotkey_Manager;
using S = Paladin_Retribution.Core.Spell;
using SB = Paladin_Retribution.Core.Helpers.Spell_Book;
using TM = Paladin_Retribution.Core.Managers.Talent_Manager;
using U = Paladin_Retribution.Core.Unit;
#endregion

namespace Paladin_Retribution.Rotation
{
    class Conditions
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }
        private static WoWUnit currentTarget { get { return StyxWoW.Me.CurrentTarget; } } 

        #region [Method] - Auto Attack
        private static Composite autoAttack()
        {
            return new Action(ret =>
            {
                if (!Me.IsAutoAttacking && U.isUnitValid(currentTarget, SB.s_AutoAttack))
                    Lua.DoString("StartAttack()");
                return RunStatus.Failure;
            });
        }
        #endregion  

        #region [Method] - Avenging Wrath
        public static Composite avenging_wrath()
        {
            return S.blindCast(SB.s_AvengingWrath, ret => true);
        }
        #endregion

        #region [Method] - Crusader Strike
        public static Composite crusader_strike(WoWUnit Unit, int spellID)
        {
            return S.Cast(SB.s_CrusaderStrike, ret => U.isUnitValid(Unit, spellID));
        }
        #endregion

        #region [Method] - Divine Storm
        public static Composite divine_storm(WoWUnit Unit, int spellID)
        {
            return S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(Unit, spellID));
        }
        #endregion

        #region [Method] - Execution Sentence
        public static Composite execution_sentence(WoWUnit Unit, int spellID)
        {
            return S.Cast(SB.s_ExecutionSentence, ret => U.isUnitValid(Unit, spellID));
        }
        #endregion

        #region [Method] - Exorcism
        public static Composite exorcism(WoWUnit Unit, int spellID)
        {
            return S.Cast(SB.s_Exorcism, ret => U.isUnitValid(Unit, spellID));
        }
        #endregion

        #region [Method] - Final Verdict
        public static Composite final_verdict(WoWUnit Unit, int spellID)
        {
            return S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(Unit, spellID));
        }
        #endregion

        #region [Method] - Hammer of the Righteous
        public static Composite hammer_of_the_righteous(WoWUnit Unit, int spellID)
        {
            return S.Cast(SB.s_HammerOfTheRighteous, ret => U.isUnitValid(Unit, spellID));
        }
        #endregion

        #region [Method] - Hammer of Wrath
        public static Composite hammer_of_wrath(WoWUnit Unit, int spellID)
        {
            return S.Cast(SB.s_HammerOfWrath, ret => U.isUnitValid(Unit, spellID));
        }
        #endregion

        #region [Method] - Holy Avenger
        public static Composite holy_avenger()
        {
            return S.blindCast(SB.s_HolyAvenger, ret => true);
        }
        #endregion

        #region [Method] - Judgment
        public static Composite judgement(WoWUnit Unit, int spellID)
        {
            return S.Cast(SB.s_Judgement, ret => U.isUnitValid(Unit, spellID));
        }
        #endregion

        #region [Method] - Lights Hammer
        public static Composite lights_hammer(WoWUnit Unit, int spellID)
        {
            return S.dropCast(SB.s_LightsHammer, ret => Unit, ret => U.isUnitValid(Unit, spellID));
        }
        #endregion

        #region [Method] - Seal of Righteousness
        public static Composite seal_of_righteousness()
        {
            return S.Buff(SB.s_SealOfRighteousness, ret => true);
        }
        #endregion

        #region [Method] - Seal of Truth
        public static Composite seal_of_truth(WoWUnit Unit, int spellID)
        {
            return S.Buff(SB.s_SealOfTruth, ret => true);
        }
        #endregion

        #region [Method] - Seraphim
        public static Composite seraphim()
        {
            return S.blindCast(SB.s_Seraphim, ret => true);
        }
        #endregion

        #region [Method] - Templars Verdict
        public static Composite templars_verdict(WoWUnit Unit, int spellID)
        {
            return S.Cast(SB.s_TemplarsVerdict, ret => U.isUnitValid(Unit, spellID));
        }
        #endregion

    }
}
