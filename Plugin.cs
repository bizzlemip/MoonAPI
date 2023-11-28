using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MOON_API
{
    
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Core : BaseUnityPlugin
    {
        private Harmony _harmony;
        public static IDictionary<string, SelectableLevel> ModdedMoons = new Dictionary<string, SelectableLevel>();
        public static IDictionary<string, GameObject> MoonPrefabs = new Dictionary<string, GameObject>();
        public static IDictionary<string, int> ModdedIds = new Dictionary<string, int>();
        public static IDictionary<string, SelectableLevel> GetMoons()
        {
            return (ModdedMoons);
        }
        public static void AddMoon(SelectableLevel level, GameObject Prefab)
        {
            AddPrefab(level.name, Prefab);
            ModdedMoons[level.name] = level;
        }
        private static void AddPrefab(string MoonName, GameObject Prefab)
        {
            MoonPrefabs[MoonName] = Prefab;
        }
        public IDictionary<string, SelectableLevel> mlevels;
        private void Awake()
        {
            _harmony = new Harmony("MOON_API");
            _harmony.PatchAll();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "InitSceneLaunchOptions" && StartOfRound.Instance!=null && StartOfRound.Instance.currentLevel!= null && StartOfRound.Instance.currentLevel.name!=null && MOON_API.Core.MoonPrefabs.ContainsKey(StartOfRound.Instance.currentLevel.name))
            {
                GameObject[] RootObjects = UnityEngine.SceneManagement.SceneManager.GetSceneByName("InitSceneLaunchOptions").GetRootGameObjects();
                foreach (GameObject obj in RootObjects)
                {
                    Debug.Log(obj.name);
                    Debug.Log("A");
                    if (obj.name == "InitSceneScript" || obj.name == "EventSystem" || obj.name == "Volume" || obj.name == "UICamera" || obj.name == "Canvas")
                    {
                        obj.SetActive(false);
                    }
                }
                Instantiate(MOON_API.Core.MoonPrefabs[StartOfRound.Instance.currentLevel.name]);
            }
        }
    }
}