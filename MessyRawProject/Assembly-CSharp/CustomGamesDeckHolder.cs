using System;
using UnityEngine;

// Token: 0x02000180 RID: 384
public class CustomGamesDeckHolder
{
	// Token: 0x06000BF7 RID: 3063 RVA: 0x00054C04 File Offset: 0x00052E04
	public static CustomGamesDeckHolder defaultFromRect(Rect rect)
	{
		float num = (float)Screen.height * 0.001f;
		CustomGamesDeckHolder customGamesDeckHolder = new CustomGamesDeckHolder();
		Rect rect2 = GeomUtil.cropShare(rect, new Rect(0f, 0f, 0.5f, 1f));
		Rect rect3 = GeomUtil.cropShare(rect, new Rect(0.5f, 0f, 0.5f, 1f));
		customGamesDeckHolder.frameRectP1 = rect2;
		CustomGamesDeckHolder customGamesDeckHolder2 = customGamesDeckHolder;
		customGamesDeckHolder2.frameRectP1.xMax = customGamesDeckHolder2.frameRectP1.xMax - 5f * num;
		customGamesDeckHolder.frameRectP2 = rect3;
		CustomGamesDeckHolder customGamesDeckHolder3 = customGamesDeckHolder;
		customGamesDeckHolder3.frameRectP2.xMin = customGamesDeckHolder3.frameRectP2.xMin + 5f * num;
		customGamesDeckHolder.rectP1 = GeomUtil.inflate(customGamesDeckHolder.frameRectP1, -20f * num, -5f * num);
		customGamesDeckHolder.rectP2 = GeomUtil.inflate(customGamesDeckHolder.frameRectP2, -20f * num, -5f * num);
		return customGamesDeckHolder;
	}

	// Token: 0x0400092D RID: 2349
	public Rect rectP1;

	// Token: 0x0400092E RID: 2350
	public Rect rectP2;

	// Token: 0x0400092F RID: 2351
	public Rect frameRectP1;

	// Token: 0x04000930 RID: 2352
	public Rect frameRectP2;
}
