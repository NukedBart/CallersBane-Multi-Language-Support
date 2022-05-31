using System;

// Token: 0x020002E6 RID: 742
public class DeckSaveLimitedMessage : Message
{
	// Token: 0x060012E4 RID: 4836 RVA: 0x0000E295 File Offset: 0x0000C495
	public DeckSaveLimitedMessage(string deckName, long[] cards) : this(deckName, cards, null)
	{
	}

	// Token: 0x060012E5 RID: 4837 RVA: 0x0000E2A0 File Offset: 0x0000C4A0
	public DeckSaveLimitedMessage(string deckName, long[] cards, string metadata)
	{
		this.name = deckName;
		this.cards = cards;
		this.metadata = metadata;
	}

	// Token: 0x04000FB6 RID: 4022
	public string name;

	// Token: 0x04000FB7 RID: 4023
	public long[] cards;

	// Token: 0x04000FB8 RID: 4024
	public string metadata;
}
