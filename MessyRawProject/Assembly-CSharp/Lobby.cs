using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000209 RID: 521
public class Lobby : AbstractCommListener, IOkCallback, ICancelCallback, IOkChoiceCallback, IOkCancelCallback, IOkChoiceCancelCallback, IGoldShardsCallback, IOkStringCallback, IOkStringCancelCallback
{
	// Token: 0x0600105F RID: 4191 RVA: 0x0006DDC8 File Offset: 0x0006BFC8
	private void Start()
	{
		App.ChatUI.SetIsBattling(false);
		this.lobbyGUISkin = (GUISkin)ResourceManager.Load("_GUISkins/Lobby");
		this.regularUI = (GUISkin)ResourceManager.Load("_GUISkins/RegularUI");
		this.clearLobbyGUISkin = ScriptableObject.CreateInstance<GUISkin>();
		this.guiSkinDisabled = (GUISkin)ResourceManager.Load("_GUISkins/DisabledButtons");
		this.GUISkins = new List<GUISkin>();
		GUISkin guiskin = ScriptableObject.CreateInstance<GUISkin>();
		guiskin.button.normal.background = ResourceManager.LoadTexture("Arena/menu_sptut_");
		guiskin.button.hover.background = ResourceManager.LoadTexture("Arena/menu_sptut_mo");
		guiskin.button.active.background = ResourceManager.LoadTexture("Arena/menu_sptut_d");
		this.GUISkins.Add(guiskin);
		GUISkin guiskin2 = ScriptableObject.CreateInstance<GUISkin>();
		guiskin2.button.normal.background = ResourceManager.LoadTexture("Arena/menu_spquick_");
		guiskin2.button.hover.background = ResourceManager.LoadTexture("Arena/menu_spquick_mo");
		guiskin2.button.active.background = ResourceManager.LoadTexture("Arena/menu_spquick_d");
		this.GUISkins.Add(guiskin2);
		GUISkin guiskin3 = ScriptableObject.CreateInstance<GUISkin>();
		guiskin3.button.normal.background = ResourceManager.LoadTexture("Arena/menu_trials_");
		guiskin3.button.hover.background = ResourceManager.LoadTexture("Arena/menu_trials_mo");
		guiskin3.button.active.background = ResourceManager.LoadTexture("Arena/menu_trials_d");
		this.GUISkins.Add(guiskin3);
		GUISkin guiskin4 = ScriptableObject.CreateInstance<GUISkin>();
		guiskin4.button.normal.background = ResourceManager.LoadTexture("Arena/menu_mpquick_");
		guiskin4.button.hover.background = ResourceManager.LoadTexture("Arena/menu_mpquick_mo");
		guiskin4.button.active.background = ResourceManager.LoadTexture("Arena/menu_mpquick_d");
		this.GUISkins.Add(guiskin4);
		GUISkin guiskin5 = ScriptableObject.CreateInstance<GUISkin>();
		guiskin5.button.normal.background = ResourceManager.LoadTexture("Arena/menu_mpranked_");
		guiskin5.button.hover.background = ResourceManager.LoadTexture("Arena/menu_mpranked_mo");
		guiskin5.button.active.background = ResourceManager.LoadTexture("Arena/menu_mpranked_d");
		this.GUISkins.Add(guiskin5);
		GUISkin guiskin6 = ScriptableObject.CreateInstance<GUISkin>();
		guiskin6.button.normal.background = ResourceManager.LoadTexture("Arena/judgement_");
		guiskin6.button.hover.background = ResourceManager.LoadTexture("Arena/judgement_mo");
		guiskin6.button.active.background = ResourceManager.LoadTexture("Arena/judgement_d");
		this.GUISkins.Add(guiskin6);
		this.helpButtonStyle = this.setupGuiButtonStyle(new GUIStyle(), "Icons/menu_help_btn_");
		this.closeButtonStyle = this.setupGuiButtonStyle(new GUIStyle(), "Icons/menu_close_btn_");
		this.audioScript = App.AudioScript;
		this.comm = App.Communicator;
		this.comm.addListener(this);
		this.tradeSystem = new GameObject("Trade System").AddComponent<TradeSystem>();
		this.tradeSystem.Init(0.01f, 0.129f, 0.98f, 0.572f, this.cardRenderTexture);
		App.ChatUI.ChatRoomLeftEvent += this.chatRoomLeft;
		App.LobbyMenu.SetEnabled(true);
		App.ChatUI.SetMode(OnlineState.LOBBY);
		App.ChatUI.SetEnabled(true);
		App.ChatUI.Show(true);
		if (App.SceneValues.loadTradeSystem)
		{
			TradeResponseMessage tradeResponseMessage = App.SceneValues.tradeResponseMessage;
			ProfileInfo profileInfo = tradeResponseMessage.from;
			ProfileInfo profileInfo2 = tradeResponseMessage.to;
			if (tradeResponseMessage.from.id != App.MyProfile.ProfileInfo.id)
			{
				ProfileInfo profileInfo3 = profileInfo;
				profileInfo = profileInfo2;
				profileInfo2 = profileInfo3;
			}
			App.ChatUI.Show(true);
			App.ChatUI.SetLocked(true);
			App.ChatUI.SetCanOpenContextMenu(false);
			this.tradeSystem.StartTrade(this.cardsPlayer1, this.cardsPlayer2, profileInfo.name, profileInfo2.name, App.MyProfile.ProfileData.gold);
			this.waitingForTradeRoom = true;
			App.SceneValues.loadTradeSystem = false;
		}
		this.audioScript.PlayMusic("Music/Theme");
		if (App.WaitingReward != null)
		{
			SceneLoader.loadScene("_LimitedReward");
		}
		else
		{
			App.LobbyMenu.fadeInScene();
			App.GlobalMessageHandler.LimitedRewardEvent += this.LimitedRewardEvent;
			this.doneLoading = true;
			if (App.SceneValues.lobby != null && App.SceneValues.lobby.enterLimited)
			{
				App.SceneValues.lobby = null;
				App.Communicator.send(new DeckListLimitedMessage());
			}
		}
	}

