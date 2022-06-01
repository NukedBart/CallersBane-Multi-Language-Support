using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JsonFx.Json;
using UnityEngine;

// Token: 0x0200002A RID: 42
public class DownloadAssetLoader : AssetLoader
{
	// Token: 0x06000203 RID: 515 RVA: 0x0000391A File Offset: 0x00001B1A
	private void Start()
	{
		App.Communicator.addListener(this);
	}

	// Token: 0x06000204 RID: 516 RVA: 0x00020948 File Offset: 0x0001EB48
	public override void handleMessage(Message msg)
	{
		if (msg is GetTwitterFeedMessage)
		{
			GetTwitterFeedMessage getTwitterFeedMessage = (GetTwitterFeedMessage)msg;
			if (!string.IsNullOrEmpty(getTwitterFeedMessage.feed))
			{
				JsonReader jsonReader = new JsonReader();
				try
				{
					this.twitterSearch = jsonReader.Read<TwitterSearch>(getTwitterFeedMessage.feed);
				}
				catch (Exception ex)
				{
					Log.warning("Failed to parse twitter search\n" + ex);
				}
			}
			if (this.twitterSearch != null)
			{
				foreach (Tweet tweet in this.twitterSearch.statuses)
				{
					base.StartCoroutine(this.DownloadTwitterImage(tweet.user.name, tweet.user.profile_image_url));
				}
			}
		}
	}

	// Token: 0x06000205 RID: 517 RVA: 0x00020A14 File Offset: 0x0001EC14
	public override void Init()
	{
		if (this.inited)
		{
			return;
		}
		this.inited = true;
		this.dummyTexture = new Texture2D(300, 225, 5, false);
		Color[] pixels = this.dummyTexture.GetPixels();
		for (int i = 0; i < pixels.Length; i++)
		{
			pixels[i] = new Color(0.1f, 0.07f, 0.03f);
		}
		this.dummyTexture.SetPixels(pixels);
		this.dummyTexture.Apply();
		this.savePath = Application.persistentDataPath + "/cardImagePreview/";
		base.StartCoroutine("UpdateAssetBundle");
		base.StartCoroutine("FetchNews");
	}

