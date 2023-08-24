using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x0200038C RID: 908
public class Popups : MonoBehaviour, IOverlayClickCallback
{
	// Token: 0x0600140F RID: 5135 RVA: 0x0000ECDF File Offset: 0x0000CEDF
	public bool IsShowingPopup()
	{
		return this.currentPopupType != Popups.PopupType.NONE || this.showReconnectPopup;
	}

	// Token: 0x06001410 RID: 5136 RVA: 0x0000ECF5 File Offset: 0x0000CEF5
	public void ShowReconnectPopup()
	{
		this.overlay.enabled = true;
		this.showReconnectPopup = true;
	}

	// Token: 0x06001411 RID: 5137 RVA: 0x0000ED0A File Offset: 0x0000CF0A
	public void ShowOverlay()
	{
		this.overlay.enabled = true;
	}

	// Token: 0x06001412 RID: 5138 RVA: 0x0000ED18 File Offset: 0x0000CF18
	public void HideOverlay()
	{
		this.overlay.enabled = false;
	}

	// Token: 0x06001413 RID: 5139 RVA: 0x0000ED26 File Offset: 0x0000CF26
	public void SetErrorText(string errorText)
	{
		this.errorText = errorText;
	}

	// Token: 0x06001414 RID: 5140 RVA: 0x0000ED2F File Offset: 0x0000CF2F
	private static GUIContent content(string s)
	{
		return (s == null) ? null : new GUIContent(s);
	}

	// Token: 0x06001415 RID: 5141 RVA: 0x00079314 File Offset: 0x00077514
	public void ShowOkCancel(IOkCancelCallback okCancelCallback, string popupType, string header, string description, string okText, string cancelText)
	{
		this.ShowPopup(Popups.PopupType.OK_CANCEL);
		this.okCallback = okCancelCallback;
		this.cancelCallback = okCancelCallback;
		this.popupType = popupType;
		this.header = header;
		this.description = description;
		this.okText = Popups.content(okText);
		this.cancelText = Popups.content(cancelText);
		this.isLarge = Popups.needsLarge(description);
	}

	// Token: 0x06001416 RID: 5142 RVA: 0x00079374 File Offset: 0x00077574
	public void ShowOkCancelRenderer(IOkCancelCallback okCancelCallback, string popupType, string header, Rect rect, Action renderer, string okText, string cancelText)
	{
		this.ShowPopup(Popups.PopupType.OK_CANCEL);
		this.okCallback = okCancelCallback;
		this.cancelCallback = okCancelCallback;
		this.popupType = popupType;
		this.header = header;
		this.description = string.Empty;
		this.okText = Popups.content(okText);
		this.cancelText = Popups.content(cancelText);
		this.SetRectAndRenderer(renderer, rect);
	}

	// Token: 0x06001417 RID: 5143 RVA: 0x0000ED43 File Offset: 0x0000CF43
	public Popups SetRectAndRenderer(Action renderer, Rect rect)
	{
		this.popupRect = new Rect?(rect);
		this.popupRenderer = renderer;
		return this;
	}

	// Token: 0x06001418 RID: 5144 RVA: 0x000793D4 File Offset: 0x000775D4
	public void ShowOkChoiceCancel(IOkChoiceCancelCallback callback, string popupType, string header, string description, string okText1, string okText2)
	{
		this.ShowPopup(Popups.PopupType.OK_CHOICE_CANCEL);
		this.okChoiceCallback = callback;
		this.cancelCallback = callback;
		this.popupType = popupType;
		this.header = header;
		this.description = description;
		this.okText = Popups.content(okText1);
		this.cancelText = Popups.content(okText2);
	}

	// Token: 0x06001419 RID: 5145 RVA: 0x00079428 File Offset: 0x00077628
	public void ShowGoldShardsSelector(IGoldShardsCallback callback, string popupType, string itemName, int itemId, int costGold, int costShards, int currentGold, int currentShards)
	{
		this.ShowPopup(Popups.PopupType.GOLD_SHARDS_SELECT);
		this.goldShardsCallback = callback;
		this.cancelCallback = callback;
		this.popupType = popupType;
		this.header = "Confirm purchase";
		this.description = "<color=#aaaaaa>You are about to purchase:</color> <color=#ffeeaa>" + itemName + "</color>\n\n";
		this.goldShardsItemId = itemId;
		this.goldShardGoldCost = costGold;
		this.goldShardShardsCost = costShards;
		this.goldShardGoldCurrent = currentGold;
		this.goldShardShardsCurrent = currentShards;
	}

	// Token: 0x0600141A RID: 5146 RVA: 0x0007949C File Offset: 0x0007769C
	public void ShowOk(IOkCallback okCallback, string popupType, string header, string description, string okText)
	{
		this.ShowPopup(Popups.PopupType.OK);
		this.okCallback = okCallback;
		this.popupType = popupType;
		this.header = header;
		this.description = description;
		this.okText = Popups.content("Ok");
		this.isLarge = Popups.needsLarge(description);
	}

	// Token: 0x0600141B RID: 5147 RVA: 0x0000ED59 File Offset: 0x0000CF59
	public void SetLarge(bool leftAlign)
	{
		this.isLarge = true;
		this.largeLeftAlign = leftAlign;
	}

	// Token: 0x0600141C RID: 5148 RVA: 0x0000ED69 File Offset: 0x0000CF69
	private static bool needsLarge(string description)
	{
		return description.Length >= 320;
	}

	// Token: 0x0600141D RID: 5149 RVA: 0x0000ED7B File Offset: 0x0000CF7B
	private void _onDemoOk(string popupType)
	{
		App.SceneValues.store.openBuyGamePopup = true;
		SceneLoader.loadScene("_Store");
	}

	// Token: 0x0600141E RID: 5150 RVA: 0x0000ED97 File Offset: 0x0000CF97
	public void ShowDemoOk()
	{
		this.ShowOkCancel(new OkCancelCallback(new Action<string>(this._onDemoOk), null), "__demoFail", "Feature locked", "This feature is locked in the demo version. You need to purchase the full game to use this feature.", "Purchase", "Cancel");
	}

	// Token: 0x0600141F RID: 5151 RVA: 0x000794EC File Offset: 0x000776EC
	public void ShowSellCard(IOkStringCancelCallback okCancelCallback, string popupType, Card card, int lowestPrice, int suggestedPrice, int amountForSale, float tax)
	{
		this.ShowPopup(Popups.PopupType.SELL_CARD);
		this.popupType = popupType;
		this.header = string.Empty;
		this.okStringCallback = okCancelCallback;
		this.cancelCallback = okCancelCallback;
		float num2;
		float num = num2 = (float)Screen.height * 0.63f;
		this.cardOverlay.Init(this.cardRenderTexture, new Rect((float)Screen.width * 0.5f - (float)Screen.height * 0.19f - num2 / 2f, (float)Screen.height * 0.51f - num / 2f, num2, num), 3);
		this.cardOverlay.Show(card);
		this.cardOverlay.GetCardView().enableShowStats();
		this.sellCardLowestPrice = lowestPrice;
		this.textEntry = string.Empty + suggestedPrice;
		this.sellCardAmountForSale = amountForSale;
		this.sellCardTax = (float)((int)(tax * 100f));
		this.okText = Popups.content("Put up for sale").lockDemo();
		this.cancelText = Popups.content("Cancel");
	}

	// Token: 0x06001420 RID: 5152 RVA: 0x000795F8 File Offset: 0x000777F8
	public void ShowScrollText(IOkCallback okCallback, string popupType, string header, string description, string okText)
	{
		this.ShowPopup(Popups.PopupType.SCROLL_TEXT);
		this.okCallback = okCallback;
		this.popupType = popupType;
		this.header = header;
		this.description = description;
		this.okText = Popups.content("Ok");
		this.scrollTextScroll = Vector2.zero;
	}

	// Token: 0x06001421 RID: 5153 RVA: 0x00079648 File Offset: 0x00077848
	public void ShowTextImage(IOkCallback okCallback, string popupType, string header, string description, Texture2D image, string okText)
	{
		this.ShowPopup(Popups.PopupType.TEXT_IMAGE);
		this.okCallback = okCallback;
		this.popupType = popupType;
		this.header = header;
		this.description = description;
		this.image = image;
		this.okText = Popups.content("Ok");
		this.scrollTextScroll = Vector2.zero;
	}

	// Token: 0x06001422 RID: 5154 RVA: 0x0000EDCA File Offset: 0x0000CFCA
	public void ShowDeckSelector(IDeckCallback deckChosenCallback, ICancelCallback cancelCallback, List<DeckInfo> deckList, bool showDeleteDeckIcon, bool allowInvalidClicks)
	{
		this.ShowDeckSelector(deckChosenCallback, cancelCallback, deckList, showDeleteDeckIcon, allowInvalidClicks, null);
	}

	// Token: 0x06001423 RID: 5155 RVA: 0x000796A0 File Offset: 0x000778A0
	public void ShowDeckSelector(IDeckCallback deckChosenCallback, ICancelCallback cancelCallback, List<DeckInfo> deckList, bool showDeleteDeckIcon, bool allowInvalidClicks, string okText)
	{
		this.ShowPopup(Popups.PopupType.DECK_SELECTOR);
		this.deckFilter = string.Empty;
		this.deckChosenCallback = deckChosenCallback;
		this.cancelCallback = cancelCallback;
		this.popupType = "deckselector";
		this.deckList = deckList;
		this.header = "Choose deck";
		this.okText = Popups.content(okText);
		this.showDeleteDeckIcon = showDeleteDeckIcon;
		this.deckScroll = Vector2.zero;
		this.allowInvalidClicks = allowInvalidClicks;
	}

	// Token: 0x06001424 RID: 5156 RVA: 0x0000EDDA File Offset: 0x0000CFDA
	public void ShowTowerChallengeSelector(GameActionManager towerChallengeChosenCallback, ICancelCallback cancelCallback)
	{
		this.cancelCallback = cancelCallback;
		this.towerChallengeChosenCallback = towerChallengeChosenCallback;
		this.header = "Select Trial";
		this.popupType = "deckselector";
		this.ShowPopup(Popups.PopupType.TOWER_CHALLENGE_SELECTOR);
	}

	// Token: 0x06001425 RID: 5157 RVA: 0x0000EE08 File Offset: 0x0000D008
	public void ShowTutorialChallengeSelector(GameActionManager towerChallengeChosenCallback, ICancelCallback cancelCallback)
	{
		this.cancelCallback = cancelCallback;
		this.towerChallengeChosenCallback = towerChallengeChosenCallback;
		this.header = "Select Tutorial";
		this.popupType = "deckselector";
		this.ShowPopup(Popups.PopupType.TUTORIAL_CHALLENGE_SELECTOR);
	}

	// Token: 0x06001426 RID: 5158 RVA: 0x00079714 File Offset: 0x00077914
	public void ShowSaveDeck(IDeckSaveCallback callback, string loadedDeckName, string problems)
	{
		this.ShowPopup(Popups.PopupType.SAVE_DECK);
		this.deckSaveCallback = callback;
		this.okStringCallback = callback;
		this.cancelCallback = callback;
		this.popupType = "savedeck";
		this.saveDeckName = loadedDeckName;
		this.saveDeckProblems = problems;
		this.header = "Save deck";
		this.description = "Please name your deck";
		this.okText = Popups.content("Save");
		this.cancelText = Popups.content("Cancel");
		this.focusInput = true;
		this.useAsAIDeck = false;
	}

	// Token: 0x06001427 RID: 5159 RVA: 0x0000EE36 File Offset: 0x0000D036
	public void ShowTextArea(IOkStringCancelCallback callback, string popupType, string header, string description, string okText, string cancelText, string initialEntryText, bool canModify, bool closeOnOK)
	{
		this.ShowTextEntry(callback, popupType, header, description, okText, cancelText, initialEntryText);
		this.ShowPopup((!canModify) ? Popups.PopupType.TEXT_AREA_READONLY : Popups.PopupType.TEXT_AREA);
		this.closeOnOK = closeOnOK;
	}

