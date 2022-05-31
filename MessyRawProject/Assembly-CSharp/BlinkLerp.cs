using System;
using UnityEngine;

// Token: 0x0200005B RID: 91
public class BlinkLerp : MonoBehaviour
{
	// Token: 0x060003EE RID: 1006 RVA: 0x0002FF0C File Offset: 0x0002E10C
	private void Start()
	{
		this.startTime = Time.time;
		this.hasLerp = base.gameObject.renderer.material.HasProperty("_Lerp");
		this.startLerp = ((!this.hasLerp) ? 1f : base.gameObject.renderer.material.GetFloat("_Lerp"));
	}

	// Token: 0x060003EF RID: 1007 RVA: 0x0002FF7C File Offset: 0x0002E17C
	private void Update()
	{
		float lerpValue = BlinkLerp.getLerpValue(Time.time - this.startTime);
		this.setLerp(0.5f * lerpValue);
	}

	// Token: 0x060003F0 RID: 1008 RVA: 0x00004956 File Offset: 0x00002B56
	private void setLerp(float lerp)
	{
		if (!this.hasLerp)
		{
			return;
		}
		base.gameObject.renderer.material.SetFloat("_Lerp", lerp);
	}

	// Token: 0x060003F1 RID: 1009 RVA: 0x0000497F File Offset: 0x00002B7F
	private void OnDestroy()
	{
		this.setLerp(this.startLerp);
	}

	// Token: 0x060003F2 RID: 1010 RVA: 0x0002FFA8 File Offset: 0x0002E1A8
	public static float getLerpValue(float afterTime)
	{
		return 0.5f + 0.5f * Mathf.Cos(5.0265484f * afterTime);
	}

	// Token: 0x04000284 RID: 644
	private float startTime;

	// Token: 0x04000285 RID: 645
	private float startLerp;

	// Token: 0x04000286 RID: 646
	private bool hasLerp;
}
