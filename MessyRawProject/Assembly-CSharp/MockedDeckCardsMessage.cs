using System;
using System.Linq;

// Token: 0x020002A0 RID: 672
public class MockedDeckCardsMessage : DeckCardsMessage
{
	// Token: 0x06001259 RID: 4697 RVA: 0x00077D08 File Offset: 0x00075F08
	public MockedDeckCardsMessage(DeckSaveMessage m)
	{
		this.deck = m.name;
		this.metadata = m.metadata;
		CardType ct = new CardType();
		this.cards = Enumerable.ToArray<Card>(Enumerable.Select<string, Card>(m.cards, (string s) => new Card((long)int.Parse(s), ct)));
	}
}
