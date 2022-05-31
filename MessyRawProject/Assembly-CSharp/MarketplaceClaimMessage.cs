using System;

// Token: 0x02000306 RID: 774
public class MarketplaceClaimMessage : Message
{
	// Token: 0x0600131C RID: 4892 RVA: 0x0000E482 File Offset: 0x0000C682
	public MarketplaceClaimMessage(long transactionId)
	{
		this.transactionId = transactionId;
	}

	// Token: 0x04001005 RID: 4101
	public long transactionId;
}
