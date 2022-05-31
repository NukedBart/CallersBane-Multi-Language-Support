using System;

// Token: 0x0200024B RID: 587
public class EMHealUnit : EffectMessage
{
	// Token: 0x060011AD RID: 4525 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override bool isAction()
	{
		return true;
	}

	// Token: 0x060011AE RID: 4526 RVA: 0x0000D779 File Offset: 0x0000B979
	public bool showAnimation()
	{
		return this.healed;
	}

	// Token: 0x060011AF RID: 4527 RVA: 0x0000D781 File Offset: 0x0000B981
	public bool showPopup()
	{
		return this.hp > 0;
	}

	// Token: 0x04000E4F RID: 3663
	public TilePosition source;

	// Token: 0x04000E50 RID: 3664
	public TilePosition target;

	// Token: 0x04000E51 RID: 3665
	public int hp;

	// Token: 0x04000E52 RID: 3666
	public int amount;

	// Token: 0x04000E53 RID: 3667
	public bool healed;
}
