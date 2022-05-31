using System;

// Token: 0x02000240 RID: 576
public class EMDamageIdol : EffectMessage
{
	// Token: 0x06001196 RID: 4502 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override bool isAction()
	{
		return true;
	}

	// Token: 0x04000E16 RID: 3606
	public IdolInfo idol;

	// Token: 0x04000E17 RID: 3607
	public int amount;

	// Token: 0x04000E18 RID: 3608
	public AttackType attackType;
}
