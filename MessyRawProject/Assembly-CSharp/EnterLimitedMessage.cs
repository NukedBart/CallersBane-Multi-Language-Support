using System;

// Token: 0x020002E7 RID: 743
[Update(new Type[]
{
	typeof(ProfileDataInfoMessage)
})]
public class EnterLimitedMessage : Message
{
	// Token: 0x060012E6 RID: 4838 RVA: 0x0000E2BD File Offset: 0x0000C4BD
	public EnterLimitedMessage(bool payWithShards)
	{
		this.payWithShards = payWithShards;
	}

	// Token: 0x060012E7 RID: 4839 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public EnterLimitedMessage()
	{
	}

	// Token: 0x04000FB9 RID: 4025
	public bool payWithShards;
}
