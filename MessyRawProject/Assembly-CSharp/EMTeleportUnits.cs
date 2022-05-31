using System;

// Token: 0x02000264 RID: 612
public class EMTeleportUnits : EffectMessage
{
	// Token: 0x060011E8 RID: 4584 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override bool isAction()
	{
		return true;
	}

	// Token: 0x04000E98 RID: 3736
	public TeleportInfo[] units;
}
