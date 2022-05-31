using System;

// Token: 0x020002F0 RID: 752
public class SelectCardLimitedMessage : Message
{
	// Token: 0x060012F3 RID: 4851 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public SelectCardLimitedMessage()
	{
	}

	// Token: 0x060012F4 RID: 4852 RVA: 0x0000E316 File Offset: 0x0000C516
	public SelectCardLimitedMessage(int cardTypeId)
	{
		this.cardTypeId = cardTypeId;
	}

	// Token: 0x04000FCF RID: 4047
	public int cardTypeId;
}
