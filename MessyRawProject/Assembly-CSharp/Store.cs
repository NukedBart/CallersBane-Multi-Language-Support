using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommConfig;
using Irrelevant.Assets;
using UnityEngine;

// Token: 0x02000400 RID: 1024
public class Store : AbstractCommListener, ICardListCallback, IOkCallback, ICancelCallback, IOkCancelCallback, IGoldShardsCallback, IOkStringCallback, IOkStringCancelCallback, iCardRule, iEffect
{
	// Token: 0x06001698 RID: 5784 RVA: 0x0008A2E8 File Offset: 0x000884E8
	private void Start()
	{
		if (App.Communicator.UsePurchaseEnvironment == PurchaseEnvironment.Disabled)
		{
			Store.ENABLE_PURCHASES = false;
		}
		this.cardOverlay = new GameObject("Card Overlay").AddComponent<CardOverlay>();
		this.sellCardOverlay = new GameObject("Card Overlay Sell").AddComponent<CardOverlay>();
		float num = (float)Screen.height * 0.6f;
		float num2 = num;
		Rect cardPos;
		cardPos..ctor((float)Screen.height * 0.728f - num2 / 2f, (float)Screen.height * 0.45f - num / 2f, num2, num);
		this.cardOverlay.Init(this.cardRenderTexture, cardPos, 22);
		this.sellCardOverlay.Init(this.cardRenderTexture, 10);
		float num3 = (float)Screen.height * 0.7f;
		float num4 = num3 * 0.8f;
		Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3((float)Screen.height * 0.85f, (float)Screen.height * 0.45f, 3f));
		this.cardDisplayPos = vector;
		this.cardEffectPos = new Vector3(vector.x + 0.0592f, vector.y + 0.192f, 4f);
		this.merchScroll = new Vector2(num4 - 25f, 0f);
		this.boughtMerchScroll = new Vector2(num4 - 25f, 0f);
		this.showInfoScroll = new Vector2(num4 - 25f, 0f);
		this.buyMenuObj = new GameObject();
		this.buyMenuObj.transform.position = new Vector3(-1f, 0f, 0f);
		this.sellMenuObj = new GameObject();
		this.sellMenuObj.transform.position = new Vector3(-1f, 0f, 0f);
		GameObject gameObject = PrimitiveFactory.createPlane();
		gameObject.transform.position = new Vector3(0f, 0f, 5.5f);
		gameObject.transform.eulerAngles = new Vector3(90f, 180f, 0f);
		gameObject.transform.localScale = new Vector3(1.42f, 1f, 0.8f);
		gameObject.renderer.material.mainTexture = ResourceManager.LoadTexture("Store/shop_bg");
		gameObject.name = "bg plane";
		Texture2D mainTexture = ResourceManager.LoadTexture("Store/keeper_big_sis");
		switch (new Random().Next(3))
		{
		case 0:
			mainTexture = ResourceManager.LoadTexture("Store/keeper_big_sis");
			break;
		case 1:
			mainTexture = ResourceManager.LoadTexture("Store/keeper_lil_bro");
			break;
		case 2:
			mainTexture = ResourceManager.LoadTexture("Store/keeper_ol_gramps");
			break;
		}
		GameObject gameObject2 = PrimitiveFactory.createPlane();
		gameObject2.transform.position = new Vector3(0.94f, -0.48f, 5.4f);
		gameObject2.transform.eulerAngles = new Vector3(90f, 180f, 0f);
		gameObject2.transform.localScale = new Vector3(0.695f, 1f, 0.66f);
		gameObject2.renderer.material = new Material(ResourceManager.LoadShader("Transparent/Diffuse"));
		gameObject2.renderer.material.mainTexture = mainTexture;
		gameObject2.name = "fg plane";
		this.buttonSkin = (GUISkin)ResourceManager.Load("_GUISkins/LobbyMenu");
		this.lobbySkin = (GUISkin)ResourceManager.Load("_GUISkins/Lobby");
		this.storeSkin = (GUISkin)ResourceManager.Load("_GUISkins/Store");
		this.storeHoverStyle = new GUIStyle(this.storeSkin.button);
		GUIStyleState active = this.storeHoverStyle.active;
		Texture2D background = this.storeHoverStyle.hover.background;
		this.storeHoverStyle.normal.background = background;
		active.background = background;
		GUIStyleState active2 = this.storeHoverStyle.active;
		Color textColor = this.storeHoverStyle.hover.textColor;
		this.storeHoverStyle.normal.textColor = textColor;
		active2.textColor = textColor;
		this.comm = App.Communicator;
		this.comm.addListener(this);
		this.comm.send(new GetStoreItemsMessage());
		this.comm.send(new LibraryViewMessage());
		this.sellFrame = new GameObject("Sell List").AddComponent<CardListPopup>();
		this.sellFrame.SetPersistence(App.Config.settings.cardlist.store_sell);
		this.sellFrame.Init(new Rect((float)Screen.width * 0.01f, (float)Screen.height * 0.18f, (float)Screen.height * 0.4f, (float)Screen.height * 0.7f), true, true, this.playerLibrary, this, null, new GUIContent("Sell"), false, true, false, false, null, true);
		this.sellFrame.SetOpacity(1f);
		this.showBuyMenu();
		App.ChatUI.Show(false);
		base.StartCoroutine(this.FadeInSceneAfterAWhile());
		if (App.SceneValues.store.openShardPurchasePopup)
		{
			this.ShowShardsPurchase();
		}
		else if (App.SceneValues.store.openBuyGamePopup)
		{
			this.ShowFullGamePurchase();
		}
		App.SceneValues.store.reset();
		App.AudioScript.PlayMusic("Music/Store", AudioScript.EPostFadeoutBehaviour.PAUSE);
	}

	// Token: 0x06001699 RID: 5785 RVA: 0x0008A85C File Offset: 0x00088A5C
	private IEnumerator FadeInSceneAfterAWhile()
	{
		yield return new WaitForSeconds(0.1f);
		App.LobbyMenu.fadeInScene();
		yield break;
	}

	// Token: 0x0600169A RID: 5786 RVA: 0x00010570 File Offset: 0x0000E770
	private void createText(string text, float time)
	{
		this.textText = text;
		this.textTimer = 3f;
	}

	// Token: 0x0600169B RID: 5787 RVA: 0x0008A870 File Offset: 0x00088A70
	private void showNextCard()
	{
		this.allowBuy = false;
		this.storeSkin = (GUISkin)ResourceManager.Load("_GUISkins/StoreDisabled");
		this.boughtCard = this.boughtCards[0];
		this.boughtCards.RemoveAt(0);
		App.AudioScript.PlaySFX("Sounds/scroll_purchase_01");
		if (this.boughtCard.level >= 1)
		{
			this.playBuyEffect("Scroll_appear_3a_appear");
			this.playBuyEffect("Scroll_appear_3b_rimshine");
		}
		else
		{
			switch (this.boughtCard.getRarity())
			{
			case 0:
				this.playBuyEffect("Scroll_appear_1a_appear");
				this.playBuyEffect("Scroll_appear_1_2b_rimshine");
				break;
			case 1:
				this.playBuyEffect("Scroll_appear_2a_appear");
				this.playBuyEffect("Scroll_appear_1_2b_rimshine");
				break;
			case 2:
				this.playBuyEffect("Scroll_appear_3a_appear");
				this.playBuyEffect("Scroll_appear_3b_rimshine");
				break;
			}
		}
		this.currentRarityString = this.boughtCard.getRarityString();
		if (this.boughtCard.level >= 1)
		{
			string text = (!this.boughtPack) ? "Upgraded!\n" : "Bonus! ";
			this.currentRarityString = string.Concat(new object[]
			{
				"<color=#eecc44>",
				text,
				"</color>Tier ",
				this.boughtCard.getTier(),
				" ",
				this.currentRarityString.ToLower()
			});
		}
		this.currentRarity = this.boughtCard.getRarity();
		this.currentLevel = this.boughtCard.level;
		this.transpTimer = 0.3f;
	}

	// Token: 0x0600169C RID: 5788 RVA: 0x0008AA20 File Offset: 0x00088C20
	private static void removeBuyEffects()
	{
		for (;;)
		{
			GameObject gameObject = GameObject.Find("BuyEffect_");
			if (gameObject == null)
			{
				break;
			}
			Object.DestroyImmediate(gameObject);
		}
	}

	// Token: 0x0600169D RID: 5789 RVA: 0x0008AA54 File Offset: 0x00088C54
	private void playBuyEffect(string file)
	{
		GameObject gameObject = new GameObject();
		gameObject.AddComponent<MeshRenderer>();
		gameObject.name = "BuyEffect_";
		EffectPlayer effectPlayer = gameObject.AddComponent<EffectPlayer>();
		Material materialToUse = new Material(ResourceManager.LoadShader("Scrolls/StoreEffect/Unlit/Transparent"));
		effectPlayer.setMaterialToUse(materialToUse);
		effectPlayer.init("BuyEffect/" + file, 1, this, 94000, new Vector3(0.409f, 0.4264f, 0.5f), false, string.Empty, 0);
		effectPlayer.getAnimPlayer().waitForUpdate();
		gameObject.transform.localPosition = this.cardEffectPos;
		gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
	}

	// Token: 0x0600169E RID: 5790 RVA: 0x0008AB08 File Offset: 0x00088D08
	public override void handleMessage(Message msg)
	{
		if (msg is GetStoreItemsMessage)
		{
			this.faceUpArr = new List<Store.Item>();
			this.faceDownArr = new List<Store.Item>();
			this.avatarItems = new List<Store.Item>();
			this.preConstDeckArr = new List<Store.Item>();
			this.idolItems = new List<Store.Item>();
			GetStoreItemsMessage getStoreItemsMessage = (GetStoreItemsMessage)msg;
			this.scrollReturnGold = getStoreItemsMessage.cardSellbackGold;
			for (int i = 0; i < getStoreItemsMessage.items.Length; i++)
			{
				Items items = getStoreItemsMessage.items[i];
				switch (items.itemType)
				{
				case Items.Type.CARD_FACE_DOWN:
					this.faceDownArr.Add(new Store.Item(items.itemId, "faceDownCards", items.costGold, items.costShards, items.description, "Store/shop_item_icon", items.isPublic, items.isPurchased, items.cardTypeId, items.itemName, items.cardTypeIds, items.expires));
					break;
				case Items.Type.CARD_FACE_UP:
					this.faceUpArr.Add(new Store.Item(items.itemId, "faceUpCards", items.costGold, items.costShards, items.description, CardTypeManager.getInstance().get(items.cardTypeId).cardImage.ToString(), items.isPublic, items.isPurchased, items.cardTypeId, CardTypeManager.getInstance().get(items.cardTypeId).name, items.cardTypeIds, items.expires));
					break;
				case Items.Type.CARD_DECAY:
					this.faceDownArr.Add(new Store.Item(items.itemId, "faceDownCards", items.costGold, items.costShards, items.description, "Store/shop_item_resource_bg_decay", items.isPublic, items.isPurchased, items.cardTypeId, items.itemName, items.cardTypeIds, items.expires));
					break;
				case Items.Type.CARD_ORDER:
					this.faceDownArr.Add(new Store.Item(items.itemId, "faceDownCards", items.costGold, items.costShards, items.description, "Store/shop_item_resource_bg_order", items.isPublic, items.isPurchased, items.cardTypeId, items.itemName, items.cardTypeIds, items.expires));
					break;
				case Items.Type.CARD_GROWTH:
					this.faceDownArr.Add(new Store.Item(items.itemId, "faceDownCards", items.costGold, items.costShards, items.description, "Store/shop_item_resource_bg_growth", items.isPublic, items.isPurchased, items.cardTypeId, items.itemName, items.cardTypeIds, items.expires));
					break;
				case Items.Type.CARD_ENERGY:
					this.faceDownArr.Add(new Store.Item(items.itemId, "faceDownCards", items.costGold, items.costShards, items.description, "Store/shop_item_resource_bg_energy", items.isPublic, items.isPurchased, items.cardTypeId, items.itemName, items.cardTypeIds, items.expires));
					break;
				case Items.Type.CARD_PACK:
				{
					Store.Item item = new Store.Item(items.itemId, "faceDownCards", items.costGold, items.costShards, items.description, "Store/shop_item_bundle", items.isPublic, items.isPurchased, items.cardTypeId, items.itemName, items.cardTypeIds, items.expires);
					item.isCardPack = true;
					this.faceDownArr.Add(item);
					break;
				}
				case Items.Type.CARD_PACK_NEW:
				{
					Store.Item item2 = new Store.Item(items.itemId, "faceDownCards", items.costGold, items.costShards, items.description, "Store/shop_item_bundle_new", items.isPublic, items.isPurchased, items.cardTypeId, items.itemName, items.cardTypeIds, items.expires);
					item2.isCardPack = true;
					this.faceDownArr.Add(item2);
					break;
				}
				case Items.Type.DECK:
				{
					string description = string.Empty;
					if (!string.IsNullOrEmpty(items.deckDescription))
					{
						description = items.deckDescription.Replace("\\n", "\n");
					}
					this.preConstDeckArr.Add(new Store.Item(items.itemId, "preConstDeck", items.costGold, items.costShards, description, "Store/" + items.deckName.Replace(" ", "_"), items.isPublic, items.isPurchased, items.cardTypeId, items.deckName, items.cardTypeIds, items.expires));
					break;
				}
				case Items.Type.IDOL:
				{
					IdolType idolType = IdolTypeManager.getInstance().get(items.idolId);
					if (idolType != null)
					{
						this.idolItems.Add(new Store.Item(items.itemId, "idol", items.costGold, items.costShards, items.description, idolType.getFullHealthFilename(), items.isPublic, items.isPurchased, items.cardTypeId, idolType.name, items.cardTypeIds, items.expires));
					}
					break;
				}
				case Items.Type.AVATAR:
				{
					AvatarPart avatarPart = AvatarPartTypeManager.getInstance().get((int)items.avatarPart);
					if (avatarPart != null)
					{
						string text = ' ' + AvatarPartTypeManager.getSetName(avatarPart) + '.';
						Store.Item item3 = new Store.Item(items.itemId, "avatar_part", items.costGold, items.costShards, items.description + text, "Profile/sets/" + avatarPart.getFullFilename(), items.isPublic, items.isPurchased, items.cardTypeId, "Avatar head", items.cardTypeIds, items.expires);
						item3.avatarPart = avatarPart;
						this.avatarItems.Add(item3);
					}
					break;
				}
				case Items.Type.AVATAR_OUTFIT:
				{
					AvatarInfo avatar = items.getAvatar();
					string text2 = ' ' + AvatarPartTypeManager.getInstance().getSetName(avatar) + '.';
					Store.Item item4 = new Store.Item(items.itemId, "avatar_outfit", items.costGold, items.costShards, items.avatar.description + "\n\n" + items.description + text2, "Store/avatar_items/" + items.avatar.image, items.isPublic, items.isPurchased, items.cardTypeId, items.avatar.name, items.cardTypeIds, items.expires);
					item4.avatar = avatar;
					this.avatarItems.Add(item4);
					break;
				}
				}
			}
			this.scrollsRecieved = true;
		}
		if (msg is LibraryViewMessage)
		{
			this.playerLibrary.Clear();
			this.playerLibrary.AddRange(((LibraryViewMessage)msg).cards);
			this.playerLibrary.Sort(this.sorter);
		}
		if (msg is BuyStoreItemResponseMessage)
		{
			this.currentRarityString = string.Empty;
			this.currentRarity = 0;
			BuyStoreItemResponseMessage buyStoreItemResponseMessage = (BuyStoreItemResponseMessage)msg;
			if (buyStoreItemResponseMessage.cards.Length > 0)
			{
				this.boughtCards = new List<Card>(buyStoreItemResponseMessage.cards);
				this.boughtPack = (buyStoreItemResponseMessage.cards.Length > 1);
				this.playerLibrary.AddRange(this.boughtCards);
			}
			else
			{
				this.boughtCards = null;
			}
			if (buyStoreItemResponseMessage.isEmpty())
			{
				App.Popups.ShowOk(this, "itemresponseempty", "Code redeemed", "You already own all items contained in this bundle.", "Ok");
			}
			else if (buyStoreItemResponseMessage.isCardsOnlyMessage())
			{
				this.showNextCard();
			}
			else if (buyStoreItemResponseMessage.isSingleDeckMessage())
			{
				DeckInfo deckInfo = buyStoreItemResponseMessage.deckInfos[0];
				App.Popups.ShowOk(this, "deckpurchased", "Deck purchased", deckInfo.name + " has been added to your collection.\nIt can be viewed in the Deck Builder.", "Ok");
			}
			else if (buyStoreItemResponseMessage.isSingleAvatarMessage())
			{
				AvatarInfoDeserializer avatarInfoDeserializer = buyStoreItemResponseMessage.avatars[0];
				App.Popups.ShowOk(this, "avatarpurchased", "Avatar purchased", avatarInfoDeserializer.name + " has been added to your collection.\nIt can be selected in the Profile menu.", "Ok");
			}
			else if (buyStoreItemResponseMessage.isSingleIdolMessage())
			{
				IdolType idolType2 = IdolTypeManager.getInstance().get(buyStoreItemResponseMessage.idols[0]);
				App.Popups.ShowOk(this, "idolpurchased", "Idol purchased", idolType2.name + " has been added to your collection.\nIt can be selected in the Profile menu.", "Ok");
			}
			else
			{
				App.Popups.ShowOk(this, "itemresponsemultiple", "Items unlocked", buyStoreItemResponseMessage.getUnlockString(), "Ok");
			}
		}
		if (msg is PurchaseStatusMessage)
		{
			base.StopCoroutine("ShowProcessingPopup");
			PurchaseStatusMessage purchaseStatusMessage = (PurchaseStatusMessage)msg;
			if (purchaseStatusMessage.orderStatus == OrderStatus.SUCCEEDED)
			{
				App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_victory");
				if (purchaseStatusMessage.variantId == "scrolls")
				{
					App.MyProfile.ProfileInfo.UnlockFullGame();
				}
			}
			App.Popups.ShowOk(this, "purchaseStatus", purchaseStatusMessage.title, purchaseStatusMessage.text, "Ok");
		}
		if (msg is FailMessage)
		{
			FailMessage failMessage = (FailMessage)msg;
			if (failMessage.isType(typeof(BuyCardMessage)) && failMessage.info != null)
			{
				this.createText(failMessage.info, 2f);
			}
			else if (failMessage.isType(typeof(BuyStoreItemMessage)) && failMessage.info != null)
			{
				App.Popups.ShowOk(this, "buystoreitemfail", "Purchase failed", failMessage.info, "Ok");
				this.allowBuy = true;
				this.storeSkin = (GUISkin)ResourceManager.Load("_GUISkins/Store");
			}
			if (failMessage.isType(typeof(RedeemMessage)))
			{
				App.Popups.ShowOk(this, "redeemfail", "Failed to redeem code", failMessage.info, "Ok");
			}
		}
		base.handleMessage(msg);
	}

	// Token: 0x0600169F RID: 5791 RVA: 0x00003FDC File Offset: 0x000021DC
	public void effectAnimDone(EffectPlayer effect, bool loop)
	{
		DefaultIEffectCallback.instance().effectAnimDone(effect, loop);
	}

	// Token: 0x060016A0 RID: 5792 RVA: 0x000028DF File Offset: 0x00000ADF
	public void locator(EffectPlayer effect, AnimLocator loc)
	{
	}

	// Token: 0x060016A1 RID: 5793 RVA: 0x0008B4D4 File Offset: 0x000896D4
	private CardView createCardView(Card card)
	{
		GameObject gameObject = PrimitiveFactory.createPlane();
		gameObject.name = "CardRule";
		CardView cardView = gameObject.AddComponent<CardView>();
		cardView.init(this, card, -1);
		cardView.applyHighResTexture();
		return cardView;
	}

	// Token: 0x060016A2 RID: 5794 RVA: 0x0008B50C File Offset: 0x0008970C
	private void showCardRule(Card ui)
	{
		if (this.boughtCards.Count == 0)
		{
			this.enableAllowBuy();
		}
		this.stackCardRuleTimer = 0.4f;
		CardView cardView = this.createCardView(ui);
		this.cardRule = cardView.gameObject;
		this.cardRule.transform.localPosition = this.cardDisplayPos;
		this.cardRule.transform.localEulerAngles = new Vector3(90f, 180f, 0f);
		this.cardRule.transform.localScale = CardView.CardLocalScale(3.55f);
		this.cardStackArr.Insert(0, ui);
		this.showSellButton();
	}

	// Token: 0x060016A3 RID: 5795 RVA: 0x00010584 File Offset: 0x0000E784
	private void enableAllowBuy()
	{
		this.enableAllowBuyTimer = 1f;
	}

	// Token: 0x060016A4 RID: 5796 RVA: 0x000028DF File Offset: 0x00000ADF
	public void HideCardView()
	{
	}

	// Token: 0x060016A5 RID: 5797 RVA: 0x000028DF File Offset: 0x00000ADF
	public void ActivateTriggeredAbility(string id, TilePosition pos)
	{
	}

	// Token: 0x060016A6 RID: 5798 RVA: 0x0008B5B8 File Offset: 0x000897B8
	private void showInformation(Store.Item item)
	{
		if (!this.allowBuy)
		{
			return;
		}
		this.hideInformation();
		this.showInfo = item;
		if (this.cardRule != null)
		{
			this.stackCardRule();
		}
		if (item.type == "faceUpCards")
		{
			CardType type = CardTypeManager.getInstance().get(item.cardTypeId);
			Card card = new Card(123L, type, false);
			this.cardOverlay.Show(card);
			this.ownedCardsOfTypeCount = Enumerable.Count<Card>(this.playerLibrary, (Card c) => c.typeId == item.cardTypeId);
		}
	}

	// Token: 0x060016A7 RID: 5799 RVA: 0x00010591 File Offset: 0x0000E791
	private void hideInformation()
	{
		this.showInfo = null;
		this.sellCardOverlay.Hide();
		this.cardOverlay.Hide();
	}

	// Token: 0x060016A8 RID: 5800 RVA: 0x0008B66C File Offset: 0x0008986C
	private void OnGUI()
	{
		GUI.skin = this.storeSkin;
		if (Store.ENABLE_PURCHASES)
		{
			float num = (float)Screen.height * 0.01f;
			if (App.IsPremium())
			{
				Rect rect;
				rect..ctor((float)Screen.width - (float)Screen.height * 0.37f, (float)Screen.height * 0.18f, (float)Screen.height * 0.16f, (float)Screen.height * 0.04f);
				if (App.GUI.Button(rect, new GUIContent("Redeem code")))
				{
					this.ShowRedeem();
				}
			}
			else
			{
				float num2 = (float)Screen.width - (float)Screen.height * 0.36f;
				float num3 = (float)Screen.height * 0.36f;
				float num4 = (float)Screen.height * 0.2f;
				float num5 = (float)Screen.height * 0.04f;
				Rect rect2;
				rect2..ctor(num2 - num3 / 2f, (float)Screen.height * 0.16f, num3, num3 * 215f / 512f);
				Rect rect3;
				rect3..ctor(num2 - num4 / 2f - num, rect2.yMax - num, num4 + num * 2f, num5 + num * 2f);
				Rect rect4;
				rect4..ctor(num2 - num4 / 2f, rect2.yMax, num4, num5);
				GUI.DrawTexture(rect2, ResourceManager.LoadTexture("Store/buy_game_attractor"));
				GUI.DrawTexture(rect3, ResourceManager.LoadTexture("Store/purchase_shards_box"));
				int fontSize = GUI.skin.button.fontSize;
				GUI.skin.button.fontSize = Screen.height / 36;
				if (App.GUI.Button(rect4, new GUIContent("Purchase full version")))
				{
					this.ShowFullGamePurchase();
				}
				GUI.skin.button.fontSize = fontSize;
			}
		}
		GUI.depth = 23;
		if (this.lockGUI)
		{
			GUI.skin = ScriptableObject.CreateInstance<GUISkin>();
			GUI.Button(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), string.Empty);
		}
		GUI.skin = this.storeSkin;
		if (!this.scrollsRecieved)
		{
			return;
		}
		if (this.boughtCards != null && this.boughtCards.Count >= 0)
		{
			bool flag = this.boughtPack || this.currentLevel >= 1;
			GUI.DrawTexture(new Rect((float)Screen.height * 0.72f, (float)Screen.height * 0.895f, (float)Screen.height * 0.27f, (float)Screen.height * 0.075f), ResourceManager.LoadTexture("Store/purchase_shards_box"));
			TextAnchor alignment = GUI.skin.label.alignment;
			GUI.skin.label.alignment = 1;
			Color textColor = GUI.skin.label.normal.textColor;
			GUI.skin.label.normal.textColor = new Color(0.45f + (float)this.currentRarity * 0.25f, 0.4f + (float)this.currentRarity * 0.19f, 0.35f, 1f);
			int fontSize2 = GUI.skin.label.fontSize;
			if (!flag)
			{
				GUI.skin.label.fontSize = Screen.height / 26;
			}
			else
			{
				GUI.skin.label.fontSize = Screen.height / 38;
			}
			string text = this.currentRarityString;
			if (this.boughtPack)
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					"\n<color=#eeddbb>",
					this.boughtCards.Count,
					(this.boughtCards.Count != 1) ? " scrolls" : " scroll",
					" left in pack</color>"
				});
			}
			GUI.Label(new Rect((float)Screen.height * 0.72f, (float)Screen.height * (0.9025f + ((!flag) ? 0.0075f : 0f)), (float)Screen.height * 0.27f, (float)fontSize2 * 2.5f), text);
			GUI.skin.label.fontSize = fontSize2;
			GUI.skin.label.normal.textColor = textColor;
			if (this.boughtPack)
			{
				float num6 = (float)Screen.height * 0.08f;
				float num7 = (float)Screen.height * 0.08f / 1.25f;
				GUI.DrawTexture(new Rect((float)Screen.height * 0.7f - num6 / 2.5f, (float)Screen.height * 0.927f - num7 / 2.5f, num6, num7), ResourceManager.LoadTexture(this.lastItemImageString));
			}
			GUI.skin.label.alignment = alignment;
		}
		float num8 = (float)(10 + Screen.height / 72);
		GUI.skin.label.fontSize = (int)num8;
		int fontSize3 = GUI.skin.label.fontSize;
		float num9 = (float)Screen.height * 0.7f;
		float num10 = num9 * 0.8f;
		float num11 = num10 * 0.8f / 3f;
		float num12 = num11 * 1.4f;
		float num13 = (float)Screen.height * 0.84f;
		Color textColor2 = GUI.skin.label.normal.textColor;
		GUI.skin = (GUISkin)ResourceManager.Load("_GUISkins/ChatUI");
		GUI.skin.label.normal.textColor = textColor2;
		if (this.showInfo != null)
		{
			new ScrollsFrame(new Rect(num10 - 10f, (float)Screen.height * 0.15f, (float)Screen.height * 0.35f, num13 * 1.1f)).AddNinePatch(ScrollsFrame.Border.DARK_SHARP, NinePatch.Patches.TOP_LEFT | NinePatch.Patches.TOP | NinePatch.Patches.LEFT | NinePatch.Patches.CENTER | NinePatch.Patches.BOTTOM_LEFT | NinePatch.Patches.BOTTOM).Draw();
			Rect rect5;
			rect5..ctor(num10, (float)Screen.height * 0.16f, (float)Screen.height * 0.35f, num13);
			GUI.BeginGroup(rect5);
			if (this.showInfo.type == "avatar_outfit" || this.showInfo.type == "idol" || this.showInfo.type == "avatar_part")
			{
				int fontSize4 = GUI.skin.label.fontSize;
				TextAnchor alignment2 = GUI.skin.label.alignment;
				GUI.skin.label.alignment = 1;
				GUI.skin.label.fontSize = (int)((float)fontSize3 * 1.3f);
				GUI.Label(new Rect(0f, (float)Screen.height * 0.035f, rect5.width, (float)Screen.height * 0.1f), this.showInfo.name);
				GUI.skin.label.alignment = alignment2;
				GUI.skin.label.fontSize = fontSize4;
			}
			float num14 = (float)Screen.height * 0.55f;
			if (this.showInfo.type != "faceUpCards" && this.showInfo.type != "avatar_outfit" && this.showInfo.type != "idol" && this.showInfo.type != "avatar_part")
			{
				float num15 = (float)Screen.height * 0.291f;
				float num16 = num15 / 1.33333f;
				GUI.color = new Color(1f, 1f, 1f, 0.5f);
				GUI.Box(new Rect(15f, 10f, num15, num16 + 1f), string.Empty);
				GUI.color = Color.white;
				GUI.DrawTexture(new Rect(17f, 12f, num15 - 4f, num16 - 4f), ResourceManager.LoadTexture(this.showInfo.image));
				if (this.showInfo.image == "Store/shop_item_resource_bg_decay" || this.showInfo.image == "Store/shop_item_resource_bg_energy" || this.showInfo.image == "Store/shop_item_resource_bg_growth" || this.showInfo.image == "Store/shop_item_resource_bg_order")
				{
					GUI.DrawTexture(new Rect(2f + (float)Screen.height * 0.09f, 2f + (float)Screen.height * 0.06f, num15 * 0.7f - 4f, num16 * 0.7f - 4f), ResourceManager.LoadTexture("Store/shop_item_icon"));
				}
				num14 = (float)Screen.height * 0.31f / 1.3333f + (float)Screen.height * 0.05f;
			}
			if (this.showInfo.type == "preConstDeck")
			{
				GUI.color = new Color(1f, 1f, 1f, 0.5f);
				float num17 = (float)Screen.height * 0.31f;
				float num18 = num17 / 1.33333f;
				GUI.Box(new Rect(15f, num18 + 5f, num17 - 11f, num13 - (num18 + 5f) - 5f - (float)Screen.height * 0.09f), string.Empty);
				GUI.Box(new Rect(15f + (num17 - 11f) - 20f, num18 + 5f + 5f, 15f, num13 - (num18 + 5f) - 15f - (float)Screen.height * 0.09f), string.Empty);
				GUI.color = Color.white;
				bool wordWrap = GUI.skin.label.wordWrap;
				GUI.skin.label.wordWrap = true;
				GUI.skin.label.fontSize = (int)Math.Round((double)((float)fontSize3 * 0.8f));
				string text3 = this.showInfo.description + this.showInfo.cardNames;
				float num19 = GUI.skin.GetStyle("label").CalcHeight(new GUIContent(text3), (float)Screen.height * 0.291f - 25f - 10f);
				this.showInfoScroll = GUI.BeginScrollView(new Rect(15f, num18 + 5f + 5f, num17 - 16f, num13 - (num18 + 20f) - (float)Screen.height * 0.09f), this.showInfoScroll, new Rect(0f, 0f, num17 - 16f - 25f, num19), false, false);
				GUI.skin.label.alignment = 0;
				GUI.Label(new Rect(5f, 0f, (float)Screen.height * 0.291f - 25f - 10f, num19), text3);
				GUI.skin.label.wordWrap = false;
				GUI.skin.label.wordWrap = wordWrap;
				GUI.EndScrollView();
			}
			if (this.showInfo.type == "avatar_outfit")
			{
				float num20 = 0.8f * Mathf.Min(rect5.width / (float)Avatar.DefaultWidth, rect5.height / (float)Avatar.DefaultHeight);
				Rect rect6;
				rect6..ctor(0f, 0f, (float)Avatar.DefaultWidth * num20, (float)Avatar.DefaultHeight * num20);
				rect6.x = 0.5f * (rect5.width - rect6.width);
				rect6.y = 0.16f * (rect5.height - rect6.height);
				Avatar.Create(this.showInfo.avatar, 0).draw(rect6, true);
				num14 = rect6.yMax;
			}
			if (this.showInfo.type == "avatar_part")
			{
				Texture2D texture2D = ResourceManager.LoadTexture(this.showInfo.image);
				float num21 = (float)Screen.height * 0.001f * (float)texture2D.width;
				float num22 = num21 * (float)texture2D.height / (float)texture2D.width;
				float num23 = (rect5.width - num21) * 0.5f;
				GUI.DrawTexture(new Rect(num23, (float)Screen.height * 0.1f, num21, num22), texture2D);
			}
			if (this.showInfo.type == "idol")
			{
				float num24 = (float)Screen.height * 0.275f;
				float num25 = 1.0791367f * num24;
				GUI.DrawTexture(new Rect(17f, (float)Screen.height * 0.1f, num24, num25), ResourceManager.LoadTexture(this.showInfo.image));
			}
			if (this.showInfo.type != "preConstDeck")
			{
				bool wordWrap2 = GUI.skin.label.wordWrap;
				GUI.skin.label.alignment = 1;
				GUI.skin.label.wordWrap = true;
				GUI.Label(new Rect(15f, num14, (float)Screen.height * 0.291f, (float)Screen.height * 0.2f), this.showInfo.description + (string.IsNullOrEmpty(this.showInfo.expires) ? string.Empty : ("\n\n<color=#aaa598>Expires " + this.showInfo.expires + ".</color>")));
				GUI.skin.label.wordWrap = false;
				GUI.skin.label.wordWrap = wordWrap2;
			}
			if (this.showInfo.type == "faceUpCards")
			{
				GUI.color = new Color(1f, 0.75f, 0.75f);
				GUI.Label(new Rect(15f, (float)Screen.height * 0.71f, (float)Screen.height * 0.291f, (float)Screen.height * 0.2f), "You own " + this.ownedCardsOfTypeCount + " of this scroll.");
				GUI.color = Color.white;
			}
			GUI.skin = this.storeSkin;
			GUI.enabled = this.showInfo.inStock;
			GUIContent c = (!this.showInfo.inStock) ? this.contentSoldOut : ((!this.showInfo.isDemoPurchasable()) ? this.contentLocked : this.contentUnlocked);
			if (App.GUI.Button(new Rect(15f, (float)Screen.height * 0.75f, (float)Screen.height * 0.31f - 11f, (float)Screen.height * 0.05f), c))
			{
				this.buyButtonClicked(this.showInfo);
			}
			GUI.enabled = true;
			GUI.skin = (GUISkin)ResourceManager.Load("_GUISkins/ChatUI");
			if (GUI.Button(new Rect(0f, 0f, (float)Screen.height * 0.31f, num13), string.Empty) && this.canHideInformationOnClick())
			{
				this.hideInformation();
			}
			GUI.EndGroup();
		}
		Rect rect7;
		rect7..ctor((float)Screen.width * this.buyMenuObj.transform.position.x, (float)Screen.height * 0.16f, num10 + 20f, (float)Screen.height * 0.84f);
		GUI.BeginGroup(rect7);
		new ScrollsFrame(new Rect(-10f, 0f - num13 * 0.05f, num10 + 20f, num13 * 1.1f)).AddNinePatch(ScrollsFrame.Border.DARK_SHARP, NinePatch.Patches.TOP_LEFT | NinePatch.Patches.TOP | NinePatch.Patches.LEFT | NinePatch.Patches.CENTER | NinePatch.Patches.BOTTOM_LEFT | NinePatch.Patches.BOTTOM).Draw();
		GUI.skin.label.fontSize = (int)num8;
		GUI.color = new Color(1f, 1f, 1f, 0.5f);
		GUI.Box(new Rect(5f, 5f, num10 - 10f, num13 - 10f), string.Empty);
		GUI.Box(new Rect(num10 - 25f, 10f, 15f, num13 - 20f), string.Empty);
		GUI.color = Color.white;
		this.merchScroll = GUI.BeginScrollView(new Rect(10f, 10f, num10 - 20f, num13 - 20f), this.merchScroll, new Rect(0f, 0f, num10 - 100f, this.merchScrollHeight), false, false);
		float num26 = (float)Screen.height * 0.04f;
		float num27 = num26 / 2f;
		num27 += (float)this.drawItemsDefault(this.faceDownArr, "Random Scrolls", num27);
		num27 += num26;
		num27 += (float)this.drawItemsDefault(this.faceUpArr, "Just for You", num27);
		num27 += num26;
		num27 += (float)this.drawItemsDefault(this.preConstDeckArr, "Preconstructed Decks", num27);
		num27 += num26;
		num27 += (float)this.drawItemsDefault(Enumerable.ToList<Store.Item>(Enumerable.Where<Store.Item>(this.avatarItems, (Store.Item x) => x.avatar != null)), "Avatars", num27);
		num27 += num26 * 0.25f;
		num27 += (float)this.drawItemsDefault(Enumerable.ToList<Store.Item>(Enumerable.Where<Store.Item>(this.avatarItems, (Store.Item x) => x.avatarPart != null)), string.Empty, num27);
		num27 += num26;
		num27 += (float)this.drawItemsDefault(this.idolItems, "Idols", num27);
		this.merchScrollHeight = ((this.merchScrollHeight >= num27) ? num27 : num27);
		GUI.EndScrollView();
		GUI.EndGroup();
		GUI.BeginGroup(new Rect((float)Screen.width - (float)Screen.height * 0.17f - 10f, (float)Screen.height * 0.16f, (float)Screen.width - ((float)Screen.width - (float)Screen.height * 0.17f) + 20f, num13));
		new ScrollsFrame(new Rect(0f, 0f - num13 * 0.05f, (float)Screen.width - ((float)Screen.width - (float)Screen.height * 0.17f) + 20f, num13 * 1.1f)).AddNinePatch(ScrollsFrame.Border.DARK_SHARP, NinePatch.Patches.TOP | NinePatch.Patches.TOP_RIGHT | NinePatch.Patches.CENTER | NinePatch.Patches.RIGHT | NinePatch.Patches.BOTTOM | NinePatch.Patches.BOTTOM_RIGHT).Draw();
		GUI.color = new Color(1f, 1f, 1f, 0.5f);
		GUI.Box(new Rect(15f, 25f, (float)Screen.width - ((float)Screen.width - (float)Screen.height * 0.17f) - 10f, num13 - 30f), string.Empty);
		GUI.color = Color.white;
		GUI.skin.label.alignment = 1;
		GUI.skin.label.fontSize = (int)Math.Round((double)((float)fontSize3 * 1f));
		GUI.Label(new Rect(15f, 5f, (float)Screen.width - ((float)Screen.width - (float)Screen.height * 0.17f) - 10f, 30f), "Purchases");
		GUI.Box(new Rect((float)Screen.width - ((float)Screen.width - (float)Screen.height * 0.17f) - 15f, 30f, 15f, num13 - 40f), string.Empty);
		this.boughtMerchScroll = GUI.BeginScrollView(new Rect(0f, 30f, (float)Screen.width - ((float)Screen.width - (float)Screen.height * 0.17f), num13 - 40f), this.boughtMerchScroll, new Rect(0f, 0f, (float)Screen.width - ((float)Screen.width - (float)Screen.height * 0.17f) - 100f, this.boughtMerchScrollHeight), false, false);
		float num28 = 0f;
		for (int i = 0; i < this.cardStackArr.Count; i++)
		{
			Card card = this.cardStackArr[i];
			GUI.BeginGroup(new Rect((float)Screen.height * 0.03f, num12 * 0.77f * (float)i, num11 * 0.74f, num12 * 0.74f));
			if (this.cardRule == null || card.getId() != this.cardRule.GetComponent<CardView>().getCardInfo().getId())
			{
				GUI.color = new Color(0.5f, 0.5f, 0.5f, 1f);
			}
			GUI.Box(new Rect(0f, 0f, num11 * 0.74f, num12 * 0.74f), string.Empty);
			GUI.DrawTexture(new Rect(2f, 2f, num11 * 0.74f - 4f, (num11 * 0.74f - 4f) * 0.75f), App.AssetLoader.LoadCardImage(card.getCardImage()));
			GUI.DrawTexture(new Rect(2f, num12 * 0.74f * 0.42f, num11 * 0.74f - 4f, num12 * 0.74f * 0.56f), ResourceManager.LoadTexture("Store/shop_thumb_gradient3"));
			GUI.skin.label.alignment = 0;
			GUI.Label(new Rect(num11 * 0.07f * 0.74f, num12 * 0.74f * 0.55f, num11 * 0.74f - num11 * 0.07f * 0.74f - 2f, 30f), card.getName());
			GUI.skin = this.storeSkin;
			int fontSize5 = GUI.skin.button.fontSize;
			GUI.skin.button.fontSize = (int)Math.Round((double)((float)fontSize5 * 0.65f));
			Color color = GUI.color;
			GUI.color = Color.white;
			if (this.cardRule != null && card.getId() == this.cardRule.GetComponent<CardView>().getCardInfo().getId())
			{
				GUI.enabled = false;
			}
			if (GUI.Button(new Rect(num11 * 0.07f * 0.74f, num12 * 0.8f * 0.74f, num11 * 0.86f * 0.74f, num12 * 0.15f * 0.74f), "View") && this.allowBuy)
			{
				this.hideInformation();
				float theY = (float)Screen.height * 0.16f + 10f + num12 * 0.67f * (float)i + (float)Screen.height * 0.03f;
				this.zoomInCard(i, theY);
			}
			GUI.enabled = true;
			GUI.color = color;
			GUI.color = new Color(1f, 1f, 1f, 1f);
			GUI.skin.button.fontSize = fontSize5;
			GUI.skin = (GUISkin)ResourceManager.Load("_GUISkins/ChatUI");
			if (i == this.cardStackArr.Count - 1)
			{
				num28 = num12 * 0.77f * (float)i + num12 * 0.74f;
			}
			GUI.EndGroup();
		}
		this.boughtMerchScrollHeight = ((this.boughtMerchScrollHeight >= num28) ? this.boughtMerchScrollHeight : num28);
		GUI.EndScrollView();
		GUI.EndGroup();
		GUI.skin.label.normal.textColor = Color.white;
		this.drawSubMenu();
	}

	// Token: 0x060016A9 RID: 5801 RVA: 0x000105B0 File Offset: 0x0000E7B0
	private bool canHideInformationOnClick()
	{
		return !this.cardOverlay.isShowing() || !this.cardOverlay.IsHovered();
	}

	// Token: 0x060016AA RID: 5802 RVA: 0x0008CE44 File Offset: 0x0008B044
	private void buyButtonClicked(Store.Item item)
	{
		if (!this.allowBuy)
		{
			return;
		}
		if (item.type != "faceDownCards")
		{
			App.Popups.ShowGoldShardsSelector(this, "goldshards", item.name, item.itemId, item.priceGold, item.priceShards, App.MyProfile.ProfileData.gold, App.MyProfile.ProfileData.shards);
		}
		else
		{
			this.PurchaseItem(item.itemId, false);
		}
	}

	// Token: 0x060016AB RID: 5803 RVA: 0x0008CECC File Offset: 0x0008B0CC
	public void PopupOk(string popupType)
	{
		if (popupType != null)
		{
			if (Store.<>f__switch$map1F == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
				dictionary.Add("sellscrolls", 0);
				dictionary.Add("getpurchaseinfofail", 1);
				dictionary.Add("fullgamepurchase", 2);
				Store.<>f__switch$map1F = dictionary;
			}
			int num;
			if (Store.<>f__switch$map1F.TryGetValue(popupType, ref num))
			{
				switch (num)
				{
				case 0:
				{
					long[] array = new long[this.selectedCards.Count];
					for (int i = 0; i < this.selectedCards.Count; i++)
					{
						Card card = this.selectedCards[i];
						array[i] = card.getId();
						if (this.cardRule != null && card.getId() == this.cardRule.GetComponent<CardView>().getCardInfo().getId())
						{
							Object.Destroy(this.cardRule);
							this.cardRule = null;
							this.sellButtonAlpha = 0f;
						}
						for (int j = 0; j < this.cardStackArr.Count; j++)
						{
							Card card2 = this.cardStackArr[j];
							if (card.getId() == card2.getId())
							{
								this.cardStackArr.RemoveAt(j);
								j--;
							}
						}
					}
					this.comm.send(new SellCardsMessage(array));
					this.comm.send(new LibraryViewMessage());
					this.lockGUI = false;
					break;
				}
				case 1:
					Application.OpenURL("https://account.mojang.com");
					break;
				case 2:
					Application.OpenURL("https://account.mojang.com/demo/upgrade/" + App.MyProfile.ProfileInfo.profileUuid);
					break;
				}
			}
		}
	}

	// Token: 0x060016AC RID: 5804 RVA: 0x0008D08C File Offset: 0x0008B28C
	public void PopupCancel(string popupType)
	{
		if (popupType != null)
		{
			if (Store.<>f__switch$map20 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
				dictionary.Add("sellscrolls", 0);
				Store.<>f__switch$map20 = dictionary;
			}
			int num;
			if (Store.<>f__switch$map20.TryGetValue(popupType, ref num))
			{
				if (num == 0)
				{
					this.lockGUI = false;
				}
			}
		}
	}

	// Token: 0x060016AD RID: 5805 RVA: 0x000105D2 File Offset: 0x0000E7D2
	public void PopupOk(string popupType, string choice)
	{
		if (popupType == "redeemcode")
		{
			App.Communicator.send(new RedeemMessage(choice));
		}
	}

	// Token: 0x060016AE RID: 5806 RVA: 0x000105F5 File Offset: 0x0000E7F5
	public void PopupGoldSelected(int itemId)
	{
		this.PurchaseItem(itemId, false);
	}

	// Token: 0x060016AF RID: 5807 RVA: 0x000105FF File Offset: 0x0000E7FF
	public void PopupShardsSelected(int itemId)
	{
		this.PurchaseItem(itemId, true);
	}

	// Token: 0x060016B0 RID: 5808 RVA: 0x000028DF File Offset: 0x00000ADF
	public void ShowShardsPurchase()
	{
	}

	// Token: 0x060016B1 RID: 5809 RVA: 0x00010609 File Offset: 0x0000E809
	public void ShowFullGamePurchase()
	{
		if (Store.ENABLE_PURCHASES)
		{
			App.Popups.ShowOkCancel(this, "fullgamepurchase", "Purchase full version", I18n.Text("Before purchasing, please be aware:\n<color=#ff8855>The {GAME_NAME} servers will be permanently shut down some time after July 1st, 2016.</color>\n\nTo upgrade to a full account, click the \"Upgrade\" button below to go to our website and purchase the game. When you're done, please restart the game."), "Upgrade", "Cancel");
		}
	}

	// Token: 0x060016B2 RID: 5810 RVA: 0x0001063E File Offset: 0x0000E83E
	public void ShowRedeem()
	{
		App.Popups.ShowTextEntry(this, "redeemcode", "Redeem code", "Please enter your additional items code below.", "Redeem", "Cancel");
	}

	// Token: 0x060016B3 RID: 5811 RVA: 0x0008D0F0 File Offset: 0x0008B2F0
	private IEnumerator ShowProcessingPopup()
	{
		int count = 0;
		for (;;)
		{
			string dots = ".";
			for (int i = 0; i < count; i++)
			{
				dots += ".";
			}
			App.Popups.ShowInfo("Processing", "Processing payment information. Please wait" + dots + "\n");
			count = (count + 1) % 3;
			yield return new WaitForSeconds(0.3f);
		}
		yield break;
	}

	// Token: 0x060016B4 RID: 5812 RVA: 0x0008D104 File Offset: 0x0008B304
	private IEnumerator ShowConnectingPopup()
	{
		int count = 0;
		float startTime = Time.time;
		do
		{
			string dots = ".";
			for (int i = 0; i < count; i++)
			{
				dots += ".";
			}
			App.Popups.ShowInfo("Connecting", "Connecting to the purchase system. Please wait" + dots + "\n");
			count = (count + 1) % 3;
			yield return new WaitForSeconds(0.3f);
		}
		while (Time.time - startTime <= 10f);
		string s = "Could not connect to the purchase system. Please check your Internet connection or try again later.";
		Log.warning("Store.connectionfailed: " + s);
		App.Popups.ShowOk(this, "connectionfailed", "Connection failed", s, "Ok");
		yield break;
	}

	// Token: 0x060016B5 RID: 5813 RVA: 0x0008D120 File Offset: 0x0008B320
	private Store.Item GetItemById(int itemId)
	{
		List<Store.Item>[] array = new List<Store.Item>[]
		{
			this.preConstDeckArr,
			this.faceUpArr,
			this.faceDownArr,
			this.avatarItems,
			this.idolItems
		};
		foreach (List<Store.Item> list in array)
		{
			foreach (Store.Item item in list)
			{
				if (item.itemId == itemId)
				{
					return item;
				}
			}
		}
		return null;
	}

	// Token: 0x060016B6 RID: 5814 RVA: 0x0008D1DC File Offset: 0x0008B3DC
	private void PurchaseItem(int itemId, bool useShards)
	{
		Store.Item itemById = this.GetItemById(itemId);
		MonoBehaviour.print("Pressed - itemId = " + itemById.itemId);
		this.hideInformation();
		this.allowBuy = false;
		if (!itemById.isCardPack)
		{
			this.enableAllowBuy();
		}
		this.storeSkin = (GUISkin)ResourceManager.Load("_GUISkins/StoreDisabled");
		if (this.cardRule != null)
		{
			this.stackCardRule();
			base.StartCoroutine(this.buyItem(itemById, useShards));
		}
		else
		{
			this.lastItemImageString = itemById.image;
			this.comm.send(new BuyStoreItemMessage(itemById.itemId, useShards));
		}
		if (itemById.type != "faceDownCards" && ((!useShards && App.MyProfile.ProfileData.gold >= itemById.priceGold) || (useShards && App.MyProfile.ProfileData.shards >= itemById.priceShards)))
		{
			itemById.inStock = false;
		}
	}

	// Token: 0x060016B7 RID: 5815 RVA: 0x0008D2EC File Offset: 0x0008B4EC
	private void drawSubMenu()
	{
		GUIPositioner subMenuPositioner = App.LobbyMenu.getSubMenuPositioner(1f, 2, 160f);
		Rect subMenuRect = App.LobbyMenu.getSubMenuRect(1f);
		GUI.DrawTexture(subMenuRect, ResourceManager.LoadTexture("ChatUI/menu_bar_sub"));
		GUIStyle button = this.lobbySkin.button;
		int fontSize = Screen.height / 36;
		this.lobbySkin.label.fontSize = fontSize;
		button.fontSize = fontSize;
		if (LobbyMenu.drawButton(subMenuPositioner.getButtonRect(-0.5f), "Buy", this.lobbySkin))
		{
			this.hideInformation();
			this.showBuyMenu();
		}
		if (LobbyMenu.drawButton(subMenuPositioner.getButtonRect(0.5f), "Sell", this.lobbySkin))
		{
			this.hideInformation();
			this.showSellMenu();
		}
		Store.drawStoreMarketplaceMenu(this.lobbySkin, false);
		GUI.enabled = true;
	}

	// Token: 0x060016B8 RID: 5816 RVA: 0x0008D3C4 File Offset: 0x0008B5C4
	public static void drawStoreMarketplaceMenu(GUISkin lobbySkin, bool enableStoreButton)
	{
		GUIPositioner subMenuPositioner = App.LobbyMenu.getSubMenuPositioner(1f, 2, 160f);
		int fontSize = lobbySkin.button.fontSize;
		int fontSize2 = lobbySkin.label.fontSize;
		GUIStyle button = lobbySkin.button;
		int fontSize3 = Screen.height / 36;
		lobbySkin.label.fontSize = fontSize3;
		button.fontSize = fontSize3;
		if (enableStoreButton)
		{
			if (LobbyMenu.drawButton(subMenuPositioner.getButtonRect(2f), "Store", lobbySkin))
			{
				App.LobbyMenu.loadSceneWithFade("_Store");
			}
		}
		else if (LobbyMenu.drawButton(subMenuPositioner.getButtonRect(2f), Store.GuiContentBlackMarket, lobbySkin))
		{
			App.LobbyMenu.loadSceneWithFade("_Marketplace");
		}
		lobbySkin.label.fontSize = fontSize2;
		lobbySkin.button.fontSize = fontSize;
	}

	// Token: 0x060016B9 RID: 5817 RVA: 0x0008D498 File Offset: 0x0008B698
	private void displayShopItemDescription(Store.Item item)
	{
		string type = item.type;
		if (type != null)
		{
			if (Store.<>f__switch$map21 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("cards", 0);
				dictionary.Add("randomCard", 1);
				Store.<>f__switch$map21 = dictionary;
			}
			int num;
			if (Store.<>f__switch$map21.TryGetValue(type, ref num))
			{
				if (num != 0)
				{
					if (num != 1)
					{
					}
				}
			}
		}
	}

	// Token: 0x060016BA RID: 5818 RVA: 0x0008D510 File Offset: 0x0008B710
	private void showSellMenu()
	{
		App.Communicator.send(new LibraryViewMessage());
		float num = (!this.showBuy) ? 0f : 0.65f;
		iTween.MoveTo(this.buyMenuObj, iTween.Hash(new object[]
		{
			"x",
			-0.5f,
			"time",
			1f,
			"easetype",
			iTween.EaseType.easeInExpo
		}));
		iTween.MoveTo(this.sellMenuObj, iTween.Hash(new object[]
		{
			"x",
			0f,
			"time",
			1f,
			"delay",
			num,
			"easetype",
			iTween.EaseType.easeOutExpo
		}));
		this.showSell = true;
		this.showBuy = false;
		if (this.cardRule != null)
		{
			this.stackCardRule();
		}
	}

	// Token: 0x060016BB RID: 5819 RVA: 0x0008D624 File Offset: 0x0008B824
	private void showBuyMenu()
	{
		float num = (!this.showSell) ? 0f : 0.65f;
		iTween.MoveTo(this.buyMenuObj, iTween.Hash(new object[]
		{
			"x",
			0f,
			"time",
			1f,
			"delay",
			num,
			"easetype",
			iTween.EaseType.easeOutExpo
		}));
		iTween.MoveTo(this.sellMenuObj, iTween.Hash(new object[]
		{
			"x",
			-0.5f,
			"time",
			1f,
			"easetype",
			iTween.EaseType.easeInExpo
		}));
		this.showBuy = true;
		this.showSell = false;
	}

	// Token: 0x060016BC RID: 5820 RVA: 0x0008D710 File Offset: 0x0008B910
	private IEnumerator buyItem(Store.Item item, bool useShards)
	{
		yield return new WaitForSeconds(0.5f);
		this.lastItemImageString = item.image;
		this.comm.send(new BuyStoreItemMessage(item.itemId, useShards));
		yield break;
	}

	// Token: 0x060016BD RID: 5821 RVA: 0x00010664 File Offset: 0x0000E864
	private void showSellButton()
	{
		this.sellButtonAlpha = 0.01f;
	}

	// Token: 0x060016BE RID: 5822 RVA: 0x0008D748 File Offset: 0x0008B948
	private void stackCardRule()
	{
		if (this.boughtCards != null && this.boughtCards.Count == 0)
		{
			this.boughtCards = null;
		}
		Vector3 a = Camera.main.ScreenToWorldPoint(new Vector3((float)Screen.width * 0.95f, (float)(Screen.height - 170), this.cardRule.transform.position.z));
		this.stackCardRule(a);
	}

	// Token: 0x060016BF RID: 5823 RVA: 0x0008D7C0 File Offset: 0x0008B9C0
	private void stackCardRule(Vector3 a)
	{
		float num = 0.8f;
		iTween.MoveTo(this.cardRule, iTween.Hash(new object[]
		{
			"x",
			a.x,
			"y",
			a.y,
			"z",
			a.z,
			"time",
			num,
			"easetype",
			iTween.EaseType.easeInExpo
		}));
		iTween.ScaleTo(this.cardRule, iTween.Hash(new object[]
		{
			"z",
			this.cardRule.transform.localScale.z * 0.14f,
			"x",
			this.cardRule.transform.localScale.x * 0.14f,
			"time",
			num,
			"easetype",
			iTween.EaseType.easeInExpo
		}));
		Object.Destroy(this.cardRule, num);
		this.cardRule = null;
		this.sellButtonAlpha = 0f;
		Store.removeBuyEffects();
	}

	// Token: 0x060016C0 RID: 5824 RVA: 0x0008D90C File Offset: 0x0008BB0C
	private void zoomInCard(int cardNum, float theY)
	{
		GameObject gameObject = this.createCardView(this.cardStackArr[cardNum]).gameObject;
		Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3((float)Screen.width * 0.95f, (float)Screen.height - theY - (float)Screen.height * 0.06f + this.boughtMerchScroll.y, gameObject.transform.position.z));
		if (this.cardRule != null)
		{
			Vector3 a;
			a..ctor(0f, 0f, 0f);
			for (int i = 0; i < this.cardStackArr.Count; i++)
			{
				if (this.cardStackArr[i].getId() == this.cardRule.GetComponent<CardView>().getCardInfo().getId())
				{
					float num = (float)Screen.height * 0.7f;
					float num2 = num * 1f;
					float num3 = num2 * 0.8f / 3f;
					float num4 = num3 * 1.4f;
					float num5 = (float)Screen.height * 0.16f + 10f + num4 * 0.67f * (float)i + (float)Screen.height * 0.03f;
					a = Camera.main.ScreenToWorldPoint(new Vector3((float)Screen.width * 0.95f, (float)Screen.height - num5 - (float)Screen.height * 0.06f + this.boughtMerchScroll.y, gameObject.transform.position.z));
					break;
				}
			}
			this.stackCardRule(a);
		}
		gameObject.transform.position = position;
		gameObject.transform.localEulerAngles = new Vector3(90f, 180f, 0f);
		gameObject.transform.localScale = new Vector3(0f, 1f, 0f);
		iTween.MoveTo(gameObject, iTween.Hash(new object[]
		{
			"x",
			this.cardDisplayPos.x,
			"y",
			this.cardDisplayPos.y,
			"z",
			this.cardDisplayPos.z,
			"time",
			0.6f,
			"easetype",
			iTween.EaseType.easeInExpo
		}));
		iTween.ScaleTo(gameObject, iTween.Hash(new object[]
		{
			"z",
			0.5568f,
			"x",
			0.3301f,
			"time",
			0.6f,
			"easetype",
			iTween.EaseType.easeInExpo
		}));
		this.cardRule = gameObject;
		this.sellButtonAlpha = 0.01f;
	}

	// Token: 0x060016C1 RID: 5825 RVA: 0x0008DBF8 File Offset: 0x0008BDF8
	private void FixedUpdate()
	{
		if (this.sellButtonAlpha > 0f && this.sellButtonAlpha < 1f)
		{
			this.sellButtonAlpha += 0.01f;
		}
		if (this.showNextCardTimer >= 0f && (this.showNextCardTimer -= Time.deltaTime) <= 0f)
		{
			this.showNextCardTimer = -1f;
			this.showNextCard();
		}
		if (this.enableAllowBuyTimer >= 0f && (this.enableAllowBuyTimer -= Time.deltaTime) <= 0f)
		{
			this.storeSkin = (GUISkin)ResourceManager.Load("_GUISkins/Store");
			this.allowBuy = true;
		}
		if (this.stackCardRuleTimer >= 0f && (this.stackCardRuleTimer -= Time.deltaTime) <= 0f)
		{
			this.stackCardRuleTimer = -1f;
		}
	}

	// Token: 0x060016C2 RID: 5826 RVA: 0x0008DCFC File Offset: 0x0008BEFC
	private void Update()
	{
		this.sellFrame.SetOffX(this.sellMenuObj.transform.position.x * (float)Screen.width);
		this.textTimer -= Time.deltaTime;
		if (this.transpTimer > 0f)
		{
			this.transpTimer -= Time.deltaTime;
			if (this.transpTimer <= 0f)
			{
				this.showCardRule(this.boughtCard);
			}
		}
		if (Input.GetMouseButtonDown(0) && !this.lockGUI && this.showNextCardTimer == -1f && this.stackCardRuleTimer == -1f)
		{
			RaycastHit raycastHit = default(RaycastHit);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, ref raycastHit) && raycastHit.collider.gameObject.name == "CardRule")
			{
				if (this.boughtCards != null && this.boughtCards.Count > 0)
				{
					this.stackCardRule();
					this.showNextCardTimer = 0.85f;
				}
				else if (this.cardRule == raycastHit.collider.gameObject)
				{
					this.stackCardRule();
				}
			}
		}
	}

	// Token: 0x060016C3 RID: 5827 RVA: 0x00010671 File Offset: 0x0000E871
	public void ButtonClicked(CardListPopup popup, ECardListButton button)
	{
		Log.info("ButtonClicked 1");
	}

	// Token: 0x060016C4 RID: 5828 RVA: 0x0008DE54 File Offset: 0x0008C054
	public void SellScrolls()
	{
		if (this.selectedCards.Count == 0)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < this.selectedCards.Count; i++)
		{
			Card card = this.selectedCards[i];
			int num2 = this.scrollReturnGold[card.getRarity()];
			for (int j = 0; j < card.level; j++)
			{
				num2 *= 3;
			}
			num += num2;
		}
		App.Popups.ShowOkCancel(this, "sellscrolls", "Selling scrolls", "The storekeeper offers you " + num + " gold. Do you accept?", "Sell", "Cancel");
	}

	// Token: 0x060016C5 RID: 5829 RVA: 0x0001067D File Offset: 0x0000E87D
	public void ButtonClicked(CardListPopup popup, ECardListButton button, List<Card> selectedCards)
	{
		this.selectedCards = selectedCards;
		this.SellScrolls();
		this.lockGUI = true;
	}

	// Token: 0x060016C6 RID: 5830 RVA: 0x00010693 File Offset: 0x0000E893
	public void ItemButtonClicked(CardListPopup popup, Card card)
	{
		Log.info("ItemButtonClicked");
	}

	// Token: 0x060016C7 RID: 5831 RVA: 0x000028DF File Offset: 0x00000ADF
	public void ItemHovered(CardListPopup popup, Card card)
	{
	}

	// Token: 0x060016C8 RID: 5832 RVA: 0x0001069F File Offset: 0x0000E89F
	public void ItemClicked(CardListPopup popup, Card card)
	{
		if (popup == this.sellFrame)
		{
			this.sellCardOverlay.Show(card);
		}
	}

	// Token: 0x060016C9 RID: 5833 RVA: 0x0008DF04 File Offset: 0x0008C104
	private void drawStoreItemImage(Store.Item item, Rect rect)
	{
		GUIContent guicontent = new GUIContent(this.getTexture(item));
		if (!item.isDemoPurchasable())
		{
			guicontent.lockDemo();
		}
		if (item.type == "avatar_part")
		{
			float num = (float)guicontent.image.width * 0.5f;
			float num2 = (rect.width - num) * 0.5f;
			rect..ctor(rect);
			rect.x += num2;
			rect.width = num;
			rect.height = num * (float)guicontent.image.height / (float)guicontent.image.width;
		}
		if (guicontent.image != null)
		{
			App.GUI.DrawTexture(rect, guicontent);
		}
		if (GUI.Button(rect, string.Empty))
		{
			this.showInformation(item);
		}
	}

	// Token: 0x060016CA RID: 5834 RVA: 0x000106BE File Offset: 0x0000E8BE
	private Texture getTexture(Store.Item item)
	{
		if (item.type == "faceUpCards")
		{
			return App.AssetLoader.LoadCardImage(item.image);
		}
		return ResourceManager.LoadTexture(item.image);
	}

	// Token: 0x060016CB RID: 5835 RVA: 0x0008DFE0 File Offset: 0x0008C1E0
	private int drawItemsDefault(List<Store.Item> items, string title, float y)
	{
		if (items.Count == 0)
		{
			return 0;
		}
		float num = (float)(10 + Screen.height / 72);
		GUI.skin.label.fontSize = (int)num;
		int fontSize = GUI.skin.label.fontSize;
		float num2 = (float)Screen.height * 0.7f;
		float num3 = num2 * 0.8f;
		float num4 = num3 * 0.8f / 3f;
		float num5 = num4 * 1.4f;
		Color textColor = GUI.skin.label.normal.textColor;
		GUI.skin = (GUISkin)ResourceManager.Load("_GUISkins/ChatUI");
		GUI.skin.label.normal.textColor = textColor;
		GUI.skin.label.alignment = 1;
		GUI.skin.label.fontSize = (int)Math.Round((double)((float)fontSize * 1.4f));
		GUI.Label(new Rect(0f, y, num3 - 40f, 100f), title);
		GUI.skin.label.fontSize = fontSize;
		int num6 = 3;
		int num7 = 1 + (items.Count - 1) / num6;
		for (int i = 0; i < items.Count; i++)
		{
			Store.Item item = items[i];
			int num8 = i / num6;
			int num9 = i % num6;
			float num10 = (num3 - (float)Screen.height * 0.075f - num4 * (float)num6) / (float)(num6 * 2) * (((float)num9 + 1.4f) * 2f);
			float num11 = (num3 - (float)Screen.height * 0.075f - num4 * (float)num6) / (float)(num6 * 2) * 2f * (float)num8;
			GUI.BeginGroup(new Rect(num4 * (float)num9 + num10, y + num5 * (float)num8 + num11 + (float)Screen.height * 0.015f + (float)Screen.height * 0.035f, num4, num5));
			GUI.Box(new Rect(0f, 0f, num4, num5), string.Empty);
			Rect rect;
			rect..ctor(5f, 5f, num4 - 10f, (num4 - 10f) * 0.75f);
			if (item.type == "idol")
			{
				rect = GeomUtil.scaleCentered(rect, 0.8f, 1.2f);
				rect.y += rect.height * 0.1f;
			}
			this.drawStoreItemImage(item, rect);
			Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(0f, 0f));
			bool flag = GeomUtil.getTranslated(rect, vector.x, vector.y).Contains(GUIUtil.getScreenMousePos());
			if (!item.inStock)
			{
				GUI.DrawTexture(new Rect(5f, 5f, num4 - 10f, (num4 - 10f) * 0.75f), ResourceManager.LoadTexture("Store/sold_out"));
			}
			if (item.image.Contains("shop_item_resource_bg"))
			{
				GUI.DrawTexture(new Rect(2f + (float)Screen.height * 0.045f, 2f + (float)Screen.height * 0.03f, num4 * 0.7f - 4f, (num4 * 0.7f - 4f) * 0.75f), ResourceManager.LoadTexture("Store/shop_item_icon"));
			}
			if (!string.IsNullOrEmpty(item.name))
			{
				GUI.DrawTexture(new Rect(2f, num5 * 0.42f, num4 - 4f, num5 * 0.56f), ResourceManager.LoadTexture("Store/shop_thumb_gradient3"));
			}
			GUI.skin.label.fontSize = (int)Math.Round((double)((float)fontSize * 0.65f));
			GUI.skin.label.alignment = 1;
			GUI.Label(new Rect(4f, num5 * 0.55f, num4 - 4f, 30f), item.name);
			GUI.skin.label.fontSize = (int)Math.Round((double)((float)fontSize * 0.8f));
			GUI.skin.label.alignment = 0;
			GUI.DrawTexture(new Rect(num4 * 0.07f, num5 * 0.7f, num4 / 8f, num4 / 8f), ResourceManager.LoadTexture("Shared/gold_icon_small"));
			GUI.Label(new Rect(num4 * 0.07f + num4 / 8f, num5 * 0.7f, num4, 20f), " " + item.priceGoldString());
			if (item.priceShards > 0)
			{
				GUI.DrawTexture(new Rect(num4 / 1.8f, num5 * 0.7f, num4 / 8f, num4 / 8f), ResourceManager.LoadTexture("Shared/voidshard_icon_small"));
				GUI.Label(new Rect(num4 / 1.8f + num4 / 8f, num5 * 0.7f, num4 / 2f - num4 * 0.07f, 20f), " " + item.priceCashString());
			}
			GUI.skin.label.alignment = 2;
			GUI.skin = this.storeSkin;
			GUI.enabled = (item.inStock && (item.isDemoPurchasable() || App.IsPremium()));
			if (GUI.Button(new Rect(num4 * 0.07f, num5 * 0.8f, num4 * 0.62f, num5 * 0.15f), (!item.inStock) ? this.contentSoldOut : this.contentUnlocked))
			{
				this.buyButtonClicked(item);
			}
			GUI.enabled = true;
			GUIStyle guistyle = (!flag) ? this.storeSkin.button : this.storeHoverStyle;
			if (GUI.Button(new Rect(num4 * 0.07f + num4 * 0.66f, num5 * 0.8f, num4 * 0.2f, num5 * 0.15f), "?", guistyle))
			{
				this.showInformation(item);
			}
			GUI.skin = (GUISkin)ResourceManager.Load("_GUISkins/ChatUI");
			GUI.EndGroup();
			GUI.skin.label.fontSize = fontSize;
		}
		return (int)((float)Screen.height * 0.035f + (num5 * (float)num7 + (num3 - (float)Screen.height * 0.075f - num4 * (float)num6) / (float)(num6 * 2) * 2f * 1f + (float)Screen.height * 0.03f));
	}

	// Token: 0x040013F0 RID: 5104
	private const int MERCH_SCROLL = 0;

	// Token: 0x040013F1 RID: 5105
	public static bool ENABLE_PURCHASES = true;

	// Token: 0x040013F2 RID: 5106
	public RenderTexture cardRenderTexture;

	// Token: 0x040013F3 RID: 5107
	private GUISkin storeSkin;

	// Token: 0x040013F4 RID: 5108
	private GUIStyle storeHoverStyle;

	// Token: 0x040013F5 RID: 5109
	private GUISkin lobbySkin;

	// Token: 0x040013F6 RID: 5110
	private List<Store.Item> faceUpArr;

	// Token: 0x040013F7 RID: 5111
	private List<Store.Item> faceDownArr;

	// Token: 0x040013F8 RID: 5112
	private List<Store.Item> preConstDeckArr;

	// Token: 0x040013F9 RID: 5113
	private List<Store.Item> avatarItems;

	// Token: 0x040013FA RID: 5114
	private List<Store.Item> idolItems;

	// Token: 0x040013FB RID: 5115
	private float merchScrollHeight;

	// Token: 0x040013FC RID: 5116
	private float boughtMerchScrollHeight;

	// Token: 0x040013FD RID: 5117
	private Communicator comm;

	// Token: 0x040013FE RID: 5118
	private bool scrollsRecieved;

	// Token: 0x040013FF RID: 5119
	private GameObject opponentCardRule;

	// Token: 0x04001400 RID: 5120
	private GameObject cardRule;

	// Token: 0x04001401 RID: 5121
	public ResourceManager resources = new ResourceManager();

	// Token: 0x04001402 RID: 5122
	private List<Card> selectedCards;

	// Token: 0x04001403 RID: 5123
	private GUISkin buttonSkin;

	// Token: 0x04001404 RID: 5124
	private Vector3 cardDisplayPos;

	// Token: 0x04001405 RID: 5125
	private Vector3 cardEffectPos;

	// Token: 0x04001406 RID: 5126
	private CardListPopup sellFrame;

	// Token: 0x04001407 RID: 5127
	private EList<Card> playerLibrary = new EList<Card>();

	// Token: 0x04001408 RID: 5128
	private DeckSorter sorter = new DeckSorter().byColor().byName().byLevel();

	// Token: 0x04001409 RID: 5129
	private Card boughtCard;

	// Token: 0x0400140A RID: 5130
	private List<Card> boughtCards;

	// Token: 0x0400140B RID: 5131
	private List<Card> cardStackArr = new List<Card>();

	// Token: 0x0400140C RID: 5132
	private bool showBuy;

	// Token: 0x0400140D RID: 5133
	private bool showSell;

	// Token: 0x0400140E RID: 5134
	private bool allowBuy = true;

	// Token: 0x0400140F RID: 5135
	private GameObject sellMenuObj;

	// Token: 0x04001410 RID: 5136
	private GameObject buyMenuObj;

	// Token: 0x04001411 RID: 5137
	private float sellButtonAlpha;

	// Token: 0x04001412 RID: 5138
	private int[] scrollReturnGold;

	// Token: 0x04001413 RID: 5139
	private CardOverlay cardOverlay;

	// Token: 0x04001414 RID: 5140
	private CardOverlay sellCardOverlay;

	// Token: 0x04001415 RID: 5141
	private Vector2 merchScroll;

	// Token: 0x04001416 RID: 5142
	private Vector2 showInfoScroll;

	// Token: 0x04001417 RID: 5143
	private Vector2 boughtMerchScroll;

	// Token: 0x04001418 RID: 5144
	private float showNextCardTimer = -1f;

	// Token: 0x04001419 RID: 5145
	private float enableAllowBuyTimer = -1f;

	// Token: 0x0400141A RID: 5146
	private float stackCardRuleTimer = -1f;

	// Token: 0x0400141B RID: 5147
	private string currentRarityString = string.Empty;

	// Token: 0x0400141C RID: 5148
	private int currentRarity;

	// Token: 0x0400141D RID: 5149
	private int currentLevel;

	// Token: 0x0400141E RID: 5150
	private bool boughtPack;

	// Token: 0x0400141F RID: 5151
	private string lastItemImageString;

	// Token: 0x04001420 RID: 5152
	private float textTimer;

	// Token: 0x04001421 RID: 5153
	private string textText;

	// Token: 0x04001422 RID: 5154
	private float transpTimer;

	// Token: 0x04001423 RID: 5155
	public bool lockGUI;

	// Token: 0x04001424 RID: 5156
	private int ownedCardsOfTypeCount = -1;

	// Token: 0x04001425 RID: 5157
	private Store.Item showInfo;

	// Token: 0x04001426 RID: 5158
	private GUIContent contentUnlocked = new GUIContent("Buy");

	// Token: 0x04001427 RID: 5159
	private GUIContent contentLocked = new GUIContent("Buy").lockDemo();

	// Token: 0x04001428 RID: 5160
	private GUIContent contentSoldOut = new GUIContent("Sold out");

	// Token: 0x04001429 RID: 5161
	private Store.PurchasingItem purchasingItem;

	// Token: 0x0400142A RID: 5162
	private static GUIContent GuiContentBlackMarket = new GUIContent("Black market");

	// Token: 0x02000401 RID: 1025
	private enum PurchasingItem
	{
		// Token: 0x04001431 RID: 5169
		NONE,
		// Token: 0x04001432 RID: 5170
		SHARDS,
		// Token: 0x04001433 RID: 5171
		FULL_GAME
	}

	// Token: 0x02000402 RID: 1026
	private class Item
	{
		// Token: 0x060016CE RID: 5838 RVA: 0x0008E680 File Offset: 0x0008C880
		public Item(int itemId, string type, int priceGold, int priceCash, string description, string image, bool isPublic, bool isPurchased, int cardTypeId, string name, long[] cardTypeIds, string expires)
		{
			this.itemId = itemId;
			this.type = type;
			this.priceGold = priceGold;
			this.priceShards = priceCash;
			this.description = description;
			this.image = image;
			this.cardTypeId = cardTypeId;
			this.name = name;
			this.inStock = !isPurchased;
			this.expires = expires;
			this.isCardPack = false;
			ArrayList arrayList = new ArrayList();
			if (cardTypeIds.Length > 0)
			{
				foreach (long num in cardTypeIds)
				{
					Hashtable hashtable = new Hashtable();
					bool flag = true;
					for (int j = 0; j < arrayList.Count; j++)
					{
						string text = CardTypeManager.getInstance().get((int)num).name.ToString();
						if (((Hashtable)arrayList[j])["name"] == text)
						{
							((Hashtable)arrayList[j])["count"] = (int)((Hashtable)arrayList[j])["count"] + 1;
							flag = false;
						}
					}
					if (flag)
					{
						hashtable.Add("name", CardTypeManager.getInstance().get((int)num).name.ToString());
						hashtable.Add("count", 1);
						arrayList.Add(hashtable);
					}
				}
				this.cardNames = "\n\nScrolls included:\n";
				for (int k = 0; k < arrayList.Count; k++)
				{
					string text2 = this.cardNames;
					this.cardNames = string.Concat(new object[]
					{
						text2,
						((Hashtable)arrayList[k])["count"],
						"x ",
						((Hashtable)arrayList[k])["name"],
						"\n"
					});
				}
			}
		}

		// Token: 0x060016CF RID: 5839 RVA: 0x0008E880 File Offset: 0x0008CA80
		public bool isDemoPurchasable()
		{
			return this.type != "avatar_outfit" && this.type != "preConstDeck" && this.type != "idol" && this.type != "avatar_part";
		}

		// Token: 0x060016D0 RID: 5840 RVA: 0x0001070D File Offset: 0x0000E90D
		public string priceGoldString()
		{
			return this.priceGold.ToString();
		}

		// Token: 0x060016D1 RID: 5841 RVA: 0x0001071A File Offset: 0x0000E91A
		public string priceCashString()
		{
			return this.priceShards.ToString();
		}

		// Token: 0x060016D2 RID: 5842 RVA: 0x0008E8E0 File Offset: 0x0008CAE0
		public static Store.Item Card()
		{
			long[] cardTypeIds = new long[]
			{
				1L,
				2L,
				3L
			};
			return new Store.Item(0, "shop_item_icon", 10, 10, "description", "Store/shop_item_icon", false, false, 0, string.Empty, cardTypeIds, string.Empty);
		}

		// Token: 0x04001434 RID: 5172
		public string image;

		// Token: 0x04001435 RID: 5173
		public string type;

		// Token: 0x04001436 RID: 5174
		public int priceGold;

		// Token: 0x04001437 RID: 5175
		public int priceShards;

		// Token: 0x04001438 RID: 5176
		public bool inStock = true;

		// Token: 0x04001439 RID: 5177
		public string description;

		// Token: 0x0400143A RID: 5178
		public int itemId;

		// Token: 0x0400143B RID: 5179
		public int cardTypeId;

		// Token: 0x0400143C RID: 5180
		public string name;

		// Token: 0x0400143D RID: 5181
		public string cardNames;

		// Token: 0x0400143E RID: 5182
		public string expires;

		// Token: 0x0400143F RID: 5183
		public bool isCardPack;

		// Token: 0x04001440 RID: 5184
		public AvatarInfo avatar;

		// Token: 0x04001441 RID: 5185
		public AvatarPart avatarPart;
	}
}
