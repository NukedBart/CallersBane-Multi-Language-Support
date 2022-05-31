using System;

// Token: 0x02000350 RID: 848
public class ReportBugMessage : Message
{
	// Token: 0x06001397 RID: 5015 RVA: 0x0000E864 File Offset: 0x0000CA64
	public ReportBugMessage(string report, string systemInfo, string clientLog)
	{
		this.report = report;
		this.systemInfo = systemInfo;
		this.clientLog = clientLog;
	}

	// Token: 0x040010AF RID: 4271
	public string report;

	// Token: 0x040010B0 RID: 4272
	public string systemInfo;

	// Token: 0x040010B1 RID: 4273
	public string clientLog;
}
