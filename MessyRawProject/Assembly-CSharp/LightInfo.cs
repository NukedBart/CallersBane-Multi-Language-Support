using System;
using UnityEngine;

// Token: 0x0200003F RID: 63
public class LightInfo
{
	// Token: 0x06000290 RID: 656 RVA: 0x00003E36 File Offset: 0x00002036
	public LightInfo(Vector3 pos, Color color, int range)
	{
		this.pos = pos;
		this.color = color;
		this.range = range;
	}

	// Token: 0x0400016F RID: 367
	public Vector3 pos;

	// Token: 0x04000170 RID: 368
	public Color color;

	// Token: 0x04000171 RID: 369
	public int range = 10;
}
