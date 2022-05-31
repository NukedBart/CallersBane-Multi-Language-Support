using System;

// Token: 0x02000275 RID: 629
public class EMGameState : InternalEffectMessage
{
	// Token: 0x0600120E RID: 4622 RVA: 0x0000DAB9 File Offset: 0x0000BCB9
	public EMGameState(GameStateMessage state)
	{
		this.state = state;
	}

	// Token: 0x04000EBF RID: 3775
	public GameStateMessage state;
}