	// Token: 0x06001060 RID: 4192 RVA: 0x0006E298 File Offset: 0x0006C498
	private GUIStyle setupGuiButtonStyle(GUIStyle style, string baseFilename)
	{
		style.normal.background = ResourceManager.LoadTexture(baseFilename);
		style.hover.background = ResourceManager.LoadTexture(baseFilename + "mo");
		style.active.background = ResourceManager.LoadTexture(baseFilename + "d");
		return style;
	}

	// Token: 0x06001061 RID: 4193 RVA: 0x0000CB95 File Offset: 0x0000AD95
	private void LimitedRewardEvent()
	{
		SceneLoader.loadScene("_LimitedReward");
	}

	// Token: 0x06001062 RID: 4194 RVA: 0x0000CBA1 File Offset: 0x0000ADA1
	public override void OnDestroy()
	{
		base.OnDestroy();
		if (this.doneLoading)
		{
			App.GlobalMessageHandler.LimitedRewardEvent -= this.LimitedRewardEvent;
		}
		App.ChatUI.ChatRoomLeftEvent -= this.chatRoomLeft;
	}

	// Token: 0x06001063 RID: 4195 RVA: 0x0006E2F0 File Offset: 0x0006C4F0
	public void chatRoomLeft(string room)
	{
		if (this.tradeSystem != null && this.tradeSystem.IsInTrade() && this.tradeSystem.GetTradeRoomName() == room)
		{
			App.Communicator.send(new TradeCancelMessage());
		}
	}

	// Token: 0x06001064 RID: 4196 RVA: 0x0000CBE0 File Offset: 0x0000ADE0
	private void FixedUpdate()
	{
		if (this.fadeIn > 0f)
		{
			this.fadeIn -= 0.03f;
		}
	}

	// Token: 0x06001065 RID: 4197 RVA: 0x0000CC04 File Offset: 0x0000AE04
	private void Update()
	{
		App.LobbyMenu.SetButtonsEnabled(!this.tradeSystem.IsInTrade());
	}

	// Token: 0x06001066 RID: 4198 RVA: 0x0006E344 File Offset: 0x0006C544
	private static string stripTextFromFullVersionNag(string s)
	{
		if (App.IsDemo())
		{
			return s;
		}
		int num = s.IndexOf("\n(");
		if (num < 0)
		{
			return s;
		}
		return s.Substring(0, num).TrimEnd(new char[0]);
	}

