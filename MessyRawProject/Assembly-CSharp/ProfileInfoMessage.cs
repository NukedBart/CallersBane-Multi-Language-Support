using System;

// Token: 0x02000338 RID: 824
public class ProfileInfoMessage : Message
{
	// Token: 0x06001374 RID: 4980 RVA: 0x0000E72B File Offset: 0x0000C92B
	public static implicit operator MyProfileInfo(ProfileInfoMessage msg)
	{
		return new MyProfileInfo(msg.profile, msg.userUuid, msg.profileUuid);
	}

	// Token: 0x0400106B RID: 4203
	public ProfileInfo profile;

	// Token: 0x0400106C RID: 4204
	public string userUuid;

	// Token: 0x0400106D RID: 4205
	public string profileUuid;
}
