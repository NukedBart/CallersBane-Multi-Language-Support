using System;

// Token: 0x02000241 RID: 577
public class EMDamageUnit : EffectMessage
{
	// Token: 0x06001198 RID: 4504 RVA: 0x000774FC File Offset: 0x000756FC
	public EffectMessage createFakeHit()
	{
		return new EMDamageUnit
		{
			targetTile = this.targetTile,
			attackType = this.attackType,
			damageType = this.damageType,
			hp = this.hp,
			kill = false,
			amount = -1,
			fake = true
		};
	}

	// Token: 0x06001199 RID: 4505 RVA: 0x0000D68A File Offset: 0x0000B88A
	public bool isFake()
	{
		return this.fake;
	}

	// Token: 0x0600119A RID: 4506 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override bool isAction()
	{
		return true;
	}

	// Token: 0x0600119B RID: 4507 RVA: 0x0000D692 File Offset: 0x0000B892
	public override string ToString()
	{
		return string.Format("[EMDamageUnit] fake? {0}", this.fake);
	}

	// Token: 0x04000E19 RID: 3609
	public TilePosition targetTile;

	// Token: 0x04000E1A RID: 3610
	public int amount;

	// Token: 0x04000E1B RID: 3611
	public int hp;

	// Token: 0x04000E1C RID: 3612
	public bool kill;

	// Token: 0x04000E1D RID: 3613
	public Card sourceCard;

	// Token: 0x04000E1E RID: 3614
	public DamageType damageType;

	// Token: 0x04000E1F RID: 3615
	public AttackType attackType;

	// Token: 0x04000E20 RID: 3616
	private bool fake;
}
