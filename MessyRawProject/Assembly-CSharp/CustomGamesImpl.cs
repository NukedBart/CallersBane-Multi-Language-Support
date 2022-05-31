using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000185 RID: 389
public class CustomGamesImpl : ICommListener, IOkCallback, ICancelCallback, IOkCancelCallback
{
	// Token: 0x06000C10 RID: 3088 RVA: 0x00054EC4 File Offset: 0x000530C4
	public CustomGamesImpl()
	{
		this.regularUISkin = (GUISkin)ResourceManager.Load("_GUISkins/RegularUI");
		this.emptySkin = (GUISkin)ResourceManager.Load("_GUISkins/EmptySkin");
		this.styleBet = new GUIStyle(this.regularUISkin.label);
		this.styleBet.alignment = 3;
		this.styleShowDeck = new GUIStyle(this.regularUISkin.button);
		this.styleCloseButton = ((GUISkin)ResourceManager.Load("_GUISkins/CloseButton")).button;
		this.styleFlavour = new GUIStyle(this.regularUISkin.label);
		this.styleFlavour.alignment = 0;
		this.styleFlavour.normal.textColor = new Color(0.8f, 0.73f, 0.65f);
		this.styleFlavour.wordWrap = true;
		this.styleTitle = new GUIStyle(this.regularUISkin.label);
		this.styleTitle.wordWrap = false;
		this.styleTitle.alignment = 0;
		this.styleTitle.normal.textColor = this.textColor;
		this.styleDeckTitle = new GUIStyle(this.styleTitle);
		this.currentPopupType = CustomGamesImpl.PopupType.NONE;
		this.highlightedButtonStyle = new GUIStyle(this.regularUISkin.button);
		this.highlightedButtonStyle.normal.background = this.highlightedButtonStyle.hover.background;
		this.challengee = App.SceneValues.customGames.challengee;
		this.resolution = ChangeDetectors.resolution();
		this.ShowCustomGameSelector(App.SceneValues.customGames.isSinglePlayer);
		this.searchfield = new TextField(App.SceneValues.customGames.lastSearch, " Search for title or author", this.regularUISkin.textField);
	}

	// Token: 0x06000C12 RID: 3090 RVA: 0x00055100 File Offset: 0x00053300
	private void ShowCustomGameSelector(bool isSinglePlayer)
	{
		this.showDecks = false;
		this.isSinglePlayer = isSinglePlayer;
		this.codeEntry = string.Empty;
		this.inputScroll = (this.gamesScroll = (CustomGamesImpl.detailsScroll = Vector2.zero));
		this.inEditMode = false;
		this.selectedCustomGameIndex = -1;
		this.currentPopupType = ((!isSinglePlayer) ? CustomGamesImpl.PopupType.CUSTOM_GAME_MULTIPLAYER_SELECTOR : CustomGamesImpl.PopupType.CUSTOM_GAME_SINGLEPLAYER_SELECTOR);
		this.nameEntry = string.Empty;
	}

	// Token: 0x06000C13 RID: 3091 RVA: 0x0005516C File Offset: 0x0005336C
	private void setupFontSizes()
	{
		float num = 0.001f * (float)Screen.height;
		GUIStyle guistyle = this.styleBet;
		int fontSize = (int)(26f * num);
		this.styleShowDeck.fontSize = fontSize;
		guistyle.fontSize = fontSize;
		this.styleFlavour.fontSize = (int)(27f * num);
		this.styleTitle.fontSize = (int)(37.5f * num);
		this.styleDeckTitle.fontSize = (int)(37.5f * num);
	}

