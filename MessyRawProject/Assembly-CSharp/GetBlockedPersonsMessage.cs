using System;

// Token: 0x020002D6 RID: 726
public class GetBlockedPersonsMessage : Message
{
	// Token: 0x04000F9D RID: 3997
	[ServerToClient]
	public string[] blocked;
}
