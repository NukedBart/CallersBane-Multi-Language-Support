using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x020000F2 RID: 242
public class CardFilter
{
	// Token: 0x06000809 RID: 2057 RVA: 0x000071E6 File Offset: 0x000053E6
	private CardFilter()
	{
	}

	// Token: 0x0600080B RID: 2059 RVA: 0x00043A8C File Offset: 0x00041C8C
	public bool isIncluded(Card c)
	{
		foreach (CardFilter.Filter filter in this.filters)
		{
			if (!filter(c))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600080C RID: 2060 RVA: 0x00007243 File Offset: 0x00005443
	public List<Card> getFiltered(IEnumerable<Card> cards)
	{
		return this.getFiltered<Card>(cards, (Card c) => c);
	}

	// Token: 0x0600080D RID: 2061 RVA: 0x00043AF8 File Offset: 0x00041CF8
	public List<T> getFiltered<T>(IEnumerable<T> cards, Func<T, Card> cardFunc)
	{
		foreach (CardFilter.CFilter cfilter in this.collectionFilters)
		{
			CardFilter.Filter f = cfilter((this.overriddenCollection == null) ? Enumerable.ToList<Card>(Enumerable.Select<T, Card>(cards, (T tc) => cardFunc.Invoke(tc))) : this.overriddenCollection);
			cards = Enumerable.Where<T>(cards, (T t) => f(cardFunc.Invoke(t)));
		}
		return Enumerable.ToList<T>(Enumerable.Where<T>(cards, (T t) => this.isIncluded(cardFunc.Invoke(t))));
	}

	// Token: 0x0600080E RID: 2062 RVA: 0x00007269 File Offset: 0x00005469
	public CardFilter setOverridedCollectionForFiltering(List<Card> collection)
	{
		this.overriddenCollection = collection;
		return this;
	}

	// Token: 0x0600080F RID: 2063 RVA: 0x00043BD0 File Offset: 0x00041DD0
	private static CardFilter.Filter And(CardFilter.Filter a, CardFilter.Filter b)
	{
		if (a == null || b == null)
		{
			return null;
		}
		return (Card c) => a(c) && b(c);
	}

	// Token: 0x06000810 RID: 2064 RVA: 0x00043C18 File Offset: 0x00041E18
	private static CardFilter.Filter createAmountFilter(string s, CardFilter.AmountFunction f)
	{
		Predicate<int> cf = CardFilter.CreateComparer(s);
		if (cf == null)
		{
			return null;
		}
		return (Card c) => cf.Invoke(f(c));
	}

	// Token: 0x06000811 RID: 2065 RVA: 0x00043C58 File Offset: 0x00041E58
	private static CardFilter.CFilter createCountFilter(string s)
	{
		Predicate<int> f = CardFilter.CreateComparer(s);
		if (f == null)
		{
			return null;
		}
		return delegate(List<Card> cards)
		{
			Dictionary<int, int> typeCount = new Dictionary<int, int>();
			foreach (Card card in cards)
			{
				if (!typeCount.ContainsKey(card.typeId))
				{
					typeCount[card.typeId] = 1;
				}
				else
				{
					typeCount[card.typeId] = typeCount[card.typeId] + 1;
				}
			}
			return delegate(Card c)
			{
				int num;
				typeCount.TryGetValue(c.typeId, ref num);
				return f.Invoke(num);
			};
		};
	}

	// Token: 0x06000812 RID: 2066 RVA: 0x00043C90 File Offset: 0x00041E90
	public static Predicate<int> CreateComparer(string valueString)
	{
		Match match = Regex.Match(valueString, "\\d+");
		if (!match.Success)
		{
			return null;
		}
		int value = int.Parse(match.Value);
		char c2 = valueString.get_Chars(valueString.Length - 1);
		if (c2 == '+')
		{
			return (int c) => c >= value;
		}
		if (c2 == '-')
		{
			return (int c) => c <= value;
		}
		return (int c) => c == value;
	}

	// Token: 0x06000813 RID: 2067 RVA: 0x00007273 File Offset: 0x00005473
	public static CardFilter from(string s)
	{
		return CardFilter.from(CardFilter.SplitPairs(s));
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x00043D14 File Offset: 0x00041F14
	public static CardFilter from(string[] strings)
	{
		CardFilter cardFilter = new CardFilter();
		for (int i = 0; i < strings.Length; i++)
		{
			string text = strings[i];
			string s = text.Trim().ToLower();
			if (s.Length != 0)
			{
				string text2;
				string text3;
				CardFilter.PairToKeyValue(s, out text2, out text3);
				if (CardFilter.isCollectionFilterKey(text2))
				{
					CardFilter.CFilter cfilter = null;
					if (text2 == "#" || text2 == "count")
					{
						cfilter = CardFilter.createCountFilter(text3);
					}
					if (cfilter != null)
					{
						cardFilter.collectionFilters.Add(cfilter);
					}
				}
				else
				{
					CardFilter.Filter filter = CardFilter.createCardFilter(text2, text3);
					if (filter == CardFilter.NotHandledFilter)
					{
						filter = ((Card c) => c.getNameLower().Contains(s));
					}
					if (filter != null)
					{
						cardFilter.filters.Add(filter);
					}
				}
			}
		}
		return cardFilter;
	}

	// Token: 0x06000815 RID: 2069 RVA: 0x00043E14 File Offset: 0x00042014
	private static CardFilter.Filter createCardFilter(string key, string value)
	{
		if (key == "t" || key == "type")
		{
			return (Card c) => (c.getPieceKindText() + c.getPieceType()).ToLower().Contains(value);
		}
		if (key == "r" || key == "rarity")
		{
			return (Card c) => c.getRarityString().ToLower().StartsWith(value);
		}
		if (key == "d" || key == "desc")
		{
			return (Card c) => c.getDescriptionAndAbilitiesLowerCase().Contains(value);
		}
		if (key == "fl" || key == "flavor")
		{
			return (Card c) => c.getDescriptionFlavor().ToLower().Contains(value);
		}
		if (key == "tagk")
		{
			return (Card c) => string.Join(" ", Enumerable.ToArray<string>(c.getCardType().tags.Keys)).Contains(value);
		}
		if (key == "tagv")
		{
			return (Card c) => string.Join(" ", StringUtil.toStringArray<object>(c.getCardType().tags.Values)).Contains(value);
		}
		if (key == "l" || key == "tier" || key == "level")
		{
			return CardFilter.createAmountFilter(value, (Card c) => c.getTier());
		}
		if (key == "s" || key == "set")
		{
			return CardFilter.createAmountFilter(value, (Card c) => c.getCardType().set);
		}
		if (key == "c" || key == "cost")
		{
			return CardFilter.createAmountFilter(value, (Card c) => c.getCostTotal());
		}
		if (key == "ap" || key == "attack")
		{
			return CardFilter.And((Card c) => c.getPieceKind().isUnit(), CardFilter.createAmountFilter(value, (Card c) => c.getAttackPower()));
		}
		if (key == "cd" || key == "countdown")
		{
			return CardFilter.And((Card c) => c.getPieceKind().isUnit(), CardFilter.createAmountFilter(value, (Card c) => c.getAttackInterval()));
		}
		if (key == "hp" || key == "health")
		{
			return CardFilter.And((Card c) => c.getPieceKind().isUnit(), CardFilter.createAmountFilter(value, (Card c) => c.getHitPoints()));
		}
		if (key == "$" || key == "€" || key == "g" || key == "gold")
		{
			return CardFilter.createAmountFilter(value, (Card c) => (c.data == null) ? 0 : c.dataAs<int>());
		}
		if (key == "res" || key == "resource" || key == "f" || key == "faction")
		{
			string[] res = value.Split(new char[]
			{
				','
			});
			return delegate(Card c)
			{
				if (value.Length == 0)
				{
					return true;
				}
				string text = c.getCardType().getResource().ToString().ToLower();
				string[] res;
				foreach (string text2 in res)
				{
					if (text2.Length > 0 && text.StartsWith(text2))
					{
						return true;
					}
				}
				return false;
			};
		}
		return CardFilter.NotHandledFilter;
	}

	// Token: 0x06000816 RID: 2070 RVA: 0x00007280 File Offset: 0x00005480
	private static bool isCollectionFilterKey(string key)
	{
		return Enumerable.Contains<string>(CardFilter.collectionFilterKeys, key);
	}

	// Token: 0x06000817 RID: 2071 RVA: 0x0000728D File Offset: 0x0000548D
	public static string[] SplitPairs(string s)
	{
		return Regex.Replace(s, ":\\s*", ":").Split(new char[]
		{
			' '
		});
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x0004422C File Offset: 0x0004242C
	public static bool PairToKeyValue(string s, out string key, out string value)
	{
		s = s.Trim();
		int num = s.IndexOf(':');
		if (num < 0)
		{
			string empty;
			value = (empty = string.Empty);
			key = empty;
			return false;
		}
		key = s.Substring(0, num);
		value = s.Substring(Mathf.Min(num + 1, key.Length + 1));
		return true;
	}

	// Token: 0x040005E7 RID: 1511
	private List<CardFilter.Filter> filters = new List<CardFilter.Filter>();

	// Token: 0x040005E8 RID: 1512
	private List<CardFilter.CFilter> collectionFilters = new List<CardFilter.CFilter>();

	// Token: 0x040005E9 RID: 1513
	private List<Card> overriddenCollection;

	// Token: 0x040005EA RID: 1514
	private static CardFilter.Filter NotHandledFilter = (Card c) => true;

	// Token: 0x040005EB RID: 1515
	private static readonly string[] collectionFilterKeys = new string[]
	{
		"#",
		"count"
	};

	// Token: 0x020000F3 RID: 243
	// (Invoke) Token: 0x06000826 RID: 2086
	private delegate bool Filter(Card c);

	// Token: 0x020000F4 RID: 244
	// (Invoke) Token: 0x0600082A RID: 2090
	private delegate CardFilter.Filter CFilter(List<Card> cards);

	// Token: 0x020000F5 RID: 245
	// (Invoke) Token: 0x0600082E RID: 2094
	private delegate int AmountFunction(Card c);
}
