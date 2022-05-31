using System;

// Token: 0x020002BF RID: 703
public class PingMessage : Message, AuthenticationNotRequired
{
	// Token: 0x06001293 RID: 4755 RVA: 0x000059E4 File Offset: 0x00003BE4
	public override bool shouldLog()
	{
		return false;
	}

	// Token: 0x04000F65 RID: 3941
	[ServerToClient]
	public long time;
}
