using CommonBehaviors.Actions;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Inventory;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paladin_Retribution.Core.Managers;
using Styx.WoWInternals;
using Styx.Common;
using Action = Styx.TreeSharp.Action;
using System.Windows.Media;

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
    class Rotation
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } } 
        private static WoWUnit currentTarget { get { return StyxWoW.Me.CurrentTarget; } }

        public static int num_t17 = Inventory.CountEquippedItems(new List<uint> { 115565, 115566, 115567, 115568, 115569 }); // Tier 17
        public static int num_t18 = Inventory.CountEquippedItems(new List<uint> { 124318, 124328, 124333, 124339, 124345 }); // Tier 18

        public static bool t17_equipped = (num_t17 >= 2 && num_t18 < 2) || (num_t17 < 2 && num_t18 < 2); // if at least 2pc t17 equipped but no t18 set bonuses
        public static bool t18_equipped = !t17_equipped; // if 2/4pc t18 or no set bonuses
        public static bool t18_trinket = Inventory.HasItemEquipped(124518); // Paladin trinket off Archimonde (Libram of Vindication)
        
        public static int myEnemies = 0;
        public static int enemies = 0;
        public static int activeEn = -1;
        
        #region [Method] - Combat Buffing
        public static Composite combatBuffing()
        {
            return new PrioritySelector(
                new Decorator(ret => HkM.manualOn || !Me.IsAlive || !Me.GotTarget, new ActionAlwaysSucceed()),


                //actions+=/auto_attack,target_if=dot.censure.remains<4
                //actions+=/execution_sentence,if=!talent.seraphim.enabled
                S.Cast(SB.s_ExecutionSentence, ret => U.isUnitValid(currentTarget, SB.s_ExecutionSentence) && HkM.cooldownsOn && (!TM.t_Seraphim)),
                //actions+=/execution_sentence,sync=seraphim,if=talent.seraphim.enabled
                S.Cast(SB.s_ExecutionSentence, ret => U.isUnitValid(currentTarget, SB.s_ExecutionSentence) && HkM.cooldownsOn && (TM.t_Seraphim && U.seraphim)),
                //actions+=/lights_hammer,if=!talent.seraphim.enabled
                S.dropCast(SB.s_LightsHammer, ret => currentTarget, ret => U.isUnitValid(currentTarget, SB.s_LightsHammer) && HkM.cooldownsOn && (!TM.t_Seraphim)),
                //actions+=/lights_hammer,sync=seraphim,if=talent.seraphim.enabled
                S.dropCast(SB.s_LightsHammer, ret => currentTarget, ret => U.isUnitValid(currentTarget, SB.s_LightsHammer) && HkM.cooldownsOn && (TM.t_Seraphim && U.seraphim)),
                //actions+=/avenging_wrath,sync=seraphim,if=talent.seraphim.enabled
                //S.blindCast(SB.s_AvengingWrath, ret => HkM.cooldownsOn),
                //actions+=/avenging_wrath,if=!talent.seraphim.enabled&set_bonus.tier18_4pc=0
                //actions+=/avenging_wrath,if=!talent.seraphim.enabled&time<20&set_bonus.tier18_4pc=1
                //actions+=/holy_avenger,sync=avenging_wrath,if=!talent.seraphim.enabled
                S.blindCast(SB.s_HolyAvenger, ret => HkM.cooldownsOn && (U.avengingWrath && !TM.t_Seraphim)),
                //actions+=/holy_avenger,sync=seraphim,if=talent.seraphim.enabled
                S.blindCast(SB.s_HolyAvenger, ret => HkM.cooldownsOn && (U.seraphim && TM.t_Seraphim)),
                //actions+=/holy_avenger,if=holy_power<=2&!talent.seraphim.enabled
                S.blindCast(SB.s_HolyAvenger, ret => HkM.cooldownsOn && (U.holyPower <= 2 && TM.t_Seraphim)),
                //actions+=/blood_fury
                //actions+=/berserking
                //actions+=/arcane_torrent
                //actions+=/seraphim
                S.blindCast(SB.s_Seraphim, ret => TM.t_Seraphim),
                S.Buff(SB.s_SealOfTruth, ret => !HkM.righteousness && !U.sealOfTruth),
                S.Buff(SB.s_SealOfRighteousness, ret => HkM.righteousness && !U.sealOfRighteousness)
                );
        } 
        #endregion

        #region [Method] - Precombat Buffing
        public static Composite preCombatBuffing()
        {
            return new PrioritySelector(
                new Decorator(ret => HkM.manualOn || !Me.IsAlive || Me.IsCasting || Me.IsChanneling || Me.Mounted || Me.OnTaxi, new ActionAlwaysSucceed()),
                S.Buff(SB.s_SealOfTruth, ret => !HkM.righteousness && !U.sealOfTruth),
                S.Buff(SB.s_SealOfRighteousness, ret => HkM.righteousness && !U.sealOfRighteousness)
                );
        }  
        #endregion

        #region [Method] - Rotation Selector
        public static Composite rotationSelector()
        {
            return new PrioritySelector(
                new Decorator(ret => HkM.manualOn || !Me.IsAlive || !Me.GotTarget, new ActionAlwaysSucceed()),
                new Switch<bool>(ret => t17_equipped,
                    new SwitchArgument<bool>(true,
                        new PrioritySelector(
                            // NO TIER --- TIER 17

                            ActionAoeCount(),

                            // Autoattack
                            new Action(ret =>
                            {
                                if (!Me.IsAutoAttacking && U.isUnitValid(currentTarget, SB.s_AutoAttack))
                                    Lua.DoString("StartAttack()");
                                return RunStatus.Failure;
                            }),

                            //new Decorator(ret => HkM.aoeOn && enemies >= 3, new PrioritySelector(     // until I find a way to get nearby unit count
                            new Decorator(ret => HkM.aoeOn && enemies >= 3, new PrioritySelector(
                                // 3+ TARGETS //
                                //actions.cleave=final_verdict,if=buff.final_verdict.down&holy_power=5
                                S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (!U.finalVerdict && U.holyPower == 5)),
                                //actions.cleave+=/divine_storm,if=buff.divine_crusader.react&holy_power=5&buff.final_verdict.up
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && U.holyPower == 5 && U.finalVerdict)),
                                //actions.cleave+=/divine_storm,if=holy_power=5&buff.final_verdict.up
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.holyPower == 5 && U.finalVerdict)),
                                //actions.cleave+=/divine_storm,if=buff.divine_crusader.react&holy_power=5&!talent.final_verdict.enabled
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && (U.divineCrusader && U.holyPower == 5 && !TM.t_FinalVerdict)),
                                //actions.cleave+=/divine_storm,if=holy_power=5&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*4)&!talent.final_verdict.enabled
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.holyPower == 5 && (!TM.t_Seraphim || U.cd_seraphim_remains > 5000) && !TM.t_FinalVerdict)),
                                //actions.cleave+=/hammer_of_wrath
                                S.Cast(SB.s_HammerOfWrath, ret => U.isUnitValid(currentTarget, SB.s_HammerOfWrath)),
                                //actions.cleave+=/hammer_of_the_righteous,if=t18_class_trinket=1&buff.focus_of_vengeance.remains<gcd.max*2
                                S.Cast(SB.s_HammerOfTheRighteous, ret => U.isUnitValid(currentTarget, SB.s_HammerOfTheRighteous) && (t18_trinket && U.focusOfVengeance_remains < 3000)),
                                //actions.cleave+=/judgment,if=talent.empowered_seals.enabled&seal.righteousness&buff.liadrins_righteousness.remains<cooldown.judgment.duration
                                S.Cast(SB.s_Judgement, ret => U.isUnitValid(currentTarget, SB.s_Judgement) && (TM.t_EmpoweredSeals && U.sealOfRighteousness && U.liadrinsRighteousness_remains < 5000)),
                                //actions.cleave+=/exorcism,if=buff.blazing_contempt.up&holy_power<=2&buff.holy_avenger.down
                                S.Cast(SB.s_Exorcism, ret => U.isUnitValid(currentTarget, SB.s_Exorcism) && (U.blazingContempt && U.holyPower <= 2 && !U.holyAvenger)),
                                //actions.cleave+=/divine_storm,if=buff.divine_crusader.react&buff.final_verdict.up&(buff.avenging_wrath.up|target.health.pct<35)
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && U.finalVerdict && (U.avengingWrath || currentTarget.HealthPercent < 35))),
                                //actions.cleave+=/divine_storm,if=buff.final_verdict.up&(buff.avenging_wrath.up|target.health.pct<35)
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.finalVerdict && (U.avengingWrath || currentTarget.HealthPercent < 35))),
                                //actions.cleave+=/final_verdict,if=buff.final_verdict.down&(buff.avenging_wrath.up|target.health.pct<35)
                                S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (!U.finalVerdict && (U.avengingWrath || currentTarget.HealthPercent < 35))),
                                //actions.cleave+=/divine_storm,if=buff.divine_crusader.react&(buff.avenging_wrath.up|target.health.pct<35)&!talent.final_verdict.enabled
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && (U.avengingWrath || currentTarget.HealthPercent < 35) && !TM.t_FinalVerdict)),
                                //actions.cleave+=/divine_storm,if=holy_power=5&(buff.avenging_wrath.up|target.health.pct<35)&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*3)&!talent.final_verdict.enabled
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.holyPower == 5 && (U.avengingWrath || currentTarget.HealthPercent < 35) && (!TM.t_Seraphim || U.cd_seraphim_remains < 3000) && !TM.t_FinalVerdict)),
                                //actions.cleave+=/divine_storm,if=holy_power=4&(buff.avenging_wrath.up|target.health.pct<35)&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*4)&!talent.final_verdict.enabled
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.holyPower == 4 && (U.avengingWrath || currentTarget.HealthPercent < 35) && (!TM.t_Seraphim || U.cd_seraphim_remains < 4000) && !TM.t_FinalVerdict)),
                                //actions.cleave+=/divine_storm,if=holy_power=3&(buff.avenging_wrath.up|target.health.pct<35)&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*5)&!talent.final_verdict.enabled
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.holyPower == 3 && (U.avengingWrath || currentTarget.HealthPercent < 35) && (!TM.t_Seraphim || U.cd_seraphim_remains < 5000) && !TM.t_FinalVerdict)),
                                //actions.cleave+=/hammer_of_the_righteous,if=spell_targets.hammer_of_the_righteous>=4&talent.seraphim.enabled
                                S.Cast(SB.s_HammerOfTheRighteous, ret => U.isUnitValid(currentTarget, SB.s_HammerOfTheRighteous) && (enemies >= 4 && TM.t_Seraphim)),
                                //actions.cleave+=/hammer_of_the_righteous,,if=spell_targets.hammer_of_the_righteous>=4&(holy_power<=3|(holy_power=4&target.health.pct>=35&buff.avenging_wrath.down))
                                S.Cast(SB.s_HammerOfTheRighteous, ret => U.isUnitValid(currentTarget, SB.s_HammerOfTheRighteous) && (enemies >= 4 && (U.holyPower <= 3 || (U.holyPower == 4 && currentTarget.HealthPercent >= 35 && !U.avengingWrath)))),
                                //actions.cleave+=/crusader_strike,if=talent.seraphim.enabled
                                S.Cast(SB.s_CrusaderStrike, ret => U.isUnitValid(currentTarget, SB.s_CrusaderStrike) && (TM.t_Seraphim)),
                                //actions.cleave+=/crusader_strike,if=holy_power<=3|(holy_power=4&target.health.pct>=35&buff.avenging_wrath.down)
                                S.Cast(SB.s_CrusaderStrike, ret => U.isUnitValid(currentTarget, SB.s_CrusaderStrike) && (U.holyPower <= 3 || (U.holyPower == 4 && currentTarget.HealthPercent >= 35 && !U.avengingWrath))),
                                    //actions.cleave+=/exorcism,if=glyph.mass_exorcism.enabled&!set_bonus.tier17_4pc=1
                                        //S.Cast(SB.s_Exorcism, ret => U.isUnitValid(currentTarget, 30) && (U.blazingContempt && U.holyPower <= 2 && !U.holyAvenger)),
                                    //actions.cleave+=/judgment,cycle_targets=1,if=last_judgment_target!=target&talent.seraphim.enabled&glyph.double_jeopardy.enabled
                                //actions.cleave+=/judgment,if=talent.seraphim.enabled
                                S.Cast(SB.s_Judgement, ret => U.isUnitValid(currentTarget, SB.s_Judgement) && (TM.t_Seraphim)),
                                    //actions.cleave+=/judgment,cycle_targets=1,if=last_judgment_target!=target&glyph.double_jeopardy.enabled&(holy_power<=3|(holy_power=4&cooldown.crusader_strike.remains>=gcd*2&target.health.pct>35&buff.avenging_wrath.down))
                                //actions.cleave+=/judgment,if=holy_power<=3|(holy_power=4&cooldown.crusader_strike.remains>=gcd*2&target.health.pct>35&buff.avenging_wrath.down)
                                S.Cast(SB.s_Judgement, ret => U.isUnitValid(currentTarget, SB.s_Judgement) && (U.holyPower <= 3 | (U.holyPower == 4 && U.cd_crusaderStrike_remains >= 7000 && currentTarget.HealthPercent > 35 && !U.avengingWrath))),
                                //actions.cleave+=/divine_storm,if=buff.divine_crusader.react&buff.final_verdict.up
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && U.finalVerdict)),
                                //actions.cleave+=/divine_storm,if=buff.divine_purpose.react&buff.final_verdict.up
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divinePurpose && U.finalVerdict)),
                                //actions.cleave+=/divine_storm,if=holy_power>=4&buff.final_verdict.up
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.holyPower >= 4 && U.finalVerdict)),
                                //actions.cleave+=/final_verdict,if=buff.divine_purpose.react&buff.final_verdict.down
                                S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divinePurpose && !U.finalVerdict)),
                                //actions.cleave+=/final_verdict,if=holy_power>=4&buff.final_verdict.down
                                S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.holyPower >= 4 && !U.finalVerdict)),
                                //actions.cleave+=/divine_storm,if=buff.divine_crusader.react&!talent.final_verdict.enabled
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && !TM.t_FinalVerdict)),
                                //actions.cleave+=/divine_storm,if=holy_power>=4&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*5)&!talent.final_verdict.enabled
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.holyPower >= 4 && (!TM.t_Seraphim || U.cd_seraphim_remains > 7000) && !TM.t_FinalVerdict)),
                                //actions.cleave+=/exorcism,if=talent.seraphim.enabled
                                S.Cast(SB.s_Exorcism, ret => U.isUnitValid(currentTarget, SB.s_Exorcism) && (TM.t_Seraphim)),
                                //actions.cleave+=/exorcism,if=holy_power<=3|(holy_power=4&(cooldown.judgment.remains>=gcd*2&cooldown.crusader_strike.remains>=gcd*2&target.health.pct>35&buff.avenging_wrath.down))
                                S.Cast(SB.s_Exorcism, ret => U.isUnitValid(currentTarget, SB.s_Exorcism) && (U.holyPower <= 3 || (U.holyPower == 4 && (U.cd_judgement_remains > 2500 && U.cd_crusaderStrike_remains > 2500 && currentTarget.HealthPercent > 35 && !U.avengingWrath)))),
                                //actions.cleave+=/divine_storm,if=holy_power>=3&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*6)&!talent.final_verdict.enabled
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.holyPower >= 3 && (!TM.t_Seraphim || U.cd_seraphim_remains > 8000) && !TM.t_FinalVerdict)),
                                //actions.cleave+=/divine_storm,if=holy_power>=3&buff.final_verdict.up
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.holyPower >= 3 && U.finalVerdict)),
                                //actions.cleave+=/final_verdict,if=holy_power>=3&buff.final_verdict.down
                                S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.holyPower >= 3 && !U.finalVerdict))

                            )),
                            // SINGLE/DOUBLE TARGET //
                            //actions.single=divine_storm,if=buff.divine_crusader.react&(holy_power=5|buff.holy_avenger.up&holy_power>=3)&buff.final_verdict.up
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && (U.holyPower == 5 || U.holyAvenger && U.holyPower >= 3) && U.finalVerdict)),
                            //actions.single+=/divine_storm,if=buff.divine_crusader.react&(holy_power=5|buff.holy_avenger.up&holy_power>=3)&spell_targets.divine_storm=2&!talent.final_verdict.enabled
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && (U.holyPower == 5 || U.holyAvenger && U.holyPower >= 3) && enemies == 2 && !U.finalVerdict)), 
                            //actions.single+=/divine_storm,if=(holy_power=5|buff.holy_avenger.up&holy_power>=3)&spell_targets.divine_storm=2&buff.final_verdict.up
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && ((U.holyPower == 5 || U.holyAvenger && U.holyPower >= 3) && enemies == 2 && U.finalVerdict)),
                            //actions.single+=/divine_storm,if=buff.divine_crusader.react&(holy_power=5|buff.holy_avenger.up&holy_power>=3)&(talent.seraphim.enabled&cooldown.seraphim.remains<gcd*4)
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && (U.holyPower == 5 || U.holyAvenger && U.holyPower >= 3) && (TM.t_Seraphim && U.cd_seraphim_remains < 4000))),
                            //actions.single+=/templars_verdict,if=(holy_power=5|buff.holy_avenger.up&holy_power>=3)&(buff.avenging_wrath.down|target.health.pct>35)&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*4)
                            S.Cast(SB.s_TemplarsVerdict, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && !TM.t_FinalVerdict && ((U.holyPower == 5 || U.holyAvenger && U.holyPower >= 3) && (!U.avengingWrath || currentTarget.HealthPercent > 35) && (!TM.t_Seraphim || U.cd_seraphim_remains > 4000))),
                            //actions.single+=/templars_verdict,if=buff.divine_purpose.react&buff.divine_purpose.remains<3
                            S.Cast(SB.s_TemplarsVerdict, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && !TM.t_FinalVerdict && (U.divinePurpose && U.divinePurpose_remains < 3000)),
                            //actions.single+=/divine_storm,if=buff.divine_crusader.react&buff.divine_crusader.remains<3&buff.final_verdict.up
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && U.divineCrusader_remains < 3000 && U.finalVerdict)),
                            //actions.single+=/final_verdict,if=holy_power=5|buff.holy_avenger.up&holy_power>=3
                            S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && TM.t_FinalVerdict && (U.holyPower == 5 || U.holyAvenger && U.holyPower >= 3)),
                            //actions.single+=/final_verdict,if=buff.divine_purpose.react&buff.divine_purpose.remains<3
                            S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && TM.t_FinalVerdict && (U.divinePurpose && U.divinePurpose_remains < 3000)),
                            //actions.single+=/crusader_strike,if=t18_class_trinket=1&buff.focus_of_vengeance.remains<gcd.max*2
                            S.Cast(SB.s_CrusaderStrike, ret => U.isUnitValid(currentTarget, SB.s_CrusaderStrike) && (t18_trinket && U.focusOfVengeance_remains < 3000)),
                            //actions.single+=/hammer_of_wrath
                            S.Cast(SB.s_HammerOfWrath, ret => U.isUnitValid(currentTarget, SB.s_HammerOfWrath)),
                            //actions.single+=/exorcism,if=buff.blazing_contempt.up&holy_power<=2&buff.holy_avenger.down
                            S.Cast(SB.s_Exorcism, ret => U.isUnitValid(currentTarget, SB.s_Exorcism) && (U.blazingContempt && U.holyPower <= 2 && !U.holyAvenger)),
                            //actions.single+=/divine_storm,if=buff.divine_crusader.react&buff.final_verdict.up&(buff.avenging_wrath.up|target.health.pct<35)
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && U.finalVerdict && (U.avengingWrath || currentTarget.HealthPercent < 35))),
                            //actions.single+=/divine_storm,if=spell_targets.divine_storm=2&buff.final_verdict.up&(buff.avenging_wrath.up|target.health.pct<35)
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (enemies == 2 && U.finalVerdict && (U.avengingWrath || currentTarget.HealthPercent < 35))),
                            //actions.single+=/final_verdict,if=buff.avenging_wrath.up|target.health.pct<35
                            S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && TM.t_FinalVerdict && (U.avengingWrath || currentTarget.HealthPercent < 35)),
                            //actions.single+=/divine_storm,if=buff.divine_crusader.react&spell_targets.divine_storm=2&(buff.avenging_wrath.up|target.health.pct<35)&!talent.final_verdict.enabled
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && enemies == 2 && (U.avengingWrath || currentTarget.HealthPercent < 35) && !TM.t_FinalVerdict)),
                            //actions.single+=/templars_verdict,if=holy_power=5&(buff.avenging_wrath.up|target.health.pct<35)&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*3)
                            S.Cast(SB.s_TemplarsVerdict, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && !TM.t_FinalVerdict && (U.holyPower == 5 && (U.avengingWrath || currentTarget.HealthPercent < 35) && (!TM.t_Seraphim || U.cd_seraphim_remains > 3000))),
                            //actions.single+=/templars_verdict,if=holy_power=4&(buff.avenging_wrath.up|target.health.pct<35)&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*4)
                            S.Cast(SB.s_TemplarsVerdict, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && !TM.t_FinalVerdict && (U.holyPower == 4 && (U.avengingWrath || currentTarget.HealthPercent < 35) && (!TM.t_Seraphim || U.cd_seraphim_remains > 4000))),
                            //actions.single+=/templars_verdict,if=holy_power=3&(buff.avenging_wrath.up|target.health.pct<35)&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*5)
                            S.Cast(SB.s_TemplarsVerdict, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && !TM.t_FinalVerdict && (U.holyPower == 3 && (U.avengingWrath || currentTarget.HealthPercent < 35) && (!TM.t_Seraphim || U.cd_seraphim_remains > 5000))),
                            //actions.single+=/judgment,if=talent.empowered_seals.enabled&seal.truth&buff.maraads_truth.remains<cooldown.judgment.duration*1.5
                            S.Cast(SB.s_Judgement, ret => U.isUnitValid(currentTarget, SB.s_Judgement) && (TM.t_EmpoweredSeals && U.sealOfTruth && U.maraadsTruth_remains < 7500)),
                            //actions.single+=/judgment,if=talent.empowered_seals.enabled&seal.righteousness&buff.liadrins_righteousness.remains<cooldown.judgment.duration*1.5
                            S.Cast(SB.s_Judgement, ret => U.isUnitValid(currentTarget, SB.s_Judgement) && (TM.t_EmpoweredSeals && U.sealOfRighteousness && U.liadrinsRighteousness_remains < 7500)),
                            //actions.single+=/seal_of_truth,if=talent.empowered_seals.enabled&buff.maraads_truth.remains<(cooldown.judgment.duration|buff.maraads_truth.down)&(buff.avenging_wrath.up|target.health.pct<35)
                            S.Buff(SB.s_SealOfTruth, ret => TM.t_EmpoweredSeals && (!U.maraadsTruth || U.maraadsTruth_remains < 3000) && (U.avengingWrath || currentTarget.HealthPercent < 35)),
                            //actions.single+=/seal_of_righteousness,if=talent.empowered_seals.enabled&buff.liadrins_righteousness.remains<cooldown.judgment.duration&buff.maraads_truth.remains>cooldown.judgment.duration*1.5&target.health.pct<35&!buff.wings_of_liberty.up&!buff.bloodlust.up
                            S.Buff(SB.s_SealOfRighteousness, ret => TM.t_EmpoweredSeals && (!U.liadrinsRighteousness || U.liadrinsRighteousness_remains < 3000) && U.maraadsTruth_remains > 7500 && currentTarget.HealthPercent < 35 && !U.wingsOfLiberty && !U.bloodlust),
                            //actions.single+=/crusader_strike,if=talent.seraphim.enabled
                            S.Cast(SB.s_CrusaderStrike, ret => U.isUnitValid(currentTarget, SB.s_CrusaderStrike) && (TM.t_Seraphim)),
                            //actions.single+=/crusader_strike,if=holy_power<=3|(holy_power=4&target.health.pct>=35&buff.avenging_wrath.down)
                            S.Cast(SB.s_CrusaderStrike, ret => U.isUnitValid(currentTarget, SB.s_CrusaderStrike) && (U.holyPower <= 3 || (U.holyPower == 4 && currentTarget.HealthPercent >= 35 && !U.avengingWrath))),
                            //actions.single+=/divine_storm,if=buff.divine_crusader.react&(buff.avenging_wrath.up|target.health.pct<35)&!talent.final_verdict.enabled
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && (U.avengingWrath || currentTarget.HealthPercent < 35) && !TM.t_FinalVerdict)),
                                //actions.single+=/exorcism,if=glyph.mass_exorcism.enabled&spell_targets.exorcism>=2&!glyph.double_jeopardy.enabled&!set_bonus.tier17_4pc=1
                                    //S.Cast(SB.s_Exorcism, ret => U.isUnitValid(currentTarget, 30) && ()),
                                //actions.single+=/judgment,cycle_targets=1,if=last_judgment_target!=target&talent.seraphim.enabled&glyph.double_jeopardy.enabled
                            //actions.single+=/judgment,if=talent.seraphim.enabled
                            S.Cast(SB.s_Judgement, ret => U.isUnitValid(currentTarget, SB.s_Judgement) && (TM.t_Seraphim)),
                                //actions.single+=/judgment,cycle_targets=1,if=last_judgment_target!=target&glyph.double_jeopardy.enabled&(holy_power<=3|(holy_power=4&cooldown.crusader_strike.remains>=gcd*2&target.health.pct>35&buff.avenging_wrath.down))
                            //actions.single+=/judgment,if=holy_power<=3|(holy_power=4&cooldown.crusader_strike.remains>=gcd*2&target.health.pct>35&buff.avenging_wrath.down)
                            S.Cast(SB.s_Judgement, ret => U.isUnitValid(currentTarget, SB.s_Judgement) && (U.holyPower <= 3 | (U.holyPower == 4 && U.cd_crusaderStrike_remains >= 7000 && currentTarget.HealthPercent > 35 && !U.avengingWrath))),
                            //actions.single+=/divine_storm,if=buff.divine_crusader.react&buff.final_verdict.up
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && U.finalVerdict)),
                            //actions.single+=/divine_storm,if=spell_targets.divine_storm=2&holy_power>=4&buff.final_verdict.up
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && enemies == 2 && U.holyPower >= 4 && U.finalVerdict)),
                            //actions.single+=/final_verdict,if=buff.divine_purpose.react
                            S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && TM.t_FinalVerdict && (U.divinePurpose)),
                            //actions.single+=/final_verdict,if=holy_power>=4
                            S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && TM.t_FinalVerdict && (U.holyPower >= 4)),
                            //actions.single+=/seal_of_truth,if=talent.empowered_seals.enabled&buff.maraads_truth.remains<cooldown.judgment.duration*1.5&buff.liadrins_righteousness.remains>cooldown.judgment.duration*1.5
                            S.Buff(SB.s_SealOfTruth, ret => TM.t_EmpoweredSeals && ((!U.maraadsTruth || U.maraadsTruth_remains < 7500) && U.liadrinsRighteousness_remains > 7500)),
                            //actions.single+=/seal_of_righteousness,if=talent.empowered_seals.enabled&buff.liadrins_righteousness.remains<cooldown.judgment.duration*1.5&buff.maraads_truth.remains>cooldown.judgment.duration*1.5&!buff.bloodlust.up
                            S.Buff(SB.s_SealOfRighteousness, ret => TM.t_EmpoweredSeals && ((!U.liadrinsRighteousness || U.liadrinsRighteousness_remains < 7500) && U.maraadsTruth_remains > 7500 && !U.bloodlust)),
                            //actions.single+=/divine_storm,if=buff.divine_crusader.react&spell_targets.divine_storm=2&holy_power>=4&!talent.final_verdict.enabled
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && enemies == 2 && U.holyPower >= 4 && !TM.t_FinalVerdict)),
                            //actions.single+=/templars_verdict,if=buff.divine_purpose.react
                            S.Cast(SB.s_TemplarsVerdict, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && !TM.t_FinalVerdict && (U.divinePurpose)),
                            //actions.single+=/divine_storm,if=buff.divine_crusader.react&!talent.final_verdict.enabled
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && !TM.t_FinalVerdict)),
                            //actions.single+=/templars_verdict,if=holy_power>=4&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*5)
                            S.Cast(SB.s_TemplarsVerdict, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && !TM.t_FinalVerdict && (U.holyPower >= 4 && (!TM.t_Seraphim || U.cd_seraphim_remains > 7000))),
                            //actions.single+=/exorcism,if=talent.seraphim.enabled
                            S.Cast(SB.s_Exorcism, ret => U.isUnitValid(currentTarget, SB.s_Exorcism) && (TM.t_Seraphim)),
                            //actions.single+=/exorcism,if=holy_power<=3|(holy_power=4&(cooldown.judgment.remains>=gcd*2&cooldown.crusader_strike.remains>=gcd*2&target.health.pct>35&buff.avenging_wrath.down))
                            S.Cast(SB.s_Exorcism, ret => U.isUnitValid(currentTarget, SB.s_Exorcism) && (U.holyPower <= 3 || (U.holyPower == 4 && (U.cd_judgement_remains > 2500 && U.cd_crusaderStrike_remains > 2500 && currentTarget.HealthPercent > 35 && !U.avengingWrath)))),
                            //actions.single+=/divine_storm,if=spell_targets.divine_storm=2&holy_power>=3&buff.final_verdict.up
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && enemies == 2 && U.holyPower >= 3 && U.finalVerdict)),
                            //actions.single+=/final_verdict,if=holy_power>=3
                            S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && TM.t_FinalVerdict && (U.holyPower >= 3)),
                            //actions.single+=/templars_verdict,if=holy_power>=3&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*6)
                            S.Cast(SB.s_TemplarsVerdict, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && !TM.t_FinalVerdict && (U.holyPower >= 3 && (!TM.t_Seraphim || U.cd_seraphim_remains > 8000)))

                            )),
                    new SwitchArgument<bool>(false,
                        new PrioritySelector(
                            // TIER 18

                            new Decorator(ret => HkM.aoeOn && enemies >= 3, new PrioritySelector(
                                // 3+ TARGETS //
                                //actions.cleave=final_verdict,if=buff.final_verdict.down&holy_power=5
                                S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (!U.finalVerdict && U.holyPower == 5)),
                                //actions.cleave+=/divine_storm,if=buff.divine_crusader.react&holy_power=5&buff.final_verdict.up
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && U.holyPower == 5 && U.finalVerdict)),
                                //actions.cleave+=/divine_storm,if=holy_power=5&buff.final_verdict.up
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.holyPower == 5 && U.finalVerdict)),
                                //actions.cleave+=/divine_storm,if=buff.divine_crusader.react&holy_power=5&!talent.final_verdict.enabled
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && (U.divineCrusader && U.holyPower == 5 && !TM.t_FinalVerdict)),
                                //actions.cleave+=/divine_storm,if=holy_power=5&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*4)&!talent.final_verdict.enabled
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && (U.holyPower == 5 && (!TM.t_Seraphim || U.cd_seraphim_remains > 5000) && !TM.t_FinalVerdict)),
                                //actions.cleave+=/hammer_of_wrath
                                S.Cast(SB.s_HammerOfWrath, ret => U.isUnitValid(currentTarget, SB.s_HammerOfWrath)),
                                //actions.cleave+=/hammer_of_the_righteous,if=t18_class_trinket=1&buff.focus_of_vengeance.remains<gcd.max*2
                                S.Cast(SB.s_HammerOfTheRighteous, ret => U.isUnitValid(currentTarget, SB.s_HammerOfTheRighteous) && (t18_trinket && U.focusOfVengeance_remains < 3000)),
                                //actions.cleave+=/judgment,if=talent.empowered_seals.enabled&seal.righteousness&buff.liadrins_righteousness.remains<cooldown.judgment.duration
                                S.Cast(SB.s_Judgement, ret => U.isUnitValid(currentTarget, SB.s_Judgement) && (TM.t_EmpoweredSeals && U.sealOfRighteousness && U.liadrinsRighteousness_remains < 5000)),
                                //actions.cleave+=/exorcism,if=buff.blazing_contempt.up&holy_power<=2&buff.holy_avenger.down
                                S.Cast(SB.s_Exorcism, ret => U.isUnitValid(currentTarget, SB.s_Exorcism) && (U.blazingContempt && U.holyPower <= 2 && !U.holyAvenger)),
                                //actions.cleave+=/divine_storm,if=buff.divine_crusader.react&buff.final_verdict.up&(buff.avenging_wrath.up|target.health.pct<35)
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && U.finalVerdict && (U.avengingWrath || currentTarget.HealthPercent < 35))),
                                //actions.cleave+=/divine_storm,if=buff.final_verdict.up&(buff.avenging_wrath.up|target.health.pct<35)
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.finalVerdict && (U.avengingWrath || currentTarget.HealthPercent < 35))),
                                //actions.cleave+=/final_verdict,if=buff.final_verdict.down&(buff.avenging_wrath.up|target.health.pct<35)
                                S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (!U.finalVerdict && (U.avengingWrath || currentTarget.HealthPercent < 35))),
                                //actions.cleave+=/divine_storm,if=buff.divine_crusader.react&(buff.avenging_wrath.up|target.health.pct<35)&!talent.final_verdict.enabled
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && (U.divineCrusader && (U.avengingWrath || currentTarget.HealthPercent < 35) && !TM.t_FinalVerdict)),
                                //actions.cleave+=/divine_storm,if=holy_power=5&(buff.avenging_wrath.up|target.health.pct<35)&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*3)&!talent.final_verdict.enabled
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && (U.holyPower == 5 && (U.avengingWrath || currentTarget.HealthPercent < 35) && (!TM.t_Seraphim || U.cd_seraphim_remains < 3000) && !TM.t_FinalVerdict)),
                                //actions.cleave+=/divine_storm,if=holy_power=4&(buff.avenging_wrath.up|target.health.pct<35)&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*4)&!talent.final_verdict.enabled
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && (U.holyPower == 4 && (U.avengingWrath || currentTarget.HealthPercent < 35) && (!TM.t_Seraphim || U.cd_seraphim_remains < 4000) && !TM.t_FinalVerdict)),
                                //actions.cleave+=/divine_storm,if=holy_power=3&(buff.avenging_wrath.up|target.health.pct<35)&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*5)&!talent.final_verdict.enabled
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && (U.holyPower == 3 && (U.avengingWrath || currentTarget.HealthPercent < 35) && (!TM.t_Seraphim || U.cd_seraphim_remains < 5000) && !TM.t_FinalVerdict)),
                                //actions.cleave+=/hammer_of_the_righteous,if=spell_targets.hammer_of_the_righteous>=4&talent.seraphim.enabled
                                S.Cast(SB.s_HammerOfTheRighteous, ret => U.isUnitValid(currentTarget, SB.s_HammerOfTheRighteous) && (enemies >= 4 && TM.t_Seraphim)),
                                //actions.cleave+=/hammer_of_the_righteous,,if=spell_targets.hammer_of_the_righteous>=4&(holy_power<=3|(holy_power=4&target.health.pct>=35&buff.avenging_wrath.down))
                                S.Cast(SB.s_HammerOfTheRighteous, ret => U.isUnitValid(currentTarget, SB.s_HammerOfTheRighteous) && (enemies >= 4 && (U.holyPower <= 3 || (U.holyPower == 4 && currentTarget.HealthPercent >= 35 && !U.avengingWrath)))),
                                //actions.cleave+=/crusader_strike,if=talent.seraphim.enabled
                                S.Cast(SB.s_CrusaderStrike, ret => U.isUnitValid(currentTarget, SB.s_CrusaderStrike) && (TM.t_Seraphim)),
                                //actions.cleave+=/crusader_strike,if=holy_power<=3|(holy_power=4&target.health.pct>=35&buff.avenging_wrath.down)
                                S.Cast(SB.s_CrusaderStrike, ret => U.isUnitValid(currentTarget, SB.s_CrusaderStrike) && (U.holyPower <= 3 || (U.holyPower == 4 && currentTarget.HealthPercent >= 35 && !U.avengingWrath))),
                                    //actions.cleave+=/exorcism,if=glyph.mass_exorcism.enabled&!set_bonus.tier17_4pc=1
                                    //actions.cleave+=/judgment,cycle_targets=1,if=last_judgment_target!=target&talent.seraphim.enabled&glyph.double_jeopardy.enabled
                                //actions.cleave+=/judgment,if=talent.seraphim.enabled
                                S.Cast(SB.s_Judgement, ret => U.isUnitValid(currentTarget, SB.s_Judgement) && (TM.t_Seraphim)),
                                    //actions.cleave+=/judgment,cycle_targets=1,if=last_judgment_target!=target&glyph.double_jeopardy.enabled&(holy_power<=3|(holy_power=4&cooldown.crusader_strike.remains>=gcd*2&target.health.pct>35&buff.avenging_wrath.down))
                                //actions.cleave+=/judgment,if=holy_power<=3|(holy_power=4&cooldown.crusader_strike.remains>=gcd*2&target.health.pct>35&buff.avenging_wrath.down)
                                S.Cast(SB.s_Judgement, ret => U.isUnitValid(currentTarget, SB.s_Judgement) && (U.holyPower <= 3 | (U.holyPower == 4 && U.cd_crusaderStrike_remains >= 7000 && currentTarget.HealthPercent > 35 && !U.avengingWrath))),
                                //actions.cleave+=/divine_storm,if=buff.divine_crusader.react&buff.final_verdict.up
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && U.finalVerdict)),
                                //actions.cleave+=/divine_storm,if=buff.divine_purpose.react&buff.final_verdict.up
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divinePurpose && U.finalVerdict)),
                                //actions.cleave+=/divine_storm,if=holy_power>=4&buff.final_verdict.up
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.holyPower >= 4 && U.finalVerdict)),
                                //actions.cleave+=/final_verdict,if=buff.divine_purpose.react&buff.final_verdict.down
                                S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divinePurpose && !U.finalVerdict)),
                                //actions.cleave+=/final_verdict,if=holy_power>=4&buff.final_verdict.down
                                S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.holyPower >= 4 && !U.finalVerdict)),
                                //actions.cleave+=/divine_storm,if=buff.divine_crusader.react&!talent.final_verdict.enabled
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && (U.divineCrusader && !TM.t_FinalVerdict)),
                                //actions.cleave+=/divine_storm,if=holy_power>=4&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*5)&!talent.final_verdict.enabled
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && (U.holyPower >= 4 && (!TM.t_Seraphim || U.cd_seraphim_remains > 7000) && !TM.t_FinalVerdict)),
                                //actions.cleave+=/exorcism,if=talent.seraphim.enabled
                                S.Cast(SB.s_Exorcism, ret => U.isUnitValid(currentTarget, SB.s_Exorcism) && (TM.t_Seraphim)),
                                //actions.cleave+=/exorcism,if=holy_power<=3|(holy_power=4&(cooldown.judgment.remains>=gcd*2&cooldown.crusader_strike.remains>=gcd*2&target.health.pct>35&buff.avenging_wrath.down))
                                S.Cast(SB.s_Exorcism, ret => U.isUnitValid(currentTarget, SB.s_Exorcism) && (U.holyPower <= 3 || (U.holyPower == 4 && (U.cd_judgement_remains > 2500 && U.cd_crusaderStrike_remains > 2500 && currentTarget.HealthPercent > 35 && !U.avengingWrath)))),
                                //actions.cleave+=/divine_storm,if=holy_power>=3&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*6)&!talent.final_verdict.enabled
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && (U.holyPower >= 3 && (!TM.t_Seraphim || U.cd_seraphim_remains > 8000) && !TM.t_FinalVerdict)),
                                //actions.cleave+=/divine_storm,if=holy_power>=3&buff.final_verdict.up
                                S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.holyPower >= 3 && U.finalVerdict)),
                                //actions.cleave+=/final_verdict,if=holy_power>=3&buff.final_verdict.down
                                S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.holyPower >= 3 && !U.finalVerdict))

                            )),
                            // SINGLE/DOUBLE TARGET //
                            //actions.single=divine_storm,if=buff.divine_crusader.react&(holy_power=5|buff.holy_avenger.up&holy_power>=3)&buff.final_verdict.up
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && (U.holyPower == 5 || U.holyAvenger && U.holyPower >= 3) && U.finalVerdict)),
                            //actions.single+=/divine_storm,if=buff.divine_crusader.react&(holy_power=5|buff.holy_avenger.up&holy_power>=3)&spell_targets.divine_storm=2&!talent.final_verdict.enabled
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && (U.holyPower == 5 || U.holyAvenger && U.holyPower >= 3) && enemies == 2 && !U.finalVerdict)),
                            //actions.single+=/divine_storm,if=(holy_power=5|buff.holy_avenger.up&holy_power>=3)&spell_targets.divine_storm=2&buff.final_verdict.up
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && ((U.holyPower == 5 || U.holyAvenger && U.holyPower >= 3) && enemies == 2 && U.finalVerdict)),
                            //actions.single+=/divine_storm,if=buff.divine_crusader.react&(holy_power=5|buff.holy_avenger.up&holy_power>=3)&(talent.seraphim.enabled&cooldown.seraphim.remains<gcd*4)
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && (U.holyPower == 5 || U.holyAvenger && U.holyPower >= 3) && (TM.t_Seraphim && U.cd_seraphim_remains < 4000))),
                            //actions.single+=/templars_verdict,if=(holy_power=5|buff.holy_avenger.up&holy_power>=3)&(buff.avenging_wrath.down|target.health.pct>35)&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*4)
                            S.Cast(SB.s_TemplarsVerdict, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && !TM.t_FinalVerdict && ((U.holyPower == 5 || U.holyAvenger && U.holyPower >= 3) && (!U.avengingWrath || currentTarget.HealthPercent > 35) && (!TM.t_Seraphim || U.cd_seraphim_remains > 4000))),
                            //actions.single+=/templars_verdict,if=buff.divine_purpose.react&buff.divine_purpose.remains<3
                            S.Cast(SB.s_TemplarsVerdict, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && !TM.t_FinalVerdict && (U.divinePurpose && U.divinePurpose_remains < 3000)),
                            //actions.single+=/divine_storm,if=buff.divine_crusader.react&buff.divine_crusader.remains<3&buff.final_verdict.up
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && U.divineCrusader_remains < 3000 && U.finalVerdict)),
                            //actions.single+=/final_verdict,if=holy_power=5|buff.holy_avenger.up&holy_power>=3
                            S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && TM.t_FinalVerdict && (U.holyPower == 5 || U.holyAvenger && U.holyPower >= 3)),
                            //actions.single+=/final_verdict,if=buff.divine_purpose.react&buff.divine_purpose.remains<3
                            S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && TM.t_FinalVerdict && (U.divinePurpose && U.divinePurpose_remains < 3000)),
                            //actions.single+=/crusader_strike,if=t18_class_trinket=1&buff.focus_of_vengeance.remains<gcd.max*2
                            S.Cast(SB.s_CrusaderStrike, ret => U.isUnitValid(currentTarget, SB.s_CrusaderStrike) && (t18_trinket && U.focusOfVengeance_remains < 3000)),
                            //actions.single+=/hammer_of_wrath
                            S.Cast(SB.s_HammerOfWrath, ret => U.isUnitValid(currentTarget, SB.s_HammerOfWrath)),
                            //actions.single+=/exorcism,if=buff.blazing_contempt.up&holy_power<=2&buff.holy_avenger.down
                            S.Cast(SB.s_Exorcism, ret => U.isUnitValid(currentTarget, SB.s_Exorcism) && (U.blazingContempt && U.holyPower <= 2 && !U.holyAvenger)),
                            //actions.single+=/divine_storm,if=buff.divine_crusader.react&buff.final_verdict.up&(buff.avenging_wrath.up|target.health.pct<35)
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && U.finalVerdict && (U.avengingWrath || currentTarget.HealthPercent < 35))),
                            //actions.single+=/divine_storm,if=spell_targets.divine_storm=2&buff.final_verdict.up&(buff.avenging_wrath.up|target.health.pct<35)
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (enemies == 2 && U.finalVerdict && (U.avengingWrath || currentTarget.HealthPercent < 35))),
                            //actions.single+=/final_verdict,if=buff.avenging_wrath.up|target.health.pct<35
                            S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && TM.t_FinalVerdict && (U.avengingWrath || currentTarget.HealthPercent < 35)),
                            //actions.single+=/divine_storm,if=buff.divine_crusader.react&spell_targets.divine_storm=2&(buff.avenging_wrath.up|target.health.pct<35)&!talent.final_verdict.enabled
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && enemies == 2 && (U.avengingWrath || currentTarget.HealthPercent < 35) && !TM.t_FinalVerdict)),
                            //actions.single+=/templars_verdict,if=holy_power=5&(buff.avenging_wrath.up|target.health.pct<35)&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*3)
                            S.Cast(SB.s_TemplarsVerdict, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && !TM.t_FinalVerdict && (U.holyPower == 5 && (U.avengingWrath || currentTarget.HealthPercent < 35) && (!TM.t_Seraphim || U.cd_seraphim_remains > 3000))),
                            //actions.single+=/templars_verdict,if=holy_power=4&(buff.avenging_wrath.up|target.health.pct<35)&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*4)
                            S.Cast(SB.s_TemplarsVerdict, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && !TM.t_FinalVerdict && (U.holyPower == 4 && (U.avengingWrath || currentTarget.HealthPercent < 35) && (!TM.t_Seraphim || U.cd_seraphim_remains > 4000))),
                            //actions.single+=/templars_verdict,if=holy_power=3&(buff.avenging_wrath.up|target.health.pct<35)&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*5)
                            S.Cast(SB.s_TemplarsVerdict, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && !TM.t_FinalVerdict && (U.holyPower == 3 && (U.avengingWrath || currentTarget.HealthPercent < 35) && (!TM.t_Seraphim || U.cd_seraphim_remains > 5000))),
                            //actions.single+=/judgment,if=talent.empowered_seals.enabled&seal.truth&buff.maraads_truth.remains<cooldown.judgment.duration*1.5
                            S.Cast(SB.s_Judgement, ret => U.isUnitValid(currentTarget, SB.s_Judgement) && (TM.t_EmpoweredSeals && U.sealOfTruth && U.maraadsTruth_remains < 7500)),
                            //actions.single+=/judgment,if=talent.empowered_seals.enabled&seal.righteousness&buff.liadrins_righteousness.remains<cooldown.judgment.duration*1.5
                            S.Cast(SB.s_Judgement, ret => U.isUnitValid(currentTarget, SB.s_Judgement) && (TM.t_EmpoweredSeals && U.sealOfRighteousness && U.liadrinsRighteousness_remains < 7500)),
                            //actions.single+=/seal_of_truth,if=talent.empowered_seals.enabled&buff.maraads_truth.remains<(cooldown.judgment.duration|buff.maraads_truth.down)&(buff.avenging_wrath.up|target.health.pct<35)&!buff.wings_of_liberty.up
                            S.Buff(SB.s_SealOfTruth, ret => TM.t_EmpoweredSeals && (!U.maraadsTruth || U.maraadsTruth_remains < 3000) && (U.avengingWrath || currentTarget.HealthPercent < 35) && !U.wingsOfLiberty),
                            //actions.single+=/seal_of_righteousness,if=talent.empowered_seals.enabled&buff.liadrins_righteousness.remains<cooldown.judgment.duration&buff.maraads_truth.remains>cooldown.judgment.duration*1.5&target.health.pct<35&!buff.wings_of_liberty.up&!buff.bloodlust.up
                            S.Buff(SB.s_SealOfRighteousness, ret => TM.t_EmpoweredSeals && (!U.liadrinsRighteousness || U.liadrinsRighteousness_remains < 3000) && U.maraadsTruth_remains > 7500 && currentTarget.HealthPercent < 35 && !U.wingsOfLiberty && !U.bloodlust),
                            //actions.single+=/crusader_strike,if=talent.seraphim.enabled
                            S.Cast(SB.s_CrusaderStrike, ret => U.isUnitValid(currentTarget, SB.s_CrusaderStrike) && (TM.t_Seraphim)),
                            //actions.single+=/crusader_strike,if=holy_power<=3|(holy_power=4&target.health.pct>=35&buff.avenging_wrath.down)
                            S.Cast(SB.s_CrusaderStrike, ret => U.isUnitValid(currentTarget, SB.s_CrusaderStrike) && (U.holyPower <= 3 || (U.holyPower == 4 && currentTarget.HealthPercent >= 35 && !U.avengingWrath))),
                            //actions.single+=/divine_storm,if=buff.divine_crusader.react&(buff.avenging_wrath.up|target.health.pct<35)&!talent.final_verdict.enabled
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && (U.avengingWrath || currentTarget.HealthPercent < 35) && !TM.t_FinalVerdict)),
                                    //actions.single+=/exorcism,if=glyph.mass_exorcism.enabled&spell_targets.exorcism>=2&!glyph.double_jeopardy.enabled&!set_bonus.tier17_4pc=1
                                //actions.single+=/judgment,cycle_targets=1,if=last_judgment_target!=target&talent.seraphim.enabled&glyph.double_jeopardy.enabled
                            //actions.single+=/judgment,if=talent.seraphim.enabled
                            S.Cast(SB.s_Judgement, ret => U.isUnitValid(currentTarget, SB.s_Judgement) && (TM.t_Seraphim)),
                                //actions.single+=/judgment,cycle_targets=1,if=last_judgment_target!=target&glyph.double_jeopardy.enabled&(holy_power<=3|(holy_power=4&cooldown.crusader_strike.remains>=gcd*2&target.health.pct>35&buff.avenging_wrath.down))
                            //actions.single+=/judgment,if=holy_power<=3|(holy_power=4&cooldown.crusader_strike.remains>=gcd*2&target.health.pct>35&buff.avenging_wrath.down)
                            S.Cast(SB.s_Judgement, ret => U.isUnitValid(currentTarget, SB.s_Judgement) && (U.holyPower <= 3 | (U.holyPower == 4 && U.cd_crusaderStrike_remains >= 2500 && currentTarget.HealthPercent > 35 && !U.avengingWrath))),
                            //actions.single+=/divine_storm,if=buff.divine_crusader.react&buff.final_verdict.up
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && U.finalVerdict)),
                            //actions.single+=/divine_storm,if=spell_targets.divine_storm=2&holy_power>=4&buff.final_verdict.up
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && enemies == 2 && U.holyPower >= 4 && U.finalVerdict)),
                            //actions.single+=/final_verdict,if=buff.divine_purpose.react
                            S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && TM.t_FinalVerdict && (U.divinePurpose)),
                            //actions.single+=/final_verdict,if=holy_power>=4
                            S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && TM.t_FinalVerdict && (U.holyPower >= 4)),
                            //actions.single+=/seal_of_truth,if=talent.empowered_seals.enabled&buff.maraads_truth.remains<cooldown.judgment.duration*1.5&buff.liadrins_righteousness.remains>cooldown.judgment.duration*1.5
                            S.Buff(SB.s_SealOfTruth, ret => TM.t_EmpoweredSeals && ((!U.maraadsTruth || U.maraadsTruth_remains < 7500) && U.liadrinsRighteousness_remains > 7500)),
                            //actions.single+=/seal_of_righteousness,if=talent.empowered_seals.enabled&buff.liadrins_righteousness.remains<cooldown.judgment.duration*1.5&buff.maraads_truth.remains>cooldown.judgment.duration*1.5&!buff.bloodlust.up
                            S.Buff(SB.s_SealOfRighteousness, ret => TM.t_EmpoweredSeals && ((!U.liadrinsRighteousness || U.liadrinsRighteousness_remains < 7500) && U.maraadsTruth_remains > 7500 && !U.bloodlust)),
                            //actions.single+=/divine_storm,if=buff.divine_crusader.react&spell_targets.divine_storm=2&holy_power>=4&!talent.final_verdict.enabled
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && enemies == 2 && U.holyPower >= 4 && !TM.t_FinalVerdict)),
                            //actions.single+=/templars_verdict,if=buff.divine_purpose.react
                            S.Cast(SB.s_TemplarsVerdict, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && !TM.t_FinalVerdict && (U.divinePurpose)),
                            //actions.single+=/divine_storm,if=buff.divine_crusader.react&!talent.final_verdict.enabled
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && !TM.t_FinalVerdict)),
                            //actions.single+=/templars_verdict,if=holy_power>=4&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*5)
                            S.Cast(SB.s_TemplarsVerdict, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && !TM.t_FinalVerdict && (U.holyPower >= 4 && (!TM.t_Seraphim || U.cd_seraphim_remains > 7000))),
                            //actions.single+=/exorcism,if=talent.seraphim.enabled
                            S.Cast(SB.s_Exorcism, ret => U.isUnitValid(currentTarget, SB.s_Exorcism) && (TM.t_Seraphim)),
                            //actions.single+=/exorcism,if=holy_power<=3|(holy_power=4&(cooldown.judgment.remains>=gcd*2&cooldown.crusader_strike.remains>=gcd*2&target.health.pct>35&buff.avenging_wrath.down))
                            S.Cast(SB.s_Exorcism, ret => U.isUnitValid(currentTarget, SB.s_Exorcism) && (U.holyPower <= 3 || (U.holyPower == 4 && (U.cd_judgement_remains > 2500 && U.cd_crusaderStrike_remains > 2500 && currentTarget.HealthPercent > 35 && !U.avengingWrath)))),
                            //actions.single+=/divine_storm,if=spell_targets.divine_storm=2&holy_power>=3&buff.final_verdict.up
                            S.Cast(SB.s_DivineStorm, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && (U.divineCrusader && enemies == 2 && U.holyPower >= 3 && U.finalVerdict)),
                            //actions.single+=/final_verdict,if=holy_power>=3
                            S.Cast(SB.s_FinalVerdict, ret => U.isUnitValid(currentTarget, SB.s_FinalVerdict) && TM.t_FinalVerdict && (U.holyPower >= 3)),
                            //actions.single+=/templars_verdict,if=holy_power>=3&(!talent.seraphim.enabled|cooldown.seraphim.remains>gcd*6)
                            S.Cast(SB.s_TemplarsVerdict, ret => U.isUnitValid(currentTarget, SB.s_TemplarsVerdict) && !TM.t_FinalVerdict && (U.holyPower >= 3 && (!TM.t_Seraphim || U.cd_seraphim_remains > 8000)))
                            
                            ))));
        }  
        #endregion

        private static Action ActionAoeCount()
        {
            return new Action(ret =>
            {
                enemies = U.activeEnemies(Me.Location, 8f).Count();
                return RunStatus.Failure;
            });
        }
    }
}
