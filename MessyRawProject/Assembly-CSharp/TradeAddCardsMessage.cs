using System;

// Token: 0x0200036A RID: 874
public class TradeAddCardsMessage : Message
{
	// Token: 0x060013CB RID: 5067 RVA: 0x0000EACC File Offset: 0x0000CCCC
	public TradeAddCardsMessage(long[] cardIds)
	{
		this.cardIds = cardIds;
	}

	// Token: 0x040010F5 RID: 4341
	public long[] cardIds;
}
