using System;

// Token: 0x02000321 RID: 801
public class PlaySinglePlayerCustomQuickmatchMessage : PlayGameBaseMessage
{
	// Token: 0x0600134B RID: 4939 RVA: 0x0000E5CD File Offset: 0x0000C7CD
	public PlaySinglePlayerCustomQuickmatchMessage()
	{
	}

	// Token: 0x0600134C RID: 4940 RVA: 0x0000E5DC File Offset: 0x0000C7DC
	public PlaySinglePlayerCustomQuickmatchMessage(int customGameId)
	{
		this.customGameId = customGameId;
	}

	// Token: 0x0400103E RID: 4158
	public int customGameId = -1;

	// Token: 0x0400103F RID: 4159
	public AiDifficulty difficulty;
}
