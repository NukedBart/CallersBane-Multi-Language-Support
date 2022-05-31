using System;

// Token: 0x020002DE RID: 734
public class SendFriendRequestMessage : Message
{
	// Token: 0x060012D8 RID: 4824 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public SendFriendRequestMessage()
	{
	}

	// Token: 0x060012D9 RID: 4825 RVA: 0x0000E206 File Offset: 0x0000C406
	public SendFriendRequestMessage(string profileName)
	{
		this.profileName = profileName;
	}

	// Token: 0x04000FAA RID: 4010
	public string profileName;
}
