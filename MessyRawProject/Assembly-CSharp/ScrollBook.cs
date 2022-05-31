using System;
using System.Collections;
using System.Collections.Generic;
using Gui;
using UnityEngine;

// Token: 0x020001A8 RID: 424
public class ScrollBook : MonoBehaviour
{
	// Token: 0x06000D5F RID: 3423 RVA: 0x0005D970 File Offset: 0x0005BB70
	private void setLayer(int layerIndex)
	{
		if (layerIndex < 0 || layerIndex >= 2)
		{
			throw new ArgumentException("layerIndex out of range");
		}
		this.currentLayer().savedXScroll = this.xScroll;
		this.currentLayerIndex = layerIndex;
		this.piles = this.currentLayer().piles;
		this.setScroll(this.currentLayer().savedXScroll);
	}

	// Token: 0x06000D60 RID: 3424 RVA: 0x0000AB09 File Offset: 0x00008D09
	private ScrollBook.BookLayerState currentLayer()
	{
		return this.layers[this.currentLayerIndex];
	}

	// Token: 0x06000D61 RID: 3425 RVA: 0x0000AB1C File Offset: 0x00008D1C
	private bool isInside(float x, float y)
	{
		return this.boundingBoxInput.Contains(new Vector2(x, y));
	}

	// Token: 0x06000D62 RID: 3426 RVA: 0x0005D9D0 File Offset: 0x0005BBD0
	private GameObject createCardView(Card card, int layer)
	{
		GameObject gameObject = PrimitiveFactory.createPlane(true);
		gameObject.renderer.material = new Material(ResourceManager.LoadShader("Scrolls/Unlit/Transparent"));
		CardView cardView = gameObject.AddComponent<CardView>();
		cardView.init(null, card, -1);
		cardView.setLayer(layer);
		cardView.setTooltipEnabled(false);
		return gameObject;
	}

