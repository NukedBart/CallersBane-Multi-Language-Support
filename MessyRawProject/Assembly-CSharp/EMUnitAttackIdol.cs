using System;

// Token: 0x02000269 RID: 617
public class EMUnitAttackIdol : EffectMessage
{
	// Token: 0x060011F6 RID: 4598 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override bool isAction()
	{
		return true;
	}

	// Token: 0x04000EA6 RID: 3750
	public TilePosition attacker;

	// Token: 0x04000EA7 RID: 3751
	public int idol;
}
