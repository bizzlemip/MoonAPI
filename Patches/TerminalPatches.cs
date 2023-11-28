using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MOON_API.Patches
{
    [HarmonyPatch]
    class TerminalPatches
    {
        private static bool DontRun = false;
        [HarmonyPatch(typeof(Terminal),"Awake")]
        [HarmonyPrefix]
        public static bool Awake(Terminal __instance)
        {
            if (DontRun)
            {
                return(true);
            }
			IDictionary<string, SelectableLevel> moddedMoons = MOON_API.Core.GetMoons();//deal later
            TerminalNode keyresult = __instance.terminalNodes.allKeywords[21].specialKeywordResult;
            keyresult.displayText.Substring(keyresult.displayText.Length - 3);
            foreach (KeyValuePair<string, SelectableLevel> entry in moddedMoons)
            {
                keyresult.displayText += "\n* " + entry.Value.PlanetName + " [planetTime]";
            }
            keyresult.displayText += "\n\n";
            DontRun = true;
            return (true);
        }
    }
}
