using System;

// Token: 0x02000260 RID: 608
public class EMStatsUpdate : EffectMessage
{
	// Token: 0x04000E8C RID: 3724
	public TilePosition target;

	// Token: 0x04000E8D RID: 3725
	public int hp;

	// Token: 0x04000E8E RID: 3726
	public int ap;

	// Token: 0x04000E8F RID: 3727
	public int ac;

	// Token: 0x04000E90 RID: 3728
	public EnchantmentInfo[] buffs;
}
