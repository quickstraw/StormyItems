using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace HaloItem.Items
{
    public class CrackedHalo : ItemBase
	{
        private static BuffDef buffDef;

        public override string ItemName => "Cracked Halo";

        public override string ItemLangTokenName => "CRACKED_HALO";

        public override string ItemPickupDesc => "Increases movement speed when in the air.";

        public override string ItemFullDescription => "Being in the air increases your movement speed by <style=cIsUtility>21%</style> <style=cStack>(+21% per stack)</style>.";

        public override string ItemLore => "A fallen angels misfortune is your gain.";

        public override ItemTier Tier => ItemTier.Tier1;

        public override GameObject ItemModel => Assets.mainBundle.LoadAsset<GameObject>("Assets/Import/cracked_halo/crackedhalo.prefab");

        public override Sprite ItemIcon => Assets.mainBundle.LoadAsset<Sprite>("Assets/Import/cracked_halo_icon/CrackedHaloIcon.png");


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
            int haloCount = body.inventory.GetItemCount(ItemDef);
            if(haloCount > 0 && !body.characterMotor.isGrounded)
            {
                args.moveSpeedMultAdd += (0.21f * haloCount);
            }
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
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
