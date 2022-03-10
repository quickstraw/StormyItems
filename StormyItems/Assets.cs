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

namespace HaloItem
{
	//Static class for ease of access
	public static class Assets
	{
		//The mod's AssetBundle
		public static AssetBundle mainBundle;
	    //A constant of the AssetBundle's name.
		public const string bundleName = "crackedhalo";
		// Not necesary, but useful if you want to store the bundle on its own folder.
		// public const string assetBundleFolder = "AssetBundles";

		//The direct path to your AssetBundle
		public static string AssetBundlePath
		{
			get
			{
				//This returns the path to your assetbundle assuming said bundle is on the same folder as your DLL. If you have your bundle in a folder, you can uncomment the statement below this one.
				return Path.Combine(Path.GetDirectoryName(Main.PInfo.Location), bundleName);
				//return Path.Combine(MainClass.PInfo.Location, assetBundleFolder, myBundle);
			}
		}

		public static void Init()
		{
			//Loads the assetBundle from the Path, and stores it in the static field.
			mainBundle = AssetBundle.LoadFromFile(AssetBundlePath);
		}
	}

}
