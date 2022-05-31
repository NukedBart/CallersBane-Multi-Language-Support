using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x02000096 RID: 150
public class SacrificeSet : Sacrificer
{
	// Token: 0x0600056B RID: 1387 RVA: 0x000056F7 File Offset: 0x000038F7
	public SacrificeSet(bool multiColorSacrifice)
	{
		this.multiColorSacrifice = multiColorSacrifice;
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x00005711 File Offset: 0x00003911
	public void sacrifice(ResourceType type)
	{
		this.forResources.Add(type);
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x00005720 File Offset: 0x00003920
	public bool canSacrifice(ResourceType type)
	{
		return this.canSacrificeAny(new ResourceType[]
		{
			type
		});
	}

	// Token: 0x0600056E RID: 1390 RVA: 0x00039598 File Offset: 0x00037798
	public bool canSacrificeAny(IEnumerable<ResourceType> available)
	{
		if (!this.multiColorSacrifice)
		{
			return this.forResources.Count == 0;
		}
		if (this.forResources.Contains(ResourceType.CARDS))
		{
			return false;
		}
		foreach (ResourceType resourceType in available)
		{
			if (!this.forResources.Contains(resourceType))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600056F RID: 1391 RVA: 0x00005732 File Offset: 0x00003932
	public virtual List<ResourceType> getSacrificable(List<ResourceType> available)
	{
		return Enumerable.ToList<ResourceType>(Enumerable.Where<ResourceType>(available, (ResourceType r) => this.canSacrifice(r)));
	}

	// Token: 0x040003DF RID: 991
	private HashSet<ResourceType> forResources = new HashSet<ResourceType>();

	// Token: 0x040003E0 RID: 992
	private bool multiColorSacrifice;
}
