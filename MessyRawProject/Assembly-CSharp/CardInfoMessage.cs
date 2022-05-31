using System;

// Token: 0x02000238 RID: 568
public class CardInfoMessage : Message
{
	// Token: 0x06001186 RID: 4486 RVA: 0x0007746C File Offset: 0x0007566C
	public static long[] getIds(CardInfoMessage[] cards)
	{
		if (cards == null)
		{
			return null;
		}
		long[] array = new long[cards.Length];
		for (int i = 0; i < cards.Length; i++)
		{
			array[i] = cards[i].card.id;
		}
		return array;
	}

	// Token: 0x04000E05 RID: 3589
	public Card card;

	// Token: 0x04000E06 RID: 3590
	public bool hasEnoughResources;

	// Token: 0x04000E07 RID: 3591
	public DataInfo data = new DataInfo();
}
