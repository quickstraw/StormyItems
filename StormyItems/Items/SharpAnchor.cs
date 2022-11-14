using BepInEx.Configuration;
using On.RoR2;
using R2API;
using RoR2;
using StormyItems.AssetHelpers;
using StormyItems.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using ItemDisplay = RoR2.ItemDisplay;

namespace StormyItems.Items
{
    class SharpAnchor : ItemBase
    {
        public override string ItemName => "Sharp Anchor";

        public override string ItemLangTokenName => "SHARP_ANCHOR";

        public override string ItemPickupDesc => "Increases damage of all nearby allies after standing still for 1 second.";

        public override string ItemFullDescription => "After standing still for <style=cIsDamage>1</style> second, create a zone that increases damage by <style=cIsDamage>20%</style> <style=cStack>(+20% per stack)</style> for all allies within <style=cIsDamage>2.3m</style> <style=cStack>(+1.0m per stack)</style>.";

        public override string ItemLore => "\"Cut the anchor loose! We'll never make it out in time!\"";

        public override ItemTier Tier => ItemTier.Tier1;

        public override ItemTag[] ItemTags { get; set; } = new ItemTag[] { ItemTag.Damage };

        public override GameObject ItemModel => Assets.MainAssets.LoadAsset<GameObject>("Assets/Import/SharpAnchor/sharp_anchor/SharpAnchor.prefab");

        public override Sprite ItemIcon => Assets.MainAssets.LoadAsset<Sprite>("Assets/Import/SharpAnchor/sharp_anchor_icon/SharpAnchorIcon.png");

        public override Sprite BuffIcon => Assets.MainAssets.LoadAsset<Sprite>("Assets/Import/SharpAnchor/sharp_anchor_icon/SharpAnchorBuffIcon.png");

        public static GameObject ZonePrefab => Assets.MainAssets.LoadAsset<GameObject>("Assets/Import/SharpAnchor/sharp_anchor_zone/SharpAnchorZone.prefab");

        public static List<GameObject> Zones = new List<GameObject>();

