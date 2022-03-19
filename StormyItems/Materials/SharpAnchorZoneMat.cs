using RoR2;
using StormyItems.Items;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace StormyItems.Materials
{
    class SharpAnchorZoneMat : MaterialBase
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

			/**
			Log.LogMessage("Checking image properties...");
			Log.LogMessage(cTex.name);
			Log.LogMessage(cTex.wrapMode);
			Log.LogMessage(cTex.filterMode);
			Log.LogMessage(cTex.dimension);
			Log.LogMessage(cTex.activeTextureColorSpace);
			Log.LogMessage(cTex.anisoLevel);
			Log.LogMessage(cTex.graphicsFormat);
			Log.LogMessage(cTex.hideFlags);
			Log.LogMessage(cTex.mipMapBias);
			Log.LogMessage(cTex.mipmapCount);
			Log.LogMessage(cTex.texelSize);
			Log.LogMessage(cTex.minimumMipmapLevel);
			Log.LogMessage(cTex.calculatedMipmapLevel);
			Log.LogMessage(cTex.desiredMipmapLevel);
			Log.LogMessage(cTex.format);
			Log.LogMessage(cTex.requestedMipmapLevel);

			Log.LogMessage(auraTex.name);
			Log.LogMessage(auraTex.wrapMode);
			Log.LogMessage(auraTex.filterMode);
			Log.LogMessage(auraTex.dimension);
			Log.LogMessage(auraTex.activeTextureColorSpace);
			Log.LogMessage(auraTex.anisoLevel);
			Log.LogMessage(auraTex.graphicsFormat);
			Log.LogMessage(auraTex.hideFlags);
			Log.LogMessage(auraTex.mipMapBias);
			Log.LogMessage(auraTex.mipmapCount);
			Log.LogMessage(auraTex.texelSize);
			Log.LogMessage(auraTex.minimumMipmapLevel);
			Log.LogMessage(auraTex.calculatedMipmapLevel);
			Log.LogMessage(auraTex.desiredMipmapLevel);
			Log.LogMessage(auraTex.format);
			Log.LogMessage(auraTex.requestedMipmapLevel);
			**/

			//string message = (auraTex == null) + " | " + (cTex == null);
			//Log.LogMessage(message);

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
			Zone = anchorZone;
		}
    }
}
