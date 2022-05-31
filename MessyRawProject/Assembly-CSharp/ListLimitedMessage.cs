using System;

// Token: 0x020002EC RID: 748
public class ListLimitedMessage : Message
{
	// Token: 0x04000FC3 RID: 4035
	[ServerToClient]
	private DeckInfo[] ongoing;
}
