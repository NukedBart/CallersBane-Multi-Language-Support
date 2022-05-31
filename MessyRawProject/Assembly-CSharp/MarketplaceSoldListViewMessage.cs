using System;

// Token: 0x0200030D RID: 781
public class MarketplaceSoldListViewMessage : Message
{
	// Token: 0x0400101A RID: 4122
	[ServerToClient]
	public TransactionInfo[] sold;
}
