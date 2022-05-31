using System;

// Token: 0x02000454 RID: 1108
public class StopWatch
{
	// Token: 0x060018A8 RID: 6312 RVA: 0x00011ED7 File Offset: 0x000100D7
	public StopWatch()
	{
		this.reset();
	}

	// Token: 0x060018A9 RID: 6313 RVA: 0x00011EE5 File Offset: 0x000100E5
	public StopWatch start()
	{
		this._st = DateTime.Now;
		this._z = true;
		return this;
	}

	// Token: 0x060018AA RID: 6314 RVA: 0x00092CB0 File Offset: 0x00090EB0
	public double stop()
	{
		if (!this._z)
		{
			return 0.0;
		}
		DateTime now = DateTime.Now;
		this._last = 0.001 * (now - this._st).TotalMilliseconds;
		this._tt += this._last;
		this._z = false;
		if (this._last > this._max)
		{
			this._max = this._last;
		}
		this._count++;
		return this._tt;
	}

	// Token: 0x060018AB RID: 6315 RVA: 0x00011EFA File Offset: 0x000100FA
	public double getLast()
	{
		return this._last;
	}

	// Token: 0x060018AC RID: 6316 RVA: 0x00011F02 File Offset: 0x00010102
	public double getTotal()
	{
		return this._tt;
	}

	// Token: 0x060018AD RID: 6317 RVA: 0x00011F0A File Offset: 0x0001010A
	public double getMax()
	{
		return this._max;
	}

	// Token: 0x060018AE RID: 6318 RVA: 0x00011F12 File Offset: 0x00010112
	public int getCount()
	{
		return this._count;
	}

	// Token: 0x060018AF RID: 6319 RVA: 0x00011F1A File Offset: 0x0001011A
	public void reset()
	{
		this._z = false;
		this._tt = 0.0;
		this._max = 0.0;
	}

	// Token: 0x060018B0 RID: 6320 RVA: 0x00011F41 File Offset: 0x00010141
	public void printEvery(int n)
	{
		this.printEvery(n, string.Empty);
	}

	// Token: 0x060018B1 RID: 6321 RVA: 0x00092D48 File Offset: 0x00090F48
	public void printEvery(int n, string prepend)
	{
		if (++this._printCounter >= n)
		{
			Log.warning(string.Concat(new object[]
			{
				prepend,
				". Avg/Total/count - last: ",
				this._tt / (double)this._count,
				",",
				this._tt,
				",",
				this._count,
				" - ",
				this._last
			}));
			this._printCounter = 0;
		}
	}

	// Token: 0x04001545 RID: 5445
	private int _count;

	// Token: 0x04001546 RID: 5446
	private int _printCounter;

	// Token: 0x04001547 RID: 5447
	private DateTime _st;

	// Token: 0x04001548 RID: 5448
	private bool _z;

	// Token: 0x04001549 RID: 5449
	private double _tt;

	// Token: 0x0400154A RID: 5450
	private double _last;

	// Token: 0x0400154B RID: 5451
	private double _max;
}
