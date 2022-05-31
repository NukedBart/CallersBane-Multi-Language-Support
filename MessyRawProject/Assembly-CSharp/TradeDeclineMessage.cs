using System;

// Token: 0x0200036D RID: 877
public class TradeDeclineMessage : Message
{
	// Token: 0x060013CE RID: 5070 RVA: 0x0000EADB File Offset: 0x0000CCDB
	public TradeDeclineMessage(int profileId)
	{
		this.profileId = profileId;
	}

	// Token: 0x040010F7 RID: 4343
	public int profileId;
}