	// Token: 0x06000C14 RID: 3092 RVA: 0x000551E0 File Offset: 0x000533E0
	private void setupPositions()
	{
		this.guiHolder._u = 0.001f * (float)Screen.height;
		this.setupFontSizes();
		this.guiHolder.buttonHeight = this.guiHolder.u(50f);
		this.guiHolder.fullRect = GUIUtil.screen(new Rect(0f, 0.15f, 1f, 0.83f));
		this.guiHolder.leftRect = GeomUtil.cropShare(this.guiHolder.fullRect, new Rect(0.02f, 0f, 0.38f, 0.98f));
		this.guiHolder.rightRect = GeomUtil.cropShare(this.guiHolder.fullRect, new Rect(0.42f, 0f, 0.56f, 0.92f));
		if (this.inEditMode)
		{
			this.guiHolder.detailsRect = GeomUtil.cropShare(this.guiHolder.rightRect, new Rect(0f, 0f, 1f, 0.6f));
			this.guiHolder.inputRect = GeomUtil.cropShare(this.guiHolder.rightRect, new Rect(-0.005f, 0.61f, 1.01f, 0.32f));
			this.guiHolder.inputNameRect = this.guiHolder.inputRect;
			this.guiHolder.inputNameRect.height = (float)this.styleTitle.fontSize;
			this.guiHolder.inputNameRect.y = this.guiHolder.inputRect.yMax;
		}
		else
		{
			this.guiHolder.detailsRect = this.guiHolder.rightRect;
		}
		this.guiHolder.details = CustomGamesDetailsHolder.defaultFromRect(this.guiHolder.detailsRect, this.guiHolder.u(1f), null);
		this.guiHolder.buttonRect = new Rect(0f, 0f, this.guiHolder.leftRect.width * 0.45f, this.guiHolder.u(55f));
		this.guiHolder.buttonRect.x = this.guiHolder.detailsRect.x + (this.guiHolder.detailsRect.width - this.guiHolder.buttonRect.width) * 0.5f;
		this.guiHolder.buttonRect.y = this.guiHolder.detailsRect.yMax - this.guiHolder.u(15f) - this.guiHolder.buttonRect.height;
		float num = this.guiHolder.u(15f);
		Rect rect = GeomUtil.inflate(this.guiHolder.leftRect, -num, -num);
		this.guiHolder.gamesListRectInner = rect;
		CustomGamesGuiHolder customGamesGuiHolder = this.guiHolder;
		customGamesGuiHolder.gamesListRectInner.yMin = customGamesGuiHolder.gamesListRectInner.yMin + this.guiHolder.buttonHeight;
		this.guiHolder.searchInputRect = rect;
		this.guiHolder.searchInputRect.yMin = rect.yMax - this.guiHolder.buttonHeight;
		this.guiHolder.gamesListRectInner.yMax = this.guiHolder.searchInputRect.y - this.guiHolder.u(10f);
	}

	// Token: 0x06000C15 RID: 3093 RVA: 0x00009DD2 File Offset: 0x00007FD2
	private void updateCheckResolutionChanged()
	{
		if (this.resolution.IsChanged())
		{
			this.setupPositions();
		}
	}

	// Token: 0x06000C16 RID: 3094 RVA: 0x00055540 File Offset: 0x00053740
	public void OnGUI()
	{
		this.updateCheckResolutionChanged();
		GUI.depth = 21;
		GUI.DrawTexture(GUIUtil.screen(), ResourceManager.LoadTexture("DeckBuilder/bg"));
		App.GUI.clearTooltip();
		GUI.skin = this.regularUISkin;
		int fontSize = GUI.skin.button.fontSize;
		GUI.skin.button.fontSize = 10 + Screen.height / 72;
		if (this.currentPopupType == CustomGamesImpl.PopupType.CUSTOM_GAME_MULTIPLAYER_SELECTOR)
		{
			this.DrawCustomMatchSelector(App.CustomMatchMultiplayerInfo);
		}
		else if (this.currentPopupType == CustomGamesImpl.PopupType.CUSTOM_GAME_SINGLEPLAYER_SELECTOR)
		{
			this.DrawCustomMatchSelector(App.CustomMatchSingleplayerInfo);
		}
		GUI.skin.button.fontSize = fontSize;
		App.GUI.drawTooltip();
	}

	// Token: 0x06000C17 RID: 3095 RVA: 0x00009DEA File Offset: 0x00007FEA
	public void OnDestroy()
	{
		App.SceneValues.customGames.lastSearch = this.searchfield.text().Trim();
	}

	// Token: 0x06000C18 RID: 3096 RVA: 0x00009E0B File Offset: 0x0000800B
	private void drawBox(Rect rect)
	{
		GUI.color = new Color(1f, 1f, 1f, 0.4f);
		GUI.Box(rect, string.Empty, this.regularUISkin.box);
		GUI.color = Color.white;
	}

	// Token: 0x06000C19 RID: 3097 RVA: 0x000555FC File Offset: 0x000537FC
	private void DrawCustomMatchSelector(GetCustomGamesMessage message)
	{
		CustomGameInfo[] array = (this.selectedCustomGameTypeIndex != 0) ? ((this.selectedCustomGameTypeIndex != 1) ? message.mine : message.popular) : message.recent;
		bool isShowingMyGames = message.mine == array;
		TextAnchor alignment = GUI.skin.label.alignment;
		int num = Screen.height / 40;
		TextAnchor alignment2 = GUI.skin.label.alignment;
		bool wordWrap = GUI.skin.label.wordWrap;
		GUI.skin.label.wordWrap = false;
		GUI.skin.label.alignment = 1;
		this.drawBox(this.guiHolder.leftRect);
		if (array == null)
		{
			GUI.skin.label.wordWrap = wordWrap;
			GUI.skin.label.alignment = alignment2;
			return;
		}
		this.OnGUI_drawLeftPane(isShowingMyGames, array);
		GUI.skin.label.alignment = 0;
		GUI.skin.label.fontSize = num;
		GUI.skin.label.wordWrap = true;
		this.guiHolder.labelFontSize = (float)num;
		this.OnGUI_drawRightPane();
		GUI.skin.label.normal.textColor = Color.white;
		GUI.skin.label.alignment = alignment;
		GUI.skin.label.wordWrap = wordWrap;
	}

