using System;

// Token: 0x02000371 RID: 881
public class TradeRemoveCardMessage : Message
{
	// Token: 0x060013D2 RID: 5074 RVA: 0x0000EB18 File Offset: 0x0000CD18
	public TradeRemoveCardMessage(long cardId)
	{
		this.cardId = cardId;
	}

	// Token: 0x040010FE RID: 4350
	public long cardId;
}
