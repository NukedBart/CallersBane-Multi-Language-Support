using System;

// Token: 0x020002B9 RID: 697
public class CliResponseMessage : Message
{
	// Token: 0x0600128C RID: 4748 RVA: 0x00077F24 File Offset: 0x00076124
	public static CliResponseMessage Fake(string text)
	{
		return new CliResponseMessage
		{
			text = text
		};
	}

	// Token: 0x04000F5F RID: 3935
	public string text;
}