	// Token: 0x06000C1A RID: 3098 RVA: 0x00009E4B File Offset: 0x0000804B
	private void OnGUI_drawLeftPane(bool isShowingMyGames, CustomGameInfo[] levels)
	{
		this.OnGUI_drawCategoryButtons(this.guiHolder);
		this.OnGUI_drawMatchList(this.guiHolder, isShowingMyGames, levels);
		this.OnGUI_drawSearchField(this.guiHolder);
	}

	// Token: 0x06000C1B RID: 3099 RVA: 0x00055764 File Offset: 0x00053964
	private void OnGUI_drawSearchField(CustomGamesGuiHolder g)
	{
		Rect rect = GeomUtil.cropShare(g.searchInputRect, new Rect(0f, 0f, 0.8f, 1f));
		Rect rect2 = GeomUtil.cropShare(g.searchInputRect, new Rect(0.82f, 0f, 0.18f, 1f));
		this.searchfield.OnGUI_update(rect);
		if (App.GUI.Button(rect2, "Ok"))
		{
			this.refreshGamesList();
		}
		if (this.searchfield.isFocused() && (Input.GetKeyDown(13) || Input.GetKeyDown(271)))
		{
			this.refreshGamesList();
			this.searchfield.takeFocus();
		}
	}

	// Token: 0x06000C1C RID: 3100 RVA: 0x00055820 File Offset: 0x00053A20
	private void OnGUI_drawMatchList(CustomGamesGuiHolder g, bool isShowingMyGames, CustomGameInfo[] levels)
	{
		float num = g.u(75f);
		float num2 = g.u(70f);
		bool flag = this.isSinglePlayer;
		float num3 = (float)levels.Length * num;
		this.gamesScroll = App.GUI.BeginScrollView(g.gamesListRectInner, this.gamesScroll, new Rect(0f, 0f, this.guiHolder.rowWidth(), num3));
		for (int i = 0; i < levels.Length; i++)
		{
			CustomGameInfo customGameInfo = levels[i];
			bool flag2 = customGameInfo == null;
			bool flag3 = customGameInfo != null && customGameInfo.profileId == App.MyProfile.ProfileInfo.id;
			GUI.color = Color.white;
			Rect rect;
			rect..ctor(0f, 0f, g.rowWidth(), num2);
			if (flag2)
			{
				Texture2D texture2D = ResourceManager.LoadTexture("ChatUI/white");
				GUI.color = ColorUtil.FromHex24(15388326u, 0.4f);
				GUI.DrawTexture(GeomUtil.scaleCentered(new Rect(0f, (float)i * num + num * 0.4f, rect.width, 2f), 0.6f), texture2D);
				GUI.color = Color.white;
			}
			else
			{
				App.GUI.BeginGroup(new Rect(0f, (float)i * num, g.gamesListRectInner.width, num2));
				if (isShowingMyGames)
				{
					Rect rect2 = rect;
					rect2.height *= 0.4f;
					rect2.xMin = rect.xMax - rect2.height;
					if (App.GUI.Button(rect2, new GUIContent(), this.emptySkin.button))
					{
						this.DeleteCustomGame(customGameInfo.id.Value);
					}
				}
				if (i == this.selectedCustomGameIndex)
				{
					this.selectedLevel = customGameInfo;
					GUI.Box(rect, string.Empty, this.highlightedButtonStyle);
				}
				else if (GUI.Button(rect, string.Empty))
				{
					this.selectedCustomGameIndex = i;
					this.selectedLevel = customGameInfo;
					CustomGamesImpl.detailsScroll = Vector2.zero;
					this.inputScroll = Vector2.zero;
					this.codeEntry = this.selectedLevel.code;
					if (flag3)
					{
						this.nameEntry = this.selectedLevel.name;
					}
					else
					{
						this.nameEntry = string.Empty;
					}
					this.inEditMode = false;
					this.onLevelClicked(customGameInfo);
				}
				float num4 = g.u(5f);
				if (isShowingMyGames)
				{
					Rect rect3 = rect;
					rect3.height *= 0.4f;
					num4 += rect3.height;
					rect3.xMin = rect.xMax - rect3.height;
					App.GUI.Button(rect3, new GUIContent(), this.styleCloseButton);
				}
				if (customGameInfo.isPuzzle)
				{
					Rect rect4 = rect;
					float num5 = rect4.height * 0.6f;
					num4 += num5 * 0.9f;
					float num6 = num5;
					rect4.width = num6;
					rect4.height = num6;
					rect4.x = rect.xMax - num4;
					GUI.DrawTexture(rect4, ResourceManager.LoadTexture("Icons/puzzle_icon"));
				}
				if (customGameInfo.isCampaign)
				{
					Rect rect5 = rect;
					float num7 = rect5.height * 0.6f;
					num4 += num7 * 0.9f;
					float num6 = num7;
					rect5.width = num6;
					rect5.height = num6;
					rect5.x = rect.xMax - num4;
					GUI.DrawTexture(rect5, ResourceManager.LoadTexture("Icons/chain_icon"));
				}
				GUI.skin.label.alignment = 0;
				GUI.skin.label.fontSize = (int)Math.Round((double)(g.labelFontSize * 1.4f));
				GUI.skin.label.normal.textColor = this.textColor;
				Rect rect6 = rect;
				if (flag)
				{
					Texture2D texture2D2 = ResourceManager.LoadTexture((!customGameInfo.isCompleted) ? "Arena/scroll_browser_button_cb2" : "Arena/scroll_browser_button_cb_checked2");
					Rect rect7;
					rect7..ctor(rect6);
					float num6 = rect7.height * 0.7f;
					rect7.height = num6;
					rect7.width = num6;
					GUI.DrawTexture(rect7, texture2D2);
				}
				rect6 = GeomUtil.scaleCentered(rect6, 0.9f, 0.85f);
				if (flag)
				{
					rect6.xMin += (float)Screen.height * 0.025f;
				}
				GUI.Label(rect6, customGameInfo.name);
				GUI.skin.label.normal.textColor = ColorUtil.FromHex24(8746597u);
				GUI.skin.label.fontSize = (int)Math.Round((double)(g.labelFontSize * 0.95f));
				GUI.skin.label.alignment = 6;
				GUI.Label(rect6, "Created by " + customGameInfo.profileName);
				GUI.skin.label.normal.textColor = Color.white;
				if (customGameInfo.isRated())
				{
					Rect rect8 = GeomUtil.cropShare(rect, new Rect(0.985f, 0.6f, 0f, 0.32f));
					float height = rect8.height;
					float num8 = height * 0.85f;
					rect8.x = rect8.xMax - 5f * num8;
					rect8.width = height;
					float num9 = customGameInfo.rating();
					for (int j = 0; j < 5; j++)
					{
						float num10 = num9 - (float)j;
						string filename = (num10 >= 0.5f) ? ((num10 >= 1f) ? "Icons/RatingStars/star_1" : "Icons/RatingStars/star_2") : "Icons/RatingStars/star_0";
						GUI.DrawTexture(rect8, ResourceManager.LoadTexture(filename));
						rect8.x += num8;
					}
				}
				App.GUI.EndGroup();
			}
		}
		App.GUI.EndScrollView();
	}

