using BepInEx.Configuration;
using R2API;
using RoR2;
using StormyItems.Materials;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

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

        public override GameObject ItemModel => DroneCoolantAssetHelper.PickupPrefab;

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
            RoR2.CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
            MasterSummon.onServerMasterSummonGlobal += OnServerMasterSummonGlobal;
        }

        private void OnServerMasterSummonGlobal(MasterSummon.MasterSummonReport summonReport)
        {
            if (summonReport.leaderMasterInstance)
            {
                CharacterMaster summonMasterInstance = summonReport.summonMasterInstance;
                if (summonMasterInstance)
                {
                    CharacterBody body = summonMasterInstance.GetBody();
                    if (body)
                    {
                        int count = GetCount(body);
                        UpdateMinionInventory(count, summonMasterInstance.inventory, body.bodyFlags);
                    }
                }
            }
        }

        private void OnInventoryChanged(CharacterBody body)
        {
            int count = GetCount(body);
            if(count > 0)
            {
                UpdateAllMinions(body);
            }
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
        }

        private void UpdateAllMinions(CharacterBody body)
        {
            if (body != null && body.master != null)
            {
                MinionOwnership.MinionGroup minionGroup = MinionOwnership.MinionGroup.FindGroup(body.master.netId);
                if (minionGroup != null)
                {
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
                                    UpdateMinionInventory(GetCount(body), droneMaster.inventory, droneBody.bodyFlags);
                                }
                            }
                        }
                    }
                    //this.previousStack = newStack;
                }
            }
        }

        private void UpdateMinionInventory(int stack, Inventory inventory, CharacterBody.BodyFlags bodyFlags)
        {
            ItemDef boost = GetBoost();
            if (inventory && stack > 0 && (bodyFlags & CharacterBody.BodyFlags.Mechanical) > CharacterBody.BodyFlags.None)
            {
                int itemCount = inventory.GetItemCount(boost);
                if (itemCount < stack)
                {
                    inventory.GiveItem(boost, stack - itemCount);
                }
                else if (itemCount > stack)
                {
                    inventory.RemoveItem(boost, itemCount - stack);
                }
            }
            else
            {
                inventory.ResetItem(boost);
            }
        }

        private ItemDef GetBoost()
        {
            ItemBase outItem = null;
            foreach(ItemBase item in Main.Items)
            {
                if(item.ItemLangTokenName == "DRONE_COOLANT_BOOST")
                {
                    outItem = item;
                }
            }
            return outItem.ItemDef;
        }

    }
}
