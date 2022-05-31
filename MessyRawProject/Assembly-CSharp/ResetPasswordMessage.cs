using System;

// Token: 0x02000223 RID: 547
public class ResetPasswordMessage : Message
{
	// Token: 0x06001169 RID: 4457 RVA: 0x0000D4AD File Offset: 0x0000B6AD
	public ResetPasswordMessage(string username, string password)
	{
		this.username = username;
		this.password = LoginMessage.hashPassword(password);
	}

	// Token: 0x04000DD9 RID: 3545
	public string username;

	// Token: 0x04000DDA RID: 3546
	public string password;
}
