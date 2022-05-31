using System;

// Token: 0x0200026B RID: 619
public class EMUnitPlayAnimation : EffectMessage, MovedOutLogic_Effect
{
	// Token: 0x060011FA RID: 4602 RVA: 0x0007782C File Offset: 0x00075A2C
	public void eval(IGame game, EffectDone ed)
	{
		Unit unit = game.getUnit(this.tile);
		if (this.animation == EMUnitPlayAnimation.Animation.ACTION)
		{
			unit.playAnimation("ActivateAbility");
		}
		if (this.animation == EMUnitPlayAnimation.Animation.HIT)
		{
			unit.playAnimation("Damage");
			ed.callDone();
		}
	}

	// Token: 0x04000EAA RID: 3754
	public TilePosition tile;

	// Token: 0x04000EAB RID: 3755
	public EMUnitPlayAnimation.Animation animation;

	// Token: 0x0200026C RID: 620
	public enum Animation
	{
		// Token: 0x04000EAD RID: 3757
		ACTION,
		// Token: 0x04000EAE RID: 3758
		HIT
	}
}
