using System;

// Token: 0x0200035A RID: 858
public class SpectatableGameInfo
{
	// Token: 0x060013A6 RID: 5030 RVA: 0x0000E8D8 File Offset: 0x0000CAD8
	public bool isSpectate()
	{
		return !this.isReplay;
	}

	// Token: 0x060013A7 RID: 5031 RVA: 0x0000E8E3 File Offset: 0x0000CAE3
	public string getWinner()
	{
		return ((!this.whiteWon) ? this.blackPlayer : this.whitePlayer).name;
	}

	// Token: 0x040010CC RID: 4300
	public long gameId;

	// Token: 0x040010CD RID: 4301
	public string serverId;

	// Token: 0x040010CE RID: 4302
	public int round;

	// Token: 0x040010CF RID: 4303
	public int? spectators;

	// Token: 0x040010D0 RID: 4304
	public bool? started = new bool?(true);

	// Token: 0x040010D1 RID: 4305
	public long? startTime;

	// Token: 0x040010D2 RID: 4306
	public string date;

	// Token: 0x040010D3 RID: 4307
	public GameType gameType;

	// Token: 0x040010D4 RID: 4308
	public SpectatablePlayerInfo whitePlayer;

	// Token: 0x040010D5 RID: 4309
	public SpectatablePlayerInfo blackPlayer;

	// Token: 0x040010D6 RID: 4310
	public string title;

	// Token: 0x040010D7 RID: 4311
	public bool isReplay;

	// Token: 0x040010D8 RID: 4312
	public bool whiteWon;
}