	// Token: 0x06000C1D RID: 3101 RVA: 0x00009E73 File Offset: 0x00008073
	private void customGamesListChanged(int listIndex)
	{
		if (this.selectedCustomGameTypeIndex == listIndex)
		{
			return;
		}
		this.selectedCustomGameTypeIndex = listIndex;
		if (listIndex != 2)
		{
			this.nameEntry = string.Empty;
		}
		this.selectedCustomGameIndex = -1;
	}

	// Token: 0x06000C1E RID: 3102 RVA: 0x00055E24 File Offset: 0x00054024
	private void OnGUI_drawCategoryButtons(CustomGamesGuiHolder g)
	{
		float num = g.rowWidth() / 3f;
		float num2 = g.gamesListRectInner.y - g.buttonHeight * 1.1f;
		GUI.Box(new Rect(g.gamesListRectInner.x + (float)this.selectedCustomGameTypeIndex * num, num2, num, g.buttonHeight), string.Empty, this.highlightedButtonStyle);
		if (App.GUI.Button(new Rect(g.gamesListRectInner.x + 0f * num, num2, num, g.buttonHeight), "Recent"))
		{
			this.customGamesListChanged(0);
		}
		if (App.GUI.Button(new Rect(g.gamesListRectInner.x + 1f * num, num2, num, g.buttonHeight), "Popular"))
		{
			this.customGamesListChanged(1);
		}
		if (App.GUI.Button(new Rect(g.gamesListRectInner.x + 2f * num, num2, num, g.buttonHeight), "Mine"))
		{
			this.customGamesListChanged(2);
		}
	}

	// Token: 0x06000C1F RID: 3103 RVA: 0x00009EA2 File Offset: 0x000080A2
	private void OnGUI_drawRightPane()
	{
		this.OnGUI_drawCompileAndPlayButtons(this.guiHolder, true);
		this.OnGUI_drawInputField(this.guiHolder);
		this.OnGUI_drawGameInfo(this.guiHolder.details, this.descData);
		this.OnGUI_drawCompileAndPlayButtons(this.guiHolder, false);
	}

