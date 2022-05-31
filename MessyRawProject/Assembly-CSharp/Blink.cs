using System;
using UnityEngine;

// Token: 0x02000059 RID: 89
public class Blink : MonoBehaviour
{
	// Token: 0x060003E4 RID: 996 RVA: 0x00004911 File Offset: 0x00002B11
	private void Start()
	{
		this.startTime = Time.time;
		this.startColor = base.gameObject.renderer.material.color;
	}

	// Token: 0x060003E5 RID: 997 RVA: 0x0002FD20 File Offset: 0x0002DF20
	private void Update()
	{
		float lerpValue = Blink.getLerpValue(Time.time - this.startTime);
		Color color = base.gameObject.renderer.material.color;
		color.r = (color.g = (color.b = lerpValue));
		base.gameObject.renderer.material.color = color;
	}

	// Token: 0x060003E6 RID: 998 RVA: 0x00004939 File Offset: 0x00002B39
	private void OnDestroy()
	{
		base.gameObject.renderer.material.color = this.startColor;
	}

	// Token: 0x060003E7 RID: 999 RVA: 0x0002FD88 File Offset: 0x0002DF88
	public static float getLerpValue(float afterTime)
	{
		return 0.75f + 0.25f * Mathf.Cos(5.0265484f * afterTime);
	}

	// Token: 0x0400027E RID: 638
	private float startTime;

	// Token: 0x0400027F RID: 639
	private Color startColor;
}
