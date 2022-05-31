using System;

// Token: 0x020002EF RID: 751
public class Reward
{
	// Token: 0x060012F0 RID: 4848 RVA: 0x00002DDA File Offset: 0x00000FDA
	public Reward()
	{
	}

	// Token: 0x060012F1 RID: 4849 RVA: 0x0000E2DB File Offset: 0x0000C4DB
	public Reward(int gold, int common, int uncommon, int rare)
	{
		this.gold = gold;
		this.common = common;
		this.uncommon = uncommon;
		this.rare = rare;
	}

	// Token: 0x060012F2 RID: 4850 RVA: 0x0000E300 File Offset: 0x0000C500
	public int GetTotalScrolls()
	{
		return this.common + this.uncommon + this.rare;
	}

	// Token: 0x04000FCB RID: 4043
	public int gold;

	// Token: 0x04000FCC RID: 4044
	public int common;

	// Token: 0x04000FCD RID: 4045
	public int uncommon;

	// Token: 0x04000FCE RID: 4046
	public int rare;
}
