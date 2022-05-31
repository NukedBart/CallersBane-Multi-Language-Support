using System;
using System.Collections;
using System.Collections.Generic;
using Gui;
using UnityEngine;

// Token: 0x0200008B RID: 139
public class SiftOverlay : MonoBehaviour
{
	// Token: 0x06000521 RID: 1313 RVA: 0x000377FC File Offset: 0x000359FC
	public void init(Card[] cards, SiftCallback cardFactory, bool allowInput)
	{
		this.cards = new List<Card>(cards);
		this.siftCallback = cardFactory;
		this.allowInput = allowInput;
		this.gui = new Gui3D(UnityUtil.getFirstOrtographicCamera());
		this.regularStyle = new GUIStyle(((GUISkin)ResourceManager.Load("_GUISkins/RegularUI")).button);
		this.regularStyle.fontSize = Screen.height / 36;
		foreach (Card card in cards)
		{
			CardView cardView = cardFactory.create(card);
			cardView.name = "Sift_" + card.getName();
			cardView.doHandleInput = true;
			cardView.doHandleClicks = false;
			cardView.enableShowHelp();
			cardView.setLayer(Layers.BattleModeUI_NoHandManager);
			cardView.setRaycastCamera(UnityUtil.getFirstOrtographicCamera());
			cardView.setRenderQueue(600);
			this.views.Add(new SiftOverlay.View(cardView));
		}
		int renderQueue = (this.views.Count <= 0) ? 97000 : (this.views[0].cardView.renderer.material.renderQueue - 100);
		this.overlay = PrimitiveFactory.createPlane(true);
		this.overlay.renderer.material = new Material(ResourceManager.LoadShader("Transparent/Diffuse"));
		this.overlay.renderer.material.mainTexture = ResourceManager.LoadTexture("Login/black");
		this.overlay.renderer.material.color = new Color(0f, 0f, 0f, 0f);
		this.overlay.renderer.material.renderQueue = renderQueue;
		this.overlay.layer = Layers.BattleModeUI;
		this.titleFont = (Font)ResourceManager.Load("Fonts/HoneyMeadBB_bold", typeof(Font));
		base.StartCoroutine(this.fadeIn());
		this.setupPositions();
	}

	// Token: 0x06000522 RID: 1314 RVA: 0x000379F8 File Offset: 0x00035BF8
	private void Update()
	{
		if (!this.allowInput)
		{
			return;
		}
		foreach (SiftOverlay.View view in this.views)
		{
			bool flag = view.updateScale();
			if (flag && Input.GetMouseButtonDown(0))
			{
				if (this.selected == view)
				{
					view.cardView.doHandleClicks = true;
					view.cardView.handleMouseOver(Input.mousePosition, true);
					view.cardView.doHandleClicks = false;
				}
				if (this.selected != view)
				{
					if (this.selected != null)
					{
						this.selected.cardView.onDeselect();
					}
					this.selected = ((this.selected == view) ? null : view);
					if (this.selected != null)
					{
						this.selected.cardView.onSelect();
					}
				}
			}
		}
	}

	// Token: 0x06000523 RID: 1315 RVA: 0x00005489 File Offset: 0x00003689
	public void close()
	{
		base.StartCoroutine(this.fadeOut());
	}

	// Token: 0x06000524 RID: 1316 RVA: 0x00037B0C File Offset: 0x00035D0C
	private void OnGUI()
	{
		Rect rect = GUIUtil.screen(new Rect(0f, 0.15f, 1f, 1f));
		Color color;
		color..ctor(0.8f, 0.8f, 0.8f, Mathf.Clamp01(1.5f * this.alpha));
		TextAnchor alignment = GUI.skin.label.alignment;
		Font font = GUI.skin.label.font;
		GUI.skin.label.alignment = 1;
		GUI.skin.label.font = this.titleFont;
		GUIUtil.drawShadowText(rect, "Pick one scroll", color, Screen.height / 24);
		GUI.skin.label.alignment = alignment;
		GUI.skin.label.font = font;
		this.OnGUI_drawPickButton();
	}

	// Token: 0x06000525 RID: 1317 RVA: 0x00037BE0 File Offset: 0x00035DE0
	private void OnGUI_drawPickButton()
	{
		if (!this.allowInput)
		{
			return;
		}
		GUI.enabled = (this.selected != null);
		Rect rect = GUIUtil.screen(new Rect(0f, 0.7f, 1f, 0.06f));
		rect = GeomUtil.resizeCentered(rect, rect.height * 5f);
		if (GUI.Button(rect, "Pick", this.regularStyle) && this.selected != null)
		{
			this.siftCallback.siftCard(this.selected.cardView.getCardInfo());
		}
		GUI.enabled = true;
	}

