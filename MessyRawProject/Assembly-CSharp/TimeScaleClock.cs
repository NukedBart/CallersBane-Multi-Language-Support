using System;

// Token: 0x02000415 RID: 1045
public class TimeScaleClock : iTween.ITimer
{
	// Token: 0x0600171A RID: 5914 RVA: 0x000109E1 File Offset: 0x0000EBE1
	public TimeScaleClock setSpeed(float s)
	{
		this.scale = s;
		return this;
	}

	// Token: 0x0600171B RID: 5915 RVA: 0x000109EB File Offset: 0x0000EBEB
	public float getSpeed()
	{
		return this.scale;
	}

	// Token: 0x0600171C RID: 5916 RVA: 0x000109F3 File Offset: 0x0000EBF3
	public float getTime()
	{
		return this.time;
	}

	// Token: 0x0600171D RID: 5917 RVA: 0x000109FB File Offset: 0x0000EBFB
	public float getDeltaFrameTime()
	{
		return this.deltaTime;
	}

	// Token: 0x0600171E RID: 5918 RVA: 0x00010A03 File Offset: 0x0000EC03
	public void update(float deltaTime)
	{
		this.deltaTime = deltaTime * this.scale;
		this.time += this.deltaTime;
	}

	// Token: 0x04001488 RID: 5256
	private float time;

	// Token: 0x04001489 RID: 5257
	private float scale = 1f;

	// Token: 0x0400148A RID: 5258
	private float deltaTime;
}
