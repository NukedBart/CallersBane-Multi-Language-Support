using System;

// Token: 0x020003E2 RID: 994
public class SvFloat : SettingsValue<float>
{
	// Token: 0x060015D3 RID: 5587 RVA: 0x0000FEA3 File Offset: 0x0000E0A3
	public SvFloat(float v)
	{
		base.value = v;
	}

	// Token: 0x060015D4 RID: 5588 RVA: 0x0000FEC8 File Offset: 0x0000E0C8
	public SvFloat Min(float low)
	{
		this.min = low;
		return this;
	}

	// Token: 0x060015D5 RID: 5589 RVA: 0x0000FED2 File Offset: 0x0000E0D2
	public SvFloat Max(float high)
	{
		this.max = high;
		return this;
	}

	// Token: 0x060015D6 RID: 5590 RVA: 0x0000FEDC File Offset: 0x0000E0DC
	public SvFloat Range(float low, float high)
	{
		this.min = low;
		this.max = high;
		this.set(base.value);
		return this;
	}

	// Token: 0x060015D7 RID: 5591 RVA: 0x0000FEF9 File Offset: 0x0000E0F9
	public SvFloat Unit()
	{
		return this.Range(0f, 1f);
	}

	// Token: 0x060015D8 RID: 5592 RVA: 0x0000FF0B File Offset: 0x0000E10B
	public override void load(string s)
	{
		this.set(StringUtil.Parse(s, base.value));
	}

	// Token: 0x060015D9 RID: 5593 RVA: 0x0000FF1F File Offset: 0x0000E11F
	public override void set(float v)
	{
		this._value = Mth.clamp(v, this.min, this.max);
	}

	// Token: 0x04001315 RID: 4885
	private float min = float.MinValue;

	// Token: 0x04001316 RID: 4886
	private float max = float.MaxValue;
}
