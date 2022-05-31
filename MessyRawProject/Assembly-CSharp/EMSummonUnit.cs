using System;

// Token: 0x02000261 RID: 609
public class EMSummonUnit : EffectMessage
{
	// Token: 0x060011E3 RID: 4579 RVA: 0x0000D948 File Offset: 0x0000BB48
	public override float timeoutSeconds()
	{
		return 7f;
	}

	// Token: 0x060011E4 RID: 4580 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override bool isAction()
	{
		return true;
	}

	// Token: 0x04000E91 RID: 3729
	public TilePosition target;

	// Token: 0x04000E92 RID: 3730
	public Card card;
}
