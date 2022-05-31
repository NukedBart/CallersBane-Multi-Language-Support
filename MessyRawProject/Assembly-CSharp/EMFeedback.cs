using System;

// Token: 0x02000274 RID: 628
public class EMFeedback : InternalEffectMessage
{
	// Token: 0x0600120D RID: 4621 RVA: 0x0000DAAA File Offset: 0x0000BCAA
	public EMFeedback(EffectMessage e)
	{
		this.msg = e;
	}

	// Token: 0x04000EBE RID: 3774
	public EffectMessage msg;
}