	// Token: 0x06001067 RID: 4199 RVA: 0x0006E388 File Offset: 0x0006C588
	private void viewLimitedDeck()
	{
		SceneValues.SV_DeckBuilder sv_DeckBuilder = new SceneValues.SV_DeckBuilder();
		sv_DeckBuilder.isLimitedMode = true;
		sv_DeckBuilder.limitedDeckInfo = this.limitedDeckToPlayWith;
		App.SceneValues.deckBuilder = sv_DeckBuilder;
		SceneLoader.loadScene("_DeckBuilderView");
	}

	// Token: 0x06001068 RID: 4200 RVA: 0x0006E3C4 File Offset: 0x0006C5C4
	public override void handleMessage(Message msg)
	{
		if (msg is DeckListLimitedMessage)
		{
			DeckListLimitedMessage deckListLimitedMessage = (DeckListLimitedMessage)msg;
			if (deckListLimitedMessage.decks.Length > 0)
			{
				this.limitedDeckToPlayWith = deckListLimitedMessage.decks[0];
				LimitedDeckInfo limitedDeckInfo = this.limitedDeckToPlayWith;
				App.Popups.ShowOkChoiceCancel(this, "judgementchoice", "Judgement", string.Concat(new object[]
				{
					"Current deck: <color=#eeddaa>",
					limitedDeckInfo.name,
					"</color>\n<color=#bbaa99>Wins: ",
					limitedDeckInfo.wins,
					"/",
					limitedDeckInfo.winsMax,
					"\nLosses: ",
					limitedDeckInfo.losses,
					"/",
					limitedDeckInfo.lossesMax,
					"</color>\n\nWhat would you like to do?"
				}), "Play match", "Deck options");
			}
			else
			{
				this.comm.send(new GetInformationLimitedMessage());
			}
		}
		if (!(msg is OkMessage) || ((OkMessage)msg).isType(typeof(WithdrawLimitedMessage)))
		{
		}
		if (msg is GetHelpLimitedMessage)
		{
			string text = Lobby.stripTextFromFullVersionNag(((GetHelpLimitedMessage)msg).help);
			App.Popups.ShowTextImage(this, "limitedpopup", "How does Judgement work?", "<color=#eeddcc>" + text + "</color>", ResourceManager.LoadTexture("Limited/limitedScreen"), "Ok");
		}
		if (msg is GetHelpForGameTypeMessage)
		{
			GetHelpForGameTypeMessage getHelpForGameTypeMessage = (GetHelpForGameTypeMessage)msg;
			string @string = getHelpForGameTypeMessage.gameType.getString();
			string text2 = Lobby.stripTextFromFullVersionNag(getHelpForGameTypeMessage.help);
			string filename = "Help/help_decor_" + getHelpForGameTypeMessage.gameType.ToString().ToLower();
			App.Popups.ShowTextImage(this, "helpgametypepopup", @string, "<color=#eeddcc>" + text2 + "</color>", ResourceManager.LoadTexture(filename), "Ok");
		}
		if (msg is GetHelpTestingGroundsMessage)
		{
			App.Popups.ShowTextImage(this, "testgroundspopup", "What are the Testing Grounds?", "<color=#eeddcc>" + ((GetHelpTestingGroundsMessage)msg).help + "</color>", ResourceManager.LoadTexture("Arena/testingGrounds"), "Ok");
		}
		if (msg is GetInformationLimitedMessage)
		{
			GetInformationLimitedMessage getInformationLimitedMessage = (GetInformationLimitedMessage)msg;
			if (getInformationLimitedMessage.hasInProgressLimited)
			{
				SceneLoader.loadScene("_Limited");
			}
			else if (getInformationLimitedMessage.priceGold == 0)
			{
				string description = (!App.ServerSettings.shardsEnabled) ? "Welcome! Building a Judgement deck costs a small amount\nof gold, but the first one is on us. Enjoy!" : "Welcome! Building a Judgement deck costs a small amount\nof gold or shards, but the first one is on us. Enjoy!";
				App.Popups.ShowOk(this, "limitedfirsttime", "Judgement Entry", description, "Begin");
			}
			else
			{
				App.Popups.ShowGoldShardsSelector(this, "goldshards", "Judgement Entry", -1, getInformationLimitedMessage.priceGold, getInformationLimitedMessage.priceShards, App.MyProfile.ProfileData.gold, App.MyProfile.ProfileData.shards);
			}
			App.SceneValues.limited = new SceneValues.SV_Limited();
			App.SceneValues.limited.cardsPerRow = getInformationLimitedMessage.cardsPerRow;
			App.SceneValues.limited.targetCollectionSize = getInformationLimitedMessage.collectionSize;
		}
		if (msg is EnterLimitedMessage)
		{
			SceneLoader.loadScene("_Limited");
		}
		if (msg is TradeDeckInvalidationWarningMessage)
		{
			TradeDeckInvalidationWarningMessage tradeDeckInvalidationWarningMessage = (TradeDeckInvalidationWarningMessage)msg;
			App.Popups.ShowOk(this, "deckinvalidationwarning", "Notice", "The added scroll will be removed from the\nfollowing decks if you accept this trade:\n\n" + DeckUtil.GetFormattedDeckNames(tradeDeckInvalidationWarningMessage.deckNames), "Ok");
		}
		if (msg is TradeResponseMessage)
		{
			TradeResponseMessage tradeResponseMessage = (TradeResponseMessage)msg;
			if (tradeResponseMessage.status == "CANCEL_BARGAIN")
			{
				this.tradeSystem.CloseTrade();
				App.ChatUI.SetLocked(false);
				App.ChatUI.SetCanOpenContextMenu(true);
			}
		}
		if (msg is TradeViewMessage)
		{
			TradeViewMessage tradeViewMessage = (TradeViewMessage)msg;
			if (tradeViewMessage.from.accepted && tradeViewMessage.to.accepted)
			{
				this.tradeSystem.CloseTrade();
				App.ChatUI.SetLocked(false);
				App.ChatUI.SetCanOpenContextMenu(true);
				App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_trade_complete");
			}
			else if (tradeViewMessage.from.profile.id == App.MyProfile.ProfileInfo.id)
			{
				this.tradeSystem.UpdateView(tradeViewMessage.modified, tradeViewMessage.from, tradeViewMessage.to);
			}
			else
			{
				this.tradeSystem.UpdateView(tradeViewMessage.modified, tradeViewMessage.to, tradeViewMessage.from);
			}
		}
		if (msg is LibraryViewMessage)
		{
			bool flag = ((LibraryViewMessage)msg).profileId == App.MyProfile.ProfileInfo.id;
			List<Card> list = (!flag) ? this.cardsPlayer2 : this.cardsPlayer1;
			list.Clear();
			foreach (Card card in ((LibraryViewMessage)msg).cards)
			{
				list.Add(card);
			}
			list.Sort(this.sorter);
			this.tradeSystem.UpdateView(false, new TradeInfo(), new TradeInfo());
		}
		if (msg is RoomEnterMessage)
		{
			RoomEnterMessage roomEnterMessage = (RoomEnterMessage)msg;
			if (this.waitingForTradeRoom)
			{
				this.tradeSystem.SetTradeRoomName(roomEnterMessage.roomName);
				this.waitingForTradeRoom = false;
			}
		}
		base.handleMessage(msg);
	}

