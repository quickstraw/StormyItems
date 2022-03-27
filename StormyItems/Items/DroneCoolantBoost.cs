using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StormyItems.Items
{
    public class DroneCoolantBoost : ItemBase
    {
        public override string ItemName => "Drone Coolant Boost";

        public override string ItemLangTokenName => "DRONE_COOLANT_BOOST";

        public override string ItemPickupDesc => "";

        public override string ItemFullDescription => "";

        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.NoTier;

        public override GameObject ItemModel => Assets.MainAssets.LoadAsset<GameObject>("Assets/Import/DroneCoolant/DroneCoolant.prefab");

        public override Sprite ItemIcon => Assets.MainAssets.LoadAsset<Sprite>("Assets/Import/DroneCoolant/DroneCoolantIcon.png");

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void StartInit(ConfigFile config)
        {
            CreateLang();
            CreateItem();

            RecalculateStatsAPI.GetStatCoefficients += OnRecalculateStats;
        }

        private void OnRecalculateStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetCount(sender);
            args.attackSpeedMultAdd += .1f * count;
            args.damageMultAdd += 0.14f * count;
        }
    }
}
