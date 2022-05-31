using System;
using NSCampaign;
using UnityEngine;

// Token: 0x020000DD RID: 221
public class UnitRenderer
{
	// Token: 0x06000777 RID: 1911 RVA: 0x00006989 File Offset: 0x00004B89
	public UnitRenderer(Player player)
	{
		this.progress = 1f;
		this.player = player;
		this.fromVisualX = this.RealX();
		this.fromVisualY = this.RealY();
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x000069BB File Offset: 0x00004BBB
	public float VX()
	{
		return this.Smoothstep(this.fromVisualX, this.RealX(), this.progress);
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x000069D5 File Offset: 0x00004BD5
	public float VY()
	{
		return this.Smoothstep(this.fromVisualY, this.RealY(), this.progress);
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x000069EF File Offset: 0x00004BEF
	public float VZ()
	{
		if (this.fromVisualY < this.RealY())
		{
			return this.RealZ();
		}
		if (this.progress < 0.9f)
		{
			return this.fromVisualZ;
		}
		return this.RealZ();
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x00006A26 File Offset: 0x00004C26
	private float RealX()
	{
		return HexUtil.GetXFor(this.player.x, this.player.y);
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x00006A43 File Offset: 0x00004C43
	private float RealY()
	{
		return HexUtil.GetYFor(this.player.x, this.player.y);
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x00006A60 File Offset: 0x00004C60
	private float RealZ()
	{
		return HexUtil.GetZ(this.player.y);
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x00006A72 File Offset: 0x00004C72
	public void MoveFrom()
	{
		this.fromVisualX = this.VX();
		this.fromVisualY = this.VY();
		this.fromVisualZ = this.VZ();
		this.progress = 0f;
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x00006AA3 File Offset: 0x00004CA3
	public void Update()
	{
		this.progress = Mathf.Min(1f, this.progress + Time.deltaTime * 4f);
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x00006AC7 File Offset: 0x00004CC7
	private float Smoothstep(float from, float to, float t)
	{
		return from + (to - from) * (t * t * (3f - 2f * t));
	}

	// Token: 0x04000594 RID: 1428
	private Player player;

	// Token: 0x04000595 RID: 1429
	private float fromVisualX;

	// Token: 0x04000596 RID: 1430
	private float fromVisualY;

	// Token: 0x04000597 RID: 1431
	private float fromVisualZ;

	// Token: 0x04000598 RID: 1432
	private float progress;
}
