using System;

// Token: 0x0200029E RID: 670
public class DeckCardsMessage : Message
{
	// Token: 0x06001256 RID: 4694 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public DeckCardsMessage()
	{
	}

	// Token: 0x06001257 RID: 4695 RVA: 0x0000DD4E File Offset: 0x0000BF4E
	public DeckCardsMessage(string deckName)
	{
		this.deck = deckName;
	}

	// Token: 0x04000F32 RID: 3890
	public string deck;

	// Token: 0x04000F33 RID: 3891
	public string metadata;

	// Token: 0x04000F34 RID: 3892
	public ResourceType[] resources;

	// Token: 0x04000F35 RID: 3893
	[ServerToClient]
	public bool valid;

	// Token: 0x04000F36 RID: 3894
	public Card[] cards;
}
