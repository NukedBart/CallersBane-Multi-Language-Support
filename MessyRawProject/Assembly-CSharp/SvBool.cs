using System;

// Token: 0x020003E0 RID: 992
public class SvBool : SettingsValue<bool>
{
	// Token: 0x060015CB RID: 5579 RVA: 0x0000FE0C File Offset: 0x0000E00C
	public SvBool(bool v)
	{
		base.value = v;
	}

	// Token: 0x060015CC RID: 5580 RVA: 0x0000FE1B File Offset: 0x0000E01B
	public override void load(string s)
	{
		base.value = StringUtil.Parse(s, base.value);
	}

	// Token: 0x060015CD RID: 5581 RVA: 0x00085018 File Offset: 0x00083218
	public override string ToString()
	{
		return base.value.ToString().ToLower();
	}

	// Token: 0x060015CE RID: 5582 RVA: 0x0000FE2F File Offset: 0x0000E02F
	public bool toggle()
	{
		base.value = !base.value;
		return base.value;
	}
}
