using System;

// Token: 0x02000287 RID: 647
public class MatchEnjoymentMessage : Message
{
	// Token: 0x0600122E RID: 4654 RVA: 0x0000DC54 File Offset: 0x0000BE54
	public MatchEnjoymentMessage(int rating)
	{
		this.rating = rating;
	}

	// Token: 0x0600122F RID: 4655 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public MatchEnjoymentMessage()
	{
	}

	// Token: 0x04000EF9 RID: 3833
	public int rating;
}
