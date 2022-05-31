using System;

// Token: 0x02000270 RID: 624
public class EffectDone
{
	// Token: 0x06001207 RID: 4615 RVA: 0x0000DA37 File Offset: 0x0000BC37
	public EffectDone(BattleMode bm, int id)
	{
		this.bm = bm;
		this.id = id;
	}

	// Token: 0x06001208 RID: 4616 RVA: 0x0000DA4D File Offset: 0x0000BC4D
	public void callDone()
	{
		this.bm.effectDone();
	}

	// Token: 0x06001209 RID: 4617 RVA: 0x0000DA5A File Offset: 0x0000BC5A
	public void callDoneIn(float seconds)
	{
		this.bm.effectDoneSoon(seconds);
	}

	// Token: 0x04000EB5 RID: 3765
	private BattleMode bm;

	// Token: 0x04000EB6 RID: 3766
	private int id;

	// Token: 0x04000EB7 RID: 3767
	private int triggerCount;
}
