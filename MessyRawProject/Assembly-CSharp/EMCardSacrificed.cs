using System;

// Token: 0x0200023B RID: 571
public class EMCardSacrificed : EffectMessage
{
	// Token: 0x0600118B RID: 4491 RVA: 0x0000D63E File Offset: 0x0000B83E
	public bool isForResource()
	{
		return !this.isForCards();
	}

	// Token: 0x0600118C RID: 4492 RVA: 0x0000D649 File Offset: 0x0000B849
	public bool isForCards()
	{
		return this.resource.isCards();
	}

	// Token: 0x0600118D RID: 4493 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override bool isAction()
	{
		return true;
	}

	// Token: 0x04000E0B RID: 3595
	public TileColor color;

	// Token: 0x04000E0C RID: 3596
	public ResourceType resource;
}
