using System;

// Token: 0x02000361 RID: 865
[Update(new Type[]
{
	typeof(ProfileDataInfoMessage)
})]
public class BuyStoreItemMessage : Message
{
	// Token: 0x060013AF RID: 5039 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public BuyStoreItemMessage()
	{
	}

	// Token: 0x060013B0 RID: 5040 RVA: 0x0000E947 File Offset: 0x0000CB47
	public BuyStoreItemMessage(int itemId, bool payWithShards)
	{
		this.itemId = itemId;
		this.payWithShards = payWithShards;
	}

	// Token: 0x040010E7 RID: 4327
	public int itemId;

	// Token: 0x040010E8 RID: 4328
	public bool payWithShards;
}
