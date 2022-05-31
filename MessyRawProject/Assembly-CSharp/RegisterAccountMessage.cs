using System;

// Token: 0x02000222 RID: 546
public class RegisterAccountMessage : Message
{
	// Token: 0x06001168 RID: 4456 RVA: 0x0000D492 File Offset: 0x0000B692
	public RegisterAccountMessage(string username, string password)
	{
		this.username = username;
		this.password = LoginMessage.hashPassword(password);
	}

	// Token: 0x04000DD7 RID: 3543
	public string username;

	// Token: 0x04000DD8 RID: 3544
	public string password;
}
