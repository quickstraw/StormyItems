using BepInEx;
using HaloItem.Items;
using R2API;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace HaloItem
{
    //This is an example plugin that can be put in BepInEx/plugins/ExamplePlugin/ExamplePlugin.dll to test out.
    //It's a small plugin that adds a relatively simple item to the game, and gives you that item whenever you press F2.

    //This attribute specifies that we have a dependency on R2API, as we're using it to add our item to the game.
    //You don't need this if you're not using R2API in your plugin, it's just to tell BepInEx to initialize R2API before this plugin so it's safe to use R2API.
    [BepInDependency(R2API.R2API.PluginGUID)]

    //This attribute is required, and lists metadata for your plugin.
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    //We will be using 2 modules from R2API: ItemAPI to add our item and LanguageAPI to add our language tokens.
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(LanguageAPI), nameof(RecalculateStatsAPI))]

    //This is the main declaration of our plugin class. BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    //BaseUnityPlugin itself inherits from MonoBehaviour, so you can use this as a reference for what you can declare and use in your plugin class: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    public class Main : BaseUnityPlugin
    {
        public static PluginInfo PInfo { get; private set; }

        //The Plugin GUID should be a unique ID for this plugin, which is human readable (as it is used in places like the config).
        //If we see this PluginGUID as it is on thunderstore, we will deprecate this mod. Change the PluginAuthor and the PluginName !
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Quickstraw";
        public const string PluginName = "CrackedHaloItem";
        public const string PluginVersion = "0.0.1";

        private ItemBase[] items;

        //The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            //Init our logging class so that we can properly log for debugging
            Log.Init(Logger);
            PInfo = Info;
            Assets.Init();

            items = new ItemBase[] {
                // Add Items here
                new CrackedHalo()
            };

            foreach (ItemBase ib in items)
            {
                ib.Init();
            }

            // This line of log will appear in the bepinex console when the Awake method is done.
            Log.LogInfo(nameof(Awake) + " done.");
        }

        //The Update() method is run on every frame of the game.
        private void Update()
        {
            foreach(ItemBase ib in items)
            {
                ib.OnUpdate();
            }
        }

        private void FixedUpdate()
        {
            foreach (ItemBase ib in items)
            {
                ib.OnFixedUpdate();
            }
        }

    }
}