	// Token: 0x06001428 RID: 5160 RVA: 0x0000EE67 File Offset: 0x0000D067
	public void ShowTextEntry(IOkStringCancelCallback callback, string popupType, string header, string description, string okText, string cancelText)
	{
		this.ShowTextEntry(callback, popupType, header, description, okText, cancelText, string.Empty);
	}

	// Token: 0x06001429 RID: 5161 RVA: 0x0007979C File Offset: 0x0007799C
	public void ShowTextEntry(IOkStringCancelCallback callback, string popupType, string header, string description, string okText, string cancelText, string initialEntryText)
	{
		this.ShowPopup(Popups.PopupType.TEXT_ENTRY);
		this.textEntry = (initialEntryText ?? string.Empty);
		this.okStringCallback = callback;
		this.cancelCallback = callback;
		this.popupType = popupType;
		this.header = header;
		this.description = description;
		this.okText = Popups.content(okText);
		this.cancelText = Popups.content(cancelText);
		this.focusInput = true;
		this.scrollTextScroll = Vector2.zero;
	}

	// Token: 0x0600142A RID: 5162 RVA: 0x00079818 File Offset: 0x00077A18
	public void ShowJoinRoom(IJoinRoomCallback callback, List<ChatRooms.JoinableRoomDesc> roomList)
	{
		this.ShowPopup(Popups.PopupType.JOIN_ROOM);
		this.joinRoomCallback = callback;
		this.cancelCallback = callback;
		this.popupType = "joinroom";
		this.roomList = roomList;
		this.roomName = string.Empty;
		this.header = "Chat rooms";
		this.okText = Popups.content("Join");
		this.roomScroll = Vector2.zero;
	}

	// Token: 0x0600142B RID: 5163 RVA: 0x0000EE7D File Offset: 0x0000D07D
	public void ShowMultibutton(IOkStringCancelCallback callback, string popupType, string header, GUIContent[] buttonList)
	{
		this.ShowPopup(Popups.PopupType.MULTIBUTTON);
		this.okStringCallback = callback;
		this.cancelCallback = callback;
		this.popupType = popupType;
		this.header = header;
		this.buttonList = buttonList;
	}

	// Token: 0x0600142C RID: 5164 RVA: 0x0000EEAA File Offset: 0x0000D0AA
	public void ShowInfo(string header, string description)
	{
		this.ShowPopup(Popups.PopupType.INFO_PROGCLOSE);
		this.popupType = "info";
		this.header = header;
		this.description = description;
	}

	// Token: 0x0600142D RID: 5165 RVA: 0x00079880 File Offset: 0x00077A80
	private void ShowShardPurchasePasswordEntry()
	{
		this.ShowPopup(Popups.PopupType.PURCHASE_PASSWORD_ENTRY);
		this.purchasePassword = string.Empty;
		this.header = "Verify your identity";
		this.description = "Please enter your password.";
		this.okText = Popups.content("Purchase");
		this.cancelText = Popups.content("Cancel");
		this.focusInput = true;
	}

	// Token: 0x0600142E RID: 5166 RVA: 0x0000EECC File Offset: 0x0000D0CC
	public void KillReconnectPopup()
	{
		if (this.showReconnectPopup)
		{
			this.overlay.enabled = false;
			this.showReconnectPopup = false;
		}
	}

	// Token: 0x0600142F RID: 5167 RVA: 0x0000EEEC File Offset: 0x0000D0EC
	public void KillCurrentPopup()
	{
		if (!this.showReconnectPopup)
		{
			this.HidePopup();
		}
	}

	// Token: 0x06001430 RID: 5168 RVA: 0x0000EEFF File Offset: 0x0000D0FF
	public void RequestPopupClose()
	{
		if (!this.showReconnectPopup && this.currentPopupType != Popups.PopupType.INFO_PROGCLOSE)
		{
			this.HidePopup();
		}
	}

	// Token: 0x06001431 RID: 5169 RVA: 0x0000EF1E File Offset: 0x0000D11E
	public void UpdateDecks(List<DeckInfo> deckList)
	{
		this.deckList = deckList;
	}

	// Token: 0x06001432 RID: 5170 RVA: 0x000798E0 File Offset: 0x00077AE0
	private void Start()
	{
		this.overlay = new GameObject("PopupBlackOverlay").AddComponent<GUIBlackOverlayButton>();
		this.overlay.Init(this, 5, false);
		this.overlay.enabled = false;
		Object.DontDestroyOnLoad(this.overlay.gameObject);
		this.regularUISkin = (GUISkin)ResourceManager.Load("_GUISkins/RegularUI");
		this.closeButtonSkin = (GUISkin)ResourceManager.Load("_GUISkins/CloseButton");
		this.emptySkin = (GUISkin)ResourceManager.Load("_GUISkins/EmptySkin");
		this.purchaseSkin = (GUISkin)ResourceManager.Load("_GUISkins/PurchaseSkin");
		this.exportButtonStyle = new GUIStyle();
		this.exportButtonStyle.normal.background = ResourceManager.LoadTexture("Icons/Deck/export_icon");
		this.exportButtonStyle.hover.background = ResourceManager.LoadTexture("Icons/Deck/export_icon_mouseover");
		RectOffset margin = this.exportButtonStyle.margin;
		int num = 0;
		this.exportButtonStyle.margin.right = num;
		num = num;
		this.exportButtonStyle.margin.top = num;
		num = num;
		this.exportButtonStyle.margin.left = num;
		margin.bottom = num;
		RectOffset padding = this.exportButtonStyle.padding;
		num = 0;
		this.exportButtonStyle.padding.right = num;
		num = num;
		this.exportButtonStyle.padding.top = num;
		num = num;
		this.exportButtonStyle.padding.left = num;
		padding.bottom = num;
		this.importButtonStyle = new GUIStyle(this.exportButtonStyle);
		this.importButtonStyle.normal.background = ResourceManager.LoadTexture("Icons/Deck/import_icon");
		this.importButtonStyle.hover.background = ResourceManager.LoadTexture("Icons/Deck/import_icon_mouseover");
		string[] strings = new string[]
		{
			"01",
			"02",
			"03",
			"04",
			"05",
			"06",
			"07",
			"08",
			"09",
			"10",
			"11",
			"12"
		};
		string[] array = new string[11];
		int year = DateTime.Now.Year;
		for (int i = 0; i < 11; i++)
		{
			array[i] = (year % 100 + i).ToString();
		}
		this.monthDrop = new GameObject("Dropdown").AddComponent<Dropdown>();
		this.monthDrop.Init(strings, 6f, true, false, 4);
		Object.DontDestroyOnLoad(this.monthDrop);
		this.yearDrop = new GameObject("Dropdown").AddComponent<Dropdown>();
		this.yearDrop.Init(array, 6f, true, false, 4);
		Object.DontDestroyOnLoad(this.yearDrop);
		this.countryDrop = new GameObject("Dropdown").AddComponent<Dropdown>();
		this.countryDrop.Init(null, 6f, false, true, 3);
		Object.DontDestroyOnLoad(this.countryDrop);
		this.currentPopupType = Popups.PopupType.NONE;
		this.highlightedButtonStyle = new GUIStyle(this.regularUISkin.button);
		this.highlightedButtonStyle.normal.background = this.highlightedButtonStyle.hover.background;
		Color labelColor;
		labelColor..ctor(1f, 0.95f, 0.85f);
		Color labelColor2;
		labelColor2..ctor(0.2f, 0.2f, 0.2f);
		this.purchaseAddressFields.Add(new ValidatedTextfield("paFirstName", 1, 256, false, "First name", labelColor));
		this.purchaseAddressFields.Add(new ValidatedTextfield("paLastName", 1, 256, false, "Last name", labelColor));
		this.purchaseAddressFields.Add(new ValidatedTextfield("paAddress1", 1, 256, false, "Address 1", labelColor));
		this.purchaseAddressFields.Add(new ValidatedTextfield("paAddress2", 0, 256, false, "Address 2", labelColor));
		this.purchaseAddressFields.Add(new ValidatedTextfield("paZipCode", 1, 9, false, "ZIP code", labelColor));
		this.purchaseAddressFields.Add(new ValidatedTextfield("paCity", 1, 256, false, "City", labelColor));
		this.purchaseAddressFields.Add(new ValidatedTextfield("paState", 1, 256, false, "State", labelColor));
		this.purchaseCreditCardFields.Add(new ValidatedTextfield("piCardNo", 16, 16, true, "Card no.", labelColor2));
		this.purchaseCreditCardFields.Add(new ValidatedTextfield("piCvc2", 3, 4, true, "CVC2", labelColor2));
		this.cardOverlay = new GameObject("Card Overlay").AddComponent<CardOverlay>();
		Object.DontDestroyOnLoad(this.cardOverlay.gameObject);
	}

