using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Styx.WoWInternals.WoWObjects;
using Styx.WoWInternals;
using Styx.CommonBot;
using Styx;
using Bots.DungeonBuddy.Helpers;

#region [] - Class Redundancy
using L = Paladin_Retribution.Core.Utilities.Log;
using SB = Paladin_Retribution.Core.Helpers.Spell_Book;
using S = Paladin_Retribution.Core.Spell;
using TM = Paladin_Retribution.Core.Managers.Talent_Manager;
using Rot = Paladin_Retribution.Rotation.Rotation;
#endregion

namespace Paladin_Retribution.Core
{
    static class Unit
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }
        private static WoWUnit currentTarget { get { return StyxWoW.Me.CurrentTarget; } }

        #region [Method] - Active Enemies
        public static IEnumerable<WoWUnit> activeEnemies(WoWPoint fromLocation, double Range)
        {
            WoWSpell tempSpell = WoWSpell.FromId(SB.s_FinalVerdict);
            Range = System.Convert.ToDouble(tempSpell.ActualMaxRange(Me));

            var Hostile = enemyCount;
            return Hostile != null ? Hostile.Where(x => x.Location.DistanceSqr(fromLocation) < Range * Range) : null;
        }

        private static List<WoWUnit> enemyCount { get; set; }

        public static void enemyAnnex(double Range)
        {
            enemyCount.Clear();
            foreach (var u in surroundingEnemies())
            {
                if (u == null || !u.IsValid)
                    continue;
                if (!u.IsAlive || u.DistanceSqr > Range * Range)
                    continue;
                if (!u.Attackable || !u.CanSelect)
                    continue;
                if (u.IsFriendly)
                    continue;
                if (u.IsNonCombatPet && u.IsCritter)
                    continue;
                enemyCount.Add(u);
            }
        }

        private static IEnumerable<WoWUnit> surroundingEnemies() { return ObjectManager.GetObjectsOfTypeFast<WoWUnit>(); }

        static Unit() 
        {
            enemyCount = new List<WoWUnit>(); 
        }  
        #endregion

        #region [Method] - Aura Detection
        public static bool auraExists(this WoWUnit Unit, int auraID, bool isMyAura = false)
        {
            try
            {
                if (Unit == null || !Unit.IsValid)
                    return false;
                WoWAura Aura = isMyAura ? Unit.GetAllAuras().FirstOrDefault(A => A.SpellId == auraID && A.CreatorGuid == Me.Guid) : Unit.GetAllAuras().FirstOrDefault(A => A.SpellId == auraID);
                return Aura != null;
            }
            catch (Exception xException)
            {
                L.diagnosticLog("Exception in auraExists(): ", xException);
                return false;
            }
        }  

        public static uint auraStacks(this WoWUnit Unit, int auraID, bool isMyAura = false)
        {
            try
            {
                if (Unit == null || !Unit.IsValid)
                    return 0;                
                WoWAura Aura = isMyAura ? Unit.GetAllAuras().FirstOrDefault(A => A.SpellId == auraID && A.CreatorGuid == Me.Guid) : Unit.GetAllAuras().FirstOrDefault(A => A.SpellId == auraID);
                return Aura != null ? Aura.StackCount : 0;
            }
            catch (Exception xException)
            {
                L.diagnosticLog("Exception in auraStacks(): ", xException);
                return 0;
            }
        }  

        public static double auraTimeLeft(this WoWUnit Unit, int auraID, bool isMyAura = false)
        {
            try
            {
                if (Unit == null || !Unit.IsValid)
                    return 0;
                WoWAura Aura = isMyAura ? Unit.GetAllAuras().FirstOrDefault(A => A.SpellId == auraID && A.CreatorGuid == Me.Guid) : Unit.GetAllAuras().FirstOrDefault(A => A.SpellId == auraID);
                return Aura != null ? Aura.TimeLeft.TotalMilliseconds : 0;
            }
            catch (Exception xException)
            {
                L.diagnosticLog("Exception in auraTimeLeft(): ", xException);
                return 9999;
            }
        } 
        #endregion

        #region [Method] - Cache
        //public static bool auraOnM;
        //public static bool auraOnTarget;
        //public static uint auraStackOnMe;
        //public static uint auraStackOnTarget;
        //public static uint mainResource;
        public static bool bloodlust;

        public static bool avengingWrath;
        public static bool blazingContempt;
        public static bool crusadersFury;
        public static bool divineCrusader;
        public static bool divinePurpose;
        public static bool finalVerdict;
        public static bool holyAvenger;
        public static bool maraadsTruth;
        public static bool liadrinsRighteousness;
        public static bool sacredShield;
        public static bool sealOfRighteousness;
        public static bool sealOfTruth;
        public static bool seraphim;

        public static bool focusOfVengeance;    // t18 trinket buff
        public static bool wingsOfLiberty;      // t18 4pc buff

        public static uint holyPower;

        public static double divinePurpose_remains;
        public static double divineCrusader_remains;
        public static double maraadsTruth_remains;
        public static double liadrinsRighteousness_remains;
        public static double focusOfVengeance_remains;

        public static double cd_judgement_remains;
        public static double cd_seraphim_remains;
        public static double cd_crusaderStrike_remains;

        public static int nearbyEnemies;

        public static void Cache()
        {
            //auraOnMe = auraExists(Me, SB.auraName);
            //auraOnTarget = auraExists(currentTarget, SB.auraName, true);
            //auraStackOnMe = auraStacks(Me, SB.auraName);
            //auraStackOnTarget = auraStacks(currentTarget, SB.auraName, true);
            //mainResource = playerPower();
            bloodlust = auraExists(Me, SB.a_lust) || auraExists(Me, SB.a_hero) || auraExists(Me, SB.a_warp) || auraExists(Me, SB.a_hyst) || auraExists(Me, SB.a_winds);

            avengingWrath = auraExists(Me, SB.a_AvengingWrath);
            blazingContempt = auraExists(Me, SB.a_BlazingContempt);
            crusadersFury = auraExists(Me, SB.a_CrusadersFury);
            divineCrusader = auraExists(Me, SB.a_DivineCrusader);
            divinePurpose = auraExists(Me, SB.a_DivinePurpose);
            finalVerdict = auraExists(Me, SB.a_FinalVerdict);
            holyAvenger = auraExists(Me, SB.a_HolyAvenger);
            maraadsTruth = auraExists(Me, SB.a_MaraadsTruth);
            liadrinsRighteousness = auraExists(Me, SB.a_LiadrinsRighteousness);
            sacredShield = auraExists(Me, SB.a_SacredShield);
            sealOfRighteousness = auraExists(Me, SB.a_SealOfRighteousness);
            sealOfTruth = auraExists(Me, SB.a_SealOfTruth);
            seraphim = auraExists(Me, SB.a_Seraphim);

            focusOfVengeance = auraExists(Me, SB.a_focusOfVengeance);   // t18 trinket buff
            wingsOfLiberty = auraExists(Me, SB.a_wingsOfLiberty);       // t18 4pc buff

            holyPower = Me.CurrentHolyPower;

            divinePurpose_remains = auraTimeLeft(Me, SB.a_DivinePurpose, true);
            divineCrusader_remains = auraTimeLeft(Me, SB.a_DivineCrusader, true);
            if (TM.t_EmpoweredSeals) {
                maraadsTruth_remains = auraTimeLeft(Me, SB.a_MaraadsTruth, true);
                liadrinsRighteousness_remains = auraTimeLeft(Me, SB.a_LiadrinsRighteousness, true);
            }
            if (Rot.t18_equipped)
                focusOfVengeance_remains = auraTimeLeft(Me, SB.a_focusOfVengeance, true);

            cd_judgement_remains = S.cooldownTimeLeft(SB.s_Judgement);
            if (TM.t_Seraphim)
                cd_seraphim_remains = S.cooldownTimeLeft(SB.s_Seraphim);
            cd_crusaderStrike_remains = S.cooldownTimeLeft(SB.s_CrusaderStrike);

            nearbyEnemies = activeEnemies(Me.Location, 8).Count();
        } 
        #endregion

        #region [Method] - Unit Evaluation
        public static bool isUnitValid(WoWUnit Unit) { return Unit != null && Unit.IsValid && Unit.IsAlive && Unit.Attackable; }

        public static bool isUnitValid(this WoWUnit Unit, int spellID)
        {
            // isUnitValid(this WoWUnit Unit, int spellID) 
            WoWSpell tempSpell = WoWSpell.FromId(spellID);
            double Range = System.Convert.ToDouble( tempSpell.ActualMaxRange(Unit));
            //L.combatLog("ActualMaxRange of " + tempSpell + ": " + Range);
            return Unit != null && Unit.IsValid && Unit.IsAlive && Unit.Attackable && Unit.DistanceSqr <= Range * Range;
            //return Unit != null && Unit.IsValid && Unit.IsAlive && Unit.Attackable;
        }  
        #endregion

    }
}
