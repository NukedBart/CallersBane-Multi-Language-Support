using System;

// Token: 0x0200044A RID: 1098
public class Logger
{
	// Token: 0x0600185C RID: 6236 RVA: 0x0001197B File Offset: 0x0000FB7B
	public Logger(Logger.LogOutputFunc output, LogLevel initialLogLevel)
	{
		this.setLevel(initialLogLevel);
		this.logger = output;
	}

	// Token: 0x0600185D RID: 6237 RVA: 0x00011991 File Offset: 0x0000FB91
	public void setLevel(LogLevel level)
	{
		this.logLevel = level;
	}

	// Token: 0x0600185E RID: 6238 RVA: 0x0001199A File Offset: 0x0000FB9A
	public void log(LogLevel level, string s)
	{
		if (level >= this.logLevel)
		{
			this.logger(new LogLine(level, s));
		}
	}

	// Token: 0x04001537 RID: 5431
	private LogLevel logLevel;

	// Token: 0x04001538 RID: 5432
	private Logger.LogOutputFunc logger;

	// Token: 0x0200044B RID: 1099
	// (Invoke) Token: 0x06001860 RID: 6240
	public delegate void LogOutputFunc(LogLine line);
}
