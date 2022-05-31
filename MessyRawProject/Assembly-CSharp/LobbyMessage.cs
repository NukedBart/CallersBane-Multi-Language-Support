using System;

// Token: 0x02000301 RID: 769
public class LobbyMessage : Message
{
	// Token: 0x06001315 RID: 4885 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override ServerRole allowedServerRoles()
	{
		return ServerRole.LOBBY;
	}
}
