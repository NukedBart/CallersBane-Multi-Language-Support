using System;
using System.Collections.Generic;
using System.Linq;
using JsonFx.Json;

// Token: 0x020001A2 RID: 418
public class DeckPortation
{
	// Token: 0x06000D1B RID: 3355 RVA: 0x0005CE98 File Offset: 0x0005B098
	public static DeckPortation.Deck fromList(string deck, string author, IList<Card> cards)
	{
		DeckPortation.Deck deck2 = new DeckPortation.Deck();
		deck2.deck = deck;
		deck2.author = author;
		deck2.types = Enumerable.ToArray<int>(Enumerable.Select<Card, int>(cards, (Card c) => c.getType()));
		return deck2;
	}

	// Token: 0x06000D1C RID: 3356 RVA: 0x0005CEE8 File Offset: 0x0005B0E8
	public static DeckPortation.Deck fromJson(string s)
	{
		return new JsonReader().Read<DeckPortation.Deck>(s);
	}

	// Token: 0x06000D1D RID: 3357 RVA: 0x0005CF04 File Offset: 0x0005B104
	public static DeckPortation.Deck fromScrollsGuide(string deckName, string author, string s)
	{
		DeckPortation.Deck deck = new DeckPortation.Deck();
		deck.deck = deckName;
		deck.author = author;
		List<int> list = new List<int>();
		string[] array = s.Split(new char[]
		{
			'\n'
		});
		foreach (string text in array)
		{
			int num = text.IndexOf(" ");
			if (num != -1)
			{
				int num2;
				if (int.TryParse(text.Substring(0, num - 1), ref num2))
				{
					string name = text.Substring(num + 1).Trim();
					int id = CardTypeManager.getInstance().get(name).id;
					for (int j = 0; j < num2; j++)
					{
						list.Add(id);
					}
				}
			}
		}
		deck.types = list.ToArray();
		return deck;
	}

	// Token: 0x020001A3 RID: 419
	public class Deck
	{
		// Token: 0x06000D1F RID: 3359 RVA: 0x0000A763 File Offset: 0x00008963
		public Deck()
		{
		}

		// Token: 0x06000D20 RID: 3360 RVA: 0x0000A78D File Offset: 0x0000898D
		public Deck(int[] types)
		{
			this.types = types;
		}

		// Token: 0x06000D21 RID: 3361 RVA: 0x0000A7BE File Offset: 0x000089BE
		public List<Card> getCards(IList<Card> collection)
		{
			return this.getCards(collection, null);
		}

		// Token: 0x06000D22 RID: 3362 RVA: 0x0005CFE8 File Offset: 0x0005B1E8
		public List<Card> getCards(IList<Card> collection, IList<int> missing)
		{
			List<Card> list = new List<Card>(collection);
			DeckSorter deckSorter = new DeckSorter().byName().byLevel().byTradable();
			list.Sort(deckSorter);
			DepletingMultiMapQuery<int, Card> depletingMultiMapQuery = new DepletingMultiMapQuery<int, Card>();
			foreach (Card card in list)
			{
				depletingMultiMapQuery.Add(card.getType(), card);
			}
			List<Card> list2 = new List<Card>();
			foreach (int num in this.types)
			{
				if (depletingMultiMapQuery.hasNext(num))
				{
					list2.Add(depletingMultiMapQuery.getNext(num));
				}
				else if (missing != null)
				{
					missing.Add(num);
				}
			}
			return list2;
		}

		// Token: 0x06000D23 RID: 3363 RVA: 0x0000A7C8 File Offset: 0x000089C8
		public string toJson()
		{
			return new JsonWriter().Write(this);
		}

		// Token: 0x04000A37 RID: 2615
		public string deck = "Unnamed";

		// Token: 0x04000A38 RID: 2616
		public string author = string.Empty;

		// Token: 0x04000A39 RID: 2617
		public int[] types = new int[0];
	}
}
