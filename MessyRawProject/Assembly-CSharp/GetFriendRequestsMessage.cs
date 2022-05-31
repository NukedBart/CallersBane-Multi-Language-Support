using System;

// Token: 0x020002D7 RID: 727
public class GetFriendRequestsMessage : Message
{
	// Token: 0x04000F9E RID: 3998
	[ServerToClient]
	public Request[] requests;
}
