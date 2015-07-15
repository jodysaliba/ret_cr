using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Styx;
using Styx.WoWInternals;

namespace Paladin_Retribution.Core.Managers
{
    class Inventory
    {
        private static readonly List<uint> EquippedGear = new List<uint>();

        static Inventory()
        {
            UpdateGear();

            #region Lua Events

            Lua.Events.AttachEvent("PLAYER_EQUIPMENT_CHANGED", UpdateGear);
            Lua.Events.AttachEvent("WEAR_EQUIPMENT_SET", UpdateGear);

            #endregion
        }

        private static void UpdateGear(object sender = null, LuaEventArgs args = null)
        {
            EquippedGear.Clear();

            for (uint i = 0; i < 18; i++)
            {
                var slotInfo = StyxWoW.Me.Inventory.GetItemBySlot(i);

                if (slotInfo != null)
                    EquippedGear.Add(slotInfo.ItemInfo.Id);
            }
        }

        internal static bool HasItemEquipped(uint id)
        {
            return EquippedGear.Contains(id);
        }

        internal static bool HasItemEquipped(List<uint> idList)
        {
            return EquippedGear.Any(idList.Contains);
        }

        public static int CountEquippedItems(List<uint> idList)
        {
            return EquippedGear.Count(idList.Contains);
        }
    }
}
