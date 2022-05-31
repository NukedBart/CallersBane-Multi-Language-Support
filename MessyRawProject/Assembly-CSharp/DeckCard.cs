using System;
using UnityEngine;

// Token: 0x020001A0 RID: 416
public class DeckCard
{
	// Token: 0x06000D12 RID: 3346 RVA: 0x0000A72F File Offset: 0x0000892F
	public DeckCard(ICardView card)
	{
		this.card = card;
		this.type = card.getCardInfo().getType();
		this.t = card.getTransform();
	}

	// Token: 0x04000A32 RID: 2610
	public readonly ICardView card;

	// Token: 0x04000A33 RID: 2611
	public readonly int type;

	// Token: 0x04000A34 RID: 2612
	public readonly Transform t;

	// Token: 0x04000A35 RID: 2613
	public bool isFiltered;
}