	// Token: 0x06001069 RID: 4201 RVA: 0x0006E944 File Offset: 0x0006CB44
	private static string lookingForOpponentString()
	{
		string[] array = new string[]
		{
			"Looking for opponent",
			"Looking for opponent.",
			"Looking for opponent..",
			"Looking for opponent..."
		};
		long num = (long)(Time.time * 1.5f);
		return array[(int)(checked((IntPtr)(num % unchecked((long)array.Length))))];
	}

	// Token: 0x0600106A RID: 4202 RVA: 0x0006E990 File Offset: 0x0006CB90
	private void drawGameQueueIconFor(Rect r, GameType gameType, Vector2 textOffset)
	{
		Rect rect = GeomUtil.cropShare(r, new Rect(0f, 0.35f, 1f, 0.65f));
		rect.width = rect.height;
		GUI.DrawTexture(rect, ResourceManager.LoadTexture("Arena/activity_thing_"));
		bool flag = App.LobbyMenu.isQueuedIn(gameType);
		if (flag)
		{
			GUI.color = new Color(1f, 1f, 1f, TimeUtil.sin01ForSpeed(2f));
			TextAnchor alignment = GUI.skin.label.alignment;
			GUI.skin.label.alignment = 3;
			GUI.DrawTexture(rect, ResourceManager.LoadTexture("Arena/activity_thing_on_only_glow"));
			GUI.color = Color.white;
			rect.x += rect.width;
			rect.width = r.width;
			rect.x += textOffset.x;
			rect.y += textOffset.y;
			GUIUtil.drawShadowText(rect, Lobby.lookingForOpponentString(), this.queueTextColor);
			GUI.skin.label.alignment = alignment;
		}
		this.drawHelpOrCloseButton(r, gameType, false);
	}

