using System;

// Token: 0x02000336 RID: 822
public class IdolTypeDeserializer
{
	// Token: 0x0600136F RID: 4975 RVA: 0x0000E6EC File Offset: 0x0000C8EC
	public short[] idols()
	{
		return new short[]
		{
			this.idol1,
			this.idol2,
			this.idol3,
			this.idol4,
			this.idol5
		};
	}

	// Token: 0x06001370 RID: 4976 RVA: 0x0000E721 File Offset: 0x0000C921
	public short getIdolType(int row)
	{
		return this.idols()[row];
	}

	// Token: 0x04001065 RID: 4197
	public short idol1;

	// Token: 0x04001066 RID: 4198
	public short idol2;

	// Token: 0x04001067 RID: 4199
	public short idol3;

	// Token: 0x04001068 RID: 4200
	public short idol4;

	// Token: 0x04001069 RID: 4201
	public short idol5;
}
