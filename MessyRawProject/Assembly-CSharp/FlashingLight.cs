using System;
using UnityEngine;

// Token: 0x02000068 RID: 104
public class FlashingLight : TimedRemoveGameObject
{
	// Token: 0x0600042D RID: 1069 RVA: 0x00030988 File Offset: 0x0002EB88
	public void init(Color color, float seconds)
	{
		base.init(seconds);
		this._light = base.gameObject.AddComponent<Light>();
		this._light.color = color;
		this._light.cullingMask = ~(1 << BattleMode.LAYER_NOLIGHT);
		this._light.intensity = this.baseIntensity;
		this._light.type = 2;
		this._light.range = this.baseRange;
		this._light.enabled = false;
	}

	// Token: 0x0600042E RID: 1070 RVA: 0x00030A0C File Offset: 0x0002EC0C
	protected override void Update()
	{
		base.Update();
		float x = this.time.time();
		if (!Mth.transformToRange(ref x, 0.075f, 1f))
		{
			return;
		}
		this._light.enabled = true;
		float num = Mth.linearUD(x, 0.05f);
		float num2 = num * (1f + 0.1f * RandomUtil.fakeGaussian());
		this._light.intensity = num2 * this.baseIntensity;
		this._light.range = num2 * this.baseRange;
	}

	// Token: 0x0600042F RID: 1071 RVA: 0x00030A94 File Offset: 0x0002EC94
	public static FlashingLight Create(Vector3 p, Color color, float seconds)
	{
		FlashingLight flashingLight = new GameObject("FlashingLight_" + ++FlashingLight._runningIndex)
		{
			transform = 
			{
				position = p + new Vector3(0f, 0.5f, 0f)
			}
		}.AddComponent<FlashingLight>();
		flashingLight.init(color, seconds);
		return flashingLight;
	}

	// Token: 0x040002B3 RID: 691
	private float baseIntensity = 3f;

	// Token: 0x040002B4 RID: 692
	private float baseRange = 4f;

	// Token: 0x040002B5 RID: 693
	private Light _light;

	// Token: 0x040002B6 RID: 694
	private static int _runningIndex = 1;
}
