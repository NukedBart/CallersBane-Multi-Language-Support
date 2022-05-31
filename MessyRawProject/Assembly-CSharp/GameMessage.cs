using System;

// Token: 0x0200027E RID: 638
public class GameMessage : Message
{
	// Token: 0x06001221 RID: 4641 RVA: 0x0000D620 File Offset: 0x0000B820
	public override ServerRole allowedServerRoles()
	{
		return ServerRole.GAME;
	}
}
