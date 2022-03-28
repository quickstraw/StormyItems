using BepInEx.Configuration;
using R2API;
using RoR2;
using StormyItems.AssetHelpers;
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

        public override GameObject ItemModel => DroneCoolantAssetHelper.PickupPrefab;

        public override Sprite ItemIcon => Assets.MainAssets.LoadAsset<Sprite>("Assets/Import/DroneCoolant/DroneCoolantIcon.png");

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            var itemDisplay = Assets.MainAssets.LoadAsset<GameObject>("Assets/Import/DroneCoolant/DroneCoolantBoostDisplay.prefab");

            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
            rules.Add("mdlDrone1", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = itemDisplay,
                    childName = "Muzzle",
                    localPos = new Vector3(0.02901F, -0.30189F, -0.58608F),
                    localAngles = new Vector3(318.3587F, 90F, 90F),
                    localScale = new Vector3(0.15F, 0.15F, 0.15F)
                }
            });
            rules.Add("mdlDrone2", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = itemDisplay,
                    childName = "Muzzle",
                    localPos = new Vector3(0F, -0.15161F, -0.34076F),
                    localAngles = new Vector3(0F, 0F, 270F),
                    localScale = new Vector3(0.15F, 0.15F, 0.15F)
                }
            });
            rules.Add("mdlDroneCommander", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = itemDisplay,
                    childName = "HeadCenter",
                    localPos = new Vector3(-0.22237F, -0.01784F, -0.20124F),
                    localAngles = new Vector3(270F, 308.2981F, 0F),
                    localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlEmergencyDrone", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = itemDisplay,
                    childName = "Muzzle",
                    localPos = new Vector3(-0.00521F, -0.55555F, 0.62276F),
                    localAngles = new Vector3(335.345F, 0F, 0F),
                    localScale = new Vector3(0.15F, 0.15F, 0.15F)
                }
            });
            rules.Add("mdlEquipmentDrone", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = itemDisplay,
                    childName = "GunBarrelBase",
                    localPos = new Vector3(-0.00001F, 0F, 0.80601F),
                    localAngles = new Vector3(7.43213F, 333.8963F, 358.2831F),
                    localScale = new Vector3(0.15F, 0.15F, 0.15F)
                }
            });
            rules.Add("mdlFlameDrone", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = itemDisplay,
                    childName = "MuzzleLeft",
                    localPos = new Vector3(0.22367F, 0.0222F, 0F),
                    localAngles = new Vector3(90F, 0F, 0F),
                    localScale = new Vector3(0.15F, 0.15F, 0.15F)
                }
            });
            rules.Add("mdlMegaDrone", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = itemDisplay,
                    childName = "GatLeft",
                    localPos = new Vector3(-0.03504F, 0.45371F, 0.37431F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.15F, 0.15F, 0.15F)
                }
            });
            rules.Add("mdlMegaDrone", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = itemDisplay,
                    childName = "GatRight",
                    localPos = new Vector3(-0.03504F, 0.45371F, 0.37431F),
                    localAngles = new Vector3(-0.00001F, 82.04089F, -0.00001F),
                    localScale = new Vector3(0.15F, 0.15F, 0.15F)
                }
            });
            rules.Add("mdlMissileDrone", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = itemDisplay,
                    childName = "MissilePack",
                    localPos = new Vector3(-0.71858F, 0.58865F, -0.01419F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.15F, 0.15F, 0.15F)
                }
            });





            return rules;
        }

        public override void StartInit(ConfigFile config)
        {
            CreateLang();
            CreateItem();

            Hooks();
        }

        private void Hooks()
        {
            GlobalEventManager.onServerDamageDealt += OnServerDamageDealt;
            RecalculateStatsAPI.GetStatCoefficients += OnRecalculateStats;
        }

        private void OnServerDamageDealt(DamageReport damageReport)
        {
            CharacterBody inflictor = damageReport.attackerBody;
            CharacterBody victim = damageReport.victimBody;

            if(inflictor && victim)
            {
                int count = GetCount(inflictor);
                if(count > 0 && inflictor.master)
                {
                    var odds = 20.0f;
                    var isProcced = Util.CheckRoll(odds, inflictor.master);
                    if (isProcced)
                    {
                        var damageInfo = damageReport.damageInfo;

                        float baseDamage = inflictor.baseDamage;
                        float damageMult = 0.5f + 0.5f * count;
                        InflictDotInfo inflictDotInfo = default(InflictDotInfo);
                        inflictDotInfo.attackerObject = damageInfo.attacker;
                        inflictDotInfo.victimObject = victim.gameObject;
                        inflictDotInfo.totalDamage = baseDamage * damageMult;
                        inflictDotInfo.damageMultiplier = 1f;
                        inflictDotInfo.dotIndex = DotController.DotIndex.Burn;
                        inflictDotInfo.maxStacksFromAttacker = uint.MaxValue;
                        InflictDotInfo dotInfo = inflictDotInfo;
                        //StrengthenBurnUtils.CheckDotForUpgrade(inflictor.inventory, ref dotInfo);
                        DotController.InflictDot(ref dotInfo);
                    }
                }
            }
        }

        private void OnRecalculateStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetCount(sender);
            if(count > 0)
            {
                args.attackSpeedMultAdd += 0.1f * count;
                //args.critAdd += 10.0f * count;
            }
        }

        
    }
}