        public static RoR2.BuffDef buffDef;

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
                    childName = "Pelvis",
                    localPos = new Vector3(0.12004F, 0.02228F, 0.09967F),
                    localAngles = new Vector3(357.7524F, 45.265F, 122.3607F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlHuntress", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(0.13084F, 0.00471F, 0.10742F),
                    localAngles = new Vector3(349.1329F, 51.52035F, 155.9152F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlBandit2", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(-0.09031F, 0.32003F, 0.03123F),
                    localAngles = new Vector3(327.4148F, 102.8616F, 178.5478F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlToolbot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Hip",
                    localPos = new Vector3(-1.12871F, 0.86596F, 0F),
                    localAngles = new Vector3(342.5284F, 67.11109F, 235.8252F),
                    localScale = new Vector3(1F, 1F, 1F)
                }
            });
            rules.Add("mdlEngi", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(-0.20425F, 0.13367F, 0.05242F),
                    localAngles = new Vector3(335.8164F, 299.6544F, 181.8474F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlMage", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ClavicleL",
                    localPos = new Vector3(-0.26977F, 0.24865F, -0.05668F),
                    localAngles = new Vector3(357.4147F, 72.27007F, 292.9518F),
                    localScale = new Vector3(0.08F, 0.08F, 0.08F)
                }
            });
            rules.Add("mdlMerc", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(0.0713F, 0.13439F, 0.092F),
                    localAngles = new Vector3(356.33F, 37.12303F, 130.2461F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlTreebot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "FlowerBase",
                    localPos = new Vector3(-0.46937F, 0.65393F, -0.50655F),
                    localAngles = new Vector3(11.75772F, 44.64086F, 3.34093F),
                    localScale = new Vector3(0.3F, 0.3F, 0.3F)
                }
            });
            rules.Add("mdlLoader", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "MechBase",
                    localPos = new Vector3(0.14661F, -0.1762F, -0.16444F),
                    localAngles = new Vector3(27.88511F, 12.80222F, 25.72949F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlCroco", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ClavicleR",
                    localPos = new Vector3(-2.56176F, 1.23863F, 1.97781F),
                    localAngles = new Vector3(30.78585F, 230.8304F, 262.4353F),
                    localScale = new Vector3(1F, 1F, 1F)
                }
            });
            rules.Add("mdlCaptain", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Chest",
                    localPos = new Vector3(-0.08804F, 0.19676F, -0.23835F),
                    localAngles = new Vector3(38.01167F, 21.40266F, 31.49897F),
                    localScale = new Vector3(0.3F, 0.3F, 0.3F)
                }
            });
            rules.Add("mdlRailGunner", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(0.11456F, 0.11182F, 0.11737F),
                    localAngles = new Vector3(305.4421F, 226.9561F, 168.5652F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlVoidSurvivor", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ForeArmR",
                    localPos = new Vector3(0.24134F, 0.21545F, -0.0136F),
                    localAngles = new Vector3(303.8591F, 276.0721F, 21.92159F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });

            return rules;
        }

        public override void StartInit(ConfigFile config)
        {
            
            CreateLang();
            CreateItem();

            //Add associated buff
            buffDef = ScriptableObject.CreateInstance<RoR2.BuffDef>();
            buffDef.iconSprite = BuffIcon;
            buffDef.name = "Sharp Anchor";
            buffDef.isDebuff = false;
            buffDef.canStack = true;
            buffDef.buffColor = new Color(229, 114, 177);
            ContentAddition.AddBuffDef(buffDef);

            RecalculateStatsAPI.GetStatCoefficients += OnGetStatCoefficients;
            On.RoR2.CharacterBody.OnBuffFinalStackLost += OnLoseBuff;
        }

        private void OnLoseBuff(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, RoR2.CharacterBody self, RoR2.BuffDef lostBuffDef)
        {
            if(lostBuffDef == buffDef)
            {
                self.MarkAllStatsDirty();
            }
            orig(self, lostBuffDef);
        }

        private void OnGetStatCoefficients(RoR2.CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (body != null && args != null && body.inventory != null)
            {
                if (body.HasBuff(buffDef))
                {
                    args.damageMultAdd += 0.2f * body.buffs[(int)buffDef.buffIndex];
                }
            }
        }

        public override void OnBodyAdded(RoR2.CharacterBody body)
        {
            Zones.Add(null);
        }

        public override void OnBodyRemoved(RoR2.CharacterBody body, int index)
        {
            if(index < Zones.Count)
            {
                if(NetworkServer.active && Zones[index])
                {
                    UnityEngine.Object.Destroy(Zones[index]);
                }
                Zones.RemoveAt(index);
            }
        }

        public override void OnFixedUpdate()
        {
            if (!NetworkServer.active && Main.CharBodies != null && Zones != null)
            {
                return;
            }
            else
            {
                if (Main.CharBodies.Count > 0)
                {
                    for (int i = 0; i < Main.CharBodies.Count; i++)
                    {
                        if(i >= Main.CharBodies.Count || i >= Zones.Count)
                        {
                            continue;
                        }
                        RoR2.CharacterBody currChar = Main.CharBodies[i];
                        if (currChar.inventory != null)
                        {
                            int anchorCount = currChar.inventory.GetItemCount(ItemDef.itemIndex);
                            GameObject ZoneObject = Zones[i];
                            if (Zones[i] == null)
                            {
                                if (anchorCount > 0 && currChar.GetNotMoving())
                                {
                                    ZoneObject = UnityEngine.Object.Instantiate(SharpAnchorAssetHelper.Zone, currChar.footPosition, Quaternion.identity);
                                    //ZoneObject.AddComponent<RoR2.TeamFilter>().teamIndex = currChar.teamComponent.teamIndex;
                                    //var anchorZone = ZoneObject.AddComponent<SharpAnchorZone>();
                                    var teamFilter = ZoneObject.GetComponent<RoR2.TeamFilter>();
                                    teamFilter.teamIndex = currChar.teamComponent.teamIndex;
                                    var anchorZone = ZoneObject.GetComponent<SharpAnchorZone>();
                                    float networkradius = currChar.radius + 1.3f + 1.0f * anchorCount;
                                    anchorZone.teamFilter = teamFilter;
                                    anchorZone.Networkradius = networkradius;
                                    NetworkServer.Spawn(ZoneObject);
                                    Zones[i] = ZoneObject;
                                }
                            }
                            else if (Zones[i] != null)
                            {
                                if (anchorCount <= 0 || !currChar.GetNotMoving())
                                {
                                    UnityEngine.Object.Destroy(ZoneObject);
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
