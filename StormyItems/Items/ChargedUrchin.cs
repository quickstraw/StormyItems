using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace StormyItems.Items
{
    class ChargedUrchin : ItemBase
    {
        public override string ItemName => "Charged Urchin";

        public override string ItemLangTokenName => "CHARGED_URCHIN";

        public override string ItemPickupDesc => "While you have a shield, shock enemies on taking damage.";

        public override string ItemFullDescription => "Gain a <style=cIsHealth>shield</style> equal to <style=cIsHealth>10%</style> of your maximum health. While you have a shield, taking damage <style=cIsDamage>shocks</style> enemies for <style=cIsDamage>80%</style> <style=cStack>(+80% per stack)</style> damage.";
                                                      //"Gain a <style=cIsHealth>shield</style> equal to <style=cIsHealth>4%</style> of your maximum health. While you have a shield, taking damage fires <style=cIsDamage>lightning</style> for <style=cIsDamage>80%</style> <style=cStack>(+80% per stack)</style> damage taken on a target within <style=cIsDamage>20m</style> <style=cStack>(+2m per stack)</style>.";

        public override string ItemLore => "\"They can't hurt you anymore. They're all gone.\"";

        public override ItemTier Tier => ItemTier.Tier2;

        public override ItemTag[] ItemTags { get; set; } = new ItemTag[] { ItemTag.Damage, ItemTag.Utility };

        public override GameObject ItemModel => Assets.MainAssets.LoadAsset<GameObject>("Assets/Import/ChargedUrchin/ChargedUrchin.prefab");

        public override Sprite ItemIcon => Assets.MainAssets.LoadAsset<Sprite>("Assets/Import/ChargedUrchin/ChargedUrchinIcon.png");

        public GameObject ProjectilePrefab => Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ChainLightning/ChainLightningOrbEffect.prefab").WaitForCompletion();

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
            RecalculateStatsAPI.GetStatCoefficients += Shield;
        }

        /// <summary>
        /// Methods hooks in to RecalculateStats and gives holders a shield.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Shield(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if(GetCount(sender) > 0)
            {
                args.baseShieldAdd += sender.maxHealth * 0.10f;
            }
        }

        /// <summary>
        /// When an item holder is hit and has a shield, retaliate with a shock attack.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="damageInfo"></param>
        private void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo damageInfo)
        {
            try
            {
                int count = GetCount(self.body);
                if (count > 0 && self && self.shield > 0)
                {
                    //float sDamage = damageInfo.damage * count *  0.8f;
                    float sDamage = self.body.damage * count * 0.8f;
                    if (damageInfo.attacker && damageInfo.attacker.GetComponent<HealthComponent>())
                    {
                        // Create a shock attack DamageInfo -- Want to replace this with a chain lightning effect.
                        var ShockAttack = new DamageInfo()
                        {
                            attacker = self.body.gameObject,
                            crit = self.body.RollCrit(),
                            damage = sDamage,
                            damageType = DamageType.Shock5s,
                            procCoefficient = 1
                        };
                        damageInfo.attacker.GetComponent<HealthComponent>().TakeDamage(ShockAttack);
                    }
                }
            } catch(NullReferenceException e)
            {

            }
            orig(self, damageInfo);
        }
    }
}
