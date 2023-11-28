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
    class MoonPatches
    {
		public static T[] ResizeArray<T>(T[] oldArray, int newSize)
		{
			var newArray = new T[newSize];
			oldArray.CopyTo(newArray, 0);
			return newArray;
		}
		private static int AddToMoons(StartOfRound SOR, SelectableLevel Moon)
        {
			int id = -1;
			for (int i = 0; i < SOR.levels.Length; i++)
			{
				if (SOR.levels[i] is null)
				{
					id = i;
					break;
				}
			}
			if (id == -1)
            {
				throw new NullReferenceException("No null value found in StartOfRound.levels");
            }
			SOR.levels[id] = Moon;
			return (id);
        }
        [HarmonyPatch(typeof(GameNetworkManager), "SaveGameValues")]
        [HarmonyPostfix]
        public static void SaveGameValues(GameNetworkManager __instance)
        {
			if (!__instance.isHostingGame)
			{
				return;
			}
			if (!StartOfRound.Instance.inShipPhase)
			{
				return;
			}
			try
			{
				if (StartOfRound.Instance.currentLevel!=null && MOON_API.Core.ModdedMoons.ContainsKey(StartOfRound.Instance.currentLevel.name))
                {
					ES3.Save("ModdedPlanetID", StartOfRound.Instance.currentLevel.name, __instance.currentSaveFileName);
                }
			}
			catch (Exception arg)
			{
				Debug.LogError($"Error while trying to save game values when disconnecting as host: {arg}");
			}
			return;
        }
		[HarmonyPatch(typeof(StartOfRound),"Awake")]
		[HarmonyPrefix]	
		[HarmonyPriority(0)]//Has to run after any theoretical moon initializations that need StartOfRound variables.
		private static void Awake(StartOfRound __instance)
        {
			__instance.levels = ResizeArray(__instance.levels, __instance.levels.Length + MOON_API.Core.ModdedMoons.Count());
			foreach (KeyValuePair<string, SelectableLevel> entry in MOON_API.Core.ModdedMoons)
			{
				int ID = AddToMoons(__instance, entry.Value);
				entry.Value.levelID = ID;
				MOON_API.Core.ModdedIds[entry.Key] = ID;
			}
        }
    }
}
