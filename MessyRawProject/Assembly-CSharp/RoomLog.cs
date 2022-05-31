using System;
using System.Collections.Generic;

// Token: 0x020003B2 RID: 946
public class RoomLog
{
	// Token: 0x0600152E RID: 5422 RVA: 0x0000F801 File Offset: 0x0000DA01
	public List<RoomLog.ChatLine> GetLines()
	{
		return this.lines;
	}

	// Token: 0x0600152F RID: 5423 RVA: 0x0000F809 File Offset: 0x0000DA09
	public void AddLine(RoomLog.ChatLine line)
	{
		this.lines.Add(line);
		if (this.lines.Count > 500)
		{
			this.lines.RemoveAt(0);
		}
	}

	// Token: 0x04001265 RID: 4709
	private const int MAX_LINES = 500;

	// Token: 0x04001266 RID: 4710
	private List<RoomLog.ChatLine> lines = new List<RoomLog.ChatLine>();

	// Token: 0x04001267 RID: 4711
	public bool allRead = true;

	// Token: 0x020003B3 RID: 947
	public class ChatLine
	{
		// Token: 0x06001530 RID: 5424 RVA: 0x00082C6C File Offset: 0x00080E6C
		public ChatLine(string time, string text, string from, AdminRole senderAdminRole, FeatureType senderFeatureType)
		{
			this.timestamp = time;
			this.text = text;
			this.from = from;
			this.senderAdminRole = senderAdminRole;
			this.senderFeatureType = senderFeatureType;
		}

		// Token: 0x04001268 RID: 4712
		public string timestamp = string.Empty;

		// Token: 0x04001269 RID: 4713
		public string text = string.Empty;

		// Token: 0x0400126A RID: 4714
		public string from = string.Empty;

		// Token: 0x0400126B RID: 4715
		public AdminRole senderAdminRole;

		// Token: 0x0400126C RID: 4716
		public FeatureType senderFeatureType;
	}
}
