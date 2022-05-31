using System;

// Token: 0x020003A8 RID: 936
public class ProfileInfo
{
	// Token: 0x06001514 RID: 5396 RVA: 0x00002DDA File Offset: 0x00000FDA
	public ProfileInfo()
	{
	}

	// Token: 0x06001515 RID: 5397 RVA: 0x00082978 File Offset: 0x00080B78
	public ProfileInfo(ProfileInfo p)
	{
		this.id = p.id;
		this.name = p.name;
		this.adminRole = p.adminRole;
		this.featureType = p.featureType;
		this.isParentalConsentNeeded = p.isParentalConsentNeeded;
	}

	// Token: 0x06001516 RID: 5398 RVA: 0x0000F724 File Offset: 0x0000D924
	public void UnlockFullGame()
	{
		this.featureType = FeatureType.PREMIUM;
	}

	// Token: 0x06001517 RID: 5399 RVA: 0x0000F72D File Offset: 0x0000D92D
	public void SetFeatureType(FeatureType featureType)
	{
		this.featureType = featureType;
	}

	// Token: 0x0400123C RID: 4668
	public int id;

	// Token: 0x0400123D RID: 4669
	public string name;

	// Token: 0x0400123E RID: 4670
	public AdminRole adminRole;

	// Token: 0x0400123F RID: 4671
	public FeatureType featureType;

	// Token: 0x04001240 RID: 4672
	public bool isParentalConsentNeeded;
}
