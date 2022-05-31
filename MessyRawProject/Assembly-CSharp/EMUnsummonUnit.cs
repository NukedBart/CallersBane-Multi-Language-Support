using System;

// Token: 0x0200026D RID: 621
public class EMUnsummonUnit : EffectMessage
{
	// Token: 0x060011FC RID: 4604 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override bool isAction()
	{
		return true;
	}

	// Token: 0x04000EAF RID: 3759
	public TilePosition target;
}
