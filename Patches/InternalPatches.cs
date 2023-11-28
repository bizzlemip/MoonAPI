using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MOON_API.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class InternalPatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(StartOfRound)
                .GetMethod("SetTimeAndPlanetToSavedSettings",
                            BindingFlags.NonPublic | BindingFlags.Instance);
        }
        [HarmonyPrefix]
        static bool Prefix(StartOfRound __instance)
        {
            string currentSaveFileName = GameNetworkManager.Instance.currentSaveFileName;
            __instance.randomMapSeed = ES3.Load("RandomSeed", currentSaveFileName, 0);
            string ModName = ES3.Load("ModdedPlanetId", currentSaveFileName, "");
            int levelID;
            if (!String.IsNullOrEmpty(ModName) && MOON_API.Core.ModdedIds.ContainsKey(ModName))
            {
                levelID = MOON_API.Core.ModdedIds[ModName];
            }
            else
            {
                levelID = ES3.Load("CurrentPlanetID", currentSaveFileName, __instance.defaultPlanet);
            }
            if (!__instance.testRoom.activeInHierarchy)
            {
                __instance.ChangeLevel(levelID);
                __instance.ChangePlanet();
            }
            TimeOfDay.Instance.timesFulfilledQuota = ES3.Load("QuotasPassed", currentSaveFileName, 0);
            TimeOfDay.Instance.profitQuota = ES3.Load("ProfitQuota", currentSaveFileName, TimeOfDay.Instance.quotaVariables.startingQuota);
            TimeOfDay.Instance.totalTime = TimeOfDay.Instance.lengthOfHours * (float)TimeOfDay.Instance.numberOfHours;
            TimeOfDay.Instance.timeUntilDeadline = ES3.Load("DeadlineTime", currentSaveFileName, (int)(TimeOfDay.Instance.totalTime * (float)TimeOfDay.Instance.quotaVariables.deadlineDaysAmount));
            TimeOfDay.Instance.quotaFulfilled = ES3.Load("QuotaFulfilled", currentSaveFileName, 0);
            TimeOfDay.Instance.SetBuyingRateForDay();
            __instance.gameStats.daysSpent = ES3.Load("Stats_DaysSpent", currentSaveFileName, 0);
            __instance.gameStats.deaths = ES3.Load("Stats_Deaths", currentSaveFileName, 0);
            __instance.gameStats.scrapValueCollected = ES3.Load("Stats_ValueCollected", currentSaveFileName, 0);
            __instance.gameStats.allStepsTaken = ES3.Load("Stats_StepsTaken", currentSaveFileName, 0);
            TimeOfDay.Instance.UpdateProfitQuotaCurrentTime();
            __instance.SetPlanetsWeather();
            UnityEngine.Object.FindObjectOfType<Terminal>().SetItemSales();
            if (__instance.gameStats.daysSpent == 0)
            {
                MethodInfo PlayFirstDayShipAnimation = (typeof(StartOfRound)).GetMethod("PlayFirstDayShipAnimation", BindingFlags.NonPublic | BindingFlags.Static);
                Object[] Parameters = {true};
                PlayFirstDayShipAnimation.Invoke(__instance, Parameters);
            }
            if (TimeOfDay.Instance.timeUntilDeadline > 0f && TimeOfDay.Instance.daysUntilDeadline <= 0 && TimeOfDay.Instance.timesFulfilledQuota <= 0)
            {
                MethodInfo PlayFirstDayShipAnimation = (typeof(StartOfRound)).GetMethod("playDaysLeftAlertSFXDelayed", BindingFlags.NonPublic | BindingFlags.Static);
                PlayFirstDayShipAnimation.Invoke(__instance, null);
            }
            return (false);
        }
    }
}
