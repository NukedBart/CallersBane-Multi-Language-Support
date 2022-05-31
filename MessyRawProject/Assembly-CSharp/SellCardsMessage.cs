using System;

// Token: 0x02000366 RID: 870
[Update(new Type[]
{
	typeof(ProfileDataInfoMessage)
})]
public class SellCardsMessage : Message
{
	// Token: 0x060013C6 RID: 5062 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public SellCardsMessage()
	{
	}

	// Token: 0x060013C7 RID: 5063 RVA: 0x0000EA9F File Offset: 0x0000CC9F
	public SellCardsMessage(long[] cardIds)
	{
		this.cardIds = cardIds;
	}

	// Token: 0x040010F2 RID: 4338
	public long[] cardIds;
}
