using System;
using System.Collections;
using System.Collections.Generic;
using Gui;
using UnityEngine;

// Token: 0x020003FC RID: 1020
public class Marketplace : AbstractCommListener, ICardListCallback, IOkCallback, ICancelCallback, IOkCancelCallback, IOkStringCallback, IOkStringCancelCallback
{
	// Token: 0x06001665 RID: 5733 RVA: 0x00088384 File Offset: 0x00086584
	private void Start()
	{
		Log.info("BM: Start / listener added");
		App.Communicator.addListener(this);
		this.regularUI = (GUISkin)ResourceManager.Load("_GUISkins/RegularUI");
		this.buttonSkin = (GUISkin)ResourceManager.Load("_GUISkins/Lobby");
		App.ChatUI.Show(false);
		App.AudioScript.PlayMusic("Music/Store", AudioScript.EPostFadeoutBehaviour.PAUSE);
		base.StartCoroutine("RefreshBuyCoroutine");
		this.RefreshSellview();
		this.buttonIconAddEnabled = ResourceManager.LoadTexture("ChatUI/buttonicon_add");
		this.buttonIconAddDisabled = ResourceManager.LoadTexture("ChatUI/buttonicon_add_disabled");
		float num = (float)Screen.height * 0.025f;
		this.buyScrollsList = new GameObject("CL / BUY: All scrolls").AddComponent<CardListPopup>();
		this.buyScrollsList.Init(new Rect((float)(Screen.width / 2) - (float)Screen.height * 0.6f + num, (float)Screen.height * 0.28f, (float)Screen.height * 0.5f, (float)Screen.height * 0.63f), false, false, this.buyScrolls, this, null, null, false, false, false, false, null, true);
		this.buyScrollsList.SetCardDescriptionWriter(new CardListPopup.CardDescriptionWriter(this.BuyDescriptionBuilder));
		this.buyScrollsList.SetRightAdjustedCardDescriptionWriter(new CardListPopup.CardDescriptionWriter(this.CollectionCountDescriptionWriter));
		this.buyScrollsList.SetToggleText("Sold\nout");
		this.buyScrollsList.SetPersistence(App.Config.settings.cardlist.marketplace_buy);
		this.buyScrollsList.SetOpacity(1f);
		CardListPopup cardListPopup = this.buyScrollsList;
		CardListPopup.Button[] array = new CardListPopup.Button[3];
		array[0] = CardListPopup.SortByNameButton;
		array[1] = CardListPopup.SortByCostButton;
		array[2] = new CardListPopup.ButtonCycler().add(new CardListPopup.Button(new GUIContent("Price"), delegate(CardListPopup c)
		{
			c.SetSorter(new DeckSorter().byColor().byDataDesc<int>().byName());
		})).add(new CardListPopup.Button(new GUIContent("Price"), delegate(CardListPopup c)
		{
			c.SetSorter(new DeckSorter().byColor().byData<int>().byName());
		}));
		cardListPopup.AddButtons(array);
		this.libraryScrollsList = new GameObject("CL / SELL: Library scrolls").AddComponent<CardListPopup>();
		this.libraryScrollsList.Init(new Rect((float)(Screen.width / 2) - (float)Screen.height * 0.6f + num, (float)Screen.height * 0.28f, (float)Screen.height * 0.35f, (float)Screen.height * 0.63f), false, false, this.libraryScrolls, this, null, null, false, false, false, false, this.buttonIconAddEnabled, true);
		this.libraryScrollsList.SetOpacity(1f);
		this.libraryScrollsList.SetPersistence(App.Config.settings.cardlist.marketplace_library);
		this.libraryScrollsList.enabled = false;
		this.sellingScrollsList = new GameObject("CL / SELL: Selling scrolls").AddComponent<CardListPopup>();
		this.sellingScrollsList.Init(new Rect((float)(Screen.width / 2) - (float)Screen.height * 0.23f + num, (float)Screen.height * 0.28f, (float)Screen.height * 0.35f, (float)Screen.height * 0.63f), false, false, this.sellingScrolls, this, null, null, false, false, false, false, ResourceManager.LoadTexture("ChatUI/buttonicon_remove"), true);
		this.sellingScrollsList.SetUseLockedButton(false);
		this.sellingScrollsList.SetCardDescriptionWriter(new CardListPopup.CardDescriptionWriter(this.SellDescriptionBuilder));
		this.sellingScrollsList.SetOpacity(1f);
		this.sellingScrollsList.enabled = false;
		this.soldScrollsList = new GameObject("CL / SELL: Sold scrolls").AddComponent<CardListPopup>();
		this.soldScrollsList.Init(new Rect((float)(Screen.width / 2) + (float)Screen.height * 0.2f + num, (float)Screen.height * 0.28f, (float)Screen.height * 0.35f, (float)Screen.height * 0.63f), false, false, this.soldScrolls, this, null, null, false, false, false, false, ResourceManager.LoadTexture("Store/black_market_collect_gold"), true);
		this.soldScrollsList.SetCardDescriptionWriter(new CardListPopup.CardDescriptionWriter(this.SoldDescriptionBuilder));
		this.soldScrollsList.SetOpacity(1f);
		this.soldScrollsList.SetToggleText("Show\nclaimed");
		this.soldScrollsList.SetPersistence(App.Config.settings.cardlist.marketplace_sold);
		this.soldScrollsList.enabled = false;
		float num2 = this.buyScrollsList.getRect().xMax + (float)Screen.height * 0.08f;
		this.buyRightRectBox = new Rect(num2, (float)Screen.height * 0.28f, this.soldScrollsList.getRect().xMax - num2, (float)Screen.height * 0.63f);
		this.buyCardOverlay = new GameObject("Card Overlay, Buy").AddComponent<CardOverlay>();
		float num3 = (float)Screen.height * 0.6f;
		float num4 = num3;
		this.buyCardOverlayRect = new Rect(num2 - num4 * 0.25f, (float)Screen.height * 0.2f, num4, num4);
		this.buyCardOverlay.Init(this.cardRenderTexture, this.buyCardOverlayRect, 15);
		this.infoCardOverlay = new GameObject("Card Overlay, Info").AddComponent<CardOverlay>();
		this.infoCardOverlay.Init(this.cardRenderTexture, 15);
		App.LobbyMenu.fadeInScene();
		this.gui3d = new Gui3D(Camera.main);
		this.subMenuRect = App.LobbyMenu.getSubMenuRect(1f);
		this.tierDrop = new GameObject("Tier dropdown").AddComponent<Dropdown>();
		this.tierDrop.DropdownChangedEvent += this.DropdownChangedEvent;
		this.tierDrop.Init(new string[]
		{
			"Show all tiers",
			"Tier 1",
			"Tier 2",
			"Tier 3"
		}, 4.5f, true, false, 15);
		this.tierDrop.SetEnabled(true);
		this.tierDrop.SetSkin(this.regularUI);
		this.tierDrop.SetCenterY(true);
		App.InviteManager.clearInviteListTyped(Invite.InviteType.SOLD_MARKET_SCROLLS);
		App.Communicator.send(new RemoveMessageMessage(MessageMessage.Type.SOLD_MARKET_SCROLLS));
		if (App.SceneValues.marketplace != null)
		{
			Log.info("BM: HasSceneValue");
			if (App.SceneValues.marketplace.openSellView)
			{
				Log.info("BM: OpenSellView");
				this.tierDrop.SetEnabled(false);
				this.buyScrollsList.enabled = false;
				this.libraryScrollsList.enabled = true;
				this.sellingScrollsList.enabled = true;
				this.soldScrollsList.enabled = true;
				this.buyScrollsList.SetOpacity(0f);
				this.libraryScrollsList.SetOpacity(1f);
				this.sellingScrollsList.SetOpacity(1f);
				this.soldScrollsList.SetOpacity(1f);
				this.currentView = Marketplace.EMarketplaceView.SELL;
				this.RefreshSellview();
			}
			App.SceneValues.marketplace = null;
		}
	}

