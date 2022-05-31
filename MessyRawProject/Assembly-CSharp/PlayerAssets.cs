using System;

// Token: 0x02000253 RID: 595
public class PlayerAssets
{
	// Token: 0x060011B9 RID: 4537 RVA: 0x0000D7BD File Offset: 0x0000B9BD
	public PlayerAssets(ResourceGroup available, ResourceGroup output, int handSize)
	{
		this.availableResources = available;
		this.outputResources = output;
		this.handSize = handSize;
	}

	// Token: 0x04000E61 RID: 3681
	public ResourceGroup availableResources;

	// Token: 0x04000E62 RID: 3682
	public ResourceGroup outputResources;

	// Token: 0x04000E63 RID: 3683
	public EMCostUpdate costUpdate;

	// Token: 0x04000E64 RID: 3684
	public EMRuleAdded[] ruleUpdates;

	// Token: 0x04000E65 RID: 3685
	public int handSize;

	// Token: 0x04000E66 RID: 3686
	public int librarySize;

	// Token: 0x04000E67 RID: 3687
	public int graveyardSize;
}
