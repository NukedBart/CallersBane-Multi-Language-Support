using System;

// Token: 0x02000144 RID: 324
public class AuthProfile
{
	// Token: 0x06000A9F RID: 2719 RVA: 0x00002DDA File Offset: 0x00000FDA
	public AuthProfile()
	{
	}

	// Token: 0x06000AA0 RID: 2720 RVA: 0x00008F4F File Offset: 0x0000714F
	public AuthProfile(string id, string name)
	{
		this.id = id;
		this.name = name;
	}

	// Token: 0x04000828 RID: 2088
	public string id;

	// Token: 0x04000829 RID: 2089
	public string name;
}
