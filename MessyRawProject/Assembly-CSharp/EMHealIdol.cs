using System;

// Token: 0x0200024A RID: 586
public class EMHealIdol : EffectMessage
{
	// Token: 0x060011AB RID: 4523 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override bool isAction()
	{
		return true;
	}

	// Token: 0x04000E4C RID: 3660
	public TilePosition source;

	// Token: 0x04000E4D RID: 3661
	public IdolInfo idol;

	// Token: 0x04000E4E RID: 3662
	public int amount;
}
