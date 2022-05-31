using System;

// Token: 0x0200027F RID: 639
public class GameStateMessage : Message
{
	// Token: 0x06001223 RID: 4643 RVA: 0x0000DBF1 File Offset: 0x0000BDF1
	public PlayerGameState getState(TileColor color)
	{
		if (color == TileColor.white)
		{
			return this.whiteGameState;
		}
		if (color == TileColor.black)
		{
			return this.blackGameState;
		}
		return null;
	}

	// Token: 0x04000EDD RID: 3805
	public PlayerGameState whiteGameState;

	// Token: 0x04000EDE RID: 3806
	public PlayerGameState blackGameState;

	// Token: 0x04000EDF RID: 3807
	[ServerToClient]
	public TileColor activeColor;

	// Token: 0x04000EE0 RID: 3808
	[ServerToClient]
	public EndPhaseMessage.Phase phase;

	// Token: 0x04000EE1 RID: 3809
	[ServerToClient]
	public int turn;

	// Token: 0x04000EE2 RID: 3810
	[ServerToClient]
	public bool hasSacrificed;

	// Token: 0x04000EE3 RID: 3811
	[ServerToClient]
	public int secondsLeft;
}
