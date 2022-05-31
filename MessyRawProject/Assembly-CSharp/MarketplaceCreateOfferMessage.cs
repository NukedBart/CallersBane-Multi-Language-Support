using System;

// Token: 0x02000308 RID: 776
public class MarketplaceCreateOfferMessage : Message
{
	// Token: 0x06001320 RID: 4896 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public MarketplaceCreateOfferMessage()
	{
	}

	// Token: 0x06001321 RID: 4897 RVA: 0x0000E4B6 File Offset: 0x0000C6B6
	public MarketplaceCreateOfferMessage(long cardId, int price)
	{
		this.cardId = cardId;
		this.price = price;
	}

	// Token: 0x0400100C RID: 4108
	public long cardId;

	// Token: 0x0400100D RID: 4109
	public int price;
}
