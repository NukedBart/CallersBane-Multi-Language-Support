using System;

// Token: 0x0200036E RID: 878
public class TradeInfo
{
	// Token: 0x060013CF RID: 5071 RVA: 0x0000EAEA File Offset: 0x0000CCEA
	public TradeInfo()
	{
		this.cardIds = new long[0];
		this.profile = new ProfileInfo();
	}

	// Token: 0x040010F8 RID: 4344
	public ProfileInfo profile;

	// Token: 0x040010F9 RID: 4345
	public long[] cardIds;

	// Token: 0x040010FA RID: 4346
	public int gold;

	// Token: 0x040010FB RID: 4347
	public bool accepted;
}
