using System;

// Token: 0x020002B8 RID: 696
public class WhisperMessage : ChatMessageMessage
{
	// Token: 0x06001288 RID: 4744 RVA: 0x0000DEDA File Offset: 0x0000C0DA
	public WhisperMessage()
	{
	}

	// Token: 0x06001289 RID: 4745 RVA: 0x0000DF07 File Offset: 0x0000C107
	public WhisperMessage(string toProfileName, string text)
	{
		this.toProfileName = toProfileName;
		base.text = text;
	}

	// Token: 0x0600128A RID: 4746 RVA: 0x0000DF1D File Offset: 0x0000C11D
	public string GetChatroomName()
	{
		if (this.from == App.MyProfile.ProfileInfo.name)
		{
			return this.toProfileName;
		}
		return this.from;
	}

	// Token: 0x04000F5E RID: 3934
	public string toProfileName;
}
