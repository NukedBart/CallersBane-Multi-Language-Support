using System;

// Token: 0x02000268 RID: 616
public class EMUnitAttackDone : EffectMessage
{
	// Token: 0x060011F4 RID: 4596 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override bool isAction()
	{
		return true;
	}

	// Token: 0x04000EA5 RID: 3749
	public TilePosition source;
}
