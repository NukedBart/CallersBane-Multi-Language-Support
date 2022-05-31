using System;

// Token: 0x0200022E RID: 558
public class EndPhaseMessage : BattleAction
{
	// Token: 0x06001177 RID: 4471 RVA: 0x00077448 File Offset: 0x00075648
	public EndPhaseMessage()
	{
		this.phase = default(EndPhaseMessage.Phase?);
	}

	// Token: 0x06001178 RID: 4472 RVA: 0x0000D576 File Offset: 0x0000B776
	public EndPhaseMessage(EndPhaseMessage.Phase phase)
	{
		this.phase = new EndPhaseMessage.Phase?(phase);
	}

	// Token: 0x04000DF8 RID: 3576
	public EndPhaseMessage.Phase? phase;

	// Token: 0x0200022F RID: 559
	public enum Phase
	{
		// Token: 0x04000DFA RID: 3578
		Init,
		// Token: 0x04000DFB RID: 3579
		PreMain,
		// Token: 0x04000DFC RID: 3580
		Main,
		// Token: 0x04000DFD RID: 3581
		End
	}
}
