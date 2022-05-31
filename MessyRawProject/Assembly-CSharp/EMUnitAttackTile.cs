using System;

// Token: 0x0200026A RID: 618
public class EMUnitAttackTile : EffectMessage
{
	// Token: 0x060011F8 RID: 4600 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override bool isAction()
	{
		return true;
	}

	// Token: 0x04000EA8 RID: 3752
	public TilePosition source;

	// Token: 0x04000EA9 RID: 3753
	public TilePosition target;
}
