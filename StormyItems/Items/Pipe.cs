using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StormyItems.Items
{
    public class Pipe : ItemBase
    {
        public override string ItemName => "Antique Pipe";

        public override string ItemLangTokenName => "ANTIQUE_PIPE";

        public override string ItemPickupDesc => "This is a pipe.";

        public override string ItemFullDescription => "This is really a pipe.";

        public override string ItemLore => "\"Where'd my pipe go?\"";

        public override ItemTier Tier => ItemTier.Tier2;

        public override GameObject ItemModel => Assets.MainAssets.LoadAsset<GameObject>("Assets/Import/Pipe/Pipe.prefab");

        public override Sprite ItemIcon => Assets.MainAssets.LoadAsset<Sprite>("Assets/Import/Pipe/PipeIcon.png");

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void StartInit(ConfigFile config)
        {
            CreateLang();
            CreateItem();

            Hooks();
        }

        private void Hooks()
        {

        }
    }
}
