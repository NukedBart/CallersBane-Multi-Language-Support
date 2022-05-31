using System;
using System.Collections.Generic;

// Token: 0x0200023E RID: 574
public class EMCostUpdate : EffectMessage
{
	// Token: 0x06001192 RID: 4498 RVA: 0x0000D665 File Offset: 0x0000B865
	public int getCost(Card card, int defaultCost)
	{
		if (this._costDict == null)
		{
			this._buildCache();
		}
		return CollectionUtil.getOrDefault<int, int>(this._costDict, card.getType(), defaultCost);
	}

	// Token: 0x06001193 RID: 4499 RVA: 0x000774B0 File Offset: 0x000756B0
	public void _buildCache()
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		foreach (EMCostUpdate.CostInfo costInfo in this.costs)
		{
			dictionary[costInfo.cardTypeId] = costInfo.cost;
		}
		this._costDict = dictionary;
	}

	// Token: 0x04000E11 RID: 3601
	public int profileId;

	// Token: 0x04000E12 RID: 3602
	public EMCostUpdate.CostInfo[] costs;

	// Token: 0x04000E13 RID: 3603
	private Dictionary<int, int> _costDict;

	// Token: 0x0200023F RID: 575
	public class CostInfo
	{
		// Token: 0x04000E14 RID: 3604
		public int cardTypeId;

		// Token: 0x04000E15 RID: 3605
		public int cost;
	}
}
