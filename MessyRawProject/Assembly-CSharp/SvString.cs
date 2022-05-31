using System;

// Token: 0x020003E5 RID: 997
public class SvString : SettingsValue<string>
{
	// Token: 0x060015EC RID: 5612 RVA: 0x00010029 File Offset: 0x0000E229
	public SvString()
	{
	}

	// Token: 0x060015ED RID: 5613 RVA: 0x00010031 File Offset: 0x0000E231
	public SvString(string s)
	{
		this.load(s);
	}

	// Token: 0x060015EE RID: 5614 RVA: 0x00010040 File Offset: 0x0000E240
	public override void load(string s)
	{
		base.value = s;
	}

	// Token: 0x060015EF RID: 5615 RVA: 0x00010049 File Offset: 0x0000E249
	public override string ToString()
	{
		return (base.value == null) ? string.Empty : base.value;
	}
}
