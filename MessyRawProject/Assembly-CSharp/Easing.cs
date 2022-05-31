using System;
using UnityEngine;

// Token: 0x0200042D RID: 1069
public class Easing
{
	// Token: 0x060017BC RID: 6076 RVA: 0x00011156 File Offset: 0x0000F356
	public static float Linear(Easing.Data d)
	{
		return d.src + d.t * d.delta();
	}

	// Token: 0x060017BD RID: 6077 RVA: 0x00091484 File Offset: 0x0008F684
	public static float InOutQuad(Easing.Data d)
	{
		float num = d.t + d.t;
		if (num < 1f)
		{
			return d.delta() / 2f * num * num + d.src;
		}
		return d.delta() / -2f * ((num - 1f) * (num - 3f) - 1f) + d.src;
	}

	// Token: 0x0200042E RID: 1070
	public class Data
	{
		// Token: 0x060017BE RID: 6078 RVA: 0x0001116C File Offset: 0x0000F36C
		public Data(float src, float dst) : this(src, dst, 0f)
		{
		}

		// Token: 0x060017BF RID: 6079 RVA: 0x0001117B File Offset: 0x0000F37B
		public Data(float src, float dst, float t)
		{
			this.src = src;
			this.dst = dst;
			this.t = t;
		}

		// Token: 0x060017C0 RID: 6080 RVA: 0x00011198 File Offset: 0x0000F398
		public float delta()
		{
			return this.dst - this.src;
		}

		// Token: 0x040014EC RID: 5356
		public float src;

		// Token: 0x040014ED RID: 5357
		public float dst;

		// Token: 0x040014EE RID: 5358
		public float t;
	}

	// Token: 0x0200042F RID: 1071
	public class Holder
	{
		// Token: 0x060017C1 RID: 6081 RVA: 0x000111A7 File Offset: 0x0000F3A7
		public Holder(Easing.Function easingFunction, Easing.Data data) : this(easingFunction, data, 1f)
		{
		}

		// Token: 0x060017C2 RID: 6082 RVA: 0x000111B6 File Offset: 0x0000F3B6
		public Holder(Easing.Function easingFunction, Easing.Data data, float maxTime)
		{
			this.func = easingFunction;
			this.data = data;
			this.maxTime = maxTime;
			this.startTime = Time.time;
		}

		// Token: 0x060017C3 RID: 6083 RVA: 0x000111DE File Offset: 0x0000F3DE
		public Easing.Data getData()
		{
			return this.data;
		}

		// Token: 0x060017C4 RID: 6084 RVA: 0x000111E6 File Offset: 0x0000F3E6
		public float value(float t)
		{
			this.data.t = Mth.clamp(t / this.maxTime, 0f, 1f);
			return this.func(this.data);
		}

		// Token: 0x060017C5 RID: 6085 RVA: 0x0001121B File Offset: 0x0000F41B
		public float currentValue()
		{
			return this.value(Time.time - this.startTime);
		}

		// Token: 0x040014EF RID: 5359
		private Easing.Function func;

		// Token: 0x040014F0 RID: 5360
		private Easing.Data data;

		// Token: 0x040014F1 RID: 5361
		private float maxTime;

		// Token: 0x040014F2 RID: 5362
		private float startTime;
	}

	// Token: 0x02000430 RID: 1072
	// (Invoke) Token: 0x060017C7 RID: 6087
	public delegate float Function(Easing.Data d);
}
