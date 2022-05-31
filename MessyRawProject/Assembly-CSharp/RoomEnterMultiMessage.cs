using System;
using System.Collections.Generic;

// Token: 0x02000342 RID: 834
public class RoomEnterMultiMessage : Message
{
	// Token: 0x06001382 RID: 4994 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public RoomEnterMultiMessage()
	{
	}

	// Token: 0x06001383 RID: 4995 RVA: 0x0000E7ED File Offset: 0x0000C9ED
	public RoomEnterMultiMessage(string[] roomNames)
	{
		this.roomNames = roomNames;
	}

	// Token: 0x06001384 RID: 4996 RVA: 0x0000E7FC File Offset: 0x0000C9FC
	public RoomEnterMultiMessage(List<string> roomNames) : this(roomNames.ToArray())
	{
	}

	// Token: 0x04001092 RID: 4242
	public string[] roomNames;
}
