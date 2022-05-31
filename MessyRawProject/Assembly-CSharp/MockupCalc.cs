using System;
using UnityEngine;

// Token: 0x0200044F RID: 1103
public class MockupCalc
{
	// Token: 0x06001877 RID: 6263 RVA: 0x00011AF3 File Offset: 0x0000FCF3
	public MockupCalc(int width, int height)
	{
		this.w = (float)width;
		this.h = (float)height;
	}

	// Token: 0x06001878 RID: 6264 RVA: 0x00011B0B File Offset: 0x0000FD0B
	public Vector2 p(Vector2 pp)
	{
		return this.p(pp.x, pp.y);
	}

	// Token: 0x06001879 RID: 6265 RVA: 0x00011B21 File Offset: 0x0000FD21
	public Vector2 p(float x, float y)
	{
		return new Vector2(this.X(x), this.Y(y));
	}

	// Token: 0x0600187A RID: 6266 RVA: 0x00011B36 File Offset: 0x0000FD36
	public Rect r(Rect rr)
	{
		return this.r(rr.x, rr.y, rr.width, rr.height);
	}

	// Token: 0x0600187B RID: 6267 RVA: 0x00011B5A File Offset: 0x0000FD5A
	public Rect r(float x, float y, float width, float height)
	{
		return new Rect(this.X(x), this.Y(y), this.X(width), this.Y(height));
	}

	// Token: 0x0600187C RID: 6268 RVA: 0x00011B7E File Offset: 0x0000FD7E
	public Rect pr(Vector2 pScreen, float width, float height)
	{
		return new Rect(pScreen.x, pScreen.y, this.X(width), this.Y(height));
	}

	// Token: 0x0600187D RID: 6269 RVA: 0x00011BA1 File Offset: 0x0000FDA1
	public Rect rAspectW(Rect rr)
	{
		return this.rAspectW(rr.x, rr.y, rr.width, rr.height);
	}

	// Token: 0x0600187E RID: 6270 RVA: 0x00011BC5 File Offset: 0x0000FDC5
	public Rect rAspectW(float x, float y, float width, float height)
	{
		return new Rect(this.X(x), this.Y(y), this.X(width), this.X(height));
	}

	// Token: 0x0600187F RID: 6271 RVA: 0x00011BE9 File Offset: 0x0000FDE9
	public Rect rAspectH(Rect rr)
	{
		return this.rAspectH(rr.x, rr.y, rr.width, rr.height);
	}

	// Token: 0x06001880 RID: 6272 RVA: 0x00011C0D File Offset: 0x0000FE0D
	public Rect rAspectH(float x, float y, float width, float height)
	{
		return new Rect(this.X(x), this.Y(y), this.Y(width), this.Y(height));
	}

	// Token: 0x06001881 RID: 6273 RVA: 0x00011C31 File Offset: 0x0000FE31
	public Rect prAspectW(Vector2 pScreen, float width, float height)
	{
		return new Rect(pScreen.x, pScreen.y, this.X(width), this.X(height));
	}

	// Token: 0x06001882 RID: 6274 RVA: 0x00011C54 File Offset: 0x0000FE54
	public Rect prAspectH(Vector2 pScreen, float width, float height)
	{
		return new Rect(pScreen.x, pScreen.y, this.Y(width), this.Y(height));
	}

	// Token: 0x06001883 RID: 6275 RVA: 0x00011C77 File Offset: 0x0000FE77
	public Rect inverseR(Rect r)
	{
		return new Rect(this.IX(r.x), this.IY(r.y), this.IX(r.width), this.IY(r.height));
	}

	// Token: 0x06001884 RID: 6276 RVA: 0x00011CB2 File Offset: 0x0000FEB2
	public float refWidth()
	{
		return this.w;
	}

	// Token: 0x06001885 RID: 6277 RVA: 0x00011CBA File Offset: 0x0000FEBA
	public float refHeight()
	{
		return this.h;
	}

	// Token: 0x06001886 RID: 6278 RVA: 0x00011CC2 File Offset: 0x0000FEC2
	public float X(float x)
	{
		return Mathf.Round(this._x(x));
	}

	// Token: 0x06001887 RID: 6279 RVA: 0x00011CD0 File Offset: 0x0000FED0
	public float Y(float y)
	{
		return Mathf.Round(this._y(y));
	}

	// Token: 0x06001888 RID: 6280 RVA: 0x00011CDE File Offset: 0x0000FEDE
	public float IX(float x)
	{
		return Mathf.Round(x / this._x(1f));
	}

	// Token: 0x06001889 RID: 6281 RVA: 0x00011CF2 File Offset: 0x0000FEF2
	public float IY(float y)
	{
		return Mathf.Round(y / this._y(1f));
	}

	// Token: 0x0600188A RID: 6282 RVA: 0x00011D06 File Offset: 0x0000FF06
	private float _x(float x)
	{
		return (float)Screen.width * x / this.w;
	}

	// Token: 0x0600188B RID: 6283 RVA: 0x00011D17 File Offset: 0x0000FF17
	private float _y(float y)
	{
		return (float)Screen.height * y / this.h;
	}

	// Token: 0x0400153F RID: 5439
	private float w;

	// Token: 0x04001540 RID: 5440
	private float h;
}
