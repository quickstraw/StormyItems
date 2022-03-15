using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using StormyItems.Utils;


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

        public override GameObject ItemModel => Assets.MainAssets.LoadAsset<GameObject>("Assets/Import/cracked_halo/crackedhalo.prefab");

        public override Sprite ItemIcon => Assets.MainAssets.LoadAsset<Sprite>("Assets/Import/cracked_halo_icon/CrackedHaloIcon.png");


        //Call Init() in main class
        public override void Init(ConfigFile config)
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

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = Assets.MainAssets.LoadAsset<GameObject>("Assets/Import/cracked_halo/crackedhalo_display.prefab");
            //ItemBodyModelPrefab = ItemModel;
            var itemDisplay = ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            //itemDisplay.rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

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
                    localPos = new Vector3(0,0,1.0f),
                    localAngles = new Vector3(0,0,0),
                    localScale = new Vector3(2f,2f,2f)
                }
            });
            rules.Add("mdlHuntress", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0,0,0),
                    localAngles = new Vector3(0,0,0),
                    localScale = new Vector3(0.3f,0.3f,0.3f)
                }
            });
            rules.Add("mdlToolbot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0,0,0),
                    localAngles = new Vector3(0,0,0),
                    localScale = new Vector3(0.3f,0.3f,0.3f)
                }
            });
            rules.Add("mdlEngi", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0,0,0),
                    localAngles = new Vector3(0,0,0),
                    localScale = new Vector3(0.3f,0.3f,0.3f)
                }
            });
            rules.Add("mdlMerc", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0,0,0),
                    localAngles = new Vector3(0,0,0),
                    localScale = new Vector3(0.3f,0.3f,0.3f)
                }
            });
            rules.Add("mdlTreebot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0,0,0),
                    localAngles = new Vector3(0,0,0),
                    localScale = new Vector3(0.3f,0.3f,0.3f)
                }
            });
            rules.Add("mdlLoader", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0,0,0),
                    localAngles = new Vector3(0,0,0),
                    localScale = new Vector3(0.3f,0.3f,0.3f)
                }
            });
            rules.Add("mdlCroco", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0,0,0),
                    localAngles = new Vector3(0,0,0),
                    localScale = new Vector3(0.3f,0.3f,0.3f)
                }
            });
            rules.Add("mdlCaptain", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0,0,0),
                    localAngles = new Vector3(0,0,0),
                    localScale = new Vector3(0.3f,0.3f,0.3f)
                }
            });
            rules.Add("mdlBandit2", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0,0,0),
                    localAngles = new Vector3(0,0,0),
                    localScale = new Vector3(0.3f,0.3f,0.3f)
                }
            });

            return rules;
        }

        public override void OnUpdate()
        {
            //This if statement checks if the player has currently pressed F2.
            if (Input.GetKeyDown(KeyCode.F2))
            {
                //Get the player body to use a position:	
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                //And then drop our defined item in front of the player.

                Log.LogInfo($"Player pressed F2. Spawning our custom item at coordinates {transform.position}");
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemDef.itemIndex), transform.position, transform.forward * 20f);
            }
        }

        public override void OnFixedUpdate()
        {
            ProvideBuff();
        }

        private void ProvideBuff()
        {
            if(PlayerCharacterMasterController.instances.Count > 0&& PlayerCharacterMasterController.instances[0].master.GetBody() != null)
            {
                CharacterBody currChar = PlayerCharacterMasterController.instances[0].master.GetBody();
                int haloCount = currChar.inventory.GetItemCount(ItemDef.itemIndex);

                if (haloCount > 0 && !currChar.characterMotor.isGrounded)
                {
                    currChar.AddBuff(buffDef);

                }

                if (currChar.HasBuff(buffDef))
                {
                    if (haloCount <= 0 || currChar.characterMotor.isGrounded)
                    {
                        // Set dirty bit to recalculate stats
                        currChar.SetDirtyBit(1);
                        currChar.RemoveBuff(buffDef);
                    }
                }
            }
        }
    }
}
