using System;

// Token: 0x020000DC RID: 220
public class HexUtil
{
	// Token: 0x0600076F RID: 1903 RVA: 0x0000690A File Offset: 0x00004B0A
	public static void SetTileWidth(float width)
	{
		HexUtil.tileWidth = width;
		HexUtil.tileHeight = width * 0.5f / 1.4f;
	}

	// Token: 0x1700006C RID: 108
	// (get) Token: 0x06000770 RID: 1904 RVA: 0x00006924 File Offset: 0x00004B24
	public static float TileWidth
	{
		get
		{
			return HexUtil.tileWidth;
		}
	}

	// Token: 0x1700006D RID: 109
	// (get) Token: 0x06000771 RID: 1905 RVA: 0x0000692B File Offset: 0x00004B2B
	public static float TileHeight
	{
		get
		{
			return HexUtil.tileHeight;
		}
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x00006932 File Offset: 0x00004B32
	public static float GetXFor(float x, float y)
	{
		return HexUtil.GetXFor(Mth.iFloor(x), Mth.iFloor(y));
	}

	// Token: 0x06000773 RID: 1907 RVA: 0x00006945 File Offset: 0x00004B45
	public static float GetXFor(int x, int y)
	{
		return HexUtil.tileWidth * (float)x + (float)(y % 2) * (HexUtil.tileWidth / 2f);
	}

	// Token: 0x06000774 RID: 1908 RVA: 0x00006960 File Offset: 0x00004B60
	public static float GetYFor(float x, float y)
	{
		return HexUtil.GetYFor(Mth.iFloor(x), Mth.iFloor(y));
	}

	// Token: 0x06000775 RID: 1909 RVA: 0x00006973 File Offset: 0x00004B73
	public static float GetYFor(int x, int y)
	{
		return HexUtil.tileHeight * (float)y;
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x0000697D File Offset: 0x00004B7D
	public static float GetZ(int y)
	{
		return (float)(1000 - 2 * y);
	}

	// Token: 0x04000592 RID: 1426
	public static float tileWidth = 1.4f;

	// Token: 0x04000593 RID: 1427
	private static float tileHeight = 0.5f;
}
