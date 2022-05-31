using System;

// Token: 0x020002CA RID: 714
public class GetCustomGamesSpMessage : GetCustomGamesMessage
{
	// Token: 0x060012B2 RID: 4786 RVA: 0x0000E0BD File Offset: 0x0000C2BD
	public GetCustomGamesSpMessage() : this(null)
	{
	}

	// Token: 0x060012B3 RID: 4787 RVA: 0x0000E0C6 File Offset: 0x0000C2C6
	public GetCustomGamesSpMessage(string search)
	{
		this.search = search;
	}
}
