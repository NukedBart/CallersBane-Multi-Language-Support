using System;

// Token: 0x020003E3 RID: 995
public class SvInt : SettingsValue<int>
{
	// Token: 0x060015DA RID: 5594 RVA: 0x0000FF39 File Offset: 0x0000E139
	public SvInt(int v)
	{
		base.value = v;
	}

	// Token: 0x060015DB RID: 5595 RVA: 0x0000FF5E File Offset: 0x0000E15E
	public SvInt Min(int low)
	{
		this.min = low;
		return this;
	}

	// Token: 0x060015DC RID: 5596 RVA: 0x0000FF68 File Offset: 0x0000E168
	public SvInt Max(int high)
	{
		this.max = high;
		return this;
	}

	// Token: 0x060015DD RID: 5597 RVA: 0x0000FF72 File Offset: 0x0000E172
	public SvInt Range(int low, int high)
	{
		this.min = low;
		this.max = high;
		this.set(base.value);
		return this;
	}

	// Token: 0x060015DE RID: 5598 RVA: 0x0000FF8F File Offset: 0x0000E18F
	public override void load(string s)
	{
		this.set(StringUtil.Parse(s, base.value));
	}

	// Token: 0x060015DF RID: 5599 RVA: 0x0000FFA3 File Offset: 0x0000E1A3
	public override void set(int v)
	{
		this._value = Mth.clamp(v, this.min, this.max);
	}

	// Token: 0x04001317 RID: 4887
	private int min = int.MinValue;

	// Token: 0x04001318 RID: 4888
	private int max = int.MaxValue;
}
