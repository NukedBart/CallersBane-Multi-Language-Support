using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000121 RID: 289
public class CardView2 : AbstractCardView
{
	// Token: 0x06000963 RID: 2403 RVA: 0x0004A3A8 File Offset: 0x000485A8
	public void init(Card card)
	{
		if (!CardView2._inited)
		{
			CardView2.initStatics();
		}
		this.card = card;
		Material material;
		if (!CardView2._enabledCache.TryGetValue(card.typeId, ref material))
		{
			material = CardView2.createMaterialFor(card);
			CardView2._enabledCache[card.typeId] = material;
			Material material2 = CardView2.createMaterialFor(card);
			material2.color = AbstractCardView.DisabledColor;
			CardView2._disabledCache[card.typeId] = material2;
		}
		base.renderer.material = CardView2._enabledScroll;
		this.image = PrimitiveFactory.createPlane(false);
		UnityUtil.addChild(base.gameObject, this.image);
		Vector3 localPosition = this.image.transform.localPosition;
		localPosition.x = -0.12f;
		localPosition.y = -0.001f;
		localPosition.z = 0.4f;
		this.image.transform.localPosition = localPosition;
		Vector3 localScale = this.image.transform.localScale;
		localScale.x = 0.8f;
		localScale.z = 0.55f;
		this.image.transform.localScale = localScale;
		this.image.renderer.sharedMaterial = material;
		UnityUtil.traverse(base.gameObject, delegate(GameObject g)
		{
			g.renderer.castShadows = false;
			g.renderer.receiveShadows = false;
		});
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x0004A504 File Offset: 0x00048704
	private static Material createMaterialFor(Card card)
	{
		Material material = CardView2.material();
		material.mainTexture = App.AssetLoader.LoadCardImage(card.getCardImage());
		return material;
	}

	// Token: 0x06000965 RID: 2405 RVA: 0x0004A530 File Offset: 0x00048730
	private static Material materialWithTexture(string textureFilename)
	{
		Material material = CardView2.material();
		material.mainTexture = ResourceManager.LoadTexture(textureFilename);
		return material;
	}

	// Token: 0x06000966 RID: 2406 RVA: 0x000080AD File Offset: 0x000062AD
	public static Material material()
	{
		return new Material(ResourceManager.LoadShader("Scrolls/Unlit/Transparent"));
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x000080BE File Offset: 0x000062BE
	private static void initStatics()
	{
		CardView2._inited = true;
		CardView2._enabledScroll = CardView2.materialWithTexture("Scrolls/NewNewGraphics/small");
		CardView2._disabledScroll = CardView2.materialWithTexture("Scrolls/NewNewGraphics/small");
		CardView2._disabledScroll.color = AbstractCardView.DisabledColor;
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x000028DF File Offset: 0x00000ADF
	protected override void onUpdateGraphics()
	{
	}

	// Token: 0x06000969 RID: 2409 RVA: 0x000028DF File Offset: 0x00000ADF
	public override void setLocked(bool locked, bool useLargeLock)
	{
	}

	// Token: 0x0600096A RID: 2410 RVA: 0x0004A550 File Offset: 0x00048750
	public override void renderAsEnabled(bool enabled, float time)
	{
		if (enabled == this._enabled)
		{
			return;
		}
		if (enabled)
		{
			this.image.renderer.sharedMaterial = CardView2._enabledCache[this.card.typeId];
			base.renderer.sharedMaterial = CardView2._enabledScroll;
		}
		else
		{
			this.image.renderer.sharedMaterial = CardView2._disabledCache[this.card.typeId];
			base.renderer.sharedMaterial = CardView2._disabledScroll;
		}
		this._enabled = enabled;
	}

	// Token: 0x0400070A RID: 1802
	private static Dictionary<int, Material> _enabledCache = new Dictionary<int, Material>();

	// Token: 0x0400070B RID: 1803
	private static Dictionary<int, Material> _disabledCache = new Dictionary<int, Material>();

	// Token: 0x0400070C RID: 1804
	private static Material _enabledScroll;

	// Token: 0x0400070D RID: 1805
	private static Material _disabledScroll;

	// Token: 0x0400070E RID: 1806
	private static bool _inited = false;

	// Token: 0x0400070F RID: 1807
	private bool _enabled = true;

	// Token: 0x04000710 RID: 1808
	private GameObject image;

	// Token: 0x04000711 RID: 1809
	private GameObject costBoard;

	// Token: 0x04000712 RID: 1810
	private GameObject statBoard;
}
