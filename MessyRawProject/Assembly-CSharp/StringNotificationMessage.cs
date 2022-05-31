using System;

// Token: 0x020002E1 RID: 737
public class StringNotificationMessage : Message
{
	// Token: 0x060012DE RID: 4830 RVA: 0x0000E233 File Offset: 0x0000C433
	public StringNotificationMessage(string header, string message)
	{
		this.message = message;
		this.header = header;
	}

	// Token: 0x04000FAE RID: 4014
	public string message;

	// Token: 0x04000FAF RID: 4015
	public string header;
}
