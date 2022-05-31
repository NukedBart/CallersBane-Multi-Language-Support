using System;

// Token: 0x0200028F RID: 655
public class SpectateLeaveGameMessage : Message
{
	// Token: 0x0600123C RID: 4668 RVA: 0x0000D620 File Offset: 0x0000B820
	public override ServerRole allowedServerRoles()
	{
		return ServerRole.GAME;
	}
}
