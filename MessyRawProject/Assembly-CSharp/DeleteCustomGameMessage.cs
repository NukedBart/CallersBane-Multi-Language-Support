using System;

// Token: 0x020002C8 RID: 712
public class DeleteCustomGameMessage : Message
{
	// Token: 0x060012AF RID: 4783 RVA: 0x0000E082 File Offset: 0x0000C282
	public DeleteCustomGameMessage(int customGameId)
	{
		this.customGameId = customGameId;
	}

	// Token: 0x04000F6F RID: 3951
	public int customGameId;
}
