using System;
using Gui;
using Stijn.Localization;
using UnityEngine;

// Token: 0x0200021D RID: 541
public class MainMenu : AbstractCommListener, IOkCallback, ICancelCallback, IOkCancelCallback
{
	// Token: 0x06001142 RID: 4418 RVA: 0x000750A0 File Offset: 0x000732A0
	private void Start()
	{
		this.regularUI = (GUISkin)ResourceManager.Load("_GUISkins/RegularUI");
		Application.targetFrameRate = 60;
		this._gui3dMaterial = new Material(ResourceManager.LoadShader("Scrolls/Unlit/Transparent"));
		this.comm = App.Communicator;
		this.comm.addListener(this);
		this.comm.send(new OverallStatsMessage());
		this.comm.send(new GetTwitterFeedMessage());
		App.LobbyMenu.fadeInScene();
		this.cloudSpeed = Random.Range(0.0006f, 0.0015f);
		this.timeOffset = (float)(new Random().NextDouble() * 3600.0);
		this._gui = new Gui3D(Camera.main);
		this._gui.setDefaultMaterial(this._gui3dMaterial);
		this.randomBg = MainMenu.bgs[Random.Range(0, MainMenu.bgs.Length)];
		for (int i = 0; i < 4; i++)
		{
			string fnfmt = string.Concat(new object[]
			{
				this.randomBg.path,
				"img_",
				i,
				"_p{x}"
			});
			this.layers[i] = LargeGuiImage.fromFilenamesAndCutSize(fnfmt, this.randomBg.width, 1024, 1024, 1024);
			this.layers[i].setGui(this._gui);
		}
		this.infoStyle = new GUIStyle(this.regularUI.label);
		this.infoStyle.alignment = 3;
		this.infoStyle.fontSize = Screen.height / 40;
		this.infoStyle.wordWrap = true;
		this.watchButtonStyle = new GUIStyle(this.infoStyle);
		this.watchButtonStyle.alignment = 4;
		this.watchButtonStyle.border = this.regularUI.button.border;
		this.watchButtonStyle.normal.background = this.regularUI.button.normal.background;
		this.watchButtonStyle.hover.background = this.regularUI.button.hover.background;
		this.watchButtonStyle.active.background = this.regularUI.button.active.background;
		this.tweetStyle = new GUIStyle(this.infoStyle);
		this.tweetStyle.fontSize = Screen.height / 50;
		this.tweetStyle.alignment = 0;
		this.quoteStyle = new GUIStyle(this.tweetStyle);
		this.quoteStyle.fontSize = Screen.height / 50;
		this.quoteStyle.alignment = 2;
		this.emptyStyle = ((GUISkin)ResourceManager.Load("_GUISkins/EmptySkin")).button;
		App.ChatUI.Show(false);
	}

	// Token: 0x06001143 RID: 4419 RVA: 0x0000D338 File Offset: 0x0000B538
	private void Update()
	{
		this._gui.frameBegin();
		this.drawLayers();
		this._gui.frameEnd();
	}

	// Token: 0x06001144 RID: 4420 RVA: 0x00075374 File Offset: 0x00073574
	private void drawLayers()
	{
		float num = this.baseSpeed;
		this.drawLayer(3, this.cloudSpeed, 0f);
		this.drawLayer(2, num * this.layer2, 0f);
		this.drawLayer(1, num * this.layer1, 0f);
		this.drawLayer(0, num * this.layer0, 0f);
	}

	// Token: 0x06001145 RID: 4421 RVA: 0x000753D8 File Offset: 0x000735D8
	private void drawLayer(int layer, float speed, float offset)
	{
		float num = LobbyMenu.getMenuRect().yMax - (float)Screen.height * 0.005f;
		Rect dst;
		dst..ctor(0f, num, (float)Screen.width, (float)Screen.height - num);
		float num2 = offset + (Time.time + this.timeOffset) * speed;
		Rect src;
		src..ctor((float)this.randomBg.width * num2, 0f, 1024f * dst.width / dst.height, 1024f);
		this.layers[layer].Draw(dst, src);
	}

