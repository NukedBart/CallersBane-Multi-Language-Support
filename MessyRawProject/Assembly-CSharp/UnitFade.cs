using System;
using UnityEngine;

// Token: 0x02000439 RID: 1081
internal class UnitFade : FloatFade
{
	// Token: 0x060017ED RID: 6125 RVA: 0x000112AF File Offset: 0x0000F4AF
	public UnitFade(float initialValue, float incDelta, float decDelta)
	{
		this._value = initialValue;
		this._inc = incDelta;
		this._dec = decDelta;
	}

	// Token: 0x060017EE RID: 6126 RVA: 0x000918B0 File Offset: 0x0008FAB0
	public float update(bool raise)
	{
		if (raise)
		{
			this._value += this._inc;
		}
		else
		{
			this._value -= this._dec;
		}
		this._value = Mathf.Clamp01(this._value);
		return this._value;
	}

	// Token: 0x060017EF RID: 6127 RVA: 0x000112CC File Offset: 0x0000F4CC
	public float get()
	{
		return this._value;
	}

	// Token: 0x04001503 RID: 5379
	private float _value;

	// Token: 0x04001504 RID: 5380
	private float _inc;

	// Token: 0x04001505 RID: 5381
	private float _dec;
}
