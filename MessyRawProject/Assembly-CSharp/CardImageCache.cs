using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000118 RID: 280
public class CardImageCache
{
	// Token: 0x060008CC RID: 2252 RVA: 0x00007A44 File Offset: 0x00005C44
	private CardImageCache()
	{
		this._resourceDirectory = OsSpec.getDownloadDataPath() + "/cardImages/";
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x00045A74 File Offset: 0x00043C74
	public Texture2D GetLoadedImage(string fn)
	{
		Texture2D result = null;
		if (this._images.TryGetValue(fn, ref result))
		{
			return result;
		}
		string text = this._resourceDirectory + fn;
		if (File.Exists(text))
		{
			result = this.storeFromBytes(fn, File.ReadAllBytes(text));
		}
		return result;
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x00007A6C File Offset: 0x00005C6C
	public Texture2D Write(string fn, byte[] bytes)
	{
		File.WriteAllBytes(this._resourceDirectory + fn, bytes);
		return this.storeFromBytes(fn, bytes);
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x00045AC0 File Offset: 0x00043CC0
	private Texture2D storeFromBytes(string fn, byte[] bytes)
	{
		Texture2D texture2D = new Texture2D(2, 2, 3, false);
		texture2D.LoadImage(bytes);
		this._images[fn] = texture2D;
		return texture2D;
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x00007A88 File Offset: 0x00005C88
	public static CardImageCache instance()
	{
		if (CardImageCache._instance == null)
		{
			CardImageCache._instance = new CardImageCache();
		}
		return CardImageCache._instance;
	}

	// Token: 0x060008D1 RID: 2257 RVA: 0x00007AA3 File Offset: 0x00005CA3
	public static void free()
	{
		CardImageCache._instance = null;
	}

	// Token: 0x04000692 RID: 1682
	private Dictionary<string, Texture2D> _images = new Dictionary<string, Texture2D>();

	// Token: 0x04000693 RID: 1683
	private string _resourceDirectory;

	// Token: 0x04000694 RID: 1684
	private static CardImageCache _instance;
}
