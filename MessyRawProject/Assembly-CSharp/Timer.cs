using System;
using UnityEngine;

// Token: 0x0200045B RID: 1115
public class Timer
{
	// Token: 0x060018D6 RID: 6358 RVA: 0x0001209B File Offset: 0x0001029B
	public Timer(float ticksPerSecond)
	{
		if (ticksPerSecond <= 0f)
		{
			throw new ArgumentException("ticksPerSecond must be >= 0");
		}
		this._ticksPerSecond = ticksPerSecond;
		this._secondsPerTick = 1f / ticksPerSecond;
		this.restart();
	}

	// Token: 0x060018D7 RID: 6359 RVA: 0x000120D3 File Offset: 0x000102D3
	public void restart()
	{
		this._lastTime = Time.time;
		this._alpha = 0f;
		this._leftOver = 0f;
	}

	// Token: 0x060018D8 RID: 6360 RVA: 0x000934C0 File Offset: 0x000916C0
	public int tick()
	{
		float time = Time.time;
		this._leftOver += time - this._lastTime;
		int num = 0;
		while (this._leftOver >= this._secondsPerTick)
		{
			this._leftOver -= this._secondsPerTick;
			num++;
		}
		this._lastTime = time;
		this._alpha = this._leftOver / this._secondsPerTick;
		return num;
	}

	// Token: 0x060018D9 RID: 6361 RVA: 0x000120F6 File Offset: 0x000102F6
	public float alpha()
	{
		return this._alpha;
	}

	// Token: 0x060018DA RID: 6362 RVA: 0x000120FE File Offset: 0x000102FE
	public float ticksPerSecond()
	{
		return this._ticksPerSecond;
	}

	// Token: 0x04001551 RID: 5457
	private float _ticksPerSecond;

	// Token: 0x04001552 RID: 5458
	private float _secondsPerTick;

	// Token: 0x04001553 RID: 5459
	private float _lastTime;

	// Token: 0x04001554 RID: 5460
	private float _alpha;

	// Token: 0x04001555 RID: 5461
	private float _leftOver;
}
