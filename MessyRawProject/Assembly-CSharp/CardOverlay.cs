using System;
using System.Collections;
using Gui;
using UnityEngine;

// Token: 0x02000134 RID: 308
public class CardOverlay : MonoBehaviour, IOverlayClickCallback, iCardRule
{
	// Token: 0x060009EC RID: 2540 RVA: 0x0004C8F4 File Offset: 0x0004AAF4
	private void Start()
	{
		this.overlay = new GameObject("CardOverlayBlackBG").AddComponent<GUIBlackOverlayButton>();
		this.overlay.Init(this, this.guiDepth + 1, false);
		this.overlay.enabled = false;
		this.overlay.transform.parent = base.transform;
		this.cardCamera = new GameObject("CardCamera").AddComponent<Camera>();
		this.cardCamera.clearFlags = 2;
		this.cardCamera.cullingMask = 1 << Layers.InFrontOfUI;
		this.cardCamera.orthographic = true;
		this.cardCamera.orthographicSize = 0.75f;
		this.cardCamera.nearClipPlane = 0.3f;
		this.cardCamera.farClipPlane = 15f;
		this.cardCamera.depth = -1f;
		this.cardCamera.enabled = false;
		this.cardCamera.targetTexture = this.renderTexture;
		this.cardCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
		this.cardCamera.transform.parent = base.transform;
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x000084BB File Offset: 0x000066BB
	public CardView GetCardView()
	{
		return this.cardView;
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x000084C3 File Offset: 0x000066C3
	public void OverlayClicked()
	{
		if (this.hideOverlayOnClick && !this.IsHovered())
		{
			this.Hide();
		}
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x0004CA28 File Offset: 0x0004AC28
	public void Init(RenderTexture renderTexture, int guiDepth)
	{
		float num = (float)Screen.height * 0.75f;
		float num2 = (float)Screen.width * 0.5f - num / 2f;
		float num3 = (float)Screen.height * 0.5f - num / 2f;
		this.Init(renderTexture, new Rect(num2, num3, num, num), guiDepth);
		this.useOverlay = true;
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x000084E1 File Offset: 0x000066E1
	public void Init(RenderTexture renderTexture, Rect cardPos, int guiDepth)
	{
		this.renderTexture = renderTexture;
		this.useOverlay = false;
		this.guiDepth = guiDepth;
		this.SetSquareRect(cardPos);
		if (this.cardCamera != null)
		{
			this.cardCamera.targetTexture = renderTexture;
		}
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x0000851C File Offset: 0x0000671C
	public void SetSquareRect(Rect squareRect)
	{
		this.cardPos = CardOverlay.convertOldRectToNew(squareRect);
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x0004CA84 File Offset: 0x0004AC84
	private static Rect convertOldRectToNew(Rect r)
	{
		Vector3 vector = CardView.CardLocalScale();
		float num = vector.x / vector.z;
		float num2 = 0.9f;
		return GeomUtil.scaleCentered(r, num2 * num, num2);
	}

	// Token: 0x060009F3 RID: 2547 RVA: 0x0004CAB8 File Offset: 0x0004ACB8
	public void Show(Card card)
	{
		this.cardCamera.enabled = true;
		this.overlay.enabled = this.useOverlay;
		this.overlay.SetAlpha(0.5f);
		this.show = true;
		if (this.cardView != null)
		{
			this.DestroyCardView();
		}
		if (this.cardView == null)
		{
			this.cardView = PrimitiveFactory.createPlane().AddComponent<CardView>();
			this.cardView.name = "CardOverlay_CardView";
			this.cardView.init(this, card, 100);
			this.cardView.doHandleInput = false;
			this.cardView.applyHighResTexture();
			this.cardView.setLayer(Layers.InFrontOfUI);
			this.cardView.enableShowHelp();
			this.cardView.setRaycastCamera(this.cardCamera);
			Rect dst = GeomUtil.scaleCentered(GUIUtil.screen(), 0.925f);
			new Gui3D(this.cardCamera).DrawObject(dst, this.cardView.gameObject);
		}
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x0000852A File Offset: 0x0000672A
	public void Hide()
	{
		this.cardCamera.enabled = false;
		this.overlay.enabled = false;
		this.show = false;
		this.DestroyCardView();
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x00008551 File Offset: 0x00006751
	public bool isShowing()
	{
		return this.show;
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x00008559 File Offset: 0x00006759
	public void FadeOut(float duration)
	{
		base.StartCoroutine(this.FadeOutCoroutine(duration));
	}

	// Token: 0x060009F7 RID: 2551 RVA: 0x0004CBC0 File Offset: 0x0004ADC0
	public IEnumerator FadeOutCoroutine(float duration)
	{
		float t = 0f;
		float timeStarted = Time.time;
		while (t < 1f)
		{
			t = Mathf.Min((Time.time - timeStarted) / duration, 1f);
			this.cardView.setTransparency(1f - t);
			this.overlay.SetAlpha(0.5f - t * 0.5f);
			yield return null;
		}
		this.cardView.setTransparency(0f);
		this.overlay.SetAlpha(0f);
		this.Hide();
		yield break;
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x00008569 File Offset: 0x00006769
	public void SetHideOverlayOnClick(bool hideOverlayOnClick)
	{
		this.hideOverlayOnClick = hideOverlayOnClick;
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x00008572 File Offset: 0x00006772
	private void DestroyCardView()
	{
		if (this.cardView != null)
		{
			Object.DestroyImmediate(this.cardView.gameObject);
			this.cardView = null;
		}
	}

	// Token: 0x060009FA RID: 2554 RVA: 0x0000859C File Offset: 0x0000679C
	private Vector2 mapMousePosition()
	{
		return this.mapMousePosition(Input.mousePosition);
	}

	// Token: 0x060009FB RID: 2555 RVA: 0x0004CBEC File Offset: 0x0004ADEC
	private Vector2 mapMousePosition(Vector2 p)
	{
		return new Vector2((float)this.renderTexture.width * (p.x - this.cardPos.x) / this.cardPos.width, (float)this.renderTexture.height * (1f - ((float)Screen.height - (p.y + this.cardPos.y)) / this.cardPos.height));
	}

	// Token: 0x060009FC RID: 2556 RVA: 0x000085AE File Offset: 0x000067AE
	private void Update()
	{
		if (!this.show)
		{
			return;
		}
		this.cardView.handleMouseOver(this.mapMousePosition(), Input.GetMouseButtonDown(0));
	}

	// Token: 0x060009FD RID: 2557 RVA: 0x000085D4 File Offset: 0x000067D4
	private void OnGUI()
	{
		if (!this.show)
		{
			return;
		}
		GUI.depth = this.guiDepth;
		GUI.DrawTexture(this.cardPos, this.renderTexture);
	}

	// Token: 0x060009FE RID: 2558 RVA: 0x000085FE File Offset: 0x000067FE
	public bool IsHovered()
	{
		return this.cardView != null && this.cardView.isClicked(this.mapMousePosition(), true);
	}

	// Token: 0x060009FF RID: 2559 RVA: 0x000028DF File Offset: 0x00000ADF
	public void HideCardView()
	{
	}

	// Token: 0x06000A00 RID: 2560 RVA: 0x000028DF File Offset: 0x00000ADF
	public void ActivateTriggeredAbility(string id, TilePosition pos)
	{
	}

	// Token: 0x040007A7 RID: 1959
	private const float cardScale = 0.925f;

	// Token: 0x040007A8 RID: 1960
	private GUIBlackOverlayButton overlay;

	// Token: 0x040007A9 RID: 1961
	private bool show;

	// Token: 0x040007AA RID: 1962
	private RenderTexture renderTexture;

	// Token: 0x040007AB RID: 1963
	private CardView cardView;

	// Token: 0x040007AC RID: 1964
	private Camera cardCamera;

	// Token: 0x040007AD RID: 1965
	private Rect cardPos;

	// Token: 0x040007AE RID: 1966
	private bool useOverlay;

	// Token: 0x040007AF RID: 1967
	private bool hideOverlayOnClick = true;

	// Token: 0x040007B0 RID: 1968
	private int guiDepth;
}
