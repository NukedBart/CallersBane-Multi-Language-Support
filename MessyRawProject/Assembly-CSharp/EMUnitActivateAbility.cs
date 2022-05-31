using System;

// Token: 0x02000267 RID: 615
public class EMUnitActivateAbility : EffectMessage
{
	// Token: 0x060011EF RID: 4591 RVA: 0x0000D99C File Offset: 0x0000BB9C
	public EMUnitActivateAbility(Unit unit, string name)
	{
		this.unit = unit.getTilePosition();
		this.name = name;
	}

	// Token: 0x060011F0 RID: 4592 RVA: 0x0000D636 File Offset: 0x0000B836
	public EMUnitActivateAbility()
	{
	}

	// Token: 0x060011F1 RID: 4593 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override bool isAction()
	{
		return true;
	}

	// Token: 0x060011F2 RID: 4594 RVA: 0x0000D9B7 File Offset: 0x0000BBB7
	public bool isMoveLike()
	{
		return ActiveAbility.isMoveLike(this.name);
	}

	// Token: 0x04000EA3 RID: 3747
	public string name;

	// Token: 0x04000EA4 RID: 3748
	public TilePosition unit;
}