	// Token: 0x06001433 RID: 5171 RVA: 0x00079DB0 File Offset: 0x00077FB0
	private void OnGUI()
	{
		if (this.currentPopupType == Popups.PopupType.NONE && !this.showReconnectPopup)
		{
			return;
		}
		GUI.depth = 4;
		App.GUI.clearTooltip();
		GUI.skin = this.regularUISkin;
		int fontSize = GUI.skin.button.fontSize;
		GUI.skin.button.fontSize = 10 + Screen.height / 72;
		float num = (float)Screen.height * 0.03f;
		this.customExtraSpace = 0f;
		Rect rect;
		if (this.currentPopupType == Popups.PopupType.DECK_SELECTOR)
		{
			this.customExtraSpace = (float)Screen.height * 0.05f;
			rect..ctor((float)Screen.width * 0.5f - (float)Screen.height * 0.45f, (float)Screen.height * 0.255f - this.customExtraSpace / 2f, (float)Screen.height * 0.9f, (float)Screen.height * 0.49f + this.customExtraSpace);
		}
		else if (this.currentPopupType == Popups.PopupType.SHARD_PURCHASE_ONE || this.currentPopupType == Popups.PopupType.PURCHASE_PAYMENT_DETAILS)
		{
			rect..ctor((float)Screen.width * 0.5f - (float)Screen.height * 0.47f, (float)Screen.height * 0.2f, (float)Screen.height * 0.94f, (float)Screen.height * 0.6f);
		}
		else if (this.currentPopupType == Popups.PopupType.TOWER_CHALLENGE_SELECTOR)
		{
			rect..ctor((float)Screen.width * 0.5f - (float)Screen.height * 0.5f, (float)Screen.height * 0.1f, (float)Screen.height * 1f, (float)Screen.height * 0.8f);
		}
		else if (this.currentPopupType == Popups.PopupType.TUTORIAL_CHALLENGE_SELECTOR)
		{
			rect = GeomUtil.getCentered(new Rect(0f, 0f, (float)Screen.height * 0.9f, (float)Screen.height * 0.7f), true, true);
		}
		else if (this.currentPopupType == Popups.PopupType.CUSTOM_GAME_MULTIPLAYER_SELECTOR || this.currentPopupType == Popups.PopupType.CUSTOM_GAME_SINGLEPLAYER_SELECTOR)
		{
			rect = GeomUtil.getCentered(new Rect(0f, 0f, (float)Screen.height * 1.2f, (float)Screen.height * 1f), true, true);
		}
		else if (this.currentPopupType == Popups.PopupType.SCROLL_TEXT)
		{
			rect..ctor((float)Screen.width * 0.5f - (float)Screen.height * 0.5f, (float)Screen.height * 0.1f, (float)Screen.height * 1f, (float)Screen.height * 0.8f);
		}
		else if (this.currentPopupType == Popups.PopupType.TEXT_IMAGE)
		{
			rect..ctor((float)Screen.width * 0.5f - (float)Screen.height * 0.47f, (float)Screen.height * 0.18f, (float)Screen.height * 0.94f, (float)Screen.height * 0.64f);
		}
		else if (this.currentPopupType == Popups.PopupType.TEXT_AREA || this.currentPopupType == Popups.PopupType.TEXT_AREA_READONLY)
		{
			rect..ctor((float)Screen.width * 0.5f - (float)Screen.height * 0.35f, (float)Screen.height * 0.3f, (float)Screen.height * 0.7f, (float)Screen.height * 0.5f);
		}
		else if (this.currentPopupType == Popups.PopupType.SELL_CARD)
		{
			rect..ctor((float)Screen.width * 0.5f - (float)Screen.height * 0.37f, (float)Screen.height * 0.2f, (float)Screen.height * 0.74f, (float)Screen.height * 0.6f);
		}
		else
		{
			rect..ctor((float)Screen.width * 0.5f - (float)Screen.height * 0.35f, (float)Screen.height * 0.3f, (float)Screen.height * 0.7f, (float)Screen.height * 0.4f);
		}
		if (this.isLarge)
		{
			rect = GeomUtil.scaleCentered(rect, 1.1f, 1.4f);
		}
		if (this.popupRect != null)
		{
			rect = GeomUtil.inflate(this.popupRect.Value, (float)Screen.height * 0.05f, (float)Screen.height * 0.15f);
		}
		Rect rect2;
		rect2..ctor(rect.x + num, rect.y + num, rect.width - 2f * num, rect.height - 2f * num);
		float num2 = (float)Screen.height * 0.03f;
		Rect r;
		r..ctor(rect2.xMax - num2, rect2.y, num2, num2);
		new ScrollsFrame(rect).AddNinePatch(ScrollsFrame.Border.LIGHT_CURVED, NinePatch.Patches.CENTER).Draw();
		if (this.showReconnectPopup)
		{
			float num3 = (float)Screen.height * 0.055f;
			float num4 = rect2.height * 0.8f;
			int fontSize2 = GUI.skin.label.fontSize;
			GUI.skin.label.fontSize = 14 + Screen.height / 32;
			GUI.Label(new Rect(rect2.x, rect2.y, rect2.width, num3), "Connection lost");
			GUI.skin.label.fontSize = fontSize2;
			GUI.skin.label.fontSize = 12 + Screen.height / 60;
			GUI.Label(new Rect(rect2.x, rect2.y + num3, rect2.width, num4), "Reconnecting...");
			if (ConnectionRegistry.secondsDisconnected() >= 6f && SceneLoader.isScene(new string[]
			{
				"_LoginView"
			}))
			{
				GUI.Label(new Rect(rect2.x, rect2.y + num3 * 3f, rect2.width, num4), "Servers might be down for maintenance.");
			}
			GUI.skin.label.fontSize = fontSize2;
		}
		else
		{
			float num5 = (float)Screen.height * 0.055f;
			float num6 = rect2.height * ((this.currentPopupType != Popups.PopupType.SAVE_DECK && this.currentPopupType != Popups.PopupType.TEXT_ENTRY && this.currentPopupType != Popups.PopupType.PURCHASE_PASSWORD_ENTRY) ? ((this.currentPopupType != Popups.PopupType.INFO_PROGCLOSE) ? 0.6f : 0.8f) : 0.35f);
			int fontSize3 = GUI.skin.label.fontSize;
			GUI.skin.label.fontSize = 14 + Screen.height / 32;
			TextAnchor alignment = GUI.skin.label.alignment;
			GUI.skin.label.alignment = 1;
			GUI.Label(new Rect(rect2.x, rect2.y, rect2.width, num5), this.header);
			GUI.skin.label.fontSize = fontSize3;
			GUI.skin.label.alignment = alignment;
			if (this.currentPopupType == Popups.PopupType.GOLD_SHARDS_SELECT || this.currentPopupType == Popups.PopupType.OK_CANCEL || this.currentPopupType == Popups.PopupType.OK_CHOICE_CANCEL || this.currentPopupType == Popups.PopupType.OK || this.currentPopupType == Popups.PopupType.INFO_PROGCLOSE || this.currentPopupType == Popups.PopupType.SAVE_DECK || this.currentPopupType == Popups.PopupType.TEXT_ENTRY || this.currentPopupType == Popups.PopupType.PURCHASE_PASSWORD_ENTRY || this.currentPopupType == Popups.PopupType.TEXT_AREA || this.currentPopupType == Popups.PopupType.TEXT_AREA_READONLY)
			{
				bool wordWrap = GUI.skin.label.wordWrap;
				float num7 = rect2.y + num5;
				TextAnchor alignment2 = GUI.skin.label.alignment;
				if (this.isLarge)
				{
					num6 = rect2.height - num5 - (float)Screen.height * 0.05f;
					if (this.largeLeftAlign)
					{
						GUI.skin.label.alignment = 3;
					}
				}
				if (this.currentPopupType == Popups.PopupType.TEXT_AREA || this.currentPopupType == Popups.PopupType.TEXT_AREA_READONLY)
				{
					num7 += (float)Screen.height * 0.025f;
					GUI.skin.label.alignment = 1;
				}
				GUI.skin.label.wordWrap = true;
				GUI.skin.label.fontSize = ((!this.isLarge) ? (12 + Screen.height / 60) : (Screen.height / 32));
				Rect rect3;
				rect3..ctor(rect2.x, num7, rect2.width, num6);
				GUI.Label(rect3, this.description);
				GUI.skin.label.fontSize = fontSize3;
				GUI.skin.label.alignment = alignment2;
				GUI.skin.label.wordWrap = wordWrap;
			}
			else if (this.currentPopupType == Popups.PopupType.SCROLL_TEXT)
			{
				bool wordWrap2 = GUI.skin.label.wordWrap;
				GUI.skin.label.wordWrap = true;
				TextAnchor alignment3 = GUI.skin.label.alignment;
				GUI.skin.label.alignment = 0;
				Rect rect4;
				rect4..ctor(rect2.x, rect2.y + num5, rect2.width, rect2.height * 0.82f);
				GUI.BeginGroup(rect4);
				this.scrollTextScroll = GUILayout.BeginScrollView(this.scrollTextScroll, new GUILayoutOption[]
				{
					GUILayout.Width(rect4.width),
					GUILayout.Height(rect4.height)
				});
				string[] array = this.description.Split(new string[]
				{
					"\r\n\r\n"
				}, 1);
				foreach (string text in array)
				{
					GUILayout.Label(text, new GUILayoutOption[0]);
				}
				GUILayout.EndScrollView();
				GUI.EndGroup();
				GUI.skin.label.alignment = alignment3;
				GUI.skin.label.wordWrap = wordWrap2;
			}
			else if (this.currentPopupType == Popups.PopupType.TEXT_IMAGE)
			{
				Rect rect5;
				rect5..ctor(rect2.x, rect2.y + num5, rect2.width, rect2.height * 0.78f);
				GUI.BeginGroup(rect5);
				this.scrollTextScroll = GUILayout.BeginScrollView(this.scrollTextScroll, new GUILayoutOption[]
				{
					GUILayout.Width(rect5.width),
					GUILayout.Height(rect5.height)
				});
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUIStyle guistyle = new GUIStyle(GUI.skin.label);
				guistyle.alignment = 0;
				guistyle.fontSize = 10 + Screen.height / 80;
				GUILayout.Space(rect2.width * 0.03f);
				GUILayout.Label(this.description, guistyle, new GUILayoutOption[0]);
				float num8 = rect2.width * 0.3f;
				float num9 = num8 * (float)this.image.height / (float)this.image.width;
				GUILayout.Space(rect2.width * 0.02f);
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.Space(rect2.width * 0.01f);
				GUILayout.Label(this.image, new GUILayoutOption[]
				{
					GUILayout.Width(num8),
					GUILayout.Height(num9)
				});
				GUILayout.EndVertical();
				GUILayout.Space(rect2.width * 0.03f);
				GUILayout.EndHorizontal();
				GUILayout.EndScrollView();
				GUI.EndGroup();
			}
			if (this.currentPopupType == Popups.PopupType.OK || this.currentPopupType == Popups.PopupType.SCROLL_TEXT || this.currentPopupType == Popups.PopupType.TEXT_IMAGE)
			{
				Rect rect6;
				rect6..ctor((float)Screen.width * 0.5f - (float)Screen.height * 0.1f, rect2.yMax - (float)Screen.height * 0.05f, (float)Screen.height * 0.2f, (float)Screen.height * 0.05f);
				if (GUI.Button(rect6, this.okText, this.regularUISkin.button))
				{
					this.okCallback.PopupOk(this.popupType);
					this.HidePopup();
				}
			}
			else if (this.currentPopupType == Popups.PopupType.OK_CANCEL || this.currentPopupType == Popups.PopupType.OK_CHOICE_CANCEL || this.currentPopupType == Popups.PopupType.SAVE_DECK || this.currentPopupType == Popups.PopupType.TEXT_ENTRY || this.currentPopupType == Popups.PopupType.TEXT_AREA || this.currentPopupType == Popups.PopupType.TEXT_AREA_READONLY || this.currentPopupType == Popups.PopupType.PURCHASE_PASSWORD_ENTRY || this.currentPopupType == Popups.PopupType.SELL_CARD)
			{
				bool flag = false;
				Rect r2 = this.getOkButtonRect(rect2);
				if (this.currentPopupType == Popups.PopupType.SELL_CARD)
				{
					r2 = this._GetSingleButtonRectModified(rect2, 0.1f, -0.01f, 0.8f, 1f);
				}
				if (this.GUIButton(r2, this.okText))
				{
					flag = true;
				}
				if ((Input.GetKeyDown(271) || Input.GetKeyDown(13)) && (this.currentPopupType == Popups.PopupType.TEXT_ENTRY || this.currentPopupType == Popups.PopupType.SAVE_DECK || this.currentPopupType == Popups.PopupType.PURCHASE_PASSWORD_ENTRY))
				{
					flag = true;
				}
				if (flag)
				{
					int num10 = this.popupId;
					Popups.PopupType popupType = this.currentPopupType;
					if (popupType == Popups.PopupType.TEXT_ENTRY || popupType == Popups.PopupType.TEXT_AREA || popupType == Popups.PopupType.TEXT_AREA_READONLY || popupType == Popups.PopupType.SELL_CARD)
					{
						this.okStringCallback.PopupOk(this.popupType, this.textEntry);
					}
					else if (popupType == Popups.PopupType.SAVE_DECK)
					{
						if (this.CheckDeckName())
						{
							if (this.useAsAIDeck)
							{
								this.deckSaveCallback.PopupSaveAIDeck(this.popupType, this.saveDeckName);
							}
							else
							{
								this.okStringCallback.PopupOk(this.popupType, this.saveDeckName);
							}
						}
					}
					else if (popupType == Popups.PopupType.OK_CHOICE_CANCEL)
					{
						this.okChoiceCallback.PopupOk(this.popupType, 0);
					}
					else
					{
						this.okCallback.PopupOk(this.popupType);
					}
					if (this.closeOnOK && num10 == this.popupId)
					{
						this.HidePopup();
					}
				}
				Rect rect7;
				rect7..ctor((float)Screen.width * 0.5f + (float)Screen.height * 0.01f, rect2.yMax - (float)Screen.height * 0.05f, (float)Screen.height * 0.2f, (float)Screen.height * 0.05f);
				if (this.currentPopupType == Popups.PopupType.SAVE_DECK)
				{
					Rect rect8 = rect7;
					rect8.width = rect8.height;
					rect8.x += rect7.x - r2.x + rect8.width / 4f;
					if (GUI.Button(rect8, string.Empty, this.exportButtonStyle))
					{
						this.deckSaveCallback.PopupExport(this.popupType, this.saveDeckName);
					}
				}
				if (this.currentPopupType == Popups.PopupType.SELL_CARD)
				{
					rect7 = this._GetSingleButtonRectModified(rect2, 0.27f, -0.01f, 0.8f, 1f);
				}
				if (this._HasCancelButton() && this.GUIButton(rect7, this.cancelText))
				{
					Popups.PopupType popupType2 = this.currentPopupType;
					this.HidePopup();
					if (popupType2 == Popups.PopupType.OK_CHOICE_CANCEL)
					{
						this.okChoiceCallback.PopupOk(this.popupType, 1);
					}
					else
					{
						this.cancelCallback.PopupCancel(this.popupType);
					}
				}
			}
			else if (this.currentPopupType == Popups.PopupType.MULTIBUTTON)
			{
				float num11 = (float)Screen.height * 0.05f;
				float num12 = num11 + (float)Screen.height * 0.01f;
				float num13 = num11 * 0.5f;
				int j = 0;
				int num14 = 0;
				while (j < this.buttonList.Length)
				{
					if (this.buttonList[j] == null)
					{
						num13 += num11 * 0.5f;
						num14--;
					}
					else
					{
						Rect r3;
						r3..ctor((float)Screen.width * 0.5f - (float)Screen.height * 0.1f, num13 + rect2.y + rect2.height * 0.55f - ((float)this.buttonList.Length / 2f - (float)num14) * num12 + (num12 - num11), (float)Screen.height * 0.2f, num11);
						if (this.GUIButton(r3, this.buttonList[j]))
						{
							this.HidePopup();
							this.okStringCallback.PopupOk(this.popupType, this.buttonList[j].text);
						}
					}
					j++;
					num14++;
				}
			}
			else if (this.currentPopupType == Popups.PopupType.JOIN_ROOM)
			{
			}
			if (this.ShouldShowCloseButton())
			{
				GUI.skin = this.closeButtonSkin;
				if (this.GUIButton(r, string.Empty))
				{
					this.HidePopup();
					if (this.cancelCallback != null)
					{
						this.cancelCallback.PopupCancel(this.popupType);
					}
				}
				GUI.skin = this.regularUISkin;
			}
			if (this.currentPopupType == Popups.PopupType.JOIN_ROOM)
			{
				this.DrawJoinRoom(rect2);
			}
			else if (this.currentPopupType == Popups.PopupType.SAVE_DECK || this.currentPopupType == Popups.PopupType.PURCHASE_PASSWORD_ENTRY || this.currentPopupType == Popups.PopupType.TEXT_ENTRY || this.currentPopupType == Popups.PopupType.TEXT_AREA || this.currentPopupType == Popups.PopupType.TEXT_AREA_READONLY || this.currentPopupType == Popups.PopupType.SELL_CARD)
			{
				int fontSize4 = GUI.skin.textField.fontSize;
				GUI.skin.textField.fontSize = 10 + Screen.height / 72;
				Rect rect9;
				rect9..ctor(rect2.x + rect2.width * 0.2f, rect2.y + rect2.height * 0.5f, rect2.width * 0.6f, rect2.height * 0.14f);
				if (this.currentPopupType == Popups.PopupType.SAVE_DECK)
				{
					rect9.y -= rect2.height * 0.1f;
				}
				GUI.SetNextControlName("inputTextField");
				if (this.currentPopupType == Popups.PopupType.SAVE_DECK)
				{
					this.saveDeckName = GUI.TextField(rect9, this.saveDeckName);
					if (!string.IsNullOrEmpty(this.saveDeckProblems))
					{
						Color color = GUI.color;
						GUIStyle guistyle2 = new GUIStyle(GUI.skin.label);
						guistyle2.alignment = 1;
						guistyle2.wordWrap = true;
						float num15 = (!this.saveDeckProblems.Contains("\n")) ? 0.064f : 0.048f;
						Rect rect10 = GeomUtil.scaleCentered(GeomUtil.getTranslated(rect9, 0f, (float)Screen.height * num15), 1.8f, 1f);
						rect10.height *= 1.3f;
						GUI.color = ColorUtil.FromHex24(10526880u);
						GUI.Label(rect10, this.saveDeckProblems, guistyle2);
						GUI.color = color;
					}
					Rect rect11;
					rect11..ctor(rect2.x + rect2.width * 0.32f, rect2.y + rect2.height * 0.7f, rect2.width * 0.5f, rect2.height * 0.08f);
					Rect rect12;
					rect12..ctor(rect2.x + rect2.width * 0.24f, rect2.y + rect2.height * 0.7f, rect2.height * 0.08f, rect2.height * 0.08f);
				}
				else if (this.currentPopupType == Popups.PopupType.TEXT_ENTRY)
				{
					this.textEntry = GUI.TextField(rect9, this.textEntry);
				}
				else if (this.currentPopupType == Popups.PopupType.TEXT_AREA || this.currentPopupType == Popups.PopupType.TEXT_AREA_READONLY)
				{
					float num16 = (!string.IsNullOrEmpty(this.description)) ? 0f : (rect2.height * 0.15f);
					rect9..ctor(rect2.x + rect2.width * 0.1f, rect2.y + rect2.height * 0.35f - num16, rect2.width * 0.8f, num16 + rect2.height * 0.5f);
					Rect rect13 = rect9;
					GUILayout.BeginArea(rect13);
					this.scrollTextScroll = GUILayout.BeginScrollView(this.scrollTextScroll, new GUILayoutOption[]
					{
						GUILayout.Width(rect13.width),
						GUILayout.Height(rect13.height)
					});
					if (this.currentPopupType == Popups.PopupType.TEXT_AREA_READONLY)
					{
						GUILayout.TextArea(this.textEntry, new GUILayoutOption[]
						{
							GUILayout.MinHeight(rect13.height * 0.9f),
							GUILayout.ExpandHeight(true)
						});
					}
					else
					{
						this.textEntry = GUILayout.TextArea(this.textEntry, new GUILayoutOption[]
						{
							GUILayout.MinHeight(rect13.height * 0.9f),
							GUILayout.ExpandHeight(true)
						});
					}
					GUILayout.EndScrollView();
					GUILayout.EndArea();
				}
				else if (this.currentPopupType == Popups.PopupType.SELL_CARD)
				{
					TextAnchor alignment4 = GUI.skin.label.alignment;
					int fontSize5 = GUI.skin.label.fontSize;
					GUI.skin.label.fontSize = Screen.height / 34;
					GUI.skin.label.alignment = 3;
					GUI.Label(new Rect(rect2.x + rect2.width * 0.54f, rect2.y + rect2.height * 0.693f, rect2.width * 0.21f, (float)Screen.height * 0.04f), "<color=#eeddcc>Your price:</color>");
					GUI.skin.label.alignment = 4;
					GUI.skin.label.fontSize = Screen.height / 20;
					GUI.Label(new Rect(rect2.x + rect2.width * 0.46f, rect2.y + rect2.height * 0.03f, rect2.width * 0.52f, (float)Screen.height * 0.1f), "Sell " + this.cardOverlay.GetCardView().getCardType().name + "?");
					GUI.skin.label.fontSize = Screen.height / 40;
					GUI.Label(new Rect(rect2.x + rect2.width * 0.54f, rect2.y + rect2.height * 0.56f, rect2.width * 0.38f, (float)Screen.height * 0.04f), "<color=#bbaa88>Available for sale (this tier): " + this.sellCardAmountForSale + "</color>");
					GUI.Label(new Rect(rect2.x + rect2.width * 0.54f, rect2.y + rect2.height * 0.61f, rect2.width * 0.38f, (float)Screen.height * 0.04f), "<color=#bbaa88>Lowest current price: " + ((this.sellCardAmountForSale <= 0) ? "N/A" : (this.sellCardLowestPrice + " gold")) + "</color>");
					GUI.skin.label.fontSize = Screen.height / 48;
					int totalAmount;
					int.TryParse(this.textEntry, ref totalAmount);
					string text2 = string.Concat(new object[]
					{
						"<color=#aa9977>The fence takes a ",
						this.sellCardTax,
						"% cut.\nYour share: ",
						MarketplaceCreateOfferInfoMessage.amountToKeep(totalAmount, 0.01f * this.sellCardTax),
						" gold.</color>"
					});
					GUI.Label(new Rect(rect2.x + rect2.width * 0.54f, rect2.y + rect2.height * 0.775f, rect2.width * 0.38f, (float)Screen.height * 0.045f), text2);
					GUI.skin.label.fontSize = fontSize5;
					GUI.skin.label.alignment = alignment4;
					rect9..ctor(rect2.x + rect2.width * 0.711f, rect2.y + rect2.height * 0.7f, rect2.width * 0.21f, (float)Screen.height * 0.04f);
					this.textEntry = GUI.TextField(rect9, this.textEntry);
					Regex regex = new Regex("[^\\d]");
					this.textEntry = regex.Replace(this.textEntry, string.Empty);
				}
				else
				{
					this.purchasePassword = GUI.PasswordField(rect9, this.purchasePassword, '*');
				}
				GUI.skin.textField.fontSize = fontSize4;
				if (this.focusInput)
				{
					GUI.FocusControl("inputTextField");
					this.focusInput = false;
				}
			}
			else if (this.currentPopupType == Popups.PopupType.DECK_SELECTOR)
			{
				this.DrawDeckSelector(rect2);
			}
			else if (this.currentPopupType == Popups.PopupType.SHARD_PURCHASE_ONE)
			{
				this.DrawPurchasePopupOne(rect2);
			}
			else if (this.currentPopupType == Popups.PopupType.TOWER_CHALLENGE_SELECTOR)
			{
				this.DrawTowerChallengeSelector(rect2, App.TowerChallengeInfo);
			}
			else if (this.currentPopupType == Popups.PopupType.TUTORIAL_CHALLENGE_SELECTOR)
			{
				this.DrawTowerChallengeSelector(rect2, App.TutorialChallengeInfo);
			}
			else if (this.currentPopupType == Popups.PopupType.GOLD_SHARDS_SELECT)
			{
				this.DrawGoldShardsSelector(rect2);
			}
			if (this.popupRenderer != null)
			{
				this.popupRenderer.Invoke();
			}
			GUI.skin.button.fontSize = fontSize;
			App.GUI.drawTooltip();
		}
	}

