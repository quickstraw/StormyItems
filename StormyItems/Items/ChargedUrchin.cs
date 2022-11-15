using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.Projectile;
using StormyItems.Utils;
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
                        bool damagedByOther = damageInfo.attacker.GetComponent<HealthComponent>().body != self.body; // If you didn't damage yourself...
                        bool damagedByNonShock = damageInfo.damageType != DamageType.Shock5s; // If the damage isn't shock (not from another urchin)...
                        if (damagedByOther && damagedByNonShock)
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
                }
            } catch(NullReferenceException e)
            {

            }
            orig(self, damageInfo);
        }

        /// <summary>
        /// Give ItemDisplays to characters. Currently doesn't work for some reason.
        /// </summary>
        /// <returns></returns>
        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            var displayPrefab = ItemModel;//Assets.MainAssets.LoadAsset<GameObject>("Assets/Import/cracked_halo/crackedhalo_display.prefab");
            //ItemBodyModelPrefab = ItemModel;
            var itemDisplay = displayPrefab.AddComponent<ItemDisplay>();
            itemDisplay.rendererInfos = ItemHelpers.ItemDisplaySetup(displayPrefab);

            //var itemDisplay2 = ItemModel.AddComponent<ItemDisplay>();
            //itemDisplay2.rendererInfos = ItemHelpers.ItemDisplaySetup(ItemModel);


            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
            rules.Add("mdlCommandoDualies", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "UpperArmR",
                    localPos = new Vector3(-0.11256F, 0.1353F, 0.00072F),
                    localAngles = new Vector3(27.63244F, 5.54006F, 94.413F),
                    localScale = new Vector3(0.07F, 0.07F, 0.07F)
                }
            });
            rules.Add("mdlHuntress", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "UpperArmR",
                    localPos = new Vector3(-0.08651F, 0.15199F, -0.01627F),
                    localAngles = new Vector3(272.1979F, 159.2721F, 269.0122F),
                    localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlBandit2", new RoR2.ItemDisplayRule[]
{
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "LowerArmL",
                    localPos = new Vector3(-0.05566F, 0.08409F, -0.03757F),
                    localAngles = new Vector3(358.3224F, 323.3761F, 87.74458F),
                    localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlToolbot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "UpperArmR",
                    localPos = new Vector3(0.5327F, 1.55394F, 0.04764F),
                    localAngles = new Vector3(354.8191F, 358.4659F, 270.0213F),
                    localScale = new Vector3(0.5F, 0.5F, 0.5F)
                }
            });
            rules.Add("mdlEngi", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "CannonHeadR",
                    localPos = new Vector3(0.03931F, 0.29354F, 0.21703F),
                    localAngles = new Vector3(90F, 0F, 0F),
                    localScale = new Vector3(0.07F, 0.07F, 0.07F)
                }
            });
            rules.Add("mdlMage", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "LowerArmR",
                    localPos = new Vector3(0.01799F, 0.29124F, -0.00729F),
                    localAngles = new Vector3(8.62532F, 1.91588F, 8.35728F),
                    localScale = new Vector3(0.07F, 0.07F, 0.07F)
                }
            });
            rules.Add("mdlMerc", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "UpperArmR",
                    localPos = new Vector3(-0.18407F, 0.03359F, -0.07675F),
                    localAngles = new Vector3(27.47348F, 328.8876F, 108.735F),
                    localScale = new Vector3(0.07F, 0.07F, 0.07F)
                }
            });
            rules.Add("mdlTreebot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "FlowerBase",
                    localPos = new Vector3(0.93083F, -0.08649F, -0.70011F),
                    localAngles = new Vector3(342.0522F, 21.7778F, 313.0658F),
                    localScale = new Vector3(0.15F, 0.15F, 0.15F)
                }
            });
            rules.Add("mdlLoader", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "UpperArmR",
                    localPos = new Vector3(-0.07621F, 0.21813F, -0.03763F),
                    localAngles = new Vector3(335.0159F, 323.6973F, 83.55108F),
                    localScale = new Vector3(0.07F, 0.07F, 0.07F)
                }
            });
            rules.Add("mdlCroco", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "UpperArmR",
                    localPos = new Vector3(1.67884F, 0.90734F, 0.70363F),
                    localAngles = new Vector3(4.32716F, 328.3336F, 280.9079F),
                    localScale = new Vector3(0.6F, 0.6F, 0.6F)
                }
            });
            rules.Add("mdlCaptain", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "UpperArmR",
                    localPos = new Vector3(-0.04629F, -0.06605F, 0.1296F),
                    localAngles = new Vector3(294.6761F, 239.1639F, 244.1749F),
                    localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlRailGunner", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Backpack",
                    localPos = new Vector3(-0.36075F, 0.22185F, -0.00393F),
                    localAngles = new Vector3(0.01459F, 357.371F, 90.31775F),
                    localScale = new Vector3(0.06F, 0.06F, 0.06F)
                }
            });
            rules.Add("mdlVoidSurvivor", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ForeArmL",
                    localPos = new Vector3(0.05907F, 0.22405F, 0.01742F),
                    localAngles = new Vector3(359.5845F, 1.88131F, 282.4504F),
                    localScale = new Vector3(0.06F, 0.06F, 0.06F)
                }
            });

            return rules;
        }

    }
}
