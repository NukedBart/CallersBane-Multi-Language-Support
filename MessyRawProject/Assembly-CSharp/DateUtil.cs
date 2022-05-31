using System;

// Token: 0x02000428 RID: 1064
internal static class DateUtil
{
	// Token: 0x1700012A RID: 298
	// (get) Token: 0x0600179D RID: 6045 RVA: 0x000913D0 File Offset: 0x0008F5D0
	public static DateUtil.DateQuery utcToday
	{
		get
		{
			return new DateUtil.DateQuery(DateTime.UtcNow.Date);
		}
	}

	// Token: 0x1700012B RID: 299
	// (get) Token: 0x0600179E RID: 6046 RVA: 0x00010F9B File Offset: 0x0000F19B
	public static DateUtil.DateQuery today
	{
		get
		{
			return new DateUtil.DateQuery(DateTime.Today);
		}
	}

	// Token: 0x02000429 RID: 1065
	public class DateQuery
	{
		// Token: 0x0600179F RID: 6047 RVA: 0x00010FA7 File Offset: 0x0000F1A7
		public DateQuery(DateTime dt)
		{
			this.dt = dt.Date;
		}

		// Token: 0x060017A0 RID: 6048 RVA: 0x00010FBC File Offset: 0x0000F1BC
		public bool isMonthAndDay(int month, int day)
		{
			return this.isMonth(month) && this.isDay(day);
		}

		// Token: 0x060017A1 RID: 6049 RVA: 0x00010FD4 File Offset: 0x0000F1D4
		public bool isDay(int day)
		{
			return day == this.dt.Day;
		}

		// Token: 0x060017A2 RID: 6050 RVA: 0x00010FE4 File Offset: 0x0000F1E4
		public bool isMonth(int month)
		{
			return month == this.dt.Month;
		}

		// Token: 0x040014E8 RID: 5352
		private DateTime dt;
	}
}
