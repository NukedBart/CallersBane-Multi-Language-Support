using System;

// Token: 0x02000354 RID: 852
public class SpectateGameMessage : Message
{
	// Token: 0x0600139B RID: 5019 RVA: 0x0000E8A6 File Offset: 0x0000CAA6
	public SpectateGameMessage(long gameId)
	{
		this.gameId = gameId;
	}

	// Token: 0x0600139C RID: 5020 RVA: 0x0000D620 File Offset: 0x0000B820
	public override ServerRole allowedServerRoles()
	{
		return ServerRole.GAME;
	}

	// Token: 0x040010B6 RID: 4278
	public long gameId;
}
