using System;

// Token: 0x02000278 RID: 632
public class EMSurrenderIdolEffect : InternalEffectMessage
{
	// Token: 0x06001211 RID: 4625 RVA: 0x0000DADF File Offset: 0x0000BCDF
	public EMSurrenderIdolEffect(TileColor color, int idolId)
	{
		this.color = color;
		this.idolId = idolId;
	}

	// Token: 0x04000EC1 RID: 3777
	public TileColor color;

	// Token: 0x04000EC2 RID: 3778
	public int idolId;
}
