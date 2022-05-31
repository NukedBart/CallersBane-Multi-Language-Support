using System;

// Token: 0x020003A9 RID: 937
public class MyProfileInfo : ProfileInfo
{
	// Token: 0x06001518 RID: 5400 RVA: 0x0000F736 File Offset: 0x0000D936
	public MyProfileInfo()
	{
	}

	// Token: 0x06001519 RID: 5401 RVA: 0x0000F73E File Offset: 0x0000D93E
	public MyProfileInfo(ProfileInfo profile, string userUuid, string profileUuid) : base(profile)
	{
		this.userUuid = userUuid;
		this.profileUuid = profileUuid;
	}

	// Token: 0x04001241 RID: 4673
	public string userUuid;

	// Token: 0x04001242 RID: 4674
	public string profileUuid;
}
