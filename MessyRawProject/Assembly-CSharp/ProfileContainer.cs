using System;

// Token: 0x020003A7 RID: 935
public class ProfileContainer
{
	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x06001510 RID: 5392 RVA: 0x0000F702 File Offset: 0x0000D902
	// (set) Token: 0x06001511 RID: 5393 RVA: 0x0000F70A File Offset: 0x0000D90A
	public ProfileData ProfileData
	{
		get
		{
			return this.profileData;
		}
		set
		{
			this.profileData = value;
		}
	}

	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x06001512 RID: 5394 RVA: 0x0000F713 File Offset: 0x0000D913
	// (set) Token: 0x06001513 RID: 5395 RVA: 0x0000F71B File Offset: 0x0000D91B
	public MyProfileInfo ProfileInfo
	{
		get
		{
			return this.profileInfo;
		}
		set
		{
			this.profileInfo = value;
		}
	}

	// Token: 0x0400123A RID: 4666
	private ProfileData profileData = new ProfileData();

	// Token: 0x0400123B RID: 4667
	private MyProfileInfo profileInfo = new MyProfileInfo();
}
