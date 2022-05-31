using System;

// Token: 0x02000235 RID: 565
public class SiftCardMessage : BattleAction
{
	// Token: 0x06001181 RID: 4481 RVA: 0x0000D60C File Offset: 0x0000B80C
	public SiftCardMessage(Card card)
	{
		this.cardId = card.getId();
	}

	// Token: 0x04000E03 RID: 3587
	public long cardId;
}
