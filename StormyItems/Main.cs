﻿using BepInEx;
using StormyItems.Items;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

namespace StormyItems
{
    //This attribute specifies that we have a dependency on R2API, as we're using it to add our items to the game.
    //You don't need this if you're not using R2API in your plugin, it's just to tell BepInEx to initialize R2API before this plugin so it's safe to use R2API.
    [BepInDependency(R2API.R2API.PluginGUID)]

    //This attribute is required, and lists metadata for your plugin.
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]

    //This is the main declaration of our plugin class. BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    //BaseUnityPlugin itself inherits from MonoBehaviour, so you can use this as a reference for what you can declare and use in your plugin class: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    public class Main : BaseUnityPlugin
    {
        public static PluginInfo PInfo { get; private set; }

        //The Plugin GUID should be a unique ID for this plugin, which is human readable (as it is used in places like the config).
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Quickstraw";
        public const string PluginName = "StormyItems";
        public const string PluginVersion = "0.9.11";

        public static List<CharacterBody> CharBodies = new List<CharacterBody>();
        public static List<bool> IsGrounded = new List<bool>();

        public static List<ItemBase> Items;

        public void Start()
        {
            Assets.Init();
            Assets.Start();

            //This section automatically scans the project for all items
            var ItemTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ItemBase)));
            var tempItems = new List<ItemBase>();
            foreach (var itemType in ItemTypes)
            {
                ItemBase item = (ItemBase)System.Activator.CreateInstance(itemType);
                tempItems.Add(item);
            }
            foreach(var item in tempItems)
            {
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
            On.RoR2.Items.ContagiousItemManager.Init += AddVoidItemsToDict;

            On.RoR2.CharacterMotor.OnLanded += CharacterMotor_OnLanded;
            On.RoR2.CharacterMotor.OnLeaveStableGround += CharacterMotor_OnLeaveStableGround;

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
                //Log.LogError(e.StackTrace);
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
            if (item.Tier == ItemTier.NoTier)
            {
                itemList.Add(item);
                return true;
            }

            var enabled = Config.Bind<bool>("Item: " + item.ItemName, "Enable Item?", true, "Should this item appear in runs?").Value;
            
            bool defValue = false;
            // Blacklist default:
            if (item.ItemLangTokenName == "CHARGED_URCHIN") {
                defValue = true;
            }

            var aiBlacklist = Config.Bind<bool>("Item: " + item.ItemName, "Blacklist Item from AI Use?", defValue, "Should the AI not be able to obtain this item?").Value;

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
            bool grounded = true;
            if(body.characterMotor != null)
            {
                grounded = body.characterMotor.isGrounded;
            }
            IsGrounded.Add(grounded);
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
                    IsGrounded.RemoveAt(i);
                    index = i;
                    break;
                }
            }
            foreach (ItemBase ib in Items)
            {
                ib.OnBodyRemoved(body, index);
            }
        }

        // Hooks for "in the air" items
        private void CharacterMotor_OnLeaveStableGround(On.RoR2.CharacterMotor.orig_OnLeaveStableGround orig, CharacterMotor self)
        {
            self.body.MarkAllStatsDirty();
            orig(self);
        }

        private void CharacterMotor_OnLanded(On.RoR2.CharacterMotor.orig_OnLanded orig, CharacterMotor self)
        {
            self.body.MarkAllStatsDirty();
            orig(self);
        }

        private void AddVoidItemsToDict(On.RoR2.Items.ContagiousItemManager.orig_Init orig)
        {
            List<ItemDef.Pair> newVoidPairs = new List<ItemDef.Pair>();
            Log.LogMessage("Adding Void Pairs...");
            foreach (var item in Items)
            {
                if (item.RequiresSOTV)
                {
                    item.AddVoidPair(newVoidPairs);
                }
            }
            var key = DLC1Content.ItemRelationshipTypes.ContagiousItem;
            Log.LogMessage(key);
            var voidPairs = ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem];
            Log.LogMessage("Finishing Void Pairs...");
            ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem] = voidPairs.Union(newVoidPairs).ToArray();

            orig();
        }

    }
}