	// Token: 0x06001146 RID: 4422 RVA: 0x00075470 File Offset: 0x00073670
	private void OnGUI()
	{
		GUI.depth = 21;
		Rect rect;
		rect..ctor((float)Screen.width * 0.5f - (float)Screen.height * 0.22f, (float)Screen.height * 0.27f, (float)Screen.height * 0.44f, (float)Screen.height * 0.6f);
		Rect rect2;
		rect2..ctor((float)Screen.width * 0.5f - (float)Screen.height * 0.52f, (float)Screen.height * 0.3f, (float)Screen.height * 0.29f, (float)Screen.height * 0.54f);
		Rect rect3;
		rect3..ctor((float)Screen.width * 0.5f + (float)Screen.height * 0.23f, (float)Screen.height * 0.3f, (float)Screen.height * 0.29f, (float)Screen.height * 0.54f);
		new ScrollsFrame(rect).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.CENTER).Draw(new Color(1f, 1f, 1f, 0.8f));
		new ScrollsFrame(rect2).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.CENTER).Draw(new Color(1f, 1f, 1f, 0.8f));
		new ScrollsFrame(rect3).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.CENTER).Draw(new Color(1f, 1f, 1f, 0.8f));
		GUI.skin = this.regularUI;
		Rect rect4;
		rect4..ctor((float)Screen.height * 0.029f, (float)Screen.height * 0.95f, (float)Screen.height * 0.1f, (float)Screen.height * 0.035f);
		if (GUI.Button(rect4, "<color=#bbaa99>Credits</color>"))
		{
			App.Popups.ShowScrollText(this, "creditspopup", "Credits", (ResourceManager.Load("Credits/credits") as TextAsset).text, "Close");
		}
		Rect rect5;
		rect5..ctor(rect4);
		rect5.x += rect4.width * 1.3f;
		if (GUI.Button(rect5, "<color=#bbaa99>Conduct</color>"))
		{
			App.Popups.ShowScrollText(this, "conductpopup", "A word about Conduct", (ResourceManager.Load("Credits/conduct") as TextAsset).text, "Close");
		}
		Rect rect6;
		rect6..ctor(rect4);
		rect6.x = (float)Screen.width - rect6.width - (float)Screen.height * 0.029f;
		rect6.y = LobbyMenu.getMenuRect().yMax + (float)Screen.height * 0.01f;
		if (GUI.Button(rect6, "<color=#bbaa99>Sign out</color>"))
		{
			App.Popups.ShowOkCancel(this, "signout", "Sign out", "Are you sure you want to sign out?", "Sign out", "Cancel");
		}
		float num = (float)Screen.height * 0.023f;
		Rect rect7;
		rect7..ctor(rect.x + num, rect.y + 3f * num, rect.width - num * 2f, rect.height - num * 4f);
		GUILayout.BeginArea(rect7);
		this.newsScroll = GUILayout.BeginScrollView(this.newsScroll, new GUILayoutOption[]
		{
			GUILayout.Width(rect7.width),
			GUILayout.Height(rect7.height)
		});
		GUILayout.Label(App.AssetLoader.GetNews(), this.infoStyle, new GUILayoutOption[0]);
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		Rect rect8;
		rect8..ctor(rect2.x + num, rect2.y + num, rect2.width - num * 2f, rect2.height - num * 2f);
		GUILayout.BeginArea(rect8);
		this.feedScroll = GUILayout.BeginScrollView(this.feedScroll, new GUILayoutOption[]
		{
			GUILayout.Width(rect8.width),
			GUILayout.Height(rect8.height)
		});
		if (App.IsStandalone)
		{
			this.OnGUI_statistics();
			this.OnGUI_weeklyWinners();
		}
		else
		{
			this.OnGUI_twitterFeed();
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		Rect rect9;
		rect9..ctor(rect3.x + num, rect3.y + num, rect3.width - num * 2f, rect3.height - num * 2f);
		GUILayout.BeginArea(rect9);
		this.statsScroll = GUILayout.BeginScrollView(this.statsScroll, new GUILayoutOption[]
		{
			GUILayout.Width(rect9.width),
			GUILayout.Height(rect9.height)
		});
		if (!App.IsStandalone)
		{
			this.OnGUI_statistics();
			this.OnGUI_weeklyWinners();
		}
		this.OnGUI_topRanked();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		Texture2D texture2D = ResourceManager.LoadTexture("Logos/scrollslogo");
		float num2 = (float)Screen.height * 0.12f;
		float num3 = num2 * (float)texture2D.width / (float)texture2D.height;
		GUI.DrawTexture(new Rect((float)Screen.width * 0.5f - num3 / 2f, (float)Screen.height * 0.255f - num2 / 2f, num3, num2), texture2D);
	}

	// Token: 0x06001147 RID: 4423 RVA: 0x0000D356 File Offset: 0x0000B556
	public override void handleMessage(Message msg)
	{
		if (msg is OverallStatsMessage)
		{
			this.stats = (OverallStatsMessage)msg;
			App.ChatUI.SetWeeklyWinners(this.stats.weeklyWinners);
		}
	}

	// Token: 0x06001148 RID: 4424 RVA: 0x0000D381 File Offset: 0x0000B581
	public void PopupOk(string popupType)
	{
		if (popupType == "signout")
		{
			App.SignOut();
		}
	}

	// Token: 0x06001149 RID: 4425 RVA: 0x000028DF File Offset: 0x00000ADF
	public void PopupCancel(string popupType)
	{
	}

	// Token: 0x0600114A RID: 4426 RVA: 0x00075980 File Offset: 0x00073B80
	private void OnGUI_twitterFeed()
	{
		TwitterSearch twitterFeed = App.AssetLoader.GetTwitterFeed();
		if (twitterFeed == null)
		{
			GUILayout.Label("Loading tweets...", new GUILayoutOption[0]);
			return;
		}
		foreach (Tweet tweet in twitterFeed.statuses)
		{
			if (!tweet.text.StartsWith("@"))
			{
				Texture2D twitterImage = App.AssetLoader.GetTwitterImage(tweet.user.name);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				Rect lastRect;
				lastRect..ctor(0f, 0f, 1f, 1f);
				if (twitterImage != null)
				{
					TextAnchor alignment = GUI.skin.label.alignment;
					GUI.skin.label.alignment = 4;
					GUILayout.Label(twitterImage, new GUILayoutOption[]
					{
						GUILayout.Width((float)Screen.height * 0.04f),
						GUILayout.Height((float)Screen.height * 0.04f)
					});
					GUI.skin.label.alignment = alignment;
					lastRect = GUILayoutUtility.GetLastRect();
				}
				GUILayout.Label(tweet.text, this.tweetStyle, new GUILayoutOption[0]);
				if (twitterImage == null)
				{
					lastRect = GUILayoutUtility.GetLastRect();
				}
				GUILayout.EndHorizontal();
				GUILayout.Label("- <color=#ffdd88>" + tweet.user.screen_name + "</color>", this.quoteStyle, new GUILayoutOption[0]);
				Rect coveringRect = GeomUtil.getCoveringRect(new Rect[]
				{
					lastRect,
					GUILayoutUtility.GetLastRect()
				});
				if (coveringRect.x > 1.1f && GUI.Button(coveringRect, string.Empty, this.emptyStyle))
				{
					Application.OpenURL("https://twitter.com/" + tweet.user.screen_name + "/status/" + tweet.id_str);
				}
			}
		}
	}

	// Token: 0x0600114B RID: 4427 RVA: 0x00075B58 File Offset: 0x00073D58
	private void OnGUI_statistics()
	{
		if (this.stats == null)
		{
			GUILayout.Label("Loading server info...", new GUILayoutOption[0]);
			GUILayout.Label("Loading statistics...", new GUILayoutOption[0]);
			return;
		}
		GUILayout.Label("<color=#aaaaaa>Client version:</color> " + SharedConstants.getGameVersion() + "\n", this.infoStyle, new GUILayoutOption[0]);
		GUILayout.Label("<color=#aaaaaa>Server info</color>", this.infoStyle, new GUILayoutOption[0]);
		GUILayout.Label("<color=#ffdd88>Name:</color> " + this.stats.serverName, this.infoStyle, new GUILayoutOption[0]);
		GUILayout.Label("<color=#ffdd88>Version:</color> " + this.comm.getServerVersion().ToString(), this.infoStyle, new GUILayoutOption[0]);
		GUILayout.Label("<color=#ffdd88>Users online:</color> " + this.stats.nrOfProfiles, this.infoStyle, new GUILayoutOption[0]);
		GUILayout.Label("<color=#ffdd88>Online last 24 hours:</color> " + this.stats.loginsLast24h, this.infoStyle, new GUILayoutOption[0]);
	}

	// Token: 0x0600114C RID: 4428 RVA: 0x00075C70 File Offset: 0x00073E70
	private void OnGUI_weeklyWinners()
	{
		if (this.stats == null || this.stats.weeklyWinners == null || this.stats.weeklyWinners.Length == 0)
		{
			return;
		}
		string text = LocalizationManager.GetText("UI_WEEKLY_WINNERS", new object[0]);
		GUILayout.Label("<color=#ffdd88>" + text + "</color>", this.infoStyle, new GUILayoutOption[0]);
		foreach (WeeklyWinner weeklyWinner in this.stats.weeklyWinners)
		{
			GUILayout.Space(-(float)Screen.height / 200f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (weeklyWinner.gameId != null)
			{
				if (GUILayout.Button(ResourceManager.LoadTexture("Shared/spectate_eye"), this.watchButtonStyle, new GUILayoutOption[]
				{
					GUILayout.Height((float)this.infoStyle.fontSize),
					GUILayout.Width((float)this.infoStyle.fontSize)
				}))
				{
					SpectateGameRequestMessage m = new SpectateGameRequestMessage(weeklyWinner.gameId.Value);
					App.Communicator.send(m);
				}
			}
			else
			{
				GUILayout.Label(string.Empty, this.infoStyle, new GUILayoutOption[]
				{
					GUILayout.Height((float)this.infoStyle.fontSize),
					GUILayout.Width((float)this.infoStyle.fontSize)
				});
			}
			GUILayout.Label(ResourceManager.LoadTexture(weeklyWinner.getIcon()), new GUILayoutOption[]
			{
				GUILayout.Width((float)this.infoStyle.fontSize),
				GUILayout.Height((float)this.infoStyle.fontSize)
			});
			GUILayout.Label(weeklyWinner.userName, this.infoStyle, new GUILayoutOption[]
			{
				GUILayout.Height((float)this.infoStyle.fontSize)
			});
			GUILayout.EndHorizontal();
		}
	}

	// Token: 0x0600114D RID: 4429 RVA: 0x00075E34 File Offset: 0x00074034
	private void OnGUI_topRanked()
	{
		if (this.stats == null)
		{
			return;
		}
		OverallStatsMessage.RankedPlayer[] topList = this.stats.getTopList();
		GUILayout.Label("<color=#ffdd88>Top rated players</color>", this.infoStyle, new GUILayoutOption[0]);
		for (int i = 0; i < topList.Length; i++)
		{
			GUILayout.Space(-(float)Screen.height / 200f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (topList[i].gameId != null)
			{
				if (GUILayout.Button(ResourceManager.LoadTexture("Shared/spectate_eye"), this.watchButtonStyle, new GUILayoutOption[]
				{
					GUILayout.Height((float)this.infoStyle.fontSize),
					GUILayout.Width((float)this.infoStyle.fontSize)
				}))
				{
					SpectateGameRequestMessage m = new SpectateGameRequestMessage(topList[i].gameId.Value);
					App.Communicator.send(m);
				}
			}
			else
			{
				GUILayout.Label(string.Empty, this.infoStyle, new GUILayoutOption[]
				{
					GUILayout.Height((float)this.infoStyle.fontSize),
					GUILayout.Width((float)this.infoStyle.fontSize)
				});
			}
			GUILayout.Label(string.Concat(new object[]
			{
				"<color=#aaaaaa>",
				topList[i].rank,
				".</color> ",
				topList[i].name
			}), this.infoStyle, new GUILayoutOption[]
			{
				GUILayout.Height((float)this.infoStyle.fontSize)
			});
			GUILayout.EndHorizontal();
		}
	}

	// Token: 0x04000DA5 RID: 3493
	private Vector2 newsScroll = new Vector2(0f, 0f);

	// Token: 0x04000DA6 RID: 3494
	private Vector2 feedScroll = new Vector2(0f, 0f);

	// Token: 0x04000DA7 RID: 3495
	private Vector2 statsScroll = new Vector2(0f, 0f);

	// Token: 0x04000DA8 RID: 3496
	private OverallStatsMessage stats;

	// Token: 0x04000DA9 RID: 3497
	private GUIStyle infoStyle;

	// Token: 0x04000DAA RID: 3498
	private GUIStyle tweetStyle;

	// Token: 0x04000DAB RID: 3499
	private GUIStyle quoteStyle;

	// Token: 0x04000DAC RID: 3500
	private GUIStyle watchButtonStyle;

	// Token: 0x04000DAD RID: 3501
	private GUIStyle emptyStyle;

	// Token: 0x04000DAE RID: 3502
	private static MainMenu.BackgroundDesc[] bgs = new MainMenu.BackgroundDesc[]
	{
		new MainMenu.BackgroundDesc(7282, "BackGrounds/splash1/")
	};

	// Token: 0x04000DAF RID: 3503
	private MainMenu.BackgroundDesc randomBg;

	// Token: 0x04000DB0 RID: 3504
	private float timeOffset;

	// Token: 0x04000DB1 RID: 3505
	private Communicator comm;

	// Token: 0x04000DB2 RID: 3506
	private Gui3D _gui;

	// Token: 0x04000DB3 RID: 3507
	private Material _gui3dMaterial;

	// Token: 0x04000DB4 RID: 3508
	private float cloudSpeed;

	// Token: 0x04000DB5 RID: 3509
	private float baseSpeed = 0.007f;

	// Token: 0x04000DB6 RID: 3510
	private float layer2 = 0.1f;

	// Token: 0x04000DB7 RID: 3511
	private float layer1 = 0.2f;

	// Token: 0x04000DB8 RID: 3512
	private float layer0 = 0.67f;

	// Token: 0x04000DB9 RID: 3513
	private LargeGuiImage[] layers = new LargeGuiImage[4];

	// Token: 0x04000DBA RID: 3514
	private GUISkin regularUI;

	// Token: 0x0200021E RID: 542
	private class BackgroundDesc
	{
		// Token: 0x0600114E RID: 4430 RVA: 0x0000D395 File Offset: 0x0000B595
		public BackgroundDesc(int imageWidth, string path)
		{
			this.width = imageWidth;
			this.path = path;
		}

		// Token: 0x04000DBB RID: 3515
		public int width;

		// Token: 0x04000DBC RID: 3516
		public string path;
	}
}
