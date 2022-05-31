using System;

// Token: 0x020002D0 RID: 720
public class BlockPersonMessage : Message
{
	// Token: 0x060012C1 RID: 4801 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public BlockPersonMessage()
	{
	}

	// Token: 0x060012C2 RID: 4802 RVA: 0x0000E18D File Offset: 0x0000C38D
	public BlockPersonMessage(string profileName)
	{
		this.profileName = profileName;
	}

	// Token: 0x04000F90 RID: 3984
	public string profileName;
}
