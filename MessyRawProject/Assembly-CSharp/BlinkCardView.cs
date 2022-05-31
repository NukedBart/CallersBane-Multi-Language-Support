using System;
using UnityEngine;

// Token: 0x0200005A RID: 90
public class BlinkCardView : MonoBehaviour
{
	// Token: 0x060003E9 RID: 1001 RVA: 0x0002FDB0 File Offset: 0x0002DFB0
	private void Start()
	{
		this.startTime = Time.time;
		this.startColor = base.gameObject.renderer.material.color;
		this.cardView = base.gameObject.GetComponent<CardView>();
		if (this.cardView == null)
		{
			this._renderer = base.gameObject.renderer;
		}
	}

	// Token: 0x060003EA RID: 1002 RVA: 0x0002FE18 File Offset: 0x0002E018
	private void Update()
	{
		float lerpValue = Blink.getLerpValue(Time.time - this.startTime);
		Color white = Color.white;
		white.a = 1f - 0.25f * BlinkCardView.getLerpValue(Time.time - this.startTime);
		if (this.cardView != null)
		{
			this.cardView.setColor(white);
		}
		else
		{
			this._renderer.material.color = white;
		}
	}

	// Token: 0x060003EB RID: 1003 RVA: 0x0002FE94 File Offset: 0x0002E094
	private void OnDestroy()
	{
		if (this.cardView != null)
		{
			this.cardView.setColor(this.startColor, 0.1f);
		}
		else
		{
			this._renderer.material.color = this.startColor;
		}
	}

	// Token: 0x060003EC RID: 1004 RVA: 0x0002FEE4 File Offset: 0x0002E0E4
	public static float getLerpValue(float afterTime)
	{
		return 0.5f - 0.5f * Mathf.Cos(5.0265484f * afterTime);
	}

	// Token: 0x04000280 RID: 640
	private float startTime;

	// Token: 0x04000281 RID: 641
	private Color startColor;

	// Token: 0x04000282 RID: 642
	private CardView cardView;

	// Token: 0x04000283 RID: 643
	private Renderer _renderer;
}
