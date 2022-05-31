using System;
using UnityEngine;

// Token: 0x02000032 RID: 50
public class StandaloneAssetLoader : AssetLoader
{
	// Token: 0x06000245 RID: 581 RVA: 0x0000391A File Offset: 0x00001B1A
	public void Start()
	{
		App.Communicator.addListener(this);
	}

	// Token: 0x06000246 RID: 582 RVA: 0x00003B45 File Offset: 0x00001D45
	public override void Init()
	{
		App.PostCardImagesLoaded();
	}

	// Token: 0x06000247 RID: 583 RVA: 0x00003B4C File Offset: 0x00001D4C
	public override string GetNews()
	{
		return this.news;
	}

	// Token: 0x06000248 RID: 584 RVA: 0x00003B54 File Offset: 0x00001D54
	public override Texture2D LoadCardImage(string name)
	{
		if (name == "0")
		{
			return AssetLoader.dummyCardImage;
		}
		return this.LoadTexture2D(name);
	}

	// Token: 0x06000249 RID: 585 RVA: 0x000217C4 File Offset: 0x0001F9C4
	public Texture2D LoadTexture2D(string name)
	{
		Texture2D texture2D = ResourceManager.LoadTexture("CardImagesSmall/" + name);
		if (texture2D != null)
		{
			return texture2D;
		}
		return AssetLoader.dummyCardImage;
	}

	// Token: 0x0600024A RID: 586 RVA: 0x00003B73 File Offset: 0x00001D73
	public override void handleMessage(Message msg)
	{
		if (msg is NewsMessage)
		{
			this.news = ((NewsMessage)msg).news;
		}
	}

	// Token: 0x04000129 RID: 297
	private string news = string.Empty;
}
