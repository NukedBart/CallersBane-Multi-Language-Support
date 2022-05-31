using System;

// Token: 0x020002CB RID: 715
public class GetCustomGamesMpMessage : GetCustomGamesMessage
{
	// Token: 0x060012B4 RID: 4788 RVA: 0x0000E0D5 File Offset: 0x0000C2D5
	public GetCustomGamesMpMessage() : this(null)
	{
	}

	// Token: 0x060012B5 RID: 4789 RVA: 0x0000E0C6 File Offset: 0x0000C2C6
	public GetCustomGamesMpMessage(string search)
	{
		this.search = search;
	}
}
