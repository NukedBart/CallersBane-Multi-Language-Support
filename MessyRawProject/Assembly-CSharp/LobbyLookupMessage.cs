using System;

// Token: 0x020002FC RID: 764
public class LobbyLookupMessage : Message, AuthenticationNotRequired
{
	// Token: 0x0600130C RID: 4876 RVA: 0x0000E461 File Offset: 0x0000C661
	public override ServerRole allowedServerRoles()
	{
		return ServerRole.LOOKUP;
	}

	// Token: 0x04000FEE RID: 4078
	[ServerToClient]
	public string ip;

	// Token: 0x04000FEF RID: 4079
	[ServerToClient]
	public int port;
}
