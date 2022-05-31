using System;

// Token: 0x02000355 RID: 853
public class SpectateGameRequestMessage : Message
{
	// Token: 0x0600139D RID: 5021 RVA: 0x0000E8B5 File Offset: 0x0000CAB5
	public SpectateGameRequestMessage(long gameId)
	{
		this.gameId = gameId;
	}

	// Token: 0x040010B7 RID: 4279
	public long gameId;
}
