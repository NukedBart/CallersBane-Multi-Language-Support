using System;

// Token: 0x02000303 RID: 771
public class MarketplaceAvailableOffersListViewMessage : Message
{
	// Token: 0x06001317 RID: 4887 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public MarketplaceAvailableOffersListViewMessage()
	{
	}

	// Token: 0x06001318 RID: 4888 RVA: 0x0000E464 File Offset: 0x0000C664
	public MarketplaceAvailableOffersListViewMessage(int? level)
	{
		this.cardLevel = level;
	}

	// Token: 0x04000FFF RID: 4095
	public int? cardLevel;

	// Token: 0x04001000 RID: 4096
	[ServerToClient]
	public MarketplaceTypeAvailability[] available;
}
