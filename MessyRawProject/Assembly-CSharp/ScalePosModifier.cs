using System;
using UnityEngine;

// Token: 0x020000CD RID: 205
internal class ScalePosModifier
{
	// Token: 0x06000704 RID: 1796 RVA: 0x000066A7 File Offset: 0x000048A7
	public ScalePosModifier() : this(1f)
	{
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x000066B4 File Offset: 0x000048B4
	public ScalePosModifier(float scale) : this(scale, 0f, 0f)
	{
	}

	// Token: 0x06000706 RID: 1798 RVA: 0x000066C7 File Offset: 0x000048C7
	public ScalePosModifier(float scale, float x, float y)
	{
		this.scale = scale;
		this.offset = new Vector3(x, y, 0f);
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x0003F434 File Offset: 0x0003D634
	public override string ToString()
	{
		return string.Concat(new object[]
		{
			"ScalePosModifier(Scale=",
			this.scale,
			", Offset=",
			this.offset,
			")"
		});
	}

	// Token: 0x06000708 RID: 1800 RVA: 0x0003F480 File Offset: 0x0003D680
	public static ScalePosModifier create(CardType ct)
	{
		if (ct.useDummyAnimationBundle())
		{
			return new ScalePosModifier();
		}
		return new ScalePosModifier(ct.getTag<float>("unit_scale", 1f), ct.getTag<float>("unit_offsetx", 0f), ct.getTag<float>("unit_offsety", 0f));
	}

	// Token: 0x0400050C RID: 1292
	public float scale;

	// Token: 0x0400050D RID: 1293
	public Vector3 offset;
}
