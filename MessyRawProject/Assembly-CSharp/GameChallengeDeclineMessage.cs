using System;

// Token: 0x020002AF RID: 687
public class GameChallengeDeclineMessage : GameChallengeBaseMessage
{
	// Token: 0x06001278 RID: 4728 RVA: 0x0000DE29 File Offset: 0x0000C029
	public GameChallengeDeclineMessage(int profileId)
	{
		this.profileId = profileId;
	}

	// Token: 0x04000F4C RID: 3916
	public int profileId;
}
