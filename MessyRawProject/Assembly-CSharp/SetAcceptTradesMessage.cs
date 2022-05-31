using System;

// Token: 0x02000367 RID: 871
public class SetAcceptTradesMessage : LobbyMessage
{
	// Token: 0x060013C8 RID: 5064 RVA: 0x0000EAAE File Offset: 0x0000CCAE
	public SetAcceptTradesMessage(bool isAccepting)
	{
		this.acceptTrades = isAccepting;
	}

	// Token: 0x040010F3 RID: 4339
	public bool acceptTrades;
}
