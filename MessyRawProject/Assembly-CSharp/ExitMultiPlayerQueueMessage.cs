using System;

// Token: 0x020002AC RID: 684
public class ExitMultiPlayerQueueMessage : GameChallengeBaseMessage
{
	// Token: 0x06001273 RID: 4723 RVA: 0x0000DE05 File Offset: 0x0000C005
	public ExitMultiPlayerQueueMessage()
	{
	}

	// Token: 0x06001274 RID: 4724 RVA: 0x0000DE0D File Offset: 0x0000C00D
	public ExitMultiPlayerQueueMessage(GameType gameType)
	{
		this.gameType = new GameType?(gameType);
	}

	// Token: 0x04000F4B RID: 3915
	public GameType? gameType;
}
