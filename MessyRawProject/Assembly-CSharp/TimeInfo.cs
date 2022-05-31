using System;
using UnityEngine;

// Token: 0x0200006F RID: 111
public class TimeInfo
{
	// Token: 0x06000450 RID: 1104 RVA: 0x00004CEB File Offset: 0x00002EEB
	public TimeInfo(float maxTime)
	{
		this._maxTime = maxTime;
		this.restart();
	}

	// Token: 0x06000451 RID: 1105 RVA: 0x00004D00 File Offset: 0x00002F00
	public void restart()
	{
		this._startTime = Time.time;
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x00004D0D File Offset: 0x00002F0D
	public float time()
	{
		return Time.time - this._startTime;
	}

	// Token: 0x06000453 RID: 1107 RVA: 0x00004D1B File Offset: 0x00002F1B
	public float maxTime()
	{
		return this._maxTime;
	}

	// Token: 0x06000454 RID: 1108 RVA: 0x00004D23 File Offset: 0x00002F23
	public float life()
	{
		return this.time() / this._maxTime;
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x00004D32 File Offset: 0x00002F32
	public float left()
	{
		return 1f - this.life();
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x00004D40 File Offset: 0x00002F40
	public bool isDone()
	{
		return this.left() <= 0f;
	}

	// Token: 0x040002D9 RID: 729
	private float _startTime;

	// Token: 0x040002DA RID: 730
	private float _maxTime;
}
