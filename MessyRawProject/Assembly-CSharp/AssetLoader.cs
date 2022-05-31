using System;
using UnityEngine;

// Token: 0x02000029 RID: 41
public class AssetLoader : AbstractCommListener
{
	// Token: 0x1700002C RID: 44
	// (get) Token: 0x060001FA RID: 506 RVA: 0x000038E6 File Offset: 0x00001AE6
	protected static Texture2D dummyCardImage
	{
		get
		{
			return ResourceManager.LoadTexture("Scrolls/cardimage_placeholder");
		}
	}

	// Token: 0x060001FB RID: 507 RVA: 0x000028DF File Offset: 0x00000ADF
	public virtual void Init()
	{
	}

	// Token: 0x060001FC RID: 508 RVA: 0x000038E6 File Offset: 0x00001AE6
	public virtual Texture2D LoadCardImage(string name)
	{
		return ResourceManager.LoadTexture("Scrolls/cardimage_placeholder");
	}

	// Token: 0x060001FD RID: 509 RVA: 0x000028DF File Offset: 0x00000ADF
	public virtual void SetAssetURL(string url)
	{
	}

	// Token: 0x060001FE RID: 510 RVA: 0x000028DF File Offset: 0x00000ADF
	public virtual void SetNewsURL(string url)
	{
	}

	// Token: 0x060001FF RID: 511 RVA: 0x000038F2 File Offset: 0x00001AF2
	public virtual string GetNews()
	{
		return string.Empty;
	}

	// Token: 0x06000200 RID: 512 RVA: 0x00020928 File Offset: 0x0001EB28
	public virtual TwitterSearch GetTwitterFeed()
	{
		return new TwitterSearch
		{
			statuses = new Tweet[0]
		};
	}

	// Token: 0x06000201 RID: 513 RVA: 0x000038F9 File Offset: 0x00001AF9
	public virtual Texture2D GetTwitterImage(string name)
	{
		return null;
	}
}
