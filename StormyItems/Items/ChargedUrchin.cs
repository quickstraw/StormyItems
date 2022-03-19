using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StormyItems.Items
{
    class ChargedUrchin : ItemBase
    {
        public override string ItemName => "Charged Urchin";

        public override string ItemLangTokenName => "CHARGED_URCHIN";

        public override string ItemPickupDesc => "While you have a shield, shock enemies on taking damage";

        public override string ItemFullDescription => "Gain a <style=cIsHealth>shield</style> equal to <style=cIsHealth>4%</style> of your maximum health. While you have a shield, taking damage fires <style=cIsDamage>lightning</style> for <style=cIsDamage>80%</style> <style=cStack>(+80% per stack)</style> damage taken on a target within <style=cIsDamage>20m</style> <style=cStack>(+2m per stack)</style>.";

        public override string ItemLore => "\"They can't hurt you anymore. They're all gone.\"";

        public override ItemTier Tier => ItemTier.Tier2;

        public override ItemTag[] ItemTags { get; set; } = new ItemTag[] { ItemTag.Damage, ItemTag.Utility };

        public override GameObject ItemModel => Assets.MainAssets.LoadAsset<GameObject>("Assets/Import/ChargedUrchin/ChargedUrchin.prefab");

        public override Sprite ItemIcon => Assets.MainAssets.LoadAsset<Sprite>("Assets/Import/ChargedUrchin/ChargedUrchinIcon.png");

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
            On.RoR2.HealthComponent.TakeDamage += OnTakeDamage;
        }

        private void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo damageInfo)
        {
            int count = GetCount(self.body);
            if(count > 0 && self.shield > 0)
            {
                float damage = damageInfo.damage;
            }
            orig(self, damageInfo);
        }
    }
}
