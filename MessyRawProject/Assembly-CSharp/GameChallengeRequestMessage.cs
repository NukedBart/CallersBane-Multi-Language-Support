using System;

// Token: 0x020002B1 RID: 689
public class GameChallengeRequestMessage : AbstractGameChallengeMessage
{
	// Token: 0x0600127B RID: 4731 RVA: 0x0000DE46 File Offset: 0x0000C046
	public GameChallengeRequestMessage(int profileId)
	{
		this.profileId = profileId;
	}

	// Token: 0x0600127C RID: 4732 RVA: 0x0000DE5C File Offset: 0x0000C05C
	public GameChallengeRequestMessage(int profileId, int customGameId)
	{
		this.profileId = profileId;
		this.customGameId = customGameId;
	}

	// Token: 0x04000F4F RID: 3919
	public int customGameId = -1;

	// Token: 0x04000F50 RID: 3920
	public bool chooseDeck;
}
