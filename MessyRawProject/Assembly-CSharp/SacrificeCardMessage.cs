using System;

// Token: 0x02000234 RID: 564
public class SacrificeCardMessage : BattleAction
{
	// Token: 0x06001180 RID: 4480 RVA: 0x0000D5F1 File Offset: 0x0000B7F1
	public SacrificeCardMessage(Card card, ResourceType resource)
	{
		this.card = card.getId();
		this.resource = resource;
	}

	// Token: 0x04000E01 RID: 3585
	public long card;

	// Token: 0x04000E02 RID: 3586
	public ResourceType resource;
}
