using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000462 RID: 1122
public class Watch : AbstractCommListener, ICancelCallback, IOkStringCallback, IOkStringCancelCallback
{
	// Token: 0x0600190E RID: 6414 RVA: 0x00093F70 File Offset: 0x00092170
	private void Start()
	{
		this.resolution = ChangeDetectors.resolution();
		this.regularUI = (GUISkin)ResourceManager.Load("_GUISkins/RegularUI");
		this.settingsSkin = (GUISkin)ResourceManager.Load("_GUISkins/Settings");
		this.settingsUILabel = this.settingsSkin.label;
		this.modeButtonStyle = new GUIStyle(this.regularUI.button);
		this.midLabelStyle = new GUIStyle(this.regularUI.label);
		this.highlightedButtonStyle = new GUIStyle(this.regularUI.button);
		this.highlightedButtonStyle.normal.background = this.highlightedButtonStyle.hover.background;
		this.sorter.byRating().byStarted();
		this.mode = Watch.Mode.Replay;
		this.setMode(Watch.Mode.Spectate);
		App.Communicator.addListener(this);
		Application.targetFrameRate = 60;
		App.ChatUI.Show(false);
		App.Communicator.send(new SpectateListGamesMessage());
		App.LobbyMenu.fadeInScene();
		this.stringToGameType.Add("All", GameType.None);
		this.stringToGameType.Add("Ranked", GameType.MP_RANKED);
		this.stringToGameType.Add("Multiplayer quickmatch", GameType.MP_QUICKMATCH);
		this.stringToGameType.Add("Judgement", GameType.MP_LIMITED);
		this.stringToGameType.Add("Challenge", GameType.MP_UNRANKED);
		this.stringToGameType.Add("Skirmish", GameType.SP_QUICKMATCH);
		this.stringToGameType.Add("Trial", GameType.SP_TOWERMATCH);
		this.gametypeDrop = new GameObject("Dropdown").AddComponent<Dropdown>();
		this.gametypeDrop.Init(Enumerable.ToArray<string>(this.stringToGameType.Keys), 4f, true, false, 15);
		this.gametypeDrop.SetEnabled(true);
		this.gametypeDrop.SetSkin(this.regularUI);
		this.gametypeDrop.DropdownChangedEvent += this.DropdownChangedEvent;
	}

