using System;

// Token: 0x0200024D RID: 589
public class EMMoveUnit : EffectMessage
{
	// Token: 0x060011B2 RID: 4530 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override bool isAction()
	{
		return true;
	}

	// Token: 0x04000E55 RID: 3669
	public TilePosition from;

	// Token: 0x04000E56 RID: 3670
	public TilePosition to;
}
