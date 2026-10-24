using BepInEx.Configuration;
using R2API;
using RoR2;
using StormyItems.AssetHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace StormyItems.Items
{
    public class DroneCoolant : ItemBase
    {
        public override string ItemName => "Illegal Drone Coolant";

        public override string ItemLangTokenName => "ILLEGAL_DRONE_COOLANT";

        public override string ItemPickupDesc => "Your drones are overclocked to gain attack speed and a chance to burn enemies.";

        public override string ItemFullDescription => "Your drones are overclocked to gain <style=cIsDamage>+10%</style> <style=cStack>(+10% per stack)</style> attack speed and a <style=cIsDamage>20%</style> chance to <style=cIsDamage>burn</style> enemies for <style=cIsDamage>100%</style> <style=cStack>(+50% per stack)</style> base damage.";

        public override string ItemLore => "An ultra-cold compound was found deep in space. Retrieving...";

        public override ItemTier Tier => ItemTier.Tier2;

        public override ItemTag[] ItemTags { get; set; } = new ItemTag[] { ItemTag.Damage };

        public override GameObject ItemModel => DroneCoolantAssetHelper.PickupPrefab;

        public override Sprite ItemIcon => Assets.MainAssets.LoadAsset<Sprite>("Assets/Import/DroneCoolant/DroneCoolantIcon.png");

        private ItemDef boost;

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
            RoR2.CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
            MasterSummon.onServerMasterSummonGlobal += OnServerMasterSummonGlobal;
        }

        private void OnServerMasterSummonGlobal(MasterSummon.MasterSummonReport summonReport)
        {
            CharacterMaster leaderMaster = summonReport.leaderMasterInstance;
            CharacterMaster summonMasterInstance = summonReport.summonMasterInstance;
            if (leaderMaster && summonMasterInstance)
            {
                CharacterBody body = summonMasterInstance.GetBody();
                if (body)
                {
                    //Count comes from the leader, not the newly summoned drone
                    UpdateMinionInventory(GetCount(leaderMaster), summonMasterInstance.inventory, body.bodyFlags);
                }
            }
        }

        private void OnInventoryChanged(CharacterBody body)
        {
            if (!NetworkServer.active)
            {
                return;
            }
            //Runs even when the count is 0 so drones lose the boost when the item is removed
            UpdateAllMinions(body);
        }

        private void UpdateAllMinions(CharacterBody body)
        {
            if (body != null && body.master != null)
            {
                MinionOwnership.MinionGroup minionGroup = MinionOwnership.MinionGroup.FindGroup(body.master.netId);
                if (minionGroup != null)
                {
                    int count = GetCount(body);
                    foreach (MinionOwnership minionOwnership in minionGroup.members)
                    {
                        if (minionOwnership)
                        {
                            CharacterMaster droneMaster = minionOwnership.GetComponent<CharacterMaster>();
                            if (droneMaster && droneMaster.inventory)
                            {
                                CharacterBody droneBody = droneMaster.GetBody();
                                if (droneBody)
                                {
                                    UpdateMinionInventory(count, droneMaster.inventory, droneBody.bodyFlags);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateMinionInventory(int stack, Inventory inventory, CharacterBody.BodyFlags bodyFlags)
        {
            ItemDef boost = GetBoost();
            if (!inventory || !boost)
            {
                return;
            }
            if ((bodyFlags & CharacterBody.BodyFlags.Mechanical) == CharacterBody.BodyFlags.None)
            {
                stack = 0;
            }

            int itemCount = inventory.GetItemCountPermanent(boost);
            if (itemCount < stack)
            {
                inventory.GiveItemPermanent(boost, stack - itemCount);
            }
            else if (itemCount > stack)
            {
                inventory.RemoveItemPermanent(boost, itemCount - stack);
            }
        }

        private ItemDef GetBoost()
        {
            if (!boost)
            {
                foreach (ItemBase item in Main.Items)
                {
                    if (item.ItemLangTokenName == "DRONE_COOLANT_BOOST")
                    {
                        boost = item.ItemDef;
                        break;
                    }
                }
            }
            return boost;
        }

    }
}
