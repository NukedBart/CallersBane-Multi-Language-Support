using System;

// Token: 0x02000210 RID: 528
public class MappedString
{
	// Token: 0x060010D1 RID: 4305 RVA: 0x00002DDA File Offset: 0x00000FDA
	public MappedString()
	{
	}

	// Token: 0x060010D2 RID: 4306 RVA: 0x0000D049 File Offset: 0x0000B249
	public MappedString(string key, string value)
	{
		this.key = key;
		this.value = value;
	}

	// Token: 0x04000D38 RID: 3384
	public string key;

	// Token: 0x04000D39 RID: 3385
	public string value;
}
