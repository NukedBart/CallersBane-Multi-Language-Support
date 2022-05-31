using System;

// Token: 0x02000246 RID: 582
public class EMEndGame : EffectMessage
{
	// Token: 0x060011A1 RID: 4513 RVA: 0x0000D6F2 File Offset: 0x0000B8F2
	public GameStatistics getGameStatistics(TileColor color)
	{
		if (color == TileColor.white)
		{
			return this.whiteStats;
		}
		if (color == TileColor.black)
		{
			return this.blackStats;
		}
		return null;
	}

	// Token: 0x060011A2 RID: 4514 RVA: 0x0000D710 File Offset: 0x0000B910
	public GameRewardStatistics getGameRewardStatistics(TileColor color)
	{
		if (color == TileColor.white)
		{
			return this.whiteGoldReward;
		}
		if (color == TileColor.black)
		{
			return this.blackGoldReward;
		}
		return null;
	}

	// Token: 0x060011A3 RID: 4515 RVA: 0x0000D72E File Offset: 0x0000B92E
	public Card[] cardReward(bool isWinner)
	{
		return (!isWinner) ? this.loserCardRewards : this.cardRewards;
	}

	// Token: 0x060011A4 RID: 4516 RVA: 0x0000D747 File Offset: 0x0000B947
	public bool hasCardReward(bool isWinner)
	{
		return this.cardReward(isWinner).Length > 0;
	}

	// Token: 0x04000E30 RID: 3632
	public TileColor winner;

	// Token: 0x04000E31 RID: 3633
	public GameStatistics whiteStats;

	// Token: 0x04000E32 RID: 3634
	public GameStatistics blackStats;

	// Token: 0x04000E33 RID: 3635
	public GameRewardStatistics whiteGoldReward;

	// Token: 0x04000E34 RID: 3636
	public GameRewardStatistics blackGoldReward;

	// Token: 0x04000E35 RID: 3637
	public TowerLevel challengeInfo;

	// Token: 0x04000E36 RID: 3638
	public Card[] cardRewards = new Card[0];

	// Token: 0x04000E37 RID: 3639
	public Card[] loserCardRewards = new Card[0];

	// Token: 0x04000E38 RID: 3640
	public CustomGameInfo lossGame;

	// Token: 0x04000E39 RID: 3641
	public CustomGameInfo nextGame;
}
