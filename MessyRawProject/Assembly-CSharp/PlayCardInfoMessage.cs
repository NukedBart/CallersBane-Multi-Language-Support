using System;

// Token: 0x02000232 RID: 562
public class PlayCardInfoMessage : BattleAction
{
	// Token: 0x0600117D RID: 4477 RVA: 0x0000D5A8 File Offset: 0x0000B7A8
	public PlayCardInfoMessage(Card card)
	{
		this.card = card.getId();
	}

	// Token: 0x04000DFE RID: 3582
	public long card;
}