	// Token: 0x0600190F RID: 6415 RVA: 0x000123E7 File Offset: 0x000105E7
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.gametypeDrop.DropdownChangedEvent -= this.DropdownChangedEvent;
	}

	// Token: 0x06001910 RID: 6416 RVA: 0x00012406 File Offset: 0x00010606
	private bool isReplay()
	{
		return this.mode == Watch.Mode.Replay;
	}

	// Token: 0x06001911 RID: 6417 RVA: 0x00012411 File Offset: 0x00010611
	private bool isSpectate()
	{
		return this.mode == Watch.Mode.Spectate;
	}

	// Token: 0x06001912 RID: 6418 RVA: 0x00094160 File Offset: 0x00092360
	public void DropdownChangedEvent(int selectedIndex, string selection)
	{
		GameType gameType = this.stringToGameType[selection];
		if (gameType == this.currentGameType)
		{
			return;
		}
		this.currentGameType = gameType;
		this.FilterMatches();
		if (this.isReplay())
		{
			this.requestReplayGames();
		}
	}

	// Token: 0x06001913 RID: 6419 RVA: 0x0001241C File Offset: 0x0001061C
	private void FilterMatches()
	{
		if (this.isReplay())
		{
			this.FilterReplayMatches();
		}
		else
		{
			this.FilterSpectateMatches();
		}
	}

	// Token: 0x06001914 RID: 6420 RVA: 0x000941A8 File Offset: 0x000923A8
	private void FilterReplayMatches()
	{
		if (this.currentGameType == GameType.None)
		{
			this.filteredReplayMatches = new List<SpectatableGameInfo>(this.replayMatches);
		}
		else
		{
			this.filteredReplayMatches = this.replayMatches.FindAll((SpectatableGameInfo x) => x.gameType == this.currentGameType);
		}
		this.filteredReplayMatches.Sort(this.sorter);
	}

	// Token: 0x06001915 RID: 6421 RVA: 0x00094204 File Offset: 0x00092404
	private void FilterSpectateMatches()
	{
		if (this.currentGameType == GameType.None)
		{
			this.filteredSpectateMatches = new List<SpectatableGameInfo>(this.spectateMatches);
		}
		else
		{
			this.filteredSpectateMatches = this.spectateMatches.FindAll((SpectatableGameInfo x) => x.gameType == this.currentGameType);
		}
		if (!App.Config.settings.spectate_show_unstarted)
		{
			this.filteredSpectateMatches = Enumerable.ToList<SpectatableGameInfo>(Enumerable.Where<SpectatableGameInfo>(this.filteredSpectateMatches, (SpectatableGameInfo x) => x.started.Value));
		}
		this.filteredSpectateMatches.Sort(this.sorter);
	}

	// Token: 0x06001916 RID: 6422 RVA: 0x000942AC File Offset: 0x000924AC
	private void setupPositions()
	{
		float num = 0.001f * (float)Screen.height;
		this.setupFontSizes();
		this.scrollArea = new Rect((float)(Screen.width / 2) - (float)Screen.height * 0.5f, (float)Screen.height * 0.18f, (float)Screen.height * 0.7f, (float)Screen.height * 0.74f);
		this.rightArea = new Rect(this.scrollArea.xMax + (float)Screen.height * 0.07f, this.scrollArea.y, (float)Screen.height * 0.23f, this.scrollArea.height);
		this.sortRect = this.rightArea;
		this.sortRect.y = this.sortRect.y + 100f * num;
		Rect rect;
		rect..ctor(this.rightArea.x, this.sortRect.y + (float)Screen.height * 0.3f, this.rightArea.width, (float)Screen.height * 0.04f);
		this.gametypeDrop.SetRect(rect);
		float num2 = 30f * num;
		Rect rect2;
		rect2..ctor(this.scrollArea.x - num2, this.scrollArea.y - num2, this.scrollArea.width + num2 * 2f, this.scrollArea.height + num2 * 2f);
		this.leftFrame = new ScrollsFrame(rect2).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.CENTER);
		Rect rect3;
		rect3..ctor(this.rightArea.x - num2, this.rightArea.y - num2, this.rightArea.width + num2 * 2f, this.rightArea.height + num2 * 2f);
		this.rightFrame = new ScrollsFrame(rect3).AddNinePatch(ScrollsFrame.Border.DARK_SHARP, NinePatch.Patches.CENTER);
		this.rightInnerRect = GeomUtil.inflate(rect3, -13f * num);
		Rect full = this.rightInnerRect;
		this.modeSpectateRect = GeomUtil.cropShare(full, new Rect(0f, 0f, 0.5f, 0.04f));
		this.modeReplayRect = GeomUtil.cropShare(full, new Rect(0.5f, 0f, 0.5f, 0.04f));
		this.gameTypeRect = new Rect(this.rightArea.x, this.sortRect.y + (float)Screen.height * 0.25f, this.rightArea.width, (float)Screen.height * 0.05f);
		this.rightTitleRect = this.gameTypeRect;
		this.rightTitleRect.y = this.rightArea.y + 40f * num;
		this.findReplayRects = new Rect[5];
		for (int i = 0; i < this.findReplayRects.Length; i++)
		{
			this.findReplayRects[i] = this.gameTypeRect;
			this.findReplayRects[i].y = rect3.yMax - ((float)(4 - i) * 1.1f + 1.5f) * this.gameTypeRect.height;
		}
	}

	// Token: 0x06001917 RID: 6423 RVA: 0x000945D8 File Offset: 0x000927D8
	private void setupFontSizes()
	{
		float num = 0.001f * (float)Screen.height;
		this.modeButtonStyle.fontSize = (int)(24f * num);
		this.midLabelStyle.fontSize = (int)(24f * num);
	}

	// Token: 0x06001918 RID: 6424 RVA: 0x0001243A File Offset: 0x0001063A
	private void updateCheckResolutionChanged()
	{
		if (this.resolution.IsChanged())
		{
			this.setupPositions();
		}
	}

	// Token: 0x06001919 RID: 6425 RVA: 0x00094618 File Offset: 0x00092818
	private void OnGUI()
	{
		GUI.depth = 21;
		GUI.skin = this.regularUI;
		this.updateCheckResolutionChanged();
		int fontSize = GUI.skin.label.fontSize;
		int fontSize2 = GUI.skin.button.fontSize;
		TextAnchor alignment = GUI.skin.label.alignment;
		GUI.DrawTexture(GUIUtil.screen(), ResourceManager.LoadTexture("DeckBuilder/bg"));
		this.OnGUI_drawLeft();
		this.OnGUI_drawRight();
		GUI.color = new Color(1f, 1f, 1f, this.fadeIn);
		GUI.DrawTexture(GUIUtil.screen(), ResourceManager.LoadTexture("Shared/blackFiller"));
		GUI.color = new Color(1f, 1f, 1f, 1f);
		GUI.skin.label.alignment = alignment;
		GUI.skin.button.fontSize = fontSize2;
		GUI.skin.label.fontSize = fontSize;
	}

	// Token: 0x0600191A RID: 6426 RVA: 0x00012452 File Offset: 0x00010652
	private void OnGUI_drawLeft()
	{
		this.leftFrame.Draw();
		this.OnGUI_drawGamesList((!this.isReplay()) ? this.filteredSpectateMatches : this.filteredReplayMatches);
	}

	// Token: 0x0600191B RID: 6427 RVA: 0x00012481 File Offset: 0x00010681
	private void OnGUI_drawRight()
	{
		this.rightFrame.Draw();
		if (this.isReplay())
		{
			GUI.Label(this.rightTitleRect, "Showing replays for " + this.replayName, this.midLabelStyle);
		}
		this.OnGUI_drawButtons();
	}

	// Token: 0x0600191C RID: 6428 RVA: 0x00094710 File Offset: 0x00092910
	private void OnGUI_drawButtons()
	{
		GUI.skin.label.alignment = 4;
		GUIStyle label = GUI.skin.label;
		int num = Screen.height / 36;
		this.highlightedButtonStyle.fontSize = num;
		num = num;
		GUI.skin.button.fontSize = num;
		label.fontSize = num;
		GUI.Label(this.gameTypeRect, "Game type");
		GUI.Box((!this.isReplay()) ? this.modeSpectateRect : this.modeReplayRect, string.Empty, this.highlightedButtonStyle);
		if (App.GUI.Button(this.modeReplayRect, "Replays", this.modeButtonStyle))
		{
			this.setMode(Watch.Mode.Replay);
		}
		if (App.GUI.Button(this.modeSpectateRect, "Spectate", this.modeButtonStyle))
		{
			this.setMode(Watch.Mode.Spectate);
		}
		this.OnGUI_drawSortButtons();
		if (this.isSpectate())
		{
			this.OnGUI_drawShowGamesEarly();
			if (GUI.Button(new Rect(this.rightArea.x, this.rightArea.yMax - (float)Screen.height * 0.04f, this.rightArea.width, (float)Screen.height * 0.04f), "Refresh list"))
			{
				App.Communicator.send(new SpectateListGamesMessage());
			}
		}
		if (this.isReplay())
		{
			GUI.Label(this.findReplayRects[0], "Find replays:");
			if (App.GUI.Button(this.findReplayRects[1], "Mine"))
			{
				this.requestReplayGamesFor(App.MyProfile.ProfileInfo.name);
			}
			if (App.GUI.Button(this.findReplayRects[2], "Everyone"))
			{
				this.requestReplayGamesFor(null);
			}
			if (App.GUI.Button(this.findReplayRects[3], "Choose player..."))
			{
				App.Popups.ShowTextEntry(this, "choose_player", "Find replays", "Enter player name", "Ok", "Cancel");
			}
		}
	}

	// Token: 0x0600191D RID: 6429 RVA: 0x000124C0 File Offset: 0x000106C0
	private List<SpectatableGameInfo> filteredMatches()
	{
		return (!this.isReplay()) ? this.filteredSpectateMatches : this.filteredReplayMatches;
	}

	// Token: 0x0600191E RID: 6430 RVA: 0x00094938 File Offset: 0x00092B38
	private void OnGUI_drawSortButtons()
	{
		float num = this.sortRect.y + (float)Screen.height * 0.02f;
		float num2 = (float)Screen.height * 0.05f;
		GUI.Label(new Rect(this.sortRect.x, num, this.sortRect.width, (float)Screen.height * 0.04f), "Sort by");
		if (App.GUI.Button(new Rect(this.rightArea.x, num + 1f * num2, this.rightArea.width, (float)Screen.height * 0.04f), "Recent", (this.sortingIndex != 0) ? GUI.skin.button : this.highlightedButtonStyle))
		{
			this.sorter.clear();
			this.sorter.byTimestamp();
			this.filteredMatches().Sort(this.sorter);
			this.sortingIndex = 0;
		}
		if (this.isSpectate() && App.GUI.Button(new Rect(this.rightArea.x, num + 2f * num2, this.rightArea.width, (float)Screen.height * 0.04f), "Most popular", (this.sortingIndex != 1) ? GUI.skin.button : this.highlightedButtonStyle))
		{
			this.sorter.clear();
			this.sorter.byPopularity();
			this.filteredMatches().Sort(this.sorter);
			this.sortingIndex = 1;
		}
		if (App.GUI.Button(new Rect(this.rightArea.x, num + (float)((!this.isSpectate()) ? 2 : 3) * num2, this.rightArea.width, (float)Screen.height * 0.04f), "Rating", (this.sortingIndex != 2) ? GUI.skin.button : this.highlightedButtonStyle))
		{
			this.sorter.clear();
			this.sorter.byRating();
			this.filteredMatches().Sort(this.sorter);
			this.sortingIndex = 2;
		}
	}

	// Token: 0x0600191F RID: 6431 RVA: 0x00094B74 File Offset: 0x00092D74
	private void OnGUI_drawShowGamesEarly()
	{
		Rect rect;
		rect..ctor(this.rightArea.x, this.sortRect.y + (float)Screen.height * 0.35f, this.rightArea.width, (float)Screen.height * 0.04f);
		Rect rect2 = rect;
		rect2.width = (float)Screen.height * 0.04f;
		rect2.height = (float)Screen.height * 0.04f + 2f;
		Rect rect3 = rect;
		rect3.xMin = rect2.xMax;
		this.settingsUILabel.fontSize = Screen.height / 36;
		GUI.Label(rect3, "Show games early", GUI.skin.label);
		if (GUI.Button(rect2, string.Empty))
		{
			App.Config.settings.spectate_show_unstarted.toggle();
			this.FilterMatches();
		}
		if (App.Config.settings.spectate_show_unstarted)
		{
			GUI.DrawTexture(rect2, ResourceManager.LoadTexture("Arena/scroll_browser_button_cb_checked"));
		}
		else
		{
			GUI.DrawTexture(rect2, ResourceManager.LoadTexture("Arena/scroll_browser_button_cb"));
		}
	}

	// Token: 0x06001920 RID: 6432 RVA: 0x00094C94 File Offset: 0x00092E94
	private void OnGUI_drawGamesList(List<SpectatableGameInfo> matches)
	{
		float num = (float)Screen.height * 0.075f;
		float num2 = (float)Screen.height * 0.18f;
		float num3 = this.scrollArea.width - 20f;
		float num4 = num * 0.05f;
		this.scrollPos = GUI.BeginScrollView(this.scrollArea, this.scrollPos, new Rect(0f, 0f, num3, (num + num4) * (float)matches.Count + (num2 - num)));
		int num5 = 0;
		float num6 = 0f;
		foreach (SpectatableGameInfo spectatableGameInfo in matches)
		{
			float num7 = 6f;
			float num8 = num * 0.8f;
			Rect rect;
			if (num5 == this.selectedIndex)
			{
				rect..ctor(0f, num6, num3, num2);
				GUI.color = new Color(1f, 1f, 1f, 1f);
				GUI.Box(rect, string.Empty);
				GUI.color = new Color(1f, 0.85f, 0.65f, 0.3f);
				GUI.DrawTexture(new Rect(num7, rect.y + num7, rect.width - num7 * 2f, num2 - num7 * 2f), ResourceManager.LoadTexture("ChatUI/white"));
				GUI.color = new Color(1f, 1f, 1f, 0.3f);
				GUI.DrawTexture(new Rect(num7, rect.y + num7 + (num2 - num7 * 2f), rect.width - num7 * 2f, -(num2 - num7 * 2f)), ResourceManager.LoadTexture("BattleUI/battlegui_gradient"));
				GUI.color = Color.white;
			}
			else
			{
				rect..ctor(0f, num6, num3, num);
				GUI.color = new Color(1f, 1f, 1f, 1f);
				if (GUI.Button(rect, string.Empty))
				{
					App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
					this.selectedIndex = num5;
					this.isShowingWinner = false;
				}
			}
			float num9 = (num5 != this.selectedIndex) ? 0f : ((float)Screen.height * 0.04f);
			GUI.color = ((num5 == this.selectedIndex) ? Color.white : new Color(0.5f, 0.45f, 0.4f));
			Color color = (num5 != this.selectedIndex) ? new Color(0.65f, 0.6f, 0.47f) : Color.white;
			string text = (num5 != this.selectedIndex) ? "<color=#997755>" : "<color=#eedd99>";
			if (num5 == this.selectedIndex)
			{
				GUI.skin.label.alignment = 1;
				GUI.skin.label.fontSize = Screen.height / 30;
				string text2 = spectatableGameInfo.gameType.getString();
				if (spectatableGameInfo.title != null)
				{
					text2 = text2 + " - " + spectatableGameInfo.title;
				}
				GUIUtil.drawShadowText(new Rect(rect.x, rect.y + num7, rect.width, num * 0.55f), text2, new Color(0.9f, 0.8f, 0.5f), 0, 3);
			}
			SpectatablePlayerInfo[] array = new SpectatablePlayerInfo[]
			{
				spectatableGameInfo.whitePlayer,
				spectatableGameInfo.blackPlayer
			};
			foreach (SpectatablePlayerInfo spectatablePlayerInfo in array)
			{
				bool flag = spectatablePlayerInfo == spectatableGameInfo.whitePlayer;
				int num10 = (!flag) ? -1 : 1;
				GUI.color = ((num5 == this.selectedIndex) ? Color.white : new Color(0.5f, 0.45f, 0.4f));
				string[] array3 = spectatablePlayerInfo.resources.Split(new char[]
				{
					','
				});
				int num11 = 0;
				foreach (string text3 in array3)
				{
					if (!string.IsNullOrEmpty(text3))
					{
						float num12 = (!flag) ? (rect.width - num3 * 0.01f - num8) : (num3 * 0.01f);
						GUI.DrawTexture(new Rect(num12 + (float)num10 * num8 * 0.25f * (float)num11, rect.y + (num - num8) / 2f + num9, num8 * 73f / 72f, num8), ResourceManager.LoadTexture("BattleUI/battlegui_icon_" + text3.ToLower()));
						num11++;
					}
				}
				GUI.color = Color.white;
				float num13 = (!flag) ? (num3 * 0.6f) : (num3 * 0.02f + num8 * 1.75f);
				float num14 = num3 * 0.42f - num8 * 1.75f;
				Rect rect2;
				rect2..ctor(0f, rect.y + num7 + num9, num14, num * 0.55f);
				rect2.x = ((!flag) ? (num3 * 0.57f) : (num3 * 0.43f - rect2.width));
				GUI.skin.label.alignment = ((!flag) ? 0 : 2);
				GUI.skin.label.fontSize = Screen.height / 30;
				GUIUtil.drawShadowText(rect2, spectatablePlayerInfo.name, color, 0, 3);
				GUI.skin.label.fontSize = Screen.height / 40;
				rect2.y += num * 0.4f;
				GUI.Label(rect2, text + spectatablePlayerInfo.rating + "</color>");
				GUI.skin.label.alignment = 0;
				if (num5 == this.selectedIndex && spectatableGameInfo.isSpectate())
				{
					GUI.skin.label.alignment = ((!flag) ? 2 : 0);
					GUI.Label(new Rect((!flag) ? (num3 * 0.58f) : (num3 * 0.02f), rect.y + num + num9, num3 * 0.4f, num * 0.7f), string.Concat(new object[]
					{
						"<color=#b9aa88>Units on board: ",
						spectatablePlayerInfo.units.Value,
						"\nIdols remaining: ",
						spectatablePlayerInfo.idols.Value,
						"</color>"
					}));
				}
			}
			GUI.skin.label.fontSize = Screen.height / 40;
			GUI.skin.label.alignment = 1;
			GUI.Label(new Rect(num3 * 0.4f, rect.y + num7 - num * 0.02f + num9, num3 * 0.2f, num * 0.55f), text + "- VS -</color>");
			GUI.skin.label.fontSize = Screen.height / 50;
			if (spectatableGameInfo.isSpectate())
			{
				if (spectatableGameInfo.started.Value || this.secondsUntilStart(spectatableGameInfo.startTime.Value) < -1f)
				{
					GUI.Label(new Rect(num3 * 0.4f, rect.y + num7 + num * 0.27f + num9, num3 * 0.2f, num * 0.35f), string.Concat(new object[]
					{
						text,
						"Round: ",
						spectatableGameInfo.round,
						"</color>"
					}));
					GUI.Label(new Rect(num3 * 0.4f, rect.y + num7 + num * 0.47f + num9, num3 * 0.2f, num * 0.35f), string.Concat(new object[]
					{
						text,
						"Spectating: ",
						spectatableGameInfo.spectators,
						"</color>"
					}));
				}
				else
				{
					int num15 = Mathf.Clamp(0, (int)this.secondsUntilStart(spectatableGameInfo.startTime.Value), 999);
					GUI.Label(new Rect(num3 * 0.4f, rect.y + num7 + num * 0.27f + num9, num3 * 0.2f, num * 0.35f), text + "Game starts in</color>");
					GUI.Label(new Rect(num3 * 0.4f, rect.y + num7 + num * 0.47f + num9, num3 * 0.2f, num * 0.35f), text + num15 + " seconds</color>");
				}
			}
			else
			{
				GUI.Label(new Rect(num3 * 0.4f, rect.y + num7 + num * 0.27f + num9, num3 * 0.2f, num * 0.35f), string.Concat(new object[]
				{
					text,
					"Rounds: ",
					spectatableGameInfo.round,
					"</color>"
				}));
			}
			if (num5 == this.selectedIndex)
			{
				GUI.skin.button.fontSize = Screen.height / 30;
				Rect rect3;
				rect3..ctor(rect.width * 0.4f, rect.y + num * 1.1f + num9, rect.width * 0.2f, num * 0.6f);
				if (this.isReplay())
				{
					if (App.GUI.Button(rect3, "Replay"))
					{
						this.requestReplayLog(spectatableGameInfo);
					}
					Rect rect4 = GeomUtil.cropShare(rect3, new Rect(1.5f, 0.1f, 1f, 0.8f));
					if (this.isShowingWinner)
					{
						GUI.Label(GeomUtil.scaleCentered(rect4, 1f, 1.2f), spectatableGameInfo.getWinner(), this.midLabelStyle);
					}
					else if (App.GUI.Button(rect4, "Show winner", this.modeButtonStyle))
					{
						this.isShowingWinner = true;
					}
					Rect rect5 = GeomUtil.cropShare(rect3, new Rect(-1.3f, 0.1f, 0.9f, 0.8f));
					GUIUtil.drawShadowText(rect5, spectatableGameInfo.date, this.textColor, 0, 2);
				}
				else if (App.GUI.Button(rect3, "Watch"))
				{
					App.Communicator.send(new SpectateGameRequestMessage(spectatableGameInfo.gameId));
				}
			}
			num6 += rect.height + num4;
			num5++;
		}
		GUI.EndScrollView();
	}

	// Token: 0x06001921 RID: 6433 RVA: 0x000124DE File Offset: 0x000106DE
	private void FixedUpdate()
	{
		if (this.fadeIn > 0f)
		{
			this.fadeIn -= 0.03f;
		}
	}

	// Token: 0x06001922 RID: 6434 RVA: 0x00095794 File Offset: 0x00093994
	private float secondsUntilStart(long timestamp)
	{
		float num = Time.time - this.receivedLocalTime;
		float num2 = 0.001f * (float)(timestamp - this.receivedServerTime);
		return num2 - num;
	}

	// Token: 0x06001923 RID: 6435 RVA: 0x000957C4 File Offset: 0x000939C4
	private void setMode(Watch.Mode mode)
	{
		if (this.mode == mode)
		{
			return;
		}
		this.mode = mode;
		if (this.isReplay() && !this.hasReceivedReplayList)
		{
			this.requestReplayGamesFor(null);
		}
		this.isShowingWinner = false;
		if (this.isReplay())
		{
			this.sorter.clear();
			this.sorter.byTimestamp();
			this.filteredMatches().Sort(this.sorter);
			this.sortingIndex = 0;
		}
		if (this.isSpectate())
		{
			this.sorter.clear();
			this.sorter.byRating();
			this.filteredMatches().Sort(this.sorter);
			this.sortingIndex = 2;
		}
	}

	// Token: 0x06001924 RID: 6436 RVA: 0x00012502 File Offset: 0x00010702
	private void Update()
	{
		this._handleReplayComm();
	}

	// Token: 0x06001925 RID: 6437 RVA: 0x00095880 File Offset: 0x00093A80
	private void _handleReplayComm()
	{
		if (this.replayCommSetup == null)
		{
			return;
		}
		if (!this.replayCommSetup.isDone())
		{
			return;
		}
		if (this.replayCommSetup.success())
		{
			GetReplayLogMessage m = this.replayCommSetup.getTerminationMessage() as GetReplayLogMessage;
			App.GlobalMessageHandler.handleMessage(m);
		}
		else
		{
			FailMessage failMessage = this.replayCommSetup.getTerminationMessage() as FailMessage;
			if (failMessage != null)
			{
				this.handleMessage(failMessage);
			}
		}
		this.destroyReplayComm();
	}

	// Token: 0x06001926 RID: 6438 RVA: 0x0001250A File Offset: 0x0001070A
	private void requestReplayGamesFor(string name)
	{
		this.lastRequestedName = name;
		App.Communicator.send(new ReplayListGamesMessage(name, new GameType?(this.currentGameType)));
	}

	// Token: 0x06001927 RID: 6439 RVA: 0x0001252F File Offset: 0x0001072F
	private void requestReplayGames()
	{
		this.requestReplayGamesFor(this.lastRequestedName);
	}

	// Token: 0x06001928 RID: 6440 RVA: 0x0001253D File Offset: 0x0001073D
	private void destroyReplayComm()
	{
		if (this.replayCommSetup == null)
		{
			return;
		}
		this.replayCommSetup.Destroy();
		this.replayCommSetup = null;
	}

	// Token: 0x06001929 RID: 6441 RVA: 0x00095900 File Offset: 0x00093B00
	private void requestReplayLog(SpectatableGameInfo m)
	{
		this.destroyReplayComm();
		IpPort ipPort = this.serverLookup.get(m.serverId);
		if (ipPort == null)
		{
			App.Popups.ShowOk(new OkVoidCallback(), "server_not_found", "Server offline", "This game server is currently offline.", "Ok");
			return;
		}
		GetReplayLogMessage getReplayLogMessage = new GetReplayLogMessage(m.gameId);
		if (ipPort.Equals(App.Communicator.getAddress()))
		{
			App.Communicator.send(getReplayLogMessage);
		}
		else
		{
			this.replayCommSetup = new CommSetup(ipPort, getReplayLogMessage, new CommSetup.RecvCompleteCondition(getReplayLogMessage.GetType()));
		}
	}

	// Token: 0x0600192A RID: 6442 RVA: 0x0009599C File Offset: 0x00093B9C
	public override void handleMessage(Message msg)
	{
		if (msg is SpectateListGamesMessage)
		{
			SpectateListGamesMessage spectateListGamesMessage = (SpectateListGamesMessage)msg;
			if (spectateListGamesMessage.spectatable == null)
			{
				return;
			}
			this.receivedLocalTime = Time.time;
			this.receivedServerTime = spectateListGamesMessage.currentTime;
			this.spectateMatches = new List<SpectatableGameInfo>(spectateListGamesMessage.spectatable);
			this.currentGameType = this.stringToGameType[this.gametypeDrop.GetSelectedItem()];
			this.FilterSpectateMatches();
		}
		if (msg is ReplayListGamesMessage)
		{
			ReplayListGamesMessage replayListGamesMessage = (ReplayListGamesMessage)msg;
			this.serverLookup = new ServerIdLookup(replayListGamesMessage.servers);
			this.replayName = replayListGamesMessage.profileName;
			if (this.replayName == null)
			{
				this.replayName = "everyone";
			}
			this.replayMatches = Enumerable.ToList<SpectatableGameInfo>(Enumerable.Select<ReplayGameInfo, SpectatableGameInfo>(replayListGamesMessage.replays, (ReplayGameInfo g) => g.toSpectateInfo()));
			this.currentGameType = this.stringToGameType[this.gametypeDrop.GetSelectedItem()];
			this.hasReceivedReplayList = true;
			this.FilterReplayMatches();
		}
		if (msg is FailMessage)
		{
			FailMessage failMessage = (FailMessage)msg;
			if (failMessage.isType(typeof(ReplayListGamesMessage)))
			{
				App.Popups.ShowOk(new OkVoidCallback(), "replay_request_failed", "Error", failMessage.info, "Ok");
			}
		}
	}

	// Token: 0x0600192B RID: 6443 RVA: 0x000028DF File Offset: 0x00000ADF
	public void PopupCancel(string popupType)
	{
	}

	// Token: 0x0600192C RID: 6444 RVA: 0x0001255D File Offset: 0x0001075D
	public void PopupOk(string popupType, string choice)
	{
		if (popupType == "choose_player")
		{
			this.requestReplayGamesFor(choice);
		}
	}

	// Token: 0x04001573 RID: 5491
	private GUISkin regularUI;

	// Token: 0x04001574 RID: 5492
	private GUISkin settingsSkin;

	// Token: 0x04001575 RID: 5493
	private GUIStyle settingsUILabel;

	// Token: 0x04001576 RID: 5494
	private GUIStyle modeButtonStyle;

	// Token: 0x04001577 RID: 5495
	private GUIStyle midLabelStyle;

	// Token: 0x04001578 RID: 5496
	private Vector2 scrollPos;

	// Token: 0x04001579 RID: 5497
	private float fadeIn = 1f;

	// Token: 0x0400157A RID: 5498
	private List<SpectatableGameInfo> spectateMatches = new List<SpectatableGameInfo>();

	// Token: 0x0400157B RID: 5499
	private List<SpectatableGameInfo> filteredSpectateMatches = new List<SpectatableGameInfo>();

	// Token: 0x0400157C RID: 5500
	private List<SpectatableGameInfo> replayMatches = new List<SpectatableGameInfo>();

	// Token: 0x0400157D RID: 5501
	private List<SpectatableGameInfo> filteredReplayMatches = new List<SpectatableGameInfo>();

	// Token: 0x0400157E RID: 5502
	private SpectatableGameInfoSorter sorter = new SpectatableGameInfoSorter();

	// Token: 0x0400157F RID: 5503
	private int sortingIndex;

	// Token: 0x04001580 RID: 5504
	private GUIStyle highlightedButtonStyle;

	// Token: 0x04001581 RID: 5505
	private Color textColor = ColorUtil.FromHex24(15654297u);

	// Token: 0x04001582 RID: 5506
	private Dropdown gametypeDrop;

	// Token: 0x04001583 RID: 5507
	private GameType currentGameType;

	// Token: 0x04001584 RID: 5508
	private string replayName = App.MyProfile.ProfileInfo.name;

	// Token: 0x04001585 RID: 5509
	private Dictionary<string, GameType> stringToGameType = new Dictionary<string, GameType>();

	// Token: 0x04001586 RID: 5510
	private IsValueChanged resolution;

	// Token: 0x04001587 RID: 5511
	private Watch.Mode mode = Watch.Mode.Spectate;

	// Token: 0x04001588 RID: 5512
	private bool hasReceivedReplayList;

	// Token: 0x04001589 RID: 5513
	private ServerIdLookup serverLookup = new ServerIdLookup();

	// Token: 0x0400158A RID: 5514
	private CommSetup replayCommSetup;

	// Token: 0x0400158B RID: 5515
	private ScrollsFrame leftFrame;

	// Token: 0x0400158C RID: 5516
	private ScrollsFrame rightFrame;

	// Token: 0x0400158D RID: 5517
	private Rect modeReplayRect;

	// Token: 0x0400158E RID: 5518
	private Rect modeSpectateRect;

	// Token: 0x0400158F RID: 5519
	private Rect gameTypeRect;

	// Token: 0x04001590 RID: 5520
	private Rect sortRect;

	// Token: 0x04001591 RID: 5521
	private Rect rightTitleRect;

	// Token: 0x04001592 RID: 5522
	private Rect rightInnerRect;

	// Token: 0x04001593 RID: 5523
	private Rect scrollArea;

	// Token: 0x04001594 RID: 5524
	private Rect rightArea;

	// Token: 0x04001595 RID: 5525
	private Rect[] findReplayRects;

	// Token: 0x04001596 RID: 5526
	private int selectedIndex;

	// Token: 0x04001597 RID: 5527
	private float receivedLocalTime;

	// Token: 0x04001598 RID: 5528
	private long receivedServerTime;

	// Token: 0x04001599 RID: 5529
	private bool isShowingWinner;

	// Token: 0x0400159A RID: 5530
	private string lastRequestedName;

	// Token: 0x02000463 RID: 1123
	private enum Mode
	{
		// Token: 0x0400159E RID: 5534
		Replay,
		// Token: 0x0400159F RID: 5535
		Spectate
	}
}
