using System;

// Token: 0x02000249 RID: 585
public class EMHandUpdate : EffectMessage
{
	// Token: 0x060011A9 RID: 4521 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override bool isAction()
	{
		return true;
	}

	// Token: 0x04000E49 RID: 3657
	public int profileId;

	// Token: 0x04000E4A RID: 3658
	public int maxScrollsForCycle;

	// Token: 0x04000E4B RID: 3659
	public Card[] cards;
}