	// Token: 0x06001666 RID: 5734 RVA: 0x000102DF File Offset: 0x0000E4DF
	public override void OnDestroy()
	{
		Log.info("BM: Listener removed");
		base.OnDestroy();
		this.tierDrop.DropdownChangedEvent -= this.DropdownChangedEvent;
		base.StopCoroutine("RefreshBuyCoroutine");
	}

	// Token: 0x06001667 RID: 5735 RVA: 0x00088A70 File Offset: 0x00086C70
	private void DropdownChangedEvent(int selectedIndex, string selection)
	{
		if (selectedIndex == 0)
		{
			this.buySelectedLevel = default(byte?);
		}
		else
		{
			this.buySelectedLevel = new byte?((byte)(selectedIndex - 1));
		}
		this.RefreshBuyview();
	}

	// Token: 0x06001668 RID: 5736 RVA: 0x00010313 File Offset: 0x0000E513
	private string SellDescriptionBuilder(Card card)
	{
		return this.DescriptionBuilder(card, (!this.sellingOffers.ContainsKey(card)) ? -1 : this.sellingOffers[card].price);
	}

	// Token: 0x06001669 RID: 5737 RVA: 0x00010344 File Offset: 0x0000E544
	private string SoldDescriptionBuilder(Card card)
	{
		return this.DescriptionBuilder(card, (!this.soldScrollTransactions.ContainsKey(card)) ? -1 : this.soldScrollTransactions[card].sellPrice);
	}

	// Token: 0x0600166A RID: 5738 RVA: 0x00010375 File Offset: 0x0000E575
	private string BuyDescriptionBuilder(Card card)
	{
		return this.DescriptionBuilder(card, (!this.buyOffers.ContainsKey(card)) ? -1 : this.buyOffers[card].price);
	}

	// Token: 0x0600166B RID: 5739 RVA: 0x000103A6 File Offset: 0x0000E5A6
	private string CollectionCountDescriptionWriter(Card card)
	{
		return "<color=#64503b>You own </color><color=#e0c88b>" + this.typeCounter.Count(card.getType()) + "</color>";
	}

