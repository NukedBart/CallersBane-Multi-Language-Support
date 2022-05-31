using System;
using System.Collections.Generic;

// Token: 0x02000095 RID: 149
public class AlwaysWildSacrificer : SacrificeSet
{
	// Token: 0x06000569 RID: 1385 RVA: 0x000056CA File Offset: 0x000038CA
	public AlwaysWildSacrificer() : base(false)
	{
	}

	// Token: 0x0600056A RID: 1386 RVA: 0x000056D3 File Offset: 0x000038D3
	public override List<ResourceType> getSacrificable(List<ResourceType> available)
	{
		if (!available.Contains(ResourceType.SPECIAL))
		{
			available = new List<ResourceType>(available);
			available.Add(ResourceType.SPECIAL);
		}
		return base.getSacrificable(available);
	}
}
