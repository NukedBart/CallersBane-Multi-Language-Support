using System;
using UnityEngine;

// Token: 0x02000271 RID: 625
public class EMBattleText : InternalEffectMessage
{
	// Token: 0x0600120A RID: 4618 RVA: 0x0000DA68 File Offset: 0x0000BC68
	public EMBattleText(string text, Vector3 pos, float startInSeconds)
	{
		this.text = text;
		this.pos = pos;
		this.startInSeconds = startInSeconds;
	}

	// Token: 0x04000EB8 RID: 3768
	public string text;

	// Token: 0x04000EB9 RID: 3769
	public Vector3 pos;

	// Token: 0x04000EBA RID: 3770
	public float startInSeconds;
}
