using System;

// Token: 0x020002DC RID: 732
public class RemoveFriendMessage : Message
{
	// Token: 0x060012D4 RID: 4820 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public RemoveFriendMessage()
	{
	}

	// Token: 0x060012D5 RID: 4821 RVA: 0x0000E1D2 File Offset: 0x0000C3D2
	public RemoveFriendMessage(string profileName)
	{
		this.profileName = profileName;
	}

	// Token: 0x04000FA6 RID: 4006
	public string profileName;
}
