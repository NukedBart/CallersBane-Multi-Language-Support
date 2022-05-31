using System;

// Token: 0x0200030C RID: 780
public class MarketplaceOffersViewMessage : Message
{
	// Token: 0x04001017 RID: 4119
	[ServerToClient]
	public int profileId;

	// Token: 0x04001018 RID: 4120
	[ServerToClient]
	public MarketplaceOffer[] offers;

	// Token: 0x04001019 RID: 4121
	[ServerToClient]
	public int maxNumOffers;
}
