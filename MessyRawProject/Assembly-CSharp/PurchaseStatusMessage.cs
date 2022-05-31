using System;

// Token: 0x0200033C RID: 828
public class PurchaseStatusMessage : Message
{
	// Token: 0x04001087 RID: 4231
	[ServerToClient]
	public string variantId;

	// Token: 0x04001088 RID: 4232
	[ServerToClient]
	public OrderStatus orderStatus;

	// Token: 0x04001089 RID: 4233
	[ServerToClient]
	public int quantity;

	// Token: 0x0400108A RID: 4234
	[ServerToClient]
	public string title;

	// Token: 0x0400108B RID: 4235
	[ServerToClient]
	public string text;
}
