using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x020002AA RID: 682
public class UpgradeCardMessage : Message
{
	// Token: 0x06001270 RID: 4720 RVA: 0x0000DDCB File Offset: 0x0000BFCB
	public UpgradeCardMessage(long[] cardIds)
	{
		if (cardIds != null)
		{
			this.cardIds = cardIds;
		}
	}

	// Token: 0x06001271 RID: 4721 RVA: 0x0000DDEC File Offset: 0x0000BFEC
	public static bool verifyCards(IEnumerable<Card> cards)
	{
		return Enumerable.Count<Card>(cards) == 3;
	}

	// Token: 0x04000F48 RID: 3912
	public long[] cardIds = new long[0];
}
