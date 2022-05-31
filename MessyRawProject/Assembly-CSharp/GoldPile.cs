using System;
using System.Collections;
using System.Collections.Generic;
using Gui;
using Irrelevant.Assets;
using UnityEngine;

// Token: 0x020001FA RID: 506
public class GoldPile : MonoBehaviour, iEffect
{
	// Token: 0x170000C7 RID: 199
	// (get) Token: 0x06000FD7 RID: 4055 RVA: 0x0000C940 File Offset: 0x0000AB40
	// (set) Token: 0x06000FD6 RID: 4054 RVA: 0x0000C937 File Offset: 0x0000AB37
	private protected int layer { protected get; private set; }

	// Token: 0x06000FD8 RID: 4056 RVA: 0x000028DF File Offset: 0x00000ADF
	private void Start()
	{
	}

	// Token: 0x06000FD9 RID: 4057 RVA: 0x0000C948 File Offset: 0x0000AB48
	public void setOffsetScale(Rect rect)
	{
		this.offsetScale = rect;
	}

	// Token: 0x06000FDA RID: 4058 RVA: 0x00069BD8 File Offset: 0x00067DD8
	protected Rect transformRect(Rect rect)
	{
		Rect translated = GeomUtil.getTranslated(rect, this.offsetScale.x, this.offsetScale.y);
		return GeomUtil.scaleCentered(translated, this.offsetScale.width, this.offsetScale.height);
	}

	// Token: 0x06000FDB RID: 4059 RVA: 0x0000C951 File Offset: 0x0000AB51
	protected float scaleY(float y)
	{
		return y * this.offsetScale.height;
	}

	// Token: 0x06000FDC RID: 4060 RVA: 0x00069C20 File Offset: 0x00067E20
	protected void init(Gui3D gui, int layer, bool playAnimation)
	{
		this.layer = layer;
		this._camera = gui.getCamera();
		this.playAnimation = playAnimation;
		this.imCoinsPlane = PrimitiveFactory.createTexturedPlane("Limited/goldpile", true);
		this.imShadowPlane = PrimitiveFactory.createTexturedPlane("Limited/shadow", true);
		GameObject gameObject = this.imCoinsPlane;
		this.imShadowPlane.layer = layer;
		gameObject.layer = layer;
		gui.pushTransform();
		float num = (float)Screen.height * 0.2f;
		float widthFromHeight = GeomUtil.getWidthFromHeight(num, 809f, 207f);
		Rect rect;
		rect..ctor((float)Screen.width * this.unitX - widthFromHeight / 2f, (float)Screen.height * 0.72f - num / 2f, widthFromHeight, num);
		gui.DrawObject(this.transformRect(rect), this.imShadowPlane);
		float num2 = (float)Screen.height * 0.23f;
		float widthFromHeight2 = GeomUtil.getWidthFromHeight(num2, 635f, 253f);
		Rect rect2 = this.coinsRect = new Rect((float)Screen.width * this.unitX - widthFromHeight2 / 2f, (float)Screen.height * 0.67f - num2 / 2f, widthFromHeight2, num2);
		gui.DrawObject(this.transformRect(rect2), this.imCoinsPlane);
		gui.popTransform();
		this.setAlpha(0f);
		this.setRenderQueue(this.renderQueue);
	}

	// Token: 0x06000FDD RID: 4061 RVA: 0x00069D80 File Offset: 0x00067F80
	public void setRenderQueue(int renderQueue)
	{
		this.renderQueue = renderQueue;
		if (this.imCoinsPlane != null)
		{
			this.imCoinsPlane.renderer.material.renderQueue = renderQueue;
			this.imShadowPlane.renderer.material.renderQueue = renderQueue;
		}
	}

	// Token: 0x06000FDE RID: 4062 RVA: 0x00069DD4 File Offset: 0x00067FD4
	public virtual void OnDestroy()
	{
		Object.Destroy(this.imCoinsPlane);
		Object.Destroy(this.imShadowPlane);
		foreach (EffectPlayer effectPlayer in this.effects)
		{
			if (effectPlayer.gameObject != null)
			{
				Object.Destroy(effectPlayer.gameObject);
			}
		}
	}

