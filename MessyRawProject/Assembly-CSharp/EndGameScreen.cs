using System;
using System.Collections;
using System.Collections.Generic;
using Gui;
using UnityEngine;

// Token: 0x02000077 RID: 119
public class EndGameScreen : MonoBehaviour
{
	// Token: 0x0600049A RID: 1178 RVA: 0x00031E7C File Offset: 0x0003007C
	public void init(GameMode gameMode, MiniCommunicator specComm, GameType gameType, TileColor playerColor, EMEndGame endGameStatistics, Avatar leftAvatar, Avatar rightAvatar, string leftName, string rightName, int round)
	{
		this.inited = true;
		this.gameMode = gameMode;
		this.gameType = gameType;
		this.playerColor = playerColor;
		this.endGameStatistics = endGameStatistics;
		this.leftName = leftName;
		this.rightName = rightName;
		this.leftAvatar = leftAvatar;
		this.rightAvatar = rightAvatar;
		this.specComm = specComm;
		this.round = round;
		App.GlobalMessageHandler.setTimeScale(1f);
		this.isWinner = (playerColor == endGameStatistics.winner);
		if (this.isWinner)
		{
			App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_victory");
		}
		else
		{
			App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_defeat");
		}
		this.titleTex = ((!this.isWinner) ? ResourceManager.LoadTexture("BattleMode/GUI/title_defeat") : ResourceManager.LoadTexture("BattleMode/GUI/title_victory"));
		this.statsSkin = (GUISkin)ResourceManager.Load("_GUISkins/StatsSkin");
		this.emptySkin = (GUISkin)ResourceManager.Load("_GUISkins/EmptySkin");
		this.labelSkin = ScriptableObject.CreateInstance<GUISkin>();
		this.labelSkin.label.wordWrap = false;
		this.labelSkin.label.font = (Font)ResourceManager.Load("Fonts/arial", typeof(Font));
		this.labelSkin.label.fontSize = 16;
		this.labelSkin.label.normal.background = ResourceManager.LoadTexture("BattleMode/blackDot");
		this.labelSkin.label.normal.textColor = new Color(1f, 1f, 1f, 1f);
		this.labelSkin.label.fontStyle = 1;
		this.balloonLabelStyle = new GUIStyle(((GUISkin)ResourceManager.Load("_GUISkins/RegularUI")).label);
		this.balloonLabelStyle.wordWrap = true;
		this.balloonLabelStyle.normal.textColor = new Color(0.23f, 0.16f, 0.125f);
		this.balloonLabelStyle.normal.background = null;
		this.balloonLabelStyle.alignment = 0;
		this.rewardButtonStyle = GUIUtil.createButtonStyle("BattleMode/EndScreen/ScrollReward/scroll_reward_reveal_button_");
		base.StartCoroutine(this.FadeInStats());
		base.StartCoroutine(this.FadeInLimited());
		base.StartCoroutine(this.MoveInAvatars());
		this.ggui = new Gui3D(UnityUtil.getFirstOrtographicCamera());
		this.ggui.setLayer(Layers.BattleModeUI);
		this.ggui.setRenderQueue(94000, false);
		this.setPage(EndGameScreen.Page.Reveal);
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x00005056 File Offset: 0x00003256
	public bool isDone()
	{
		return this.done;
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x0000505E File Offset: 0x0000325E
	public bool isInited()
	{
		return this.inited;
	}

	// Token: 0x0600049D RID: 1181 RVA: 0x00032110 File Offset: 0x00030310
	private void OnGUI_drawReveal()
	{
		if (this.currentPage != EndGameScreen.Page.Reveal)
		{
			return;
		}
		if (!this.hasBonusRewards())
		{
			return;
		}
		GUI.color = new Color(1f, 1f, 1f, this.statsGoldAlpha);
		float num = (float)Screen.height * 0.55f;
		float widthFromHeight = GeomUtil.getWidthFromHeight(num, 546f, 626f);
		Rect rect;
		rect..ctor(0.5f * ((float)Screen.width - widthFromHeight), 0.2f * (float)Screen.height, widthFromHeight, num);
		rect = GeomUtil.scaleCentered(rect, 1f + 0.015f * Mathf.Sin(5f * Time.time));
		GUI.DrawTexture(rect, ResourceManager.LoadTexture("BattleMode/EndScreen/ScrollReward/scroll_reward"));
		float num2 = (float)Screen.height * 0.2f;
		float num3 = 2.3991227f * num2;
		if (GUI.Button(new Rect(0.5f * ((float)Screen.width - num3), 0.55f * (float)Screen.height, num3, num2), string.Empty, this.rewardButtonStyle))
		{
			this.showReward();
		}
		GUI.color = Color.white;
	}

	// Token: 0x0600049E RID: 1182 RVA: 0x00032228 File Offset: 0x00030428
	private void showReward()
	{
		float num = 0.6f;
		Rect rect;
		rect..ctor(0f, (float)Screen.height * 0.2f, num * (float)Screen.height * 0.633f, num * (float)Screen.height * 1.024f);
		Rect centered = GeomUtil.getCentered(rect, true, false);
		Card card = this.endGameStatistics.cardReward(this.isWinner)[0];
		this.cardReveal = new CardReveal().Init(this.ggui, delegate
		{
			this.showGoldCountupSoon();
		}, this.cardRenderTexture, centered);
		this.cardReveal.Show(card);
		this.setPage(EndGameScreen.Page.Rewards);
	}

	// Token: 0x0600049F RID: 1183 RVA: 0x00005066 File Offset: 0x00003266
	private void showGoldCountupSoon()
	{
		base.StartCoroutine(EnumeratorUtil.chain(new IEnumerator[]
		{
			EnumeratorUtil.Func(new WaitForSeconds(2.5f)),
			EnumeratorUtil.Func(new Action(this.showGoldCountup))
		}));
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x000050A0 File Offset: 0x000032A0
	private void showGoldCountup()
	{
		this.setPage(EndGameScreen.Page.GoldCountup);
	}

	// Token: 0x060004A1 RID: 1185 RVA: 0x000050A9 File Offset: 0x000032A9
	private void removeReward()
	{
		if (this.cardReveal == null)
		{
			return;
		}
		this.cardReveal.Hide();
		this.cardReveal = null;
	}

	// Token: 0x060004A2 RID: 1186 RVA: 0x000322C8 File Offset: 0x000304C8
	private void playBuyEffect(string file)
	{
		GameObject gameObject = new GameObject();
		gameObject.AddComponent<MeshRenderer>();
		gameObject.name = "BuyEffect_";
		EffectPlayer effectPlayer = gameObject.AddComponent<EffectPlayer>();
		Material materialToUse = new Material(ResourceManager.LoadShader("Scrolls/StoreEffect/Unlit/Transparent"));
		effectPlayer.setMaterialToUse(materialToUse);
		effectPlayer.init("BuyEffect/" + file, 1, DefaultIEffectCallback.instance(), 94000, new Vector3(0.409f, 0.4264f, 0.5f), false, string.Empty, 0);
		effectPlayer.getAnimPlayer().waitForUpdate();
		gameObject.transform.localPosition = new Vector3(0.05f, -0.3f, 4f);
		gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
	}

	// Token: 0x060004A3 RID: 1187 RVA: 0x0003238C File Offset: 0x0003058C
	private void Update()
	{
		if (this.ggui == null)
		{
			return;
		}
		this.pageTime += Time.deltaTime;
		if (this.headerYFactor >= 0.999f && this.gameType.isTutorial() && this.hasBonusRewards())
		{
			this.balloonAlpha = Math.Min(1f, this.balloonAlpha + 2f * Time.deltaTime);
		}
		else
		{
			this.balloonAlpha = Math.Max(0f, this.balloonAlpha - 3f * Time.deltaTime);
		}
		Color color;
		color..ctor(0f, 0f, 0f, this.gameOverOverlayAlpha * 0.84f);
		Rect dst = GUIUtil.screen(new Rect(-0.01f, -0.01f, 1.02f, 1.02f));
		this.ggui.frameBegin();
		this.ggui.setDepth(9f);
		this.ggui.DrawTexture(dst, ResourceManager.LoadTexture("ChatUI/white"));
		this.ggui.GetLastMaterial().color = color;
		this.ggui.DrawTexture(new Rect(0f, (float)Screen.height * 0.65f, (float)Screen.width, (float)Screen.height * 0.35f), ResourceManager.LoadTexture("BattleUI/battlegui_gradient_white"));
		color.a = this.gameOverOverlayAlpha * 0.6f;
		this.ggui.GetLastMaterial().color = color;
		this.ggui.frameEnd();
	}

	// Token: 0x060004A4 RID: 1188 RVA: 0x0003251C File Offset: 0x0003071C
	public void OnGUI()
	{
		if (!this.inited)
		{
			return;
		}
		GUI.depth = 7;
		if (this.currentPage == EndGameScreen.Page.Reveal && this.showingStats)
		{
			if (this.gameType == GameType.MP_LIMITED)
			{
				this.setPage(EndGameScreen.Page.GoldRewardDetails);
			}
			else if (!this.hasBonusRewards())
			{
				this.showGoldCountup();
			}
		}
		Color color = GUI.color;
		GUI.color = new Color(0f, 0f, 0f, this.gameOverOverlayAlpha * 0.84f);
		Rect rect = GUIUtil.screen(new Rect(-0.01f, -0.01f, 1.02f, 1.02f));
		GUI.color = color;
		GUISkin skin = GUI.skin;
		float rankedYOffset = (!this.gameType.isMultiplayerRanked()) ? 0f : -0.07f;
		float num = (!this.gameType.isMultiplayerRanked()) ? 0f : -0.12f;
		this.OnGUI_drawReveal();
		this.OnGUI_drawAvatars(rankedYOffset);
		GUI.color = new Color(0f, 0f, 0f, this.gameOverOverlayAlpha * 0.6f);
		GUI.color = Color.white;
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.gameOverAlpha);
		float num2 = (0.75f + (1f - this.headerYFactor) * 0.25f + (1f - this.gameOverAlpha) * 1.5f) * (float)Screen.height / 1080f;
		float num3 = (float)this.titleTex.height * num2;
		float num4 = (float)Screen.height * 0.05f;
		float num5 = 0f;
		float num6 = num5 + num4 + (float)(Screen.height / 2) - num3 * 0.5f - this.headerYFactor * ((float)(Screen.height / 2) - num3 * 0.3f);
		Rect rect2;
		rect2..ctor((float)(Screen.width / 2) - (float)(this.titleTex.width / 2) * num2, num6, (float)this.titleTex.width * num2, (float)this.titleTex.height * num2);
		this.OnGUI_drawBalloon(rect2);
		GUI.DrawTexture(rect2, this.titleTex);
		if (this.victorySlamAlpha > 0f)
		{
			float num7 = this.victorySlamAlpha;
			num2 *= 1f + num7 * 0.75f;
			num3 = (float)this.titleTex.height * num2;
			num6 = num4 + (float)(Screen.height / 2) - num3 * 0.5f - this.headerYFactor * ((float)(Screen.height / 2) - num3 * 0.3f);
			GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1f - Mathf.Pow(num7, 0.7f));
			GUI.DrawTexture(new Rect((float)(Screen.width / 2) - (float)(this.titleTex.width / 2) * num2, num6, (float)this.titleTex.width * num2, (float)this.titleTex.height * num2), this.titleTex);
		}
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.statsGoldAlpha);
		GUI.skin = this.statsSkin;
		if (this.currentPage == EndGameScreen.Page.Rewards && this.cardReveal != null && this.pageTime >= 1f && GUI.Button(GUIUtil.screen(), string.Empty, this.emptySkin.label))
		{
			this.showGoldCountup();
		}
		this.OnGUI_drawButtons();
		GUI.skin.label.fontSize = (int)((float)Screen.height * this.statsFontSize);
		GUI.skin.label.alignment = 0;
		float fieldHeight = (float)Screen.height * this.statsFontSize * this.statsLineHeight;
		float goldFieldHeight = (float)Screen.height * this.statsGoldFontSize * this.statsLineHeight;
		float boxWidth = (float)Screen.width * this.statsWidth;
		float offX = (float)Screen.width * this.statsPosition.x;
		float offY = (float)Screen.height * this.statsPosition.y + num5;
		if (this.currentPage == EndGameScreen.Page.GoldRewardDetails)
		{
			if (this.gameType.isLimited())
			{
				this.OnGUI_drawLimitedProgress(goldFieldHeight, offX, boxWidth);
			}
			else
			{
				this.OnGUI_drawGoldRewards(goldFieldHeight, num5);
			}
		}
		else if (this.currentPage == EndGameScreen.Page.Statistics)
		{
			this.OnGUI_drawStatistics(fieldHeight, offX, offY, boxWidth, num5);
		}
		this.OnGUI_drawMatchEnjoyment(fieldHeight);
		Rect text;
		text..ctor((-1f + this.avatarXFactor) * (float)Screen.width, (float)Screen.height * (0.88f + num), (float)Screen.height * 0.4f, (float)Screen.height * 0.1f);
		this.OnGUI_drawNames(text, num);
		this.OnGUI_drawRatingUpdate(text);
		GUI.color = color;
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x00032A50 File Offset: 0x00030C50
	private void OnGUI_drawBalloon(Rect rect)
	{
		if (this.balloonAlpha <= 0f)
		{
			return;
		}
	}

	// Token: 0x060004A6 RID: 1190 RVA: 0x00032A70 File Offset: 0x00030C70
	private void OnGUI_drawAvatars(float rankedYOffset1)
	{
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.gameOverAlpha);
		float num = (float)Screen.height * 0.8f;
		float num2 = num * 567f / 991f;
		Rect rect;
		rect..ctor((-1f + this.avatarXFactor) * (float)Screen.width, (float)Screen.height * (0.1f + rankedYOffset1), num2, num);
		Rect rect2;
		rect2..ctor((float)Screen.width - rect.x - rect.width, rect.y, rect.width, rect.height);
		this.leftAvatar.draw(rect, false);
		this.rightAvatar.draw(rect2, true);
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x00032B48 File Offset: 0x00030D48
	private void setPage(EndGameScreen.Page newPage)
	{
		Log.warning(string.Concat(new object[]
		{
			"Changing from ",
			this.currentPage,
			" -> ",
			newPage,
			" @ ",
			Time.time
		}));
		if (newPage == this.currentPage)
		{
			return;
		}
		this.pageTime = 0f;
		EndGameScreen.Page oldPage = this.currentPage;
		this.currentPage = newPage;
		this.onPageChanged(oldPage, newPage);
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x000050C9 File Offset: 0x000032C9
	private bool hasBonusRewards()
	{
		return this.gameMode == GameMode.Play && this.endGameStatistics.hasCardReward(this.isWinner);
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x000050EB File Offset: 0x000032EB
	private static bool isGoldPage(EndGameScreen.Page page)
	{
		return page == EndGameScreen.Page.GoldCountup;
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x00032BD0 File Offset: 0x00030DD0
	private void onPageChanged(EndGameScreen.Page oldPage, EndGameScreen.Page newPage)
	{
		if (oldPage == EndGameScreen.Page.Rewards)
		{
			this.removeReward();
		}
		if (EndGameScreen.isGoldPage(oldPage))
		{
			Object.Destroy(this.goldPile);
		}
		if (EndGameScreen.isGoldPage(newPage))
		{
			if (this.goldPile != null)
			{
				Object.Destroy(this.goldPile);
			}
			this.goldPile = base.gameObject.AddComponent<GoldCountup>();
			this.goldPile.setOffsetScale(new Rect(0f, (float)Screen.height * -0.2f, 0.9f, 0.9f));
			Gui3D gui3D = new Gui3D(UnityUtil.getFirstOrtographicCamera());
			gui3D.setBaseDepth(9f);
			this.goldPile.init(gui3D, EndGameScreen.totalReward(this.endGameStatistics, this.playerColor), Layers.BattleModeUI, newPage == EndGameScreen.Page.GoldCountup);
			this.goldPile.run();
		}
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x00032CAC File Offset: 0x00030EAC
	private EndGameScreen.Page nextPage()
	{
		EndGameScreen.Page page = this.currentPage + 1;
		if (page == EndGameScreen.Page.Last)
		{
			return EndGameScreen.Page.Statistics;
		}
		return page;
	}

	// Token: 0x060004AC RID: 1196 RVA: 0x00032CCC File Offset: 0x00030ECC
	private void OnGUI_drawButtons()
	{
		if (this.currentPage <= EndGameScreen.Page.Rewards)
		{
			return;
		}
		EndGameScreen.Page page = this.nextPage();
		string text = "<No text for state " + page + ">";
		if (page == EndGameScreen.Page.Statistics)
		{
			text = "Match info";
		}
		if (EndGameScreen.isGoldPage(page))
		{
			text = "Show gold";
		}
		if (page == EndGameScreen.Page.GoldRewardDetails)
		{
			text = "Gold details";
		}
		Message nextGameMessage = this.getNextGameMessage();
		bool flag = this.isWinner && this.endGameStatistics.nextGame != null;
		bool flag2 = !this.isWinner && this.endGameStatistics.lossGame != null;
		bool flag3 = nextGameMessage != null || (this.isWinner && flag) || flag2;
		float num = (float)(Screen.width / 2) - (float)Screen.height * 0.15f;
		float num2 = (float)Screen.height * 0.62f;
		float num3 = 0.6f;
		GUI.color = new Color(num3, num3, num3, 1f);
		if (App.GUI.Button(GeomUtil.scaleCentered(new Rect(num, num2, (float)Screen.height * 0.3f, (float)Screen.height * 0.05f), 0.75f, 0.9f), text))
		{
			this.setPage(this.nextPage());
		}
		GUI.color = Color.white;
		if (App.GUI.Button(GeomUtil.scaleCentered(new Rect(num, (float)Screen.height * 0.91f, (float)Screen.height * 0.3f, (float)Screen.height * 0.05f), 0.9f), "Back to lobby"))
		{
			this.GoToLobby();
		}
		if (flag3)
		{
			string text2 = (!flag2 && (flag || !EndGameScreen.isPlayAgain(nextGameMessage))) ? "Play next" : "Play again";
			if (App.GUI.Button(GeomUtil.scaleCentered(new Rect(num, (float)Screen.height * 0.84f, (float)Screen.height * 0.3f, (float)Screen.height * 0.05f), 1.25f, 1.2f), text2))
			{
				App.SceneValues.lobby = new SceneValues.SV_Lobby();
				if (flag)
				{
					App.SceneValues.lobby.nextGame = this.endGameStatistics.nextGame;
				}
				else if (flag2)
				{
					App.SceneValues.lobby.nextGame = this.endGameStatistics.lossGame;
				}
				else
				{
					App.SceneValues.lobby.enterBattleMessage = nextGameMessage;
				}
				this.GoToLobby();
			}
		}
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x00032F68 File Offset: 0x00031168
	private Message getNextGameMessage()
	{
		if (this.gameMode != GameMode.Play)
		{
			return null;
		}
		if (!this.isWinner)
		{
			return null;
		}
		if (this.gameType.isTutorial())
		{
			int num = (App.TutorialChallengeInfo == null) ? -1 : App.TutorialChallengeInfo.NextTutorialId(this.refId);
			if (num > 0)
			{
				return new PlaySinglePlayerTutorialMessage(num);
			}
		}
		return null;
	}

	// Token: 0x060004AE RID: 1198 RVA: 0x000050F1 File Offset: 0x000032F1
	private static bool isPlayAgain(Message msg)
	{
		return !(msg is PlaySinglePlayerTutorialMessage) && !(msg is PlaySinglePlayerTowerMatchMessage);
	}

	// Token: 0x060004AF RID: 1199 RVA: 0x00032FD0 File Offset: 0x000311D0
	private void OnGUI_drawStatistics(float fieldHeight, float offX, float offY, float boxWidth, float extraOffsetY)
	{
		offY -= 0.05f * (float)Screen.height;
		TileColor c = this.playerColor;
		TileColor color = c.otherColor();
		GameStatistics gameStatistics = this.endGameStatistics.getGameStatistics(this.playerColor);
		GameStatistics gameStatistics2 = this.endGameStatistics.getGameStatistics(color);
		string[] array = new string[]
		{
			"Time used",
			"Damage dealt (idols)",
			"Damage dealt (units)",
			"Units played",
			"Spells + Enchantments",
			"Scrolls drawn",
			"Highest damage"
		};
		TimeSpan timeSpan = TimeSpan.FromMilliseconds((double)gameStatistics.totalMs);
		string text = ((int)timeSpan.TotalMinutes).ToString().PadLeft(2, '0') + ":" + timeSpan.Seconds.ToString().PadLeft(2, '0');
		TimeSpan timeSpan2 = TimeSpan.FromMilliseconds((double)gameStatistics2.totalMs);
		string text2 = ((int)timeSpan2.TotalMinutes).ToString().PadLeft(2, '0') + ":" + timeSpan2.Seconds.ToString().PadLeft(2, '0');
		Color textColor = GUI.skin.label.normal.textColor;
		string[] array2 = new string[]
		{
			text,
			text2,
			gameStatistics.idolDamage.ToString(),
			gameStatistics2.idolDamage.ToString(),
			gameStatistics.unitDamage.ToString(),
			gameStatistics2.unitDamage.ToString(),
			gameStatistics.unitsPlayed.ToString(),
			gameStatistics2.unitsPlayed.ToString(),
			(gameStatistics.spellsPlayed + gameStatistics.enchantmentsPlayed).ToString(),
			(gameStatistics2.spellsPlayed + gameStatistics2.enchantmentsPlayed).ToString(),
			gameStatistics.scrollsDrawn.ToString(),
			gameStatistics2.scrollsDrawn.ToString(),
			gameStatistics.mostDamageUnit.ToString(),
			gameStatistics2.mostDamageUnit.ToString()
		};
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.statsGoldAlpha);
		TextAnchor alignment = GUI.skin.label.alignment;
		GUI.skin.label.alignment = 4;
		GUI.skin.label.normal.textColor = this.statsColor;
		GUI.skin.label.alignment = 4;
		GUI.Label(new Rect(offX + boxWidth * 0.36f, offY - fieldHeight, boxWidth * 0.35f, fieldHeight), this.leftName);
		GUI.Label(new Rect(offX + boxWidth * 0.69f, offY - fieldHeight, boxWidth * 0.35f, fieldHeight), this.rightName);
		GUI.skin.label.alignment = alignment;
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			Color color2 = GUI.color;
			if (num % 2 == 0)
			{
				GUI.color = new Color(color2.r, color2.g, color2.b, color2.a * 0.4f);
			}
			else
			{
				GUI.color = new Color(color2.r, color2.g, color2.b, color2.a * 0.2f);
			}
			GUI.Box(new Rect(offX - boxWidth * 0.05f, offY + fieldHeight * (float)i, boxWidth * 1.1f, fieldHeight * 1.1f), string.Empty);
			GUI.color = color2;
			GUI.Label(new Rect(offX, offY + fieldHeight * (float)i, boxWidth, fieldHeight), array[i]);
			num++;
		}
		GUI.skin.label.alignment = 4;
		for (int j = 0; j < 2; j++)
		{
			for (int k = j; k < array2.Length; k += 2)
			{
				GUI.Label(new Rect(offX + boxWidth * 0.4f + (float)j * boxWidth * 0.33f, offY + fieldHeight * (float)(k / 2), boxWidth * 0.25f, fieldHeight), array2[k]);
			}
		}
		GUI.skin.label.normal.textColor = new Color(0.85f, 0.75f, 0.5f);
		long num2 = gameStatistics.totalMs + gameStatistics2.totalMs;
		TimeSpan timeSpan3 = TimeSpan.FromMilliseconds((double)num2);
		string text3 = ((int)timeSpan3.TotalMinutes).ToString().PadLeft(2, '0') + ":" + timeSpan3.Seconds.ToString().PadLeft(2, '0');
		GUI.Label(new Rect(0f, offY + fieldHeight * (float)(array.Length + 1), (float)Screen.width, fieldHeight * 2f), string.Concat(new object[]
		{
			"Match length: ",
			text3,
			"   -   Round: ",
			this.round
		}));
		GUI.skin.label.alignment = alignment;
		if (this.ratingUpdate != null)
		{
			offY += fieldHeight * (float)array.Length;
			string[] array3 = new string[]
			{
				"Rating"
			};
			int[] array4 = new int[]
			{
				this.ratingUpdate.whiteNewRating,
				this.ratingUpdate.whiteRatingChange,
				this.ratingUpdate.blackNewRating,
				this.ratingUpdate.blackRatingChange
			};
			int num3 = 0;
			int num4 = 2;
			if (this.playerColor == TileColor.black)
			{
				int num5 = num3;
				num3 = num4;
				num4 = num5;
			}
			GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.ratingAlpha);
			GUI.skin.label.normal.textColor = this.statsColor;
			for (int l = 0; l < array3.Length; l++)
			{
				Color color3 = GUI.color;
				if (num % 2 == 0)
				{
					GUI.color = new Color(color3.r, color3.g, color3.b, color3.a * 0.4f);
				}
				else
				{
					GUI.color = new Color(color3.r, color3.g, color3.b, color3.a * 0.2f);
				}
				GUI.Box(new Rect(offX - boxWidth * 0.05f, offY + fieldHeight * (float)l, boxWidth * 1.1f, fieldHeight * 1.1f), string.Empty);
				GUI.color = color3;
				GUI.Label(new Rect(offX, offY + fieldHeight * (float)l, boxWidth, fieldHeight), array3[l]);
				num++;
			}
			GUI.skin.label.alignment = 4;
			string text4 = ((int)(this.ratingAlpha * 255f)).ToString("X2").ToLower();
			string text5 = "<color=#998877" + text4 + ">";
			for (int m = 0; m < 2; m++)
			{
				int num6 = (m != 0) ? num4 : num3;
				for (int n = 0; n < array4.Length / 2; n += 2)
				{
					int num7 = num6 + n;
					GUI.Label(new Rect(offX + boxWidth * 0.4f + (float)m * boxWidth * 0.33f, offY + fieldHeight * 0f, boxWidth * 0.25f, fieldHeight), string.Concat(new string[]
					{
						array4[num7].ToString(),
						text5,
						"    ",
						(array4[num7 + 1] <= 0) ? "-" : "+",
						Mathf.Abs(array4[num7 + 1]).ToString(),
						"</color>"
					}));
				}
			}
			GUI.skin.label.alignment = alignment;
		}
		GUI.skin.label.normal.textColor = textColor;
	}

	// Token: 0x060004B0 RID: 1200 RVA: 0x00033800 File Offset: 0x00031A00
	private void OnGUI_drawLimitedProgress(float goldFieldHeight, float offX, float boxWidth)
	{
		if (this.limitedProgress == null)
		{
			return;
		}
		float num = (float)Screen.height * this.statsGoldYPos * 0.9f;
		float num2 = goldFieldHeight * 0.75f;
		float num3 = (float)Screen.height * 0.025f;
		Rect rect;
		rect..ctor(offX - boxWidth * 0.05f - num3, num - num3, boxWidth * 1.1f + 2f * num3, num2 * (float)(this.limitedProgress.rewardLevels.Length + 2) + 2f * num3);
		new ScrollsFrame(rect).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.CENTER).Draw(new Color(1f, 1f, 1f, this.limitedAlpha * 0.25f));
		GUI.color = new Color(0.8f, 0.7f, 0.6f, this.limitedAlpha);
		GUI.skin.label.alignment = 1;
		GUI.skin.label.fontSize = (int)((float)Screen.height * this.statsGoldFontSize * 0.65f);
		GUI.Label(new Rect((float)Screen.width * 0.34f, num + num2 * ((float)this.limitedProgress.rewardLevels.Length + 1.2f), (float)Screen.width * 0.32f, num2), string.Concat(new object[]
		{
			"Losses: ",
			this.limitedProgress.losses,
			" / ",
			this.limitedProgress.lossesMax
		}));
		GUI.skin.label.fontSize = (int)((float)Screen.height * this.statsGoldFontSize * 0.75f);
		GUI.skin.label.alignment = 3;
		GUI.color = new Color(1f, 1f, 1f, 0.2f * this.limitedAlpha);
		Rect rect2;
		rect2..ctor(offX - boxWidth * 0.05f, num, boxWidth * 1.1f, num2 * (float)(this.limitedProgress.rewardLevels.Length + 1));
		GUI.Box(rect2, string.Empty);
		GUI.color = new Color(1f, 1f, 1f, this.limitedAlpha);
		GUI.Label(new Rect((float)Screen.width * 0.34f, num + num2 * 0f, (float)Screen.width * 0.24f, num2), "Wins");
		GUI.Label(new Rect((float)Screen.width * 0.5f, num + num2 * 0f, (float)Screen.width * 0.24f, num2), "Gold");
		GUI.Label(new Rect((float)Screen.width * 0.61f, num + num2 * 0f, (float)Screen.width * 0.24f, num2), "Scrolls");
		GUI.color = new Color(1f, 0.8f, 0.6f, 0.4f * this.limitedAlpha);
		GUI.DrawTexture(new Rect(offX - boxWidth * 0.05f + 2f, num + num2 * 1f - 2f, boxWidth * 1.1f - 4f, 2f), ResourceManager.LoadTexture("ChatUI/white"));
		GUI.skin.label.fontSize = (int)((float)Screen.height * this.statsGoldFontSize * 0.6f);
		int num4 = 0;
		foreach (Reward reward in this.limitedProgress.rewardLevels)
		{
			string text = string.Empty;
			if (reward.common > 0)
			{
				text = text + reward.common + "C";
			}
			if (reward.uncommon > 0)
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					(reward.common <= 0) ? string.Empty : ", ",
					reward.uncommon,
					"U"
				});
			}
			if (reward.rare > 0)
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					(reward.common <= 0 && reward.uncommon <= 0) ? string.Empty : ", ",
					reward.rare,
					"R"
				});
			}
			if (num4 == this.limitedProgress.wins - 1 && this.isWinner)
			{
				GUI.color = new Color(1f, 0.7f, 0.4f, 0.3f * ((!this.limitedWinFading) ? this.limitedAlpha : this.previousRewardLevelAlpha));
				GUI.DrawTexture(new Rect(offX - boxWidth * 0.05f + 2f, num + num2 * (float)(num4 + 1), boxWidth * 1.1f - 4f, num2), ResourceManager.LoadTexture("ChatUI/white"));
				GUI.color = new Color(1f, 1f, 1f, this.limitedAlpha);
			}
			if (num4 == this.limitedProgress.wins)
			{
				float num5 = 0f;
				if (this.isWinner)
				{
					GUI.color = new Color(1f + this.saturator, 0.7f + this.saturator, 0.4f + this.saturator, (0.3f + this.saturator * 0.25f) * ((!this.limitedWinFading) ? 0f : this.currentRewardLevelAlpha));
					num5 = this.saturator * (float)Screen.height * 0.0075f;
				}
				else
				{
					GUI.color = new Color(1f, 0.7f, 0.4f, 0.3f * this.limitedAlpha);
				}
				GUI.DrawTexture(new Rect(offX - boxWidth * 0.05f + 2f - num5 * 2f, num + num2 * (float)(num4 + 1) - num5, boxWidth * 1.1f - 4f + num5 * 4f, num2 + num5 * 2f), ResourceManager.LoadTexture("ChatUI/white"));
				GUI.color = new Color(1f, 1f, 1f, this.limitedAlpha);
			}
			GUI.color = new Color(1f, 1f, 1f, this.limitedAlpha);
			GUI.Label(new Rect((float)Screen.width * 0.34f, num + num2 * (float)(num4 + 1), (float)Screen.width * 0.24f, num2), string.Empty + num4);
			GUI.color = new Color(1f, 0.9f, 0.4f, this.limitedAlpha);
			GUI.Label(new Rect((float)Screen.width * 0.5f, num + num2 * (float)(num4 + 1), (float)Screen.width * 0.24f, num2), string.Empty + reward.gold);
			GUI.Label(new Rect((float)Screen.width * 0.61f, num + num2 * (float)(num4 + 1), (float)Screen.width * 0.24f, num2), text);
			GUI.color = new Color(1f, 1f, 1f, this.limitedAlpha);
			num4++;
		}
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x00033F64 File Offset: 0x00032164
	private static int totalReward(EMEndGame g, TileColor playerColor)
	{
		GameRewardStatistics gameRewardStatistics = g.getGameRewardStatistics(playerColor);
		TowerLevel challengeInfo = g.challengeInfo;
		return gameRewardStatistics.totalReward + ((challengeInfo == null || !challengeInfo.isCompleted) ? 0 : challengeInfo.goldReward);
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x00033FA4 File Offset: 0x000321A4
	private void OnGUI_drawGoldRewards(float goldFieldHeight, float extraOffsetY)
	{
		TowerLevel challengeInfo = this.endGameStatistics.challengeInfo;
		GameRewardStatistics gameRewardStatistics = this.endGameStatistics.getGameRewardStatistics(this.playerColor);
		List<string> list = new List<string>();
		list.Add((!this.isWinner) ? "Match" : "Victory");
		list.Add("Completion");
		list.Add("Idols");
		if (challengeInfo != null && challengeInfo.isCompleted)
		{
			if (this.gameType == GameType.SP_TUTORIAL)
			{
				list.Add("Tutorial");
			}
			else
			{
				list.Add("Trial");
			}
		}
		if (gameRewardStatistics.betReward > 0)
		{
			list.Add("Bet");
		}
		list.Add("Total");
		TextAnchor alignment = GUI.skin.label.alignment;
		GUI.skin.label.alignment = 1;
		GUI.skin.label.normal.textColor = new Color(0.6f, 0.5f, 0.3f);
		if (this.gameType.isMultiplayerChallenge())
		{
			GUI.Label(new Rect((float)(Screen.width / 2) - (float)Screen.height * 0.4f, (float)Screen.height * 0.96f, (float)Screen.height * 0.8f, (float)Screen.height * 0.03f), "* Gold is only awarded for the first five challenge matches of the day");
		}
		GUI.skin.label.normal.textColor = new Color(0.8f, 0.75f, 0.6f);
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.statsGoldAlpha);
		GUI.skin.label.fontSize = (int)((float)Screen.height * this.statsGoldFontSize * 1.5f);
		GUIUtil.drawBorderedText(new Rect((float)Screen.width * 0.28f, extraOffsetY + (float)Screen.height * this.statsGoldYPos - ((list.Count != 5) ? 0f : (goldFieldHeight * 0.5f)), (float)Screen.width * 0.44f, goldFieldHeight * 1.3f), "Gold details" + ((!this.gameType.isMultiplayerChallenge()) ? string.Empty : "*"), new Color(0.95f, 0.8f, 0.5f));
		GUI.skin.label.alignment = 3;
		GUI.skin.label.fontSize = (int)((float)Screen.height * this.statsGoldFontSize);
		float num = (float)Screen.height * this.statsGoldYPos + goldFieldHeight * 1.5f;
		for (int i = 0; i < list.Count; i++)
		{
			GUIUtil.drawBorderedText(new Rect((float)Screen.width * 0.37f, extraOffsetY + num + goldFieldHeight * (float)i - ((list.Count != 5) ? 0f : (goldFieldHeight * 0.5f)), (float)Screen.width * 0.26f, goldFieldHeight), list[i], new Color(0.95f, 0.8f, 0.5f));
		}
		List<string> list2 = new List<string>();
		list2.Add(gameRewardStatistics.matchReward.ToString() + ((gameRewardStatistics.tierMatchReward <= 0) ? string.Empty : ("+" + gameRewardStatistics.tierMatchReward.ToString())));
		list2.Add(gameRewardStatistics.matchCompletionReward.ToString());
		list2.Add(gameRewardStatistics.idolsDestroyedReward.ToString());
		if (challengeInfo != null && challengeInfo.isCompleted)
		{
			string text = (challengeInfo.goldReward <= 0) ? string.Empty : challengeInfo.goldReward.ToString();
			list2.Add(text);
		}
		if (gameRewardStatistics.betReward > 0)
		{
			list2.Add(gameRewardStatistics.betReward.ToString());
		}
		list2.Add(EndGameScreen.totalReward(this.endGameStatistics, this.playerColor).ToString());
		GUI.skin.label.alignment = 5;
		for (int j = 0; j < list2.Count; j++)
		{
			GUIUtil.drawBorderedText(new Rect((float)Screen.width * 0.37f, extraOffsetY + num + goldFieldHeight * (float)j - ((list2.Count != 5) ? 0f : (goldFieldHeight * 0.5f)), (float)Screen.width * 0.26f, goldFieldHeight), list2[j] + " g", new Color(1f, 0.7f, 0.05f));
		}
		GUI.skin.label.alignment = alignment;
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x0003447C File Offset: 0x0003267C
	private void OnGUI_drawMatchEnjoyment(float fieldHeight)
	{
		if (this.gameType.isTutorial() || this.gameMode != GameMode.Play)
		{
			return;
		}
		if (this.currentPage <= EndGameScreen.Page.Rewards)
		{
			return;
		}
		float num = (float)Screen.height * 0.7f;
		float num2 = num + (float)Screen.height * 0.08f;
		GUI.skin.label.alignment = 4;
		GUI.skin.label.normal.textColor = new Color(0.85f, 0.76f, 0.5f);
		GUI.skin.label.fontSize = Screen.height / 40;
		GUI.Label(new Rect(0f, num, (float)Screen.width, fieldHeight * 2.5f), "Please rate your enjoyment of\nthis match (one is worst, five is best):");
		float num3 = (float)Screen.height * 0.05f;
		float num4 = num3 * 1.2f;
		GUI.skin = this.emptySkin;
		int num5 = 0;
		for (int i = 0; i < 5; i++)
		{
			Rect rect;
			rect..ctor((float)(Screen.width / 2) + num4 * (-2.5f + (float)i) + (num4 - num3) / 2f, num2, num3, num3);
			Rect rect2;
			rect2..ctor((float)(Screen.width / 2) + num4 * (-2.5f + (float)i), num2, num4, num3);
			GUI.DrawTexture(rect, ResourceManager.LoadTexture("BattleMode/GUI/star_bg"));
			if (rect2.Contains(GUIUtil.getScreenMousePos()))
			{
				num5 = i + 1;
			}
			if (GUI.Button(rect2, string.Empty))
			{
				this.matchEnjoyment = i + 1;
				App.Communicator.send(new MatchEnjoymentMessage(this.matchEnjoyment));
			}
		}
		if (GUI.Button(new Rect(0f, num2, (float)Screen.width, num3), string.Empty))
		{
			this.matchEnjoyment = 0;
			App.Communicator.send(new MatchEnjoymentMessage(0));
		}
		for (int j = 0; j < this.matchEnjoyment; j++)
		{
			GUI.DrawTexture(new Rect((float)(Screen.width / 2) + num4 * (-2.5f + (float)j) + (num4 - num3) / 2f, num2, num3, num3), ResourceManager.LoadTexture("BattleMode/GUI/star"));
		}
		for (int k = 0; k < num5; k++)
		{
			GUI.DrawTexture(new Rect((float)(Screen.width / 2) + num4 * (-2.5f + (float)k) + (num4 - num3) / 2f, num2, num3, num3), ResourceManager.LoadTexture("BattleMode/GUI/star_highlight"));
		}
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x000346F0 File Offset: 0x000328F0
	private void OnGUI_drawNames(Rect text1, float rankedYOffset2)
	{
		GUI.skin = this.statsSkin;
		GUI.skin.label.fontSize = Screen.height / 16;
		GUI.skin.label.alignment = 4;
		GUI.skin.label.fontSize = Screen.height / (4 + Math.Max(12, this.leftName.Length));
		GUIUtil.drawBorderedText(text1, this.leftName, new Color(1f, 0.95f, 0.8f));
		GUI.skin.label.fontSize = Screen.height / (4 + Math.Max(12, this.rightName.Length));
		GUIUtil.drawBorderedText(new Rect((float)Screen.width - text1.x - text1.width, text1.y, text1.width, text1.height), this.rightName, new Color(1f, 0.95f, 0.8f));
	}

	// Token: 0x060004B5 RID: 1205 RVA: 0x000347F0 File Offset: 0x000329F0
	private void OnGUI_drawRatingUpdate(Rect text1)
	{
		if (this.ratingUpdate == null)
		{
			return;
		}
		int num;
		int num2;
		int num3;
		int rankIndex;
		if (this.playerColor == TileColor.white)
		{
			num = this.ratingUpdate.whiteNewRank;
			num2 = this.ratingUpdate.whiteOldRank;
			num3 = this.ratingUpdate.whiteWinsForRank;
			rankIndex = this.ratingUpdate.blackNewRank;
			int num4 = this.ratingUpdate.blackOldRank;
			int num5 = this.ratingUpdate.blackWinsForRank;
		}
		else
		{
			num = this.ratingUpdate.blackNewRank;
			num2 = this.ratingUpdate.blackOldRank;
			num3 = this.ratingUpdate.blackWinsForRank;
			rankIndex = this.ratingUpdate.whiteNewRank;
			int num4 = this.ratingUpdate.whiteOldRank;
			int num5 = this.ratingUpdate.whiteWinsForRank;
		}
		GUI.skin.label.fontSize = Screen.height / 32;
		float num6 = (float)Screen.height * 0.07f;
		float num7 = (float)Screen.height * 0.07f * 512f / 183f;
		Rect r;
		r..ctor(text1.x + (float)Screen.height * 0.2f - num7 / 2f, (float)Screen.height * 0.841f, num7, num6);
		Rect rect;
		rect..ctor((float)Screen.width - r.x - r.width, r.y, r.width, r.height);
		for (int i = 0; i < 2; i++)
		{
			if (i != 0 || num != num2)
			{
				Rank rank = Ranks.Get((i != 0) ? num : num2);
				if (rank != null)
				{
					float kx = 1f;
					if (num2 != num)
					{
						if (this.rankSlamAlpha > 0f && this.rankSlamAlpha < 1f)
						{
							GUI.color = new Color(1f, 1f, 1f, 0.5f - this.rankSlamAlpha / 2f);
							float kx2 = 1f + 1.2f * this.rankSlamAlpha;
							Rect rect2 = GeomUtil.scaleCentered(r, kx2);
							GUI.DrawTexture(rect2, ResourceManager.LoadTexture(rank.imagePath));
						}
						GUI.color = new Color(1f, 1f, 1f, (i != 0) ? this.rankAlpha : (1f - this.rankAlpha));
						if (i == 1)
						{
							kx = 4f - 3f * this.rankAlpha;
						}
					}
					Rect rect3 = GeomUtil.scaleCentered(r, kx);
					GUI.DrawTexture(rect3, ResourceManager.LoadTexture(rank.imagePath));
					GUI.skin.label.fontSize = Screen.height / 28;
					Rect rect4;
					rect4..ctor(text1.x, r.yMax, (float)Screen.height * 0.4f, (float)Screen.height * 0.03f);
					GUIUtil.drawBorderedText(rect4, rank.name, new Color(1f, 0.95f, 0.8f));
					if (num2 != num && i == 1)
					{
						GUI.skin.label.fontSize = Screen.height / 42;
						GUIUtil.drawBorderedText(new Rect(rect4.x, rect4.yMax - (float)Screen.height * 0.005f, rect4.width, rect4.height), (num <= num2) ? "Rank lost!" : "Rank increased!", new Color(1f, 0.95f, 0.8f));
					}
					GUI.color = Color.white;
				}
				else
				{
					GUIUtil.drawBorderedText(new Rect(text1.x, r.y + r.height / 2f, (float)Screen.height * 0.4f, (float)Screen.height * 0.07f), "Win " + num3 + " more matches\nto see your rank.", new Color(1f, 0.95f, 0.8f));
				}
			}
		}
		Rank rank2 = Ranks.Get(rankIndex);
		if (rank2 != null)
		{
			GUI.DrawTexture(rect, ResourceManager.LoadTexture(rank2.imagePath));
			GUI.skin.label.fontSize = Screen.height / 28;
			GUIUtil.drawBorderedText(new Rect((float)Screen.width - text1.x - (float)Screen.height * 0.4f, rect.yMax, (float)Screen.height * 0.4f, (float)Screen.height * 0.03f), rank2.name, new Color(1f, 0.95f, 0.8f));
		}
		else
		{
			GUIUtil.drawBorderedText(new Rect((float)Screen.width - text1.x - (float)Screen.height * 0.4f, r.y + r.height / 2f, (float)Screen.height * 0.4f, (float)Screen.height * 0.07f), "This player is not\ncurrently ranked.", new Color(1f, 0.95f, 0.8f));
		}
	}

	// Token: 0x060004B6 RID: 1206 RVA: 0x00034CFC File Offset: 0x00032EFC
	private void GoToLobby()
	{
		if (this.gameMode == GameMode.Play)
		{
			App.Communicator.send(new LeaveGameMessage());
			App.LobbyMenu.ClearQueueStatuses();
		}
		if (this.gameMode == GameMode.Spectate)
		{
			this.specComm.send(new SpectateLeaveGameMessage());
		}
		this.done = true;
		App.Communicator.joinLobby(true);
	}

	// Token: 0x060004B7 RID: 1207 RVA: 0x0000510C File Offset: 0x0000330C
	public void SetRatingUpdateMessage(RatingUpdateMessage msg)
	{
		this.ratingUpdate = msg;
		base.StopCoroutine("FadeInRatingUpdates");
		base.StartCoroutine("FadeInRatingUpdates");
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x0000512C File Offset: 0x0000332C
	public void SetRewardProgressLimitedMessage(RewardProgressLimitedMessage msg)
	{
		this.limitedProgress = msg;
		base.StopCoroutine("FadeInLimited");
		base.StartCoroutine("FadeInLimited");
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x0000514C File Offset: 0x0000334C
	public void SetGameRefId(int refId)
	{
		this.refId = refId;
	}

	// Token: 0x060004BA RID: 1210 RVA: 0x00034D60 File Offset: 0x00032F60
	private IEnumerator FadeInStats()
	{
		this.tValue = 0f;
		while (this.tValue < 1f)
		{
			this.tValue += Time.deltaTime * 2f;
			if (this.tValue > 1f)
			{
				this.tValue = 1f;
			}
			this.gameOverOverlayAlpha = this.tValue * this.tValue * (3f - 2f * this.tValue);
			this.gameOverAlpha = Mathf.Pow(this.tValue, 2f);
			yield return null;
		}
		if (this.isWinner)
		{
			this.tValue = 0f;
			while (this.tValue < 1f)
			{
				this.tValue += Time.deltaTime * 2f;
				if (this.tValue > 1f)
				{
					this.tValue = 1f;
				}
				this.victorySlamAlpha = this.tValue;
				yield return null;
			}
		}
		yield return new WaitForSeconds(0.3f);
		this.tValue = 0f;
		while (this.tValue < 1f)
		{
			this.tValue += Time.deltaTime * 3.5f;
			if (this.tValue > 1f)
			{
				this.tValue = 1f;
			}
			this.headerYFactor = this.tValue * this.tValue * (3f - 2f * this.tValue);
			yield return null;
		}
		yield return new WaitForSeconds(0f);
		this.showingStats = true;
		this.tValue = 0f;
		while (this.tValue < 1f)
		{
			this.tValue += Time.deltaTime;
			if (this.tValue > 1f)
			{
				this.tValue = 1f;
			}
			this.statsGoldAlpha = this.tValue * this.tValue * (3f - 2f * this.tValue);
			yield return null;
		}
		yield break;
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x00034D7C File Offset: 0x00032F7C
	private IEnumerator FadeInLimited()
	{
		while (!this.showingStats)
		{
			yield return null;
		}
		this.tValue = 0f;
		while (this.tValue < 1f)
		{
			this.tValue += Time.deltaTime;
			if (this.tValue > 1f)
			{
				this.tValue = 1f;
			}
			this.limitedAlpha = this.tValue * this.tValue * (3f - 2f * this.tValue);
			yield return null;
		}
		if (this.isWinner)
		{
			yield return new WaitForSeconds(0.4f);
			Debug.Log("Fading limited!");
			this.limitedWinFading = true;
			float t = 0f;
			while (t < 1f)
			{
				t += Time.deltaTime * 6.5f;
				if (t > 1f)
				{
					t = 1f;
				}
				this.previousRewardLevelAlpha = 1f - t * t * (3f - 2f * t);
				yield return null;
			}
			t = 0f;
			while (t < 1f)
			{
				t += Time.deltaTime * 6.5f;
				if (t > 1f)
				{
					t = 1f;
				}
				this.currentRewardLevelAlpha = t * t * (3f - 2f * t);
				yield return null;
			}
			t = 0f;
			while (t < 1f)
			{
				t += Time.deltaTime * 4.5f;
				if (t > 1f)
				{
					t = 1f;
				}
				this.saturator = Mathf.Sin(t * 3.1415927f);
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x00034D98 File Offset: 0x00032F98
	private IEnumerator MoveInAvatars()
	{
		yield return new WaitForSeconds(0.75f);
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime;
			if (t > 1f)
			{
				t = 1f;
			}
			this.avatarXFactor = t * t * (3f - 2f * t);
			yield return null;
		}
		yield break;
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x00034DB4 File Offset: 0x00032FB4
	private IEnumerator FadeInRatingUpdates()
	{
		while (!this.showingStats)
		{
			yield return null;
		}
		this.ratingT = 0f;
		while (this.ratingT < 1f)
		{
			this.ratingT += Time.deltaTime / 1.7f;
			if (this.ratingT > 1f)
			{
				this.ratingT = 1f;
			}
			this.ratingAlpha = this.ratingT * this.ratingT * (3f - 2f * this.ratingT);
			yield return null;
		}
		yield return new WaitForSeconds(0.5f);
		App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_end_turn_01");
		this.ratingT = 0f;
		while (this.ratingT < 1f)
		{
			this.ratingT += Time.deltaTime / 0.3f;
			if (this.ratingT > 1f)
			{
				this.ratingT = 1f;
			}
			this.rankAlpha = Mathf.Pow(this.ratingT, 2f);
			yield return null;
		}
		this.ratingT = 0f;
		while (this.ratingT < 1f)
		{
			this.ratingT += Time.deltaTime / 0.4f;
			if (this.ratingT > 1f)
			{
				this.ratingT = 1f;
			}
			this.rankSlamAlpha = this.ratingT;
			yield return null;
		}
		yield break;
	}

	// Token: 0x040002F9 RID: 761
	private bool isWinner;

	// Token: 0x040002FA RID: 762
	private bool inited;

	// Token: 0x040002FB RID: 763
	private bool done;

	// Token: 0x040002FC RID: 764
	private TileColor playerColor;

	// Token: 0x040002FD RID: 765
	private int refId;

	// Token: 0x040002FE RID: 766
	private Texture2D titleTex;

	// Token: 0x040002FF RID: 767
	private Avatar leftAvatar;

	// Token: 0x04000300 RID: 768
	private Avatar rightAvatar;

	// Token: 0x04000301 RID: 769
	[SerializeField]
	private Vector2 statsPosition = new Vector2(0.13f, 0.5f);

	// Token: 0x04000302 RID: 770
	[SerializeField]
	private float statsFontSize = 0.027f;

	// Token: 0x04000303 RID: 771
	[SerializeField]
	private float statsWidth = 0.2f;

	// Token: 0x04000304 RID: 772
	[SerializeField]
	private float statsGoldFontSize = 0.027f;

	// Token: 0x04000305 RID: 773
	private float statsGoldYPos = 0.25f;

	// Token: 0x04000306 RID: 774
	[SerializeField]
	private float statsLineHeight = 1.1f;

	// Token: 0x04000307 RID: 775
	[SerializeField]
	private Color statsColor = Color.white;

	// Token: 0x04000308 RID: 776
	[SerializeField]
	private RenderTexture cardRenderTexture;

	// Token: 0x04000309 RID: 777
	private GUIStyle balloonLabelStyle;

	// Token: 0x0400030A RID: 778
	private EMEndGame endGameStatistics;

	// Token: 0x0400030B RID: 779
	private RatingUpdateMessage ratingUpdate;

	// Token: 0x0400030C RID: 780
	private RewardProgressLimitedMessage limitedProgress;

	// Token: 0x0400030D RID: 781
	private GUISkin statsSkin;

	// Token: 0x0400030E RID: 782
	private GUISkin labelSkin;

	// Token: 0x0400030F RID: 783
	private GUISkin emptySkin;

	// Token: 0x04000310 RID: 784
	private GUIStyle rewardButtonStyle;

	// Token: 0x04000311 RID: 785
	private string leftName;

	// Token: 0x04000312 RID: 786
	private string rightName;

	// Token: 0x04000313 RID: 787
	private GameType gameType;

	// Token: 0x04000314 RID: 788
	private GameMode gameMode;

	// Token: 0x04000315 RID: 789
	private MiniCommunicator specComm;

	// Token: 0x04000316 RID: 790
	private int round;

	// Token: 0x04000317 RID: 791
	private EndGameScreen.Page currentPage;

	// Token: 0x04000318 RID: 792
	private float pageTime;

	// Token: 0x04000319 RID: 793
	private GoldCountup goldPile;

	// Token: 0x0400031A RID: 794
	private int matchEnjoyment;

	// Token: 0x0400031B RID: 795
	private IGui _gui = new UnityGui2D();

	// Token: 0x0400031C RID: 796
	private CardReveal cardReveal;

	// Token: 0x0400031D RID: 797
	private Gui3D ggui;

	// Token: 0x0400031E RID: 798
	private float rewardStringIndexTime;

	// Token: 0x0400031F RID: 799
	private string rewardString = "Oho! Congratulations. Here's a scroll for your efforts.";

	// Token: 0x04000320 RID: 800
	private float balloonAlpha;

	// Token: 0x04000321 RID: 801
	private float gameOverOverlayAlpha;

	// Token: 0x04000322 RID: 802
	private float gameOverAlpha;

	// Token: 0x04000323 RID: 803
	private float victorySlamAlpha;

	// Token: 0x04000324 RID: 804
	private float statsGoldAlpha;

	// Token: 0x04000325 RID: 805
	private float avatarXFactor;

	// Token: 0x04000326 RID: 806
	private float headerYFactor;

	// Token: 0x04000327 RID: 807
	private bool showingStats;

	// Token: 0x04000328 RID: 808
	private float tValue;

	// Token: 0x04000329 RID: 809
	private float limitedAlpha;

	// Token: 0x0400032A RID: 810
	private float previousRewardLevelAlpha;

	// Token: 0x0400032B RID: 811
	private float currentRewardLevelAlpha;

	// Token: 0x0400032C RID: 812
	private float saturator;

	// Token: 0x0400032D RID: 813
	private bool limitedWinFading;

	// Token: 0x0400032E RID: 814
	private float ratingAlpha;

	// Token: 0x0400032F RID: 815
	private float rankAlpha;

	// Token: 0x04000330 RID: 816
	private float rankSlamAlpha;

	// Token: 0x04000331 RID: 817
	private float ratingT;

	// Token: 0x02000078 RID: 120
	private enum Page
	{
		// Token: 0x04000333 RID: 819
		None,
		// Token: 0x04000334 RID: 820
		Reveal,
		// Token: 0x04000335 RID: 821
		Rewards,
		// Token: 0x04000336 RID: 822
		GoldCountup,
		// Token: 0x04000337 RID: 823
		Statistics,
		// Token: 0x04000338 RID: 824
		GoldRewardDetails,
		// Token: 0x04000339 RID: 825
		Last
	}
}