	// Token: 0x06000C20 RID: 3104 RVA: 0x00055F3C File Offset: 0x0005413C
	private void OnGUI_drawInputField(CustomGamesGuiHolder g)
	{
		if (!this.inEditMode || this.showDecks)
		{
			return;
		}
		GUI.skin.label.fontSize = (int)Math.Round((double)(g.labelFontSize * 1.25f));
		GUI.skin.label.normal.textColor = this.textColor;
		GUI.Label(GeomUtil.cropShare(g.inputNameRect, new Rect(0.05f, 0f, 0.15f, 1f)), "Name");
		GUI.enabled = this.inEditMode;
		this.nameEntry = GUI.TextField(GeomUtil.cropShare(g.inputNameRect, new Rect(0.2f, 0f, 0.79f, 1f)), this.nameEntry);
		GUI.enabled = true;
		GUILayout.BeginArea(g.inputRect);
		Color color = GUI.skin.textArea.normal.textColor;
		Color color2 = (!this.inEditMode) ? ColorUtil.Darken(color, 0.2f) : color;
		GUIStyleState normal = GUI.skin.textArea.normal;
		Color color3 = color2;
		GUI.skin.textArea.onHover.textColor = color3;
		color3 = color3;
		GUI.skin.textArea.onActive.textColor = color3;
		normal.textColor = color3;
		this.inputScroll = GUILayout.BeginScrollView(this.inputScroll, new GUILayoutOption[]
		{
			GUILayout.Width(g.inputRect.width - g.u(24f)),
			GUILayout.Height(g.inputRect.height)
		});
		if (this.inEditMode)
		{
			this.codeEntry = GUILayout.TextArea(this.codeEntry, new GUILayoutOption[]
			{
				GUILayout.Height(g.inputRect.height * 0.9f),
				GUILayout.ExpandHeight(true)
			});
		}
		else
		{
			GUILayout.Label(this.codeEntry, GUI.skin.textArea, new GUILayoutOption[]
			{
				GUILayout.MinHeight(g.inputRect.height * 0.9f),
				GUILayout.ExpandHeight(true)
			});
		}
		GUIStyleState normal2 = GUI.skin.textArea.normal;
		color3 = color;
		GUI.skin.textArea.onHover.textColor = color3;
		color3 = color3;
		GUI.skin.textArea.onActive.textColor = color3;
		normal2.textColor = color3;
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	// Token: 0x06000C21 RID: 3105 RVA: 0x000561AC File Offset: 0x000543AC
	private void OnGUI_drawCompileAndPlayButtons(CustomGamesGuiHolder g, bool allowClicks)
	{
		if (this.showDecks)
		{
			return;
		}
		float num = g.leftRect.width * 0.45f;
		float num2 = g.u(55f);
		float num3 = g.fullRect.yMax - 1.1f * num2;
		float num4 = g.detailsRect.x + (g.detailsRect.width - num) * 0.5f;
		if (this.inEditMode || this.selectedLevel.id != null)
		{
			GUIContent c;
			if (this.inEditMode)
			{
				c = new GUIContent("Save and Play");
			}
			else
			{
				c = new GUIContent("Play");
			}
			if (App.GUI.Button(new Rect(num4 + ((!this.inEditMode) ? 0f : (num * 0.9f)), num3, num, num2), c) && allowClicks)
			{
				if (this.inEditMode)
				{
					this.SaveCustomGame(this.isSinglePlayer, this.nameEntry.Trim(), this.codeEntry, CustomGamesImpl.SaveAction.SavePlay);
				}
				else
				{
					App.GameActionManager.CustomGameChosen(this.selectedLevel.id.Value, this.selectedLevel.chooseDeckP1, this.selectedLevel.chooseDifficulty);
				}
			}
		}
		if (this.inEditMode && App.GUI.Button(new Rect(num4 - num * 0.9f, num3, 0.6f * num, num2), "Verify") && allowClicks)
		{
			this.SaveCustomGame(this.isSinglePlayer, this.nameEntry.Trim(), this.codeEntry, CustomGamesImpl.SaveAction.Compile);
		}
		if (this.inEditMode && App.GUI.Button(new Rect(num4 - num * 0.2f, num3, num, num2), "Save") && allowClicks)
		{
			this.SaveCustomGame(this.isSinglePlayer, this.nameEntry.Trim(), this.codeEntry, CustomGamesImpl.SaveAction.Save);
		}
		if (!this.inEditMode && App.GUI.Button(new Rect(num4, g.detailsRect.yMax - num2 - 15f, num, num2), (this.selectedLevel.id == null) ? "Create new" : "Edit") && allowClicks)
		{
			this.selectedCustomGameIndex = -1;
			this.inEditMode = true;
			this.descData.clear();
			this.setupPositions();
		}
	}

	// Token: 0x06000C22 RID: 3106 RVA: 0x00009EE1 File Offset: 0x000080E1
	public void OnGUI_drawGameInfo(CustomGamesDetailsHolder r, CustomGamesDescriptionData d)
	{
		if (this.showDecks)
		{
			this.OnGUI_drawGameDecks(r, d);
		}
		else
		{
			this.OnGUI_drawGameDetails(r, d);
		}
	}

	// Token: 0x06000C23 RID: 3107 RVA: 0x00056428 File Offset: 0x00054628
	private void OnGUI_drawGameDecks(CustomGamesDetailsHolder r, CustomGamesDescriptionData d)
	{
		float num = 0.001f * (float)Screen.height;
		float num2 = 55f * num;
		this.leftDeckScrollPosition = this.OnGUI_drawDeckList(r.deck.frameRectP1, r.deck.rectP1, d.deckP1, this.leftDeckScrollPosition, true);
		this.rightDeckScrollPosition = this.OnGUI_drawDeckList(r.deck.frameRectP2, r.deck.rectP2, d.deckP2, this.rightDeckScrollPosition, false);
		Rect centered = GeomUtil.getCentered(new Rect(0f, r.deck.frameRectP1.yMax + 10f * num, 200f * num, num2), r.frameRect, true, false);
		if (GUI.Button(centered, "Hide decks"))
		{
			this.showDecks = false;
		}
	}

	// Token: 0x06000C24 RID: 3108 RVA: 0x000564F8 File Offset: 0x000546F8
	private Vector2 OnGUI_drawDeckList(Rect frameRect, Rect rect, SizedString s, Vector2 scrollPosition, bool isYou)
	{
		float num = 0.001f * (float)Screen.height;
		this.drawBox(frameRect);
		if (string.IsNullOrEmpty(s.text))
		{
			return Vector2.zero;
		}
		Rect rect2;
		rect2..ctor(0f, 50f * num, s.width, s.height);
		Rect rect3 = rect2;
		rect3.yMin = 0f;
		Vector2 result = GUI.BeginScrollView(frameRect, scrollPosition, rect3);
		GUI.Label(new Rect(10f * num, 0f, s.width, rect2.y), (!isYou) ? "Your opponent's deck" : "Your deck", this.styleDeckTitle);
		CustomGamesImpl.label(rect2, s);
		GUI.EndScrollView();
		return result;
	}

	// Token: 0x06000C25 RID: 3109 RVA: 0x00009F03 File Offset: 0x00008103
	private static void label(Rect rect, SizedString s)
	{
		GUI.Label(rect, s.text, s.style);
	}

	// Token: 0x06000C26 RID: 3110 RVA: 0x000565B4 File Offset: 0x000547B4
	private void OnGUI_drawGameDetails(CustomGamesDetailsHolder r, CustomGamesDescriptionData d)
	{
		float num = 0.001f * (float)Screen.height;
		this.drawBox(r.frameRect);
		Rect rect = GeomUtil.scaleCentered(r.frameRect, 1f, 0.98f);
		CustomGamesImpl.detailsScroll = GUI.BeginScrollView(rect, CustomGamesImpl.detailsScroll, r.contentRect());
		this.drawBox(GeomUtil.inflate(r.descRect, 15f * num, 5f * num));
		if (d.isEmpty())
		{
			GUI.EndScrollView();
			return;
		}
		CustomGamesImpl.label(r.titleRect, d.name);
		if (!string.IsNullOrEmpty(d.flavor.text))
		{
			CustomGamesImpl.label(r.flavorRect, d.flavor);
		}
		CustomGamesImpl.label(r.descRect, d.desc);
		if (!string.IsNullOrEmpty(d.game().bet))
		{
			this.drawBox(r.betRect);
			Rect rect2 = GeomUtil.inflate(r.betRect, 0f, -5f * num);
			Rect rect3 = rect2;
			float num2 = rect3.height * 0.8f;
			rect3.height = num2;
			rect3.width = num2;
			rect3.x += 3f * num;
			rect3.y += 3f * num;
			GUI.DrawTexture(rect3, ResourceManager.LoadTexture("Shared/gold_icon_small"));
			GUI.Label(rect2, d.bet, this.styleBet);
		}
		if ((!string.IsNullOrEmpty(d.game().deckP1) || !string.IsNullOrEmpty(d.game().deckP2)) && GUI.Button(r.deckButtonRect, "Show decks", this.styleShowDeck))
		{
			this.showDecks = true;
		}
		GUI.EndScrollView();
	}

	// Token: 0x06000C27 RID: 3111 RVA: 0x00056778 File Offset: 0x00054978
	private void refreshGamesList()
	{
		string search = this.searchfield.text().Trim();
		App.GameActionManager.RefreshCustomGamesList(this.isSinglePlayer, search);
	}

	// Token: 0x06000C28 RID: 3112 RVA: 0x000028DF File Offset: 0x00000ADF
	public void onConnect(OnConnectData data)
	{
	}

	// Token: 0x06000C29 RID: 3113 RVA: 0x000567A8 File Offset: 0x000549A8
	public void handleMessage(Message message)
	{
		if (message is OkMessage)
		{
			OkMessage okMessage = (OkMessage)message;
			if (okMessage.isType(typeof(DeleteCustomGameMessage)))
			{
				this.refreshGamesList();
			}
		}
		if (message is FailMessage)
		{
			FailMessage failMessage = (FailMessage)message;
			if (failMessage.isType(typeof(PlaySinglePlayerCustomQuickmatchMessage)))
			{
				App.Popups.ShowOk(new OkVoidCallback(), "custom_game_error", "Could not start custom game", failMessage.info, "Ok");
			}
		}
		if (message is SaveCustomGameMessage)
		{
			SaveCustomGameMessage saveCustomGameMessage = (SaveCustomGameMessage)message;
			if (saveCustomGameMessage.errors.Count == 0)
			{
				CustomGameInfo customGame = saveCustomGameMessage.customGame;
				if (saveCustomGameMessage.compileOnly)
				{
					customGame.name = "Verification successful";
				}
				else
				{
					this.refreshGamesList();
					if (this.doSaveAndPlay)
					{
						App.GameActionManager.CustomGameChosen(saveCustomGameMessage.customGame.id.Value, saveCustomGameMessage.customGame.chooseDeckP1, saveCustomGameMessage.customGame.chooseDifficulty);
					}
				}
				this.onLevelClicked(customGame);
			}
			else
			{
				string descriptionP = string.Join("\n", Enumerable.ToArray<string>(Enumerable.Select<ErrorMsg, string>(saveCustomGameMessage.errors, (ErrorMsg x) => x.msg)));
				saveCustomGameMessage.customGame.name = "Verification failed";
				saveCustomGameMessage.customGame.descriptionP1 = descriptionP;
				this.onLevelClicked(saveCustomGameMessage.customGame);
			}
		}
	}

	// Token: 0x06000C2A RID: 3114 RVA: 0x00056920 File Offset: 0x00054B20
	private void SaveCustomGame(bool isSinglePlayer, string name, string setupCode, CustomGamesImpl.SaveAction saveAction)
	{
		bool compileOnly = saveAction == CustomGamesImpl.SaveAction.Compile;
		this.doSaveAndPlay = (saveAction == CustomGamesImpl.SaveAction.SavePlay);
		App.Communicator.send(new SaveCustomGameMessage(isSinglePlayer, name, setupCode, compileOnly));
	}

	// Token: 0x06000C2B RID: 3115 RVA: 0x00009F17 File Offset: 0x00008117
	public void DeleteCustomGame(int customGameId)
	{
		this._deleteCustomGameId = customGameId;
		App.Popups.ShowOkCancel(this, "delete_ruleset", "Delete custom rules", "Really delete these custom rules?", "Delete", "Cancel");
	}

	// Token: 0x06000C2C RID: 3116 RVA: 0x00009F44 File Offset: 0x00008144
	public void PopupOk(string popupType)
	{
		if (popupType == "delete_ruleset")
		{
			App.Communicator.send(new DeleteCustomGameMessage(this._deleteCustomGameId));
		}
	}

	// Token: 0x06000C2D RID: 3117 RVA: 0x000028DF File Offset: 0x00000ADF
	public void PopupCancel(string popupType)
	{
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x00056954 File Offset: 0x00054B54
	private void onLevelClicked(CustomGameInfo level)
	{
		this.selectedLevel = level;
		this.showDecks = false;
		CustomGamesDeckHolder customGamesDeckHolder = CustomGamesDeckHolder.defaultFromRect(this.guiHolder.rightRect);
		Log.warning("Setting level");
		this.descData.update(level, this.guiHolder.detailsRect.width, customGamesDeckHolder.rectP1.width, this.guiHolder.u(1f), this.styleTitle, this.styleFlavour, this.youArePlayerP2);
		this.setupPositions();
		this.guiHolder.details = CustomGamesDetailsHolder.defaultFromRect(this.guiHolder.detailsRect, this.guiHolder.u(1f), this.descData);
		this.guiHolder.details.deck = customGamesDeckHolder;
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x00056A1C File Offset: 0x00054C1C
	private Action createRenderFunction(CustomGameInfo g, Rect r)
	{
		this.setupFontSizes();
		float u = 0.001f * (float)Screen.height;
		CustomGamesDeckHolder customGamesDeckHolder = CustomGamesDeckHolder.defaultFromRect(r);
		CustomGamesDescriptionData descData = new CustomGamesDescriptionData();
		descData.update(g, r.width, customGamesDeckHolder.rectP1.width, u, this.styleTitle, this.styleFlavour, this.youArePlayerP2);
		CustomGamesDetailsHolder detailsHolder = CustomGamesDetailsHolder.defaultFromRect(r, u, descData);
		detailsHolder.deck = customGamesDeckHolder;
		return delegate()
		{
			this.OnGUI_drawGameInfo(detailsHolder, descData);
		};
	}

	// Token: 0x06000C30 RID: 3120 RVA: 0x00009F6C File Offset: 0x0000816C
	public static void ShowGameDetailsPopup(CustomGameInfo g)
	{
		CustomGamesImpl.ShowGameDetailsPopup(new OkCancelCallback(null, null), "custom_game_challenge", g);
	}

	// Token: 0x06000C31 RID: 3121 RVA: 0x00009F80 File Offset: 0x00008180
	public static void ShowGameDetailsPopup(IOkCancelCallback callback, string popupId, CustomGameInfo g)
	{
		CustomGamesImpl.ShowGameDetailsPopup(callback, popupId, g, true);
	}

	// Token: 0x06000C32 RID: 3122 RVA: 0x00056AB8 File Offset: 0x00054CB8
	public static void ShowGameDetailsPopup(IOkCancelCallback callback, string popupId, CustomGameInfo g, bool isPlayerTwo)
	{
		CustomGamesImpl customGamesImpl = new CustomGamesImpl();
		customGamesImpl.youArePlayerP2 = isPlayerTwo;
		Rect rect = GeomUtil.scaleCentered(GUIUtil.screen(), 0.5f, 0.5f);
		Action renderer = customGamesImpl.createRenderFunction(g, rect);
		App.Popups.ShowOkCancelRenderer(callback, popupId, (!isPlayerTwo) ? "Continue" : "Challenge", rect, renderer, "Ok", "Cancel");
	}

	// Token: 0x04000945 RID: 2373
	private GUISkin regularUISkin;

	// Token: 0x04000946 RID: 2374
	private GUISkin emptySkin;

	// Token: 0x04000947 RID: 2375
	private GUIStyle styleFlavour;

	// Token: 0x04000948 RID: 2376
	private GUIStyle styleTitle;

	// Token: 0x04000949 RID: 2377
	private GUIStyle styleDeckTitle;

	// Token: 0x0400094A RID: 2378
	private GUIStyle styleBet;

	// Token: 0x0400094B RID: 2379
	private GUIStyle styleShowDeck;

	// Token: 0x0400094C RID: 2380
	private GUIStyle styleCloseButton;

	// Token: 0x0400094D RID: 2381
	private GUIStyle highlightedButtonStyle;

	// Token: 0x0400094E RID: 2382
	private IOkCallback okCallback;

	// Token: 0x0400094F RID: 2383
	private ICancelCallback cancelCallback;

	// Token: 0x04000950 RID: 2384
	private GUIContent okText;

	// Token: 0x04000951 RID: 2385
	private string codeEntry = string.Empty;

	// Token: 0x04000952 RID: 2386
	private string nameEntry = string.Empty;

	// Token: 0x04000953 RID: 2387
	private string challengee;

	// Token: 0x04000954 RID: 2388
	private Vector2 gamesScroll = Vector2.zero;

	// Token: 0x04000955 RID: 2389
	private static Vector2 detailsScroll = Vector2.zero;

	// Token: 0x04000956 RID: 2390
	private Vector2 inputScroll = Vector2.zero;

	// Token: 0x04000957 RID: 2391
	private bool inEditMode;

	// Token: 0x04000958 RID: 2392
	private bool isSinglePlayer;

	// Token: 0x04000959 RID: 2393
	public bool youArePlayerP2;

	// Token: 0x0400095A RID: 2394
	private TextField searchfield;

	// Token: 0x0400095B RID: 2395
	private readonly Color textColor = new Color(1f, 0.95f, 0.85f);

	// Token: 0x0400095C RID: 2396
	private CustomGamesImpl.PopupType currentPopupType;

	// Token: 0x0400095D RID: 2397
	private CustomGamesGuiHolder guiHolder = new CustomGamesGuiHolder();

	// Token: 0x0400095E RID: 2398
	private CustomGamesDescriptionData descData = new CustomGamesDescriptionData();

	// Token: 0x0400095F RID: 2399
	private IsValueChanged resolution;

	// Token: 0x04000960 RID: 2400
	private int selectedCustomGameIndex;

	// Token: 0x04000961 RID: 2401
	private int selectedCustomGameTypeIndex;

	// Token: 0x04000962 RID: 2402
	private CustomGameInfo selectedLevel = new CustomGameInfo();

	// Token: 0x04000963 RID: 2403
	public bool showDecks;

	// Token: 0x04000964 RID: 2404
	private Vector2 leftDeckScrollPosition;

	// Token: 0x04000965 RID: 2405
	private Vector2 rightDeckScrollPosition;

	// Token: 0x04000966 RID: 2406
	private bool doSaveAndPlay;

	// Token: 0x04000967 RID: 2407
	private int _deleteCustomGameId;

	// Token: 0x02000186 RID: 390
	private enum PopupType
	{
		// Token: 0x0400096A RID: 2410
		NONE,
		// Token: 0x0400096B RID: 2411
		CUSTOM_GAME_MULTIPLAYER_SELECTOR,
		// Token: 0x0400096C RID: 2412
		CUSTOM_GAME_SINGLEPLAYER_SELECTOR
	}

	// Token: 0x02000187 RID: 391
	private enum SaveAction
	{
		// Token: 0x0400096E RID: 2414
		Compile,
		// Token: 0x0400096F RID: 2415
		Save,
		// Token: 0x04000970 RID: 2416
		SavePlay
	}
}
