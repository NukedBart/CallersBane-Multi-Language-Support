using System;

// Token: 0x020002B6 RID: 694
public class GameChatMessageMessage : ChatMessageMessage, GameNotRequired
{
	// Token: 0x06001284 RID: 4740 RVA: 0x0000DEDA File Offset: 0x0000C0DA
	public GameChatMessageMessage()
	{
	}

	// Token: 0x06001285 RID: 4741 RVA: 0x0000DEE2 File Offset: 0x0000C0E2
	public GameChatMessageMessage(string text)
	{
		base.text = text;
	}
}
