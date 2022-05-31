using System;

// Token: 0x02000283 RID: 643
public class GetGameLogMessage : Message, AuthenticationNotRequired
{
	// Token: 0x06001227 RID: 4647 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public GetGameLogMessage()
	{
	}

	// Token: 0x06001228 RID: 4648 RVA: 0x0000DC37 File Offset: 0x0000BE37
	public GetGameLogMessage(long gameId, string color, string key)
	{
		this.gameId = gameId;
		this.color = color;
		this.key = key;
	}

	// Token: 0x06001229 RID: 4649 RVA: 0x000059E4 File Offset: 0x00003BE4
	public override bool shouldLog()
	{
		return false;
	}

	// Token: 0x04000EF1 RID: 3825
	public long gameId;

	// Token: 0x04000EF2 RID: 3826
	public string color;

	// Token: 0x04000EF3 RID: 3827
	public string key;

	// Token: 0x04000EF4 RID: 3828
	public string log;
}
