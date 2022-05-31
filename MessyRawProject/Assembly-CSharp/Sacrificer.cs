using System;
using System.Collections.Generic;

// Token: 0x02000097 RID: 151
public interface Sacrificer
{
	// Token: 0x06000571 RID: 1393
	void sacrifice(ResourceType type);

	// Token: 0x06000572 RID: 1394
	bool canSacrifice(ResourceType type);

	// Token: 0x06000573 RID: 1395
	List<ResourceType> getSacrificable(List<ResourceType> available);
}