	// Token: 0x0600166C RID: 5740 RVA: 0x00088AAC File Offset: 0x00086CAC
	private string DescriptionBuilder(Card card, int price)
	{
		string text = string.Empty;
		if (price >= 0)
		{
			text = text + price + " gold";
		}
		if (card.level > 0)
		{
			this.separator(ref text);
			text = text + "Tier " + card.getTier();
		}
		this.separator(ref text);
		text += card.getRarityString();
		return text;
	}

	// Token: 0x0600166D RID: 5741 RVA: 0x000103CD File Offset: 0x0000E5CD
	private void separator(ref string s)
	{
		if (!string.IsNullOrEmpty(s))
		{
			s += " - ";
		}
	}

	// Token: 0x0600166E RID: 5742 RVA: 0x00088B1C File Offset: 0x00086D1C
	private IEnumerator RefreshBuyCoroutine()
	{
		for (;;)
		{
			this.RefreshBuyview();
			yield return new WaitForSeconds(30f);
		}
		yield break;
	}

	// Token: 0x0600166F RID: 5743 RVA: 0x00088B38 File Offset: 0x00086D38
	private void RefreshBuyview()
	{
		Communicator communicator = App.Communicator;
		byte? b = this.buySelectedLevel;
		communicator.send(new MarketplaceAvailableOffersListViewMessage((b == null) ? default(int?) : new int?((int)b.Value)));
	}

	// Token: 0x06001670 RID: 5744 RVA: 0x000103E9 File Offset: 0x0000E5E9
	private void RefreshSellview()
	{
		Log.info("BM: RefreshSellView");
		App.Communicator.send(new MarketplaceOffersViewMessage());
		App.Communicator.send(new MarketplaceSoldListViewMessage());
		App.Communicator.send(new LibraryViewMessage());
	}

	// Token: 0x06001671 RID: 5745 RVA: 0x00088B84 File Offset: 0x00086D84
	private void onLibraryUpdated()
	{
		this.typeCounter = new CollectionUtil.Counter<int>();
		foreach (Card card in this.libraryScrolls)
		{
			this.typeCounter.Add(card.getType());
		}
	}

	// Token: 0x06001672 RID: 5746 RVA: 0x00010425 File Offset: 0x0000E625
	private bool canSellMoreScrolls()
	{
		return this.sellingScrolls.Count < this.maxNumOffers;
	}

	// Token: 0x06001673 RID: 5747 RVA: 0x00088BF0 File Offset: 0x00086DF0
	private void setSellingOffers(MarketplaceOffer[] offers)
	{
		this.sellingScrolls.Clear();
		this.sellingOffers.Clear();
		foreach (MarketplaceOffer marketplaceOffer in offers)
		{
			this.sellingScrolls.Add(marketplaceOffer.card);
			this.sellingOffers.Add(marketplaceOffer.card, marketplaceOffer);
		}
		this.sellingScrolls.Sort(new DeckSorter().byColor().byName());
		this.libraryScrollsList.SetItemButtonTexture((!this.canSellMoreScrolls()) ? this.buttonIconAddDisabled : this.buttonIconAddEnabled);
	}

