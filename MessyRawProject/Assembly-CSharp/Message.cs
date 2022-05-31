using System;

// Token: 0x02000316 RID: 790
public class Message
{
	// Token: 0x06001333 RID: 4915 RVA: 0x000786FC File Offset: 0x000768FC
	public Message()
	{
		string text = base.GetType().ToString();
		int num = text.LastIndexOf("Message");
		if (text.EndsWith("Message"))
		{
			this.msg = text.Substring(0, num);
		}
		else
		{
			Log.error("This Message subclass doesn't end with 'Message'! : " + text);
			this.msg = "<Unnamed>";
		}
	}

	// Token: 0x06001334 RID: 4916 RVA: 0x0000E500 File Offset: 0x0000C700
	public override string ToString()
	{
		return MessageFactory.toString(this);
	}

	// Token: 0x06001335 RID: 4917 RVA: 0x0000E508 File Offset: 0x0000C708
	public virtual bool shouldLogS2C()
	{
		return this.shouldLog();
	}

	// Token: 0x06001336 RID: 4918 RVA: 0x0000E508 File Offset: 0x0000C708
	public virtual bool shouldLogC2S()
	{
		return this.shouldLog();
	}

	// Token: 0x06001337 RID: 4919 RVA: 0x00004AAC File Offset: 0x00002CAC
	public virtual bool shouldLog()
	{
		return true;
	}

	// Token: 0x06001338 RID: 4920 RVA: 0x0000E510 File Offset: 0x0000C710
	public bool fromCommunicator(MiniCommunicator c)
	{
		return c != null && this.comm == c;
	}

	// Token: 0x06001339 RID: 4921 RVA: 0x0000E52D File Offset: 0x0000C72D
	public string getRawText()
	{
		return this._rawText;
	}

	// Token: 0x0600133A RID: 4922 RVA: 0x0000E535 File Offset: 0x0000C735
	public void setRawText(string s)
	{
		this._rawText = s;
	}

	// Token: 0x0600133B RID: 4923 RVA: 0x0000E53E File Offset: 0x0000C73E
	public virtual ServerRole allowedServerRoles()
	{
		return ServerRole.LOBBY | ServerRole.GAME | ServerRole.RESOURCE | ServerRole.LOOKUP;
	}

	// Token: 0x0600133C RID: 4924 RVA: 0x0000E542 File Offset: 0x0000C742
	public bool isAllowedForServerRole(ServerRole role)
	{
		return Message.hasRole(role, this.allowedServerRoles());
	}

	// Token: 0x0600133D RID: 4925 RVA: 0x0000E550 File Offset: 0x0000C750
	public static bool hasRole(ServerRole all, ServerRole query)
	{
		return (all & query) != (ServerRole)0;
	}

	// Token: 0x04001028 RID: 4136
	public string msg;

	// Token: 0x04001029 RID: 4137
	public MiniCommunicator comm;

	// Token: 0x0400102A RID: 4138
	private string _rawText;
}