	// Token: 0x0600106B RID: 4203 RVA: 0x0006EAC4 File Offset: 0x0006CCC4
	private void drawHelpOrCloseButton(Rect r, GameType gameType, bool canTrigger)
	{
		bool flag = App.LobbyMenu.isQueuedIn(gameType);
		Rect rect = GeomUtil.cropShare(r, new Rect(0.8f, 0f, 0f, 0.7f));
		rect.width = rect.height;
		GUIStyle guistyle = (!flag) ? this.helpButtonStyle : this.closeButtonStyle;
		if (GUI.Button(rect, string.Empty, guistyle) && canTrigger)
		{
			if (flag)
			{
				App.Communicator.send(new ExitMultiPlayerQueueMessage(gameType));
			}
			else
			{
				this.showHelpFor(gameType);
			}
		}
	}

	// Token: 0x0600106C RID: 4204 RVA: 0x0000CC1E File Offset: 0x0000AE1E
	private void showHelpFor(GameType gameType)
	{
		if (gameType.isLimited())
		{
			App.Communicator.send(new GetHelpLimitedMessage());
		}
		else
		{
			App.Communicator.send(new GetHelpForGameTypeMessage(gameType));
		}
	}

	// Token: 0x0600106D RID: 4205 RVA: 0x0006EB60 File Offset: 0x0006CD60
	private void OnGUI()
	{
		GUI.depth = 21;
		GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), ResourceManager.LoadTexture("DeckBuilder/bg"));
		if (!this.tradeSystem.IsInTrade())
		{
			GUI.skin = this.lobbyGUISkin;
			int buttonFontSize = LobbyMenu.getButtonFontSize();
			GUI.skin.label.fontSize = buttonFontSize;
			GUI.skin.button.fontSize = buttonFontSize;
			GUI.skin = this.guiSkinDisabled;
			GUI.skin.label.fontSize = buttonFontSize;
			GUI.skin.button.fontSize = buttonFontSize;
			MockupCalc mockupCalc = new MockupCalc(2048, 1536);
			float num = mockupCalc.X(1000f);
			float num2 = mockupCalc.X(1048f);
			float num3 = mockupCalc.Y(260f);
			float y = mockupCalc.Y(348f);
			float num4 = mockupCalc.Y(548f);
			float num5 = mockupCalc.Y(748f);
			Rect rectFor = this.getRectFor(0, num, y, 2);
			Rect rectFor2 = this.getRectFor(0, num, num4, 2);
			Rect rectFor3 = this.getRectFor(0, num, num5, 2);
			Rect rectFor4 = this.getRectFor(0, num2, y, 0);
			Rect rectFor5 = this.getRectFor(0, num2, num4, 0);
			Rect rectFor6 = this.getRectFor(0, num2, num5, 0);
			for (int i = 0; i < 2; i++)
			{
				Rect rect = (i != 0) ? rectFor4 : rectFor;
				string filename = (i != 0) ? "Arena/button_bg_flag_mp" : "Arena/button_bg_flag_sp";
				rect.yMin = LobbyMenu.getMenuRect().yMax * 1f;
				rect.yMax += 5f * (num5 - num4);
				GUI.DrawTexture(rect, ResourceManager.LoadTexture(filename));
			}
			float num6 = 0.85f;
			float num7 = 0.75f;
			GUI.color = new Color(num6, num7, num7, 1f);
			Rect rectFor7 = this.getRectFor(0, num, num3, 2);
			Texture2D texture2D = ResourceManager.LoadTexture("Arena/menu_text_singleplayer");
			Rect rect2 = mockupCalc.prAspectH(new Vector2(0f, num3), (float)texture2D.width, (float)texture2D.height);
			rect2.x = rectFor7.x + (rectFor7.width - rect2.width) / 2f;
			GUI.DrawTexture(rect2, texture2D);
			Rect rectFor8 = this.getRectFor(0, num2, num3, 0);
			Texture2D texture2D2 = ResourceManager.LoadTexture("Arena/menu_text_multiplayer");
			Rect rect3 = mockupCalc.prAspectH(new Vector2(0f, num3), (float)texture2D2.width, (float)texture2D2.height);
			rect3.x = rectFor8.x + (rectFor8.width - rect3.width) / 2f;
			GUI.DrawTexture(rect3, texture2D2);
			GUI.color = Color.white;
			this.drawHelpOrCloseButton(rectFor, GameType.SP_TUTORIAL, true);
			this.drawHelpOrCloseButton(rectFor2, GameType.SP_TOWERMATCH, true);
			this.drawHelpOrCloseButton(rectFor3, GameType.SP_QUICKMATCH, true);
			if (this.drawImageButton(rectFor, 0))
			{
				App.GameActionManager.StartGame(GameActionManager.StartType.START_TUTORIAL);
			}
			if (this.drawImageButton(rectFor2, 2))
			{
				App.GameActionManager.StartGame(GameActionManager.StartType.START_TOWER_CHALLENGE);
			}
			if (this.drawImageButton(rectFor3, 1))
			{
				App.GameActionManager.StartGame(GameActionManager.StartType.START_SINGLEPLAYER_QUICK);
			}
			this.drawHelpOrCloseButton(rectFor, GameType.SP_TUTORIAL, true);
			this.drawHelpOrCloseButton(rectFor2, GameType.SP_TOWERMATCH, true);
			this.drawHelpOrCloseButton(rectFor3, GameType.SP_QUICKMATCH, true);
			this.drawHelpOrCloseButton(rectFor4, GameType.MP_QUICKMATCH, true);
			this.drawHelpOrCloseButton(rectFor5, GameType.MP_RANKED, true);
			this.drawHelpOrCloseButton(rectFor6, GameType.MP_LIMITED, true);
			if (App.ServerSettings.mpQuickMatchEnabled)
			{
				if (this.drawImageButton(rectFor4, 3))
				{
					this.buttonMpQuick();
				}
			}
			else
			{
				App.GUI.DrawTexture(rectFor4, new GUIContent(ResourceManager.LoadTexture("Arena/menu_mpquick_nope")));
			}
			if (this.drawImageButton(rectFor5, 4))
			{
				this.buttonMpRanked();
			}
			if (this.drawImageButton(rectFor6, 5))
			{
				this.buttonMpLimited();
			}
			float height = this.getRectFor(0, 0f, 0f, 0).height;
			Vector2 textOffset;
			textOffset..ctor(0.23f * height, 0.16f * height);
			this.drawGameQueueIconFor(rectFor4, GameType.MP_QUICKMATCH, textOffset);
			this.drawGameQueueIconFor(rectFor5, GameType.MP_RANKED, textOffset);
			this.drawGameQueueIconFor(rectFor6, GameType.MP_LIMITED, textOffset);
			GUI.skin = this.regularUI;
			int fontSize = GUI.skin.button.fontSize;
			float width = this.getRectFor(0, 0f, 0f, 0).width;
			if (GUI.Button(GeomUtil.scaleCentered(new Rect(num - width, num5 + (num5 - num4), width, (float)Screen.height * 0.05f), 0.75f, 1f), "Tips & tricks"))
			{
				Application.OpenURL("http://academy.scrollsguide.com/guide/the-fundamentals-of-scrolls");
			}
			if (GUI.Button(GeomUtil.scaleCentered(new Rect(num2, num5 + (num5 - num4), width, (float)Screen.height * 0.05f), 0.75f, 1f), "Watch games"))
			{
				SceneLoader.loadScene("_Watch");
			}
			GUI.skin.button.fontSize = fontSize;
			GUI.skin = this.lobbyGUISkin;
			GUI.color = new Color(1f, 1f, 1f, this.fadeIn);
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), ResourceManager.LoadTexture("Shared/blackFiller"));
			GUI.color = new Color(1f, 1f, 1f, 1f);
		}
	}

	// Token: 0x0600106E RID: 4206 RVA: 0x0006F0E4 File Offset: 0x0006D2E4
	private Rect getRectFor(int index, float x, float y, TextAlignment align)
	{
		Texture2D background = this.GUISkins[0].button.normal.background;
		MockupCalc mockupCalc = new MockupCalc(2048, 1536);
		float width = (float)background.width;
		float height = (float)background.height;
		Rect result = mockupCalc.prAspectH(new Vector2(x, y), width, height);
		if (align == 1)
		{
			result.x -= result.width / 2f;
		}
		if (align == 2)
		{
			result.x -= result.width;
		}
		return result;
	}

	// Token: 0x0600106F RID: 4207 RVA: 0x0000CC51 File Offset: 0x0000AE51
	private bool drawImageButtonCentered(int index, float x, float y)
	{
		return this.drawImageButton(index, x, y, 1);
	}

	// Token: 0x06001070 RID: 4208 RVA: 0x0000CC5D File Offset: 0x0000AE5D
	private bool drawImageButton(int index, float x, float y, TextAlignment align)
	{
		return this.drawImageButton(this.getRectFor(index, x, y, align), index);
	}

	// Token: 0x06001071 RID: 4209 RVA: 0x0006F180 File Offset: 0x0006D380
	private bool drawImageButton(Rect r, int index)
	{
		bool flag = index == 5;
		GUIStyle button = this.GUISkins[index].button;
		GUIContent c = (!flag) ? this.contentUnlocked : this.contentCenterLocked;
		if (index == 0 && App.Config.settings.showTutorialArrows())
		{
			c = this.contentHelpArrow;
		}
		return App.GUI.Button(r, c, button);
	}

	// Token: 0x06001072 RID: 4210 RVA: 0x0006F1EC File Offset: 0x0006D3EC
	public void PopupOk(string popupType)
	{
		if (popupType == "limitedfirsttime")
		{
			App.Communicator.send(new EnterLimitedMessage(false));
		}
		if (popupType == "withdrawlimited")
		{
			App.SceneValues.lobby = new SceneValues.SV_Lobby();
			App.SceneValues.lobby.enterLimited = true;
			App.Communicator.send(new WithdrawLimitedMessage(this.limitedDeckToPlayWith.name));
		}
	}

	// Token: 0x06001073 RID: 4211 RVA: 0x0006F264 File Offset: 0x0006D464
	public void PopupOk(string popupType, int choice)
	{
		if (popupType == "judgementchoice")
		{
			if (choice == 1)
			{
				App.Popups.ShowMultibutton(this, "judgementoptions", "Deck options", new GUIContent[]
				{
					new GUIContent("View deck"),
					new GUIContent("Build new deck")
				});
			}
			else
			{
				App.GameActionManager.StartLimited(this.limitedDeckToPlayWith.name);
			}
		}
	}

	// Token: 0x06001074 RID: 4212 RVA: 0x000028DF File Offset: 0x00000ADF
	public void PopupCancel(string popupType)
	{
	}

	// Token: 0x06001075 RID: 4213 RVA: 0x0000CC71 File Offset: 0x0000AE71
	public void PopupGoldSelected(int itemId)
	{
		App.Communicator.send(new EnterLimitedMessage(false));
	}

	// Token: 0x06001076 RID: 4214 RVA: 0x0000CC84 File Offset: 0x0000AE84
	public void PopupShardsSelected(int itemId)
	{
		App.Communicator.send(new EnterLimitedMessage(true));
	}

	// Token: 0x06001077 RID: 4215 RVA: 0x0000CC97 File Offset: 0x0000AE97
	public void ShowShardsPurchase()
	{
		App.SceneValues.store.openShardPurchasePopup = true;
		SceneLoader.loadScene("_Store");
	}

	// Token: 0x06001078 RID: 4216 RVA: 0x0006F2D8 File Offset: 0x0006D4D8
	private void buttonMpQuick()
	{
		if (App.LobbyMenu.isQueuedIn(GameType.MP_QUICKMATCH))
		{
			App.Communicator.send(new ExitMultiPlayerQueueMessage(GameType.MP_QUICKMATCH));
		}
		else
		{
			GameActionManager.StartType startType = GameActionManager.fromMpGameType(GameType.MP_QUICKMATCH);
			App.GameActionManager.StartGame(startType);
		}
	}

	// Token: 0x06001079 RID: 4217 RVA: 0x0006F320 File Offset: 0x0006D520
	private void buttonMpRanked()
	{
		if (App.LobbyMenu.isQueuedIn(GameType.MP_RANKED))
		{
			App.Communicator.send(new ExitMultiPlayerQueueMessage(GameType.MP_RANKED));
		}
		else
		{
			GameActionManager.StartType startType = GameActionManager.fromMpGameType(GameType.MP_RANKED);
			App.GameActionManager.StartGame(startType);
		}
	}

	// Token: 0x0600107A RID: 4218 RVA: 0x0000CCB3 File Offset: 0x0000AEB3
	private void buttonMpLimited()
	{
		if (App.LobbyMenu.isQueuedIn(GameType.MP_LIMITED))
		{
			App.Communicator.send(new ExitMultiPlayerQueueMessage(GameType.MP_LIMITED));
		}
		else
		{
			App.Communicator.send(new DeckListLimitedMessage());
		}
	}

	// Token: 0x0600107B RID: 4219 RVA: 0x0006F368 File Offset: 0x0006D568
	public void PopupOk(string popupType, string choice)
	{
		if (popupType == "judgementoptions")
		{
			if (choice == "View deck")
			{
				this.viewLimitedDeck();
			}
			if (choice == "Build new deck")
			{
				App.Popups.ShowOkCancel(this, "withdrawlimited", "Withdraw current deck?", "You can only have one Judgement deck at a time. Would you like to withdraw your current deck and collect the rewards?", "Withdraw", "Keep deck");
			}
		}
	}

	// Token: 0x04000CEE RID: 3310
	private const int ID_TUTORIAL = 0;

	// Token: 0x04000CEF RID: 3311
	private const int ID_SP_QUICK = 1;

	// Token: 0x04000CF0 RID: 3312
	private const int ID_SP_TOWER = 2;

	// Token: 0x04000CF1 RID: 3313
	private const int ID_MP_QUICK = 3;

	// Token: 0x04000CF2 RID: 3314
	private const int ID_MP_RANKED = 4;

	// Token: 0x04000CF3 RID: 3315
	private const int ID_MP_LIMITED = 5;

	// Token: 0x04000CF4 RID: 3316
	private Communicator comm;

	// Token: 0x04000CF5 RID: 3317
	private AudioScript audioScript;

	// Token: 0x04000CF6 RID: 3318
	private GUISkin lobbyGUISkin;

	// Token: 0x04000CF7 RID: 3319
	private GUISkin regularUI;

	// Token: 0x04000CF8 RID: 3320
	private GUISkin clearLobbyGUISkin;

	// Token: 0x04000CF9 RID: 3321
	private GUISkin guiSkinDisabled;

	// Token: 0x04000CFA RID: 3322
	private GUIStyle helpButtonStyle;

	// Token: 0x04000CFB RID: 3323
	private GUIStyle closeButtonStyle;

	// Token: 0x04000CFC RID: 3324
	private float fadeIn = 1f;

	// Token: 0x04000CFD RID: 3325
	private TradeSystem tradeSystem;

	// Token: 0x04000CFE RID: 3326
	private List<Card> cardsPlayer1 = new List<Card>();

	// Token: 0x04000CFF RID: 3327
	private List<Card> cardsPlayer2 = new List<Card>();

	// Token: 0x04000D00 RID: 3328
	private DeckSorter sorter = new DeckSorter().byColor().byName().byLevel();

	// Token: 0x04000D01 RID: 3329
	private List<GUISkin> GUISkins;

	// Token: 0x04000D02 RID: 3330
	[SerializeField]
	private RenderTexture cardRenderTexture;

	// Token: 0x04000D03 RID: 3331
	private bool doneLoading;

	// Token: 0x04000D04 RID: 3332
	private LimitedDeckInfo limitedDeckToPlayWith;

	// Token: 0x04000D05 RID: 3333
	private bool waitingForTradeRoom;

	// Token: 0x04000D06 RID: 3334
	private Color queueTextColor = ColorUtil.FromHex24(14338438u);

	// Token: 0x04000D07 RID: 3335
	private readonly GUIContent contentUnlocked = new GUIContent();

	// Token: 0x04000D08 RID: 3336
	private readonly GUIContent contentLocked = new GUIContent().lockDemoHideIfPopup();

	// Token: 0x04000D09 RID: 3337
	private readonly GUIContent contentCenterLocked = new GUIContent().lockDemoHideIfPopup().center();

	// Token: 0x04000D0A RID: 3338
	private readonly GUIContent contentHelpArrow = new GUIContent().helpArrow();
}
