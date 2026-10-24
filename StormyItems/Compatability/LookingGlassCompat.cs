using LookingGlass.ItemStatsNameSpace;
using RoR2;
using StormyItems.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using StatType = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ValueType;
using Units = LookingGlass.ItemStatsNameSpace.ItemStatsDef.MeasurementUnits;

namespace StormyItems.Compatability
{
    /// <summary>
    /// Adds the extra stat lines LookingGlass shows under item descriptions.
    /// Every method touching LookingGlass types is NoInlining so nothing loads unless LookingGlass is installed.
    /// </summary>
    internal static class LookingGlassCompat
    {
        public const string GUID = "droppod.lookingglass";
        public static bool IsLookingGlassInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(GUID);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Init()
        {
            ItemCatalog.availability.CallWhenAvailable(RegisterItemStats);
        }

        private struct Line
        {
            public string description;
            public StatType valueType;
            public Units units;

            public Line(string description, StatType valueType, Units units)
            {
                this.description = description;
                this.valueType = valueType;
                this.units = units;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ItemStatsDef Create(Line[] lines)
        {
            var def = new ItemStatsDef();
            foreach (var line in lines)
            {
                def.descriptions.Add(line.description);
                def.valueTypes.Add(line.valueType);
                def.measurementUnits.Add(line.units);
            }
            return def;
        }

        /// <summary>Stats that only depend on the stack count.</summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void AddItem(ItemBase item, Func<int, List<float>> values, params Line[] lines)
        {
            var def = Create(lines);
            def.calculateValuesFlat = values;
            Register(item, def);
        }

        /// <summary>Stats that depend on the holder. The master can be null (e.g. logbook).</summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void AddMasterItem(ItemBase item, Func<CharacterMaster, int, List<float>> values, params Line[] lines)
        {
            var def = Create(lines);
            def.calculateValues = values;
            Register(item, def);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void Register(ItemBase item, ItemStatsDef def)
        {
            // Disabled items are never added to Main.Items, so they come through as null.
            if (item == null || !item.ItemDef || item.ItemDef.itemIndex == ItemIndex.None)
                return;
            ItemDefinitions.allItemDefinitions[(int)item.ItemDef.itemIndex] = def;
        }

        private static T Find<T>() where T : ItemBase => Main.Items?.OfType<T>().FirstOrDefault();

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void RegisterItemStats()
        {
            try
            {
                // Tier 1
                AddItem(Find<CrackedHalo>(), count => new List<float> { 0.25f * count },
                    new Line("Airborne Movement Speed: ", StatType.Utility, Units.Percentage));

                // Radius matches SharpAnchor.OnFixedUpdate: body radius + 1.3m + 1m per stack
                AddMasterItem(Find<SharpAnchor>(), (master, count) =>
                {
                    CharacterBody body = master ? master.GetBody() : null;
                    float bodyRadius = body ? body.radius : 0f;
                    return new List<float> { 0.2f * count, bodyRadius + 1.3f + 1.0f * count };
                },
                    new Line("Zone Damage: ", StatType.Damage, Units.Percentage),
                    new Line("Zone Radius: ", StatType.Utility, Units.Meters));

                // Tier 2
                AddItem(Find<ChargedUrchin>(), count => new List<float> { 0.10f, 0.8f * count },
                    new Line("Shield: ", StatType.Health, Units.PercentHealth),
                    new Line("Shock Damage: ", StatType.Damage, Units.Percentage));

                // The boost is applied to drones, so the burn roll uses the drone's luck, not the holder's
                AddItem(Find<DroneCoolant>(), count => new List<float> { 0.1f * count, 0.2f, 0.5f + 0.5f * count },
                    new Line("Drone Attack Speed: ", StatType.Damage, Units.Percentage),
                    new Line("Drone Burn Chance: ", StatType.Damage, Units.Percentage),
                    new Line("Drone Burn Damage: ", StatType.Damage, Units.Percentage));

                // Void
                AddItem(Find<Talon>(), count => new List<float> { 0.15f * count },
                    new Line("Airborne Damage: ", StatType.Void, Units.Percentage));
            }
            catch (Exception e)
            {
                Log.LogError("Failed to register LookingGlass item stats: " + e);
            }
        }
    }
}
