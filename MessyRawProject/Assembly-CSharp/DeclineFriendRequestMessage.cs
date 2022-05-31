using System;

// Token: 0x020002D1 RID: 721
public class DeclineFriendRequestMessage : Message
{
	// Token: 0x060012C3 RID: 4803 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public DeclineFriendRequestMessage()
	{
	}

	// Token: 0x060012C4 RID: 4804 RVA: 0x0000E19C File Offset: 0x0000C39C
	public DeclineFriendRequestMessage(string requestId)
	{
		this.requestId = requestId;
	}

	// Token: 0x04000F91 RID: 3985
	public string requestId;
}
