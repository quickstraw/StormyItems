using BepInEx.Configuration;
using R2API;
using RoR2;
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

        public override GameObject ItemModel => Assets.MainAssets.LoadAsset<GameObject>("Assets/Import/DroneCoolant/DroneCoolant.prefab");

        public override Sprite ItemIcon => Assets.MainAssets.LoadAsset<Sprite>("Assets/Import/DroneCoolant/DroneCoolantIcon.png");

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
                        StrengthenBurnUtils.CheckDotForUpgrade(inflictor.inventory, ref dotInfo);
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
