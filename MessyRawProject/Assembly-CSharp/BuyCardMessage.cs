using System;

// Token: 0x02000360 RID: 864
public class BuyCardMessage : Message
{
	// Token: 0x060013AD RID: 5037 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public BuyCardMessage()
	{
	}

	// Token: 0x060013AE RID: 5038 RVA: 0x0000E938 File Offset: 0x0000CB38
	public BuyCardMessage(int id)
	{
		this.id = id;
	}

	// Token: 0x040010E5 RID: 4325
	public Card card;

	// Token: 0x040010E6 RID: 4326
	public int id;
}
