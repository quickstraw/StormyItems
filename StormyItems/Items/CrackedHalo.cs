using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using StormyItems.Utils;
using UnityEngine.Networking;

namespace StormyItems.Items
{
    public class CrackedHalo : ItemBase
	{
        public static GameObject ItemBodyModelPrefab;
        private static BuffDef buffDef;

        public override string ItemName => "Cracked Halo";

        public override string ItemLangTokenName => "CRACKED_HALO";

        public override string ItemPickupDesc => "Increases movement speed when in the air.";

        public override string ItemFullDescription => "Being in the air increases your movement speed by <style=cIsUtility>25%</style> <style=cStack>(+25% per stack)</style>.";

        public override string ItemLore => "A fallen angels misfortune is your gain.";

        public override ItemTier Tier => ItemTier.Tier1;

        public override ItemTag[] ItemTags { get; set; } = new ItemTag[] { ItemTag.Utility };

        public override GameObject ItemModel => Assets.MainAssets.LoadAsset<GameObject>("Assets/Import/cracked_halo/crackedhalo.prefab");

        public override Sprite ItemIcon => Assets.MainAssets.LoadAsset<Sprite>("Assets/Import/cracked_halo_icon/CrackedHaloIcon.png");


        //Call Init() in main class
        public override void StartInit(ConfigFile config)
        {
            CreateLang();
            CreateItem();

            //Add associated buff
            buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.iconSprite = ItemDef.pickupIconSprite;
            buffDef.name = "Cracked Halo";
            buffDef.isDebuff = false;
            buffDef.canStack = false;
            buffDef.buffColor = new Color(246, 255, 71);
            ContentAddition.AddBuffDef(buffDef);

            //But now we have defined an item, but it doesn't do anything yet. So we'll need to define that ourselves.
            RecalculateStatsAPI.GetStatCoefficients += OnGetStatCoefficients;
        }

        /// <summary>
        /// If an item holder is not on the ground, give it a speed boost.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="args"></param>
        private void OnGetStatCoefficients(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if(body != null && args != null && body.inventory != null)
            {
                int haloCount = body.inventory.GetItemCount(ItemDef);
                if (haloCount > 0 && body.characterMotor != null && !body.characterMotor.isGrounded)
                {
                    args.moveSpeedMultAdd += (0.25f * haloCount);
                }
            }
        }

        /// <summary>
        /// Give ItemDisplays to characters. Currently doesn't work for some reason.
        /// </summary>
        /// <returns></returns>
        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = ItemModel;//Assets.MainAssets.LoadAsset<GameObject>("Assets/Import/cracked_halo/crackedhalo_display.prefab");
            //ItemBodyModelPrefab = ItemModel;
            var itemDisplay = ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            itemDisplay.rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            //var itemDisplay2 = ItemModel.AddComponent<ItemDisplay>();
            //itemDisplay2.rendererInfos = ItemHelpers.ItemDisplaySetup(ItemModel);
            

            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
            rules.Add("mdlCommandoDualies", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0,0.6f,-0.05f),
                    localAngles = new Vector3(0,0,15f),
                    localScale = new Vector3(0.1f,0.1f,0.1f)
                }
            });
            rules.Add("mdlHuntress", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0,0.5f,-0.05f),
                    localAngles = new Vector3(0,0,15f),
                    localScale = new Vector3(0.1f,0.1f,0.1f)
                }
            });
            rules.Add("mdlBandit2", new RoR2.ItemDisplayRule[]
{
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0,0.33f,0.0f),
                    localAngles = new Vector3(0,0,20f),
                    localScale = new Vector3(0.1f,0.1f,0.1f)
                }
});
            rules.Add("mdlToolbot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.083f,3.27f,2.1f),
                    localAngles = new Vector3(303f,160f,31f),
                    localScale = new Vector3(0.5f,0.5f,0.5f)
                }
            });
            rules.Add("mdlEngi", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HeadCenter",
                    localPos = new Vector3(0F, 0.375F, 0.041F),
                    localAngles = new Vector3(7.96373F, 359.8193F, 14.61625F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlMage", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0,0.3f,-0.055f),
                    localAngles = new Vector3(0,0,15f),
                    localScale = new Vector3(0.08f,0.08f,0.08f)
                }
            });
            rules.Add("mdlMerc", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0,0.41f,0.0f),
                    localAngles = new Vector3(0,0,15f),
                    localScale = new Vector3(0.1f,0.1f,0.1f)
                }
            });
            rules.Add("mdlTreebot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "FlowerBase",
                    localPos = new Vector3(0,2.05f,-0.07f),
                    localAngles = new Vector3(0,0,15f),
                    localScale = new Vector3(0.2f,0.2f,0.2f)
                }
            });
            rules.Add("mdlLoader", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0,0.4f,0.0f),
                    localAngles = new Vector3(0,0,15f),
                    localScale = new Vector3(0.1f,0.1f,0.1f)
                }
            });
            rules.Add("mdlCroco", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0.01f,0.45f,3.05f),
                    localAngles = new Vector3(65.5f,201f,50f),
                    localScale = new Vector3(1f,1f,1f)
                }
            });
            rules.Add("mdlCaptain", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0,0.4f,0.0f),
                    localAngles = new Vector3(0,0,15f),
                    localScale = new Vector3(0.1f,0.1f,0.1f)
                }
            });
            rules.Add("mdlRailGunner", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0,0.3f,-0.037f),
                    localAngles = new Vector3(350,358.6f,15.5f),
                    localScale = new Vector3(0.1f,0.1f,0.1f)
                }
            });
            rules.Add("mdlVoidSurvivor", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.04569f, 0.21095f, -0.19764f),
                    localAngles = new Vector3(318.3497f, 338.3792f, 35.74068f),
                    localScale = new Vector3(0.1f, 0.1f, 0.1f)
                }
            });

            return rules;
        }

        public override void OnFixedUpdate()
        {
            if (NetworkServer.active)
            {
                ProvideBuff();
            }
        }

        private void ProvideBuff()
        {
            if(!(PlayerCharacterMasterController.instances.Count > 0 && PlayerCharacterMasterController.instances[0].master.GetBody() != null))
            {
                return;
            }
            for(int i = 0; i < Main.CharBodies.Count; i++)
            {
                CharacterBody currChar = Main.CharBodies[i];

                if(!currChar || !currChar.inventory || !currChar.characterMotor)
                {
                    continue;
                }

                int haloCount = GetCount(currChar);

                if (haloCount > 0 && !currChar.characterMotor.isGrounded)
                {
                    currChar.AddBuff(buffDef);
                }

                if (currChar.HasBuff(buffDef))
                {
                    if (haloCount <= 0 || currChar.characterMotor.isGrounded)
                    {
                        // Set dirty bit to recalculate stats
                        currChar.MarkAllStatsDirty();
                        currChar.RemoveBuff(buffDef);
                    }
                }
            }
        }
    }
}
