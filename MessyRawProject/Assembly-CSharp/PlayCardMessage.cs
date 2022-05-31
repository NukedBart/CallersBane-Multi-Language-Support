using System;
using System.Collections.Generic;

// Token: 0x02000233 RID: 563
public class PlayCardMessage : BattleAction
{
	// Token: 0x0600117E RID: 4478 RVA: 0x0000D5BC File Offset: 0x0000B7BC
	public PlayCardMessage(Card card) : this(card, null)
	{
	}

	// Token: 0x0600117F RID: 4479 RVA: 0x0000D5C6 File Offset: 0x0000B7C6
	public PlayCardMessage(Card card, List<TilePosition> selected)
	{
		this.card = card.getId();
		if (selected != null)
		{
			this.data = new DataMapHolder(TilePosition.positionStrings(selected));
		}
	}

	// Token: 0x04000DFF RID: 3583
	public long card;

	// Token: 0x04000E00 RID: 3584
	public DataMapHolder data;
}
