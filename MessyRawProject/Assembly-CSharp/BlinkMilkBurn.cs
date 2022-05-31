using System;
using UnityEngine;

// Token: 0x0200005C RID: 92
public class BlinkMilkBurn : MonoBehaviour
{
	// Token: 0x060003F4 RID: 1012 RVA: 0x0000498D File Offset: 0x00002B8D
	private void Start()
	{
		this.startTime = Time.time;
	}

	// Token: 0x060003F5 RID: 1013 RVA: 0x0000499A File Offset: 0x00002B9A
	private void Update()
	{
		this._run(0.25f * BlinkMilkBurn.getLerpValue(Time.time - this.startTime));
	}

	// Token: 0x060003F6 RID: 1014 RVA: 0x000049B9 File Offset: 0x00002BB9
	private void _run(float v)
	{
		this.lerp = v;
		UnityUtil.traverse(base.gameObject, new Action<GameObject>(this._callback));
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x0002FFD0 File Offset: 0x0002E1D0
	private void _callback(GameObject g)
	{
		Renderer renderer = g.renderer;
		if (renderer != null && renderer.material.shader.name == Shaders.fnMilkBurn)
		{
			renderer.material.SetFloat("_Lerp", this.lerp);
		}
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x000049D9 File Offset: 0x00002BD9
	private void OnDestroy()
	{
		this._run(0f);
	}

	// Token: 0x060003F9 RID: 1017 RVA: 0x00030028 File Offset: 0x0002E228
	public static float getLerpValue(float afterTime)
	{
		return 0.5f - 0.5f * Mathf.Cos(5.340708f * afterTime);
	}

	// Token: 0x04000287 RID: 647
	private float startTime;

	// Token: 0x04000288 RID: 648
	private float lerp;
}
