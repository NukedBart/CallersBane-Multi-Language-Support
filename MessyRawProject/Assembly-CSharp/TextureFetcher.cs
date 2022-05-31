using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000119 RID: 281
public class TextureFetcher
{
	// Token: 0x060008D2 RID: 2258 RVA: 0x00007AAB File Offset: 0x00005CAB
	public TextureFetcher(MonoBehaviour coroutiner, TextureFetcher.onFetched onFetchedCallback)
	{
		this._onFetched = onFetchedCallback;
		this._coroutiner = coroutiner;
	}

	// Token: 0x060008D3 RID: 2259 RVA: 0x00045AF0 File Offset: 0x00043CF0
	public void fetch(string fn)
	{
		Texture2D loadedImage = CardImageCache.instance().GetLoadedImage(fn);
		if (loadedImage != null)
		{
			this._onFetched(loadedImage);
			return;
		}
		WWW www = new WWW(App.Communicator.GetCardDownloadURL() + "img/" + fn);
		this._coroutiner.StartCoroutine(this._waitForRequest(www, fn));
	}

	// Token: 0x060008D4 RID: 2260 RVA: 0x00045B54 File Offset: 0x00043D54
	private IEnumerator _waitForRequest(WWW www, string fn)
	{
		yield return www;
		if (www.error == null)
		{
			Texture2D image = CardImageCache.instance().Write(fn, www.bytes);
			if (image != null)
			{
				this._onFetched(image);
			}
		}
		else
		{
			Log.info("ERROR LOADING EXTERNAL IMAGE " + www.url);
		}
		yield break;
	}

	// Token: 0x04000695 RID: 1685
	private TextureFetcher.onFetched _onFetched;

	// Token: 0x04000696 RID: 1686
	private MonoBehaviour _coroutiner;

	// Token: 0x0200011A RID: 282
	// (Invoke) Token: 0x060008D6 RID: 2262
	public delegate void onFetched(Texture2D tex);
}
