using System;

// Token: 0x02000346 RID: 838
public class RoomMessage : LobbyMessage
{
	// Token: 0x06001389 RID: 5001 RVA: 0x0000DD89 File Offset: 0x0000BF89
	public RoomMessage()
	{
	}

	// Token: 0x0600138A RID: 5002 RVA: 0x0000E82A File Offset: 0x0000CA2A
	public RoomMessage(string roomName)
	{
		this.roomName = roomName;
	}

	// Token: 0x0400109C RID: 4252
	public string roomName;
}
