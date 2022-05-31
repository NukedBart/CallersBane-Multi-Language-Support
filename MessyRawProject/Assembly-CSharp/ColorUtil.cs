using System;
using UnityEngine;

// Token: 0x02000427 RID: 1063
public static class ColorUtil
{
	// Token: 0x06001791 RID: 6033 RVA: 0x00010EFA File Offset: 0x0000F0FA
	public static Color FromHex24(uint hex)
	{
		return ColorUtil.FromHex32((hex & 16777215u) | 4278190080u);
	}

	// Token: 0x06001792 RID: 6034 RVA: 0x000911A4 File Offset: 0x0008F3A4
	public static Color FromHex24(uint hex, float a)
	{
		uint num = (uint)(255f * Mth.clamp(a, 0f, 1f) + 0.5f);
		return ColorUtil.FromHex32((hex & 16777215u) | num << 24);
	}

	// Token: 0x06001793 RID: 6035 RVA: 0x000911E0 File Offset: 0x0008F3E0
	public static Color FromHex32(uint hex)
	{
		float num = (hex >> 24) * 0.003921569f;
		float num2 = (hex >> 16 & 255u) * 0.003921569f;
		float num3 = (hex >> 8 & 255u) * 0.003921569f;
		float num4 = (hex & 255u) * 0.003921569f;
		return new Color(num2, num3, num4, num);
	}

	// Token: 0x06001794 RID: 6036 RVA: 0x00010F0E File Offset: 0x0000F10E
	public static Color Grey(float g)
	{
		return new Color(g, g, g, 1f);
	}

	// Token: 0x06001795 RID: 6037 RVA: 0x00010F1D File Offset: 0x0000F11D
	public static Color FromInts(int r, int g, int b)
	{
		return ColorUtil.FromInts(r, g, b, 255);
	}

	// Token: 0x06001796 RID: 6038 RVA: 0x0009123C File Offset: 0x0008F43C
	public static Color FromInts(int r, int g, int b, int a)
	{
		return new Color((float)r * 0.003921569f, (float)g * 0.003921569f, (float)b * 0.003921569f, (float)a * 0.003921569f);
	}

	// Token: 0x06001797 RID: 6039 RVA: 0x00010F2C File Offset: 0x0000F12C
	public static Color GetWithAlpha(Color c, float newAlpha)
	{
		return new Color(c.r, c.g, c.b, newAlpha);
	}

	// Token: 0x06001798 RID: 6040 RVA: 0x00091270 File Offset: 0x0008F470
	public static Color Maximize(Color c)
	{
		float num = 1f / Mathf.Max(new float[]
		{
			c.r,
			c.g,
			c.b
		});
		return new Color(num * c.r, num * c.g, num * c.b, c.a);
	}

	// Token: 0x06001799 RID: 6041 RVA: 0x00010F49 File Offset: 0x0000F149
	public static Color Brighten(Color c, float v)
	{
		return new Color(c.r + v, c.g + v, c.b + v, c.a);
	}

	// Token: 0x0600179A RID: 6042 RVA: 0x00010F72 File Offset: 0x0000F172
	public static Color Darken(Color c, float v)
	{
		return new Color(c.r - v, c.g - v, c.b - v, c.a);
	}

	// Token: 0x0600179B RID: 6043 RVA: 0x000912D4 File Offset: 0x0008F4D4
	public static Color LerpRGB(Color src, Color dst, float a)
	{
		return new Color(src.r + (dst.r - src.r) * a, src.g + (dst.g - src.g) * a, src.b + (dst.b - src.b) * a, src.a);
	}

	// Token: 0x0600179C RID: 6044 RVA: 0x00091338 File Offset: 0x0008F538
	public static string ToHexString(Color c)
	{
		return string.Concat(new string[]
		{
			"#",
			((int)(c.r * 255f)).ToString("x2"),
			((int)(c.g * 255f)).ToString("x2"),
			((int)(c.b * 255f)).ToString("x2"),
			((int)(c.a * 255f)).ToString("x2")
		});
	}
}
