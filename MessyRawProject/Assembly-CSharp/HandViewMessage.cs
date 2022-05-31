using System;

// Token: 0x02000284 RID: 644
public class HandViewMessage : Message
{
	// Token: 0x04000EF5 RID: 3829
	[ServerToClient]
	public BattleMode.SubState type;

	// Token: 0x04000EF6 RID: 3830
	[ServerToClient]
	public Card[] cards;

	// Token: 0x04000EF7 RID: 3831
	[ServerToClient]
	public int maxScrollsForCycle;
}
