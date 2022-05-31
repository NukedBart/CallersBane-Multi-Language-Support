using System;

// Token: 0x0200035F RID: 863
public class SpectateUserRequestMessage : Message
{
	// Token: 0x060013AB RID: 5035 RVA: 0x0000E915 File Offset: 0x0000CB15
	public SpectateUserRequestMessage(ProfileInfo profile)
	{
		this.profileName = profile.name;
	}

	// Token: 0x060013AC RID: 5036 RVA: 0x0000E929 File Offset: 0x0000CB29
	public SpectateUserRequestMessage(string profileName)
	{
		this.profileName = profileName;
	}

	// Token: 0x040010E4 RID: 4324
	public string profileName;
}
