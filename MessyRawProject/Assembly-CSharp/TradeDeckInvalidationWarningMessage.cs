using System;

// Token: 0x0200036C RID: 876
public class TradeDeckInvalidationWarningMessage : Message
{
	// Token: 0x040010F6 RID: 4342
	[ServerToClient]
	public string[] deckNames;
}
