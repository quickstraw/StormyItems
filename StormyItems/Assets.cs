using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using RoR2.ContentManagement;
using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System.Reflection;
using System.Linq;

namespace StormyItems
{
	//Static class for ease of access
	public static class Assets
	{
		//The mod's AssetBundle
		public static AssetBundle MainAssets;
	    //A constant of the AssetBundle's name.
		public const string bundleName = "stormyassets";
		public static Dictionary<string, string> ShaderLookup = new Dictionary<string, string>()
		{
			{"fake ror/hopoo games/deferred/standard", "shaders/deferred/hgstandard"},
			{"fake ror/hopoo games/fx/cloud intersection remap", "shaders/fx/hgintersectioncloudremap" },
			{"fake ror/hopoo games/fx/cloud remap", "shaders/fx/hgcloudremap" },
			{"fake ror/hopoo games/fx/distortion", "shaders/fx/hgdistortion" },
			{"fake ror/hopoo games/deferred/snow topped", "shaders/deferred/hgsnowtopped" },
			{"fake ror/hopoo games/fx/solid parallax", "shaders/fx/hgsolidparallax" }
		};

		public static void Init()
		{
			//Loads the assetBundle from the Path, and stores it in the static field.
			//mainBundle = AssetBundle.LoadFromFile(AssetBundlePath);
			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("StormyItems.stormyassets"))
			{
				MainAssets = AssetBundle.LoadFromStream(stream);
			}
			ShaderConversion(MainAssets);
		}

		public static void ShaderConversion(AssetBundle assets)
		{
			var materialAssets = assets.LoadAllAssets<Material>().Where(material => material.shader.name.StartsWith("Fake RoR"));

			foreach (Material material in materialAssets)
			{
				var replacementShader = Resources.Load<Shader>(ShaderLookup[material.shader.name.ToLowerInvariant()]);
				if (replacementShader)
				{
					material.shader = replacementShader;
				}
			}
		}
	}

}
