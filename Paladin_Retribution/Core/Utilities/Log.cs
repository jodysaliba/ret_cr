using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Styx.Common;

using Rot = Paladin_Retribution.Rotation.Rotation;

namespace Paladin_Retribution.Core.Utilities
{
    class Log
    {
        public static string lastCombatMSG;

        #region [Method] - Combat Log
        public static void combatLog(string Message, params object[] args)
        {
            if (Message == lastCombatMSG)
                return;
            //Logging.Write(Colors.OrangeRed, "[Shineey] {0} - Enemies: {1}", String.Format(Message, args), Rot.enemies);
            Logging.Write(Colors.OrangeRed, "[Shineey] {0}", String.Format(Message, args));
            lastCombatMSG = Message;
        }
        #endregion

        #region [Method] - Diagnostics Log
        public static void diagnosticLog(string Message, params object[] args)
        {
            if (Message == null)
                return;
            Logging.WriteDiagnostic(Colors.Firebrick, "{0}", String.Format(Message, args));
        }  
        #endregion
    }
}
