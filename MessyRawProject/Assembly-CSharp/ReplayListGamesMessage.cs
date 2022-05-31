using System;

// Token: 0x02000358 RID: 856
public class ReplayListGamesMessage : LobbyMessage
{
	// Token: 0x060013A0 RID: 5024 RVA: 0x0000DD89 File Offset: 0x0000BF89
	public ReplayListGamesMessage()
	{
	}

	// Token: 0x060013A1 RID: 5025 RVA: 0x00078C0C File Offset: 0x00076E0C
	public ReplayListGamesMessage(string name) : this(name, default(GameType?))
	{
	}

	// Token: 0x060013A2 RID: 5026 RVA: 0x00078C2C File Offset: 0x00076E2C
	public ReplayListGamesMessage(string name, GameType? gameType)
	{
		this.profileName = name;
		this.gameType = ((!(gameType != GameType.None)) ? default(GameType?) : gameType);
	}

	// Token: 0x040010BB RID: 4283
	public string profileName;

	// Token: 0x040010BC RID: 4284
	public GameType? gameType;

	// Token: 0x040010BD RID: 4285
	public ReplayGameInfo[] replays;

	// Token: 0x040010BE RID: 4286
	public ServerIdAddress[] servers;
}
