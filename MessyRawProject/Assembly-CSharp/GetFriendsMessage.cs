using System;

// Token: 0x020002D8 RID: 728
public class GetFriendsMessage : Message
{
	// Token: 0x04000F9F RID: 3999
	[ServerToClient]
	public Person[] friends;
}
