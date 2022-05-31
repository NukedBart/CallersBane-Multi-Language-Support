using System;
using UnityEngine;

// Token: 0x02000182 RID: 386
public class SizedString
{
	// Token: 0x06000C00 RID: 3072 RVA: 0x00009D08 File Offset: 0x00007F08
	private SizedString(string text, float width, float usedWidth, float height, GUIStyle style)
	{
		this.text = text;
		this.width = width;
		this.height = height;
		this.style = style;
		this.usedWidth = usedWidth;
	}

	// Token: 0x06000C01 RID: 3073 RVA: 0x00054E04 File Offset: 0x00053004
	public SizedString(string text, float maxWidth, GUIStyle style) : this(text, maxWidth, style.CalcSize(new GUIContent(text)).x, style.CalcHeight(new GUIContent(text), maxWidth), style)
	{
	}

	// Token: 0x0400093B RID: 2363
	public readonly string text;

	// Token: 0x0400093C RID: 2364
	public readonly float width;

	// Token: 0x0400093D RID: 2365
	public readonly float height;

	// Token: 0x0400093E RID: 2366
	public readonly float usedWidth;

	// Token: 0x0400093F RID: 2367
	public readonly GUIStyle style;
}
