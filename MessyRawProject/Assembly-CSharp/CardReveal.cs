using System;
using Gui;
using Irrelevant.Assets;
using UnityEngine;

// Token: 0x0200007D RID: 125
internal class CardReveal : iEffect
{
	// Token: 0x060004D8 RID: 1240 RVA: 0x000051B4 File Offset: 0x000033B4
	public CardReveal Init(Gui3D gui, Action onDone, RenderTexture rt, Rect rect)
	{
		this.gui = gui;
		this.rect = rect;
		this.onDone = onDone;
		return this;
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x000357D8 File Offset: 0x000339D8
	public void Show(Card card)
	{
		this.card = card;
		this.view = CardReveal.createCardView(card);
		Rect dst = GeomUtil.scaleCentered(this.rect, this.scale, this.scale);
		this.size = new Vector2(this.rect.width / 331.6f, this.rect.height / 536.4f);
		Log.warning("size: " + this.size);
		dst.y += dst.height * 0.033f;
		dst.x -= dst.width * 0.015f;
		this.gui.DrawObject(dst, this.view.gameObject);
		this.playBuyEffect(card);
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x000358AC File Offset: 0x00033AAC
	public void Hide()
	{
		if (this.view != null)
		{
			Object.Destroy(this.view.gameObject);
			this.view = null;
		}
		if (this.appear != null)
		{
			Object.Destroy(this.appear.gameObject);
			this.appear = null;
		}
		if (this.shine != null)
		{
			Object.Destroy(this.shine.gameObject);
			this.shine = null;
		}
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x00035934 File Offset: 0x00033B34
	private static CardView createCardView(Card card)
	{
		GameObject gameObject = PrimitiveFactory.createPlane();
		gameObject.name = "CardRule";
		CardView cardView = gameObject.AddComponent<CardView>();
		cardView.setTooltipEnabled(false);
		cardView.init(null, card, 240);
		cardView.applyHighResTexture();
		cardView.setLayer(Layers.BattleModeUI);
		return cardView;
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x00035980 File Offset: 0x00033B80
	public void playBuyEffect(Card card)
	{
		App.AudioScript.PlaySFX("Sounds/scroll_purchase_01");
		if (card.level >= 1 || card.getRarity() >= 2)
		{
			this.appear = this.playBuyEffect("Scroll_appear_3a_appear");
			this.shine = this.playBuyEffect("Scroll_appear_3b_rimshine");
		}
		else if (card.getRarity() == 1)
		{
			this.appear = this.playBuyEffect("Scroll_appear_2a_appear");
			this.shine = this.playBuyEffect("Scroll_appear_1_2b_rimshine");
		}
		else if (card.getRarity() == 0)
		{
			this.appear = this.playBuyEffect("Scroll_appear_1a_appear");
			this.shine = this.playBuyEffect("Scroll_appear_1_2b_rimshine");
		}
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x00035A3C File Offset: 0x00033C3C
	private EffectPlayer playBuyEffect(string file)
	{
		GameObject gameObject = new GameObject();
		this.gui.DrawObject(this.rect.center.x, this.rect.center.y, gameObject);
		gameObject.AddComponent<MeshRenderer>();
		gameObject.name = "BuyEffect_";
		EffectPlayer effectPlayer = gameObject.AddComponent<EffectPlayer>();
		Material materialToUse = new Material(ResourceManager.LoadShader("Scrolls/StoreEffect/Unlit/Transparent"));
		effectPlayer.setMaterialToUse(materialToUse);
		effectPlayer.init("BuyEffect/" + file, 1, this, 96500, new Vector3(this.size.x, this.size.y * 0.9875f, 1f) * (this.scale * 42.5f), false, string.Empty, 0);
		effectPlayer.layer = Layers.BattleModeUI;
		effectPlayer.getAnimPlayer().waitForUpdate();
		effectPlayer.transform.localEulerAngles = Vector3.zero;
		return effectPlayer;
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x000028DF File Offset: 0x00000ADF
	public void OnGUI()
	{
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x000051CD File Offset: 0x000033CD
	private void setState(int newState)
	{
		if (newState == this.state)
		{
			return;
		}
		this.onStateChanged(this.state, newState);
		this.state = newState;
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x000051F0 File Offset: 0x000033F0
	private void onStateChanged(int oldState, int newState)
	{
		if (newState == 2)
		{
		}
	}

	// Token: 0x060004E1 RID: 1249 RVA: 0x000051F9 File Offset: 0x000033F9
	public void effectAnimDone(EffectPlayer effect, bool loop)
	{
		this.setState(2);
		DefaultIEffectCallback.instance().effectAnimDone(effect, false);
		if (this.onDone != null)
		{
			this.onDone.Invoke();
		}
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x000028DF File Offset: 0x00000ADF
	public void locator(EffectPlayer effect, AnimLocator loc)
	{
	}

	// Token: 0x04000348 RID: 840
	private int state;

	// Token: 0x04000349 RID: 841
	private Card card;

	// Token: 0x0400034A RID: 842
	private GameObject parent;

	// Token: 0x0400034B RID: 843
	private Gui3D gui;

	// Token: 0x0400034C RID: 844
	private Rect rect;

	// Token: 0x0400034D RID: 845
	private Action onDone;

	// Token: 0x0400034E RID: 846
	private CardView view;

	// Token: 0x0400034F RID: 847
	private float scale = 1f;

	// Token: 0x04000350 RID: 848
	private Vector2 size;

	// Token: 0x04000351 RID: 849
	private EffectPlayer appear;

	// Token: 0x04000352 RID: 850
	private EffectPlayer shine;
}
