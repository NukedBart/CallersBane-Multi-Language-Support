using System;

// Token: 0x02000323 RID: 803
public abstract class PlayGameBaseMessage : Message
{
	// Token: 0x0600134F RID: 4943 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override ServerRole allowedServerRoles()
	{
		return ServerRole.LOBBY;
	}

	// Token: 0x04001040 RID: 4160
	public string deck;
}
