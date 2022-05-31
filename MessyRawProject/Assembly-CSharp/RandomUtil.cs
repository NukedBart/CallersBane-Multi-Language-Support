using System;
using System.Collections.Generic;

// Token: 0x02000452 RID: 1106
public static class RandomUtil
{
	// Token: 0x060018A0 RID: 6304 RVA: 0x00011E3F File Offset: 0x0001003F
	public static T choice<T>(IList<T> items)
	{
		if (items == null || items.Count == 0)
		{
			throw new ArgumentException("items must be of size >= 1");
		}
		return items[RandomUtil.rnd.Next(items.Count)];
	}

	// Token: 0x060018A1 RID: 6305 RVA: 0x00092C38 File Offset: 0x00090E38
	public static List<T> sample<T>(IList<T> items, int count)
	{
		List<T> list = new List<T>(items);
		RandomUtil.shuffle<T>(list);
		return list.GetRange(0, count);
	}

	// Token: 0x060018A2 RID: 6306 RVA: 0x00092C5C File Offset: 0x00090E5C
	public static void shuffle<T>(IList<T> items)
	{
		int count = items.Count;
		for (int i = count - 1; i > 0; i--)
		{
			int num = RandomUtil.rnd.Next(i + 1);
			T t = items[i];
			items[i] = items[num];
			items[num] = t;
		}
	}

	// Token: 0x060018A3 RID: 6307 RVA: 0x00011E73 File Offset: 0x00010073
	public static float random()
	{
		return (float)RandomUtil.rnd.NextDouble();
	}

	// Token: 0x060018A4 RID: 6308 RVA: 0x00011E80 File Offset: 0x00010080
	public static float fakeGaussian()
	{
		return (float)(RandomUtil.rnd.NextDouble() - RandomUtil.rnd.NextDouble());
	}

	// Token: 0x060018A5 RID: 6309 RVA: 0x00011E98 File Offset: 0x00010098
	public static float random(float min, float max)
	{
		return min + (max - min) * RandomUtil.random();
	}

	// Token: 0x04001542 RID: 5442
	private static Random rnd = new Random();
}
