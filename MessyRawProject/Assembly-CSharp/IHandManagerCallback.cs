using System;

// Token: 0x020001EC RID: 492
public interface IHandManagerCallback
{
	// Token: 0x06000F7F RID: 3967
	float IncreaseMultiplier(int rarity);

	// Token: 0x06000F80 RID: 3968
	int GetCostForCard(Card card);

	// Token: 0x06000F81 RID: 3969
	void SpecificCardDeselected(CardView card);
}
