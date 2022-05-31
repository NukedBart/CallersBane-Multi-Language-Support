using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x02000417 RID: 1047
public class TradeSystem : MonoBehaviour, ICardListCallback, iCardRule
{
	// Token: 0x0600172A RID: 5930 RVA: 0x00010AD3 File Offset: 0x0000ECD3
	public void SetTradeRoomName(string name)
	{
		this.tradeRoomName = name;
	}

	// Token: 0x0600172B RID: 5931 RVA: 0x00010ADC File Offset: 0x0000ECDC
	public string GetTradeRoomName()
	{
		return this.tradeRoomName;
	}

	// Token: 0x0600172C RID: 5932 RVA: 0x0008F4F8 File Offset: 0x0008D6F8
	private void Start()
	{
		this.tradeSkin = (GUISkin)ResourceManager.Load("_GUISkins/TradeSystem");
		this.tradeSkinClose = (GUISkin)ResourceManager.Load("_GUISkins/TradeSystemCloseButton");
		this.lobbySkin = (GUISkin)ResourceManager.Load("_GUISkins/Lobby");
		this.outerFrame1 = new ScrollsFrame(this.outerArea1).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.TOP | NinePatch.Patches.TOP_RIGHT | NinePatch.Patches.CENTER | NinePatch.Patches.RIGHT | NinePatch.Patches.BOTTOM | NinePatch.Patches.BOTTOM_RIGHT).AddNinePatch(ScrollsFrame.Border.DARK_SHARP, NinePatch.Patches.TOP_LEFT | NinePatch.Patches.LEFT | NinePatch.Patches.CENTER | NinePatch.Patches.BOTTOM_LEFT);
		this.outerFrame2 = new ScrollsFrame(this.outerArea2).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.TOP_LEFT | NinePatch.Patches.TOP | NinePatch.Patches.LEFT | NinePatch.Patches.CENTER | NinePatch.Patches.BOTTOM_LEFT | NinePatch.Patches.BOTTOM).AddNinePatch(ScrollsFrame.Border.DARK_SHARP, NinePatch.Patches.TOP_RIGHT | NinePatch.Patches.CENTER | NinePatch.Patches.RIGHT | NinePatch.Patches.BOTTOM_RIGHT);
	}

	// Token: 0x0600172D RID: 5933 RVA: 0x0008F590 File Offset: 0x0008D790
	private void OnGUI()
	{
		if (this.state != TradeSystem.ETradeState.TRADE_INACTIVE)
		{
			GUI.depth = 16;
			GUI.skin = this.tradeSkin;
			GUI.skin.textField.fontSize = (int)((float)Screen.height * 0.027f);
			GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.GetOpacity());
			this.outerFrame1.Draw();
			this.outerFrame2.Draw();
			TradeSystem.PlayerTradeStatus[] array = new TradeSystem.PlayerTradeStatus[]
			{
				this.p1,
				this.p2
			};
			foreach (TradeSystem.PlayerTradeStatus playerTradeStatus in array)
			{
				Rect rect = (playerTradeStatus != this.p1) ? this.rectOfferP2 : this.rectOfferP1;
				float x = rect.x;
				GUI.skin.label.fontSize = 32;
				GUI.Label(new Rect(((playerTradeStatus != this.p1) ? rect.xMax : rect.x) - rect.width / 2f, rect.y - (float)Screen.height * 0.055f, rect.width, 0.06f * (float)Screen.height), playerTradeStatus.name);
				if (playerTradeStatus.offer.Count == 0)
				{
					Color textColor = GUI.skin.label.normal.textColor;
					GUI.skin.label.normal.textColor = new Color(0.7f, 0.6f, 0.5f);
					GUI.skin.label.fontSize = 26;
					GUI.Label(new Rect(x, rect.y + rect.height * 0.4f, rect.width, rect.height * 0.1f), "No scrolls on offer");
					GUI.skin.label.normal.textColor = textColor;
				}
				GUI.skin.label.fontSize = 22;
				GUI.DrawTexture(new Rect(rect.x + (float)Screen.height * 0.05f, rect.yMax - 0.03f * (float)Screen.height, (float)Screen.height * 0.03f, (float)Screen.height * 0.03f), ResourceManager.LoadTexture("Shared/gold_icon"));
				if (playerTradeStatus == this.p2)
				{
					GUI.enabled = false;
				}
				Rect rect2;
				rect2..ctor(rect.x + (float)Screen.width * 0.05f, rect.yMax - 0.038f * (float)Screen.height, (float)Screen.width * 0.05f, 0.04f * (float)Screen.height);
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.GetOpacity() * 0.5f);
				GUI.Box(rect2, string.Empty);
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.GetOpacity());
				playerTradeStatus.goldOfferString = GUI.TextField(rect2, playerTradeStatus.goldOfferString);
				if (playerTradeStatus == this.p2)
				{
					GUI.enabled = true;
				}
				GUI.skin = this.tradeSkinClose;
				if (GUI.Button(new Rect((float)Screen.width * 0.94f, (float)Screen.height * 0.15f, (float)Screen.height * 0.04f, (float)Screen.height * 0.04f), string.Empty))
				{
					App.Communicator.send(new TradeCancelMessage());
				}
			}
			this.p1.GoldOfferStringFilter(this.maxGoldP1);
			this.p1.UpdateGoldOfferString(true);
			if (this.goldOfferP1Last != this.p1.goldOffer)
			{
				this.SendP1GoldOffer();
				this.goldOfferP1Last = this.p1.goldOffer;
			}
		}
	}

	// Token: 0x0600172E RID: 5934 RVA: 0x0008F9B8 File Offset: 0x0008DBB8
	private void UpdateTradeViewPositions()
	{
		this.clOfferP1.SetOpacity(this.GetOpacity());
		this.clOfferP2.SetOpacity(this.GetOpacity());
		this.clInventoryP1.SetOpacity(this.GetOpacity());
		this.clInventoryP2.SetOpacity(this.GetOpacity());
	}

	// Token: 0x0600172F RID: 5935 RVA: 0x0008FA0C File Offset: 0x0008DC0C
	public void Init(float x, float y, float w, float h, RenderTexture cardRenderTexture)
	{
		this.p1 = new TradeSystem.PlayerTradeStatus(this.sorter);
		this.p2 = new TradeSystem.PlayerTradeStatus(this.sorter);
		this.goldOfferP1Last = 0;
		float num = (float)Screen.height * 0.02f;
		this.outerArea1 = new Rect(x * (float)Screen.width, y * (float)Screen.height, w * (float)Screen.width / 2f, h * (float)Screen.height);
		this.outerArea2 = new Rect(x * (float)Screen.width + w * (float)Screen.width / 2f, y * (float)Screen.height, w * (float)Screen.width / 2f, h * (float)Screen.height);
		this.innerArea = new Rect(x * (float)Screen.width + num, y * (float)Screen.height + num, w * (float)Screen.width - 2f * num, h * (float)Screen.height - 2f * num);
		float num2 = 5f;
		float num3 = 0.04f * (float)Screen.height;
		this.rectWidth = this.innerArea.width / 4f - 10f;
		this.rectInvP1 = new Rect(this.innerArea.x + num2, this.innerArea.y + num2 + num3, this.rectWidth - num2, this.innerArea.height - num2 * 2f - num3 + (float)Screen.height * 0.05f);
		this.rectOfferP1 = new Rect(this.rectInvP1.xMax + num2, this.rectInvP1.y, this.rectInvP1.width, this.rectInvP1.height - 0.05f * (float)Screen.height);
		this.rectInvP2 = new Rect(this.innerArea.xMax - this.rectWidth - num2, this.innerArea.y + num2 + num3, this.rectWidth - num2, this.innerArea.height - num2 * 2f - num3 + (float)Screen.height * 0.05f);
		this.rectOfferP2 = new Rect(this.rectInvP2.x - this.rectInvP2.width - num2, this.rectInvP2.y, this.rectInvP2.width, this.rectInvP2.height - 0.05f * (float)Screen.height);
		this.clInventoryP1 = new GameObject("Card List / Inventory P1").AddComponent<CardListPopup>();
		this.clInventoryP1.transform.parent = base.transform;
		this.clInventoryP1.Init(this.rectInvP1, false, false, this.p1.inventory, this, null, null, true, true, false, false, ResourceManager.LoadTexture("ChatUI/buttonicon_add"), true);
		this.clInventoryP1.enabled = false;
		this.clInventoryP1.SetPersistence(App.Config.settings.cardlist.trade_p1);
		this.clOfferP1 = new GameObject("Card List / Offer P1").AddComponent<CardListPopup>();
		this.clOfferP1.transform.parent = base.transform;
		this.clOfferP1.Init(this.rectOfferP1, false, false, this.p1.offer, this, null, new GUIContent("Accept"), true, true, false, true, ResourceManager.LoadTexture("ChatUI/buttonicon_remove"), true);
		this.clOfferP1.SetUseLockedButton(false);
		this.clOfferP1.enabled = false;
		this.clOfferP2 = new GameObject("Card List / Offer P2").AddComponent<CardListPopup>();
		this.clOfferP2.transform.parent = base.transform;
		this.clOfferP2.Init(this.rectOfferP2, false, false, this.p2.offer, this, null, new GUIContent("Accept"), true, false, false, true, null, true);
		this.clOfferP2.SetUseLockedButton(false);
		this.clOfferP2.enabled = false;
		this.clInventoryP2 = new GameObject("Card List / Inventory P2").AddComponent<CardListPopup>();
		this.clInventoryP2.transform.parent = base.transform;
		this.clInventoryP2.Init(this.rectInvP2, false, false, this.p2.inventory, this, null, null, false, true, false, false, null, true);
		this.clInventoryP2.SetPersistence(App.Config.settings.cardlist.trade_p2);
		this.clInventoryP2.enabled = false;
		this.cardOverlay = new GameObject("Card Overlay").AddComponent<CardOverlay>();
		this.cardOverlay.Init(cardRenderTexture, 5);
	}

	// Token: 0x06001730 RID: 5936 RVA: 0x0008FE90 File Offset: 0x0008E090
	private IEnumerator MoveIn()
	{
		this.isMoving = true;
		yield return new WaitForSeconds(0.2f);
		float speed = 6.5f;
		while (Mathf.Abs(0.005f - this.tradePos) > 0.005f)
		{
			this.tradePos += (0.005f - this.tradePos) * speed * Mathf.Min(Time.fixedDeltaTime, 1f / speed);
			this.UpdateTradeViewPositions();
			yield return null;
		}
		this.isMoving = false;
		yield break;
	}

	// Token: 0x06001731 RID: 5937 RVA: 0x0008FEAC File Offset: 0x0008E0AC
	private IEnumerator MoveOut()
	{
		this.isMoving = true;
		float speed = 6.5f;
		while (Mathf.Abs(-0.4f - this.tradePos) > 0.005f)
		{
			this.tradePos += (-0.4f - this.tradePos) * speed * Mathf.Min(Time.fixedDeltaTime, 1f / speed);
			this.UpdateTradeViewPositions();
			yield return null;
		}
		this.isMoving = false;
		this.state = TradeSystem.ETradeState.TRADE_INACTIVE;
		this.ShowRelevantPopups();
		yield break;
	}

	// Token: 0x06001732 RID: 5938 RVA: 0x00010AE4 File Offset: 0x0000ECE4
	public void AddP1CardsToInventory()
	{
		this.p1.inventory.AddRange(this.p1.allCards);
	}

	// Token: 0x06001733 RID: 5939 RVA: 0x00010B01 File Offset: 0x0000ED01
	public void AddP2CardsToInventory()
	{
		this.p2.inventory.AddRange(this.p2.allCards);
	}

	// Token: 0x06001734 RID: 5940 RVA: 0x0008FEC8 File Offset: 0x0008E0C8
	public void StartTrade(List<Card> cardsPlayer1, List<Card> cardsPlayer2, string nameP1, string nameP2, int maxGoldP1)
	{
		this.tradeRoomName = string.Empty;
		this.maxGoldP1 = maxGoldP1;
		this.p1.Reset();
		this.p2.Reset();
		this.p1.SetName(nameP1);
		this.p2.SetName(nameP2);
		this.p1.SetCards(cardsPlayer1);
		this.p2.SetCards(cardsPlayer2);
		this.state = TradeSystem.ETradeState.TRADE_MAIN;
		this.UpdateTradeViewPositions();
		this.ShowRelevantPopups();
		base.StopCoroutine("MoveIn");
		base.StopCoroutine("MoveOut");
		base.StartCoroutine("MoveIn");
	}

	// Token: 0x06001735 RID: 5941 RVA: 0x0008FF64 File Offset: 0x0008E164
	public void UpdateView(bool tradeModified, TradeInfo player1, TradeInfo player2)
	{
		if (tradeModified)
		{
			this.clOfferP1.SetButtonEnabled(ECardListButton.BUTTON_RIGHT, false);
			base.StopCoroutine("EnableAcceptButtonSoon");
			base.StartCoroutine("EnableAcceptButtonSoon");
		}
		this.p2.goldOffer = player2.gold;
		this.p2.UpdateGoldOfferString(false);
		this.p1.accepted = player1.accepted;
		this.p2.accepted = player2.accepted;
		this.p1.ClearLists();
		this.p2.ClearLists();
		foreach (Card card in this.p1.allCards)
		{
			bool flag = false;
			foreach (long num in player1.cardIds)
			{
				if (card.getId() == num)
				{
					this.p1.offer.Add(card);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.p1.inventory.Add(card);
			}
		}
		foreach (Card card2 in this.p2.allCards)
		{
			bool flag2 = false;
			foreach (long num2 in player2.cardIds)
			{
				if (card2.getId() == num2)
				{
					this.p2.offer.Add(card2);
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				this.p2.inventory.Add(card2);
			}
		}
		this.p1.SortLists();
		this.p2.SortLists();
		this.clOfferP1.SetButtonHighlighted(ECardListButton.BUTTON_RIGHT, this.p1.accepted);
		this.clOfferP2.SetButtonHighlighted(ECardListButton.BUTTON_RIGHT, this.p2.accepted);
		this.clOfferP1.SetButtonContent(ECardListButton.BUTTON_RIGHT, (!this.p1.accepted) ? new GUIContent("   Accept") : new GUIContent("   Accepted"));
		this.clOfferP2.SetButtonContent(ECardListButton.BUTTON_RIGHT, (!this.p2.accepted) ? new GUIContent("   Waiting") : new GUIContent("   Accepted"));
	}

	// Token: 0x06001736 RID: 5942 RVA: 0x00010B1E File Offset: 0x0000ED1E
	public void CloseTrade()
	{
		base.StopCoroutine("MoveIn");
		base.StopCoroutine("MoveOut");
		base.StartCoroutine("MoveOut");
		this.tradeRoomName = string.Empty;
		this.cardOverlay.Hide();
	}

	// Token: 0x06001737 RID: 5943 RVA: 0x00010B58 File Offset: 0x0000ED58
	public bool IsInTrade()
	{
		return this.state != TradeSystem.ETradeState.TRADE_INACTIVE;
	}

	// Token: 0x06001738 RID: 5944 RVA: 0x00010B66 File Offset: 0x0000ED66
	public bool IsTradeMoving()
	{
		return this.isMoving;
	}

	// Token: 0x06001739 RID: 5945 RVA: 0x00010B6E File Offset: 0x0000ED6E
	public float GetOffX()
	{
		return this.tradePos * (float)Screen.width;
	}

	// Token: 0x0600173A RID: 5946 RVA: 0x00010B7D File Offset: 0x0000ED7D
	public float GetOpacity()
	{
		return 1f - this.tradePos / -0.4f;
	}

	// Token: 0x0600173B RID: 5947 RVA: 0x00090204 File Offset: 0x0008E404
	public void ButtonClicked(CardListPopup popup, ECardListButton button)
	{
		if (this.isMoving)
		{
			return;
		}
		if (popup == this.clOfferP1 && button == ECardListButton.BUTTON_RIGHT)
		{
			if (!this.p1.accepted)
			{
				App.Communicator.send(new TradeAcceptBargainMessage());
			}
			else
			{
				App.Communicator.send(new TradeUnacceptBargainMessage());
			}
		}
		if (!(popup == this.clOfferP2) || button == ECardListButton.BUTTON_RIGHT)
		{
		}
		this.ShowRelevantPopups();
	}

	// Token: 0x0600173C RID: 5948 RVA: 0x00010B91 File Offset: 0x0000ED91
	public void ButtonClicked(CardListPopup popup, ECardListButton button, List<Card> selectedCards)
	{
		if (this.isMoving)
		{
			return;
		}
		if (popup == this.clInventoryP1 && button == ECardListButton.BUTTON_LEFT)
		{
			selectedCards.Clear();
			this.state = TradeSystem.ETradeState.TRADE_MAIN;
		}
		this.ShowRelevantPopups();
	}

	// Token: 0x0600173D RID: 5949 RVA: 0x00090288 File Offset: 0x0008E488
	public void ItemButtonClicked(CardListPopup popup, Card card)
	{
		if (this.isMoving)
		{
			return;
		}
		if (popup == this.clOfferP1)
		{
			App.Communicator.send(new TradeRemoveCardMessage(card.getId()));
		}
		else if (popup == this.clInventoryP1)
		{
			App.Communicator.send(new TradeAddCardsMessage(new long[]
			{
				card.getId()
			}));
		}
	}

	// Token: 0x0600173E RID: 5950 RVA: 0x000028DF File Offset: 0x00000ADF
	public void ItemHovered(CardListPopup popup, Card card)
	{
	}

	// Token: 0x0600173F RID: 5951 RVA: 0x00010BC9 File Offset: 0x0000EDC9
	public void ItemClicked(CardListPopup popup, Card card)
	{
		this.cardOverlay.Show(card);
	}

	// Token: 0x06001740 RID: 5952 RVA: 0x000028DF File Offset: 0x00000ADF
	public void HideCardView()
	{
	}

	// Token: 0x06001741 RID: 5953 RVA: 0x000028DF File Offset: 0x00000ADF
	public void ActivateTriggeredAbility(string id, TilePosition pos)
	{
	}

	// Token: 0x06001742 RID: 5954 RVA: 0x00090300 File Offset: 0x0008E500
	private void ShowRelevantPopups()
	{
		if (this.state == TradeSystem.ETradeState.TRADE_INACTIVE)
		{
			this.clInventoryP1.enabled = false;
			this.clInventoryP2.enabled = false;
			this.clOfferP1.enabled = false;
			this.clOfferP2.enabled = false;
		}
		else
		{
			this.clOfferP1.enabled = true;
			this.clOfferP2.enabled = true;
			this.clInventoryP1.enabled = true;
			this.clInventoryP2.enabled = true;
		}
	}

	// Token: 0x06001743 RID: 5955 RVA: 0x00010BD7 File Offset: 0x0000EDD7
	private void SendP1GoldOffer()
	{
		App.Communicator.send(new TradeSetGoldMessage(this.p1.goldOffer));
	}

	// Token: 0x06001744 RID: 5956 RVA: 0x00090380 File Offset: 0x0008E580
	private IEnumerator EnableAcceptButtonSoon()
	{
		yield return new WaitForSeconds(5.5f);
		this.clOfferP1.SetButtonEnabled(ECardListButton.BUTTON_RIGHT, true);
		yield break;
	}

	// Token: 0x0400148E RID: 5262
	private const float TRADE_ACCEPT_TIME_SECONDS = 5f;

	// Token: 0x0400148F RID: 5263
	private const float tradePosOffScreen = -0.4f;

	// Token: 0x04001490 RID: 5264
	private TradeSystem.PlayerTradeStatus p1;

	// Token: 0x04001491 RID: 5265
	private TradeSystem.PlayerTradeStatus p2;

	// Token: 0x04001492 RID: 5266
	private CardListPopup clInventoryP1;

	// Token: 0x04001493 RID: 5267
	private CardListPopup clOfferP1;

	// Token: 0x04001494 RID: 5268
	private CardListPopup clInventoryP2;

	// Token: 0x04001495 RID: 5269
	private CardListPopup clOfferP2;

	// Token: 0x04001496 RID: 5270
	private TradeSystem.ETradeState state;

	// Token: 0x04001497 RID: 5271
	private DeckSorter sorter = new DeckSorter().byColor().byName().byLevel();

	// Token: 0x04001498 RID: 5272
	private GUISkin tradeSkin;

	// Token: 0x04001499 RID: 5273
	private GUISkin tradeSkinClose;

	// Token: 0x0400149A RID: 5274
	private GUISkin lobbySkin;

	// Token: 0x0400149B RID: 5275
	private Rect outerArea1;

	// Token: 0x0400149C RID: 5276
	private Rect outerArea2;

	// Token: 0x0400149D RID: 5277
	private Rect innerArea;

	// Token: 0x0400149E RID: 5278
	private int maxGoldP1;

	// Token: 0x0400149F RID: 5279
	private int goldOfferP1Last;

	// Token: 0x040014A0 RID: 5280
	private string tradeRoomName;

	// Token: 0x040014A1 RID: 5281
	private Dictionary<CardListPopup, CardView> cardViews = new Dictionary<CardListPopup, CardView>();

	// Token: 0x040014A2 RID: 5282
	private float tradePos = -0.4f;

	// Token: 0x040014A3 RID: 5283
	private bool isMoving;

	// Token: 0x040014A4 RID: 5284
	private Rect rectInvP1;

	// Token: 0x040014A5 RID: 5285
	private Rect rectOfferP1;

	// Token: 0x040014A6 RID: 5286
	private Rect rectInvP2;

	// Token: 0x040014A7 RID: 5287
	private Rect rectOfferP2;

	// Token: 0x040014A8 RID: 5288
	private float rectWidth;

	// Token: 0x040014A9 RID: 5289
	private ScrollsFrame outerFrame1;

	// Token: 0x040014AA RID: 5290
	private ScrollsFrame outerFrame2;

	// Token: 0x040014AB RID: 5291
	private CardOverlay cardOverlay;

	// Token: 0x02000418 RID: 1048
	private enum ETradeState
	{
		// Token: 0x040014AD RID: 5293
		TRADE_INACTIVE,
		// Token: 0x040014AE RID: 5294
		TRADE_MAIN,
		// Token: 0x040014AF RID: 5295
		TRADE_ADD_CARDS,
		// Token: 0x040014B0 RID: 5296
		TRADE_VIEW_OTHER_PLAYER
	}

	// Token: 0x02000419 RID: 1049
	private class PlayerTradeStatus
	{
		// Token: 0x06001745 RID: 5957 RVA: 0x00010BF4 File Offset: 0x0000EDF4
		public PlayerTradeStatus(DeckSorter sorter)
		{
			this.sorter = sorter;
			this.inventory = new EList<Card>();
			this.offer = new EList<Card>();
			this.allCards = new List<Card>();
			this.Reset();
		}

		// Token: 0x06001746 RID: 5958 RVA: 0x00010C2A File Offset: 0x0000EE2A
		public void Reset()
		{
			this.ClearLists();
			this.name = string.Empty;
			this.goldOffer = 0;
			this.goldOfferString = "0";
			this.accepted = false;
		}

		// Token: 0x06001747 RID: 5959 RVA: 0x00010C56 File Offset: 0x0000EE56
		public void SetCards(List<Card> cards)
		{
			this.allCards = cards;
		}

		// Token: 0x06001748 RID: 5960 RVA: 0x00010C5F File Offset: 0x0000EE5F
		public void SetName(string name)
		{
			this.name = name;
		}

		// Token: 0x06001749 RID: 5961 RVA: 0x00010C68 File Offset: 0x0000EE68
		public void ClearLists()
		{
			this.inventory.Clear();
			this.offer.Clear();
		}

		// Token: 0x0600174A RID: 5962 RVA: 0x00010C80 File Offset: 0x0000EE80
		public void SortLists()
		{
			this.inventory.Sort(this.sorter);
			this.offer.Sort(this.sorter);
		}

		// Token: 0x0600174B RID: 5963 RVA: 0x0009039C File Offset: 0x0008E59C
		public void GoldOfferStringFilter(int maxGold)
		{
			this.goldOfferString = Regex.Replace(this.goldOfferString, "[^0-9]", string.Empty);
			if (int.TryParse(this.goldOfferString, ref this.goldOffer))
			{
				if (this.goldOffer > maxGold)
				{
					this.goldOffer = maxGold;
				}
			}
			else
			{
				this.goldOffer = 0;
			}
		}

		// Token: 0x0600174C RID: 5964 RVA: 0x00010CA4 File Offset: 0x0000EEA4
		public void UpdateGoldOfferString(bool hideZero)
		{
			if (hideZero && this.goldOffer == 0)
			{
				this.goldOfferString = string.Empty;
			}
			else
			{
				this.goldOfferString = string.Empty + this.goldOffer;
			}
		}

		// Token: 0x040014B1 RID: 5297
		private DeckSorter sorter;

		// Token: 0x040014B2 RID: 5298
		public string name;

		// Token: 0x040014B3 RID: 5299
		public List<Card> allCards;

		// Token: 0x040014B4 RID: 5300
		public EList<Card> inventory;

		// Token: 0x040014B5 RID: 5301
		public EList<Card> offer;

		// Token: 0x040014B6 RID: 5302
		public int goldOffer;

		// Token: 0x040014B7 RID: 5303
		public string goldOfferString;

		// Token: 0x040014B8 RID: 5304
		public bool accepted;
	}
}
