using System;
using UnityEngine;

// Token: 0x02000414 RID: 1044
public class Clocks : MonoBehaviour
{
	// Token: 0x06001718 RID: 5912 RVA: 0x0008F2FC File Offset: 0x0008D4FC
	private void Update()
	{
		float deltaTime = Time.deltaTime;
		this.tweenClock.update(deltaTime);
		this.animClock.update(deltaTime);
		this.battleModeClock.update(deltaTime);
	}

	// Token: 0x04001484 RID: 5252
	public readonly TimeScaleClock tweenClock = new TimeScaleClock();

	// Token: 0x04001485 RID: 5253
	public readonly TimeScaleClock animClock = new TimeScaleClock();

	// Token: 0x04001486 RID: 5254
	public readonly TimeScaleClock battleModeClock = new TimeScaleClock();

	// Token: 0x04001487 RID: 5255
	public readonly iTween.ITimer defaultClock = new iTween.UnityTimer();
}
