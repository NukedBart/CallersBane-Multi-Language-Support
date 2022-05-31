using System;

// Token: 0x02000307 RID: 775
public class MarketplaceCreateOfferInfoMessage : Message
{
	// Token: 0x0600131D RID: 4893 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public MarketplaceCreateOfferInfoMessage()
	{
	}

	// Token: 0x0600131E RID: 4894 RVA: 0x0000E491 File Offset: 0x0000C691
	public MarketplaceCreateOfferInfoMessage(int cardTypeId, byte cardLevel)
	{
		this.cardTypeId = cardTypeId;
		this.cardLevel = cardLevel;
	}

	// Token: 0x0600131F RID: 4895 RVA: 0x0000E4A7 File Offset: 0x0000C6A7
	public static int amountToKeep(int totalAmount, float tax)
	{
		return totalAmount - (int)(tax * (float)totalAmount + 0.001f);
	}

	// Token: 0x04001006 RID: 4102
	public int cardTypeId;

	// Token: 0x04001007 RID: 4103
	public byte cardLevel;

	// Token: 0x04001008 RID: 4104
	[ServerToClient]
	public int copiesForSale;

	// Token: 0x04001009 RID: 4105
	[ServerToClient]
	public int lowestPrice;

	// Token: 0x0400100A RID: 4106
	[ServerToClient]
	public int suggestedPrice;

	// Token: 0x0400100B RID: 4107
	[ServerToClient]
	public float tax;
}
