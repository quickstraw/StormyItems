using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StormyItems.Items
{
    class SharpAnchor : ItemBase
    {
        public override string ItemName => "Sharp Anchor";

        public override string ItemLangTokenName => "SHARP_ANCHOR";

        public override string ItemPickupDesc => "Deal bonus damage when standing still.";

        public override string ItemFullDescription => "Increase damage by <style=cIsDamage>20%</style> <style=cIsStack>(+20% per stack)</style> while standing still.";

        public override string ItemLore => "\"Cut the anchor loose! We'll never make it out in time!\"";

        public override ItemTier Tier => ItemTier.Tier1;

        public override GameObject ItemModel => Assets.MainAssets.LoadAsset<GameObject>("Assets/Import/SharpAnchor/sharp_anchor/SharpAnchor.prefab");

        public override Sprite ItemIcon => Assets.MainAssets.LoadAsset<Sprite>("Assets/Import/SharpAnchor/sharp_anchor_icon/SharpAnchorIcon.png");

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Init(ConfigFile config)
        {
            CreateLang();
            CreateItem();

            RecalculateStatsAPI.GetStatCoefficients += OnGetStatCoefficients;
        }

        
        private void OnGetStatCoefficients(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (body != null && args != null && body.inventory != null)
            {
                int anchorCount = GetCount(body);
                if (anchorCount > 0 && body.GetNotMoving())
                {
                    args.damageMultAdd += 0.2f * anchorCount;
                }
            }
        }

        public override void OnFixedUpdate()
        {
            if (PlayerCharacterMasterController.instances.Count > 0 && PlayerCharacterMasterController.instances[0].master.GetBody() != null)
            {
                CharacterBody currChar = PlayerCharacterMasterController.instances[0].master.GetBody();
                int anchorCount = currChar.inventory.GetItemCount(ItemDef.itemIndex);


            }
        }

    }
}
