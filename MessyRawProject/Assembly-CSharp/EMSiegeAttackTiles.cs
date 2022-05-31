using System;

// Token: 0x0200025D RID: 605
public class EMSiegeAttackTiles : EffectMessage
{
	// Token: 0x060011DE RID: 4574 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override bool isAction()
	{
		return true;
	}

	// Token: 0x04000E87 RID: 3719
	public TilePosition source;

	// Token: 0x04000E88 RID: 3720
	public TilePosition[] targets;
}
