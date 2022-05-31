using System;

// Token: 0x020002B7 RID: 695
public class RoomChatMessageMessage : ChatMessageMessage
{
	// Token: 0x06001286 RID: 4742 RVA: 0x0000DEDA File Offset: 0x0000C0DA
	public RoomChatMessageMessage()
	{
	}

	// Token: 0x06001287 RID: 4743 RVA: 0x0000DEF1 File Offset: 0x0000C0F1
	public RoomChatMessageMessage(string room, string text)
	{
		this.roomName = room;
		base.text = text;
	}

	// Token: 0x04000F5D RID: 3933
	public string roomName;
}
