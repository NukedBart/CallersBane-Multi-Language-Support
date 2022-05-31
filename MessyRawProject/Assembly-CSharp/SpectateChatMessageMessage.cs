using System;

// Token: 0x0200028D RID: 653
public class SpectateChatMessageMessage : Message
{
	// Token: 0x06001237 RID: 4663 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public SpectateChatMessageMessage()
	{
	}

	// Token: 0x06001238 RID: 4664 RVA: 0x0000DC63 File Offset: 0x0000BE63
	public SpectateChatMessageMessage(string text)
	{
		this.text = text;
	}

	// Token: 0x06001239 RID: 4665 RVA: 0x00077C04 File Offset: 0x00075E04
	public static SpectateChatMessageMessage FromGameChatMessage(GameChatMessageMessage m)
	{
		return new SpectateChatMessageMessage(m.text)
		{
			from = m.from,
			fromPlayer = true
		};
	}

	// Token: 0x04000F0D RID: 3853
	public string from;

	// Token: 0x04000F0E RID: 3854
	public string text;

	// Token: 0x04000F0F RID: 3855
	[ServerToClient]
	public bool fromPlayer;
}
