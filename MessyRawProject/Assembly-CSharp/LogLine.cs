using System;

// Token: 0x02000449 RID: 1097
public class LogLine
{
	// Token: 0x06001859 RID: 6233 RVA: 0x0001190A File Offset: 0x0000FB0A
	public LogLine(LogLevel level, string text)
	{
		this.level = level;
		this.text = text;
		this.time = DateTime.Now;
	}

	// Token: 0x0600185A RID: 6234 RVA: 0x00011936 File Offset: 0x0000FB36
	public string getDefaultFormatting()
	{
		return string.Concat(new string[]
		{
			this.time.ToString(),
			" :",
			this.tag,
			": ",
			this.text
		});
	}

	// Token: 0x0600185B RID: 6235 RVA: 0x00011973 File Offset: 0x0000FB73
	public override string ToString()
	{
		return this.getDefaultFormatting();
	}

	// Token: 0x04001533 RID: 5427
	public LogLevel level;

	// Token: 0x04001534 RID: 5428
	public string tag = string.Empty;

	// Token: 0x04001535 RID: 5429
	public string text;

	// Token: 0x04001536 RID: 5430
	public DateTime time;
}
