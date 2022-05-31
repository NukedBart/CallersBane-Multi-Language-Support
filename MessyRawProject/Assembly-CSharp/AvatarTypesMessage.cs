using System;

// Token: 0x0200032E RID: 814
public class AvatarTypesMessage : Message
{
	// Token: 0x06001363 RID: 4963 RVA: 0x000059E4 File Offset: 0x00003BE4
	public override bool shouldLog()
	{
		return false;
	}

	// Token: 0x0400104A RID: 4170
	public AvatarPart[] types;
}
