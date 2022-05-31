using System;

// Token: 0x0200031E RID: 798
public class RemoveMessageMessage : Message
{
	// Token: 0x06001342 RID: 4930 RVA: 0x0000E5A3 File Offset: 0x0000C7A3
	public RemoveMessageMessage(MessageMessage.Type type)
	{
		this.type = type;
	}

	// Token: 0x0400103A RID: 4154
	public MessageMessage.Type type;
}