	// Token: 0x06001434 RID: 5172 RVA: 0x0000EF27 File Offset: 0x0000D127
	private bool _HasCancelButton()
	{
		return this.cancelText != null;
	}

	// Token: 0x06001435 RID: 5173 RVA: 0x0007B720 File Offset: 0x00079920
	private Rect _GetSingleButtonRect(Rect popupInner)
	{
		return new Rect((float)Screen.width * 0.5f - (float)Screen.height * 0.1f, popupInner.yMax - (float)Screen.height * 0.05f, (float)Screen.height * 0.2f, (float)Screen.height * 0.05f);
	}

	// Token: 0x06001436 RID: 5174 RVA: 0x0007B778 File Offset: 0x00079978
	private Rect getOkButtonRect(Rect popupInner)
	{
		return (!this._HasCancelButton()) ? this._GetSingleButtonRect(popupInner) : new Rect((float)Screen.width * 0.5f - (float)Screen.height * 0.21f, popupInner.yMax - (float)Screen.height * 0.05f, (float)Screen.height * 0.2f, (float)Screen.height * 0.05f);
	}

	// Token: 0x06001437 RID: 5175 RVA: 0x0007B7E8 File Offset: 0x000799E8
	private Rect _GetSingleButtonRectModified(Rect popupInner, float xOffset, float yOffset, float widthMultiplier, float heightMultiplier)
	{
		Rect result = this._GetSingleButtonRect(popupInner);
		result.x += xOffset * (float)Screen.height;
		result.y += yOffset * (float)Screen.height;
		result.width *= widthMultiplier;
		result.height *= heightMultiplier;
		return result;
	}

