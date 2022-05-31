using System;

// Token: 0x020003AD RID: 941
public static class FeatureTypeExtensions
{
	// Token: 0x0600151B RID: 5403 RVA: 0x0000D58A File Offset: 0x0000B78A
	public static bool isDemo(this FeatureType t)
	{
		return t == FeatureType.DEMO;
	}

	// Token: 0x0600151C RID: 5404 RVA: 0x00005376 File Offset: 0x00003576
	public static bool isPremium(this FeatureType t)
	{
		return t == FeatureType.PREMIUM;
	}

	// Token: 0x0600151D RID: 5405 RVA: 0x0000F755 File Offset: 0x0000D955
	public static bool canTrade(this FeatureType t)
	{
		return FeatureTypeExtensions.premium() && t.isPremium();
	}

	// Token: 0x0600151E RID: 5406 RVA: 0x0000F76A File Offset: 0x0000D96A
	private static bool premium()
	{
		return FeatureTypeExtensions.valid() && App.MyProfile.ProfileInfo.featureType.isPremium();
	}

	// Token: 0x0600151F RID: 5407 RVA: 0x0000F78D File Offset: 0x0000D98D
	private static bool valid()
	{
		return App.MyProfile.ProfileInfo != null;
	}
}
