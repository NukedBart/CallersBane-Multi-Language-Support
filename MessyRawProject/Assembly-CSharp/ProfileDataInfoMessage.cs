using System;

// Token: 0x02000337 RID: 823
public class ProfileDataInfoMessage : Message
{
	// Token: 0x06001372 RID: 4978 RVA: 0x000059E4 File Offset: 0x00003BE4
	public override bool shouldLog()
	{
		return false;
	}

	// Token: 0x0400106A RID: 4202
	[ServerToClient]
	public ProfileData profileData;
}
