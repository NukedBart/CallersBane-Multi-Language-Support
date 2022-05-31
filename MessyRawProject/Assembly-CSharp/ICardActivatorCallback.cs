using System;
using UnityEngine;

// Token: 0x02000055 RID: 85
public interface ICardActivatorCallback
{
	// Token: 0x060003C0 RID: 960
	Rect getSacrificeDestRect(ResourceType resource);

	// Token: 0x060003C1 RID: 961
	void sacrificeCard(CardView card, ResourceType resource);

	// Token: 0x060003C2 RID: 962
	void confirmPlayCard(CardView card);

	// Token: 0x060003C3 RID: 963
	void magnifyCard(CardView card);

	// Token: 0x060003C4 RID: 964
	bool allowSacrifice(ResourceType resource);

	// Token: 0x060003C5 RID: 965
	bool allowPlayCard();

	// Token: 0x060003C6 RID: 966
	void resourceTweenComplete(ResourceType resource);

	// Token: 0x060003C7 RID: 967
	void glowResourceIcon(ResourceType resource, Vector3 worldPos);

	// Token: 0x060003C8 RID: 968
	string getResourceTooltip(ResourceType resource);
}
