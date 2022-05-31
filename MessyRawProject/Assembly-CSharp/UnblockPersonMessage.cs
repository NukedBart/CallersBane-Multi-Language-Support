using System;

// Token: 0x020002DF RID: 735
public class UnblockPersonMessage : Message
{
	// Token: 0x060012DA RID: 4826 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public UnblockPersonMessage()
	{
	}

	// Token: 0x060012DB RID: 4827 RVA: 0x0000E215 File Offset: 0x0000C415
	public UnblockPersonMessage(string profileName)
	{
		this.profileName = profileName;
	}

	// Token: 0x04000FAB RID: 4011
	public string profileName;
}