	// Token: 0x06001674 RID: 5748 RVA: 0x00088C94 File Offset: 0x00086E94
	public override void handleMessage(Message msg)
	{
		if (msg is LibraryViewMessage)
		{
			Log.info("BM: LibraryViewMessage");
			LibraryViewMessage libraryViewMessage = (LibraryViewMessage)msg;
			this.libraryScrolls.Clear();
			this.libraryScrolls.AddRange(libraryViewMessage.cards);
			this.libraryScrolls.Sort(new DeckSorter().byColor().byName());
			this.onLibraryUpdated();
		}
		else if (msg is MarketplaceOffersViewMessage)
		{
			Log.info("BM: MarketplaceOffersViewMessage");
			MarketplaceOffersViewMessage marketplaceOffersViewMessage = (MarketplaceOffersViewMessage)msg;
			this.maxNumOffers = marketplaceOffersViewMessage.maxNumOffers;
			this.setSellingOffers(marketplaceOffersViewMessage.offers);
		}
		else if (msg is MarketplaceSoldListViewMessage)
		{
			Log.info("BM: MarketplaceSoldListViewMessage");
			MarketplaceSoldListViewMessage marketplaceSoldListViewMessage = (MarketplaceSoldListViewMessage)msg;
			this.soldScrolls.Clear();
			this.soldScrollTransactions.Clear();
			foreach (TransactionInfo transactionInfo in marketplaceSoldListViewMessage.sold)
			{
				Card card = new Card((long)transactionInfo.cardId, CardTypeManager.getInstance().get(transactionInfo.cardType), !transactionInfo.claimed);
				card.level = (int)transactionInfo.level;
				this.soldScrolls.Add(card);
				this.soldScrollTransactions.Add(card, transactionInfo);
			}
			this.soldScrolls.Sort(new DeckSorter().byTradable());
		}
		else if (msg is OkMessage)
		{
			OkMessage okMessage = (OkMessage)msg;
			if (okMessage.isType(typeof(MarketplaceCreateOfferMessage)))
			{
				this.RefreshSellview();
			}
			if (okMessage.isType(typeof(MarketplaceMakeDealMessage)))
			{
				CardView cardView = this.buyCardOverlay.GetCardView();
				this.libraryScrolls.Add(cardView.getCardInfo());
				this.onLibraryUpdated();
				App.Popups.ShowOk(this, "dealmade", "Purchase complete!", cardView.getCardType().name + " has been added to your collection.", "Ok");
				this.RefreshBuyview();
				this.buyCardOverlay.Hide();
			}
			if (okMessage.isType(typeof(MarketplaceCancelOfferMessage)))
			{
				this.RefreshSellview();
			}
			if (okMessage.isType(typeof(MarketplaceClaimMessage)))
			{
				App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_coin_tally_end");
				CardType cardType = CardTypeManager.getInstance().get(this.transactionBeingClaimed.cardType);
				App.Popups.ShowOk(this, "claimgold", "Gold added", string.Concat(new object[]
				{
					"<color=#bbaa88>Tier ",
					(int)(this.transactionBeingClaimed.level + 1),
					" ",
					cardType.name,
					" sold for ",
					this.transactionBeingClaimed.sellPrice,
					" gold!\nEarned <color=#ffd055>",
					this.transactionBeingClaimed.sellPrice - this.transactionBeingClaimed.fee,
					" gold</color> (the fence collects ",
					this.transactionBeingClaimed.fee,
					").</color>"
				}), "Ok");
				this.RefreshSellview();
			}
		}
		else if (msg is FailMessage)
		{
			FailMessage failMessage = (FailMessage)msg;
			if (failMessage.isType(typeof(MarketplaceMakeDealMessage)))
			{
				App.Popups.ShowOk(this, "notenoughgold", "Purchase failed", failMessage.info, "Ok");
			}
			if (failMessage.isType(typeof(MarketplaceCreateOfferMessage)))
			{
				this.showCreateOfferFailed(failMessage.info);
			}
		}
		else if (msg is MarketplaceOffersSearchViewMessage)
		{
			Log.info("BM: MarketplaceOffersSearchViewMessage");
			MarketplaceOffersSearchViewMessage marketplaceOffersSearchViewMessage = (MarketplaceOffersSearchViewMessage)msg;
			this.buyCardOverlay.Show(marketplaceOffersSearchViewMessage.offer.card);
			this.buyCardOverlay.GetCardView().enableShowHistory();
			this.currentBuyOffer = marketplaceOffersSearchViewMessage;
		}
		else if (msg is MarketplaceAvailableOffersListViewMessage)
		{
			Log.info("BM: MarketplaceAvailableOffersListViewMessage");
			MarketplaceAvailableOffersListViewMessage marketplaceAvailableOffersListViewMessage = (MarketplaceAvailableOffersListViewMessage)msg;
			this.buyOffers.Clear();
			this.buyScrolls.Clear();
			List<Card> list = new List<Card>();
			foreach (CardType cardType2 in CardTypeManager.getInstance().getAll())
			{
				MarketplaceTypeAvailability marketplaceTypeAvailability = null;
				foreach (MarketplaceTypeAvailability marketplaceTypeAvailability2 in marketplaceAvailableOffersListViewMessage.available)
				{
					if (marketplaceTypeAvailability2.type == cardType2.id)
					{
						marketplaceTypeAvailability = marketplaceTypeAvailability2;
						break;
					}
				}
				Card card2;
				if (marketplaceTypeAvailability != null)
				{
					card2 = new Card(-1L, cardType2, true);
					card2.level = marketplaceTypeAvailability.level;
					card2.data = marketplaceTypeAvailability.price;
					this.buyOffers.Add(card2, marketplaceTypeAvailability);
				}
				else
				{
					card2 = new Card(-1L, cardType2, false);
					Card card3 = card2;
					byte? b = this.buySelectedLevel;
					card3.level = (int)((b == null) ? 0 : b.Value);
					card2.data = int.MaxValue;
				}
				list.Add(card2);
			}
			this.buyScrolls.AddRange(list);
			this.buyScrolls.Sort(new DeckSorter().byColor().byName());
			this.buyScrollsList.setOverridedCollectionForFiltering(this.libraryScrolls);
		}
		else if (msg is CheckCardDependenciesMessage)
		{
			CheckCardDependenciesMessage checkCardDependenciesMessage = (CheckCardDependenciesMessage)msg;
			if (checkCardDependenciesMessage.dependencies == null || checkCardDependenciesMessage.dependencies.Length == 0)
			{
				this.GetCreateOfferInfo();
			}
			else
			{
				App.Popups.ShowOkCancel(this, "deckinvalidationwarning", "Notice", "Selling this scroll will make the following decks illegal:\n\n" + DeckUtil.GetFormattedDeckNames(checkCardDependenciesMessage.GetDeckNames()), "Ok", "Cancel");
			}
		}
		else if (msg is MarketplaceCreateOfferInfoMessage)
		{
			MarketplaceCreateOfferInfoMessage marketplaceCreateOfferInfoMessage = (MarketplaceCreateOfferInfoMessage)msg;
			App.Popups.ShowSellCard(this, "sellcard", this.sellingCard, marketplaceCreateOfferInfoMessage.lowestPrice, marketplaceCreateOfferInfoMessage.suggestedPrice, marketplaceCreateOfferInfoMessage.copiesForSale, marketplaceCreateOfferInfoMessage.tax);
		}
	}

