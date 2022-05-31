using System;

// Token: 0x0200035C RID: 860
public class SpectateSetPermissionMessage : Message
{
	// Token: 0x060013A9 RID: 5033 RVA: 0x0000E906 File Offset: 0x0000CB06
	public SpectateSetPermissionMessage(SpectatePermission permission)
	{
		this.permission = permission;
	}

	// Token: 0x040010DE RID: 4318
	public SpectatePermission permission;
}
