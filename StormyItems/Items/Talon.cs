using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using StormyItems.Utils;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace StormyItems.Items
{
    public class Talon : ItemBase
	{
        public static GameObject ItemBodyModelPrefab;
        private static BuffDef buffDef;

        public override string ItemName => "Talon";

        public override string ItemLangTokenName => "TALON";

        public override string ItemPickupDesc => "Increases damage when in the air. <style=cIsVoid>Corrupts all Sharp Anchors.</style>";

        public override string ItemFullDescription => "Being in the air increases your damage by <style=cIsDamage>15%</style> <style=cStack>(+15% per stack)</style>. <style=cIsVoid>Corrupts all Sharp Anchors.</style>";

        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.VoidTier1;

        public override ItemTag[] ItemTags { get; set; } = new ItemTag[] { ItemTag.Damage };

        public override GameObject ItemModel => Assets.MainAssets.LoadAsset<GameObject>("Assets/Import/Talon/Talon.prefab");

        public override Sprite ItemIcon => Assets.MainAssets.LoadAsset<Sprite>("Assets/Import/Talon/TalonIcon.png");
        public static List<bool> IsGrounded = new List<bool>();


        //Call Init() in main class
        public override void StartInit(ConfigFile config)
        {
            CreateLang();
            CreateItem();

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
                int count = GetCount(body);
                if (count > 0 && body.characterMotor != null && !body.characterMotor.isGrounded)
                {
                    args.damageMultAdd += (0.15f * count);
                }
            }
        }

        /// <summary>
        /// Give ItemDisplays to characters. Currently doesn't work for some reason.
        /// </summary>
        /// <returns></returns>
        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();

            return rules;
        }

    }
}
