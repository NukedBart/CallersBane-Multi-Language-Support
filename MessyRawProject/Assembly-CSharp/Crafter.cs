using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Irrelevant.Assets;
using UnityEngine;

// Token: 0x02000178 RID: 376
public class Crafter : DeckBuilder2, iEffect
{
	// Token: 0x06000B9D RID: 2973 RVA: 0x00052EA4 File Offset: 0x000510A4
	private new IEnumerator Start()
	{
		this.loadSaveTableState = false;
		yield return base.StartCoroutine(base.Start());
		base.name = "_Crafter";
		this.gui3d.setDepth(950f);
		this.gBackdrop = PrimitiveFactory.createTexturedPlane("DeckBuilder/bg", false);
		this.gui3d.DrawObject(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), this.gBackdrop);
		this.gui3d.setDepth(945f);
		Vector3 cardSize = this.getTableCardScale() * 10f;
		float scale = 1.8f;
		cardSize.x *= scale;
		cardSize.z *= scale * 391f / 512f;
		for (int i = 0; i < 3; i++)
		{
			float xs = this.rectLeft.center.x + (float)(i - 1) * 0.95f * cardSize.x;
			float ys = this.mock.Y(1120f);
			if (i == 1)
			{
				Crafter.CraftingSlot slot = this.addSlot(i, xs, ys - this.mock.Y(20f), cardSize);
				slot.isResultSlot = true;
				slot.descAlpha = 1f;
				this.slots.Insert(0, slot);
			}
			else
			{
				this.slots.Add(this.addSlot(i, xs, ys, cardSize));
			}
		}
		this.plaqueItemSkin = (GUISkin)ResourceManager.Load("_GUISkins/Plaque");
		GUIStyle label = this.plaqueItemSkin.label;
		int fontSize = (int)((float)Screen.height / 24f);
		this.plaqueItemSkin.button.fontSize = fontSize;
		label.fontSize = fontSize;
		this.emptyStyle = new GUIStyle();
		this.emptyStyle.normal.textColor = this.UpgradeButtonTextColor;
		this.emptyStyle.font = this.plaqueItemSkin.font;
		this.emptyStyle.alignment = 4;
		this.emptyStyle.fontSize = (int)((float)Screen.height / 36f);
		this.emptyStyleShadow = new GUIStyle(this.emptyStyle);
		this.emptyStyleShadow.normal.textColor = Color.black;
		EffectPlayer.preload("BuyEffect/Scroll_appear_3a_appear");
		EffectPlayer.preload("BuyEffect/Scroll_appear_3b_rimshine");
		this.inited = true;
		yield break;
	}

	// Token: 0x06000B9E RID: 2974 RVA: 0x00052EC0 File Offset: 0x000510C0
	private Crafter.CraftingSlot addSlot(int index, float x, float y, Vector3 cardSize)
	{
		GameObject go = PrimitiveFactory.createTexturedPlane("Crafting/slot" + index + "_512", true);
		Crafter.CraftingSlot craftingSlot = new Crafter.CraftingSlot(index, this.gui3d.getPosition(x, y).pos);
		Rect rect;
		rect..ctor(x - cardSize.x / 2f, y - cardSize.z / 1.95f, cardSize.x, cardSize.z);
		craftingSlot.descRect = GeomUtil.scaleCentered(rect, 0.5f, 0.25f);
		this.gui3d.DrawObject(rect, go);
		return craftingSlot;
	}

	// Token: 0x06000B9F RID: 2975 RVA: 0x00052F60 File Offset: 0x00051160
	private new void FixedUpdate()
	{
		base.FixedUpdate();
		foreach (Crafter.CraftingSlot craftingSlot in this.slots)
		{
			if (!craftingSlot.isEmpty() || base.isDraggedCardsOverlapping(craftingSlot.descRect))
			{
				craftingSlot.descAlpha -= 0.2f;
			}
			else
			{
				craftingSlot.descAlpha += 0.1f;
			}
			craftingSlot.descAlpha = Mth.clamp(craftingSlot.descAlpha, 0f, 1f);
		}
	}

	// Token: 0x06000BA0 RID: 2976 RVA: 0x000099A1 File Offset: 0x00007BA1
	protected override Vector3 getTableCardScale()
	{
		return CardView.CardLocalScale(Camera.main.orthographicSize / 4f * 1.65f);
	}

	// Token: 0x06000BA1 RID: 2977 RVA: 0x0005301C File Offset: 0x0005121C
	private string getCardTypeName()
	{
		foreach (Crafter.CraftingSlot craftingSlot in this.slots)
		{
			if (!craftingSlot.isEmpty())
			{
				return craftingSlot.card.getCardType().name;
			}
		}
		return string.Empty;
	}

	// Token: 0x06000BA2 RID: 2978 RVA: 0x00053098 File Offset: 0x00051298
	protected override void OnGUI_drawTopbarSubmenu()
	{
		if (!this.inited)
		{
			return;
		}
		GUI.DrawTexture(this.subMenuRect, ResourceManager.LoadTexture("ChatUI/menu_bar_sub"));
		GUIPositioner subMenuPositioner = App.LobbyMenu.getSubMenuPositioner(1f, 4);
		int fontSize = GUI.skin.label.fontSize;
		GUIStyle label = GUI.skin.label;
		int fontSize2 = Screen.height / 40;
		GUI.skin.button.fontSize = fontSize2;
		label.fontSize = fontSize2;
		if (LobbyMenu.drawButton(subMenuPositioner.getButtonRect(0f), "Clear Table"))
		{
			this.clearTable();
		}
		float x = GeomUtil.v3tov2(DeckBuilder2.worldToCamera(this.getResultSlot().pos)).x;
		Rect rect = this.mock2.rAspectH(0f, 0f, 340f, 80f);
		rect.center = new Vector2(x, (float)Screen.height * 0.95f - rect.height);
		GUI.DrawTexture(rect, ResourceManager.LoadTexture("BattleUI/battlegui_resourcebox"));
		GUI.enabled = UpgradeCardMessage.verifyCards(this.getSlotCards());
		if (LobbyMenu.drawButton(rect, new GUIContent("Upgrade").lockDemo(), this.emptyStyleShadow, this.emptyStyle))
		{
			this.initiateUpgrade();
		}
		GUI.enabled = true;
		string cardTypeName = this.getCardTypeName();
		if (this.getResultSlot().descAlpha > 0f)
		{
			Color color = GUI.color;
			GUI.color = ColorUtil.GetWithAlpha(GUI.color, this.getResultSlot().descAlpha);
			Rect rect2;
			rect2..ctor(0f, 0f, (float)Screen.width, (float)Screen.height * 0.3f);
			rect2.center = GeomUtil.v3tov2(DeckBuilder2.worldToCamera(this.getResultSlot().pos));
			int fontSize3 = (int)((float)Screen.height * 0.04f);
			string text = (!string.IsNullOrEmpty(cardTypeName)) ? cardTypeName : "scroll";
			GUIUtil.drawShadowText(rect2, "Upgrade\n" + text, this.UpgradeTextColor, fontSize3);
			GUI.color = color;
		}
		foreach (Crafter.CraftingSlot craftingSlot in this.sacrificeSlots())
		{
			if (craftingSlot.descAlpha > 0f)
			{
				Color color2 = GUI.color;
				GUI.color = ColorUtil.GetWithAlpha(GUI.color, craftingSlot.descAlpha);
				Rect rect3;
				rect3..ctor(0f, 0f, (float)Screen.width, (float)Screen.height * 0.3f);
				rect3.center = GeomUtil.v3tov2(DeckBuilder2.worldToCamera(craftingSlot.pos));
				int fontSize4 = (int)((float)Screen.height * 0.028f);
				string text2 = (!string.IsNullOrEmpty(cardTypeName)) ? cardTypeName : "scroll";
				GUIUtil.drawShadowText(rect3, "Sacrifice\n" + text2, this.SacrificeTextColor, fontSize4);
				GUI.color = color2;
			}
		}
		GUIStyle label2 = GUI.skin.label;
		fontSize2 = fontSize;
		GUI.skin.button.fontSize = fontSize2;
		label2.fontSize = fontSize2;
	}

	// Token: 0x06000BA3 RID: 2979 RVA: 0x000028DF File Offset: 0x00000ADF
	protected override void OnGUI_drawTableGUI()
	{
	}

	// Token: 0x06000BA4 RID: 2980 RVA: 0x000533D8 File Offset: 0x000515D8
	protected override void OnGUI_draw3D()
	{
		this.gui3d.setDepth(940f);
		new ScrollsFrame(this.rectLeft).SetGui(this.gui3d).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.CENTER).Draw();
		this.gui3d.setDepth(0.5f);
		new ScrollsFrame(this.rectRight).SetGui(this.gui3d).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.CENTER).Draw();
	}

	// Token: 0x06000BA5 RID: 2981 RVA: 0x000099BE File Offset: 0x00007BBE
	protected override float capSearchWidth()
	{
		if (AspectRatio.now.isNarrower(AspectRatio._3_2))
		{
			return this.mock2.X(900f);
		}
		return base.capSearchWidth();
	}

	// Token: 0x06000BA6 RID: 2982 RVA: 0x000099EB File Offset: 0x00007BEB
	protected override void putCardsOnTable(List<DeckCard> cards)
	{
		cards.Reverse();
		base.putCardsOnTable(cards);
	}

	// Token: 0x06000BA7 RID: 2983 RVA: 0x000099FA File Offset: 0x00007BFA
	protected override int allowStartDraggingCardCount(Card card)
	{
		if (this._isCraftAnimationRunning)
		{
			return 0;
		}
		return Math.Max(1, base.allowStartDraggingCardCount(card));
	}

	// Token: 0x06000BA8 RID: 2984 RVA: 0x0005344C File Offset: 0x0005164C
	private bool hasCardTypeOnTable(CardType cardType)
	{
		foreach (DeckCard deckCard in this.tableCards)
		{
			if (deckCard.card.getCardType().id == cardType.id)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000BA9 RID: 2985 RVA: 0x000534C4 File Offset: 0x000516C4
	protected override Hashtable getDropOnTableMoveToTween(ICardView card)
	{
		if (this.tableCards.Count == 0)
		{
			return this._placeInClosestEmptySlot(card);
		}
		if (!this.hasCardTypeOnTable(card.getCardType()))
		{
			foreach (Crafter.CraftingSlot craftingSlot in this.slots)
			{
				if (!craftingSlot.isEmpty())
				{
					this.onLeaveSlot(craftingSlot);
					base.removeCardFromTable(craftingSlot.card.getCardInfo());
					craftingSlot.card = null;
				}
			}
			return this._placeInClosestEmptySlot(card);
		}
		Crafter.CraftingSlot craftingSlot2 = this.findClosestSlot(card.getTransform().position);
		if (craftingSlot2.isEmpty() || !this.allowReplacingCardsOfSameType)
		{
			return this._placeInSlot(card, this.findClosestEmptySlot(card.getTransform().position));
		}
		Crafter.CraftingSlot craftingSlot3 = this.findDragSourceSlot(card);
		if (craftingSlot3 != null)
		{
			ICardView card2 = craftingSlot2.card;
			this.onLeaveSlot(craftingSlot2);
			craftingSlot2.card = null;
			bool flag = card.getTransform().position.y < card2.getTransform().position.y;
			this._placeInSlot(card2, craftingSlot3, 0.2f);
			Vector3 pos = craftingSlot3.pos;
			pos.z = this.currentTableCardZ;
			Vector3 vector = pos;
			vector.x = Mathf.Lerp(card2.getTransform().position.x, pos.x, 0.5f);
			vector.y += (float)Screen.height * ((!flag) ? -0.02f : 0.03f);
			Vector3 vector2 = pos;
			vector2.x = Mathf.Lerp(card2.getTransform().position.x, pos.x, 0.1f);
			vector2.y += 0.5f * (float)Screen.height * ((!flag) ? -0.02f : 0.03f);
			Hashtable args = iTween.Hash(new object[]
			{
				"path",
				new Vector3[]
				{
					vector2,
					vector,
					pos
				},
				"time",
				0.35f,
				"easetype",
				iTween.EaseType.easeOutCubic
			});
			iTween.MoveTo(card2.getTransform().gameObject, args);
			return this._placeInSlot(card, craftingSlot2);
		}
		this.onLeaveSlot(craftingSlot2);
		base.removeCardFromTable(craftingSlot2.card.getCardInfo());
		craftingSlot2.card = null;
		return this._placeInSlot(card, craftingSlot2);
	}

	// Token: 0x06000BAA RID: 2986 RVA: 0x000537A4 File Offset: 0x000519A4
	private Hashtable _placeInClosestEmptySlot(ICardView card)
	{
		Crafter.CraftingSlot craftingSlot = this.findClosestEmptySlot(card.getTransform().position);
		if (craftingSlot == null)
		{
			return base.getDropOnTableMoveToTween(card);
		}
		return this._placeInSlot(card, craftingSlot);
	}

	// Token: 0x06000BAB RID: 2987 RVA: 0x00009A16 File Offset: 0x00007C16
	private Hashtable _placeInSlot(ICardView card, Crafter.CraftingSlot slot)
	{
		return this._placeInSlot(card, slot, 0.1f);
	}

	// Token: 0x06000BAC RID: 2988 RVA: 0x000537DC File Offset: 0x000519DC
	private Hashtable _placeInSlot(ICardView card, Crafter.CraftingSlot slot, float time)
	{
		if (slot == null)
		{
			throw new InvalidOperationException("Slot is null");
		}
		if (!slot.isEmpty())
		{
			throw new InvalidOperationException("Slot is occupied! ");
		}
		slot.card = card;
		this.onEnterSlot(slot);
		Vector3 pos = slot.pos;
		pos.z = this.currentTableCardZ;
		return iTween.Hash(new object[]
		{
			"position",
			pos,
			"time",
			time,
			"easetype",
			iTween.EaseType.easeOutCubic
		});
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x00053870 File Offset: 0x00051A70
	private Crafter.CraftingSlot findClosestSlot(Vector3 pos, bool onlyEmpty)
	{
		float num = float.MaxValue;
		Crafter.CraftingSlot result = null;
		foreach (Crafter.CraftingSlot craftingSlot in this.slots)
		{
			if (!onlyEmpty || craftingSlot.isEmpty())
			{
				float sqrMagnitude = (pos - craftingSlot.pos).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = craftingSlot;
				}
			}
		}
		return result;
	}

	// Token: 0x06000BAE RID: 2990 RVA: 0x00009A25 File Offset: 0x00007C25
	private Crafter.CraftingSlot findClosestSlot(Vector3 pos)
	{
		return this.findClosestSlot(pos, false);
	}

	// Token: 0x06000BAF RID: 2991 RVA: 0x00009A2F File Offset: 0x00007C2F
	private Crafter.CraftingSlot findClosestEmptySlot(Vector3 pos)
	{
		return this.findClosestSlot(pos, true);
	}

	// Token: 0x06000BB0 RID: 2992 RVA: 0x00053908 File Offset: 0x00051B08
	private Crafter.CraftingSlot findDragSourceSlot(ICardView card)
	{
		foreach (Crafter.CraftingSlot craftingSlot in this.slots)
		{
			if (craftingSlot.lastCard == card)
			{
				return craftingSlot;
			}
		}
		return null;
	}

	// Token: 0x06000BB1 RID: 2993 RVA: 0x00053974 File Offset: 0x00051B74
	protected override void beginDragCards(List<DeckCard> cards, bool sourceIsScrollBook)
	{
		if (this._isCraftAnimationRunning)
		{
			return;
		}
		foreach (DeckCard deckCard in cards)
		{
			foreach (Crafter.CraftingSlot craftingSlot in this.slots)
			{
				if (deckCard.card == craftingSlot.card)
				{
					this.onLeaveSlot(craftingSlot);
					craftingSlot.card = null;
				}
			}
		}
		this.allowReplacingCardsOfSameType = (cards.Count == 1);
		base.beginDragCards(cards, sourceIsScrollBook);
	}

	// Token: 0x06000BB2 RID: 2994 RVA: 0x00009A39 File Offset: 0x00007C39
	protected override bool allowAddCardToTable(Card card)
	{
		return base.getNumCardsOnTable(card.getType()) <= 3;
	}

	// Token: 0x06000BB3 RID: 2995 RVA: 0x000059E4 File Offset: 0x00003BE4
	protected override bool isFilterAffectingTable()
	{
		return false;
	}

	// Token: 0x06000BB4 RID: 2996 RVA: 0x00004AAC File Offset: 0x00002CAC
	protected override bool shouldAddCardToLibrary(Card card)
	{
		return true;
	}

	// Token: 0x06000BB5 RID: 2997 RVA: 0x00053A48 File Offset: 0x00051C48
	protected override void addCardToScrollBook(DeckCard card)
	{
		foreach (Crafter.CraftingSlot craftingSlot in this.slots)
		{
			if (craftingSlot.lastCard == card.card)
			{
				craftingSlot.lastCard = null;
			}
		}
		if (this.scrollBook.isExpanded() && this.tableCards.Count == 0)
		{
			this.scrollBook.contract();
		}
	}

	// Token: 0x06000BB6 RID: 2998 RVA: 0x00053AE0 File Offset: 0x00051CE0
	public override void clearTable()
	{
		foreach (Crafter.CraftingSlot craftingSlot in this.slots)
		{
			craftingSlot.card = null;
		}
		base.clearTable();
	}

	// Token: 0x06000BB7 RID: 2999 RVA: 0x00009A4D File Offset: 0x00007C4D
	public override void onCardEnterView(Card card, ICardView view)
	{
		view.setLocked(!card.tradable, false);
	}

	// Token: 0x06000BB8 RID: 3000 RVA: 0x00053B40 File Offset: 0x00051D40
	private IEnumerable<Card> getSlotCards()
	{
		return Enumerable.Select<Crafter.CraftingSlot, Card>(Enumerable.Where<Crafter.CraftingSlot>(this.slots, (Crafter.CraftingSlot s) => s.card != null), (Crafter.CraftingSlot s) => (s.card == null) ? null : s.card.getCardInfo());
	}

	// Token: 0x06000BB9 RID: 3001 RVA: 0x00053B98 File Offset: 0x00051D98
	private bool initiateUpgrade()
	{
		if (this.pendingUpgrade != null || this._isCraftAnimationRunning)
		{
			return false;
		}
		IEnumerable<Card> slotCards = this.getSlotCards();
		if (!UpgradeCardMessage.verifyCards(slotCards))
		{
			return false;
		}
		this.pendingUpgrade = new UpgradeCardMessage(Enumerable.ToArray<long>(Enumerable.Select<Card, long>(slotCards, (Card c) => c.getId())));
		App.Communicator.send(this.pendingUpgrade);
		return true;
	}

	// Token: 0x06000BBA RID: 3002 RVA: 0x00009A5F File Offset: 0x00007C5F
	private void upgrade()
	{
		base.StopCoroutine("upgradeStart");
		this.stopUpgrade();
		App.AudioScript.PlaySFX("Sounds/scroll_craft");
		base.StartCoroutine("upgradeStart");
	}

	// Token: 0x06000BBB RID: 3003 RVA: 0x00053C18 File Offset: 0x00051E18
	private Crafter.CraftingSlot getResultSlot()
	{
		foreach (Crafter.CraftingSlot craftingSlot in this.slots)
		{
			if (craftingSlot.isResultSlot)
			{
				return craftingSlot;
			}
		}
		return null;
	}

	// Token: 0x06000BBC RID: 3004 RVA: 0x00009A8D File Offset: 0x00007C8D
	private List<Crafter.CraftingSlot> sacrificeSlotsWithCards()
	{
		return Enumerable.ToList<Crafter.CraftingSlot>(Enumerable.Where<Crafter.CraftingSlot>(this.slots, (Crafter.CraftingSlot s) => !s.isResultSlot && s.card != null));
	}

	// Token: 0x06000BBD RID: 3005 RVA: 0x00009ABC File Offset: 0x00007CBC
	private List<Crafter.CraftingSlot> sacrificeSlots()
	{
		return Enumerable.ToList<Crafter.CraftingSlot>(Enumerable.Where<Crafter.CraftingSlot>(this.slots, (Crafter.CraftingSlot s) => !s.isResultSlot));
	}

	// Token: 0x06000BBE RID: 3006 RVA: 0x00053C80 File Offset: 0x00051E80
	private void stopUpgrade()
	{
		foreach (EffectPlayer effectPlayer in this.effectPlayers)
		{
			Object.Destroy(effectPlayer.gameObject);
		}
		this.effectPlayers.Clear();
		this._isCraftAnimationRunning = false;
	}

	// Token: 0x06000BBF RID: 3007 RVA: 0x00009AEB File Offset: 0x00007CEB
	public void handleMessage(OkMessage m)
	{
		if (m.isType(typeof(UpgradeCardMessage)) && this.pendingUpgrade != null)
		{
			this.upgrade();
			this.pendingUpgrade = null;
		}
		base.handleMessage(m);
	}

	// Token: 0x06000BC0 RID: 3008 RVA: 0x00009B21 File Offset: 0x00007D21
	public void handleMessage(FailMessage m)
	{
		if (m.isType(typeof(UpgradeCardMessage)))
		{
			this.pendingUpgrade = null;
			App.Popups.ShowOk(this, "upgrade-fail", "Upgrade failed", m.info, "Ok");
		}
	}

	// Token: 0x06000BC1 RID: 3009 RVA: 0x00053CF0 File Offset: 0x00051EF0
	private void playUpgradeResultEffect(string fn, float inSeconds)
	{
		GameObject gameObject = new GameObject();
		gameObject.AddComponent<MeshRenderer>();
		gameObject.name = "BuyEffect_" + fn;
		EffectPlayer effectPlayer = gameObject.AddComponent<EffectPlayer>();
		effectPlayer.setMaterialToUse(new Material(ResourceManager.LoadShader("Scrolls/StoreEffect/Unlit/Transparent")));
		float num = this.getTableCardScale().z * 0.78f;
		Vector3 baseScale = Vector3.one * num;
		effectPlayer.init(fn, 1, DefaultIEffectCallback.instance(), -1, baseScale, 0);
		effectPlayer.getAnimPlayer().waitForUpdate();
		effectPlayer.startInSeconds(inSeconds);
		gameObject.transform.localPosition = this.getResultSlot().pos + new Vector3(num * 0.05f, num * 0.3f, 1.2f);
	}

	// Token: 0x06000BC2 RID: 3010 RVA: 0x00003FDC File Offset: 0x000021DC
	public void effectAnimDone(EffectPlayer effect, bool loop)
	{
		DefaultIEffectCallback.instance().effectAnimDone(effect, loop);
	}

	// Token: 0x06000BC3 RID: 3011 RVA: 0x000028DF File Offset: 0x00000ADF
	public void locator(EffectPlayer effect, AnimLocator loc)
	{
	}

	// Token: 0x06000BC4 RID: 3012 RVA: 0x00053DB0 File Offset: 0x00051FB0
	private void onEnterSlot(Crafter.CraftingSlot slot)
	{
		slot.assertCard();
		slot.lastCard = slot.card;
		if (!this.scrollBook.isExpanded())
		{
			this.scrollBook.expand(slot.card.getCardInfo());
		}
		if (!slot.isResultSlot)
		{
			slot.card.renderAsEnabled(false, 0.3f);
		}
		if (!slot.card.getCardInfo().tradable)
		{
			slot.card.setLocked(!slot.isResultSlot, true);
		}
	}

	// Token: 0x06000BC5 RID: 3013 RVA: 0x00053E3C File Offset: 0x0005203C
	private void onLeaveSlot(Crafter.CraftingSlot slot)
	{
		slot.assertCard();
		if (!slot.isResultSlot)
		{
			slot.card.renderAsEnabled(true, 0.1f);
		}
		if (!slot.card.getCardInfo().tradable)
		{
			slot.card.setLocked(true, false);
		}
	}

	// Token: 0x06000BC6 RID: 3014 RVA: 0x00053E90 File Offset: 0x00052090
	private void removeSacrificedCards()
	{
		foreach (Crafter.CraftingSlot craftingSlot in this.sacrificeSlotsWithCards())
		{
			base.deleteCard(craftingSlot.card.getCardInfo());
			craftingSlot.card = (craftingSlot.lastCard = null);
		}
	}

	// Token: 0x06000BC7 RID: 3015 RVA: 0x00053F08 File Offset: 0x00052108
	private void upgradeCard()
	{
		ICardView card = this.getResultSlot().card;
		if (card == null)
		{
			return;
		}
		Card cardInfo = card.getCardInfo();
		cardInfo.upgrade();
		card.updateGraphics(cardInfo);
		card.renderAsEnabled(true, 0f);
		base.showCardRule(cardInfo);
	}

	// Token: 0x06000BC8 RID: 3016 RVA: 0x00053F50 File Offset: 0x00052150
	private IEnumerator upgradeStart()
	{
		EnumeratorUtil.FuncCaller funcCaller = new EnumeratorUtil.FuncCaller(EnumeratorUtil.Func);
		return EnumeratorUtil.chain(new IEnumerator[]
		{
			funcCaller(new Action(this.craftingSetRunning)),
			this.upgradePlayBurn(),
			funcCaller(new Action(this.removeSacrificedCards)),
			funcCaller(new Action(this.playShine)),
			this.upgradeFadeOut(),
			this.wait(0.5f),
			funcCaller(new Action(this.upgradeCard)),
			this.wait(2f),
			funcCaller(new Action(this.craftingSetNotRunning))
		});
	}

	// Token: 0x06000BC9 RID: 3017 RVA: 0x00009B5F File Offset: 0x00007D5F
	private void craftingSetRunning()
	{
		this._isCraftAnimationRunning = true;
	}

	// Token: 0x06000BCA RID: 3018 RVA: 0x00009B68 File Offset: 0x00007D68
	private void craftingSetNotRunning()
	{
		this._isCraftAnimationRunning = false;
	}

	// Token: 0x06000BCB RID: 3019 RVA: 0x00054010 File Offset: 0x00052210
	private void playShine()
	{
		int num = 0;
		this.playUpgradeResultEffect("BuyEffect/Scroll_appear_3a_appear", (float)num);
		string fn = "BuyEffect/Scroll_appear_3b_rimshine";
		ICardView card = this.getResultSlot().card;
		if (card != null && card.getCardInfo().getRarity() < 2)
		{
			fn = "BuyEffect/Scroll_appear_1_2b_rimshine";
		}
		this.playUpgradeResultEffect(fn, (float)num);
	}

	// Token: 0x06000BCC RID: 3020 RVA: 0x00054064 File Offset: 0x00052264
	private IEnumerator wait(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		yield break;
	}

	// Token: 0x06000BCD RID: 3021 RVA: 0x00054088 File Offset: 0x00052288
	private Texture getBurnTexture(int i)
	{
		i = Mth.clamp(i, 1, 15);
		string text = string.Concat(new object[]
		{
			"upgr_burn2__",
			(15 - i).ToString("D4"),
			"_",
			i
		});
		return ResourceManager.LoadTexture("Crafting/burn/" + text);
	}

	// Token: 0x06000BCE RID: 3022 RVA: 0x000540EC File Offset: 0x000522EC
	private IEnumerator upgradePlayBurn()
	{
		float t0 = Time.time;
		foreach (Crafter.CraftingSlot slot in this.sacrificeSlots())
		{
			Vector3 cs = this.getTableCardScale() * 10f;
			Vector3 pos = DeckBuilder2.worldToCamera(slot.pos);
			Rect rect = new Rect(0f, 0f, cs.x * 1.245f, cs.z * 1.13f);
			rect.center = new Vector2(pos.x + cs.x * 0.005f, pos.y);
			this.gui3d.DrawObject(rect, slot.burnAnim);
			slot.burnAnim.renderer.material.mainTexture = this.getBurnTexture(0);
			slot.burnAnim.renderer.material.color = Color.white;
			slot.burnAnim.renderer.enabled = true;
		}
		for (;;)
		{
			float floatFrame = 24f * (Time.time - t0);
			int rawFrame = (int)floatFrame;
			if (rawFrame > 15)
			{
				break;
			}
			foreach (Crafter.CraftingSlot slot2 in this.sacrificeSlots())
			{
				Texture tex = this.getBurnTexture(rawFrame);
				slot2.burnAnim.renderer.material.mainTexture = tex;
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
		yield break;
	}

	// Token: 0x06000BCF RID: 3023 RVA: 0x00054108 File Offset: 0x00052308
	private IEnumerator upgradeFadeOut()
	{
		float t0 = Time.time;
		float MaxTime = 0.5f;
		for (;;)
		{
			float t = (Time.time - t0) / MaxTime;
			if (t > 1f)
			{
				break;
			}
			Color color = new Color(1f, 1f, 1f, 1f - t);
			foreach (Crafter.CraftingSlot slot in this.sacrificeSlots())
			{
				slot.burnAnim.renderer.material.color = color;
			}
			yield return new WaitForEndOfFrame();
		}
		foreach (Crafter.CraftingSlot slot2 in this.sacrificeSlots())
		{
			slot2.burnAnim.renderer.enabled = false;
		}
		yield break;
	}

	// Token: 0x040008D5 RID: 2261
	private const float ZCraftGlow = 948f;

	// Token: 0x040008D6 RID: 2262
	private const float ZCraftBackground = 946f;

	// Token: 0x040008D7 RID: 2263
	private List<Crafter.CraftingSlot> slots = new List<Crafter.CraftingSlot>();

	// Token: 0x040008D8 RID: 2264
	private GameObject gBackdrop;

	// Token: 0x040008D9 RID: 2265
	private bool inited;

	// Token: 0x040008DA RID: 2266
	private MockupCalc mock = new MockupCalc(2023, 1536);

	// Token: 0x040008DB RID: 2267
	private MockupCalc mock2 = new MockupCalc(3200, 1800);

	// Token: 0x040008DC RID: 2268
	[SerializeField]
	private Color UpgradeTextColor = ColorUtil.FromInts(158, 129, 98);

	// Token: 0x040008DD RID: 2269
	[SerializeField]
	private Color SacrificeTextColor = ColorUtil.FromInts(141, 114, 97);

	// Token: 0x040008DE RID: 2270
	[SerializeField]
	private Color UpgradeButtonTextColor = ColorUtil.FromHex24(16774093u);

	// Token: 0x040008DF RID: 2271
	private GUISkin plaqueItemSkin;

	// Token: 0x040008E0 RID: 2272
	private GUIStyle emptyStyle;

	// Token: 0x040008E1 RID: 2273
	private GUIStyle emptyStyleShadow;

	// Token: 0x040008E2 RID: 2274
	private bool allowReplacingCardsOfSameType;

	// Token: 0x040008E3 RID: 2275
	private bool _isCraftAnimationRunning;

	// Token: 0x040008E4 RID: 2276
	private UpgradeCardMessage pendingUpgrade;

	// Token: 0x040008E5 RID: 2277
	private List<EffectPlayer> effectPlayers = new List<EffectPlayer>();

	// Token: 0x02000179 RID: 377
	private class CraftingSlot
	{
		// Token: 0x06000BD5 RID: 3029 RVA: 0x00054124 File Offset: 0x00052324
		public CraftingSlot(int index, Vector2 worldPos)
		{
			this.index = index;
			this.pos = worldPos;
			this.burnAnim = PrimitiveFactory.createPlane(false);
			this.burnAnim.renderer.material = new Material(ResourceManager.LoadShader("Transparent/Diffuse"));
			this.burnAnim.name = "burnAnim_" + index;
		}

		// Token: 0x06000BD6 RID: 3030 RVA: 0x00009BCC File Offset: 0x00007DCC
		public void assertCard()
		{
			if (this.card == null)
			{
				throw new InvalidOperationException("onEnterSlot: card is null");
			}
		}

		// Token: 0x06000BD7 RID: 3031 RVA: 0x00009BE4 File Offset: 0x00007DE4
		public bool isEmpty()
		{
			return this.card == null;
		}

		// Token: 0x040008EB RID: 2283
		public int index;

		// Token: 0x040008EC RID: 2284
		public Vector3 pos;

		// Token: 0x040008ED RID: 2285
		public ICardView card;

		// Token: 0x040008EE RID: 2286
		public ICardView lastCard;

		// Token: 0x040008EF RID: 2287
		public bool isResultSlot;

		// Token: 0x040008F0 RID: 2288
		public float descAlpha;

		// Token: 0x040008F1 RID: 2289
		public Rect descRect;

		// Token: 0x040008F2 RID: 2290
		public GameObject burnAnim;
	}
}
