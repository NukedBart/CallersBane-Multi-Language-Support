using System;

// Token: 0x0200028C RID: 652
public class RewardProgressLimitedMessage : Message
{
	// Token: 0x04000F08 RID: 3848
	[ServerToClient]
	public Reward[] rewardLevels;

	// Token: 0x04000F09 RID: 3849
	[ServerToClient]
	public int wins;

	// Token: 0x04000F0A RID: 3850
	[ServerToClient]
	public int winsMax;

	// Token: 0x04000F0B RID: 3851
	[ServerToClient]
	public int losses;

	// Token: 0x04000F0C RID: 3852
	[ServerToClient]
	public int lossesMax;
}
