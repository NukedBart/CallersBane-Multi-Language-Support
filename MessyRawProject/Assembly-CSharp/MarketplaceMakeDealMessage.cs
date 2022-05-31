using System;

// Token: 0x02000309 RID: 777
public class MarketplaceMakeDealMessage : Message
{
	// Token: 0x06001322 RID: 4898 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public MarketplaceMakeDealMessage()
	{
	}

	// Token: 0x06001323 RID: 4899 RVA: 0x0000E4CC File Offset: 0x0000C6CC
	public MarketplaceMakeDealMessage(long offerId)
	{
		this.offerId = offerId;
	}

	// Token: 0x0400100E RID: 4110
	public long offerId;
}
