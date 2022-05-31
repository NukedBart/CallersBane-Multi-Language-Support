using System;

// Token: 0x02000357 RID: 855
public class SpectateListGamesMessage : LobbyMessage
{
	// Token: 0x040010B9 RID: 4281
	[ServerToClient]
	public SpectatableGameInfo[] spectatable;

	// Token: 0x040010BA RID: 4282
	public long currentTime;
}
