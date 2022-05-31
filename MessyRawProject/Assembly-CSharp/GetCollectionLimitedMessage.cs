using System;

// Token: 0x020002E8 RID: 744
public class GetCollectionLimitedMessage : Message
{
	// Token: 0x04000FBA RID: 4026
	[ServerToClient]
	public Card[] cards;
}