	// Token: 0x06000206 RID: 518 RVA: 0x00003928 File Offset: 0x00001B28
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.unloadBundle();
		Object.Destroy(this.dummyTexture);
	}

	// Token: 0x06000207 RID: 519 RVA: 0x00003941 File Offset: 0x00001B41
	private void unloadBundle()
	{
		DownloadAssetLoader.unloadBundle(this.bundle);
		this.bundle = null;
	}

	// Token: 0x06000208 RID: 520 RVA: 0x00003955 File Offset: 0x00001B55
	private static void unloadBundle(AssetBundle bundle)
	{
		if (bundle != null)
		{
			bundle.Unload(false);
		}
	}

	// Token: 0x06000209 RID: 521 RVA: 0x0000396A File Offset: 0x00001B6A
	public override void SetAssetURL(string assetURL)
	{
		this.assetURL = assetURL;
	}

	// Token: 0x0600020A RID: 522 RVA: 0x00003973 File Offset: 0x00001B73
	public override void SetNewsURL(string newsURL)
	{
		this.newsURL = newsURL;
	}

	// Token: 0x0600020B RID: 523 RVA: 0x0000397C File Offset: 0x00001B7C
	public override string GetNews()
	{
		return this.news;
	}

	// Token: 0x0600020C RID: 524 RVA: 0x00003984 File Offset: 0x00001B84
	public override TwitterSearch GetTwitterFeed()
	{
		return this.twitterSearch;
	}

	// Token: 0x0600020D RID: 525 RVA: 0x0000398C File Offset: 0x00001B8C
	public override Texture2D GetTwitterImage(string id)
	{
		return (!this.twitterImages.ContainsKey(id)) ? null : this.twitterImages[id];
	}

	// Token: 0x0600020E RID: 526 RVA: 0x00020AD0 File Offset: 0x0001ECD0
	private IEnumerator ShowDownloadingPopup()
	{
		yield return new WaitForSeconds(0.1f);
		for (;;)
		{
			App.Popups.ShowInfo("Updating", "Updating image library:\n" + (this.wwwAssetDownload.progress * 100f).ToString("N0") + "%\n");
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600020F RID: 527 RVA: 0x000039B1 File Offset: 0x00001BB1
	public override Texture2D LoadCardImage(string name)
	{
		if (name == "0")
		{
			return ResourceManager.LoadTexture("Scrolls/cardimage_placeholder");
		}
		return this.LoadTexture2D(name);
	}

	// Token: 0x06000210 RID: 528 RVA: 0x00020AEC File Offset: 0x0001ECEC
	public Texture2D LoadTexture2D(string name)
	{
		if (this.bundle == null)
		{
			return this.dummyTexture;
		}
		Texture2D texture2D = (Texture2D)this.bundle.Load(name);
		if (texture2D != null)
		{
			return texture2D;
		}
		return this.dummyTexture;
	}

	// Token: 0x06000211 RID: 529 RVA: 0x00020B38 File Offset: 0x0001ED38
	private IEnumerator FetchNews()
	{
		Log.info("Fetching news from: " + this.newsURL);
		WWW www = new WWW(this.newsURL);
		yield return www;
		this.news = www.text;
		yield break;
	}

	// Token: 0x06000212 RID: 530 RVA: 0x00020B54 File Offset: 0x0001ED54
	private IEnumerator DownloadTwitterImage(string id, string url)
	{
		if (!this.twitterImages.ContainsKey(id))
		{
			this.twitterImages.Add(id, null);
			WWW www = new WWW(url);
			yield return www;
			this.twitterImages[id] = www.texture;
		}
		yield break;
	}

	// Token: 0x06000213 RID: 531 RVA: 0x00020B8C File Offset: 0x0001ED8C
	private IEnumerator UpdateAssetBundle()
	{
		App.LobbyMenu.SetButtonsEnabled(false);
		App.ChatUI.SetCanOpenContextMenu(false);
		string assetBundleFilename = "cardImages" + OsSpec.getOSSuffix() + ".unity3d";
		string versionFilename = "CardImageVersion.txt";
		string fullFileName = this.savePath + assetBundleFilename;
		if (!Directory.Exists(this.savePath))
		{
			Directory.CreateDirectory(this.savePath);
		}
		int savedVersion = -1;
		string versionFileLocation = this.savePath + versionFilename;
		string versionContent = FileUtil.readFileContents(versionFileLocation);
		if (versionContent != null)
		{
			try
			{
				savedVersion = (int)Convert.ToInt16(versionContent);
			}
			catch (Exception)
			{
			}
		}
		int serverVersion = 1;
		WWW www = new WWW(this.assetURL + versionFilename + "?get");
		yield return www;
		bool success = int.TryParse(www.text, ref serverVersion);
		if (success)
		{
			Log.info(string.Concat(new object[]
			{
				"AssetLoader: Server reported card image bundle version: ",
				serverVersion,
				". Saved version: ",
				savedVersion
			}));
		}
		else
		{
			Log.error("AssetLoader: Failed to get server's card image bundle version. Saved version: " + savedVersion);
		}
		bool needUpdate = savedVersion < serverVersion || !File.Exists(fullFileName);
		if (needUpdate)
		{
			Log.info("AssetLoader: Begin downloading card image asset bundle.");
			string assetbundleurl = this.assetURL + assetBundleFilename;
			this.wwwAssetDownload = new WWW(assetbundleurl);
			Debug.Log("Downloading asset bundle: " + assetbundleurl);
			base.StartCoroutine("ShowDownloadingPopup");
			yield return this.wwwAssetDownload;
			base.StopCoroutine("ShowDownloadingPopup");
			yield return null;
			yield return null;
			App.Popups.KillCurrentPopup();
			this.unloadBundle();
			if (this.wwwAssetDownload.error != null)
			{
				Log.error("AssetLoader: " + this.wwwAssetDownload.error);
			}
			else
			{
				Log.info("AssetLoader: Finished downloading card image asset bundle.");
				this.bundle = this.wwwAssetDownload.assetBundle;
				File.WriteAllBytes(fullFileName, this.wwwAssetDownload.bytes);
				TextWriter tw = new StreamWriter(this.savePath + versionFilename);
				tw.Write(serverVersion);
				tw.Close();
				Log.info("AssetLoader: Cached card image asset bundle.");
			}
		}
		else if (this.bundle == null)
		{
			bool mightBeCompressedAssets = false;
			if (mightBeCompressedAssets)
			{
				WWW localAssetBundleWww = new WWW("file://" + fullFileName);
				yield return localAssetBundleWww;
				this.bundle = localAssetBundleWww.assetBundle;
			}
			else
			{
				this.bundle = AssetBundle.CreateFromFile(fullFileName);
			}
			if (this.bundle == null)
			{
				Log.error("Couldn't create asset bundle " + assetBundleFilename);
				try
				{
					File.Delete(fullFileName);
				}
				catch (Exception ex)
				{
					Exception e = ex;
					Log.error(string.Concat(new object[]
					{
						"Failed to delete asset bundle ",
						assetBundleFilename,
						":\n",
						e
					}));
				}
			}
		}
		App.PostCardImagesLoaded();
		yield break;
	}

	// Token: 0x040000F1 RID: 241
	private bool inited;

	// Token: 0x040000F2 RID: 242
	private AssetBundle bundle;

	// Token: 0x040000F3 RID: 243
	private string assetURL;

	// Token: 0x040000F4 RID: 244
	private string newsURL;

	// Token: 0x040000F5 RID: 245
	private WWW wwwAssetDownload;

	// Token: 0x040000F6 RID: 246
	private string savePath;

	// Token: 0x040000F7 RID: 247
	private Texture2D dummyTexture;

	// Token: 0x040000F8 RID: 248
	private string news = string.Empty;

	// Token: 0x040000F9 RID: 249
	private TwitterSearch twitterSearch;

	// Token: 0x040000FA RID: 250
	private Dictionary<string, Texture2D> twitterImages = new Dictionary<string, Texture2D>();
}
