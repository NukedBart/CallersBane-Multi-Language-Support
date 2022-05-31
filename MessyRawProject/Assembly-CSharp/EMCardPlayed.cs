using System;

// Token: 0x0200023A RID: 570
public class EMCardPlayed : EffectMessage
{
	// Token: 0x06001189 RID: 4489 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override bool isAction()
	{
		return true;
	}

	// Token: 0x04000E0A RID: 3594
	public Card card;
}
