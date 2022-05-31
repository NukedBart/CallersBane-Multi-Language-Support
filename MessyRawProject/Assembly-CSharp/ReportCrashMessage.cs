using System;

// Token: 0x02000351 RID: 849
public class ReportCrashMessage : Message
{
	// Token: 0x06001398 RID: 5016 RVA: 0x0000E881 File Offset: 0x0000CA81
	public ReportCrashMessage(string data)
	{
		this.systemInfo = data;
	}

	// Token: 0x040010B2 RID: 4274
	public string systemInfo;
}
