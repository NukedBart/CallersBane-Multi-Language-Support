using System;

// Token: 0x0200043A RID: 1082
public class FpsCounter
{
	// Token: 0x060017F1 RID: 6129 RVA: 0x00091908 File Offset: 0x0008FB08
	public void Update()
	{
		this._counter++;
		long num = TimeUtil.CurrentTimeMillis();
		if (num - this._fpsLastTime >= 1000L)
		{
			this._fps = this._counter;
			this._counter = 0;
			this._fpsLastTime = num;
			this._isUpdated = true;
		}
	}

	// Token: 0x060017F2 RID: 6130 RVA: 0x000112D4 File Offset: 0x0000F4D4
	public int Fps()
	{
		this._isUpdated = false;
		return this._fps;
	}

	// Token: 0x060017F3 RID: 6131 RVA: 0x000112E3 File Offset: 0x0000F4E3
	public bool HasUpdatedFps()
	{
		return this._isUpdated;
	}

	// Token: 0x04001506 RID: 5382
	private int _fps;

	// Token: 0x04001507 RID: 5383
	private int _counter;

	// Token: 0x04001508 RID: 5384
	private long _fpsLastTime;

	// Token: 0x04001509 RID: 5385
	private bool _isUpdated;
}
