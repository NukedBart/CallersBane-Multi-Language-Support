using System;

// Token: 0x020002AE RID: 686
public class GameChallengeBaseMessage : Message
{
	// Token: 0x06001277 RID: 4727 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override ServerRole allowedServerRoles()
	{
		return ServerRole.LOBBY;
	}
}
