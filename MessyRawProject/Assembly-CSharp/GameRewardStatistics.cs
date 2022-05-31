using System;

// Token: 0x02000248 RID: 584
public class GameRewardStatistics
{
	// Token: 0x170000EE RID: 238
	// (get) Token: 0x060011A7 RID: 4519 RVA: 0x0000D755 File Offset: 0x0000B955
	public int totalReward
	{
		get
		{
			return this.matchReward + this.tierMatchReward + this.matchCompletionReward + this.idolsDestroyedReward + this.betReward;
		}
	}

	// Token: 0x04000E44 RID: 3652
	public int matchReward;

	// Token: 0x04000E45 RID: 3653
	public int tierMatchReward;

	// Token: 0x04000E46 RID: 3654
	public int matchCompletionReward;

	// Token: 0x04000E47 RID: 3655
	public int idolsDestroyedReward;

	// Token: 0x04000E48 RID: 3656
	public int betReward;
}
