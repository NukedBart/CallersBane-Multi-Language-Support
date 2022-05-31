using System;
using UnityEngine;

// Token: 0x020001D7 RID: 471
public class GUIPositioner2
{
	// Token: 0x06000EC3 RID: 3779 RVA: 0x0000BD41 File Offset: 0x00009F41
	public GUIPositioner2(MockupCalc c, Vector2[] sizes, float between)
	{
		this.calc = c;
		this.kx = between;
		this.sizes = sizes;
	}

	// Token: 0x06000EC4 RID: 3780 RVA: 0x000635EC File Offset: 0x000617EC
	public Rect getButtonRect(float index, float y)
	{
		int num = Mathf.Min((int)index, this.sizes.Length - 1);
		Rect rect = this.calc.rAspectH(0f, 0f, this.sizes[num].x, this.sizes[num].y);
		return new Rect((float)Screen.width * 0.025f + this.getXFor(index), y, rect.width, rect.height);
	}

	// Token: 0x06000EC5 RID: 3781 RVA: 0x0006366C File Offset: 0x0006186C
	private float getXFor(float findex)
	{
		int num = (int)findex;
		float num2 = this.calc.X(this.kx) * findex;
		for (int i = 0; i < num; i++)
		{
			if (i < this.sizes.Length)
			{
				num2 += this.calc.rAspectH(0f, 0f, this.sizes[i].x, this.sizes[i].y).width * 0.92f;
			}
		}
		return num2;
	}

	// Token: 0x04000B6E RID: 2926
	private MockupCalc calc;

	// Token: 0x04000B6F RID: 2927
	private Vector2[] sizes;

	// Token: 0x04000B70 RID: 2928
	private float kx;
}
