using System;
using System.Collections.Generic;

// Token: 0x0200044E RID: 1102
public class TimedLog
{
	// Token: 0x06001873 RID: 6259 RVA: 0x00011A97 File Offset: 0x0000FC97
	public static void Begin()
	{
		TimedLog.logLines.Clear();
		TimedLog.startTime = TimeUtil.CurrentTimeMillis();
	}

	// Token: 0x06001874 RID: 6260 RVA: 0x00092AC4 File Offset: 0x00090CC4
	public static void End()
	{
		string s = string.Join("\n", TimedLog.logLines.ToArray());
		if (TimeUtil.CurrentTimeMillis() - TimedLog.startTime > TimedLog.threshold)
		{
			Log.info(s);
		}
	}

	// Token: 0x06001875 RID: 6261 RVA: 0x00011AAD File Offset: 0x0000FCAD
	public static void SetThreshold(long milliseconds)
	{
		TimedLog.threshold = milliseconds;
	}

	// Token: 0x06001876 RID: 6262 RVA: 0x00011AB5 File Offset: 0x0000FCB5
	public static void LogWithTime(string str)
	{
		TimedLog.logLines.Add(string.Concat(new object[]
		{
			str,
			" [Milliseconds: ",
			TimeUtil.CurrentTimeMillis() - TimedLog.startTime,
			"]"
		}));
	}

	// Token: 0x0400153C RID: 5436
	private static List<string> logLines = new List<string>();

	// Token: 0x0400153D RID: 5437
	private static long threshold;

	// Token: 0x0400153E RID: 5438
	private static long startTime;
}
