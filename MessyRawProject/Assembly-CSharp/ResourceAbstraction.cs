using System;
using System.Collections.Generic;
using System.IO;
using JsonFx.Json;
using UnityEngine;

// Token: 0x02000030 RID: 48
public class ResourceAbstraction
{
	// Token: 0x06000233 RID: 563 RVA: 0x0002136C File Offset: 0x0001F56C
	public virtual Object Load(string path)
	{
		if (ResourceAbstraction.assetsDirectory == null || path.StartsWith("@"))
		{
			return Resources.Load((!path.StartsWith("@")) ? path : path.Substring(1));
		}
		path = path.ToLowerInvariant();
		if (!this.triedLoadingAssetIndex)
		{
			this.assetIndexLoaded = this.TryLoadAssetList();
			this.triedLoadingAssetIndex = true;
		}
		int num = path.LastIndexOf('/');
		string text = path.Substring(0, num).Replace("/", "%%");
		string text2 = path.Substring(num + 1);
		string text3 = "assets/" + text + ".unity3d";
		string text5;
		if (this.assetIndexLoaded)
		{
			string text4 = this.nameToHash[text3];
			text5 = string.Concat(new string[]
			{
				ResourceAbstraction.assetsDirectory,
				"/",
				text4.Substring(0, 2),
				"/",
				text4
			});
		}
		else
		{
			text5 = ResourceAbstraction.assetsDirectory + "/" + text3;
		}
		if (!this.bundles.ContainsKey(text5))
		{
			Log.info(string.Concat(new string[]
			{
				"Loading asset bundle: ",
				text,
				" (",
				text5,
				")"
			}));
			string fullPath = Path.GetFullPath(text5);
			Log.info("(With absolute path: " + fullPath + ")");
			AssetBundle assetBundle = AssetBundle.CreateFromFile(fullPath);
			this.bundles.Add(text5, assetBundle);
		}
		return this.bundles[text5].Load(text2);
	}

	// Token: 0x06000234 RID: 564 RVA: 0x0002150C File Offset: 0x0001F70C
	private bool TryLoadAssetList()
	{
		if (ResourceAbstraction.assetIndexPath == null)
		{
			Log.info("No asset index file provided. Defaulting to regular bundles.");
			return false;
		}
		if (!File.Exists(ResourceAbstraction.assetIndexPath))
		{
			Log.info("Asset index file does not exist: " + ResourceAbstraction.assetIndexPath);
			return false;
		}
		Log.info("Loading asset index: " + ResourceAbstraction.assetIndexPath);
		JsonReader jsonReader = new JsonReader();
		Dictionary<string, object> dictionary = (Dictionary<string, object>)jsonReader.Read(FileUtil.readFileContents(ResourceAbstraction.assetIndexPath));
		Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["objects"];
		foreach (KeyValuePair<string, object> keyValuePair in dictionary2)
		{
			this.nameToHash.Add(keyValuePair.Key.ToLowerInvariant(), (string)((Dictionary<string, object>)keyValuePair.Value)["hash"]);
		}
		return true;
	}

	// Token: 0x0400011E RID: 286
	public static string assetsDirectory;

	// Token: 0x0400011F RID: 287
	public static string assetIndexPath;

	// Token: 0x04000120 RID: 288
	private Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();

	// Token: 0x04000121 RID: 289
	private bool triedLoadingAssetIndex;

	// Token: 0x04000122 RID: 290
	private bool assetIndexLoaded;

	// Token: 0x04000123 RID: 291
	private Dictionary<string, string> nameToHash = new Dictionary<string, string>();

	// Token: 0x04000124 RID: 292
	private bool overrideUseAssetBundles;
}