	// Token: 0x06000D63 RID: 3427 RVA: 0x0005DA20 File Offset: 0x0005BC20
	private void resizeViewList(ScrollBook.BookLayerState layer)
	{
		List<ScrollBook.C> views = layer.views;
		int count = views.Count;
		int num = this.numCards - count;
		if (num < 0)
		{
			foreach (ScrollBook.C c in views.GetRange(this.numCards, -num))
			{
				Object.Destroy(c.pileView);
				Object.Destroy(c.g);
			}
			views.RemoveRange(this.numCards, -num);
		}
		if (num > 0)
		{
			CardType cardType = new CardType();
			cardType.costGrowth = 1;
			int layer2 = 12 + layer.layerIndex;
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = this.createCardView(new Card((long)i, cardType), layer2);
				GameObject gameObject2 = gameObject.gameObject;
				GameObject gameObject3 = PrimitiveFactory.createPlane(false);
				gameObject3.renderer.material = new Material(ResourceManager.LoadShader("Scrolls/Unlit/Transparent"));
				gameObject3.transform.localScale = gameObject2.transform.localScale;
				gameObject3.layer = layer2;
				gameObject3.name = string.Concat(new object[]
				{
					"Pile_Layer",
					layer.layerIndex,
					"_",
					count + i
				});
				gameObject2.name = string.Concat(new object[]
				{
					"View_Layer",
					layer.layerIndex,
					"_",
					count + i
				});
				views.Add(new ScrollBook.C(gameObject2, gameObject3));
			}
		}
	}

	// Token: 0x06000D64 RID: 3428 RVA: 0x0000AB30 File Offset: 0x00008D30
	public ScrollBook setListener(ScrollBook.IListener listener)
	{
		this.listener = listener;
		return this;
	}

	// Token: 0x06000D65 RID: 3429 RVA: 0x0000AB3A File Offset: 0x00008D3A
	public ScrollBook setInputEnabled(bool enabled)
	{
		this.inputEnabled = enabled;
		return this;
	}

	// Token: 0x06000D66 RID: 3430 RVA: 0x0000AB44 File Offset: 0x00008D44
	public ScrollBook setStackByLevel(bool enabled)
	{
		if (enabled)
		{
			return this.setStackByFunction(new ScrollBook.SameStackFunction(ScrollBook._stackByTypeAndLevel));
		}
		return this.setStackByFunction(new ScrollBook.SameStackFunction(ScrollBook._stackByType));
	}

	// Token: 0x06000D67 RID: 3431 RVA: 0x0000AB71 File Offset: 0x00008D71
	public ScrollBook setStackByFunction(ScrollBook.SameStackFunction f)
	{
		this.layerBase.setStackByFunction(f);
		return this;
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x0005DBE0 File Offset: 0x0005BDE0
	public void init(Rect rect, float baseZDepth)
	{
		if (!this.isInited)
		{
			for (int i = 0; i < 2; i++)
			{
				this.layers.Add(new ScrollBook.BookLayerState(i));
			}
			this.layerBase = this.layers[0];
			this.isInited = true;
			this.setLayer(0);
		}
		this.setRect(rect);
		this.setBoundingRect(rect);
		this.timer.restart();
		this.gui = new Gui3D(Camera.main);
		this.gui.setBaseDepth(baseZDepth);
	}

	// Token: 0x06000D69 RID: 3433 RVA: 0x0000AB80 File Offset: 0x00008D80
	public void setScrollVel(float vel)
	{
		this.scrollVelocity = vel;
	}

	// Token: 0x06000D6A RID: 3434 RVA: 0x0000AB89 File Offset: 0x00008D89
	public void scrollTo(float index)
	{
		this.setScroll(index + 0.5f);
	}

	// Token: 0x06000D6B RID: 3435 RVA: 0x0000AB98 File Offset: 0x00008D98
	public float scrollPos()
	{
		return this.xScroll - 0.5f;
	}

	// Token: 0x06000D6C RID: 3436 RVA: 0x0005DC74 File Offset: 0x0005BE74
	public void setRect(Rect rect)
	{
		this.boundingBox = new Rect(rect);
		this.cardScale = rect.height / 300f;
		this.numCards = 2 * Mathf.CeilToInt(rect.width / rect.height * 1.4f) + 1;
		this.margin = (float)(this.numCards / 2) + 0.5f;
		this.centerIndex = this.numCards / 2;
		foreach (ScrollBook.BookLayerState layer in this.layers)
		{
			this.resizeViewList(layer);
		}
	}

	// Token: 0x06000D6D RID: 3437 RVA: 0x0000ABA6 File Offset: 0x00008DA6
	public void setBoundingRect(Rect inputBoundingRect)
	{
		this.boundingBoxInput = inputBoundingRect;
	}

	// Token: 0x06000D6E RID: 3438 RVA: 0x0005DD34 File Offset: 0x0005BF34
	public void setCards(List<Card> cards)
	{
		bool flag = this.piles == null || this.piles.Count == 0;
		this.setCards(0, cards);
		if (flag)
		{
			float num = Mathf.Floor((float)(this.piles.Count - 1) * 0.5f) + 0.5f;
			this.setScroll(Mathf.Min(num, (float)(this.centerIndex - 2) + 0.5f));
		}
		if (this.currentLayerIndex == 0 || this.expansionCard == null)
		{
			return;
		}
		int cardIndexForType = this.layerBase.getCardIndexForType(this.expansionCard);
		if (cardIndexForType < 0)
		{
			return;
		}
		this.setCards(1, this.layerBase.piles[cardIndexForType].cards);
	}

	// Token: 0x06000D6F RID: 3439 RVA: 0x0005DDF8 File Offset: 0x0005BFF8
	private void setCards(int layerIndex, List<Card> cards)
	{
		ScrollBook.BookLayerState bookLayerState = this.layers[layerIndex];
		List<ScrollBook.CardPile> list = bookLayerState.piles;
		ScrollBook._IndexFinder indexFinder = new ScrollBook._IndexFinder(this, bookLayerState).push(this.lastCenterCardIndex);
		list.Clear();
		Card card = null;
		int num = 0;
		foreach (Card card2 in cards)
		{
			if (card == null || !bookLayerState.stacks(card, card2, num))
			{
				card = card2;
				num = 0;
				list.Add(new ScrollBook.CardPile());
			}
			else
			{
				num++;
			}
			list[list.Count - 1].cards.Add(card2);
		}
		if (this.currentLayerIndex == 0)
		{
			indexFinder.popIfNearby();
		}
		for (int i = 0; i < bookLayerState.views.Count; i++)
		{
			this.hide(bookLayerState.views[i].pileView);
		}
	}

	// Token: 0x06000D70 RID: 3440 RVA: 0x0005DF10 File Offset: 0x0005C110
	private void updateScroll()
	{
		int num = this.timer.tick();
		for (int i = 0; i < num; i++)
		{
			this.scrollVelocity *= 0.95f;
			this.setScroll(this.xScroll + this.scrollVelocity);
		}
	}

	// Token: 0x06000D71 RID: 3441 RVA: 0x0005DF60 File Offset: 0x0005C160
	private void setScroll(float x)
	{
		float num = 0.5f;
		float num2 = (float)this.piles.Count - 0.5f;
		if (x < num || x > num2)
		{
			this.scrollVelocity = 0f;
		}
		this.xScroll = Mth.clamp(x, num, num2);
	}

	// Token: 0x06000D72 RID: 3442 RVA: 0x0005DFB0 File Offset: 0x0005C1B0
	private Rect getRectForCardIndex(float i)
	{
		float num = i - this.xScroll + this.margin;
		float num2 = this.boundingBox.x + this.boundingBox.width / 2f - this.cardScale * 104f / 2f;
		float num3 = num - (float)this.centerIndex;
		float t = (this.currentLayerIndex <= 0) ? 1f : Mth.clamp(this.T, 0f, 1f);
		float num4 = Easing.InOutQuad(new Easing.Data(0f, 1f, t));
		float num5 = num2 + num4 * (num3 * this.cardScale * 92f);
		float num6 = this.boundingBox.y + (this.boundingBox.height - this.cardScale * 168f) * 0.32f;
		Rect result;
		result..ctor(num5, num6, this.cardScale * 104f, this.cardScale * 168f);
		return result;
	}

	// Token: 0x06000D73 RID: 3443 RVA: 0x0005E0B4 File Offset: 0x0005C2B4
	private void drawCard(Rect rect, int viewIndex, int cardIndex)
	{
		ScrollBook.CardPile cardPile = this.piles[cardIndex];
		GameObject pileView = this.currentLayer().views[viewIndex].pileView;
		int count = cardPile.cards.Count;
		float num = (count <= 1) ? 0f : -0.08f;
		if (count == 2)
		{
			pileView.renderer.material.mainTexture = ResourceManager.LoadTexture("Scrolls/NewNewGraphics/scrolls__scrollbase_piledown_1");
		}
		else if (count >= 3)
		{
			pileView.renderer.material.mainTexture = ResourceManager.LoadTexture("Scrolls/NewNewGraphics/scrolls__scrollbase_piledown_2");
		}
		pileView.renderer.enabled = (count > 1);
		rect = GeomUtil.floor(GeomUtil.scaleCentered(rect, cardPile.scale));
		float num2 = this.cardScale * 168f * cardPile.scale * num;
		float depth = this.gui.getDepth();
		float num3 = depth + (float)Mathf.Abs(this.centerIndex - viewIndex) * 1.1f;
		Rect translated = GeomUtil.getTranslated(rect, 0f, -num2);
		translated.yMin = translated.yMax - translated.height / 8f;
		this.gui.setDepth(num3 + 1.1f);
		this.gui.DrawObject(translated, pileView);
		this.gui.setDepth(num3);
		this.gui.DrawObject(rect, this.currentLayer().views[viewIndex].g);
		this.gui.setDepth(depth);
	}

	// Token: 0x06000D74 RID: 3444 RVA: 0x0005E238 File Offset: 0x0005C438
	private void rotateViews()
	{
		int num = (int)this.xScroll - (int)this._lastScroll;
		this._lastScroll = this.xScroll;
		List<ScrollBook.C> views = this.currentLayer().views;
		if (Math.Abs(num) >= views.Count)
		{
			return;
		}
		if (num < 0)
		{
			CollectionUtil.rotateRight<ScrollBook.C>(views, -num);
			for (int i = 0; i < -num; i++)
			{
				this.hideViews(i);
			}
		}
		if (num > 0)
		{
			CollectionUtil.rotateLeft<ScrollBook.C>(views, num);
			for (int j = 0; j < num; j++)
			{
				this.hideViews(views.Count - 1 - j);
			}
		}
	}

	// Token: 0x06000D75 RID: 3445 RVA: 0x0005E2D8 File Offset: 0x0005C4D8
	private void Update()
	{
		if (this.currentLayer().views.Count == 0)
		{
			return;
		}
		if (this.inputEnabled)
		{
			this.updateMouseDrag();
			this.updateDrag();
		}
		this.updateScroll();
		this.rotateViews();
		if (this._ttype > 0)
		{
			this.T += Time.deltaTime / 0.28f;
		}
		if (this._ttype < 0)
		{
			this.T -= Time.deltaTime / 0.28f;
		}
		this.gui.frameBegin();
		for (int i = 0; i < this.numCards; i++)
		{
			int num = this.viewToCardIndex(i);
			if (num < 0 || num >= this.piles.Count)
			{
				this.hideViews(i);
			}
			else
			{
				ScrollBook.C c = this.currentLayer().views[i];
				ScrollBook.CardPile cardPile = this.piles[num];
				Card card = cardPile.cards[0];
				CardView component = c.g.GetComponent<CardView>();
				if (component.needUpdateFor(card))
				{
					this.listener.onCardEnterView(card, component);
					component.forceUpdateGraphics(card);
				}
				c.cardIndex = num;
				c.hidden = false;
				Rect rectForCardIndex = this.getRectForCardIndex((float)num);
				if (i == this.centerIndex && num == this.lastCenterCardIndex)
				{
					cardPile.scale = Mathf.Min(cardPile.scale + Time.deltaTime * 1.1f, 1.1f);
				}
				else
				{
					cardPile.scale = Mathf.Max(cardPile.scale - Time.deltaTime * 1.1f, 1f);
				}
				if (i == this.centerIndex)
				{
					this.lastCenterCardIndex = num;
				}
				this.drawCard(rectForCardIndex, i, num);
				c.rect = rectForCardIndex;
			}
		}
		this.gui.frameEnd();
	}

	// Token: 0x06000D76 RID: 3446 RVA: 0x0005E4C0 File Offset: 0x0005C6C0
	private void hideViews(int viewIndex)
	{
		ScrollBook.BookLayerState bookLayerState = this.currentLayer();
		if (bookLayerState.views[viewIndex].hidden)
		{
			return;
		}
		bookLayerState.views[viewIndex].hidden = true;
		this.hide(bookLayerState.views[viewIndex].g);
		this.hide(bookLayerState.views[viewIndex].pileView);
	}

	// Token: 0x06000D77 RID: 3447 RVA: 0x0000ABAF File Offset: 0x00008DAF
	private void hide(GameObject view)
	{
		this.gui.DrawObject(new Rect(-1000f, -1000f, 1f, 1f), view);
	}

	// Token: 0x06000D78 RID: 3448 RVA: 0x0005E52C File Offset: 0x0005C72C
	private int getIndex(GameObject g)
	{
		for (int i = 0; i < this.currentLayer().views.Count; i++)
		{
			if (this.currentLayer().views[i].g == g)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000D79 RID: 3449 RVA: 0x0005E580 File Offset: 0x0005C780
	private GameObject getCardUnderMouse()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, ref raycastHit, 2000f, 1 << 12 + this.currentLayerIndex))
		{
			GameObject gameObject = raycastHit.collider.gameObject;
			if (gameObject.GetComponent<CardView>())
			{
				return gameObject;
			}
			if (gameObject.transform.parent.gameObject.GetComponent<CardView>())
			{
				return gameObject.transform.parent.gameObject;
			}
		}
		return null;
	}

	// Token: 0x06000D7A RID: 3450 RVA: 0x0005E610 File Offset: 0x0005C810
	private ScrollBook.C gameObjectToC(GameObject g)
	{
		foreach (ScrollBook.C c in this.currentLayer().views)
		{
			if (c.g == g)
			{
				return c;
			}
		}
		return null;
	}

	// Token: 0x06000D7B RID: 3451 RVA: 0x0000ABD6 File Offset: 0x00008DD6
	public GameObject getCenterCardView()
	{
		return this.currentLayer().views[this.centerIndex].g;
	}

	// Token: 0x06000D7C RID: 3452 RVA: 0x0000ABF3 File Offset: 0x00008DF3
	public Card getCenterCard()
	{
		if (this.state == ScrollBook.State.DRAGGING || this.state == ScrollBook.State.AUTOSCROLL_CENTER || Mathf.Abs(this.scrollVelocity) > 0.0005f)
		{
			return null;
		}
		return this.getCardAt(this.centerIndex);
	}

	// Token: 0x06000D7D RID: 3453 RVA: 0x0005E684 File Offset: 0x0005C884
	private Card getCardAt(int viewIndex)
	{
		int num = this.viewToCardIndex(viewIndex);
		if (num < 0 || num >= this.piles.Count)
		{
			return null;
		}
		return this.getTopmost(num);
	}

	// Token: 0x06000D7E RID: 3454 RVA: 0x0000AC30 File Offset: 0x00008E30
	private int viewToCardIndex(int index)
	{
		return index + Mth.iFloor(this.xScroll + 0.5f - this.margin);
	}

	// Token: 0x06000D7F RID: 3455 RVA: 0x0000AC4C File Offset: 0x00008E4C
	private float viewToCardIndex(float index)
	{
		return index + this.xScroll + 0.5f - this.margin;
	}

	// Token: 0x06000D80 RID: 3456 RVA: 0x0000AC63 File Offset: 0x00008E63
	public bool isExpanded()
	{
		return this.currentLayerIndex > 0;
	}

	// Token: 0x06000D81 RID: 3457 RVA: 0x0000AC6E File Offset: 0x00008E6E
	public void expand()
	{
		this.expand(this.lastCenterCardIndex);
	}

	// Token: 0x06000D82 RID: 3458 RVA: 0x0005E6BC File Offset: 0x0005C8BC
	public void expand(Card card)
	{
		int cardIndexForType = this.currentLayer().getCardIndexForType(card);
		if (cardIndexForType >= 0)
		{
			this.expand(cardIndexForType);
		}
	}

	// Token: 0x06000D83 RID: 3459 RVA: 0x0005E6E4 File Offset: 0x0005C8E4
	private void expand(int cardIndex)
	{
		if (this.expansionState != 0)
		{
			return;
		}
		if (this.currentLayerIndex >= 1)
		{
			throw new InvalidOperationException("Scrollbook stack exhausted on expand!");
		}
		this.T = -0.5f;
		this._ttype = 1;
		this.expansionState = 1;
		IEnumerator[] array = new IEnumerator[2];
		array[0] = this.fadeCards(0, 1f, -5f, (float a) => a > 0f);
		array[1] = this.setExpansionState(0);
		base.StartCoroutine(EnumeratorUtil.chain(array));
		base.StartCoroutine(this.fadeCards(1, 0f, 50f, (float a) => a < 1f));
		List<Card> list = null;
		try
		{
			if (this.inRange(cardIndex))
			{
				list = this.getCards(cardIndex);
			}
			else if (this.piles.Count >= 1)
			{
				list = this.getCards(0);
			}
			else
			{
				list = new List<Card>();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"ERR: ",
				ex,
				"\n:: ",
				this.lastCenterCardIndex,
				", ",
				this.piles.Count,
				", -- ",
				this.currentLayerIndex
			}));
		}
		int num = this.currentLayerIndex + 1;
		this.setLayer(num);
		this.setCards(num, list);
		this.expansionCard = ((list.Count <= 0) ? null : list[0]);
		foreach (ScrollBook.CardPile cardPile in this.piles)
		{
			cardPile.scale = 1.1f;
		}
		this.setScroll((float)((this.piles.Count - 1) / 2) + 0.5f);
		this.scrollVelocity = 0f;
	}

	// Token: 0x06000D84 RID: 3460 RVA: 0x0000AC7C File Offset: 0x00008E7C
	public void contract()
	{
		if (this.expansionState != 0)
		{
			return;
		}
		if (this.currentLayerIndex <= 0)
		{
			throw new InvalidOperationException("Scrollbook stack exhausted on contract!");
		}
		this.expansionState = -1;
		base.StartCoroutine(this.contractCards());
	}

	// Token: 0x06000D85 RID: 3461 RVA: 0x0005E91C File Offset: 0x0005CB1C
	private void updateMouseDrag()
	{
		Vector2 screenMousePos = GUIUtil.getScreenMousePos();
		if (Input.GetMouseButtonDown(0))
		{
			if (this.isInside(screenMousePos.x, screenMousePos.y))
			{
				this.state = ScrollBook.State.PENDING_DECISION;
			}
			this.startDrag = (this.lastDrag = screenMousePos);
			bool flag = this.startDragTime.clicked(2);
			this.dragDistance = 0f;
			GameObject cardUnderMouse = this.getCardUnderMouse();
			ScrollBook.C c = this.currentCardDragged;
			this.currentCardDragged = this.gameObjectToC(cardUnderMouse);
			this.cbDeltaX.clear();
			if (flag && this.getIndex(cardUnderMouse) == this.centerIndex && this.currentCardDragged == c)
			{
				this.onCardDoubleClicked(this.centerIndex, this.currentCardDragged);
				this.startDragTime.clear();
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			float num = Mathf.Abs(this.cbDeltaX.avg());
			float timeSinceClick = this.startDragTime.getTimeSinceClick();
			float num2 = screenMousePos.x - this.startDrag.x;
			if (this.state == ScrollBook.State.DRAGGING && num > 0.01f)
			{
				float num3 = timeSinceClick * 2000f / 15f;
				float num4 = num2 / (this.cardScale * 104f * num3);
				this.scrollVelocity = -num4;
				this.state = ScrollBook.State.AUTOSCROLL;
			}
			if (this.state == ScrollBook.State.DRAGGING || this.state == ScrollBook.State.PENDING_DECISION)
			{
				if (timeSinceClick <= 0.2f && Mathf.Abs(num2) < 10f)
				{
					GameObject cardUnderMouse2 = this.getCardUnderMouse();
					if (cardUnderMouse2 != null)
					{
						ScrollBook.C c2 = this.gameObjectToC(cardUnderMouse2);
						int index = this.getIndex(cardUnderMouse2);
						this.onCardClicked(index, c2);
					}
				}
				else
				{
					this.scrollVelocity = 0f;
					this.state = ScrollBook.State.AUTOSCROLL_FINALIZE;
				}
			}
		}
		if (Input.GetMouseButton(0))
		{
			if (this.state == ScrollBook.State.PENDING_DECISION)
			{
				Vector2 vector = screenMousePos - this.lastDrag;
				float num5 = Mathf.Abs(vector.x);
				float num6 = Mathf.Abs(vector.y);
				this.dragDistance += num5 + num6;
				if (this.dragDistance > 10f && num5 > num6)
				{
					this.state = ScrollBook.State.DRAGGING;
					this.startDragTime.clear();
				}
				else if (this.dragDistance > 10f && num6 > num5)
				{
					this.state = ScrollBook.State.IDLE;
					this.startDragTime.clear();
					if (this.currentCardDragged != null)
					{
						this.onStartDragCard(this.currentCardDragged);
					}
				}
				else if (this.startDragTime.getTimeSinceClick() > 0.4f && this.currentCardDragged != null)
				{
					this.onCardHeld(this.currentCardDragged);
				}
			}
			else if (this.state == ScrollBook.State.DRAGGING)
			{
				float num7 = (this.lastDrag.x - screenMousePos.x) / (this.cardScale * 104f);
				this.setScroll(this.xScroll + num7);
				this.cbDeltaX.add(num7);
			}
			this.lastDrag = screenMousePos;
		}
	}

	// Token: 0x06000D86 RID: 3462 RVA: 0x0005EC44 File Offset: 0x0005CE44
	private void updateDrag()
	{
		if (this.state == ScrollBook.State.AUTOSCROLL_CENTER)
		{
			this.updateCenterTween();
			return;
		}
		if (this.state != ScrollBook.State.AUTOSCROLL && this.state != ScrollBook.State.AUTOSCROLL_FINALIZE)
		{
			this.autoDirection = 0f;
			return;
		}
		if (Mathf.Abs(this.scrollVelocity) > 0.01f)
		{
			return;
		}
	}

	// Token: 0x06000D87 RID: 3463 RVA: 0x0005ECA0 File Offset: 0x0005CEA0
	private void updateFinalizeScroll()
	{
		float num = this.xScroll % 1f;
		if (this.autoDirection == 0f)
		{
			float num2 = Mathf.Abs(this.cbDeltaX.avg());
			if (num2 > 0.0001f)
			{
				this.autoDirection = Mth.sign(this.scrollVelocity);
			}
			else
			{
				this.autoDirection = (float)((num >= 0.5f) ? 1 : -1);
			}
			this.state = ScrollBook.State.AUTOSCROLL_FINALIZE;
		}
		if (num < 0.01f)
		{
			this.setScroll(this.xScroll - num);
			this.scrollVelocity = 0f;
		}
		else if (num > 0.99f)
		{
			this.setScroll(this.xScroll + 1f - num);
			this.scrollVelocity = 0f;
		}
		else
		{
			this.scrollVelocity = this.autoDirection * 0.01f;
		}
	}

	// Token: 0x06000D88 RID: 3464 RVA: 0x0000ACB5 File Offset: 0x00008EB5
	private void updateCenterTween()
	{
		this.setScroll(this.centerEasing.currentValue());
		if (Mathf.Abs(this.xScroll - this.centerEasing.getData().dst) < 0.001f)
		{
			this.setState(ScrollBook.State.IDLE);
		}
	}

	// Token: 0x06000D89 RID: 3465 RVA: 0x0000ACF5 File Offset: 0x00008EF5
	public void setState(ScrollBook.State newState)
	{
		this.state = newState;
	}

	// Token: 0x06000D8A RID: 3466 RVA: 0x0005ED88 File Offset: 0x0005CF88
	private void onCardClicked(int index, ScrollBook.C c)
	{
		if (Mathf.Abs(this.scrollVelocity) < 0.01f)
		{
			if (index != this.centerIndex)
			{
				float num = (float)c.cardIndex + 0.5f;
				Easing.Data data = new Easing.Data(this.xScroll, num);
				float maxTime = 0.3f * Mathf.Sqrt(Mathf.Abs(this.xScroll - num));
				this.centerEasing = new Easing.Holder(new Easing.Function(Easing.InOutQuad), data, maxTime);
				this.state = ScrollBook.State.AUTOSCROLL_CENTER;
			}
		}
		else
		{
			this.scrollVelocity = 0f;
		}
		Card topmost = this.getTopmost(c.cardIndex);
		this.listener.onCardClicked(topmost, c.cardIndex, c.g);
	}

	// Token: 0x06000D8B RID: 3467 RVA: 0x0000ACFE File Offset: 0x00008EFE
	private Card getTopmost(int cardIndex)
	{
		return this.currentLayer().getTopmost(cardIndex);
	}

	// Token: 0x06000D8C RID: 3468 RVA: 0x0005EE40 File Offset: 0x0005D040
	private void onCardDoubleClicked(int index, ScrollBook.C c)
	{
		Card topmost = this.getTopmost(c.cardIndex);
		this.listener.onCardDoubleClicked(topmost, c.cardIndex, c.g);
	}

	// Token: 0x06000D8D RID: 3469 RVA: 0x0005EE74 File Offset: 0x0005D074
	private void onStartDragCard(ScrollBook.C c)
	{
		Card topmost = this.getTopmost(c.cardIndex);
		this.listener.onStartDragCard(topmost, c.cardIndex, c.g);
	}

	// Token: 0x06000D8E RID: 3470 RVA: 0x0005EEA8 File Offset: 0x0005D0A8
	private void onCardHeld(ScrollBook.C c)
	{
		this.state = ScrollBook.State.CARDDRAG;
		Card topmost = this.getTopmost(c.cardIndex);
		this.listener.onCardHeld(topmost, c.cardIndex, c.g);
	}

	// Token: 0x06000D8F RID: 3471 RVA: 0x0005EEE4 File Offset: 0x0005D0E4
	public Card popCard(int cardIndex)
	{
		List<Card> list = this.popCards(cardIndex, 1);
		if (list == null || list.Count == 0)
		{
			return null;
		}
		return list[0];
	}

	// Token: 0x06000D90 RID: 3472 RVA: 0x0000AD0C File Offset: 0x00008F0C
	public int getNumPiles(int layerId)
	{
		return this.layers[layerId].piles.Count;
	}

	// Token: 0x06000D91 RID: 3473 RVA: 0x0000AD24 File Offset: 0x00008F24
	public int getNumPiles()
	{
		return this.getNumPiles(this.currentLayerIndex);
	}

	// Token: 0x06000D92 RID: 3474 RVA: 0x0000AD32 File Offset: 0x00008F32
	public List<Card> getCards(int cardIndex)
	{
		return this.piles[cardIndex].cards;
	}

	// Token: 0x06000D93 RID: 3475 RVA: 0x0000AD45 File Offset: 0x00008F45
	public List<Card> popCards(int cardIndex)
	{
		return this.popCards(cardIndex, 0);
	}

	// Token: 0x06000D94 RID: 3476 RVA: 0x0005EF14 File Offset: 0x0005D114
	public List<Card> popCards(int cardIndex, int maxCount)
	{
		if (!this.inRange(cardIndex))
		{
			return null;
		}
		if (maxCount == 0)
		{
			return null;
		}
		ScrollBook.CardPile cardPile = this.piles[cardIndex];
		if (maxCount > cardPile.cards.Count || maxCount <= 0)
		{
			maxCount = cardPile.cards.Count;
		}
		List<Card> range = cardPile.cards.GetRange(0, maxCount);
		cardPile.cards.RemoveRange(0, maxCount);
		if (cardPile.cards.Count == 0)
		{
			this.piles.RemoveAt(cardIndex);
		}
		return range;
	}

	// Token: 0x06000D95 RID: 3477 RVA: 0x0000AD4F File Offset: 0x00008F4F
	private bool inRange(int cardIndex)
	{
		return this.currentLayer().inRange(cardIndex);
	}

	// Token: 0x06000D96 RID: 3478 RVA: 0x0005EFA4 File Offset: 0x0005D1A4
	public List<ScrollBook.VisibleCardData> getVisibleCardsData()
	{
		List<ScrollBook.VisibleCardData> list = new List<ScrollBook.VisibleCardData>();
		for (int i = 0; i < this.numCards; i++)
		{
			ScrollBook.C c = this.currentLayer().views[i];
			if (!c.hidden && this.inRange(c.cardIndex))
			{
				list.Add(new ScrollBook.VisibleCardData(c.cardIndex, i == this.centerIndex, this.getCards(c.cardIndex).Count, c.g.transform.position));
			}
		}
		return list;
	}

	// Token: 0x06000D97 RID: 3479 RVA: 0x0000AD5D File Offset: 0x00008F5D
	private IEnumerator fadeCards(int layerIndex, float a, float aPerSecond, Func<float, bool> whilePredicate)
	{
		return this.fadeCards(layerIndex, a, aPerSecond, whilePredicate, 0f);
	}

	// Token: 0x06000D98 RID: 3480 RVA: 0x0005F040 File Offset: 0x0005D240
	private IEnumerator fadeCards(int layerIndex, float a, float aPerSecond, Func<float, bool> whilePredicate, float inSeconds)
	{
		if (inSeconds > 0f)
		{
			yield return new WaitForSeconds(inSeconds);
		}
		while (whilePredicate.Invoke(a))
		{
			a = Mathf.Clamp01(a + Time.deltaTime * aPerSecond);
			for (int i = 0; i < this.layers[layerIndex].views.Count; i++)
			{
				this.layers[layerIndex].setAlpha(i, a);
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	// Token: 0x06000D99 RID: 3481 RVA: 0x0005F0A8 File Offset: 0x0005D2A8
	private IEnumerator setExpansionState(int newState)
	{
		this.expansionState = newState;
		yield return null;
		yield break;
	}

	// Token: 0x06000D9A RID: 3482 RVA: 0x0005F0D4 File Offset: 0x0005D2D4
	private void scrollToType(Card card)
	{
		int num = (int)this.xScroll;
		if (this.inRange(num) && this.currentLayer().stacks(num, card))
		{
			this.setScroll((float)num + 0.5f);
			return;
		}
		int cardIndexForType = this.currentLayer().getCardIndexForType(card);
		if (cardIndexForType >= 0)
		{
			this.setScroll((float)cardIndexForType + 0.5f);
		}
	}

	// Token: 0x06000D9B RID: 3483 RVA: 0x0005F138 File Offset: 0x0005D338
	private IEnumerator contractCards()
	{
		this.T = 1f;
		this._ttype = -1;
		yield return new WaitForSeconds(0.25f);
		Card card = (this.piles.Count <= 0) ? null : this.getTopmost(0);
		this.setLayer(this.currentLayerIndex - 1);
		this.T = 1f;
		this._ttype = -1;
		base.StartCoroutine(this.fadeCards(1, 1f, -10000f, (float a) => a > 0f, 0.25f));
		IEnumerator[] array = new IEnumerator[2];
		array[0] = this.fadeCards(0, 0f, 5f, (float a) => a < 1f);
		array[1] = this.setExpansionState(0);
		base.StartCoroutine(EnumeratorUtil.chain(array));
		if (card != null)
		{
			this.scrollToType(card);
		}
		this.scrollVelocity = 0f;
		yield break;
	}

	// Token: 0x06000D9C RID: 3484 RVA: 0x00004AAC File Offset: 0x00002CAC
	private static bool _stackByType(Card a, Card b, int numStacked)
	{
		return true;
	}

	// Token: 0x06000D9D RID: 3485 RVA: 0x000059E4 File Offset: 0x00003BE4
	private static bool _stackNoStacking(Card a, Card b, int numStacked)
	{
		return false;
	}

	// Token: 0x06000D9E RID: 3486 RVA: 0x0000AD6F File Offset: 0x00008F6F
	private static bool _stackByTypeAndLevel(Card a, Card b, int numStacked)
	{
		return a.level == b.level;
	}

	// Token: 0x06000D9F RID: 3487 RVA: 0x0000AD7F File Offset: 0x00008F7F
	private static bool _stackByTradable(Card a, Card b, int numStacked)
	{
		return a.tradable == b.tradable;
	}

	// Token: 0x04000A57 RID: 2647
	private const bool useColliders = true;

	// Token: 0x04000A58 RID: 2648
	private const int NumLayers = 2;

	// Token: 0x04000A59 RID: 2649
	private const float MaxScale = 1.1f;

	// Token: 0x04000A5A RID: 2650
	private const int CardWidth = 104;

	// Token: 0x04000A5B RID: 2651
	private const int CardHeight = 168;

	// Token: 0x04000A5C RID: 2652
	private const float CFriction = 0.95f;

	// Token: 0x04000A5D RID: 2653
	private const float CAcceleration = 15f;

	// Token: 0x04000A5E RID: 2654
	private const float CMinDragWithoutSnap = 0.01f;

	// Token: 0x04000A5F RID: 2655
	private const float CClickTime = 0.2f;

	// Token: 0x04000A60 RID: 2656
	private const float CDoubleClickTime = 0.4f;

	// Token: 0x04000A61 RID: 2657
	private const float CFinalizeScrollThreshold = 0.01f;

	// Token: 0x04000A62 RID: 2658
	public const float CHoldTime = 0.4f;

	// Token: 0x04000A63 RID: 2659
	public const int LayerBookStart = 12;

	// Token: 0x04000A64 RID: 2660
	private const float AlphaPerSecond = 5f;

	// Token: 0x04000A65 RID: 2661
	private ScrollBook.IListener listener = ScrollBook.defaultListener;

	// Token: 0x04000A66 RID: 2662
	private List<ScrollBook.BookLayerState> layers = new List<ScrollBook.BookLayerState>();

	// Token: 0x04000A67 RID: 2663
	private ScrollBook.BookLayerState layerBase;

	// Token: 0x04000A68 RID: 2664
	private List<ScrollBook.CardPile> piles;

	// Token: 0x04000A69 RID: 2665
	private int currentLayerIndex;

	// Token: 0x04000A6A RID: 2666
	private float xScroll;

	// Token: 0x04000A6B RID: 2667
	private Gui3D gui;

	// Token: 0x04000A6C RID: 2668
	private int numCards;

	// Token: 0x04000A6D RID: 2669
	private float margin;

	// Token: 0x04000A6E RID: 2670
	private int centerIndex;

	// Token: 0x04000A6F RID: 2671
	private Timer timer = new Timer(120f);

	// Token: 0x04000A70 RID: 2672
	private float cardScale = 1f;

	// Token: 0x04000A71 RID: 2673
	private Rect boundingBox;

	// Token: 0x04000A72 RID: 2674
	private Rect boundingBoxInput;

	// Token: 0x04000A73 RID: 2675
	private int lastCenterCardIndex = -1;

	// Token: 0x04000A74 RID: 2676
	private bool isInited;

	// Token: 0x04000A75 RID: 2677
	private bool inputEnabled = true;

	// Token: 0x04000A76 RID: 2678
	private float scrollVelocity;

	// Token: 0x04000A77 RID: 2679
	private float T = 1f;

	// Token: 0x04000A78 RID: 2680
	private int _ttype;

	// Token: 0x04000A79 RID: 2681
	private float _lastScroll;

	// Token: 0x04000A7A RID: 2682
	private Card expansionCard;

	// Token: 0x04000A7B RID: 2683
	private ScrollBook.State state;

	// Token: 0x04000A7C RID: 2684
	private int expansionState;

	// Token: 0x04000A7D RID: 2685
	private Vector2 startDrag = default(Vector2);

	// Token: 0x04000A7E RID: 2686
	private Vector2 lastDrag = default(Vector2);

	// Token: 0x04000A7F RID: 2687
	private MultiClickCheck startDragTime = new MultiClickCheck(0.4f, 2);

	// Token: 0x04000A80 RID: 2688
	private float dragDistance;

	// Token: 0x04000A81 RID: 2689
	private Easing.Holder centerEasing;

	// Token: 0x04000A82 RID: 2690
	private ScrollBook.C currentCardDragged;

	// Token: 0x04000A83 RID: 2691
	private float autoDirection;

	// Token: 0x04000A84 RID: 2692
	private CircularFloatBuffer cbDeltaX = new CircularFloatBuffer(20);

	// Token: 0x04000A85 RID: 2693
	private static ScrollBook.IListener defaultListener = new ScrollBook.EmptyListener();

	// Token: 0x020001A9 RID: 425
	private class C
	{
		// Token: 0x06000DA2 RID: 3490 RVA: 0x0000ADA3 File Offset: 0x00008FA3
		public C(GameObject gameObject, GameObject pileView)
		{
			this.g = gameObject;
			this.pileView = pileView;
		}

		// Token: 0x04000A88 RID: 2696
		public GameObject g;

		// Token: 0x04000A89 RID: 2697
		public GameObject pileView;

		// Token: 0x04000A8A RID: 2698
		public int cardIndex;

		// Token: 0x04000A8B RID: 2699
		public bool hidden;

		// Token: 0x04000A8C RID: 2700
		public Rect rect;
	}

	// Token: 0x020001AA RID: 426
	private class CardPile
	{
		// Token: 0x04000A8D RID: 2701
		public List<Card> cards = new List<Card>();

		// Token: 0x04000A8E RID: 2702
		public float scale = 1f;
	}

	// Token: 0x020001AB RID: 427
	private class BookLayerState
	{
		// Token: 0x06000DA4 RID: 3492 RVA: 0x0005F154 File Offset: 0x0005D354
		public BookLayerState(int layerIndex)
		{
			this.layerIndex = layerIndex;
			this.stackByFunction = ((layerIndex != 0) ? new ScrollBook.SameStackFunction(ScrollBook._stackNoStacking) : new ScrollBook.SameStackFunction(ScrollBook._stackByType));
		}

		// Token: 0x06000DA5 RID: 3493 RVA: 0x0005F1B0 File Offset: 0x0005D3B0
		public void setAlpha(int vi, float a)
		{
			this.views[vi].g.GetComponent<CardView>().setTransparency(a);
			this.views[vi].pileView.renderer.material.color = new Color(1f, 1f, 1f, a);
		}

		// Token: 0x06000DA6 RID: 3494 RVA: 0x0000ADD7 File Offset: 0x00008FD7
		public bool inRange(int cardIndex)
		{
			return cardIndex >= 0 && cardIndex < this.piles.Count;
		}

		// Token: 0x06000DA7 RID: 3495 RVA: 0x0005F210 File Offset: 0x0005D410
		public int getCardIndexForType(Card type)
		{
			for (int i = 0; i < this.piles.Count; i++)
			{
				if (this.stacks(i, type))
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06000DA8 RID: 3496 RVA: 0x0000ADF1 File Offset: 0x00008FF1
		public Card getTopmost(int cardIndex)
		{
			return this.piles[cardIndex].cards[0];
		}

		// Token: 0x06000DA9 RID: 3497 RVA: 0x0000AE0A File Offset: 0x0000900A
		public void setStackByFunction(ScrollBook.SameStackFunction f)
		{
			this.stackByFunction = f;
		}

		// Token: 0x06000DAA RID: 3498 RVA: 0x0000AE13 File Offset: 0x00009013
		public bool stacks(int cardIndex, Card c)
		{
			return this.stacks(this.getTopmost(cardIndex), c, 0);
		}

		// Token: 0x06000DAB RID: 3499 RVA: 0x0000AE24 File Offset: 0x00009024
		public bool stacks(Card a, Card b, int numStacked)
		{
			return a.getType() == b.getType() && this.stackByFunction(a, b, numStacked);
		}

		// Token: 0x04000A8F RID: 2703
		public float savedXScroll;

		// Token: 0x04000A90 RID: 2704
		public int layerIndex;

		// Token: 0x04000A91 RID: 2705
		public List<ScrollBook.CardPile> piles = new List<ScrollBook.CardPile>();

		// Token: 0x04000A92 RID: 2706
		public List<ScrollBook.C> views = new List<ScrollBook.C>();

		// Token: 0x04000A93 RID: 2707
		private ScrollBook.SameStackFunction stackByFunction;
	}

	// Token: 0x020001AC RID: 428
	public enum State
	{
		// Token: 0x04000A95 RID: 2709
		IDLE,
		// Token: 0x04000A96 RID: 2710
		PENDING_DECISION,
		// Token: 0x04000A97 RID: 2711
		DRAGGING,
		// Token: 0x04000A98 RID: 2712
		AUTOSCROLL,
		// Token: 0x04000A99 RID: 2713
		AUTOSCROLL_FINALIZE,
		// Token: 0x04000A9A RID: 2714
		AUTOSCROLL_CENTER,
		// Token: 0x04000A9B RID: 2715
		CARDDRAG
	}

	// Token: 0x020001AD RID: 429
	public class VisibleCardData
	{
		// Token: 0x06000DAC RID: 3500 RVA: 0x0000AE47 File Offset: 0x00009047
		public VisibleCardData(int cardIndex, bool center, int count, Vector3 pos)
		{
			this.cardIndex = cardIndex;
			this.isCenter = center;
			this.count = count;
			this.pos = pos;
		}

		// Token: 0x04000A9C RID: 2716
		public int cardIndex;

		// Token: 0x04000A9D RID: 2717
		public int count;

		// Token: 0x04000A9E RID: 2718
		public Vector3 pos;

		// Token: 0x04000A9F RID: 2719
		public bool isCenter;
	}

	// Token: 0x020001AE RID: 430
	private class _IndexFinder
	{
		// Token: 0x06000DAD RID: 3501 RVA: 0x0000AE6C File Offset: 0x0000906C
		public _IndexFinder(ScrollBook book, ScrollBook.BookLayerState layer)
		{
			this._book = book;
			this._layer = layer;
		}

		// Token: 0x06000DAE RID: 3502 RVA: 0x0005F24C File Offset: 0x0005D44C
		public ScrollBook._IndexFinder push(int index)
		{
			this._lastIndex = ((!this._layer.inRange(index)) ? -1 : index);
			if (this._lastIndex >= 0)
			{
				this._card = this._layer.piles[index].cards[0];
			}
			return this;
		}

		// Token: 0x06000DAF RID: 3503 RVA: 0x0000AE89 File Offset: 0x00009089
		public void popIfNearby()
		{
			this.pop(this._lastIndex - 1, 3);
		}

		// Token: 0x06000DB0 RID: 3504 RVA: 0x0000AE9A File Offset: 0x0000909A
		public void pop()
		{
			this.pop(0, this._layer.piles.Count);
		}

		// Token: 0x06000DB1 RID: 3505 RVA: 0x0005F2A8 File Offset: 0x0005D4A8
		public void pop(int index, int count)
		{
			if (this._lastIndex < 0)
			{
				return;
			}
			index = Mth.clamp(index, 0, this._layer.piles.Count);
			count = Mth.clamp(count, 0, this._layer.piles.Count - index);
			for (int i = index; i < index + count; i++)
			{
				if (this._layer.stacks(i, this._card))
				{
					int num = i - this._lastIndex;
					this._book.scrollTo(this._book.scrollPos() + (float)num);
					this._layer.piles[i].scale = 1.1f;
					break;
				}
			}
		}

		// Token: 0x04000AA0 RID: 2720
		private ScrollBook _book;

		// Token: 0x04000AA1 RID: 2721
		private ScrollBook.BookLayerState _layer;

		// Token: 0x04000AA2 RID: 2722
		private Card _card;

		// Token: 0x04000AA3 RID: 2723
		private int _lastIndex = -1;
	}

	// Token: 0x020001AF RID: 431
	public interface IListener
	{
		// Token: 0x06000DB2 RID: 3506
		void onCardEnterView(Card card, ICardView view);

		// Token: 0x06000DB3 RID: 3507
		void onStartDragCard(Card card, int cardIndex, GameObject g);

		// Token: 0x06000DB4 RID: 3508
		void onCardHeld(Card card, int cardIndex, GameObject g);

		// Token: 0x06000DB5 RID: 3509
		void onCardClicked(Card card, int cardIndex, GameObject g);

		// Token: 0x06000DB6 RID: 3510
		void onCardDoubleClicked(Card card, int cardIndex, GameObject g);
	}

	// Token: 0x020001B0 RID: 432
	private class EmptyListener : ScrollBook.IListener
	{
		// Token: 0x06000DB8 RID: 3512 RVA: 0x000028DF File Offset: 0x00000ADF
		public void onCardEnterView(Card card, ICardView view)
		{
		}

		// Token: 0x06000DB9 RID: 3513 RVA: 0x000028DF File Offset: 0x00000ADF
		public void onStartDragCard(Card card, int cardIndex, GameObject g)
		{
		}

		// Token: 0x06000DBA RID: 3514 RVA: 0x000028DF File Offset: 0x00000ADF
		public void onCardHeld(Card card, int cardIndex, GameObject g)
		{
		}

		// Token: 0x06000DBB RID: 3515 RVA: 0x000028DF File Offset: 0x00000ADF
		public void onCardClicked(Card card, int cardIndex, GameObject g)
		{
		}

		// Token: 0x06000DBC RID: 3516 RVA: 0x000028DF File Offset: 0x00000ADF
		public void onCardDoubleClicked(Card card, int cardIndex, GameObject g)
		{
		}
	}

	// Token: 0x020001B1 RID: 433
	// (Invoke) Token: 0x06000DBE RID: 3518
	public delegate bool SameStackFunction(Card a, Card b, int numStacked);
}
