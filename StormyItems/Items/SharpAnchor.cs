using BepInEx.Configuration;
using On.RoR2;
using R2API;
using RoR2;
using StormyItems.Materials;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace StormyItems.Items
{
    class SharpAnchor : ItemBase
    {
        public override string ItemName => "Sharp Anchor";

        public override string ItemLangTokenName => "SHARP_ANCHOR";

        public override string ItemPickupDesc => "Deal bonus damage when standing still.";

        public override string ItemFullDescription => "Increase damage by <style=cIsDamage>20%</style> <style=cStack>(+20% per stack)</style> while standing still.";

        public override string ItemLore => "\"Cut the anchor loose! We'll never make it out in time!\"";

        public override ItemTier Tier => ItemTier.Tier1;

        public override GameObject ItemModel => Assets.MainAssets.LoadAsset<GameObject>("Assets/Import/SharpAnchor/sharp_anchor/SharpAnchor.prefab");

        public override Sprite ItemIcon => Assets.MainAssets.LoadAsset<Sprite>("Assets/Import/SharpAnchor/sharp_anchor_icon/SharpAnchorIcon.png");

        public static GameObject ZonePrefab => Assets.MainAssets.LoadAsset<GameObject>("Assets/Import/SharpAnchor/sharp_anchor_zone/SharpAnchorZone.prefab");

        public static List<GameObject> Zones = new List<GameObject>();
        private List<bool> flags = new List<bool>();

        public static RoR2.BuffDef buffDef;

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void StartInit(ConfigFile config)
        {
            CreateLang();
            CreateItem();

            //Add associated buff
            buffDef = ScriptableObject.CreateInstance<RoR2.BuffDef>();
            buffDef.iconSprite = ItemDef.pickupIconSprite;
            buffDef.name = "Sharp Anchor";
            buffDef.isDebuff = false;
            buffDef.canStack = true;
            buffDef.buffColor = new Color(250, 106, 10);
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
            flags.Add(false);
        }

        public override void OnBodyRemoved(RoR2.CharacterBody body, int index)
        {
            Zones.RemoveAt(index);
            flags.RemoveAt(index);
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
                        RoR2.CharacterBody currChar = Main.CharBodies[i];
                        if (currChar.inventory != null)
                        {
                            int anchorCount = currChar.inventory.GetItemCount(ItemDef.itemIndex);
                            GameObject ZoneObject = Zones[i];
                            if (Zones[i] == null)
                            {
                                if (anchorCount > 0 && currChar.GetNotMoving())
                                {
                                    ZoneObject = UnityEngine.Object.Instantiate<GameObject>(SharpAnchorAssetHelper.Zone, currChar.footPosition, Quaternion.identity);
                                    ZoneObject.AddComponent<RoR2.TeamFilter>().teamIndex = currChar.teamComponent.teamIndex;
                                    var anchorZone = ZoneObject.AddComponent<SharpAnchorZone>();
                                    float networkradius = currChar.radius + 1.0f + 1.0f * anchorCount;
                                    anchorZone.Networkradius = networkradius;
                                    NetworkServer.Spawn(ZoneObject);
                                    Zones[i] = ZoneObject;
                                    flags[i] = true;
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
