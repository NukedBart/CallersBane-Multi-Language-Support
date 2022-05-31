using System;

// Token: 0x02000384 RID: 900
public interface IGoldShardsCallback : ICancelCallback
{
	// Token: 0x060013FE RID: 5118
	void PopupGoldSelected(int itemId);

	// Token: 0x060013FF RID: 5119
	void PopupShardsSelected(int itemId);

	// Token: 0x06001400 RID: 5120
	void ShowShardsPurchase();
}
