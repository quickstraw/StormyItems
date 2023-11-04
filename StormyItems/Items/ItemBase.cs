using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;
using R2API;
using UnityEngine;
using RoR2.Items;
using RoR2.ExpansionManagement;
using System.Linq;
using UnityEngine.AddressableAssets;

namespace StormyItems.Items
{
    public abstract class ItemBase
    {
        public static ExpansionDef SOTV = ExpansionCatalog.expansionDefs.FirstOrDefault(expansion => expansion.nameToken == "DLC1_NAME");

        public abstract string ItemName { get; }
        public abstract string ItemLangTokenName { get; }
        public abstract string ItemPickupDesc { get; }
        public abstract string ItemFullDescription { get; }
        public abstract string ItemLore { get; }

        public abstract ItemTier Tier { get; }
        public virtual ItemTag[] ItemTags { get; set; } = new ItemTag[] { };

        public abstract GameObject ItemModel { get; }
        public abstract Sprite ItemIcon { get; }

        public virtual Sprite BuffIcon { get; }

        public ItemDef ItemDef;

        public virtual bool CanRemove { get; } = true;
        public virtual bool RequiresSOTV { get; } = false;

        public virtual bool AIBlacklisted { get; set; } = false;

        public abstract void StartInit(ConfigFile config);
        public virtual void CreateConfig(ConfigFile config) { }
        public virtual ItemBase VoidParent()
        {
            return null;
        }
        public virtual void OnUpdate()
        {

        }

        public virtual void OnFixedUpdate()
        {

        }

        //Based on ThinkInvis' methods
        public int GetCount(CharacterBody body)
        {
            if (!body || !body.inventory) { return 0; }

            return body.inventory.GetItemCount(ItemDef);
        }

        public int GetCount(CharacterMaster master)
        {
            if (!master || !master.inventory) { return 0; }

            return master.inventory.GetItemCount(ItemDef);
        }

        protected virtual void CreateLang()
        {
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_NAME", ItemName);
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_PICKUP", ItemPickupDesc);
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_DESCRIPTION", ItemFullDescription);
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_LORE", ItemLore);
        }

        public abstract ItemDisplayRuleDict CreateItemDisplayRules();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Member Access", "Publicizer001:Accessing a member that was not originally public", Justification = "<Pending>")]
        protected void CreateItem()
        {
            if (AIBlacklisted)
            {
                ItemTags = new List<ItemTag>(ItemTags) { ItemTag.AIBlacklist }.ToArray();
            }

            ItemDef = ScriptableObject.CreateInstance<ItemDef>();
            ItemDef.name = "ITEM_" + ItemLangTokenName;
            ItemDef.nameToken = "ITEM_" + ItemLangTokenName + "_NAME";
            ItemDef.pickupToken = "ITEM_" + ItemLangTokenName + "_PICKUP";
            ItemDef.descriptionToken = "ITEM_" + ItemLangTokenName + "_DESCRIPTION";
            ItemDef.loreToken = "ITEM_" + ItemLangTokenName + "_LORE";
            ItemDef.pickupModelPrefab = ItemModel;
            ItemDef.pickupIconSprite = ItemIcon;
            ItemDef.hidden = false;
            ItemDef.canRemove = CanRemove;
            switch (Tier)
            {
                case ItemTier.NoTier:
#pragma warning disable CS0618 // Type or member is obsolete
                    ItemDef.deprecatedTier = ItemTier.NoTier;
#pragma warning restore CS0618 // Type or member is obsolete
                    break;
                case ItemTier.Tier1:
                    ItemDef._itemTierDef = Addressables.LoadAssetAsync<ItemTierDef>("RoR2/Base/Common/Tier1Def.asset").WaitForCompletion();
                    break;
                case ItemTier.Tier2:
                    ItemDef._itemTierDef = Addressables.LoadAssetAsync<ItemTierDef>("RoR2/Base/Common/Tier2Def.asset").WaitForCompletion();
                    break;
                case ItemTier.Tier3:
                    ItemDef._itemTierDef = Addressables.LoadAssetAsync<ItemTierDef>("RoR2/Base/Common/Tier3Def.asset").WaitForCompletion();
                    break;
                case ItemTier.Boss:
                    ItemDef._itemTierDef = Addressables.LoadAssetAsync<ItemTierDef>("RoR2/Base/Common/BossTierDef.asset").WaitForCompletion();
                    break;
                case ItemTier.Lunar:
                    ItemDef._itemTierDef = Addressables.LoadAssetAsync<ItemTierDef>("RoR2/Base/Common/LunarTierDef.asset").WaitForCompletion();
                    break;
                case ItemTier.VoidTier1:
                    ItemDef._itemTierDef = Addressables.LoadAssetAsync<ItemTierDef>("RoR2/DLC1/Common/VoidTier1Def.asset").WaitForCompletion();
                    break;
                case ItemTier.VoidTier2:
                    ItemDef._itemTierDef = Addressables.LoadAssetAsync<ItemTierDef>("RoR2/DLC1/Common/VoidTier2Def.asset").WaitForCompletion();
                    break;
                case ItemTier.VoidTier3:
                    ItemDef._itemTierDef = Addressables.LoadAssetAsync<ItemTierDef>("RoR2/DLC1/Common/VoidTier3Def.asset").WaitForCompletion();
                    break;
                case ItemTier.VoidBoss:
                    ItemDef._itemTierDef = Addressables.LoadAssetAsync<ItemTierDef>("RoR2/DLC1/Common/VoidBossDef.asset").WaitForCompletion();
                    break;
                default:
                    ItemDef._itemTierDef = Addressables.LoadAssetAsync<ItemTierDef>("RoR2/Base/Common/Tier1Def.asset").WaitForCompletion();
                    break;
            }

            if (RequiresSOTV)
            {
                ItemDef.requiredExpansion = SOTV;
            }

            if (ItemTags.Length > 0) { ItemDef.tags = ItemTags; }

            ItemAPI.Add(new CustomItem(ItemDef, CreateItemDisplayRules()));
        }

        public void AddVoidPair(List<ItemDef.Pair> newVoidPairs)
        {
            var voidParent = VoidParent();
            if(voidParent == null)
            {
                return;
            }
            //var voidPairs = ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem].Where(x => x.itemDef1 != VoidParent.ItemDef); -- Use to overwrite other mods
            ItemDef.Pair newVoidPair = new ItemDef.Pair()
            {
                itemDef1 = voidParent.ItemDef,
                itemDef2 = ItemDef
            };
            newVoidPairs.Add(newVoidPair);
        }

        public virtual void OnBodyAdded(CharacterBody body)
        {

        }

        public virtual void OnBodyRemoved(CharacterBody body, int index)
        {

        }
    }
}
