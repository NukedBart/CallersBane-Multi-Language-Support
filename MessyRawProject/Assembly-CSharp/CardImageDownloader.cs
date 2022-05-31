using System;
using System.Collections;
using System.IO;
using UnityEngine;

// Token: 0x0200012A RID: 298
public class CardImageDownloader : AbstractCommListener
{
	// Token: 0x06000997 RID: 2455 RVA: 0x0004AC68 File Offset: 0x00048E68
	private IEnumerator Start()
	{
		this.comm = App.Communicator;
		this.comm.addListener(this);
		while (!this.comm.ReadyToUse)
		{
			yield return null;
		}
		this.url = this.comm.GetCardDownloadURL() + "img/";
		this.comm.sendRequest(new CardImagesListMessage());
		Log.info(CardImageDownloader.RESOURCE_DIRECTORY);
		if (!Directory.Exists(CardImageDownloader.RESOURCE_DIRECTORY))
		{
			Directory.CreateDirectory(CardImageDownloader.RESOURCE_DIRECTORY);
		}
		yield break;
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x00008223 File Offset: 0x00006423
	public override void handleMessage(Message msg)
	{
		if (msg is CardImagesListMessage)
		{
			base.StartCoroutine(this.downloadCards(((CardImagesListMessage)msg).images, false));
		}
		base.handleMessage(msg);
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x0004AC84 File Offset: 0x00048E84
	private IEnumerator downloadCards(long[] cardIds, bool overwrite)
	{
		foreach (long id in cardIds)
		{
			string imagename = id.ToString();
			string filename = CardImageDownloader.RESOURCE_DIRECTORY + imagename + ".png";
			if (!File.Exists(filename) || overwrite)
			{
				yield return new WaitForSeconds(0.1f);
				Log.info(this.url + id);
				WWW www = new WWW(this.url + imagename);
				base.StartCoroutine(this.WaitForRequest(www, filename));
			}
		}
		Log.info("Finished!");
		yield break;
	}

	// Token: 0x0600099A RID: 2458 RVA: 0x0004ACBC File Offset: 0x00048EBC
	private IEnumerator WaitForRequest(WWW www, string filename)
	{
		yield return www;
		string status = www.text;
		if (www.error == null)
		{
			Texture2D tex = new Texture2D(1, 1, 4, true);
			www.LoadImageIntoTexture(tex);
			File.WriteAllBytes(filename, tex.EncodeToPNG());
			Log.info("Saved image " + filename);
		}
		else
		{
			Log.info("ERROR LOADING EXTERNAL IMAGE:" + www.url);
		}
		yield break;
	}

	// Token: 0x0600099B RID: 2459 RVA: 0x000028DF File Offset: 0x00000ADF
	private void Update()
	{
	}

	// Token: 0x0400073C RID: 1852
	private const bool overwrite = false;

	// Token: 0x0400073D RID: 1853
	private Communicator comm;

	// Token: 0x0400073E RID: 1854
	private string url;

	// Token: 0x0400073F RID: 1855
	private static string RESOURCE_DIRECTORY = Application.dataPath + "/Temp/BUILD-cardImages/";
}
