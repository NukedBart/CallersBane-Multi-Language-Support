using System;

// Token: 0x020002FD RID: 765
public class OverallStatsMessage : Message
{
	// Token: 0x0600130E RID: 4878 RVA: 0x000785C0 File Offset: 0x000767C0
	public OverallStatsMessage.RankedPlayer[] getTopList()
	{
		OverallStatsMessage.RankedPlayer[] array = new OverallStatsMessage.RankedPlayer[this.topRanked.Length];
		int num = -99999999;
		int rank = 0;
		for (int i = 0; i < this.topRanked.Length; i++)
		{
			if (this.topRanked[i].rating != num)
			{
				num = this.topRanked[i].rating;
				rank = i + 1;
			}
			array[i] = this.topRanked[i].clone();
			array[i].rank = rank;
		}
		return array;
	}

	// Token: 0x04000FF0 RID: 4080
	[ServerToClient]
	public string serverName;

	// Token: 0x04000FF1 RID: 4081
	[ServerToClient]
	public long loginsLast24h;

	// Token: 0x04000FF2 RID: 4082
	[ServerToClient]
	public OverallStatsMessage.RankedPlayer[] topRanked;

	// Token: 0x04000FF3 RID: 4083
	[ServerToClient]
	public int nrOfProfiles;

	// Token: 0x04000FF4 RID: 4084
	[ServerToClient]
	public WeeklyWinner[] weeklyWinners;

	// Token: 0x020002FE RID: 766
	public class RankedPlayer
	{
		// Token: 0x06001310 RID: 4880 RVA: 0x0007863C File Offset: 0x0007683C
		public OverallStatsMessage.RankedPlayer clone()
		{
			return new OverallStatsMessage.RankedPlayer
			{
				name = this.name,
				rating = this.rating,
				rank = this.rank,
				gameId = this.gameId
			};
		}

		// Token: 0x04000FF5 RID: 4085
		public string name;

		// Token: 0x04000FF6 RID: 4086
		public int rating;

		// Token: 0x04000FF7 RID: 4087
		public int rank;

		// Token: 0x04000FF8 RID: 4088
		public long? gameId;
	}
}
