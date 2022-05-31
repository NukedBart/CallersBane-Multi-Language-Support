using System;

// Token: 0x02000365 RID: 869
public class RedeemMessage : Message
{
	// Token: 0x060013C4 RID: 5060 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public RedeemMessage()
	{
	}

	// Token: 0x060013C5 RID: 5061 RVA: 0x0000EA90 File Offset: 0x0000CC90
	public RedeemMessage(string code)
	{
		this.code = code;
	}

	// Token: 0x040010F1 RID: 4337
	public string code;
}
