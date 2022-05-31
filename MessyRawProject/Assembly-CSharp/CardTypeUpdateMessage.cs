using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x0200029B RID: 667
public class CardTypeUpdateMessage : Message
{
	// Token: 0x0600124C RID: 4684 RVA: 0x0000DCBF File Offset: 0x0000BEBF
	public bool hasUpdateFor(Card ct)
	{
		return this.hasUpdateFor(ct.getCardType());
	}

	// Token: 0x0600124D RID: 4685 RVA: 0x0000DCCD File Offset: 0x0000BECD
	public bool hasUpdateFor(CardType ct)
	{
		return this._cardTypeIds.Contains(ct.id);
	}

	// Token: 0x170000F4 RID: 244
	// (get) Token: 0x0600124E RID: 4686 RVA: 0x0000DCE0 File Offset: 0x0000BEE0
	// (set) Token: 0x0600124F RID: 4687 RVA: 0x0000DCE8 File Offset: 0x0000BEE8
	public CardType[] cardTypes
	{
		get
		{
			return this._cardTypes;
		}
		set
		{
			this._cardTypes = value;
			this._cardTypeIds = new HashSet<int>(Enumerable.Select<CardType, int>(value, (CardType ct) => ct.id));
		}
	}

	// Token: 0x04000F2B RID: 3883
	private CardType[] _cardTypes;

	// Token: 0x04000F2C RID: 3884
	private HashSet<int> _cardTypeIds = new HashSet<int>();
}
