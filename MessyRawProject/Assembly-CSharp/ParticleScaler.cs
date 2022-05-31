using System;
using UnityEngine;

// Token: 0x0200037E RID: 894
[ExecuteInEditMode]
public class ParticleScaler : MonoBehaviour
{
	// Token: 0x060013F5 RID: 5109 RVA: 0x0000EC7F File Offset: 0x0000CE7F
	private void Start()
	{
		this.prevScale = this.particleScale;
	}

	// Token: 0x060013F6 RID: 5110 RVA: 0x000028DF File Offset: 0x00000ADF
	private void Update()
	{
	}

	// Token: 0x060013F7 RID: 5111 RVA: 0x000028DF File Offset: 0x00000ADF
	private static void ScaleShurikenSystems(GameObject g, float scaleFactor)
	{
	}

	// Token: 0x060013F8 RID: 5112 RVA: 0x00079204 File Offset: 0x00077404
	private static void ScaleTrailRenderers(GameObject g, float scaleFactor)
	{
		TrailRenderer[] componentsInChildren = g.GetComponentsInChildren<TrailRenderer>();
		foreach (TrailRenderer trailRenderer in componentsInChildren)
		{
			trailRenderer.startWidth *= scaleFactor;
			trailRenderer.endWidth *= scaleFactor;
		}
	}

	// Token: 0x060013F9 RID: 5113 RVA: 0x0000EC8D File Offset: 0x0000CE8D
	public static void Scale(GameObject g, float scaleFactor)
	{
		ParticleScaler.Scale(g, scaleFactor, true);
	}

	// Token: 0x060013FA RID: 5114 RVA: 0x00079250 File Offset: 0x00077450
	public static void Scale(GameObject g, float scaleFactor, bool alsoScaleGameobject)
	{
		ParticleScaler.ScaleShurikenSystems(g, scaleFactor);
		ParticleScaler.ScaleTrailRenderers(g, scaleFactor);
		if (alsoScaleGameobject)
		{
			Vector3 localScale = g.transform.localScale;
			g.transform.localScale = localScale * scaleFactor;
		}
	}

	// Token: 0x04001127 RID: 4391
	public float particleScale = 1f;

	// Token: 0x04001128 RID: 4392
	public bool alsoScaleGameobject = true;

	// Token: 0x04001129 RID: 4393
	private float prevScale;
}
