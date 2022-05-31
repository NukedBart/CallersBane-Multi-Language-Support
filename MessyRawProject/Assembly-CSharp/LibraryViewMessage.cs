using System;
using System.Collections.Generic;

// Token: 0x020002A9 RID: 681
public class LibraryViewMessage : LobbyMessage
{
	// Token: 0x0600126E RID: 4718 RVA: 0x000059E4 File Offset: 0x00003BE4
	public override bool shouldLog()
	{
		return false;
	}

	// Token: 0x0600126F RID: 4719 RVA: 0x00077EC8 File Offset: 0x000760C8
	public Card[] getTradableCards()
	{
		if (this.cards == null)
		{
			return new Card[0];
		}
		List<Card> list = new List<Card>();
		foreach (Card card in this.cards)
		{
			if (card.tradable)
			{
				list.Add(card);
			}
		}
		return list.ToArray();
	}

	// Token: 0x04000F46 RID: 3910
	public Card[] cards;

	// Token: 0x04000F47 RID: 3911
	public int profileId;
}
