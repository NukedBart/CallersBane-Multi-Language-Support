using System;

// Token: 0x02000369 RID: 873
public class TradeAcceptMessage : Message
{
	// Token: 0x060013CA RID: 5066 RVA: 0x0000EABD File Offset: 0x0000CCBD
	public TradeAcceptMessage(int profileId)
	{
		this.profileId = profileId;
	}

	// Token: 0x040010F4 RID: 4340
	public int profileId;
}
