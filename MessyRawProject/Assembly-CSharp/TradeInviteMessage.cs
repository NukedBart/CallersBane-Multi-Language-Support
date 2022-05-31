using System;

// Token: 0x02000370 RID: 880
public class TradeInviteMessage : Message
{
	// Token: 0x060013D1 RID: 5073 RVA: 0x0000EB09 File Offset: 0x0000CD09
	public TradeInviteMessage(int profileId)
	{
		this.profileId = profileId;
	}

	// Token: 0x040010FD RID: 4349
	public int profileId;
}
