using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x020002F1 RID: 753
public class SelectRewardLimitedMessage : Message
{
	// Token: 0x060012F5 RID: 4853 RVA: 0x0000E325 File Offset: 0x0000C525
	public SelectRewardLimitedMessage(string deck, List<Card> cards)
	{
		this.deck = deck;
		this.cards = Enumerable.ToArray<long>(Enumerable.Select<Card, long>(cards, (Card c) => c.id));
	}

	// Token: 0x04000FD0 RID: 4048
	public string deck;

	// Token: 0x04000FD1 RID: 4049
	public long[] cards;
}
