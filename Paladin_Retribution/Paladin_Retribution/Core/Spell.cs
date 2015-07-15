using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Styx;
using Styx.CommonBot;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.TreeSharp;
using CommonBehaviors.Actions;

#region [Method] - Class Redundancy
using Action = Styx.TreeSharp.Action;
using L = Paladin_Retribution.Core.Utilities.Log;
#endregion  

namespace Paladin_Retribution.Core
{
    static class Spell
    {
        public delegate WoWUnit unitSelectionDelegate(object Context);
        public delegate T Selection<out T>(object Context);
        public static int lastSpellCast;
        private static LocalPlayer Me { get { return StyxWoW.Me; } }
        private static WoWUnit currentTarget { get { return StyxWoW.Me.CurrentTarget; } }

        #region [Method] - Cast Method
        private static Composite castMethod(this int Spell, unitSelectionDelegate unitSelection, Selection<bool> reqs = null, bool gcdCast = false)
        {
            return new Decorator(ret => ((reqs != null && reqs(ret)) || (reqs == null)) && unitSelection != null && unitSelection(ret) != null && SpellManager.CanCast(Spell, unitSelection(ret)),
                new Action(ret =>
                {
                    if (SpellManager.Cast(Spell, unitSelection(ret)))
                    {
                        lastSpellCast = Spell;
                        L.combatLog("Casting: " + WoWSpell.FromId(Spell).Name + " on " + unitSelection(ret).SafeName);
                        if (!gcdCast)
                            return RunStatus.Success;
                    }
                    return RunStatus.Failure;
                }));
        } 

        private static Composite castMethod(this int Spell, Selection<bool> reqs = null, bool gcdCast = false)
        {
            return new Decorator(ret => ((reqs != null && reqs(ret)) || (reqs == null)) && SpellManager.CanCast(Spell),
                new Action(ret =>
                {
                    if (SpellManager.Cast(Spell))
                    {
                        lastSpellCast = Spell;
                        L.combatLog("Casting: " + WoWSpell.FromId(Spell).Name);
                        if (!gcdCast)
                            return RunStatus.Success;
                    }
                    return RunStatus.Failure;
            }));
        }  

        private static Composite dropMethod(this int Spell, unitSelectionDelegate unitSelection, Selection<bool> reqs = null, bool waitForSpell = false)
        {
            return new Decorator(ret => unitSelection != null && reqs(ret) && SpellManager.CanCast(Spell) && (Me.Location.Distance(unitSelection(ret).Location) <= WoWSpell.FromId(Spell).MaxRange || WoWSpell.FromId(Spell).MaxRange == 0),
                new Sequence(
                    new Action(ret => SpellManager.Cast(Spell)),
                    new DecoratorContinue(ctx => waitForSpell,
                        new WaitContinue(1, ret => Me.CurrentPendingCursorSpell != null && Me.CurrentPendingCursorSpell.Id == Spell, new ActionAlwaysSucceed())),
                        new Action(ret =>
                        {
                            SpellManager.ClickRemoteLocation(unitSelection(ret).Location);
                            lastSpellCast = Spell;
                            L.combatLog("Casting: " + WoWSpell.FromId(Spell).Name + " on " + unitSelection(ret).SafeName);
                        })));
        }  
        #endregion

        #region [Method] - Cooldown Detection
        public static double cooldownTimeLeft(this int Spell)
        {
            try
            {
                SpellFindResults Results;
                return SpellManager.FindSpell(Spell, out Results) ? (Results.Override != null ? Results.Override.CooldownTimeLeft.TotalMilliseconds : Results.Original.CooldownTimeLeft.TotalMilliseconds) : 9999;
            }
            catch (Exception xException)
            {
                L.diagnosticLog("Exception in cooldownTimeLeft(): ", xException);
                return 9999;
            }
        }  
        #endregion

        #region [Method] - Ground Casting
        public static Composite dropCast(this int Spell, unitSelectionDelegate unitSelection, Selection<bool> reqs = null)
        {
            return dropMethod(Spell, unitSelection, reqs);
        }
        #endregion

        #region [Method] - Non-Target Casting
        public static Composite blindCast(this int Spell, Selection<bool> reqs = null, bool gcdCast = false)
        {
            return castMethod(Spell, reqs, gcdCast);
        }
        #endregion  

        #region [Method] - Standard Casting
        public static Composite Buff(this int Spell, Selection<bool> reqs = null, bool gcdCast = false)
        {
            return castMethod(Spell, ret => Me, reqs, gcdCast);
        }

        public static Composite Cast(this int Spell, Selection<bool> reqs = null, bool gcdCast = false)
        {
            return castMethod(Spell, ret => currentTarget, reqs, gcdCast);
        }
        #endregion  
    }
}