	// Token: 0x06001438 RID: 5176 RVA: 0x0007B84C File Offset: 0x00079A4C
	private void DrawJoinRoom(Rect popupInner)
	{
		Rect rect;
		rect..ctor(popupInner.x, popupInner.y + popupInner.height * 0.18f, popupInner.width, popupInner.height * 0.56f);
		float num = (float)Screen.height * 0.015f;
		float num2 = (float)Screen.height * 0.07f;
		float num3 = num2 + num;
		Rect rect2;
		rect2..ctor(rect.x + 2f + num, rect.y + 2f + num, rect.width - 4f - 2f * num, rect.height - 4f - 2f * num);
		float num4 = rect2.width - 20f;
		int num5 = (this.roomList.Count % 2 != 0) ? (this.roomList.Count / 2 + 1) : (this.roomList.Count / 2);
		int fontSize = GUI.skin.label.fontSize;
		int fontSize2 = GUI.skin.button.fontSize;
		bool wordWrap = GUI.skin.label.wordWrap;
		GUI.skin.label.wordWrap = false;
		GUI.color = new Color(1f, 1f, 1f, 0.5f);
		GUI.Box(rect, string.Empty);
		GUI.Box(new Rect(rect2.xMax - 15f, rect2.y, 15f, rect2.height), string.Empty);
		GUI.color = Color.white;
		this.roomScroll = GUI.BeginScrollView(rect2, this.roomScroll, new Rect(0f, 0f, num4, (float)(num5 - 1) * num3 + num2));
		for (int i = 0; i < num5; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				if (2 * i + j < this.roomList.Count)
				{
					ChatRooms.JoinableRoomDesc joinableRoomDesc = this.roomList[2 * i + j];
					Rect r;
					r..ctor((float)j * num4 / 2f, (float)i * num3, num4 / 2f - num, num2);
					GUI.skin.button.fontSize = 10 + Screen.height / 60;
					if (this.GUIButton(r, joinableRoomDesc.name + ((!joinableRoomDesc.autoIncrement) ? (" (" + joinableRoomDesc.numUsers + ")") : string.Empty)))
					{
						this.HidePopup();
						this.joinRoomCallback.PopupJoinRoom(this.popupType, joinableRoomDesc.name, joinableRoomDesc.autoIncrement);
					}
				}
			}
		}
		GUI.EndScrollView();
		GUI.skin.label.fontSize = 8 + Screen.height / 72;
		GUI.Label(new Rect(popupInner.x, popupInner.yMax - (float)Screen.height * 0.09f, popupInner.width, (float)Screen.height * 0.04f), "Create or join a custom room:");
		Rect rect3;
		rect3..ctor(popupInner.x, popupInner.yMax - (float)Screen.height * 0.05f, popupInner.width - (float)Screen.height * 0.215f, (float)Screen.height * 0.05f);
		GUI.SetNextControlName("roomName");
		this.roomName = GUI.TextField(rect3, this.roomName);
		if (this.firstRun)
		{
			this.firstRun = false;
			GUI.FocusControl("roomName");
		}
		Rect r2;
		r2..ctor(popupInner.xMax - (float)Screen.height * 0.2f, popupInner.yMax - (float)Screen.height * 0.05f, (float)Screen.height * 0.2f, (float)Screen.height * 0.05f);
		GUI.skin.button.fontSize = 10 + Screen.height / 60;
		if ((this.GUIButton(r2, this.okText) || Input.GetKeyDown(13) || Input.GetKeyDown(271)) && this.roomName.Length > 0)
		{
			this.HidePopup();
			this.joinRoomCallback.PopupJoinRoom(this.popupType, this.roomName, false);
		}
		GUI.skin.button.fontSize = fontSize2;
		GUI.skin.label.fontSize = fontSize;
		GUI.skin.label.wordWrap = wordWrap;
	}

	// Token: 0x06001439 RID: 5177 RVA: 0x0007BCE0 File Offset: 0x00079EE0
	private void DrawDeckSelector(Rect popupInnerOrg)
	{
		Rect rect = popupInnerOrg;
		rect.height -= this.customExtraSpace;
		Rect full = popupInnerOrg;
		full.yMin = rect.yMax;
		GUI.Label(GeomUtil.cropShare(full, new Rect(0f, 0.1f, 0.12f, 1f)), "Filter");
		Rect share;
		share..ctor(0.12f, 0.1f, (this.okText == null) ? 0.82f : ((!(this.okText.text != "_import")) ? 0.7f : 0.54f), 1f);
		this.deckFilter = GUI.TextField(GeomUtil.cropShare(full, share), this.deckFilter);
		string deckFilterLower = this.deckFilter.ToLower();
		List<DeckInfo> list = Enumerable.ToList<DeckInfo>(Enumerable.Where<DeckInfo>(this.deckList, (DeckInfo d) => d.name.ToLower().IndexOf(deckFilterLower) >= 0));
		if (this.okText != null)
		{
			Rect okButtonRect = this.getOkButtonRect(default(Rect));
			Rect centered = GeomUtil.getCentered(okButtonRect, GeomUtil.cropShare(full, new Rect(0.62f, 0.1f, 0.38f, 1f)), true, true);
			if (this.okText.text == "_import")
			{
				float xMax = centered.xMax;
				centered.width = centered.height;
				centered.x = xMax - centered.width / 2f;
				if (GUI.Button(centered, string.Empty, this.importButtonStyle))
				{
					this.HidePopup();
					this.deckChosenCallback.PopupOk(this.popupType);
				}
			}
			else if (GUI.Button(centered, this.okText))
			{
				this.HidePopup();
				this.deckChosenCallback.PopupOk(this.popupType);
			}
		}
		list.Sort(delegate(DeckInfo a, DeckInfo b)
		{
			if (a is LabEntryInfo && !(b is LabEntryInfo))
			{
				return 1;
			}
			if (!(a is LabEntryInfo) && b is LabEntryInfo)
			{
				return -1;
			}
			if (a.valid && !b.valid)
			{
				return -1;
			}
			if (b.valid && !a.valid)
			{
				return 1;
			}
			if (a.timestamp > b.timestamp)
			{
				return -1;
			}
			if (b.timestamp > a.timestamp)
			{
				return 1;
			}
			return a.name.CompareTo(b.name);
		});
		Rect rect2;
		rect2..ctor(rect.x, rect.y + rect.height * 0.15f, rect.width, rect.height * 0.85f);
		float num = (float)Screen.height * 0.015f;
		float num2 = (float)Screen.height * 0.07f;
		float num3 = num2 + num;
		Rect rect3;
		rect3..ctor(rect2.x + 2f + num, rect2.y + 2f + num, rect2.width - 4f - 2f * num, rect2.height - 4f - 2f * num);
		float num4 = rect3.width - 20f;
		int num5 = (list.Count % 2 != 0) ? (list.Count / 2 + 1) : (list.Count / 2);
		int fontSize = GUI.skin.label.fontSize;
		TextAnchor alignment = GUI.skin.label.alignment;
		bool wordWrap = GUI.skin.label.wordWrap;
		GUI.skin.label.wordWrap = false;
		GUI.skin.label.alignment = 0;
		GUI.color = new Color(1f, 1f, 1f, 0.5f);
		GUI.Box(rect2, string.Empty);
		GUI.Box(new Rect(rect3.xMax - 15f, rect3.y, 15f, rect3.height), string.Empty);
		GUI.color = Color.white;
		this.deckScroll = GUI.BeginScrollView(rect3, this.deckScroll, new Rect(0f, 0f, num4, (float)(num5 - 1) * num3 + num2));
		for (int i = 0; i < num5; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				if (2 * i + j < list.Count)
				{
					DeckInfo deckInfo = list[2 * i + j];
					bool flag = deckInfo.valid || this.allowInvalidClicks;
					GUI.enabled = flag;
					Rect r;
					r..ctor((float)j * num4 / 2f, (float)i * num3, num4 / 2f - num, num2);
					Rect rect4;
					rect4..ctor(r.xMax - (float)Screen.height * 0.025f, r.y + (float)Screen.height * 0.005f, (float)Screen.height * 0.02f, (float)Screen.height * 0.02f);
					if (this.showDeleteDeckIcon)
					{
						GUI.skin = this.emptySkin;
						if (this.GUIButton(rect4, string.Empty))
						{
							this.deckChosenCallback.PopupDeckDeleted(deckInfo);
						}
						GUI.skin = this.regularUISkin;
					}
					if (this.GUIButton(r, string.Empty))
					{
						this.HidePopup();
						this.deckChosenCallback.PopupDeckChosen(deckInfo);
					}
					string text = deckInfo.name;
					if (!deckInfo.valid)
					{
						text = "<color=#ee5533>[Illegal]</color> " + text;
					}
					if (deckInfo is LabEntryInfo)
					{
						LabEntryInfo labEntryInfo = (LabEntryInfo)deckInfo;
						text = "<color=#eebb33>[AI deck]</color> " + text;
						GUI.skin.label.fontSize = 8 + Screen.height / 80;
						TextAnchor alignment2 = GUI.skin.label.alignment;
						GUI.skin.label.alignment = 2;
						GUI.Label(new Rect(r.x + r.width * 0.25f, r.y + r.height * 0.45f, r.width * 0.66f, r.height * 0.6f), string.Concat(new object[]
						{
							"Wins: ",
							labEntryInfo.wins,
							", Losses: ",
							labEntryInfo.losses,
							"/",
							labEntryInfo.lossesRemoveAfter
						}));
						GUI.skin.label.alignment = alignment2;
					}
					GUI.skin.label.fontSize = 10 + Screen.height / 60;
					GUI.Label(new Rect(r.x + r.width * 0.25f, r.y + r.height * 0.05f, r.width * 0.66f, r.height * 0.6f), text);
					GUI.skin.label.fontSize = 8 + Screen.height / 80;
					GUI.Label(new Rect(r.x + r.width * 0.25f, r.y + r.height * 0.45f, r.width * 0.66f, r.height * 0.6f), deckInfo.updated);
					if (!flag)
					{
						GUI.color = new Color(1f, 1f, 1f, 0.5f);
					}
					GUI.DrawTexture(new Rect(r.x + r.height * 0.4f, r.y + r.height * 0.1f, r.height * 0.7f, r.height * 0.8f), ResourceManager.LoadTexture("Arena/deck_icon"));
					if (deckInfo.resources != string.Empty)
					{
						string[] array = deckInfo.resources.Split(new char[]
						{
							','
						});
						for (int k = 0; k < array.Length; k++)
						{
							float num6 = r.height * 0.38f;
							float num7 = r.x + ((k < 2) ? 0f : (num6 * 0.85f * 73f / 72f)) + 3f;
							float num8 = r.y + ((k < 2) ? 0f : (num6 / 3f)) + 3f;
							GUI.DrawTexture(new Rect(num7, num8 + num6 * 0.85f * (float)(k % 2), num6 * 73f / 72f, num6), ResourceManager.LoadTexture("BattleUI/battlegui_icon_" + array[k]));
						}
					}
					GUI.color = Color.white;
					if (this.showDeleteDeckIcon)
					{
						GUI.skin = this.closeButtonSkin;
						if (GUI.Button(rect4, string.Empty))
						{
						}
						GUI.skin = this.regularUISkin;
					}
					GUI.enabled = true;
				}
			}
		}
		GUI.EndScrollView();
		GUI.skin.label.fontSize = fontSize;
		GUI.skin.label.alignment = alignment;
		GUI.skin.label.wordWrap = wordWrap;
	}

	// Token: 0x0600143A RID: 5178 RVA: 0x0007C5E8 File Offset: 0x0007A7E8
	private void DrawPurchasePopupOne(Rect inner)
	{
		int fontSize = GUI.skin.label.fontSize;
		int fontSize2 = GUI.skin.button.fontSize;
		Rect rect;
		rect..ctor(inner.x, inner.y + inner.height * 0.03f, inner.width, inner.height * 0.97f);
		float num = rect.width * 0.2f;
		float num2 = rect.width - num * 2f;
		float num3 = (float)(this.shardInfos.Length - 1);
		float num4 = num2 / num3 * 0.9f;
		float num5 = rect.height * 0.78f;
		for (int i = 0; i < this.shardInfos.Length; i++)
		{
			Rect r;
			r..ctor(rect.x + num + num2 / num3 * (float)i - num4 / 2f, rect.y + (float)Screen.height * 0.07f, num4, num5);
			if (this.GUIButton(r, string.Empty))
			{
				if (!this.purchaseAddressKnown)
				{
					this.countryDrop.SetEnabled(true);
				}
				if (!this.purchaseCreditCardKnown)
				{
					this.monthDrop.SetEnabled(true);
					this.yearDrop.SetEnabled(true);
				}
				this.currentPopupType = Popups.PopupType.PURCHASE_PAYMENT_DETAILS;
				this.activePurchaseId = i;
			}
			this.DrawVoidImage(r, i, true);
		}
		GUI.skin.label.fontSize = fontSize;
		GUI.skin.button.fontSize = fontSize2;
	}

	// Token: 0x0600143B RID: 5179 RVA: 0x0007C774 File Offset: 0x0007A974
	private void DrawPurchaseItemImage(Rect r, Texture2D image, Price price)
	{
		float num = (float)Screen.height * 0.011f;
		Rect rect;
		rect..ctor(r.x + num, r.y + num, r.width - num * 2f, r.height - num * 2f);
		float num2 = rect.width / rect.height;
		Rect rect2;
		rect2..ctor(0.5f - 0.5f * num2, 0f, num2, 1f);
		GUI.DrawTextureWithTexCoords(rect, image, rect2);
		GUISkin skin = GUI.skin;
		GUI.skin = this.regularUISkin;
		GUI.skin.label.fontSize = 4 + Screen.height / 25;
		string text = price.amount + " " + price.currency.currencyCode;
		for (int i = -1; i <= 1; i += 2)
		{
			for (int j = -1; j <= 1; j += 2)
			{
				GUI.Label(new Rect(r.x + (float)(2 * i), r.yMax + (float)(2 * j) - (float)Screen.height * 0.07f, r.width, (float)Screen.height * 0.07f), "<color=black>" + text + "</color>");
			}
		}
		GUI.Label(new Rect(r.x, r.yMax - (float)Screen.height * 0.07f, r.width, (float)Screen.height * 0.07f), text);
		GUI.skin.label.fontSize = 20 + Screen.height / 60;
		GUI.skin = skin;
	}

	// Token: 0x0600143C RID: 5180 RVA: 0x0007C928 File Offset: 0x0007AB28
	private void DrawVoidImage(Rect r, int i, bool drawDetailed)
	{
		double num = (double)this.shardInfos[0].numberOfShards / this.shardInfos[0].price.amount;
		Texture2D texture2D = ResourceManager.LoadTexture("Store/void_shard_" + (i + 1));
		this.DrawPurchaseItemImage(r, texture2D, this.shardInfos[i].price);
		int num2 = this.shardInfos[i].numberOfShards - (int)(this.shardInfos[i].price.amount * num);
		string text = this.shardInfos[i].numberOfShards + ((!drawDetailed) ? "\n" : " shards\n");
		string text2 = (num2 <= 0) ? string.Empty : ("Includes " + num2 + " bonus shards!");
		GUISkin skin = GUI.skin;
		GUI.skin = this.regularUISkin;
		GUI.skin.label.fontSize = 4 + Screen.height / 25;
		for (int j = -1; j <= 1; j += 2)
		{
			for (int k = -1; k <= 1; k += 2)
			{
				GUI.Label(new Rect(r.x + (float)(2 * j), r.y + (float)(2 * k) + (float)Screen.height * 0.02f, r.width, (float)Screen.height * 0.08f), "<color=black>" + text + "</color>");
			}
		}
		GUI.Label(new Rect(r.x, r.y + (float)Screen.height * 0.02f, r.width, (float)Screen.height * 0.08f), text);
		GUI.skin.label.fontSize = 4 + Screen.height / 60;
		if (drawDetailed)
		{
			for (int l = -1; l <= 1; l += 2)
			{
				for (int m = -1; m <= 1; m += 2)
				{
					GUI.Label(new Rect(r.x + (float)l, r.y + (float)m + (float)Screen.height * 0.055f, r.width, (float)Screen.height * 0.03f), "<color=black>" + text2 + "</color>");
				}
			}
			GUI.Label(new Rect(r.x, r.y + (float)Screen.height * 0.055f, r.width, (float)Screen.height * 0.03f), text2);
			GUI.skin.label.fontSize = 4 + Screen.height / 32;
		}
		GUI.skin.label.fontSize = 20 + Screen.height / 60;
		GUI.skin = skin;
	}

	// Token: 0x0600143D RID: 5181 RVA: 0x0007CBF4 File Offset: 0x0007ADF4
	private void DrawTowerChallengeSelector(Rect popupInner, TowerChallengeInfo towerLevels)
	{
		TowerLevel[] levels = towerLevels.levels;
		if (this.selectedChallengeID < 0 || this.selectedChallengeID >= levels.Length)
		{
			this.selectedChallengeID = ((levels.Length <= 0) ? -1 : 0);
		}
		TextAnchor alignment = GUI.skin.label.alignment;
		TowerLevel towerLevel = new TowerLevel();
		Rect rect;
		rect..ctor(popupInner.x, popupInner.y + popupInner.height * 0.15f, popupInner.width, popupInner.height * 0.85f);
		float num = (float)Screen.height * 0.015f;
		float num2 = (float)Screen.height * 0.07f;
		float num3 = num2 + num;
		Rect rect2;
		rect2..ctor(rect.x + 2f + num, rect.y + 2f + num, rect.width - 4f - 2f * num, rect.height - 4f - 2f * num);
		float num4 = rect.width * 0.4f - 30f;
		int num5 = Screen.height / 40;
		TextAnchor alignment2 = GUI.skin.label.alignment;
		bool wordWrap = GUI.skin.label.wordWrap;
		GUI.skin.label.wordWrap = false;
		GUI.skin.label.alignment = 1;
		GUI.color = new Color(1f, 1f, 1f, 0.4f);
		GUI.Box(new Rect(rect.x, rect.y, rect.width * 0.4f + 20f, rect.height), string.Empty);
		GUI.color = Color.white;
		if (levels == null)
		{
			GUI.skin.label.wordWrap = wordWrap;
			GUI.skin.label.alignment = alignment2;
			return;
		}
		float num6 = 55f;
		float num7 = 50f;
		float num8 = num6;
		float num9 = 0f;
		float num10 = (float)towerLevels.levelCount() * num6 + (float)towerLevels.separatorCount() * num8;
		this.deckScroll = App.GUI.BeginScrollView(new Rect(rect2.x, rect2.y, rect2.width * 0.4f + 5f, rect2.height), this.deckScroll, new Rect(0f, 0f, num4, num10));
		for (int i = 0; i < levels.Length; i++)
		{
			TowerLevel towerLevel2 = levels[i];
			bool flag = towerLevel2 == null;
			GUI.color = Color.white;
			Rect rect3;
			rect3..ctor(0f, 0f, rect.width * 0.4f - 30f, num7);
			if (flag)
			{
				Texture2D texture2D = ResourceManager.LoadTexture("ChatUI/white");
				GUI.color = ColorUtil.FromHex24(15388326u, 0.4f);
				GUI.DrawTexture(GeomUtil.scaleCentered(new Rect(0f, (float)i * num6 + num6 * 0.4f, rect3.width, 2f), 0.6f), texture2D);
				GUI.color = Color.white;
			}
			else
			{
				App.GUI.BeginGroup(new Rect(0f, num9 + (float)i * num6, rect2.width - 25f, num7));
				if (i == this.selectedChallengeID)
				{
					towerLevel = towerLevel2;
					GUI.Box(rect3, string.Empty, this.highlightedButtonStyle);
				}
				else if (GUI.Button(rect3, string.Empty))
				{
					this.selectedChallengeID = i;
					towerLevel = towerLevel2;
					this.trialScroll = Vector2.zero;
				}
				if (!towerLevel2.isCompleted && towerLevel2.isTutorialLocked(levels))
				{
					App.GUI.Box(rect3, GUITags.TutorialLocked, GUI.skin.label);
				}
				else if (towerLevel2.isDemoLocked())
				{
					App.GUI.Box(rect3, GUITags.DemoLockedNoTooltip, GUI.skin.label);
				}
				GUI.skin.label.alignment = 7;
				GUI.skin.label.fontSize = (int)Math.Round((double)((float)num5 * 1.3f));
				GUI.skin.label.normal.textColor = new Color(1f, 0.95f, 0.85f);
				GUI.Label(new Rect(0f, 0f, rect.width * 0.4f - 25f, 48f), towerLevel2.name);
				GUI.skin.label.normal.textColor = Color.white;
				if (towerLevel2.isCompleted)
				{
					Texture2D texture2D2 = ResourceManager.LoadTexture("Arena/scroll_browser_button_checkbox");
					GUI.DrawTexture(new Rect(0f, 0f, 50f, 50f), texture2D2);
				}
				Color textColor = GUI.skin.label.normal.textColor;
				if (towerLevel2.isEasy())
				{
					GUI.skin.label.normal.textColor = new Color(0.6f, 0.8f, 0.5f, 0.75f);
				}
				else if (towerLevel2.isMedium())
				{
					GUI.skin.label.normal.textColor = new Color(0.9f, 0.8f, 0.5f, 0.75f);
				}
				else
				{
					GUI.skin.label.normal.textColor = new Color(0.8f, 0.4f, 0.3f, 0.75f);
				}
				GUI.skin.label.alignment = 7;
				GUI.skin.label.fontSize = (int)Math.Round((double)((float)num5 * 1f));
				GUI.Label(new Rect(0f, 0f, rect.width * 0.4f - 25f, 25f), towerLevel2.getHeader());
				GUI.skin.label.normal.textColor = textColor;
				App.GUI.EndGroup();
			}
		}
		App.GUI.EndScrollView();
		string flavour = towerLevel.flavour;
		GUI.skin.label.alignment = 0;
		GUI.skin.label.fontSize = num5;
		GUI.skin.label.wordWrap = true;
		float num11 = GUI.skin.label.CalcHeight(new GUIContent(flavour), rect.width * 0.6f - 90f);
		string text = towerLevel.description;
		float num12 = GUI.skin.label.CalcHeight(new GUIContent(text), rect.width * 0.6f - 110f);
		Rect rect4;
		rect4..ctor(rect.x + rect.width * 0.4f + 5f + 25f, rect.y, rect.width * 0.6f - 40f, rect.height);
		GUI.BeginGroup(rect4);
		GUI.color = new Color(1f, 1f, 1f, 0.4f);
		GUI.Box(new Rect(0f, 0f, rect.width * 0.6f - 40f, rect.height - 90f), string.Empty);
		GUI.color = Color.white;
		GUI.EndGroup();
		this.trialScroll = GUI.BeginScrollView(new Rect(rect4.x, rect4.y + 5f, rect4.width - 5f, rect4.height - 100f), this.trialScroll, new Rect(0f, 0f, rect.width * 0.6f - 65f, num12 + num11 + 86f + (float)((!towerLevel.isCompleted) ? 120 : 50)));
		GUI.skin.label.normal.textColor = new Color(1f, 0.95f, 0.85f);
		GUI.skin.label.fontSize = (int)Math.Round((double)((float)num5 * 1.5f));
		GUI.Label(new Rect(20f, 10f, rect.width * 0.6f - 90f, rect.height - 20f), towerLevel.name);
		GUI.skin.label.wordWrap = true;
		GUI.skin.label.normal.textColor = new Color(0.8f, 0.73f, 0.65f);
		GUI.skin.label.alignment = 0;
		GUI.skin.label.fontSize = num5;
		GUI.Label(new Rect(20f, 40f, rect.width * 0.6f - 90f, rect.height - 20f), flavour);
		GUI.skin.label.wordWrap = false;
		GUI.skin.label.alignment = 1;
		GUI.color = new Color(1f, 1f, 1f, 0.4f);
		GUI.Box(new Rect(20f, 70f + num11, rect.width * 0.6f - 90f, num12 + (float)((!towerLevel.isCompleted) ? (120 + ((towerLevel.cardRewardCount <= 0) ? 0 : 30)) : 50)), string.Empty);
		GUI.color = Color.white;
		GUI.skin.label.normal.textColor = new Color(1f, 0.95f, 0.85f);
		GUI.skin.label.fontSize = (int)Math.Round((double)((float)num5 * 1.5f));
		string text2 = (levels.Length <= 0 || !levels[0].isTutorial()) ? "Trial Details" : "Tutorial Details";
		GUI.Label(new Rect(20f, 75f + num11, rect.width * 0.6f - 90f, rect.height - 20f), text2);
		GUI.skin.label.wordWrap = true;
		GUI.skin.label.normal.textColor = new Color(0.8f, 0.73f, 0.65f);
		GUI.skin.label.alignment = 0;
		GUI.skin.label.fontSize = num5;
		GUI.Label(new Rect(30f, 105f + num11, rect.width * 0.6f - 110f, rect.height - 20f), text);
		GUI.skin.label.wordWrap = false;
		GUI.skin.label.alignment = 1;
		GUI.skin.label.normal.textColor = Color.white;
		GUI.skin.label.normal.textColor = new Color(1f, 0.95f, 0.85f);
		GUI.skin.label.fontSize = (int)Math.Round((double)((float)num5 * 1.5f));
		if (!towerLevel.isCompleted)
		{
			float num13 = 110f + num11 + num12;
			GUI.Label(new Rect(20f, num13, rect.width * 0.6f - 90f, rect.height - 20f), "Reward");
			if (towerLevel.goldReward > 0)
			{
				num13 += 30f;
				Texture2D texture2D3 = ResourceManager.LoadTexture("Shared/gold_icon");
				GUI.DrawTexture(new Rect((rect.width * 0.6f - 50f) / 2f - 45f, num13 + 5f, 30f, 30f), texture2D3);
				GUI.skin.label.normal.textColor = Color.white;
				TextAnchor alignment3 = GUI.skin.label.alignment;
				GUI.skin.label.alignment = 0;
				GUI.Label(new Rect((rect.width * 0.6f - 35f) / 2f - 15f, num13, 150f, 50f), "x " + towerLevel.goldReward);
				GUI.skin.label.alignment = alignment3;
			}
			if (towerLevel.cardRewardCount > 0)
			{
				num13 += 30f;
				Texture2D texture2D4 = ResourceManager.LoadTexture("Shared/scroll_icon");
				GUI.DrawTexture(new Rect((rect.width * 0.6f - 50f) / 2f - 45f, num13 + 5f, 30f, 30f), texture2D4);
				GUI.skin.label.normal.textColor = Color.white;
				TextAnchor alignment4 = GUI.skin.label.alignment;
				GUI.skin.label.alignment = 0;
				GUI.Label(new Rect((rect.width * 0.6f - 35f) / 2f - 15f, num13, 150f, 50f), "x " + towerLevel.cardRewardCount);
				GUI.skin.label.alignment = alignment4;
			}
		}
		GUI.EndScrollView();
		if (levels.Length > 0)
		{
			App.GUI.BeginGroup(rect4);
			GUIContent guicontent = new GUIContent();
			string text3 = "Play Trial";
			if (levels[0].isTutorial())
			{
				text3 = "Play Tutorial";
				if (towerLevel == levels[0] && !levels[0].isCompleted)
				{
					guicontent.helpArrow();
				}
			}
			guicontent.text = text3;
			if (!towerLevel.isCompleted && towerLevel.isTutorialLocked(levels))
			{
				guicontent.lockTutorial();
			}
			else if (towerLevel.isDemoLocked())
			{
				guicontent.lockDemoNoTooltip();
			}
			if (App.GUI.Button(new Rect((rect.width * 0.6f - 40f) / 2f - rect.width * 0.15f, rect.height - 65f, rect.width * 0.3f, 50f), guicontent) && towerLevel.id >= 0)
			{
				this.HidePopup();
				this.towerChallengeChosenCallback.PopupTowerChallengeChosen(towerLevel.id, towerLevel.borrowDeck);
			}
			App.GUI.EndGroup();
		}
		GUI.skin.label.fontSize = num5;
		GUI.skin.label.normal.textColor = Color.white;
		GUI.skin.label.alignment = alignment;
		GUI.skin.label.wordWrap = wordWrap;
	}

	// Token: 0x0600143E RID: 5182 RVA: 0x0007DB18 File Offset: 0x0007BD18
	private void DrawGoldShardsSelector(Rect popupInner)
	{
		int fontSize = GUI.skin.label.fontSize;
		GUI.skin.label.fontSize = 10 + Screen.height / 60;
		float num = (float)Screen.width * 0.5f - ((!App.ServerSettings.shardsEnabled) ? (0.5f * (float)Screen.height * 0.18f) : ((float)Screen.height * 0.2f));
		Rect rect;
		rect..ctor(num, popupInner.yMax - (float)Screen.height * 0.14f, (float)Screen.height * 0.18f, (float)Screen.height * 0.04f);
		Rect rect2;
		rect2..ctor((float)Screen.width * 0.5f + (float)Screen.height * 0.02f, popupInner.yMax - (float)Screen.height * 0.14f, (float)Screen.height * 0.18f, (float)Screen.height * 0.04f);
		GUI.color = new Color(1f, 1f, 1f, 0.25f);
		GUI.Box(rect, string.Empty);
		if (App.ServerSettings.shardsEnabled)
		{
			GUI.Box(rect2, string.Empty);
		}
		GUI.color = Color.white;
		float num2 = -rect.height * 0.15f;
		GUI.DrawTexture(new Rect(rect.x + num2, rect.y + num2, rect.height - num2 * 2f, rect.height - num2 * 2f), ResourceManager.LoadTexture("Shared/gold_icon"));
		GUI.Label(rect, "<color=#ffcc66>" + this.goldShardGoldCost + "</color>");
		if (App.ServerSettings.shardsEnabled)
		{
			GUI.DrawTexture(new Rect(rect2.x + num2, rect2.y + num2, rect2.height - num2 * 2f, rect2.height - num2 * 2f), ResourceManager.LoadTexture("Shared/voidshard_icon"));
			GUI.Label(rect2, "<color=#dd99ff>" + this.goldShardShardsCost + "</color>");
		}
		GUI.skin.label.fontSize = fontSize;
		float num3 = (float)Screen.width * 0.5f - ((!App.ServerSettings.shardsEnabled) ? (0.5f * (float)Screen.height * 0.2f) : ((float)Screen.height * 0.21f));
		Rect r;
		r..ctor(num3, popupInner.yMax - (float)Screen.height * 0.07f, (float)Screen.height * 0.2f, (float)Screen.height * 0.05f);
		if (this.goldShardGoldCurrent >= this.goldShardGoldCost)
		{
			if (this.GUIButton(r, "Use gold"))
			{
				this.HidePopup();
				this.goldShardsCallback.PopupGoldSelected(this.goldShardsItemId);
			}
		}
		else
		{
			GUI.enabled = false;
			this.GUIButton(r, "Need gold");
			GUI.enabled = true;
		}
		if (App.ServerSettings.shardsEnabled)
		{
			Rect r2;
			r2..ctor((float)Screen.width * 0.5f + (float)Screen.height * 0.01f, popupInner.yMax - (float)Screen.height * 0.07f, (float)Screen.height * 0.2f, (float)Screen.height * 0.05f);
			if (this.goldShardShardsCurrent >= this.goldShardShardsCost)
			{
				if (this.GUIButton(r2, "Use shards"))
				{
					this.HidePopup();
					this.goldShardsCallback.PopupShardsSelected(this.goldShardsItemId);
				}
			}
			else
			{
				GUI.enabled = false;
				this.GUIButton(r2, "Need shards");
				GUI.enabled = true;
			}
		}
	}

	// Token: 0x0600143F RID: 5183 RVA: 0x0000EF35 File Offset: 0x0000D135
	private void GUIDropdown(Dropdown dropdown, Rect r, string label, float labelSize, Color labelColor)
	{
		dropdown.SetRect(r);
		LabelUtil.HelpLabel(r, label, labelSize, labelColor);
	}

	// Token: 0x06001440 RID: 5184 RVA: 0x0000EF49 File Offset: 0x0000D149
	private bool GUIButton(Rect r, string text)
	{
		return this.GUIButton(r, new GUIContent(text));
	}

	// Token: 0x06001441 RID: 5185 RVA: 0x0000EF58 File Offset: 0x0000D158
	private bool GUIButton(Rect r, GUIContent content)
	{
		return App.GUI.Button(r, content);
	}

	// Token: 0x06001442 RID: 5186 RVA: 0x0007DEE0 File Offset: 0x0007C0E0
	private void ShowPopup(Popups.PopupType type)
	{
		this.popupId = ++Popups.popupRunningId;
		this.popupRect = default(Rect?);
		this.popupRenderer = null;
		this.currentPopupType = type;
		this.overlay.enabled = true;
		this.firstRun = true;
		this.closeOnOK = true;
		this.isLarge = false;
		this.largeLeftAlign = true;
		this.forceShowCloseButton = false;
		this.stayOpen = false;
		this.SetErrorText(null);
	}

	// Token: 0x06001443 RID: 5187 RVA: 0x0007DF5C File Offset: 0x0007C15C
	private void HidePopup()
	{
		if (this.stayOpen)
		{
			this.stayOpen = false;
			return;
		}
		this.currentPopupType = Popups.PopupType.NONE;
		this.overlay.enabled = false;
		this.cardOverlay.Hide();
		this.monthDrop.SetEnabled(false);
		this.yearDrop.SetEnabled(false);
		this.countryDrop.SetEnabled(false);
	}

	// Token: 0x06001444 RID: 5188 RVA: 0x0000EF66 File Offset: 0x0000D166
	public void StayOpen()
	{
		this.stayOpen = true;
	}

	// Token: 0x06001445 RID: 5189 RVA: 0x00004AAC File Offset: 0x00002CAC
	private bool CheckDeckName()
	{
		return true;
	}

	// Token: 0x06001446 RID: 5190 RVA: 0x0007DFC0 File Offset: 0x0007C1C0
	private void ClearForms()
	{
		foreach (ValidatedTextfield validatedTextfield in this.purchaseCreditCardFields)
		{
			validatedTextfield.SetValue(string.Empty);
		}
		foreach (ValidatedTextfield validatedTextfield2 in this.purchaseAddressFields)
		{
			validatedTextfield2.SetValue(string.Empty);
		}
		this.countryDrop.SetSelectedItem(0);
		this.yearDrop.SetSelectedItem(0);
		this.monthDrop.SetSelectedItem(0);
	}

	// Token: 0x06001447 RID: 5191 RVA: 0x0007E090 File Offset: 0x0007C290
	private void FormToPurchaseAddress()
	{
		foreach (ValidatedTextfield validatedTextfield in this.purchaseAddressFields)
		{
			string value = validatedTextfield.GetValue();
			string name = validatedTextfield.GetName();
			if (name != null)
			{
				if (Popups.<>f__switch$map1D == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(7);
					dictionary.Add("paFirstName", 0);
					dictionary.Add("paLastName", 1);
					dictionary.Add("paAddress1", 2);
					dictionary.Add("paAddress2", 3);
					dictionary.Add("paZipCode", 4);
					dictionary.Add("paCity", 5);
					dictionary.Add("paState", 6);
					Popups.<>f__switch$map1D = dictionary;
				}
				int num;
				if (Popups.<>f__switch$map1D.TryGetValue(name, ref num))
				{
					switch (num)
					{
					case 0:
						this.purchaseAddress.firstName = value;
						break;
					case 1:
						this.purchaseAddress.lastName = value;
						break;
					case 2:
						this.purchaseAddress.street = value;
						break;
					case 3:
						this.purchaseAddress.extra = value;
						break;
					case 4:
						this.purchaseAddress.postalCode = value;
						break;
					case 5:
						this.purchaseAddress.city = value;
						break;
					case 6:
						this.purchaseAddress.state = value;
						break;
					}
				}
			}
		}
		for (int i = 0; i < this.countries.Length; i++)
		{
			if (this.countries[i].name == this.countryDrop.GetSelectedItem())
			{
				this.purchaseAddress.countryCode = this.countries[i].countryCode;
				break;
			}
		}
	}

	// Token: 0x06001448 RID: 5192 RVA: 0x0007E284 File Offset: 0x0007C484
	private void PurchaseAddressToForm()
	{
		foreach (ValidatedTextfield validatedTextfield in this.purchaseAddressFields)
		{
			string name = validatedTextfield.GetName();
			if (name != null)
			{
				if (Popups.<>f__switch$map1E == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(7);
					dictionary.Add("paFirstName", 0);
					dictionary.Add("paLastName", 1);
					dictionary.Add("paAddress1", 2);
					dictionary.Add("paAddress2", 3);
					dictionary.Add("paZipCode", 4);
					dictionary.Add("paCity", 5);
					dictionary.Add("paState", 6);
					Popups.<>f__switch$map1E = dictionary;
				}
				int num;
				if (Popups.<>f__switch$map1E.TryGetValue(name, ref num))
				{
					switch (num)
					{
					case 0:
						validatedTextfield.SetValue(this.purchaseAddress.firstName);
						break;
					case 1:
						validatedTextfield.SetValue(this.purchaseAddress.lastName);
						break;
					case 2:
						validatedTextfield.SetValue(this.purchaseAddress.street);
						break;
					case 3:
						validatedTextfield.SetValue(this.purchaseAddress.extra);
						break;
					case 4:
						validatedTextfield.SetValue(this.purchaseAddress.postalCode);
						break;
					case 5:
						validatedTextfield.SetValue(this.purchaseAddress.city);
						break;
					case 6:
						validatedTextfield.SetValue(this.purchaseAddress.state);
						break;
					}
				}
			}
		}
		Country country = null;
		foreach (Country country2 in this.countries)
		{
			if (country2.countryCode == this.purchaseAddress.countryCode)
			{
				country = country2;
			}
		}
		if (country != null)
		{
			this.countryDrop.SetSelectedItem(country.name);
		}
	}

	// Token: 0x06001449 RID: 5193 RVA: 0x000028DF File Offset: 0x00000ADF
	public void OverlayClicked()
	{
	}

	// Token: 0x0600144A RID: 5194 RVA: 0x0000EF6F File Offset: 0x0000D16F
	public void AddCloseButton()
	{
		this.forceShowCloseButton = true;
	}

	// Token: 0x0600144B RID: 5195 RVA: 0x0007E498 File Offset: 0x0007C698
	private bool ShouldShowCloseButton()
	{
		return this.forceShowCloseButton || (this.currentPopupType != Popups.PopupType.INFO_PROGCLOSE && this.currentPopupType != Popups.PopupType.OK && this.currentPopupType != Popups.PopupType.SCROLL_TEXT && this.currentPopupType != Popups.PopupType.TEXT_IMAGE);
	}

	// Token: 0x0400112C RID: 4396
	public const string ImportButton = "_import";

	// Token: 0x0400112D RID: 4397
	private GUISkin regularUISkin;

	// Token: 0x0400112E RID: 4398
	private GUISkin closeButtonSkin;

	// Token: 0x0400112F RID: 4399
	private GUISkin emptySkin;

	// Token: 0x04001130 RID: 4400
	private GUISkin purchaseSkin;

	// Token: 0x04001131 RID: 4401
	private GUIStyle importButtonStyle;

	// Token: 0x04001132 RID: 4402
	private GUIStyle exportButtonStyle;

	// Token: 0x04001133 RID: 4403
	private GUIBlackOverlayButton overlay;

	// Token: 0x04001134 RID: 4404
	private IOkCallback okCallback;

	// Token: 0x04001135 RID: 4405
	private IOkChoiceCallback okChoiceCallback;

	// Token: 0x04001136 RID: 4406
	private IOkStringCallback okStringCallback;

	// Token: 0x04001137 RID: 4407
	private ICancelCallback cancelCallback;

	// Token: 0x04001138 RID: 4408
	private IDeckSaveCallback deckSaveCallback;

	// Token: 0x04001139 RID: 4409
	private IDeckCallback deckChosenCallback;

	// Token: 0x0400113A RID: 4410
	private IGoldShardsCallback goldShardsCallback;

	// Token: 0x0400113B RID: 4411
	private IJoinRoomCallback joinRoomCallback;

	// Token: 0x0400113C RID: 4412
	private GameActionManager towerChallengeChosenCallback;

	// Token: 0x0400113D RID: 4413
	private string header;

	// Token: 0x0400113E RID: 4414
	private string description;

	// Token: 0x0400113F RID: 4415
	private string errorText;

	// Token: 0x04001140 RID: 4416
	private GUIContent okText;

	// Token: 0x04001141 RID: 4417
	private GUIContent cancelText;

	// Token: 0x04001142 RID: 4418
	private string popupType;

	// Token: 0x04001143 RID: 4419
	private int popupId;

	// Token: 0x04001144 RID: 4420
	private static int popupRunningId;

	// Token: 0x04001145 RID: 4421
	private string saveDeckName;

	// Token: 0x04001146 RID: 4422
	private string saveDeckProblems;

	// Token: 0x04001147 RID: 4423
	private string textEntry = string.Empty;

	// Token: 0x04001148 RID: 4424
	private string roomName = string.Empty;

	// Token: 0x04001149 RID: 4425
	private bool firstRun;

	// Token: 0x0400114A RID: 4426
	private Address purchaseAddress;

	// Token: 0x0400114B RID: 4427
	private string purchasePassword;

	// Token: 0x0400114C RID: 4428
	private bool purchaseAddressKnown;

	// Token: 0x0400114D RID: 4429
	private bool purchaseCreditCardKnown;

	// Token: 0x0400114E RID: 4430
	private bool purchaseCardWasStoredInVault;

	// Token: 0x0400114F RID: 4431
	private int activePurchaseId = -1;

	// Token: 0x04001150 RID: 4432
	private bool purchaseIsShardPaymentDetails;

	// Token: 0x04001151 RID: 4433
	private Texture2D purchaseItemImage;

	// Token: 0x04001152 RID: 4434
	public Price purchaseItemPrice;

	// Token: 0x04001153 RID: 4435
	private List<ChatRooms.JoinableRoomDesc> roomList;

	// Token: 0x04001154 RID: 4436
	private GUIContent[] buttonList;

	// Token: 0x04001155 RID: 4437
	private List<DeckInfo> deckList;

	// Token: 0x04001156 RID: 4438
	private bool showReconnectPopup;

	// Token: 0x04001157 RID: 4439
	private bool showDeleteDeckIcon;

	// Token: 0x04001158 RID: 4440
	private bool allowInvalidClicks;

	// Token: 0x04001159 RID: 4441
	private Vector2 deckScroll = Vector2.zero;

	// Token: 0x0400115A RID: 4442
	private Vector2 roomScroll = Vector2.zero;

	// Token: 0x0400115B RID: 4443
	private Vector2 trialScroll = Vector2.zero;

	// Token: 0x0400115C RID: 4444
	private Vector2 scrollTextScroll = Vector2.zero;

	// Token: 0x0400115D RID: 4445
	private bool focusInput;

	// Token: 0x0400115E RID: 4446
	private List<ValidatedTextfield> purchaseAddressFields = new List<ValidatedTextfield>();

	// Token: 0x0400115F RID: 4447
	private List<ValidatedTextfield> purchaseCreditCardFields = new List<ValidatedTextfield>();

	// Token: 0x04001160 RID: 4448
	private Country[] countries;

	// Token: 0x04001161 RID: 4449
	private ShardVariant[] shardInfos;

	// Token: 0x04001162 RID: 4450
	private int goldShardsItemId;

	// Token: 0x04001163 RID: 4451
	private int goldShardGoldCost;

	// Token: 0x04001164 RID: 4452
	private int goldShardShardsCost;

	// Token: 0x04001165 RID: 4453
	private int goldShardGoldCurrent;

	// Token: 0x04001166 RID: 4454
	private int goldShardShardsCurrent;

	// Token: 0x04001167 RID: 4455
	private CardOverlay cardOverlay;

	// Token: 0x04001168 RID: 4456
	private Texture2D image;

	// Token: 0x04001169 RID: 4457
	private bool closeOnOK = true;

	// Token: 0x0400116A RID: 4458
	private bool isLarge;

	// Token: 0x0400116B RID: 4459
	private bool largeLeftAlign;

	// Token: 0x0400116C RID: 4460
	private bool forceShowCloseButton;

	// Token: 0x0400116D RID: 4461
	private bool stayOpen;

	// Token: 0x0400116E RID: 4462
	private int sellCardLowestPrice;

	// Token: 0x0400116F RID: 4463
	private int sellCardAmountForSale;

	// Token: 0x04001170 RID: 4464
	private float sellCardTax;

	// Token: 0x04001171 RID: 4465
	private bool inEditMode;

	// Token: 0x04001172 RID: 4466
	private bool useAsAIDeck;

	// Token: 0x04001173 RID: 4467
	private Popups.PopupType currentPopupType;

	// Token: 0x04001174 RID: 4468
	[SerializeField]
	private RenderTexture cardRenderTexture;

	// Token: 0x04001175 RID: 4469
	private bool rememberCardInformation;

	// Token: 0x04001176 RID: 4470
	private Rect? popupRect;

	// Token: 0x04001177 RID: 4471
	private Action popupRenderer;

	// Token: 0x04001178 RID: 4472
	private Dropdown monthDrop;

	// Token: 0x04001179 RID: 4473
	private Dropdown yearDrop;

	// Token: 0x0400117A RID: 4474
	private Dropdown countryDrop;

	// Token: 0x0400117B RID: 4475
	private GUIStyle highlightedButtonStyle;

	// Token: 0x0400117C RID: 4476
	private float customExtraSpace;

	// Token: 0x0400117D RID: 4477
	private string deckFilter = string.Empty;

	// Token: 0x0400117E RID: 4478
	private int selectedChallengeID;

	// Token: 0x0200038D RID: 909
	private enum PopupType
	{
		// Token: 0x04001183 RID: 4483
		NONE,
		// Token: 0x04001184 RID: 4484
		OK_CANCEL,
		// Token: 0x04001185 RID: 4485
		OK_CHOICE_CANCEL,
		// Token: 0x04001186 RID: 4486
		OK,
		// Token: 0x04001187 RID: 4487
		MULTIBUTTON,
		// Token: 0x04001188 RID: 4488
		DECK_SELECTOR,
		// Token: 0x04001189 RID: 4489
		SAVE_DECK,
		// Token: 0x0400118A RID: 4490
		JOIN_ROOM,
		// Token: 0x0400118B RID: 4491
		INFO_PROGCLOSE,
		// Token: 0x0400118C RID: 4492
		SHARD_PURCHASE_ONE,
		// Token: 0x0400118D RID: 4493
		PURCHASE_PAYMENT_DETAILS,
		// Token: 0x0400118E RID: 4494
		TOWER_CHALLENGE_SELECTOR,
		// Token: 0x0400118F RID: 4495
		TUTORIAL_CHALLENGE_SELECTOR,
		// Token: 0x04001190 RID: 4496
		GOLD_SHARDS_SELECT,
		// Token: 0x04001191 RID: 4497
		PURCHASE_PASSWORD_ENTRY,
		// Token: 0x04001192 RID: 4498
		TEXT_ENTRY,
		// Token: 0x04001193 RID: 4499
		TEXT_AREA,
		// Token: 0x04001194 RID: 4500
		TEXT_AREA_READONLY,
		// Token: 0x04001195 RID: 4501
		SCROLL_TEXT,
		// Token: 0x04001196 RID: 4502
		TEXT_IMAGE,
		// Token: 0x04001197 RID: 4503
		SELL_CARD,
		// Token: 0x04001198 RID: 4504
		CUSTOM_GAME_MULTIPLAYER_SELECTOR,
		// Token: 0x04001199 RID: 4505
		CUSTOM_GAME_SINGLEPLAYER_SELECTOR
	}
}
