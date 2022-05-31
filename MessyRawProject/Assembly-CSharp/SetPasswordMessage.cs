using System;

// Token: 0x02000224 RID: 548
public class SetPasswordMessage : Message
{
	// Token: 0x0600116A RID: 4458 RVA: 0x0000D4C8 File Offset: 0x0000B6C8
	public SetPasswordMessage(string password, string newPassword)
	{
		this.password = LoginMessage.hashPassword(password);
		this.newPassword = LoginMessage.hashPassword(newPassword);
	}

	// Token: 0x04000DDB RID: 3547
	public string password;

	// Token: 0x04000DDC RID: 3548
	public string newPassword;
}
