using System;
using System.Collections.Generic;

namespace DeckExtensionMethods
{
	// Token: 0x020001A1 RID: 417
	public static class Collection
	{
		// Token: 0x06000D13 RID: 3347 RVA: 0x0005CBF0 File Offset: 0x0005ADF0
		public static List<Card> getCardsAt(this List<Card> cards, int index)
		{
			int num = -1;
			long num2 = -1L;
			for (int i = 0; i < cards.Count; i++)
			{
				if ((long)cards[i].getType() != num2)
				{
					num2 = (long)cards[i].getType();
					if (++num == index)
					{
						List<Card> list = new List<Card>();
						int num3 = i;
						while (num3 < cards.Count && num2 == (long)cards[num3].getType())
						{
							list.Add(cards[num3]);
							num3++;
						}
						return list;
					}
				}
			}
			return null;
		}

		// Token: 0x06000D14 RID: 3348 RVA: 0x0005CC8C File Offset: 0x0005AE8C
		public static int getTypeIndex(this List<Card> cards, long type)
		{
			long num = -1L;
			int i = 0;
			int num2 = 0;
			while (i < cards.Count)
			{
				int type2 = cards[i].getType();
				if ((long)type2 != num)
				{
					if ((long)type2 == type)
					{
						return num2;
					}
					num = (long)type2;
					num2++;
				}
				i++;
			}
			return -1;
		}

		// Token: 0x06000D15 RID: 3349 RVA: 0x0005CCDC File Offset: 0x0005AEDC
		public static int getTypeIndexForCardIndex(this List<Card> cards, int cardIndex)
		{
			long num = -1L;
			int i = 0;
			int num2 = -1;
			while (i < cards.Count)
			{
				int type = cards[i].getType();
				if ((long)type != num)
				{
					num = (long)type;
					num2++;
				}
				if (i == cardIndex)
				{
					return num2;
				}
				i++;
			}
			return -1;
		}

		// Token: 0x06000D16 RID: 3350 RVA: 0x0005CD2C File Offset: 0x0005AF2C
		public static int getTypeSubIndexForCardIndex(this List<Card> cards, int cardIndex)
		{
			int num = 0;
			long num2 = (long)cards[cardIndex].getType();
			while (cardIndex > 0 && (long)cards[--cardIndex].getType() == num2)
			{
				num++;
			}
			return num;
		}

		// Token: 0x06000D17 RID: 3351 RVA: 0x0005CD74 File Offset: 0x0005AF74
		public static int getCardIndexForTypeIndex(this List<Card> cards, long typeIndex)
		{
			long num = -1L;
			int i = 0;
			int num2 = 0;
			while (i < cards.Count)
			{
				long num3 = (long)cards[i].getType();
				if (num3 != num)
				{
					if ((long)num2 == typeIndex)
					{
						return i;
					}
					num = num3;
					num2++;
				}
				i++;
			}
			return -1;
		}

		// Token: 0x06000D18 RID: 3352 RVA: 0x0005CDC4 File Offset: 0x0005AFC4
		public static int getNumTypes(this List<Card> cards)
		{
			int num = -1;
			int num2 = 0;
			for (int i = 0; i < cards.Count; i++)
			{
				int type = cards[i].getType();
				if (type != num)
				{
					num = type;
					num2++;
				}
			}
			return num2;
		}

		// Token: 0x06000D19 RID: 3353 RVA: 0x0005CE08 File Offset: 0x0005B008
		public static int getNumOf(this List<Card> cards, int typeIndex)
		{
			int num = -1;
			int num2 = 0;
			for (int i = 0; i < cards.Count; i++)
			{
				int type = cards[i].getType();
				if (type != num)
				{
					if (num2 == typeIndex)
					{
						int num3 = 0;
						for (int j = i; j < cards.Count; j++)
						{
							if (cards[j].getType() != type)
							{
								break;
							}
							num3++;
						}
						return num3;
					}
					num = type;
					num2++;
				}
			}
			return 0;
		}
	}
}
