using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Token: 0x02000458 RID: 1112
public static class TimeUtil
{
	// Token: 0x060018CA RID: 6346 RVA: 0x00011FEA File Offset: 0x000101EA
	public static float sin01ForSpeed(float speed)
	{
		return TimeUtil.sin01ForSpeedAndTime(speed, Time.time);
	}

	// Token: 0x060018CB RID: 6347 RVA: 0x00011FF7 File Offset: 0x000101F7
	public static float sin01ForSpeedAndTime(float speed, float atTime)
	{
		return 0.5f * (Mathf.Sin(speed * atTime) + 1f);
	}

	// Token: 0x060018CC RID: 6348 RVA: 0x000933BC File Offset: 0x000915BC
	public static long Ticks()
	{
		return DateTime.Now.Ticks;
	}

	// Token: 0x060018CD RID: 6349 RVA: 0x0001200D File Offset: 0x0001020D
	public static long CurrentTimeMillis()
	{
		return TimeUtil.Ticks() / 10000L;
	}

	// Token: 0x060018CE RID: 6350 RVA: 0x000933D8 File Offset: 0x000915D8
	public static string SystemTime()
	{
		return DateTime.Now.ToString();
	}

	// Token: 0x060018CF RID: 6351 RVA: 0x0001201B File Offset: 0x0001021B
	public static int SecondsToMilliseconds(float seconds)
	{
		return (int)(seconds * 1000f);
	}

	// Token: 0x02000459 RID: 1113
	public class Timer
	{
		// Token: 0x060018D1 RID: 6353 RVA: 0x00012043 File Offset: 0x00010243
		public void stamp(string name)
		{
			this._timeStamps.Add(new TimeUtil.Timer.TimeStamp(TimeUtil.CurrentTimeMillis(), name));
		}

		// Token: 0x060018D2 RID: 6354 RVA: 0x0001205B File Offset: 0x0001025B
		public void stamp()
		{
			this._timeStamps.Add(new TimeUtil.Timer.TimeStamp(TimeUtil.CurrentTimeMillis(), null));
		}

		// Token: 0x060018D3 RID: 6355 RVA: 0x00012073 File Offset: 0x00010273
		public void clear()
		{
			this._startTime = TimeUtil.CurrentTimeMillis();
			this._timeStamps.Clear();
		}

		// Token: 0x060018D4 RID: 6356 RVA: 0x000933F4 File Offset: 0x000915F4
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			long num = this._startTime;
			foreach (TimeUtil.Timer.TimeStamp timeStamp in this._timeStamps)
			{
				long num2 = timeStamp.t - num;
				long num3 = timeStamp.t - this._startTime;
				stringBuilder.AppendLine(string.Concat(new object[]
				{
					num3,
					", ",
					num2,
					" -- ",
					timeStamp.name
				}));
				num = timeStamp.t;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0400154D RID: 5453
		private long _startTime = TimeUtil.CurrentTimeMillis();

		// Token: 0x0400154E RID: 5454
		private List<TimeUtil.Timer.TimeStamp> _timeStamps = new List<TimeUtil.Timer.TimeStamp>();

		// Token: 0x0200045A RID: 1114
		private struct TimeStamp
		{
			// Token: 0x060018D5 RID: 6357 RVA: 0x0001208B File Offset: 0x0001028B
			public TimeStamp(long t, string name)
			{
				this.t = t;
				this.name = name;
			}

			// Token: 0x0400154F RID: 5455
			public long t;

			// Token: 0x04001550 RID: 5456
			public string name;
		}
	}
}
