using System;
using UnityEngine;

// Token: 0x02000190 RID: 400
public class CircularFloatBuffer : CircularBuffer<float>
{
	// Token: 0x06000C67 RID: 3175 RVA: 0x0000A18A File Offset: 0x0000838A
	public CircularFloatBuffer(int size) : base(size)
	{
	}

	// Token: 0x06000C68 RID: 3176 RVA: 0x0005752C File Offset: 0x0005572C
	public float sum()
	{
		float num = 0f;
		foreach (float num2 in this._arr)
		{
			num += num2;
		}
		return num;
	}

	// Token: 0x06000C69 RID: 3177 RVA: 0x0000A193 File Offset: 0x00008393
	public float avg()
	{
		return this.sum() / (float)base.size();
	}

	// Token: 0x06000C6A RID: 3178 RVA: 0x00057564 File Offset: 0x00055764
	public float sumAbs()
	{
		float num = 0f;
		foreach (float num2 in this._arr)
		{
			num += Mathf.Abs(num2);
		}
		return num;
	}
}
