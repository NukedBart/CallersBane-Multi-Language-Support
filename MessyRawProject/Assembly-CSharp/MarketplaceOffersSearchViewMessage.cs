using System;

// Token: 0x0200030B RID: 779
public class MarketplaceOffersSearchViewMessage : Message
{
	// Token: 0x06001325 RID: 4901 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public MarketplaceOffersSearchViewMessage()
	{
	}

	// Token: 0x06001326 RID: 4902 RVA: 0x0000E4DB File Offset: 0x0000C6DB
	public MarketplaceOffersSearchViewMessage(long cardTypeId)
	{
		this.cardTypeId = cardTypeId;
	}

	// Token: 0x06001327 RID: 4903 RVA: 0x0000E4EA File Offset: 0x0000C6EA
	public MarketplaceOffersSearchViewMessage(long cardTypeId, byte? cardLevel)
	{
		this.cardTypeId = cardTypeId;
		this.cardLevel = cardLevel;
	}

	// Token: 0x04001013 RID: 4115
	public long cardTypeId;

	// Token: 0x04001014 RID: 4116
	public byte? cardLevel;

	// Token: 0x04001015 RID: 4117
	[ServerToClient]
	public MarketplaceOffer offer;

	// Token: 0x04001016 RID: 4118
	[ServerToClient]
	public int copiesForSale;
}
