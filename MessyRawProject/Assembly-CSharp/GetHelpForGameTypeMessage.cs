using System;

// Token: 0x020002E9 RID: 745
public class GetHelpForGameTypeMessage : Message
{
	// Token: 0x060012E9 RID: 4841 RVA: 0x0000E2CC File Offset: 0x0000C4CC
	public GetHelpForGameTypeMessage(GameType gameType)
	{
		this.gameType = gameType;
	}

	// Token: 0x04000FBB RID: 4027
	public GameType gameType;

	// Token: 0x04000FBC RID: 4028
	public string help;
}
