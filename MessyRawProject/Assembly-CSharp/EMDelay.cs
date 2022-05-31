using System;

// Token: 0x02000272 RID: 626
public class EMDelay : InternalEffectMessage
{
	// Token: 0x0600120B RID: 4619 RVA: 0x0000DA85 File Offset: 0x0000BC85
	public EMDelay(float delayTime, bool opponentOnly)
	{
		this.delayTime = delayTime;
		this.opponentOnly = opponentOnly;
	}

	// Token: 0x04000EBB RID: 3771
	public float delayTime;

	// Token: 0x04000EBC RID: 3772
	public bool opponentOnly;
}
