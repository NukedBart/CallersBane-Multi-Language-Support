using System;
using UnityEngine;

// Token: 0x0200044C RID: 1100
public static class Loggers
{
	// Token: 0x06001863 RID: 6243 RVA: 0x000119BA File Offset: 0x0000FBBA
	public static Logger UnityLogger()
	{
		return new Logger(delegate(LogLine line)
		{
			string defaultFormatting = line.getDefaultFormatting();
			switch (line.level)
			{
			case LogLevel.WARNING:
				Debug.LogWarning(defaultFormatting);
				break;
			case LogLevel.ERROR:
				Debug.LogError(defaultFormatting);
				break;
			case LogLevel.CRITICAL:
				Debug.LogError("CRITICAL: " + defaultFormatting);
				break;
			default:
				Debug.Log(defaultFormatting);
				break;
			}
		}, LogLevel.INFO);
	}

	// Token: 0x06001864 RID: 6244 RVA: 0x000119DF File Offset: 0x0000FBDF
	public static Logger NullLogger()
	{
		return new Logger(delegate(LogLine l)
		{
		}, LogLevel.CRITICAL);
	}
}
