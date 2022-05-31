using System;

// Token: 0x0200044D RID: 1101
public class Log
{
	// Token: 0x06001869 RID: 6249 RVA: 0x00011A10 File Offset: 0x0000FC10
	public static void print(object s)
	{
		Log.verbose(s);
	}

	// Token: 0x0600186A RID: 6250 RVA: 0x00011A18 File Offset: 0x0000FC18
	public static void verbose(object s)
	{
		Log._logger.log(LogLevel.VERBOSE, s.ToString());
	}

	// Token: 0x0600186B RID: 6251 RVA: 0x00011A2B File Offset: 0x0000FC2B
	public static void info(object s)
	{
		Log._logger.log(LogLevel.INFO, s.ToString());
	}

	// Token: 0x0600186C RID: 6252 RVA: 0x00011A3E File Offset: 0x0000FC3E
	public static void warning(object s)
	{
		Log._logger.log(LogLevel.WARNING, s.ToString());
	}

	// Token: 0x0600186D RID: 6253 RVA: 0x00011A51 File Offset: 0x0000FC51
	public static void error(object s)
	{
		Log._logger.log(LogLevel.ERROR, s.ToString());
	}

	// Token: 0x0600186E RID: 6254 RVA: 0x00011A64 File Offset: 0x0000FC64
	public static void critical(object s)
	{
		Log._logger.log(LogLevel.CRITICAL, s.ToString());
	}

	// Token: 0x0600186F RID: 6255 RVA: 0x00011A77 File Offset: 0x0000FC77
	public static void setLevel(LogLevel level)
	{
		Log._logger.setLevel(level);
	}

	// Token: 0x06001870 RID: 6256 RVA: 0x00011A84 File Offset: 0x0000FC84
	public static Logger getDefaultLogger()
	{
		return Log._logger;
	}

	// Token: 0x0400153B RID: 5435
	private static Logger _logger = Loggers.UnityLogger();
}
