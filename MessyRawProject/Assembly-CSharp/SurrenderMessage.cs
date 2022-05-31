using System;

// Token: 0x02000236 RID: 566
public class SurrenderMessage : BattleAction
{
	// Token: 0x06001183 RID: 4483 RVA: 0x0000D620 File Offset: 0x0000B820
	public override ServerRole allowedServerRoles()
	{
		return ServerRole.GAME;
	}
}
