using System;

// Token: 0x02000147 RID: 327
public class AuthResponse : AccessToken
{
	// Token: 0x06000AA5 RID: 2725 RVA: 0x00008F83 File Offset: 0x00007183
	public AuthResponse()
	{
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x00008F8B File Offset: 0x0000718B
	public AuthResponse(string accessToken, string userId, string profileId, string username)
	{
		this.accessToken = accessToken;
		this.user = new AuthUser(userId);
		this.selectedProfile = new AuthProfile(profileId, username);
	}

	// Token: 0x0400082C RID: 2092
	public string clientToken;

	// Token: 0x0400082D RID: 2093
	public AuthUser user;

	// Token: 0x0400082E RID: 2094
	[ServerToClient]
	public AuthProfile[] availableProfiles;

	// Token: 0x0400082F RID: 2095
	public AuthProfile selectedProfile;
}
