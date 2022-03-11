using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StormyItems.Items
{
    class RustyAnchor : ItemBase
    {
        public override string ItemName => "Rusty Anchor";

        public override string ItemLangTokenName => "RUSTY_ANCHOR";

        public override string ItemPickupDesc => "Reduce incoming damage when standing still.";

        public override string ItemFullDescription => "Increase armor by 60 while standing still.";

        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier1;

        public override GameObject ItemModel => Assets.MainAssets.LoadAsset<GameObject>("Assets/Import/rusty_anchor/RustyAnchor.prefab");

        public override Sprite ItemIcon => Assets.MainAssets.LoadAsset<Sprite>("Assets/Import/rusty_anchor_icon/RustyAnchorIcon.png");

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Init(ConfigFile config)
        {
            CreateLang();
            CreateItem();
        }
    }
}
