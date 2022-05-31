using System;

// Token: 0x02000305 RID: 773
public class MarketplaceCancelOfferMessage : Message
{
	// Token: 0x0600131A RID: 4890 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public MarketplaceCancelOfferMessage()
	{
	}

	// Token: 0x0600131B RID: 4891 RVA: 0x0000E473 File Offset: 0x0000C673
	public MarketplaceCancelOfferMessage(long offerId)
	{
		this.offerId = offerId;
	}

	// Token: 0x04001004 RID: 4100
	public long offerId;
}
