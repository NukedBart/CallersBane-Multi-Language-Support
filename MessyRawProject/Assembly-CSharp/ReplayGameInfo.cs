using System;

// Token: 0x02000359 RID: 857
public class ReplayGameInfo
{
	// Token: 0x060013A4 RID: 5028 RVA: 0x00078C78 File Offset: 0x00076E78
	public SpectatableGameInfo toSpectateInfo()
	{
		return new SpectatableGameInfo
		{
			isReplay = true,
			gameId = this.gameId,
			gameType = this.gameType,
			serverId = this.serverId,
			title = this.title,
			date = this.date,
			round = this.round,
			whiteWon = this.whiteWinner,
			whitePlayer = new SpectatablePlayerInfo(),
			whitePlayer = 
			{
				name = this.whiteProfileName,
				rating = this.whiteRating,
				resources = this.whiteResources
			},
			blackPlayer = new SpectatablePlayerInfo(),
			blackPlayer = 
			{
				name = this.blackProfileName,
				rating = this.blackRating,
				resources = this.blackResources
			}
		};
	}

	// Token: 0x040010BF RID: 4287
	public long gameId;

	// Token: 0x040010C0 RID: 4288
	public string serverId;

	// Token: 0x040010C1 RID: 4289
	public GameType gameType;

	// Token: 0x040010C2 RID: 4290
	public int round;

	// Token: 0x040010C3 RID: 4291
	public string title;

	// Token: 0x040010C4 RID: 4292
	public string date;

	// Token: 0x040010C5 RID: 4293
	public bool whiteWinner;

	// Token: 0x040010C6 RID: 4294
	public string whiteProfileName;

	// Token: 0x040010C7 RID: 4295
	public string blackProfileName;

	// Token: 0x040010C8 RID: 4296
	public string whiteResources;

	// Token: 0x040010C9 RID: 4297
	public string blackResources;

	// Token: 0x040010CA RID: 4298
	public int whiteRating;

	// Token: 0x040010CB RID: 4299
	public int blackRating;
}
