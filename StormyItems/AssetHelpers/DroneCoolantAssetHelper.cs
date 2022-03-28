using R2API;
using RoR2;
using StormyItems.Items;
using StormyItems.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace StormyItems.Materials
{
    class DroneCoolantAssetHelper : AssetHelperBase
    {
		public static GameObject PickupPrefab;
		public static GameObject DisplayPrefab;
		public static Material keepLoaded;

        public override void Init()
        {
			keepLoaded = Addressables.LoadAssetAsync<Material>("RoR2/Base/Infusion/matInfusionGlass.mat").WaitForCompletion();

			var droneCoolantPickup = Assets.MainAssets.LoadAsset<GameObject>("Assets/Import/DroneCoolant/DroneCoolantPickup.prefab");
			var droneCoolantDisplay = Assets.MainAssets.LoadAsset<GameObject>("Assets/Import/DroneCoolant/DroneCoolantDisplay.prefab");

			droneCoolantPickup.transform.GetChild(2).transform.GetChild(0).GetComponent<MeshRenderer>().material = keepLoaded;
			droneCoolantDisplay.transform.GetChild(2).transform.GetChild(0).GetComponent<MeshRenderer>().material = keepLoaded;

			var itemDisplay = droneCoolantDisplay.AddComponent<ItemDisplay>();
			itemDisplay.rendererInfos = ItemHelpers.ItemDisplaySetup(droneCoolantDisplay);

			PickupPrefab = droneCoolantPickup;
			DisplayPrefab = droneCoolantDisplay;
		}
    }
}
