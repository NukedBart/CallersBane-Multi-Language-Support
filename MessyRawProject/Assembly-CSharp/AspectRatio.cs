using System;
using UnityEngine;

// Token: 0x0200041D RID: 1053
public class AspectRatio
{
	// Token: 0x0600175F RID: 5983 RVA: 0x00010D15 File Offset: 0x0000EF15
	public AspectRatio() : this((float)Screen.width, (float)Screen.height)
	{
	}

	// Token: 0x06001760 RID: 5984 RVA: 0x00010D29 File Offset: 0x0000EF29
	public AspectRatio(float width, float height)
	{
		this.ratio = width / height;
	}

	// Token: 0x06001762 RID: 5986 RVA: 0x00010D3A File Offset: 0x0000EF3A
	public bool isEqual(AspectRatio r)
	{
		return Math.Abs(this.ratio - r.ratio) < 0.001f;
	}

	// Token: 0x06001763 RID: 5987 RVA: 0x00010D55 File Offset: 0x0000EF55
	public bool isWider(AspectRatio r)
	{
		return !this.isEqual(r) && this.ratio > r.ratio;
	}

	// Token: 0x06001764 RID: 5988 RVA: 0x00010D74 File Offset: 0x0000EF74
	public bool isNarrower(AspectRatio r)
	{
		return !this.isEqual(r) && this.ratio < r.ratio;
	}

	// Token: 0x06001765 RID: 5989 RVA: 0x00010D93 File Offset: 0x0000EF93
	public float getWidthFor(float height)
	{
		return height * this.ratio;
	}

	// Token: 0x06001766 RID: 5990 RVA: 0x00010D9D File Offset: 0x0000EF9D
	public float getHeightFor(float width)
	{
		return width / this.ratio;
	}

	// Token: 0x17000129 RID: 297
	// (get) Token: 0x06001767 RID: 5991 RVA: 0x00010DA7 File Offset: 0x0000EFA7
	public static AspectRatio now
	{
		get
		{
			return new AspectRatio();
		}
	}

	// Token: 0x040014C4 RID: 5316
	public readonly float ratio;

	// Token: 0x040014C5 RID: 5317
	public static AspectRatio _16_9 = new AspectRatio(16f, 9f);

	// Token: 0x040014C6 RID: 5318
	public static AspectRatio _16_10 = new AspectRatio(16f, 10f);

	// Token: 0x040014C7 RID: 5319
	public static AspectRatio _3_2 = new AspectRatio(3f, 2f);

	// Token: 0x040014C8 RID: 5320
	public static AspectRatio _4_3 = new AspectRatio(4f, 3f);

	// Token: 0x040014C9 RID: 5321
	public static AspectRatio _5_4 = new AspectRatio(5f, 4f);
}
