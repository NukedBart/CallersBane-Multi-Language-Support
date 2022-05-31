using System;

// Token: 0x02000450 RID: 1104
public class Mth
{
	// Token: 0x0600188D RID: 6285 RVA: 0x00011D28 File Offset: 0x0000FF28
	public static float clamp(float val, float min, float max)
	{
		if (val <= min)
		{
			return min;
		}
		if (val >= max)
		{
			return max;
		}
		return val;
	}

	// Token: 0x0600188E RID: 6286 RVA: 0x00011D3D File Offset: 0x0000FF3D
	public static int clamp(int val, int min, int max)
	{
		if (val <= min)
		{
			return min;
		}
		if (val >= max)
		{
			return max;
		}
		return val;
	}

	// Token: 0x0600188F RID: 6287 RVA: 0x00011D52 File Offset: 0x0000FF52
	public static int iFloor(float x)
	{
		if (x >= 0f)
		{
			return (int)x;
		}
		return (int)x - 1;
	}

	// Token: 0x06001890 RID: 6288 RVA: 0x00011D66 File Offset: 0x0000FF66
	public static float floor(float x)
	{
		return (float)Math.Floor((double)x);
	}

	// Token: 0x06001891 RID: 6289 RVA: 0x00011D70 File Offset: 0x0000FF70
	public static float linearUD(float x, float xhigh)
	{
		return Mth.linearUFD(x, xhigh, xhigh);
	}

	// Token: 0x06001892 RID: 6290 RVA: 0x00092B04 File Offset: 0x00090D04
	public static float linearUFD(float x, float xhigh, float xdown)
	{
		if (x <= xhigh)
		{
			return (xhigh <= 0f) ? 1f : (x / xhigh);
		}
		if (x <= xdown)
		{
			return 1f;
		}
		return (xdown >= 1f) ? 0f : (1f - (x - xdown) / (1f - xdown));
	}

	// Token: 0x06001893 RID: 6291 RVA: 0x00011D7A File Offset: 0x0000FF7A
	public static float sign(float x)
	{
		return (float)((x <= 0f) ? ((x >= 0f) ? 0 : -1) : 1);
	}

	// Token: 0x06001894 RID: 6292 RVA: 0x00011DA0 File Offset: 0x0000FFA0
	public static bool isClose(float a, float b, float epsilon)
	{
		return Math.Abs(a - b) < epsilon;
	}

	// Token: 0x06001895 RID: 6293 RVA: 0x00011DAD File Offset: 0x0000FFAD
	public static bool transformToRange(ref float v, float low, float high)
	{
		if (v < low || v > high)
		{
			return false;
		}
		v = (v - low) / (high - low);
		return true;
	}

	// Token: 0x06001896 RID: 6294 RVA: 0x00092B64 File Offset: 0x00090D64
	public static int firstNonZero(params int[] values)
	{
		foreach (int num in values)
		{
			if (num != 0)
			{
				return num;
			}
		}
		return 0;
	}
}
