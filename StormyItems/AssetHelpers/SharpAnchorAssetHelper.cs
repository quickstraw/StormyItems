using R2API;
using RoR2;
using StormyItems.Items;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace StormyItems.Materials
{
    class SharpAnchorAssetHelper : AssetHelperBase
    {
		public static GameObject Zone;
		public static Texture keepLoaded;

        public override void Init()
        {
			//Shaders/FX/
			Shader shader = Assets.IntersectionShader;//LegacyShaderAPI.Find("HGCloudIntersectionRemap".ToLowerInvariant());
			var copy = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/MushroomWard");
			var cRend = copy.transform.Find("Indicator").Find("IndicatorSphere").GetComponent<MeshRenderer>();

			var auraTex = Assets.MainAssets.LoadAsset<Texture2D>("Assets/Import/SharpAnchor/sharp_anchor_zone/texRampAnchorZoneV2.png");
			keepLoaded = Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/texCloudCrackedIce.png").WaitForCompletion();
			//"RoR2/Base/Common/texCloudStroke1.png"
			//Texture2D cTex = (Texture2D) cRend.material.GetTexture("_RemapTex");

			//shader = cRend.material.shader;
			//Addressables.LoadAssetAsync<GameObject>(key: "RoR2/Base/WarCryOnMultiKill/WarCryEffect.prefab").WaitForCompletion();
			Material material = new Material(shader);
			material.CopyPropertiesFromMaterial(cRend.material);
			material.SetTexture("_RemapTex", auraTex);//Assets.MainAssets.LoadAsset<Texture>("Assets/Import/SharpAnchor/sharp_anchor_zone/texRampAnchorZone.png"));
			material.SetTexture("_Cloud2Tex", keepLoaded);
			//material.SetTextureScale("_Cloud2Tex", new Vector2(3, 3));

			material.SetVector("_CutoffScroll", new Vector4(0, 0, 3, 2));
			material.SetFloat("_InvFade", 3.3f);
			material.SetFloat("_SoftPower", 1.0f);
			material.SetFloat("_Boost", 0.34f);
			material.SetFloat("_RimPower", 20.0f);
			material.SetFloat("_RimStrength", 0.0f);
			material.SetFloat("_AlphaBoost", 4.32f);
			material.SetFloat("_IntersectionStrength", 2.34f);

			Material = material;
			Shader = material.shader;

			//material = cRend.material;

			var anchorZone = Assets.MainAssets.LoadAsset<GameObject>("Assets/Import/SharpAnchor/sharp_anchor_zone/SharpAnchorZone.prefab");

			anchorZone.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material = material;
			anchorZone.AddComponent<NetworkIdentity>();
			anchorZone.AddComponent<TeamFilter>();
			anchorZone.AddComponent<SharpAnchorZone>();
			anchorZone.RegisterNetworkPrefab();
			Zone = anchorZone;
		}
    }
}