	// Token: 0x06000FDF RID: 4063 RVA: 0x00069E5C File Offset: 0x0006805C
	public virtual void run()
	{
		if (this.playAnimation)
		{
			this.setAlpha(0f);
			base.StartCoroutine(this._runFadeIn());
		}
		else
		{
			this.setAlpha(1f);
		}
		base.StartCoroutine(this._runGlimmer());
	}

	// Token: 0x06000FE0 RID: 4064 RVA: 0x00069EAC File Offset: 0x000680AC
	private IEnumerator _runFadeIn()
	{
		float duration = 0.5f;
		float t = 0f;
		float timeStarted = Time.time;
		while (t < 1f)
		{
			t = Mathf.Min((Time.time - timeStarted) / duration, 1f);
			this.setAlpha(t);
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000FE1 RID: 4065 RVA: 0x00069EC8 File Offset: 0x000680C8
	private void setAlpha(float alpha)
	{
		Color color;
		color..ctor(1f, 1f, 1f, alpha);
		this.imCoinsPlane.renderer.material.color = color;
		this.imShadowPlane.renderer.material.color = color;
	}

	// Token: 0x06000FE2 RID: 4066 RVA: 0x00069F18 File Offset: 0x00068118
	private IEnumerator _runGlimmer()
	{
		for (;;)
		{
			float waitDuration = Random.Range(0f, 3f);
			yield return new WaitForSeconds(waitDuration);
			this.PlayGlimmer();
		}
		yield break;
	}

	// Token: 0x06000FE3 RID: 4067 RVA: 0x00069F34 File Offset: 0x00068134
	private void PlayGlimmer()
	{
		float num = (1.5f + Random.Range(0f, 2f)) * (float)Screen.height / 30f;
		GameObject gameObject = new GameObject();
		gameObject.AddComponent<MeshRenderer>();
		gameObject.name = "Glimmer_";
		EffectPlayer effectPlayer = gameObject.AddComponent<EffectPlayer>();
		effectPlayer.setMaterialToUse(new Material(ResourceManager.LoadShader("Scrolls/StoreEffect/Unlit/Transparent")));
		effectPlayer.init("Glimmer", 1, this, this.renderQueue + 1, new Vector3(0.409f * num, 0.4264f * num, 0.5f), false, string.Empty, 0);
		effectPlayer.getAnimPlayer().waitForUpdate();
		effectPlayer.layer = this.layer;
		this.effects.Add(effectPlayer);
		Rect rect = this.transformRect(GeomUtil.scaleCentered(this.coinsRect, 0.75f, 0.75f));
		Vector3 vector = GeomUtil.v2tov3(GUIUtil.getScreenMousePos(GeomUtil.getRandomPoint(rect)), 500f);
		gameObject.transform.position = this._camera.ScreenToWorldPoint(vector);
	}

	// Token: 0x06000FE4 RID: 4068 RVA: 0x0000C960 File Offset: 0x0000AB60
	public void effectAnimDone(EffectPlayer effect, bool loop)
	{
		this.effects.Remove(effect);
		DefaultIEffectCallback.instance().effectAnimDone(effect, loop);
	}

	// Token: 0x06000FE5 RID: 4069 RVA: 0x000028DF File Offset: 0x00000ADF
	public void locator(EffectPlayer effect, AnimLocator loc)
	{
	}

	// Token: 0x04000C50 RID: 3152
	private GameObject imCoinsPlane;

	// Token: 0x04000C51 RID: 3153
	private GameObject imShadowPlane;

	// Token: 0x04000C52 RID: 3154
	protected bool playAnimation;

	// Token: 0x04000C53 RID: 3155
	private Camera _camera;

	// Token: 0x04000C54 RID: 3156
	protected float unitX = 0.5f;

	// Token: 0x04000C55 RID: 3157
	public List<EffectPlayer> effects = new List<EffectPlayer>();

	// Token: 0x04000C56 RID: 3158
	protected Rect offsetScale = new Rect(0f, 0f, 1f, 1f);

	// Token: 0x04000C57 RID: 3159
	private Rect coinsRect;

	// Token: 0x04000C58 RID: 3160
	private int renderQueue = 96500;
}
