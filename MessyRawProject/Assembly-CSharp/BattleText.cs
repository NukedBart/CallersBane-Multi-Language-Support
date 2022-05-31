using System;
using Irrelevant.Assets;
using UnityEngine;

// Token: 0x02000054 RID: 84
public class BattleText : iEffect
{
	// Token: 0x060003BD RID: 957 RVA: 0x0002E7AC File Offset: 0x0002C9AC
	public void init(string fn, Vector3 p, float startInSeconds, BattleMode callBackTarget)
	{
		this.callBackTarget = callBackTarget;
		this.gameObject = new GameObject();
		this.gameObject.AddComponent<MeshRenderer>();
		this.effect = this.gameObject.AddComponent<EffectPlayer>();
		this.effect.init("battletexts/" + fn, 1, this, 50, new Vector3(2.5f, 2.5f, 2.5f), false, string.Empty, 0);
		this.effect.gameObject.renderer.material.renderQueue = 96000;
		this.effect.transform.eulerAngles = new Vector3(51f, 270f, 0f);
		this.effect.transform.position = p;
		this.effect.layer = BattleMode.LAYER_NOLIGHT;
		this.effect.getAnimPlayer().getFrameAnimation().setFps(32f);
		this.effect.startInSeconds(startInSeconds);
	}

	// Token: 0x060003BE RID: 958 RVA: 0x00004876 File Offset: 0x00002A76
	public void effectAnimDone(EffectPlayer effect, bool loop)
	{
		this.callBackTarget.effectDone();
		Object.Destroy(effect.gameObject);
	}

	// Token: 0x060003BF RID: 959 RVA: 0x000028DF File Offset: 0x00000ADF
	public void locator(EffectPlayer p, AnimLocator loc)
	{
	}

	// Token: 0x04000262 RID: 610
	private BattleMode callBackTarget;

	// Token: 0x04000263 RID: 611
	public GameObject gameObject;

	// Token: 0x04000264 RID: 612
	private EffectPlayer effect;
}
