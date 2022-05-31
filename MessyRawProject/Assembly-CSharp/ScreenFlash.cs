using System;
using UnityEngine;

// Token: 0x0200006C RID: 108
public class ScreenFlash : MonoBehaviour
{
	// Token: 0x0600043E RID: 1086 RVA: 0x00004C13 File Offset: 0x00002E13
	public ScreenFlash init(float inSeconds, float lifeTime)
	{
		return this.init(inSeconds, lifeTime, 1f);
	}

	// Token: 0x0600043F RID: 1087 RVA: 0x000311C0 File Offset: 0x0002F3C0
	public ScreenFlash init(float inSeconds, float lifeTime, float maxAlpha)
	{
		this.startTime = Time.time + inSeconds;
		this.lifeTime = lifeTime;
		this.maxAlpha = maxAlpha;
		this.img = new Texture2D(2, 2);
		this.img.SetPixels(new Color[]
		{
			Color.white,
			Color.white,
			Color.white,
			Color.white
		});
		this.img.Apply();
		float num = 10f;
		float num2 = 0.2f * num * Mathf.Tan(Camera.main.fieldOfView * 0.5f);
		float num3 = num2 * Camera.main.aspect;
		Vector3 localScale;
		localScale..ctor(num3, 1f, num2);
		this.g = PrimitiveFactory.createPlane(false);
		this.g.transform.parent = Camera.main.transform;
		this.g.transform.localPosition = Vector3.zero;
		this.g.transform.localEulerAngles = new Vector3(270f, 0f, 0f);
		this.g.transform.localScale = localScale;
		this.g.transform.localPosition = new Vector3(0f, 0f, num);
		this.g.renderer.material = new Material(ResourceManager.LoadShader("Transparent/Diffuse"));
		this.g.renderer.material.renderQueue = 98300;
		this.g.renderer.material.color = ColorUtil.GetWithAlpha(this.color, 0f);
		this.g.renderer.material.mainTexture = this.img;
		return this;
	}

	// Token: 0x06000440 RID: 1088 RVA: 0x00004C22 File Offset: 0x00002E22
	public ScreenFlash setColor(Color color)
	{
		this.color = color;
		return this;
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x00004C2C File Offset: 0x00002E2C
	private void OnDestroy()
	{
		if (this.g != null)
		{
			Object.Destroy(this.g);
		}
	}

	// Token: 0x06000442 RID: 1090 RVA: 0x000313A0 File Offset: 0x0002F5A0
	private void Update()
	{
		if (this.g == null)
		{
			return;
		}
		float num = Time.time - this.startTime;
		if (num < 0f)
		{
			return;
		}
		float num2 = Mathf.Min(1f, num / this.lifeTime);
		float newAlpha = (num2 <= 0f || num2 >= 1f) ? 0f : this.maxAlpha;
		if (num2 >= 1f)
		{
			Object.Destroy(this);
		}
		else if (num2 > 0f)
		{
			this.g.renderer.material.color = ColorUtil.GetWithAlpha(this.color, newAlpha);
		}
	}

	// Token: 0x040002C2 RID: 706
	private float lifeTime;

	// Token: 0x040002C3 RID: 707
	private float maxAlpha;

	// Token: 0x040002C4 RID: 708
	private Color color = Color.white;

	// Token: 0x040002C5 RID: 709
	private GameObject g;

	// Token: 0x040002C6 RID: 710
	private Texture2D img;

	// Token: 0x040002C7 RID: 711
	private float startTime;
}