	// Token: 0x06001675 RID: 5749 RVA: 0x0001043A File Offset: 0x0000E63A
	private void showCreateOfferFailedTooManyOffers()
	{
		this.showCreateOfferFailed("You cannot have more than " + this.maxNumOffers + " offers active at a time.");
	}

	// Token: 0x06001676 RID: 5750 RVA: 0x0001045C File Offset: 0x0000E65C
	private void showCreateOfferFailed(string s)
	{
		App.Popups.ShowOk(this, "createofferfailed", "Cannot sell scroll", s, "Ok");
	}

	// Token: 0x06001677 RID: 5751 RVA: 0x000892E4 File Offset: 0x000874E4
	private IEnumerator SwitchView(Marketplace.EMarketplaceView view)
	{
		Log.info("BM: SwitchView: " + view.ToString());
		if (view == this.currentView)
		{
			yield break;
		}
		base.StopCoroutine("SwitchView");
		List<CardListPopup> fadeOut = new List<CardListPopup>();
		List<CardListPopup> fadeIn = new List<CardListPopup>();
		fadeOut.Add(this.buyScrollsList);
		fadeIn.Add(this.libraryScrollsList);
		fadeIn.Add(this.sellingScrollsList);
		fadeIn.Add(this.soldScrollsList);
		if (view == Marketplace.EMarketplaceView.BUY)
		{
			List<CardListPopup> tmp = fadeOut;
			fadeOut = fadeIn;
			fadeIn = tmp;
		}
		else
		{
			this.buyCardOverlay.Hide();
			this.forceDarkBG = true;
			this.tierDrop.SetEnabled(false);
		}
		while (this.overlayAlpha < 1f)
		{
			this.overlayAlpha += Time.fixedDeltaTime * 2.5f;
			foreach (CardListPopup clp in fadeOut)
			{
				clp.SetOpacity(1f - this.overlayAlpha);
			}
			yield return new WaitForEndOfFrame();
		}
		this.currentView = view;
		foreach (CardListPopup clp2 in fadeOut)
		{
			clp2.enabled = false;
		}
		foreach (CardListPopup clp3 in fadeIn)
		{
			clp3.enabled = true;
		}
		if (view == Marketplace.EMarketplaceView.SELL)
		{
			this.RefreshSellview();
		}
		else
		{
			this.RefreshBuyview();
			this.tierDrop.SetEnabled(true);
		}
		this.forceDarkBG = false;
		while (this.overlayAlpha > 0f)
		{
			this.overlayAlpha -= Time.fixedDeltaTime * 2.5f;
			foreach (CardListPopup clp4 in fadeIn)
			{
				clp4.SetOpacity(1f - this.overlayAlpha);
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	// Token: 0x06001678 RID: 5752 RVA: 0x00089310 File Offset: 0x00087510
	private void OnGUI()
	{
		GUI.depth = 23;
		GUI.skin = this.regularUI;
		int fontSize = GUI.skin.label.fontSize;
		int fontSize2 = GUI.skin.button.fontSize;
		TextAnchor alignment = GUI.skin.label.alignment;
		float num = (float)Screen.height * 0.03f;
		GUI.skin.button.fontSize = Screen.height / 38;
		GUI.skin.label.fontSize = Screen.height / 30;
		GUI.skin.label.alignment = 1;
		Rect rect;
		rect..ctor(this.buyScrollsList.getRect().x + num * 4f, this.libraryScrollsList.getRect().yMax, this.buyScrollsList.getRect().width - num * 8f, (float)Screen.height * 0.04f);
		rect.y -= rect.height;
		this.tierDrop.SetRect(rect);
		if (this.currentView == Marketplace.EMarketplaceView.SELL)
		{
			Rect rect2;
			rect2..ctor(this.libraryScrollsList.getRect().x - num, this.libraryScrollsList.getRect().y - num * 2.4f, this.sellingScrollsList.getRect().xMax - this.libraryScrollsList.getRect().x + num * 2f, this.libraryScrollsList.getRect().height + num * 3.4f);
			new ScrollsFrame(rect2).AddNinePatch(ScrollsFrame.Border.LIGHT_CURVED, NinePatch.Patches.CENTER).Draw();
			Rect rect3;
			rect3..ctor(this.soldScrollsList.getRect().x - num, this.soldScrollsList.getRect().y - num * 2.4f, this.soldScrollsList.getRect().width + num * 2f, this.soldScrollsList.getRect().height + num * 3.4f);
			new ScrollsFrame(rect3).AddNinePatch(ScrollsFrame.Border.LIGHT_CURVED, NinePatch.Patches.CENTER).Draw();
			Rect rect4 = this.libraryScrollsList.getRect();
			rect4.y -= (float)Screen.height * 0.05f;
			GUI.Label(rect4, "Your library");
			Rect rect5 = this.sellingScrollsList.getRect();
			rect5.y -= (float)Screen.height * 0.05f;
			GUI.Label(rect5, "Currently selling");
			Rect rect6 = this.soldScrollsList.getRect();
			rect6.y -= (float)Screen.height * 0.05f;
			GUI.Label(rect6, "Sold");
		}
		else
		{
			Rect rect7;
			rect7..ctor(this.buyScrollsList.getRect().x - num, this.buyScrollsList.getRect().y - num * 2.4f, this.buyScrollsList.getRect().width + num * 2f, this.buyScrollsList.getRect().height + num * 3.4f);
			new ScrollsFrame(rect7).AddNinePatch(ScrollsFrame.Border.LIGHT_CURVED, NinePatch.Patches.CENTER).Draw();
			Rect rect8;
			rect8..ctor(this.buyRightRectBox.x - num, this.buyRightRectBox.y - num * 2.4f, this.buyRightRectBox.width + num * 2f, this.buyRightRectBox.height + num * 3.4f);
			new ScrollsFrame(rect8).AddNinePatch(ScrollsFrame.Border.LIGHT_CURVED, NinePatch.Patches.CENTER).Draw();
			Rect rect9 = rect8;
			if (this.buyCardOverlay.isShowing() || this.forceDarkBG)
			{
				GUI.DrawTexture(rect9, ResourceManager.LoadTexture("Marketplace/marketplace_desaturated"));
			}
			else
			{
				GUI.DrawTexture(rect9, ResourceManager.LoadTexture("Marketplace/marketplace_saturated"));
			}
			Rect rect10 = this.buyScrollsList.getRect();
			rect10.y -= (float)Screen.height * 0.05f;
			GUI.Label(rect10, "Browse scrolls");
			if (this.buyCardOverlay.isShowing())
			{
				float x = this.buyCardOverlayRect.x;
				float width = this.buyCardOverlayRect.width;
				if (this.currentBuyOffer != null)
				{
					Rect r = GeomUtil.resizeCentered(new Rect(x, (float)Screen.height * 0.86f, width, (float)Screen.height * 0.05f), (float)Screen.height * 0.15f);
					if (this.GUIButton(r, new GUIContent("Buy").lockDemo()))
					{
						App.Communicator.send(new MarketplaceMakeDealMessage(this.currentBuyOffer.offer.id));
					}
					GUI.skin.label.fontSize = Screen.height / 40;
					GUI.Label(new Rect(x, (float)Screen.height * 0.76f, width, (float)Screen.height * 0.15f), string.Concat(new object[]
					{
						"<color=#bbaa88>Available for sale (this tier): ",
						this.currentBuyOffer.copiesForSale,
						"\nLowest price:</color> <color=#ffd055>",
						this.currentBuyOffer.offer.price,
						" gold</color>"
					}));
					GUI.color = new Color(1f, 0.75f, 0.75f);
					GUI.Label(new Rect(x, (float)Screen.height * 0.82f, width, (float)Screen.height * 0.15f), "You own " + this.typeCounter.Count(this.currentBuyOffer.offer.card.getType()) + " of this scroll.");
					GUI.color = Color.white;
				}
				else
				{
					GUI.skin.label.fontSize = Screen.height / 30;
					GUI.Label(new Rect(x, (float)Screen.height * 0.81f, width, (float)Screen.height * 0.15f), "<color=#bbaa88>Not currently available on the market</color>");
				}
			}
		}
		GUI.skin.label.alignment = alignment;
		GUI.skin.label.fontSize = fontSize;
		GUI.skin.button.fontSize = fontSize2;
		if (Event.current.type == 7)
		{
			this.gui3d.frameBegin();
			this.OnGUI_draw3D();
			this.gui3d.frameEnd();
		}
		if (this.overlayAlpha > 0f)
		{
			GUI.color = new Color(1f, 1f, 1f, this.overlayAlpha);
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), ResourceManager.LoadTexture("Shared/blackFiller"));
			GUI.color = new Color(1f, 1f, 1f, 1f);
		}
		this.OnGUI_drawTopbarSubmenu();
	}

	// Token: 0x06001679 RID: 5753 RVA: 0x00010479 File Offset: 0x0000E679
	private bool GUIButton(Rect r, GUIContent text)
	{
		if (App.GUI.Button(r, text))
		{
			App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
			return true;
		}
		return false;
	}

	// Token: 0x0600167A RID: 5754 RVA: 0x00089A20 File Offset: 0x00087C20
	private void OnGUI_drawTopbarSubmenu()
	{
		GUISkin skin = GUI.skin;
		GUI.skin = this.buttonSkin;
		GUI.DrawTexture(this.subMenuRect, ResourceManager.LoadTexture("ChatUI/menu_bar_sub"));
		GUIPositioner subMenuPositioner = App.LobbyMenu.getSubMenuPositioner(1f, 2, 160f);
		int fontSize = GUI.skin.label.fontSize;
		GUIStyle label = GUI.skin.label;
		int fontSize2 = Screen.height / 40;
		GUI.skin.button.fontSize = fontSize2;
		label.fontSize = fontSize2;
		if (LobbyMenu.drawButton(subMenuPositioner.getButtonRect(-0.5f), "Buy scrolls"))
		{
			base.StartCoroutine("SwitchView", Marketplace.EMarketplaceView.BUY);
		}
		if (LobbyMenu.drawButton(subMenuPositioner.getButtonRect(0.5f), "Sell scrolls"))
		{
			base.StartCoroutine("SwitchView", Marketplace.EMarketplaceView.SELL);
		}
		GUIStyle label2 = GUI.skin.label;
		fontSize2 = fontSize;
		GUI.skin.button.fontSize = fontSize2;
		label2.fontSize = fontSize2;
		Store.drawStoreMarketplaceMenu(this.buttonSkin, true);
		GUI.skin = skin;
	}

	// Token: 0x0600167B RID: 5755 RVA: 0x00089B30 File Offset: 0x00087D30
	private void OnGUI_draw3D()
	{
		this.gui3d.setDepth(950f);
		Texture2D tex = ResourceManager.LoadTexture("DeckBuilder/bg");
		this.gui3d.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), tex);
	}

	// Token: 0x0600167C RID: 5756 RVA: 0x0001049E File Offset: 0x0000E69E
	private void GetCreateOfferInfo()
	{
		App.Communicator.send(new MarketplaceCreateOfferInfoMessage(this.sellingCard.getCardType().id, (byte)this.sellingCard.level));
	}

	// Token: 0x0600167D RID: 5757 RVA: 0x00089B80 File Offset: 0x00087D80
	private void SellCardClicked(Card card)
	{
		if (card.tradable)
		{
			if (this.canSellMoreScrolls())
			{
				this.infoCardOverlay.Hide();
				this.sellingCard = card;
				App.Communicator.send(new CheckCardDependenciesMessage(card.getId()));
			}
			else
			{
				this.showCreateOfferFailedTooManyOffers();
			}
		}
		else
		{
			this.infoCardOverlay.Show(card);
		}
	}

	// Token: 0x0600167E RID: 5758 RVA: 0x00089BE8 File Offset: 0x00087DE8
	private void BuyCardClicked(Card card)
	{
		if (card.tradable)
		{
			App.Communicator.send(new MarketplaceOffersSearchViewMessage((long)card.getCardType().id, this.buySelectedLevel));
		}
		else
		{
			this.currentBuyOffer = null;
			this.buyCardOverlay.Show(card);
		}
	}

	// Token: 0x0600167F RID: 5759 RVA: 0x00089C3C File Offset: 0x00087E3C
	private void ClaimSalesMoney(Card card)
	{
		if (card.tradable)
		{
			TransactionInfo transactionInfo = this.soldScrollTransactions[card];
			this.transactionBeingClaimed = transactionInfo;
			App.Communicator.send(new MarketplaceClaimMessage(transactionInfo.transactionId));
		}
	}

	// Token: 0x06001680 RID: 5760 RVA: 0x000104CC File Offset: 0x0000E6CC
	public void PopupOk(string popupType)
	{
		if (popupType == "deckinvalidationwarning")
		{
			this.GetCreateOfferInfo();
		}
	}

	// Token: 0x06001681 RID: 5761 RVA: 0x00089C80 File Offset: 0x00087E80
	public void PopupOk(string popupType, string value)
	{
		if (popupType == "sellcard")
		{
			int price = 0;
			bool flag = int.TryParse(value, ref price);
			if (flag)
			{
				App.Communicator.send(new MarketplaceCreateOfferMessage(this.sellingCard.getId(), price));
			}
			else
			{
				App.Popups.ShowOk(this, "infopopup", "Error", "Something went wrong! Did you enter a numeric price?", "Ok");
			}
		}
	}

	// Token: 0x06001682 RID: 5762 RVA: 0x000104E4 File Offset: 0x0000E6E4
	public void PopupCancel(string popupType)
	{
		if (popupType == "deckinvalidationwarning")
		{
			this.sellingCard = null;
		}
	}

	// Token: 0x06001683 RID: 5763 RVA: 0x000028DF File Offset: 0x00000ADF
	public void ButtonClicked(CardListPopup popup, ECardListButton button)
	{
	}

	// Token: 0x06001684 RID: 5764 RVA: 0x000028DF File Offset: 0x00000ADF
	public void ButtonClicked(CardListPopup popup, ECardListButton button, List<Card> selectedCards)
	{
	}

	// Token: 0x06001685 RID: 5765 RVA: 0x00089CF0 File Offset: 0x00087EF0
	public void ItemButtonClicked(CardListPopup popup, Card card)
	{
		if (popup == this.libraryScrollsList)
		{
			this.SellCardClicked(card);
		}
		else if (popup == this.sellingScrollsList)
		{
			App.Communicator.send(new MarketplaceCancelOfferMessage(this.sellingOffers[card].id));
		}
		else if (popup == this.soldScrollsList)
		{
			this.ClaimSalesMoney(card);
		}
	}

	// Token: 0x06001686 RID: 5766 RVA: 0x000028DF File Offset: 0x00000ADF
	public void ItemHovered(CardListPopup popup, Card card)
	{
	}

	// Token: 0x06001687 RID: 5767 RVA: 0x00089D6C File Offset: 0x00087F6C
	public void ItemClicked(CardListPopup popup, Card card)
	{
		if (popup == this.buyScrollsList)
		{
			this.BuyCardClicked(card);
		}
		if (popup == this.libraryScrollsList)
		{
			this.SellCardClicked(card);
		}
		if (popup == this.soldScrollsList)
		{
			this.ClaimSalesMoney(card);
		}
	}

	// Token: 0x040013B6 RID: 5046
	protected const float ZBackground = 950f;

	// Token: 0x040013B7 RID: 5047
	public RenderTexture cardRenderTexture;

	// Token: 0x040013B8 RID: 5048
	private CardOverlay buyCardOverlay;

	// Token: 0x040013B9 RID: 5049
	private CardOverlay infoCardOverlay;

	// Token: 0x040013BA RID: 5050
	private GUISkin regularUI;

	// Token: 0x040013BB RID: 5051
	private GUISkin buttonSkin;

	// Token: 0x040013BC RID: 5052
	private EList<Card> buyScrolls = new EList<Card>();

	// Token: 0x040013BD RID: 5053
	private CardListPopup buyScrollsList;

	// Token: 0x040013BE RID: 5054
	private EList<Card> libraryScrolls = new EList<Card>();

	// Token: 0x040013BF RID: 5055
	private CardListPopup libraryScrollsList;

	// Token: 0x040013C0 RID: 5056
	private EList<Card> sellingScrolls = new EList<Card>();

	// Token: 0x040013C1 RID: 5057
	private CardListPopup sellingScrollsList;

	// Token: 0x040013C2 RID: 5058
	private EList<Card> soldScrolls = new EList<Card>();

	// Token: 0x040013C3 RID: 5059
	private CardListPopup soldScrollsList;

	// Token: 0x040013C4 RID: 5060
	private Card sellingCard;

	// Token: 0x040013C5 RID: 5061
	private CollectionUtil.Counter<int> typeCounter = new CollectionUtil.Counter<int>();

	// Token: 0x040013C6 RID: 5062
	private MarketplaceOffersSearchViewMessage currentBuyOffer;

	// Token: 0x040013C7 RID: 5063
	[SerializeField]
	private Material unlitMaterial;

	// Token: 0x040013C8 RID: 5064
	private Gui3D gui3d;

	// Token: 0x040013C9 RID: 5065
	private Rect subMenuRect;

	// Token: 0x040013CA RID: 5066
	private Rect buyCardOverlayRect;

	// Token: 0x040013CB RID: 5067
	private float overlayAlpha;

	// Token: 0x040013CC RID: 5068
	private Marketplace.EMarketplaceView currentView;

	// Token: 0x040013CD RID: 5069
	private Dictionary<Card, MarketplaceOffer> sellingOffers = new Dictionary<Card, MarketplaceOffer>();

	// Token: 0x040013CE RID: 5070
	private Dictionary<Card, MarketplaceTypeAvailability> buyOffers = new Dictionary<Card, MarketplaceTypeAvailability>();

	// Token: 0x040013CF RID: 5071
	private Dictionary<Card, TransactionInfo> soldScrollTransactions = new Dictionary<Card, TransactionInfo>();

	// Token: 0x040013D0 RID: 5072
	private Rect buyRightRectBox;

	// Token: 0x040013D1 RID: 5073
	private Texture buttonIconAddEnabled;

	// Token: 0x040013D2 RID: 5074
	private Texture buttonIconAddDisabled;

	// Token: 0x040013D3 RID: 5075
	private int maxNumOffers;

	// Token: 0x040013D4 RID: 5076
	private Dropdown tierDrop;

	// Token: 0x040013D5 RID: 5077
	private byte? buySelectedLevel;

	// Token: 0x040013D6 RID: 5078
	private bool forceDarkBG;

	// Token: 0x040013D7 RID: 5079
	private TransactionInfo transactionBeingClaimed;

	// Token: 0x020003FD RID: 1021
	private enum EMarketplaceView
	{
		// Token: 0x040013DB RID: 5083
		BUY,
		// Token: 0x040013DC RID: 5084
		SELL
	}
}
