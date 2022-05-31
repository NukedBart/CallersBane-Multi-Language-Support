using System;

// Token: 0x0200034E RID: 846
public abstract class LoginMessage : Message, AuthenticationNotRequired
{
	// Token: 0x06001392 RID: 5010 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public LoginMessage()
	{
	}

	// Token: 0x06001393 RID: 5011 RVA: 0x0000E841 File Offset: 0x0000CA41
	public LoginMessage setCredentials(string username, string password)
	{
		this.email = username;
		this.password = password;
		return this;
	}

	// Token: 0x06001394 RID: 5012 RVA: 0x000059E4 File Offset: 0x00003BE4
	public override bool shouldLogC2S()
	{
		return false;
	}

	// Token: 0x06001395 RID: 5013 RVA: 0x0000E852 File Offset: 0x0000CA52
	public static string hashPassword(string password)
	{
		return Hash.sha256("ScrollsClientSalt5438_" + password);
	}

	// Token: 0x040010A9 RID: 4265
	private const string Salt = "ScrollsClientSalt5438_";

	// Token: 0x040010AA RID: 4266
	public string email;

	// Token: 0x040010AB RID: 4267
	public string password;

	// Token: 0x040010AC RID: 4268
	public string authHash;

	// Token: 0x040010AD RID: 4269
	public AccessToken accessToken;
}
