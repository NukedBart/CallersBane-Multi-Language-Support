using System;

// Token: 0x02000310 RID: 784
public class GameMatchBaseMessage : Message
{
	// Token: 0x0600132D RID: 4909 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override ServerRole allowedServerRoles()
	{
		return ServerRole.LOBBY;
	}
}
