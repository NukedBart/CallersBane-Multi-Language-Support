using System;

// Token: 0x02000286 RID: 646
public class LeaveGameMessage : Message
{
	// Token: 0x0600122D RID: 4653 RVA: 0x0000D620 File Offset: 0x0000B820
	public override ServerRole allowedServerRoles()
	{
		return ServerRole.GAME;
	}

	// Token: 0x04000EF8 RID: 3832
	public string from;
}
