using BepInEx;
using StormyItems.Items;
using R2API;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace StormyItems
{
    //This is an example plugin that can be put in BepInEx/plugins/ExamplePlugin/ExamplePlugin.dll to test out.
    //It's a small plugin that adds a relatively simple item to the game, and gives you that item whenever you press F2.

    //This attribute specifies that we have a dependency on R2API, as we're using it to add our item to the game.
    //You don't need this if you're not using R2API in your plugin, it's just to tell BepInEx to initialize R2API before this plugin so it's safe to use R2API.
    [BepInDependency(R2API.R2API.PluginGUID)]

    //This attribute is required, and lists metadata for your plugin.
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    //We will be using 2 modules from R2API: ItemAPI to add our item and LanguageAPI to add our language tokens.
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(LanguageAPI), nameof(RecalculateStatsAPI), nameof(PrefabAPI))]

    //This is the main declaration of our plugin class. BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    //BaseUnityPlugin itself inherits from MonoBehaviour, so you can use this as a reference for what you can declare and use in your plugin class: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    public class Main : BaseUnityPlugin
    {
        public static PluginInfo PInfo { get; private set; }

        //The Plugin GUID should be a unique ID for this plugin, which is human readable (as it is used in places like the config).
        //If we see this PluginGUID as it is on thunderstore, we will deprecate this mod. Change the PluginAuthor and the PluginName !
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Quickstraw";
        public const string PluginName = "StormyItems";
        public const string PluginVersion = "0.0.1";

        public static List<CharacterBody> CharBodies = new List<CharacterBody>();

        public static List<ItemBase> Items;

        public void Start()
        {
            Assets.Init();
            Assets.Start();

            //This section automatically scans the project for all items
            var ItemTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ItemBase)));

            foreach (var itemType in ItemTypes)
            {
                ItemBase item = (ItemBase)System.Activator.CreateInstance(itemType);
                if (ValidateItem(item, Items))
                {
                    item.StartInit(Config);
                }
            }

        }

        //The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            //Init our logging class so that we can properly log for debugging
            Log.Init(Logger);
            PInfo = Info;
            Items = new List<ItemBase>();

            CharacterBody.onBodyStartGlobal += CollectBodies;
            CharacterBody.onBodyDestroyGlobal += DestroyBodies;
            

            // This line of log will appear in the bepinex console when the Awake method is done.
            Log.LogInfo(nameof(Awake) + " done.");
        }

        //The Update() method is run on every frame of the game.
        private void Update()
        {
            if (PlayerCharacterMasterController.instances != null && PlayerCharacterMasterController.instances.Count > 0)
            {
                foreach (ItemBase ib in Items)
                {
                    ib.OnUpdate();
                }
            }
        }

        private void FixedUpdate()
        {
            try
            {
                if (PlayerCharacterMasterController.instances != null && PlayerCharacterMasterController.instances.Count > 0 && Items != null)
                {
                    foreach (ItemBase ib in Items)
                    {
                        ib.OnFixedUpdate();
                    }
                }
            }
            catch (NullReferenceException e)
            {
                Log.LogError(e.StackTrace);
            }
        }

        /// <summary>
        /// A helper to easily set up and initialize an item from your item classes if the user has it enabled in their configuration files.
        /// <para>Additionally, it generates a configuration for each item to allow blacklisting it from AI.</para>
        /// </summary>
        /// <param name="item">A new instance of an ItemBase class."</param>
        /// <param name="itemList">The list you would like to add this to if it passes the config check.</param>
        public bool ValidateItem(ItemBase item, List<ItemBase> itemList)
        {
            var enabled = Config.Bind<bool>("Item: " + item.ItemName, "Enable Item?", true, "Should this item appear in runs?").Value;
            var aiBlacklist = Config.Bind<bool>("Item: " + item.ItemName, "Blacklist Item from AI Use?", false, "Should the AI not be able to obtain this item?").Value;
            if (enabled)
            {
                itemList.Add(item);
                if (aiBlacklist)
                {
                    item.AIBlacklisted = true;
                }
            }
            return enabled;
        }

        private void CollectBodies(CharacterBody body)
        {
            CharBodies.Add(body);
            foreach (ItemBase ib in Items)
            {
                ib.OnBodyAdded(body);
            }
        }

        private void DestroyBodies(CharacterBody body)
        {
            int index = 0;
            for (int i = CharBodies.Count - 1; i >= 0; i--)
            {
                if (CharBodies[i] == body)
                {
                    CharBodies.RemoveAt(i);
                    index = i;
                    break;
                }
            }
            foreach (ItemBase ib in Items)
            {
                ib.OnBodyRemoved(body, index);
            }
        }

    }
}
