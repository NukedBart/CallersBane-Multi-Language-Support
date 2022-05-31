using System;

// Token: 0x0200032C RID: 812
public class PlaySinglePlayerTutorialMessage : PlayGameBaseMessage
{
	// Token: 0x0600135F RID: 4959 RVA: 0x0000E5F2 File Offset: 0x0000C7F2
	public PlaySinglePlayerTutorialMessage()
	{
	}

	// Token: 0x06001360 RID: 4960 RVA: 0x0000E649 File Offset: 0x0000C849
	public PlaySinglePlayerTutorialMessage(int tutorialId)
	{
		this.levelId = tutorialId;
	}

	// Token: 0x04001044 RID: 4164
	public int levelId;
}
