using System;

// Token: 0x02000276 RID: 630
public class EMKillUnit : InternalEffectMessage
{
	// Token: 0x0600120F RID: 4623 RVA: 0x0000DAC8 File Offset: 0x0000BCC8
	public EMKillUnit(TilePosition target)
	{
		this.target = target;
	}

	// Token: 0x04000EC0 RID: 3776
	public TilePosition target;
}
