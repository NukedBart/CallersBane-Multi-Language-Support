using System;

// Token: 0x02000319 RID: 793
public class Update : Attribute
{
	// Token: 0x0600133F RID: 4927 RVA: 0x0000E563 File Offset: 0x0000C763
	public Update(params Type[] messageTypes)
	{
		this.messageTypes = messageTypes;
	}

	// Token: 0x04001031 RID: 4145
	public Type[] messageTypes;
}