	// Token: 0x06000526 RID: 1318 RVA: 0x00037C80 File Offset: 0x00035E80
	private void OnDestroy()
	{
		if (this.overlay != null)
		{
			Object.Destroy(this.overlay);
			this.overlay = null;
		}
		foreach (SiftOverlay.View view in this.views)
		{
			Object.Destroy(view.cardView.gameObject);
		}
	}

	// Token: 0x06000527 RID: 1319 RVA: 0x00037D08 File Offset: 0x00035F08
	private void setupPositions()
	{
		this.gui.DrawObject(GUIUtil.screen(), this.overlay);
		Vector3 vector = CardView.CardLocalScale();
		vector *= 0.4f * (float)Screen.height / vector.z;
		Rect rect;
		rect..ctor(0f, 0f, vector.x, vector.z);
		rect.y = 0.41f * ((float)Screen.height - rect.height);
		int num = this.views.Count + 1;
		float num2 = (float)Screen.width / (float)num;
		for (int i = 0; i < this.views.Count; i++)
		{
			Rect rect2;
			rect2..ctor(rect);
			rect2.x = (float)(i + 1) * num2 - rect2.width / 2f;
			this.gui.DrawObject(rect2, this.views[i].cardView.gameObject);
			this.views[i].setRect(rect2);
		}
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x00037E1C File Offset: 0x0003601C
	private IEnumerator fadeIn()
	{
		this.alpha = this.overlay.renderer.material.color.a;
		while (this.alpha < 1f)
		{
			this.alpha += 1f * Math.Min(0.1f, Time.deltaTime);
			this.overlay.renderer.material.color = new Color(0f, 0f, 0f, Math.Min(0.5f * this.alpha, 0.5f));
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x00037E38 File Offset: 0x00036038
	private IEnumerator fadeOut()
	{
		this.alpha = this.overlay.renderer.material.color.a;
		float originalA = this.alpha;
		while (this.alpha > 0f)
		{
			this.alpha -= 2f * Math.Min(0.1f, Time.deltaTime);
			this.overlay.renderer.material.color = new Color(0f, 0f, 0f, Math.Max(0.5f * this.alpha, 0f));
			Color cardColor = new Color(1f, 1f, 1f, this.alpha / originalA);
			foreach (SiftOverlay.View v in this.views)
			{
				v.cardView.setColor(cardColor);
			}
			yield return new WaitForEndOfFrame();
		}
		Object.Destroy(this);
		yield break;
	}

	// Token: 0x0400039C RID: 924
	private List<Card> cards = new List<Card>();

	// Token: 0x0400039D RID: 925
	private List<SiftOverlay.View> views = new List<SiftOverlay.View>();

	// Token: 0x0400039E RID: 926
	private Gui3D gui;

	// Token: 0x0400039F RID: 927
	private SiftCallback siftCallback;

	// Token: 0x040003A0 RID: 928
	private GameObject overlay;

	// Token: 0x040003A1 RID: 929
	private SiftOverlay.View selected;

	// Token: 0x040003A2 RID: 930
	private float alpha;

	// Token: 0x040003A3 RID: 931
	private Font titleFont;

	// Token: 0x040003A4 RID: 932
	private GUIStyle regularStyle;

	// Token: 0x040003A5 RID: 933
	private bool allowInput;

	// Token: 0x0200008C RID: 140
	private class View
	{
		// Token: 0x0600052A RID: 1322 RVA: 0x00005498 File Offset: 0x00003698
		public View(CardView cardView)
		{
			this.cardView = cardView;
		}

		// Token: 0x0600052B RID: 1323 RVA: 0x000054B2 File Offset: 0x000036B2
		public void setRect(Rect rect)
		{
			this.rect = rect;
			this.scale = this.cardView.transform.localScale;
		}

		// Token: 0x0600052C RID: 1324 RVA: 0x00037E54 File Offset: 0x00036054
		public bool updateScale()
		{
			return this.rect.Contains(GUIUtil.getScreenMousePos());
		}

		// Token: 0x040003A6 RID: 934
		public CardView cardView;

		// Token: 0x040003A7 RID: 935
		public Rect rect;

		// Token: 0x040003A8 RID: 936
		private Vector3 scale;

		// Token: 0x040003A9 RID: 937
		private float s = 1f;
	}
}
