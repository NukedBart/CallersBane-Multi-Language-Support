using System;

// Token: 0x02000332 RID: 818
public class IdolSetupSaveMessage : Message
{
	// Token: 0x06001369 RID: 4969 RVA: 0x0000E6AD File Offset: 0x0000C8AD
	public IdolSetupSaveMessage(short id)
	{
		this.idolTypes = new short[]
		{
			id,
			id,
			id,
			id,
			id
		};
	}

	// Token: 0x0400105B RID: 4187
	public short[] idolTypes;
}
