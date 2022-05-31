using System;

// Token: 0x02000372 RID: 882
public class TradeResponseMessage : Message
{
	// Token: 0x040010FF RID: 4351
	public const string ACCEPT = "ACCEPT";

	// Token: 0x04001100 RID: 4352
	public const string DECLINE = "DECLINE";

	// Token: 0x04001101 RID: 4353
	public const string CANCEL = "CANCEL";

	// Token: 0x04001102 RID: 4354
	public const string CANCEL_BARGAIN = "CANCEL_BARGAIN";

	// Token: 0x04001103 RID: 4355
	public const string TIMEOUT = "TIMEOUT";

	// Token: 0x04001104 RID: 4356
	public ProfileInfo from;

	// Token: 0x04001105 RID: 4357
	public ProfileInfo to;

	// Token: 0x04001106 RID: 4358
	public string status;
}
