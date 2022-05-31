using System;
using UnityEngine;

// Token: 0x020001D6 RID: 470
public class GUIPositioner
{
	// Token: 0x06000EBC RID: 3772 RVA: 0x00063528 File Offset: 0x00061728
	public GUIPositioner(float numItemsWide, float between, float itemWidth, float itemHeight)
	{
		this.w = numItemsWide;
		this.x = ((float)Screen.width - this.w * itemWidth - (this.w - 1f) * between) / 2f;
		this.kx = between + itemWidth;
		this.itemw = itemWidth;
		this.itemh = itemHeight;
	}

	// Token: 0x06000EBD RID: 3773 RVA: 0x00063584 File Offset: 0x00061784
	public Rect getButtonRect(float index)
	{
		Vector2? vector = this.offsetPos;
		Vector2 vector2 = (vector == null) ? new Vector2(this.x, 0f) : vector.Value;
		return new Rect(vector2.x + index * this.kx, vector2.y, this.itemw, this.itemh);
	}

	// Token: 0x06000EBE RID: 3774 RVA: 0x0000BC9C File Offset: 0x00009E9C
	public Rect getButtonRect(float index, float y)
	{
		return new Rect(this.x + index * this.kx, y, this.itemw, this.itemh);
	}

	// Token: 0x06000EBF RID: 3775 RVA: 0x0000BCBF File Offset: 0x00009EBF
	public Rect getButtonRect(float index, float startAtX, float y)
	{
		return new Rect(startAtX + index * this.kx, y, this.itemw, this.itemh);
	}

	// Token: 0x06000EC0 RID: 3776 RVA: 0x0000BCDD File Offset: 0x00009EDD
	public Rect getButtonRectCentered(float index, float startAtX, float y)
	{
		return new Rect(startAtX + index * this.kx - this.itemw / 2f, y, this.itemw, this.itemh);
	}

	// Token: 0x06000EC1 RID: 3777 RVA: 0x0000BD08 File Offset: 0x00009F08
	public void setOffset(Vector2 offset)
	{
		this.offsetPos = new Vector2?(offset);
	}

	// Token: 0x06000EC2 RID: 3778 RVA: 0x0000BD16 File Offset: 0x00009F16
	public float getWidthFor(int buttons)
	{
		if (buttons <= 0)
		{
			return 0f;
		}
		return (float)buttons * this.kx + (float)(buttons - 1) * (this.kx - this.itemw);
	}

	// Token: 0x04000B68 RID: 2920
	private Vector2? offsetPos;

	// Token: 0x04000B69 RID: 2921
	private float w;

	// Token: 0x04000B6A RID: 2922
	private float itemw;

	// Token: 0x04000B6B RID: 2923
	private float itemh;

	// Token: 0x04000B6C RID: 2924
	private float x;

	// Token: 0x04000B6D RID: 2925
	private float kx;
}
