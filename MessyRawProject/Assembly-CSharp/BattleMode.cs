using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gui;
using Irrelevant.Assets;
using UnityEngine;

// Token: 0x02000042 RID: 66
public class BattleMode : AbstractCommListener, IAllowNotification, IGame, ICardActivatorCallback, CardViewFactory, SiftCallback, Unit.ICallback, ITutorialBlockCheck, IBattleModeUICallback, IHandManagerCallback, iCardRule, iEffect
{
	// Token: 0x060002A2 RID: 674 RVA: 0x000232B0 File Offset: 0x000214B0
	private void Start()
	{
		this.historyBlinker = new Blinker();
		ComponentAttacher<TutorialTicker.Tag> componentAttacher = this.historyBlinker;
		List<TutorialTicker.Tag> list = new List<TutorialTicker.Tag>();
		list.Add(TutorialTicker.Tag.Blink_Unit);
		componentAttacher.update(list);
		Application.targetFrameRate = 60;
		this.audioScript = App.AudioScript;
		base.name = "_BattleMode";
		Camera.main.transparencySortMode = 2;
		App.Popups.KillCurrentPopup();
		App.LobbyMenu.SetEnabled(false);
		SceneValues.SV_BattleMode battleMode = App.SceneValues.battleMode;
		this.gameMode = battleMode.gameMode;
		App.ChatUI.SetIsBattling(this.gameMode == GameMode.Play);
		if (App.SceneValues.battleMode.msg != null)
		{
			this.gameId = battleMode.msg.gameId;
		}
		string downloadDataPath = OsSpec.getDownloadDataPath();
		if (!Directory.Exists(downloadDataPath))
		{
			Directory.CreateDirectory(downloadDataPath);
		}
		this.comm = App.Communicator;
		if (this.isSpectateOrReplay())
		{
			App.ChatUI.Show(false);
			App.ChatUI.SetMode(OnlineState.SPECTATE);
			App.ChatUI.SetLocked(false, (float)Screen.height * 0.25f);
		}
		if (this.isReplay())
		{
			base.StartCoroutine(this.startReplay(battleMode.gameLog));
		}
		if (this.gameMode == GameMode.Spectate)
		{
			IpPort address = battleMode.address;
			if (!address.Equals(App.Communicator.getAddress()))
			{
				this.comm = App.SceneValues.battleMode.specCommGameObject.GetComponent<MiniCommunicator>();
				this.comm.addListener(this);
				this.comm.setEnabled(true, true);
			}
			string spectateRoomName = ChatRooms.GetSpectateRoomName(this.gameId);
			App.ArenaChat.RoomEnter(spectateRoomName);
			App.ArenaChat.ChatRooms.SetCurrentRoom(spectateRoomName);
			App.ArenaChat.ChatRooms.ChatMessage(CliResponseMessage.Fake("You are spectating match #" + this.gameId));
		}
		if (this.gameMode == GameMode.Play)
		{
			App.ChatUI.SetEnabled(false);
			List<Room> list2 = new List<Room>(App.ArenaChat.ChatRooms.GetAllRooms());
			foreach (Room room in list2)
			{
				App.ArenaChat.RoomExit(room, true);
			}
		}
		this.comm.addListener(this);
		this.endGameScreen = base.gameObject.GetComponent<EndGameScreen>();
		this.shake = base.gameObject.AddComponent<ScreenShake>();
		this.battleModeSkin = (GUISkin)ResourceManager.Load("_GUISkins/BattleMode");
		this.battleChat = (GUISkin)ResourceManager.Load("_GUISkins/BattleChat");
		GUISkin guiskin = (GUISkin)ResourceManager.Load("_GUISkins/BattleUI");
		this.battleUISkin = ScriptableObject.CreateInstance<GUISkin>();
		this.battleUISkin.box = new GUIStyle(guiskin.box);
		this.battleUISkin.label = new GUIStyle(guiskin.label);
		this.battleUIBoxStyleSelected = new GUIStyle(this.battleUISkin.box);
		this.battleUIBoxStyleSelected.normal.background = ResourceManager.LoadTexture("BattleUI/battlegui_lowopacitybox_highlight");
		this.regularUI = (GUISkin)ResourceManager.Load("_GUISkins/RegularUI");
		this.tutorialLabelStyle = new GUIStyle(this.regularUI.label);
		this.labelSkin = ScriptableObject.CreateInstance<GUISkin>();
		this.labelSkin.label.wordWrap = true;
		this.labelSkin.label.font = (Font)ResourceManager.Load("Fonts/arial", typeof(Font));
		this.labelSkin.label.fontSize = 16;
		this.labelSkin.label.normal.background = ResourceManager.LoadTexture("BattleMode/blackDot");
		this.labelSkin.label.normal.textColor = new Color(1f, 1f, 1f, 1f);
		this.labelSkin.label.fontStyle = 1;
		this.menu = base.gameObject.AddComponent<GUIBattleModeMenu>();
		this.menu.init(this.gameMode, this.comm);
		this.handManager = base.GetComponent<HandManager>();
		this.uiGui = new Gui3D(this.uiCamera);
		string[] array = new string[]
		{
			"Battle_Ruins",
			"Battle_Suspense",
			"Battle_Ancients"
		};
		this.battleMusic = "Music/" + array[Random.Range(0, array.Length)];
		this.audioScript.PlayMusic(this.battleMusic);
		this.clock = new GUIClock(this.battleUISkin);
		this.showUnitStats = App.Config.settings.battle.show_unit_stats;
		if (App.SceneValues.battleMode.msg != null)
		{
			this.handleMessage(App.SceneValues.battleMode.msg);
		}
		if (App.SceneValues.battleMode.end != null)
		{
			this.gotoMockEndScreen();
		}
		this.btnRestartStyle = GUIUtil.createButtonStyle("BattleUI/Replay/restart");
		this.btnFfStyle = GUIUtil.createButtonStyle("BattleUI/Replay/fastforward");
		this.btnPlayStyle = GUIUtil.createButtonStyle("BattleUI/Replay/play");
		this.btnPauseStyle = GUIUtil.createButtonStyle("BattleUI/Replay/pause");
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x00023814 File Offset: 0x00021A14
	private IEnumerator startReplay(string gameLog)
	{
		if (!this.isReplay())
		{
			yield break;
		}
		yield return null;
		yield return base.StartCoroutine(ICommListenerDispatcher.enumeratorDispatch(this, gameLog));
		yield break;
	}

	// Token: 0x060002A4 RID: 676 RVA: 0x00003F5A File Offset: 0x0000215A
	private void restartReplay()
	{
		if (!this.isReplay())
		{
			return;
		}
		SceneLoader.loadScene("_BattleModeView");
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x00023840 File Offset: 0x00021A40
	public override void OnDestroy()
	{
		App.ChatUI.SetMode(OnlineState.LOBBY);
		App.ChatUI.SetLocked(false);
		if (this.comm != null)
		{
			this.comm.removeListener(this);
			if (this.comm != App.Communicator)
			{
				Object.Destroy(this.comm);
			}
		}
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x00003F72 File Offset: 0x00002172
	private bool isSpectate()
	{
		return this.gameMode == GameMode.Spectate;
	}

	// Token: 0x060002A7 RID: 679 RVA: 0x00003F7D File Offset: 0x0000217D
	private bool isReplay()
	{
		return this.gameMode == GameMode.Replay;
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x00003F88 File Offset: 0x00002188
	private bool isSpectateOrReplay()
	{
		return this.isSpectate() || this.isReplay();
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x000238A0 File Offset: 0x00021AA0
	private void sendBattleRequest(Message msg)
	{
		if (this.isSpectateOrReplay())
		{
			return;
		}
		if (!this._hasActiveGame && !(msg is GameNotRequired))
		{
			Log.info("No game, not sending msg: " + msg.msg);
			return;
		}
		if (this.comm.hasServerRole(ServerRole.GAME))
		{
			this.comm.send(msg);
		}
	}

	// Token: 0x060002AA RID: 682 RVA: 0x00023904 File Offset: 0x00021B04
	private void setupBackground(string background, long gameId)
	{
		foreach (GameObject gameObject in this.bgObjects)
		{
			Object.Destroy(gameObject);
		}
		BackgroundData background2 = App.Config.settings.battle.getBackground();
		if (background2 == null && background != null)
		{
			int backgroundIdFor = BackgroundData.getBackgroundIdFor(background);
			if (backgroundIdFor >= 0)
			{
				background2 = BackgroundData.getBackground((long)backgroundIdFor);
			}
		}
		if (background2 == null)
		{
			int backgroundIdFor2 = BackgroundData.getBackgroundIdFor(gameId);
			background2 = BackgroundData.getBackground((long)backgroundIdFor2);
		}
		this.currentBgData = background2;
		Unit.shadowColor = background2.shadowColor;
		foreach (Idol idol in this.getAllIdolsCopy())
		{
			idol.setShadowColor(background2.shadowColor);
		}
		int num = 2;
		foreach (WorldImage worldImage in background2.getImages())
		{
			Material material = new Material(ResourceManager.LoadShader("Transparent/Diffuse"));
			material.mainTextureOffset = new Vector2(0.001f, 0.001f);
			material.mainTextureScale = new Vector2(0.998f, 0.998f);
			GameObject gameObject2 = PrimitiveFactory.createPlane(false);
			Texture2D mainTexture = ResourceManager.LoadTexture(worldImage.filename);
			material.renderQueue = num++;
			gameObject2.renderer.material = material;
			gameObject2.renderer.material.mainTexture = mainTexture;
			gameObject2.transform.position = worldImage.pos;
			gameObject2.transform.localScale = worldImage.scale;
			gameObject2.transform.eulerAngles = worldImage.rot;
			this.bgObjects.Add(gameObject2);
		}
		foreach (LightInfo lightInfo in background2.getLights())
		{
			GameObject gameObject3 = new GameObject("Light");
			gameObject3.AddComponent<Light>();
			gameObject3.light.color = lightInfo.color;
			gameObject3.light.cullingMask = ~(1 << BattleMode.LAYER_NOLIGHT);
			gameObject3.transform.position = lightInfo.pos;
			gameObject3.light.intensity = 0.44f;
			gameObject3.light.type = 2;
			gameObject3.light.range = (float)lightInfo.range;
			gameObject3.light.shadows = 2;
			this.bgObjects.Add(gameObject3);
		}
		if (background2.name == "Cave1")
		{
			GameObject gameObject4 = new GameObject();
			gameObject4.AddComponent<MeshRenderer>();
			gameObject4.AddComponent<EffectPlayer>();
			EffectPlayer component = gameObject4.GetComponent<EffectPlayer>();
			component.init("torch1", 1, this, 50, new Vector3(0.5f, 0.5f, 0.5f), true, string.Empty, 0);
			gameObject4.transform.position = new Vector3(-3.4f, 0.75f, 5.6f);
			gameObject4.transform.eulerAngles = new Vector3(51f, 270f, 0f);
			GameObject gameObject5 = new GameObject();
			gameObject5.AddComponent<MeshRenderer>();
			gameObject5.AddComponent<EffectPlayer>();
			EffectPlayer component2 = gameObject5.GetComponent<EffectPlayer>();
			component2.init("torch1", 1, this, 50, new Vector3(0.5f, 0.5f, 0.5f), true, string.Empty, 0);
			gameObject5.transform.position = new Vector3(-3.4f, 0.75f, -5.64f);
			gameObject5.transform.eulerAngles = new Vector3(51f, 270f, 0f);
			this.bgObjects.Add(gameObject4);
			this.bgObjects.Add(gameObject5);
		}
		Camera.main.transform.eulerAngles = new Vector3(51f, 270f, 0f);
		Camera.main.transform.position = new Vector3(9.67f, 11.15f, 0f);
		this.shake.init(Camera.main);
	}

	// Token: 0x060002AB RID: 683 RVA: 0x00023D9C File Offset: 0x00021F9C
	private void setupBoard(GameInfoMessage m)
	{
		try
		{
			this.boardSet = true;
			Camera.main.transform.eulerAngles = new Vector3(51f, 270f, 0f);
			Camera.main.transform.position = new Vector3(9.67f, 11.15f, 0f);
			this.GUIObject = new GameObject();
			this.GUIObject.transform.parent = Camera.main.transform;
			this.GUIObject.transform.localPosition = new Vector3(0f, 0f, Camera.main.transform.position.y - 0.3f);
			this.GUIObject.name = "GUIObject";
			GameObject gameObject = PrimitiveFactory.createPlane();
			gameObject.transform.position = new Vector3(0f, 0f, 0f);
			gameObject.transform.localScale = new Vector3(4f, 4f, 4f);
			gameObject.renderer.enabled = false;
			Color gradientColor = (this.currentBgData == null) ? ColorUtil.FromInts(20, 20, 20) : this.currentBgData.gradientColor;
			this.battleUI = base.GetComponent<BattleModeUI>();
			AvatarInfo rightAvatarInfo = m.getAvatar(this.rightColor);
			if (this.gameType.isTutorial())
			{
				rightAvatarInfo = AvatarInfo.CustomBuilder().Head(1000000001);
				this.showUnitStats = true;
			}
			this.battleUI.Init(this, this, this.gameMode, m.getAvatar(this.leftColor), rightAvatarInfo, gradientColor);
			if (this.gameMode == GameMode.Spectate)
			{
				this.battleUI.ShowLeftHandSize();
			}
			this.battleUI.MoveInAvatars();
			if (m.phase == EndPhaseMessage.Phase.Init)
			{
				this.endPhase(EndPhaseMessage.Phase.Init);
			}
			this.goField = new GameObject("GameField");
			this.goLeftBoard = UnityUtil.addChild(this.goField, new GameObject("Left Board"));
			this.goLeftIdols = UnityUtil.addChild(this.goLeftBoard, new GameObject("Idols"));
			this.goLeftTiles = UnityUtil.addChild(this.goLeftBoard, new GameObject("Tiles"));
			this.goRightBoard = UnityUtil.addChild(this.goField, new GameObject("Right Board"));
			this.goRightIdols = UnityUtil.addChild(this.goRightBoard, new GameObject("Idols"));
			this.goRightTiles = UnityUtil.addChild(this.goRightBoard, new GameObject("Tiles"));
			for (int i = 0; i < 5; i++)
			{
				List<Tile> list = new List<Tile>();
				for (int j = 0; j < 3; j++)
				{
					GameObject gameObject2 = UnityUtil.addChild(this.goLeftTiles, new GameObject(i + "_" + j));
					float num = 1.15f;
					float num2 = -2f + (float)i * 1f * 0.84f * num;
					float num3 = -0.7f - (float)j * num - (float)(i % 2) * (0.5f * num);
					Transform transform = (Transform)Object.Instantiate(this.tile, new Vector3(num2, -0.11f, num3), Quaternion.identity);
					transform.parent = gameObject2.transform;
					transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
					transform.transform.localScale *= 0.98f * num;
					transform.renderer.enabled = false;
					transform.name = "mesh";
					Tile component = transform.gameObject.GetComponent<Tile>();
					component.init(this, true, this.leftColor, i, j);
					list.Add(component);
				}
				this.leftTileArr.Add(list);
				this.allTiles.AddRange(list);
			}
			for (int k = 0; k < 5; k++)
			{
				for (int l = 0; l < 3; l++)
				{
					float num4 = 1.15f;
					float num5 = -2f + (float)k * 1f * 0.84f * num4;
					float num6 = -0.7f - (float)l * num4 - (float)(k % 2) * (0.5f * num4);
					GameObject gameObject3 = PrimitiveFactory.createPlane(false);
					gameObject3.transform.parent = this.leftTileArr[k][l].transform.parent;
					gameObject3.name = "graphics";
					gameObject3.transform.position = new Vector3(num5, -0.05f, num6);
					gameObject3.transform.localScale = new Vector3(0.103f, 0.098f, 0.1138f);
					gameObject3.transform.localScale *= 0.98f * num4;
					gameObject3.transform.eulerAngles = new Vector3(0f, 90f, 0f);
					Texture2D mainTexture = ResourceManager.LoadTexture("BattleMode/Tiles/hexagon_standard");
					Material material = new Material(ResourceManager.LoadShader("Transparent/Diffuse"));
					material.mainTexture = mainTexture;
					material.renderQueue = 3900;
					gameObject3.renderer.material = material;
					this.leftTileArr[k][l].setReference(gameObject3);
				}
			}
			for (int n = 0; n < 5; n++)
			{
				List<Tile> list2 = new List<Tile>();
				for (int num7 = 0; num7 < 3; num7++)
				{
					GameObject gameObject4 = UnityUtil.addChild(this.goRightTiles, new GameObject(n + "_" + num7));
					float num8 = 1.15f;
					float num9 = -2f + (float)n * 1f * 0.84f * num8;
					float num10 = 0.7f + (float)num7 * num8 - (float)(n % 2) * (-0.5f * num8);
					Transform transform2 = (Transform)Object.Instantiate(this.tile, new Vector3(num9, -0.11f, num10), Quaternion.identity);
					transform2.parent = gameObject4.transform;
					transform2.localScale = new Vector3(0.55f, 0.55f, 0.55f);
					transform2.localScale *= 0.98f * num8;
					transform2.renderer.enabled = false;
					transform2.name = "mesh";
					Tile component2 = transform2.gameObject.GetComponent<Tile>();
					component2.init(this, false, this.rightColor, n, num7);
					list2.Add(component2);
				}
				this.rightTileArr.Add(list2);
				this.allTiles.AddRange(list2);
			}
			for (int num11 = 0; num11 < 5; num11++)
			{
				for (int num12 = 0; num12 < 3; num12++)
				{
					float num13 = 1.15f;
					float num14 = -2f + 1f * (float)num11 * 0.84f * num13;
					float num15 = 0.7f + (float)num12 * num13 - (float)(num11 % 2) * (-0.5f * num13);
					GameObject gameObject5 = PrimitiveFactory.createPlane(false);
					gameObject5.transform.parent = this.rightTileArr[num11][num12].transform.parent;
					gameObject5.name = "graphics";
					gameObject5.transform.position = new Vector3(num14, -0.05f, num15);
					gameObject5.transform.localScale = new Vector3(-0.103f, 0.098f, -0.1138f);
					gameObject5.transform.localScale *= 0.98f * num13;
					gameObject5.transform.eulerAngles = new Vector3(0f, 90f, 0f);
					Texture2D mainTexture2 = ResourceManager.LoadTexture("BattleMode/Tiles/hexagon_standard");
					Material material2 = new Material(ResourceManager.LoadShader("Transparent/Diffuse"));
					material2.mainTexture = mainTexture2;
					material2.renderQueue = 3900;
					gameObject5.renderer.material = material2;
					this.rightTileArr[num11][num12].setReference(gameObject5);
				}
			}
			for (int num16 = 0; num16 < 5; num16++)
			{
				this.leftIdolsArr.Add(this.createIdol(m.getIdolInfo(this.leftColor, num16), m.getIdolType(this.leftColor, num16)));
				this.rightIdolsArr.Add(this.createIdol(m.getIdolInfo(this.rightColor, num16), m.getIdolType(this.rightColor, num16)));
			}
			if (m.phase == EndPhaseMessage.Phase.Init && !this.gameType.isTutorial())
			{
				this.setBattleText("BattleStart", new Vector3(-7f, 0.75f, -5f), 2f);
			}
		}
		catch (Exception ex)
		{
			Log.error("Error in setUpBoard(): " + ex);
		}
	}

	// Token: 0x060002AC RID: 684 RVA: 0x000246CC File Offset: 0x000228CC
	private Idol createIdol(IdolInfo idolInfo, short typeId)
	{
		bool flag = this.isLeftColor(idolInfo.color);
		int num = (!flag) ? -1 : 1;
		GameObject gameObject = new GameObject("Idol_" + idolInfo.position);
		UnityUtil.addChild((!flag) ? this.goRightIdols : this.goLeftIdols, gameObject);
		GameObject gameObject2 = UnityUtil.addChild(gameObject, PrimitiveFactory.createPlane());
		gameObject2.transform.position = new Vector3(-2.15f + (float)idolInfo.position * 0.84f * 1.15f, 0.26f, (float)num * -4.6f);
		gameObject2.transform.localScale = new Vector3(1.618f * (float)num * 0.074f, 1f, 0.12f);
		gameObject2.transform.eulerAngles = new Vector3(39f, 90f, 0f);
		gameObject2.name = "Graphics";
		Idol idol = gameObject2.AddComponent<Idol>();
		string resource = (!flag) ? "ps_cryptwalker_" : "ps_northern_ward_";
		if (typeId >= 0)
		{
			IdolType idolType = IdolTypeManager.getInstance().get(typeId);
			if (idolType != null)
			{
				resource = idolType.filename;
			}
		}
		idol.init(this, flag, idolInfo.position, idolInfo.hp, idolInfo.maxHp, resource, this.showUnitStats);
		return idol;
	}

	// Token: 0x060002AD RID: 685 RVA: 0x0002482C File Offset: 0x00022A2C
	private IEnumerator fadeInTile(GameObject g, float delay, float alphaSpeed)
	{
		Color c = new Color(1f, 1f, 1f, 0f);
		g.renderer.material.color = c;
		while (!this._hasStartedGame)
		{
			yield return null;
		}
		delay += 3f;
		float st = App.Clocks.battleModeClock.getTime();
		while (App.Clocks.battleModeClock.getTime() - st < delay)
		{
			yield return null;
		}
		st = App.Clocks.battleModeClock.getTime();
		while (c.a < 0.1f)
		{
			c.a = Mathf.Min(0.1f, (App.Clocks.battleModeClock.getTime() - st) * alphaSpeed);
			g.renderer.material.color = c;
			yield return null;
		}
		yield break;
	}

	// Token: 0x060002AE RID: 686 RVA: 0x00003F9E File Offset: 0x0000219E
	public void setMouseLabel(string headLine, string bodyText)
	{
		if (headLine == this.mouseLabelHead && bodyText == this.mouseLabelBody)
		{
			return;
		}
		this.mouseLabelHead = headLine;
		this.mouseLabelBody = bodyText;
		this.mouseLabelTimer = Time.time;
	}

	// Token: 0x060002AF RID: 687 RVA: 0x00003FDC File Offset: 0x000021DC
	public void effectAnimDone(EffectPlayer effect, bool loop)
	{
		DefaultIEffectCallback.instance().effectAnimDone(effect, loop);
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x00003FEA File Offset: 0x000021EA
	public void locator(EffectPlayer effect, AnimLocator loc)
	{
		if (loc.name.ToLower() == "effect")
		{
			this.effectDone();
		}
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x0000400C File Offset: 0x0000220C
	private TileColor getColor(bool isLeft)
	{
		return (!isLeft) ? this.rightColor : this.leftColor;
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x00024874 File Offset: 0x00022A74
	public void animateHistory(bool isLeft)
	{
		List<Transform> spellListFor = this.getSpellListFor(this.getColor(isLeft));
		float num = (float)((!isLeft) ? 1 : -1);
		for (int i = 0; i < spellListFor.Count - 1; i++)
		{
			float num2 = 2.43f - (float)(spellListFor.Count - 1 - i) * 0.01f;
			float num3 = num * (3f - (float)(spellListFor.Count - 1 - i) * 0.2f);
			spellListFor[i].GetComponent<CardView>().setRenderQueue(-spellListFor.Count + i);
			iTween.MoveTo(spellListFor[i].gameObject, iTween.Hash(new object[]
			{
				"x",
				num2,
				"y",
				5.53f,
				"z",
				num3,
				"easetype",
				iTween.EaseType.easeOutExpo,
				"time",
				0.6f
			}));
		}
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x00024984 File Offset: 0x00022B84
	private void markMoveTiles(TilePosition center, IEnumerable<TilePosition> positions)
	{
		foreach (TilePosition pos in positions)
		{
			this.markMoveTile(center, pos);
		}
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x000249D8 File Offset: 0x00022BD8
	private void markMoveTile(TilePosition center, TilePosition pos)
	{
		Tile tile = this.getTile(pos);
		if (TilePosition.areAdjacent(center, pos))
		{
			tile.setMoveAnim(center);
		}
		tile.setMarked(Tile.SelectionType.SelectedMove);
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x00004025 File Offset: 0x00002225
	private void deselectAllTiles()
	{
		this.markTiles(null, Tile.SelectionType.None);
		this.activeAbilityId = null;
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x00024A08 File Offset: 0x00022C08
	private void markTiles(IEnumerable<TilePosition> positions, Tile.SelectionType action)
	{
		foreach (Tile tile in this.allTiles)
		{
			tile.setMarked(Tile.SelectionType.None);
		}
		if (positions == null)
		{
			return;
		}
		foreach (TilePosition p in positions)
		{
			Tile tile2 = this.getTile(p);
			if (action == Tile.SelectionType.Selected)
			{
				tile2.setTileTargetColor(this.currentResourceColor);
			}
			tile2.setMarked(action);
		}
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x00024AC8 File Offset: 0x00022CC8
	private List<Tile> getTiles(IEnumerable<TilePosition> positions)
	{
		List<Tile> list = new List<Tile>();
		foreach (TilePosition p in positions)
		{
			list.Add(this.getTile(p));
		}
		return list;
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x00004036 File Offset: 0x00002236
	public Tile getTile(TilePosition p)
	{
		return this.getTile(p.color, p.row, p.column);
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x00004050 File Offset: 0x00002250
	public Tile getTile(TileColor color, int row, int column)
	{
		return this.getTileList(color)[row][column];
	}

	// Token: 0x060002BA RID: 698 RVA: 0x00004065 File Offset: 0x00002265
	public List<List<Tile>> getTileList(TileColor color)
	{
		return (!this.isLeftColor(color)) ? this.rightTileArr : this.leftTileArr;
	}

	// Token: 0x060002BB RID: 699 RVA: 0x00024B28 File Offset: 0x00022D28
	private List<Tile> getAllTiles()
	{
		List<Tile> list = new List<Tile>();
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				list.Add(this.getTile(TileColor.white, i, j));
				list.Add(this.getTile(TileColor.black, i, j));
			}
		}
		return list;
	}

	// Token: 0x060002BC RID: 700 RVA: 0x00004084 File Offset: 0x00002284
	public bool hasUnit(TilePosition p)
	{
		return this.getUnit(p) != null;
	}

	// Token: 0x060002BD RID: 701 RVA: 0x00024B80 File Offset: 0x00022D80
	public Unit getUnit(TilePosition p)
	{
		foreach (Unit unit in this.getUnitsFor(p.color))
		{
			if (p.Equals(unit.getTilePosition()))
			{
				return unit;
			}
		}
		return null;
	}

	// Token: 0x060002BE RID: 702 RVA: 0x00004093 File Offset: 0x00002293
	public Unit getUnit(TileColor color, int row, int column)
	{
		return this.getUnit(new TilePosition(color, row, column));
	}

	// Token: 0x060002BF RID: 703 RVA: 0x000040A3 File Offset: 0x000022A3
	public Idol getIdol(IdolInfo info)
	{
		return this.getIdol(info.color, info.position);
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x000040B7 File Offset: 0x000022B7
	public Idol getIdol(TileColor color, int row)
	{
		if (this.isLeftColor(color))
		{
			return this.leftIdolsArr[row];
		}
		return this.rightIdolsArr[row];
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x000040DE File Offset: 0x000022DE
	public Idol getOpposingIdol(TilePosition p)
	{
		return this.getIdol(p.color.otherColor(), p.row);
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x000040F7 File Offset: 0x000022F7
	public bool isLeftColor(TileColor color)
	{
		return color == this.leftColor;
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x00004102 File Offset: 0x00002302
	public bool isRightColor(TileColor color)
	{
		return color == this.rightColor;
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x0000410D File Offset: 0x0000230D
	public bool isPlayer(TileColor color)
	{
		return color == this.playerColor;
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x00004118 File Offset: 0x00002318
	private bool isPlayer(int profileId)
	{
		return profileId == App.MyProfile.ProfileInfo.id;
	}

	// Token: 0x060002C6 RID: 710 RVA: 0x0000412C File Offset: 0x0000232C
	public bool isRealPlayer(TileColor color)
	{
		return this.gameMode == GameMode.Play && this.isPlayer(color);
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x00004144 File Offset: 0x00002344
	public bool isRealOpponent(TileColor color)
	{
		return this.gameMode == GameMode.Play && this.isRightColor(color);
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x00024BF4 File Offset: 0x00022DF4
	public bool isTileInList(Tile t, List<List<Tile>> ll)
	{
		foreach (List<Tile> list in ll)
		{
			foreach (Tile tile in list)
			{
				if (t == tile)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x00024C98 File Offset: 0x00022E98
	public TilePosition getPosition(Tile t)
	{
		TileColor color = (!t.isLeft()) ? this.rightColor : this.leftColor;
		return new TilePosition(color, t.row(), t.column());
	}

	// Token: 0x060002CA RID: 714 RVA: 0x0000415C File Offset: 0x0000235C
	public List<Unit> getUnitsFor(TileColor color)
	{
		return (!this.isLeftColor(color)) ? this.rightUnitsArr : this.leftUnitsArr;
	}

	// Token: 0x060002CB RID: 715 RVA: 0x00024CD4 File Offset: 0x00022ED4
	private List<Unit> getAllUnitsCopy()
	{
		List<Unit> list = new List<Unit>(this.getUnitsFor(this.leftColor));
		list.AddRange(this.getUnitsFor(this.rightColor));
		return list;
	}

	// Token: 0x060002CC RID: 716 RVA: 0x00004065 File Offset: 0x00002265
	private List<List<Tile>> getTilesFor(TileColor color)
	{
		return (!this.isLeftColor(color)) ? this.rightTileArr : this.leftTileArr;
	}

	// Token: 0x060002CD RID: 717 RVA: 0x0000417B File Offset: 0x0000237B
	private List<Idol> getIdolsFor(TileColor color)
	{
		return (!this.isLeftColor(color)) ? this.rightIdolsArr : this.leftIdolsArr;
	}

	// Token: 0x060002CE RID: 718 RVA: 0x00024D08 File Offset: 0x00022F08
	private List<Idol> getAllIdolsCopy()
	{
		List<Idol> list = new List<Idol>(this.getIdolsFor(this.leftColor));
		list.AddRange(this.getIdolsFor(this.rightColor));
		return list;
	}

	// Token: 0x060002CF RID: 719 RVA: 0x0000419A File Offset: 0x0000239A
	private List<Transform> getSpellListFor(TileColor color)
	{
		return (!this.isLeftColor(color)) ? this.rightSpellListArr : this.leftSpellListArr;
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x000041B9 File Offset: 0x000023B9
	private List<EMSelectedTiles> getSelectionHistory(TileColor color)
	{
		return (!this.isLeftColor(this.activeColor)) ? this.rightTargets : this.leftTargets;
	}

	// Token: 0x060002D1 RID: 721 RVA: 0x00024D3C File Offset: 0x00022F3C
	public void tileClicked(Tile tile)
	{
		this.audioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
		if (this.handManager.GetSelectedCard() != null)
		{
			if (!this.tileSelector.pick(this.getPosition(tile)))
			{
				this.cancelScrollAndAbility();
				return;
			}
			this.lastTileClicked = tile;
			if (this.tileSelector.hasPickedAll())
			{
				if (this.tileSelector.isValid())
				{
					this.confirmPlayCard(this.handManager.GetSelectedCard(), this.tileSelector.getPickedTiles());
				}
				this.deselectAllTiles();
			}
			else
			{
				this.markTiles(this.tileSelector.getChoiceTiles(), Tile.SelectionType.Selected);
			}
		}
		else if (this.activeAbilityId != null)
		{
			if (!this.tileSelector.pick(this.getPosition(tile)))
			{
				this.deselectAllTiles();
				this.HideCardView();
				return;
			}
			this.lastTileClicked = tile;
			if (this.tileSelector.hasPickedAll())
			{
				if (this.tileSelector.isValid())
				{
					this.sendBattleRequest(new ActivateAbilityMessage(this.activeAbilityId, this.activeAbilityPosition, this.tileSelector.getPickedTiles()));
				}
				this.deselectAllTiles();
				this.HideCardView();
			}
			else
			{
				this.markTiles(this.tileSelector.getChoiceTiles(), Tile.SelectionType.Selected);
			}
		}
		else
		{
			TilePosition position = this.getPosition(tile);
			Unit unit = this.getUnit(position);
			if (unit != null)
			{
				FilterData data = new FilterData(this, unit.getCard());
				bool flag = this.tutorial.filter(new TilePosition[]
				{
					position
				}, data).Length > 0;
				this.showUnitRule(unit);
				if (flag && this.tutorial.allowMove())
				{
					this.ActivateTriggeredAbility(unit.getMoveAbility(), position);
				}
				if (flag && this.tutorial.onTileClicked(position, unit))
				{
					this.nextTutorialSlide();
				}
			}
		}
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x00024F20 File Offset: 0x00023120
	private static Vector3 getCenter(List<Tile> tiles)
	{
		Vector3 vector = Vector3.zero;
		foreach (Tile tile in tiles)
		{
			vector += tile.transform.position;
		}
		return (tiles.Count <= 0) ? vector : (vector * (1f / (float)tiles.Count));
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x00024FAC File Offset: 0x000231AC
	private void playCardEffect(TargetArea targetArea, TileColor color, TargetAreaAnimationType animationType, List<Tile> tiles)
	{
		int num = (color != this.leftColor) ? -1 : 1;
		if (targetArea != TargetArea.SEQUENTIAL)
		{
			if (targetArea != TargetArea.RADIUS_7)
			{
				this.effectDone();
			}
			else
			{
				foreach (Tile tile in tiles)
				{
					if (animationType == TargetAreaAnimationType.AREA_FREEZE)
					{
						Unit unit = this.getUnit(tile.tilePosition());
						if (unit != null)
						{
							unit.frostWind();
						}
					}
				}
				Vector3 center = BattleMode.getCenter(tiles);
				if (animationType == TargetAreaAnimationType.AREA_EXPLOSION)
				{
					this.createEffectAnimation("transp", 94000, center, 1, new Vector3(0.5f, 0.5f, 0.5f), new Vector3(51f, 270f, 0f), false);
					this.createEffectAnimation("MushroomExplosion1/back", 94001, center, 1, new Vector3(0.5f, 0.5f, 0.5f), new Vector3(51f, 270f, 0f), false);
					this.createEffectAnimation("MushroomExplosion1/mid", 94002, center, 1, new Vector3(0.5f, 0.5f, 0.5f), new Vector3(51f, 270f, 0f), false);
					this.createEffectAnimation("MushroomExplosion1/front", 94003, center, 1, new Vector3(0.5f, 0.5f, 0.5f), new Vector3(51f, 270f, 0f), false);
				}
				else
				{
					Vector3 baseScale = Vector3.one * 0.11f;
					this.createEffectAnimation("mass_enchant", 94002, center + new Vector3(0f, 0.71f, 0.03f), 1, baseScale, new Vector3(51f, 270f, 0f), false);
				}
				this.effectDone();
			}
		}
		else
		{
			for (int i = 0; i < tiles.Count; i++)
			{
				Tile tile2 = tiles[i];
				Vector3 position = tile2.transform.position;
				if (i == 0)
				{
					float num2 = 1f;
					this.createEffectAnimation("lightning-bolt", 94003, position, -num, new Vector3(num2, num2, num2), new Vector3(51f, 270f, 0f), false);
				}
				Unit unit2 = this.getUnit(color, tile2.row(), tile2.column());
				if (unit2 != null)
				{
					unit2.damage(2);
				}
				float delay = (float)i * 0.05f;
				this.createEffectAnimation("lightning-arc", 94003, position, 1, new Vector3(0.5f, 0.5f, 0.5f), new Vector3(51f, 270f, 0f), false, delay);
				this.createEffectAnimation("sparks", 94004, position, 1, new Vector3(0.5f, 0.5f, 0.5f), new Vector3(51f, 270f, 0f), false, delay);
			}
			this.effectDone();
		}
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x000252FC File Offset: 0x000234FC
	public static GameObject createEffectAnimation(iEffect callback, string asset, int renQ, Vector3 pos, int dirMod, Vector3 baseScale, Vector3 animRotation, bool loop)
	{
		return BattleMode.createEffectAnimation(callback, asset, renQ, pos, dirMod, baseScale, animRotation, loop, 0f);
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x00025320 File Offset: 0x00023520
	public static GameObject createEffectAnimation(iEffect callback, string asset, int renQ, Vector3 pos, int dirMod, Vector3 baseScale, Vector3 animRotation, bool loop, float delay)
	{
		if (callback == null)
		{
			callback = DefaultIEffectCallback.instance();
		}
		GameObject gameObject = new GameObject();
		gameObject.AddComponent<MeshRenderer>();
		EffectPlayer effectPlayer = gameObject.AddComponent<EffectPlayer>();
		effectPlayer.init(asset, dirMod, callback, renQ, baseScale, loop, string.Empty, 0);
		if (delay != 0f)
		{
			effectPlayer.startInSeconds(delay);
		}
		effectPlayer.layer = BattleMode.LAYER_NOLIGHT;
		effectPlayer.getAnimPlayer().waitForUpdate();
		gameObject.transform.position = pos;
		gameObject.transform.eulerAngles = animRotation;
		return gameObject;
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x000253A8 File Offset: 0x000235A8
	public GameObject createEffectAnimation(string asset, int renQ, Vector3 pos, int dirMod, Vector3 baseScale, Vector3 animRotation, bool loop)
	{
		return this.createEffectAnimation(asset, renQ, pos, dirMod, baseScale, animRotation, loop, 0f);
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x000252FC File Offset: 0x000234FC
	public GameObject createEffectAnimation(string asset, int renQ, Vector3 pos, int dirMod, Vector3 baseScale, Vector3 animRotation, bool loop, float delay)
	{
		return BattleMode.createEffectAnimation(this, asset, renQ, pos, dirMod, baseScale, animRotation, loop, 0f);
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x000253CC File Offset: 0x000235CC
	public void tileOver(TilePosition tp)
	{
		Tile tile = this.getTile(tp);
		TargetArea targetArea = TargetArea.None;
		tile.mark(Tile.SelectionType.Hover);
		Unit unit = this.getUnit(tp);
		CardType.Kind kind = CardType.Kind.NONE;
		if (unit != null)
		{
			unit.flashUnitStats();
			kind = unit.getPieceKind();
			targetArea = unit.getTargetArea();
		}
		if (this.handManager.GetSelectedCard() != null)
		{
			Card cardInfo = this.handManager.GetSelectedCard().getCardInfo();
			kind = cardInfo.getPieceKind();
			targetArea = cardInfo.getTargetArea();
		}
		if (targetArea != TargetArea.None)
		{
			TilePosition tp2 = tp;
			if (kind.isUnit())
			{
				tp2 = new TilePosition(tp.color.otherColor(), tp.row, 2 - tp.column);
			}
			this.selectTargetArea(targetArea, tp2);
		}
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x0002548C File Offset: 0x0002368C
	private void selectTargetArea(TargetArea targetArea, TilePosition tp)
	{
		Tile.SelectionType markedID = targetArea.selectionType();
		List<TilePosition> list = targetArea.getTargets(tp);
		if (targetArea == TargetArea.SEQUENTIAL)
		{
			list = new List<TilePosition>();
			foreach (IEnumerable<TilePosition> enumerable in this.activeTileGroups)
			{
				if (Enumerable.Contains<TilePosition>(enumerable, tp))
				{
					list.AddRange(enumerable);
				}
			}
		}
		List<Tile> tiles = this.getTiles(list);
		foreach (Tile tile in tiles)
		{
			tile.mark(markedID);
		}
	}

	// Token: 0x060002DA RID: 730 RVA: 0x000041DD File Offset: 0x000023DD
	private bool isValid(int row, int col)
	{
		return row >= 0 && row < 5 && col >= 0 && col < 3;
	}

	// Token: 0x060002DB RID: 731 RVA: 0x00025560 File Offset: 0x00023760
	public void tileOut(GameObject tile, int xPos, int zPos)
	{
		foreach (Tile tile2 in this.allTiles)
		{
			tile2.unmark();
		}
	}

	// Token: 0x060002DC RID: 732 RVA: 0x000255BC File Offset: 0x000237BC
	private CardView createCardObject(Card card, int profileId)
	{
		GameObject gameObject = PrimitiveFactory.createPlane();
		Vector3 position;
		position..ctor(this.GUIObject.transform.position.x + 1.52f, this.GUIObject.transform.position.y - 3.13f, this.GUIObject.transform.position.z + 5.55f);
		gameObject.transform.position = position;
		CardView cardView = gameObject.AddComponent<CardView>();
		cardView.name = "Card";
		cardView.overrideCost(this.getCostForCard(card, profileId));
		cardView.init(this, card, 0);
		cardView.setProfileId(profileId);
		cardView.applyHighResTexture();
		return cardView;
	}

	// Token: 0x060002DD RID: 733 RVA: 0x00025678 File Offset: 0x00023878
	private void updateChat(string chatString)
	{
		if (this.gameMode == GameMode.Spectate)
		{
			return;
		}
		if (this.gameMode == GameMode.Play && App.AudioScript.GetSoundToggle(AudioScript.ESoundToggle.CHAT))
		{
			App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_chat");
		}
		this.chatScroll = new Vector2(0f, float.PositiveInfinity);
		this.chatLastMessageSentAtTime = Time.time;
		this.chatLog = this.chatLog + ((!string.IsNullOrEmpty(this.chatLog)) ? "\n" : string.Empty) + chatString;
		if (!this.showChatInput)
		{
			this.setChatActive(true, false);
		}
	}

	// Token: 0x060002DE RID: 734 RVA: 0x000041FB File Offset: 0x000023FB
	public int GetCostForCard(Card card)
	{
		return this.getCostForCard(card, this.leftPlayer.profileId);
	}

	// Token: 0x060002DF RID: 735 RVA: 0x0000420F File Offset: 0x0000240F
	private int getCostForCard(Card card, int profileId)
	{
		return this.getPlayer(profileId).costs().get(card);
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x00025724 File Offset: 0x00023924
	private bool nextImportantEffectIs(params Type[] types)
	{
		if (this._tmpEffectId < 0)
		{
			return false;
		}
		for (int i = this._tmpEffectId + 1; i < this._tmpEffects.Count; i++)
		{
			EffectMessage effectMessage = this._tmpEffects[this._tmpEffectId];
			if (!(effectMessage is EMDelay) && !(effectMessage is EMStatsUpdate))
			{
				foreach (Type type in types)
				{
					if (effectMessage.GetType() == type)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x000257C0 File Offset: 0x000239C0
	private void addEffect(EffectMessage msg)
	{
		this.effects.Add(msg);
		if (msg is EMCardPlayed)
		{
			this.lastCardPlayed = ((EMCardPlayed)msg).card;
			this.lastActionType = BattleMode.ActionType.CardPlayed;
			this.opponentDelay(0.6f);
		}
		else if (msg is EMUnitActivateAbility)
		{
			EMUnitActivateAbility emunitActivateAbility = (EMUnitActivateAbility)msg;
			this.lastActionType = BattleMode.ActionType.ActivateAbility;
			this.lastActivatedAbility = emunitActivateAbility;
			if (!string.IsNullOrEmpty(emunitActivateAbility.name) && !emunitActivateAbility.isMoveLike())
			{
				this.effects.InsertBefore(new EMFeedback(msg), msg);
			}
			this.aiDelay();
		}
		else if (msg is EMSelectedTiles)
		{
			EMSelectedTiles emselectedTiles = (EMSelectedTiles)msg;
			emselectedTiles.lastActionType = this.lastActionType;
			emselectedTiles.lastAbility = this.lastActivatedAbility;
			if (this.lastActionType == BattleMode.ActionType.CardPlayed || (this.lastActionType == BattleMode.ActionType.ActivateAbility && !this.lastActivatedAbility.isMoveLike()))
			{
				this.effects.InsertBefore(new EMFeedback(msg), msg);
				this.aiDelay(0.35f);
			}
		}
		else if (msg is EMSummonUnit)
		{
			if (!this.nextImportantEffectIs(new Type[]
			{
				typeof(EMSummonUnit)
			}))
			{
				this.aiDelay(0.5f);
			}
		}
		else if (msg is EMEnchantUnit)
		{
			if (!this.nextImportantEffectIs(new Type[]
			{
				typeof(EMEnchantUnit)
			}))
			{
				this.aiDelay(0.5f);
			}
		}
		else if (msg is EMMoveUnit)
		{
			this.aiDelay();
		}
		else if (msg is EMSiftClose)
		{
			this.aiDelay(3f);
		}
		else if (msg is EMHandUpdate)
		{
			EMHandUpdate emhandUpdate = (EMHandUpdate)msg;
			this.maxScrollsForCycle = emhandUpdate.maxScrollsForCycle;
			if (this.isPlayer(emhandUpdate.profileId))
			{
				if (emhandUpdate.cards.Length > this.lastHandSize)
				{
					this.aiDelay(1.5f);
				}
				this.lastHandSize = emhandUpdate.cards.Length;
			}
			else
			{
				this.aiDelay(1.5f);
			}
		}
		else if (msg is EMCardSacrificed)
		{
			if (this.isReplay() && ((EMCardSacrificed)msg).color == this.playerColor)
			{
				this.addDelay(0.7f);
			}
		}
		else if (msg is EMTurnBegin)
		{
			this.lastActionType = BattleMode.ActionType.None;
			EMTurnBegin emturnBegin = (EMTurnBegin)msg;
			if (!emturnBegin.showText)
			{
				return;
			}
			if (this.isPlayer(emturnBegin.color))
			{
				this.setBattleText("YourTurn", new Vector3(-7f, 0.75f, -5f), 0.25f);
			}
			else
			{
				this.setBattleText("OpponentTurn", new Vector3(-7f, 0.75f, -5f), 0.25f);
			}
			this.addDelay((!this.isReplay()) ? 0.5f : 1.2f);
		}
		else if (msg is EMSurrenderEffect)
		{
			EMSurrenderEffect emsurrenderEffect = (EMSurrenderEffect)msg;
			for (int i = 0; i < 5; i++)
			{
				if (this.gameMode == GameMode.Play)
				{
					this.effects.PushFirst(new EMSurrenderIdolEffect(emsurrenderEffect.color, i));
				}
				else
				{
					this.effects.Add(new EMSurrenderIdolEffect(emsurrenderEffect.color, 4 - i));
				}
			}
		}
		else if (msg is EMEndGame)
		{
			this.effects.Remove(msg);
			this.effects.InsertAfter(msg, typeof(EMSurrenderIdolEffect));
		}
		else if (msg is EMMulliganDisabled && ((EMMulliganDisabled)msg).color == this.playerColor)
		{
			this.disableMulligan();
			this.handManager.DeselectCard();
		}
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x00004223 File Offset: 0x00002423
	private void disableMulligan()
	{
		this.mulliganAvailable = false;
		base.StartCoroutine(this.fadeMulliganButton(0f));
	}

	// Token: 0x060002E3 RID: 739 RVA: 0x00025BA8 File Offset: 0x00023DA8
	public override void handleMessage(Message msg)
	{
		if (msg is NewEffectsMessage)
		{
			this._tmpEffects = NewEffectsMessage.parseEffects(msg.getRawText());
			this._tmpEffectId = 0;
			while (this._tmpEffectId < this._tmpEffects.Count)
			{
				this.addEffect(this._tmpEffects[this._tmpEffectId]);
				this._tmpEffectId++;
			}
			this._tmpEffectId = -1;
		}
		if (msg is OkMessage)
		{
			OkMessage okMessage = (OkMessage)msg;
			if (okMessage.isType(typeof(JoinBattleMessage)))
			{
				this.sendBattleRequest(new ActiveResourcesMessage());
				this.sendBattleRequest(new GameStateMessage());
				this.sendBattleRequest(new HandViewMessage());
			}
		}
		if (msg is FailMessage)
		{
			FailMessage failMessage = (FailMessage)msg;
			if (failMessage.info.Contains("No game, msg"))
			{
				this._hasActiveGame = false;
			}
		}
		this._handleMessage(msg);
		base.handleMessage(msg);
	}

	// Token: 0x060002E4 RID: 740 RVA: 0x0000423E File Offset: 0x0000243E
	private void updateResources(PlayerAssets leftAssets, PlayerAssets rightAssets)
	{
		this.battleUI.UpdateData(leftAssets, this.isRealPlayer(this.leftColor), true);
		this.battleUI.UpdateData(rightAssets, false, false);
	}

	// Token: 0x060002E5 RID: 741 RVA: 0x00025CA4 File Offset: 0x00023EA4
	public override void onConnect(OnConnectData data)
	{
		if (!data.isConnect())
		{
			return;
		}
		if (this._serverRestarted)
		{
			try
			{
				iTween.Stop();
			}
			catch (Exception)
			{
			}
			App.Popups.ShowOk(new OkVoidCallback(), "server-restart", "Server restarted", string.Empty, "Ok");
			App.Communicator.joinLobby(true);
			return;
		}
		if (this.gameMode == GameMode.Spectate && App.Intention.wasLastIntention(Intention.Spectate))
		{
			Log.warning("Sending SpectateGame");
			this.comm.send(new SpectateGameMessage(this.gameId));
		}
		if (this.gameMode == GameMode.Play && App.Intention.wasLastIntention(Intention.Game))
		{
			this.sendBattleRequest(new JoinBattleMessage());
		}
	}

	// Token: 0x060002E6 RID: 742 RVA: 0x00025D7C File Offset: 0x00023F7C
	private void _handleMessage(Message msg)
	{
		if (msg is RatingUpdateMessage)
		{
			this.endGameScreen.SetRatingUpdateMessage((RatingUpdateMessage)msg);
		}
		if (msg is RewardProgressLimitedMessage)
		{
			this.endGameScreen.SetRewardProgressLimitedMessage((RewardProgressLimitedMessage)msg);
		}
		if (msg is ServerRestartMessage)
		{
			this._serverRestarted = true;
		}
		if (msg is NewPhaseMessage)
		{
			NewPhaseMessage newPhaseMessage = (NewPhaseMessage)msg;
			if (newPhaseMessage.phase == EndPhaseMessage.Phase.Main)
			{
				this.setSecondsLeft(this.roundTime);
			}
		}
		if (msg is SpectateUpdateCountMessage)
		{
			this.currentSpectators = ((SpectateUpdateCountMessage)msg).spectatorCount;
		}
		if (msg is SpectateGetSpectatorsMessage)
		{
			SpectateGetSpectatorsMessage spectateGetSpectatorsMessage = (SpectateGetSpectatorsMessage)msg;
			if (!this.spectatorListIsShowing)
			{
				this.spectatorListIsShowing = true;
			}
			this.spectators.Clear();
			this.spectators.AddRange(spectateGetSpectatorsMessage.spectators);
		}
		if (msg is CardInfoMessage)
		{
			CardInfoMessage cardInfoMessage = (CardInfoMessage)msg;
			if (!cardInfoMessage.hasEnoughResources)
			{
				this.tileSelector = new TileSelector();
				return;
			}
			List<TilePosition[]> list = new List<TilePosition[]>();
			for (int i = 0; i < cardInfoMessage.data.selectableTiles.Count; i++)
			{
				TilePosition[] pos = cardInfoMessage.data.selectableTiles.tileSets[i];
				TilePosition[] array = this.tutorial.filter(pos, new FilterData(this, cardInfoMessage.card));
				list.Add(array);
			}
			this.tileSelector = new TileSelector(list, cardInfoMessage.data.targetArea);
			if (list.Count > 0)
			{
				bool flag = this.tileSelector.getChoiceTiles().Length > 0;
				if (flag)
				{
					this.activeAbilityId = null;
					CardView selectedCard = this.handManager.GetSelectedCard();
					if (selectedCard != null)
					{
						ResourceType resource = selectedCard.getCardInfo().getCardType().getResource();
						this.currentResourceColor = ResourceColor.get(resource);
						selectedCard.onSelect();
					}
				}
				this.markTiles(this.tileSelector.getChoiceTiles(), Tile.SelectionType.Selected);
			}
			else
			{
				if (this.handManager.AddSelectedCardConfirm(CardConfirmType.Cast))
				{
					ResourceType resource2 = this.handManager.GetSelectedCard().getCardType().getResource();
					this.currentResourceColor = ResourceColor.get(resource2);
				}
				this.deselectAllTiles();
			}
			if (cardInfoMessage.data.targetAreaGroups != null)
			{
				this.activeTileGroups.Clear();
				foreach (TilePosition array2 in cardInfoMessage.data.targetAreaGroups)
				{
					this.activeTileGroups.Add(array2);
				}
			}
		}
		if (msg is AbilityInfoMessage)
		{
			AbilityInfoMessage abilityInfoMessage = (AbilityInfoMessage)msg;
			if (!abilityInfoMessage.isPlayable)
			{
				return;
			}
			List<TilePosition[]> list2 = new List<TilePosition[]>();
			for (int k = 0; k < abilityInfoMessage.data.selectableTiles.Count; k++)
			{
				TilePosition[] pos2 = abilityInfoMessage.data.selectableTiles.tileSets[k];
				list2.Add(this.tutorial.filter(pos2, new FilterData(this, this.getUnit(abilityInfoMessage.unitPosition), abilityInfoMessage.abilityId)));
			}
			this.tileSelector = new TileSelector(list2, abilityInfoMessage.data.targetArea);
			if (list2.Count > 0)
			{
				if (ActiveAbility.isMoveLike(abilityInfoMessage.abilityId))
				{
					this.markMoveTiles(abilityInfoMessage.unitPosition, this.tileSelector.getChoiceTiles());
				}
				else
				{
					this.markTiles(this.tileSelector.getChoiceTiles(), Tile.SelectionType.Selected);
				}
				this.activeAbilityId = abilityInfoMessage.abilityId;
			}
			else
			{
				this.sendBattleRequest(new ActivateAbilityMessage(abilityInfoMessage.abilityId, this.activeAbilityPosition));
			}
		}
		if (msg is ActiveResourcesMessage)
		{
			this.resTypes.Clear();
			this.resTypes.AddRange(((ActiveResourcesMessage)msg).types);
			if (this.gameType.hasWildResources() && this.resTypes.Count >= 2)
			{
				this.resTypes.Add(ResourceType.SPECIAL);
			}
		}
		if (msg is HandViewMessage)
		{
			HandViewMessage handViewMessage = (HandViewMessage)msg;
			this.setSubState(handViewMessage.type);
			this.maxScrollsForCycle = handViewMessage.maxScrollsForCycle;
			if (handViewMessage.type == BattleMode.SubState.Normal)
			{
				this.lastHandSize = handViewMessage.cards.Length;
				this.handManager.SetHand(handViewMessage.cards, this.battleUI.GetLeftPlayerResources(), this.leftPlayer.profileId);
			}
			else if (handViewMessage.type == BattleMode.SubState.Sift)
			{
				this.handleSiftUpdate(handViewMessage.cards);
			}
		}
		if (msg is GameInfoMessage)
		{
			if (this.boardSet)
			{
				return;
			}
			GameInfoMessage gameInfoMessage = (GameInfoMessage)msg;
			if (gameInfoMessage.customSettings != null)
			{
				this.customSettings = new Tags(gameInfoMessage.customSettings);
			}
			this.leftColor = (this.playerColor = gameInfoMessage.color);
			this.rightColor = this.leftColor.otherColor();
			this.playerProfileId = (this.leftPlayer.profileId = gameInfoMessage.getPlayerProfileId(this.leftColor));
			this.rightPlayer.profileId = gameInfoMessage.getPlayerProfileId(this.rightColor);
			this.leftPlayer.name = gameInfoMessage.getPlayerName(this.leftColor);
			this.rightPlayer.name = gameInfoMessage.getPlayerName(this.rightColor);
			this.roundTime = gameInfoMessage.roundTimerSeconds;
			this.gameType = gameInfoMessage.gameType;
			this.rewardForIdolKill = gameInfoMessage.rewardForIdolKill;
			this.gameId = gameInfoMessage.gameId;
			this.gameServerAddress = gameInfoMessage.nodeId;
			this.gameServerPort = gameInfoMessage.port;
			this.showClock = gameInfoMessage.hasTimer();
			this.alwaysWild = this.customSettings.has(RuleSettings.LimitlessWild);
			if (!this.boardSet)
			{
				if (this.gameType.isTutorial())
				{
					this.tutorialBlinker = new Blinker();
					this.tutorial = TutorialFactory.getTutorialForDeck(gameInfoMessage.refId);
					this.isScriptedTutorial = TutorialFactory.isScriptedTutorial(this.tutorial);
				}
				this.setupBackground(gameInfoMessage.background, gameInfoMessage.gameId);
				this.setupBoard(gameInfoMessage);
				this.handManager.Init(this, this);
				if (this.gameType.isTutorial())
				{
					this.handManager.setCardShaderName(Shaders.fnMilkBurn);
				}
			}
			if (!gameInfoMessage.phase.isPreGame())
			{
				this.sendBattleRequest(new ActiveResourcesMessage());
				this.sendBattleRequest(new GameStateMessage());
				this.sendBattleRequest(new HandViewMessage());
				this.requestedGameState = true;
			}
			this.endGameScreen.SetGameRefId(gameInfoMessage.refId);
			this.maxTierRewardMultiplier = gameInfoMessage.maxTierRewardMultiplier;
			this.tierRewardMultiplierDelta = gameInfoMessage.tierRewardMultiplierDelta;
		}
		if (msg is GameStateMessage)
		{
			GameStateMessage gameStateMessage = (GameStateMessage)msg;
			if (this.isSpectateOrReplay())
			{
				this.addEffect(new EMGameState(gameStateMessage));
				return;
			}
			this.handleGameState(gameStateMessage);
		}
		if (msg is OpponentConnectedMessage)
		{
			OpponentConnectedMessage opponentConnectedMessage = (OpponentConnectedMessage)msg;
			this.updateChat("-- " + opponentConnectedMessage.name + " reconnected --");
		}
		if (msg is OpponentDisconnectedMessage)
		{
			OpponentDisconnectedMessage opponentDisconnectedMessage = (OpponentDisconnectedMessage)msg;
			this.updateChat("-- " + opponentDisconnectedMessage.name + " disconnected --");
		}
		if (msg is LeaveGameMessage)
		{
			LeaveGameMessage leaveGameMessage = (LeaveGameMessage)msg;
			this.updateChat("-- " + leaveGameMessage.from + " left the game --");
		}
		if (msg is GameChatMessageMessage)
		{
			this.effects.Add(new EMChatEffect((GameChatMessageMessage)msg));
		}
		if (msg is WhisperMessage)
		{
			WhisperMessage whisperMessage = (WhisperMessage)msg;
			string text = whisperMessage.text.Trim();
			if (whisperMessage.from == App.MyProfile.ProfileInfo.name)
			{
				if (text != string.Empty)
				{
					this.updateChat(string.Concat(new string[]
					{
						"<color=#ffcc60>[To ",
						whisperMessage.toProfileName,
						"] ",
						text,
						"</color>"
					}));
				}
			}
			else if (text != string.Empty)
			{
				this.updateChat(string.Concat(new string[]
				{
					"<color=#ffcc60>[From ",
					whisperMessage.from,
					"] ",
					text,
					"</color>"
				}));
			}
		}
		if (msg is CliResponseMessage)
		{
			CliResponseMessage cliResponseMessage = (CliResponseMessage)msg;
			string text2 = cliResponseMessage.text.Trim();
			if (text2 != string.Empty)
			{
				this.updateChat("<color=#ccbbbb>System: " + text2 + "</color>");
			}
		}
	}

	// Token: 0x060002E7 RID: 743 RVA: 0x00026658 File Offset: 0x00024858
	private void handleGameChatMessage(GameChatMessageMessage m)
	{
		AiTutorialTicker aiTutorialTicker = this.tutorial as AiTutorialTicker;
		if (aiTutorialTicker != null && this.rightPlayer.name == m.from)
		{
			aiTutorialTicker.addText(m.text);
			aiTutorialTicker.next();
			return;
		}
		if (!App.IsChatAllowed())
		{
			return;
		}
		if (App.FriendList.IsBlocked(m.from))
		{
			return;
		}
		string text = m.text.Trim();
		if (text != string.Empty)
		{
			this.updateChat(m.from + ": " + text);
		}
		if (this.gameMode == GameMode.Spectate)
		{
			SpectateChatMessageMessage m2 = SpectateChatMessageMessage.FromGameChatMessage(m);
			App.ArenaChat.ChatRooms.ChatMessage(m2, this.gameId);
		}
		else if (this.gameType.isMultiplayer() || this.isReplay())
		{
			App.ArenaChat.ChatRooms.ChatMessage(m, this.gameId, this.isReplay());
		}
		App.ChatUI.ScrollDownChat(false);
	}

	// Token: 0x060002E8 RID: 744 RVA: 0x00004267 File Offset: 0x00002467
	private BMPlayer getActivePlayer()
	{
		return this.getPlayer(this.activeColor);
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x00004275 File Offset: 0x00002475
	private BMPlayer getPlayer(TileColor color)
	{
		return (color != this.leftColor) ? this.rightPlayer : this.leftPlayer;
	}

	// Token: 0x060002EA RID: 746 RVA: 0x0002676C File Offset: 0x0002496C
	private BMPlayer getPlayer(int profileId)
	{
		if (profileId == this.leftPlayer.profileId)
		{
			return this.leftPlayer;
		}
		if (profileId == this.rightPlayer.profileId)
		{
			return this.rightPlayer;
		}
		throw new ArgumentException("unknown profileId: " + profileId);
	}

	// Token: 0x060002EB RID: 747 RVA: 0x00004294 File Offset: 0x00002494
	private void handleCostUpdate(EMCostUpdate m)
	{
		if (m == null)
		{
			return;
		}
		this.getPlayer(m.profileId).costs().update(m);
		App.GlobalMessageHandler.internalHandleMessage(m);
	}

	// Token: 0x060002EC RID: 748 RVA: 0x000267C0 File Offset: 0x000249C0
	private void handleGameState(GameStateMessage m)
	{
		bool flag = this.requestedGameState;
		this.requestedGameState = false;
		this.activeColor = m.activeColor;
		this.updateResources(m.getState(this.leftColor).assets, m.getState(this.rightColor).assets);
		this.battleUI.UpdateStackSize(m.whiteGameState.assets.librarySize, m.whiteGameState.assets.graveyardSize, this.isLeftColor(TileColor.white));
		this.battleUI.UpdateStackSize(m.blackGameState.assets.librarySize, m.blackGameState.assets.graveyardSize, this.isLeftColor(TileColor.black));
		this._setPlayerBoardState(m.getState(this.leftColor).board);
		this._setPlayerBoardState(m.getState(this.rightColor).board);
		this.setupTurnBegin(m.phase == EndPhaseMessage.Phase.Main);
		this.handleCostUpdate(m.getState(this.leftColor).assets.costUpdate);
		this.handleCostUpdate(m.getState(this.rightColor).assets.costUpdate);
		this.leftPlayer.rules().clear();
		this.leftPlayer.rules().add(m.getState(this.leftColor).assets.ruleUpdates);
		this.rightPlayer.rules().clear();
		this.rightPlayer.rules().add(m.getState(this.rightColor).assets.ruleUpdates);
		if (this.isRealPlayer(this.activeColor) && (m.phase == EndPhaseMessage.Phase.Init || m.phase == EndPhaseMessage.Phase.PreMain))
		{
			this.endPhase(m.phase);
		}
		if (this.isRealPlayer(this.leftColor) && !m.getState(this.leftColor).mulliganAllowed)
		{
			this.disableMulligan();
		}
		int num = (m.phase != EndPhaseMessage.Phase.Main) ? this.roundTime : m.secondsLeft;
		if (num >= 0)
		{
			this.setSecondsLeft(num);
		}
		this.addEffect(EMTurnBegin.Fake(this.activeColor, m.turn, m.hasSacrificed, num));
	}

	// Token: 0x060002ED RID: 749 RVA: 0x000042BF File Offset: 0x000024BF
	private void setSecondsLeft(int secondsLeft)
	{
		this.roundTime = secondsLeft;
		this.roundTimer = App.Clocks.battleModeClock.getTime();
	}

	// Token: 0x060002EE RID: 750 RVA: 0x00026A00 File Offset: 0x00024C00
	private void setupTurnBegin(bool canShowTurnEndButton)
	{
		if (this.isRealPlayer(this.activeColor))
		{
			this.handManager.DeselectCard();
		}
		if (this.gameMode == GameMode.Play && this.isRightColor(this.activeColor))
		{
			this.handManager.SetCardsGrayedOut(true);
		}
		else
		{
			this.handManager.SetCardsGrayedOut(false);
		}
		if (canShowTurnEndButton && this.isRealPlayer(this.activeColor))
		{
			this.battleUI.BeginTurn(true);
		}
		else
		{
			this.battleUI.BeginTurn(false);
		}
		if (this.isLeftColor(this.activeColor))
		{
			this.battleUI.ScaleAvatar(true, true);
			this.battleUI.ScaleAvatar(false, false);
		}
		else
		{
			this.battleUI.ScaleAvatar(false, true);
			this.battleUI.ScaleAvatar(true, false);
		}
	}

	// Token: 0x060002EF RID: 751 RVA: 0x00026AE4 File Offset: 0x00024CE4
	private void _setPlayerBoardState(BoardState g)
	{
		for (int i = 0; i < g.idols.Length; i++)
		{
			this.getIdol(g.color, i).setHitPoints(g.idols[i], false, false);
		}
		for (int j = 0; j < 5; j++)
		{
			for (int k = 0; k < 3; k++)
			{
				TilePosition p = new TilePosition(g.color, j, k);
				if (this.getUnit(p) != null)
				{
					this.removeUnit(p, RemovalType.Silent);
				}
			}
		}
		foreach (BoardStateTileData boardStateTileData in g.tiles)
		{
			TilePosition tilePosition = new TilePosition(g.color, boardStateTileData.position);
			Unit unit = this.summonUnit(boardStateTileData.card, App.Communicator.GetCardDownloadURL(), tilePosition);
			unit.setAttackPower(boardStateTileData.ap);
			unit.setAttackCounter(boardStateTileData.ac);
			unit.setHitPoints(boardStateTileData.hp);
			unit.setBuffs(boardStateTileData.buffs);
			this.updateChargeAnimation(tilePosition);
		}
	}

	// Token: 0x060002F0 RID: 752 RVA: 0x000042DD File Offset: 0x000024DD
	private void removeUnit(Unit unit, RemovalType rt)
	{
		this.removeUnit(unit.getTilePosition(), rt);
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x00026C0C File Offset: 0x00024E0C
	private void removeUnit(TilePosition p, RemovalType rt)
	{
		Unit unit = this.getUnit(p);
		this.getTile(p).setChargeAnimation(-1);
		string text = null;
		if (rt == RemovalType.Kill)
		{
			text = this.getKillAnimationName(unit);
		}
		if (rt == RemovalType.Unsummon)
		{
			text = "Unsummon";
		}
		if (text != null)
		{
			this.playUnitAnimation(p, text);
			if (text == "gunpowder_explosion_1")
			{
				FlashingLight.Create(unit.transform.position, Color.yellow, 0.5f);
			}
		}
		unit.Destroy();
		this.getUnitsFor(p.color).Remove(unit);
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x00026CA0 File Offset: 0x00024EA0
	private string getKillAnimationName(Unit u)
	{
		CardType cardType = u.getCard().getCardType();
		return cardType.getTag<string>("anim_kill", "perish");
	}

	// Token: 0x060002F3 RID: 755 RVA: 0x000042EC File Offset: 0x000024EC
	private GameObject playTileAnimation(TilePosition p, string anim)
	{
		return this.playTileAnimation(p, anim, 90000 + p.row);
	}

	// Token: 0x060002F4 RID: 756 RVA: 0x00026CCC File Offset: 0x00024ECC
	private GameObject playTileAnimation(TilePosition p, string anim, int renderQueue)
	{
		Vector3 p2 = this.getTile(p).transform.position + new Vector3(0f, 0.5f, 0f);
		return this.playBoardAnimation(p2, anim, renderQueue, this.isLeftColor(p.color));
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x00026D1C File Offset: 0x00024F1C
	private GameObject playUnitAnimation(TilePosition p, string anim)
	{
		if (anim.ToLower().StartsWith("ps "))
		{
			return this.playUnitParticles(p, anim.Substring(3));
		}
		Unit unit = this.getUnit(p);
		if (unit == null)
		{
			return null;
		}
		Vector3 p2 = unit.transform.position + new Vector3(0f, 0.5f, 0f);
		return this.playBoardAnimation(p2, anim, 90000 + p.row, this.isLeftColor(p.color));
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x00026DA8 File Offset: 0x00024FA8
	private GameObject playUnitParticles(TilePosition p, string anim)
	{
		Unit unit = this.getUnit(p);
		if (unit == null)
		{
			return null;
		}
		Vector3 vector = unit.transform.position + new Vector3(0f, 0.5f, 0f);
		Quaternion quaternion = Quaternion.Euler(-39f, 270f, 0f);
		GameObject gameObject = (GameObject)Object.Instantiate(ResourceManager.Load("@Particles/" + anim + "/" + anim, typeof(GameObject)), vector, quaternion);
		gameObject.renderer.material.renderQueue = Unit.getRowRenderQueue(p.row + 1);
		ParticleScaler.Scale(gameObject, this.ExplosionScale);
		ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
		if (component != null)
		{
			Object.Destroy(gameObject, component.duration + 1f);
		}
		return gameObject;
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x00026E84 File Offset: 0x00025084
	private GameObject playBoardAnimation(Vector3 p, string anim, int renderQueue, bool left)
	{
		GameObject gameObject = new GameObject();
		Vector3 baseScale = Vector3.one * 0.14f;
		gameObject.AddComponent<MeshRenderer>();
		gameObject.AddComponent<EffectPlayer>().init(anim, (!left) ? -1 : 1, this, renderQueue, baseScale, false, string.Empty, 0);
		gameObject.transform.position = p;
		gameObject.transform.eulerAngles = new Vector3(51f, 270f, 0f);
		return gameObject;
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x00026F00 File Offset: 0x00025100
	private void playAnimation(AnimConf conf)
	{
		GameObject gameObject = new GameObject();
		gameObject.AddComponent<MeshRenderer>();
		gameObject.AddComponent<EffectPlayer>().init(conf, this);
		gameObject.transform.position = conf.pos;
		gameObject.transform.eulerAngles = conf.eulerAngles;
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x00004302 File Offset: 0x00002502
	private AnimConf animConf(Unit u)
	{
		return this.animConf(u.getTilePosition(), u.transform);
	}

	// Token: 0x060002FA RID: 762 RVA: 0x00004316 File Offset: 0x00002516
	private AnimConf animConf(Tile t)
	{
		return this.animConf(t.tilePosition(), t.transform);
	}

	// Token: 0x060002FB RID: 763 RVA: 0x00026F4C File Offset: 0x0002514C
	private AnimConf animConf(TilePosition tp, Transform t)
	{
		AnimConf animConf = new AnimConf().FlipX(this.isLeftColor(tp.color)).RenderQueue(90000 + tp.row);
		animConf.pos = t.position + new Vector3(0f, 0.5f, 0f);
		return animConf;
	}

	// Token: 0x060002FC RID: 764 RVA: 0x00026FA8 File Offset: 0x000251A8
	private TilePosition getApproximateTilePosition(Vector3 p)
	{
		TileColor color = (p.z >= 0f) ? this.rightColor : this.leftColor;
		int row = CollectionUtil.getMinElement<Idol>(this.getIdolsFor(color), (Idol e) => (p - e.transform.position).sqrMagnitude).row();
		return new TilePosition(color, row, 0);
	}

	// Token: 0x060002FD RID: 765 RVA: 0x00027010 File Offset: 0x00025210
	private AnimConf animConf(Idol idol)
	{
		AnimConf animConf = new AnimConf().FlipX().RenderQueue(90000 + idol.row());
		animConf.pos = idol.transform.position + new Vector3(0f, 0.5f, 0f);
		return animConf;
	}

	// Token: 0x060002FE RID: 766 RVA: 0x0000432A File Offset: 0x0000252A
	public void removeOpponentCast()
	{
		this.effectDone();
	}

	// Token: 0x060002FF RID: 767 RVA: 0x00027064 File Offset: 0x00025264
	private Unit summonUnit(Card card, string url, TilePosition pos)
	{
		Unit result;
		try
		{
			bool flag = pos.color == this.leftColor;
			int num = (!flag) ? 1 : -1;
			GameObject gameObject = this.getTile(pos).gameObject;
			float num2 = gameObject.transform.position.x + 0.15f;
			float num3 = gameObject.transform.position.z + 0.05f * (float)(-(float)num);
			GameObject gameObject2 = new GameObject();
			gameObject2.AddComponent<MeshRenderer>();
			gameObject2.AddComponent<Unit>();
			Unit component = gameObject2.GetComponent<Unit>();
			gameObject2.name = "unit_" + card.getName();
			gameObject2.transform.position = new Vector3(num2, 0.25f, num3);
			Unit.StatsState statsState = (!this.showUnitStats) ? Unit.StatsState.LONGFLASH : Unit.StatsState.HOLD;
			component.init(url, this, num3, pos, statsState, num, card);
			if (this.showUnitStats)
			{
				component.alwaysShowStats(true);
			}
			gameObject2.transform.eulerAngles = new Vector3(51f, 270f, 0f);
			this.getUnitsFor(pos.color).Add(component);
			result = component;
		}
		catch (Exception ex)
		{
			Log.warning("Error in summonUnit(...): " + ex);
			result = null;
		}
		return result;
	}

	// Token: 0x06000300 RID: 768 RVA: 0x00004332 File Offset: 0x00002532
	private void closeChatInput()
	{
		this.closeChatInput(true);
	}

	// Token: 0x06000301 RID: 769 RVA: 0x000271DC File Offset: 0x000253DC
	private void closeChatInput(bool sendChatString)
	{
		this.showChatInput = false;
		this.chatString = this.chatString.Replace("\"", "'");
		if (sendChatString && this.gameMode == GameMode.Play && this.chatString != string.Empty)
		{
			this.chatString = App.ArenaChat.AdjustIfReplyMessage(this.chatString);
			if (!string.IsNullOrEmpty(this.chatString))
			{
				this.sendBattleRequest(new GameChatMessageMessage(this.chatString));
			}
		}
		this.chatString = string.Empty;
		this.fadeChat = true;
	}

	// Token: 0x06000302 RID: 770 RVA: 0x0000433B File Offset: 0x0000253B
	private void setChatActive(bool showLog)
	{
		this.setChatActive(showLog, false);
	}

	// Token: 0x06000303 RID: 771 RVA: 0x0002727C File Offset: 0x0002547C
	private void setChatActive(bool showLog, bool requestShowInput)
	{
		if (!App.IsChatAllowed())
		{
			return;
		}
		this.showChat = showLog;
		this.showChatInput = (requestShowInput || this.showChatInput);
		if (!showLog)
		{
			this.showChatInput = false;
		}
		if (this.showChat)
		{
			this.fadeChat = !requestShowInput;
			this.chatOpacity = 1f;
		}
	}

	// Token: 0x06000304 RID: 772 RVA: 0x00004345 File Offset: 0x00002545
	private void toggleUnitStats()
	{
		this.setShowUnitStats(!this.showUnitStats);
	}

	// Token: 0x06000305 RID: 773 RVA: 0x00004356 File Offset: 0x00002556
	private void setShowUnitStats(bool show)
	{
		this.setShowUnitStatsNoPersistence(show);
		App.Config.settings.battle.show_unit_stats.value = this.showUnitStats;
	}

	// Token: 0x06000306 RID: 774 RVA: 0x000272E0 File Offset: 0x000254E0
	private void setShowUnitStatsNoPersistence(bool show)
	{
		this.showUnitStats = show;
		foreach (Unit unit in this.getAllUnitsCopy())
		{
			unit.alwaysShowStats(this.showUnitStats);
		}
		foreach (Idol idol in this.getAllIdolsCopy())
		{
			idol.showStats(this.showUnitStats);
		}
	}

	// Token: 0x06000307 RID: 775 RVA: 0x00027394 File Offset: 0x00025594
	public void cameraLookUp()
	{
		iTween.RotateTo(base.camera.gameObject, iTween.Hash(new object[]
		{
			"x",
			51f,
			"y",
			base.camera.transform.eulerAngles.y,
			"z",
			base.camera.transform.eulerAngles.z,
			"easetype",
			iTween.EaseType.linear,
			"time",
			1f
		}));
		iTween.MoveTo(base.camera.gameObject, iTween.Hash(new object[]
		{
			"x",
			9.67f,
			"y",
			11.15f,
			"z",
			0,
			"easetype",
			iTween.EaseType.linear,
			"time",
			1f
		}));
	}

	// Token: 0x06000308 RID: 776 RVA: 0x000274CC File Offset: 0x000256CC
	public void cameraLookDown()
	{
		iTween.RotateTo(base.camera.gameObject, iTween.Hash(new object[]
		{
			"x",
			62f,
			"y",
			base.camera.transform.eulerAngles.y,
			"z",
			base.camera.transform.eulerAngles.z,
			"easetype",
			iTween.EaseType.linear,
			"time",
			1f
		}));
		iTween.MoveTo(base.camera.gameObject, iTween.Hash(new object[]
		{
			"x",
			7.5f,
			"y",
			12.57f,
			"z",
			0,
			"easetype",
			iTween.EaseType.linear,
			"time",
			1f
		}));
	}

	// Token: 0x06000309 RID: 777 RVA: 0x0000437E File Offset: 0x0000257E
	public void cameraShake(float strength)
	{
		this.cameraShake(strength, strength);
	}

	// Token: 0x0600030A RID: 778 RVA: 0x00004388 File Offset: 0x00002588
	public void cameraShakeDuration(float strength, float time)
	{
		this.cameraShake(strength, strength, Vector2.zero, time);
	}

	// Token: 0x0600030B RID: 779 RVA: 0x00004398 File Offset: 0x00002598
	public void cameraShake(float x, float y)
	{
		this.cameraShake(x, y, Vector2.zero);
	}

	// Token: 0x0600030C RID: 780 RVA: 0x000043A7 File Offset: 0x000025A7
	public void cameraShake(float x, float y, Vector2 bias)
	{
		this.cameraShake(x, y, bias, 0.5f);
	}

	// Token: 0x0600030D RID: 781 RVA: 0x000043B7 File Offset: 0x000025B7
	public void cameraShake(float x, float y, Vector2 bias, float duration)
	{
		this.shake.shake(8f * x, 8f * y, bias, duration);
	}

	// Token: 0x0600030E RID: 782 RVA: 0x000043D5 File Offset: 0x000025D5
	private void endPhase(EndPhaseMessage.Phase phase)
	{
		this.sendBattleRequest(new EndPhaseMessage(phase));
	}

	// Token: 0x0600030F RID: 783 RVA: 0x00027604 File Offset: 0x00025804
	private void endTurn()
	{
		this.audioScript.PlaySFX("Sounds/hyperduck/ui_end_turn");
		int num = 0;
		for (int i = 0; i < this.leftUnitsArr.Count; i++)
		{
			if (this.leftUnitsArr[i].getAttackInterval() == 0)
			{
				num++;
			}
		}
		if (num == 1)
		{
			base.StartCoroutine(this.playDelayedSound("Sounds/hyperduck/fanfare_tier_00", 0.2f));
		}
		else if (num == 2)
		{
			base.StartCoroutine(this.playDelayedSound("Sounds/hyperduck/fanfare_tier_02", 0.2f));
		}
		else if (num > 2)
		{
			base.StartCoroutine(this.playDelayedSound("Sounds/hyperduck/fanfare_tier_03", 0.2f));
		}
		if (this.tutorial.onEndTurn())
		{
			this.nextTutorialSlide();
		}
		this.endPhase(EndPhaseMessage.Phase.Main);
	}

	// Token: 0x06000310 RID: 784 RVA: 0x000276DC File Offset: 0x000258DC
	private IEnumerator playDelayedSound(string sound, float delay)
	{
		yield return new WaitForSeconds(delay);
		this.audioScript.PlaySFX(sound);
		yield break;
	}

	// Token: 0x06000311 RID: 785 RVA: 0x000043E3 File Offset: 0x000025E3
	private void cleanupBoard()
	{
		this.deselectAllTiles();
		this.handManager.DeselectCard();
		this.checkDestroyCardRule();
	}

	// Token: 0x06000312 RID: 786 RVA: 0x000043FD File Offset: 0x000025FD
	public ScreenFlash flashScreen(float inSeconds)
	{
		return this.flashScreen(inSeconds, 0.05f, 1f);
	}

	// Token: 0x06000313 RID: 787 RVA: 0x00027714 File Offset: 0x00025914
	public ScreenFlash flashScreen(float inSeconds, float lifetime, float amp)
	{
		ScreenFlash component = base.gameObject.GetComponent<ScreenFlash>();
		if (component)
		{
			Object.Destroy(component);
		}
		ScreenFlash screenFlash = base.gameObject.AddComponent<ScreenFlash>();
		return screenFlash.init(inSeconds, lifetime, amp);
	}

	// Token: 0x06000314 RID: 788 RVA: 0x00004410 File Offset: 0x00002610
	private void nextTutorialSlide()
	{
		this.pendingMoveNextTutorialSlide = true;
		this._updateTutorial();
	}

	// Token: 0x06000315 RID: 789 RVA: 0x00027754 File Offset: 0x00025954
	private void updateTutorialBlink(bool force)
	{
		this.tutorialBlinker.Update();
		if (!force && TimeUtil.CurrentTimeMillis() - this.lastTutorialBlinkTime < 1000L)
		{
			return;
		}
		this.lastTutorialBlinkTime = TimeUtil.CurrentTimeMillis();
		List<GameObject> list = new List<GameObject>();
		TilePosition[] array = new TilePosition[1];
		foreach (Unit unit in this.getAllUnitsCopy())
		{
			array[0] = unit.getTilePosition();
			if (this.tutorial.filter(array, new FilterData(this, unit)).Length > 0)
			{
				list.Add(unit.gameObject);
			}
		}
		List<GameObject> list2 = new List<GameObject>();
		array = new TilePosition[]
		{
			new TilePosition(this.leftColor, 0, 0)
		};
		foreach (CardView cardView in this.handManager.GetCardViewsInHand())
		{
			if (this.tutorial.filter(array, new FilterData(this, cardView.getCardInfo())).Length > 0)
			{
				list2.Add(cardView.gameObject);
			}
		}
		this.tutorialBlinker.setUnits(list);
		this.tutorialBlinker.setCards(list2);
		this.tutorialBlinker.update(Enumerable.ToList<TutorialTicker.Tag>(this.tutorial.getTags()));
	}

	// Token: 0x06000316 RID: 790 RVA: 0x000278E4 File Offset: 0x00025AE4
	private void OnGUI()
	{
		GUI.depth = 6;
		GUI.skin = this.battleModeSkin;
		this.OnGUI_drawSpectatorCount();
		this.OnGUI_drawSpectatorList();
		this.OnGUI_updateTutorial();
		GUI.skin.label.alignment = 3;
		foreach (BattleMode.AnimatedText animatedText in this.animatedTexts)
		{
			Vector3 vector = animatedText.startPos * (1f - animatedText.t) + animatedText.targetPos * animatedText.t;
			vector..ctor(vector.x, (float)Screen.height - vector.y, 0f);
			Color color = GUI.color;
			GUI.color = new Color(1f, 1f, 1f, 1f - animatedText.t);
			float num = (float)Screen.height * 0.2f;
			float num2 = (float)Screen.height * 0.1f;
			GUI.DrawTexture(new Rect(vector.x - num2 * 0.28f, vector.y - num2 / 2f + 2f + num2 * 0.35f, num2 * 0.25f, num2 * 0.25f), ResourceManager.LoadTexture("Shared/gold_icon_small"));
			GUI.Label(new Rect(vector.x, vector.y - num2 / 2f + 2f, num, num2), "<color=black>" + animatedText.text + "</color>");
			GUI.Label(new Rect(vector.x, vector.y - num2 / 2f, num, num2), string.Concat(new string[]
			{
				"<color=",
				animatedText.color,
				">",
				animatedText.text,
				"</color>"
			}));
			GUI.color = color;
		}
		GUISkin skin = GUI.skin;
		GUI.skin = this.regularUI;
		int fontSize = GUI.skin.label.fontSize;
		GUI.skin.button.fontSize = Screen.height / 40;
		if (this.gameMode == GameMode.Play && this.currentTurn <= 3 && !this.gameType.isTutorial())
		{
			GUI.color = new Color(1f, 1f, 1f, 0.7f * this.mulliganAlpha);
			Rect rect;
			rect..ctor((float)(Screen.width / 2) - (float)Screen.height * 0.125f, (float)Screen.height * 0.14f, (float)Screen.height * 0.25f, (float)Screen.height * 0.035f);
			Rect rect2;
			rect2..ctor(rect.x, rect.yMax + 12f, rect.width, rect.height);
			GUI.Box(new Rect(rect.x - 4f, rect.y - 4f, rect.width + 8f, rect.height + 8f), string.Empty);
			GUI.Box(new Rect(rect2.x - 4f, rect2.y - 4f, rect2.width + 8f, rect2.height + 8f), string.Empty);
			GUI.color = new Color(1f, 1f, 1f, this.mulliganAlpha);
			bool flag = this.mulliganAvailable && this.mulliganAlpha > 0.8f;
			if (GUI.Button(rect, "Draw new starting hand") && flag)
			{
				this.disableMulligan();
				this.comm.send(new MulliganMessage());
			}
			if (GUI.Button(rect2, "Keep hand") && flag)
			{
				this.disableMulligan();
			}
		}
		GUI.skin.button.fontSize = fontSize;
		GUI.skin = skin;
		if (!this.gameType.isTutorial() && !this.endGameScreen.isInited())
		{
			this.OnGUI_drawNameBoxes();
		}
		if (this.isReplay() && !this.endGameScreen.isInited())
		{
			this.OnGUI_drawReplayControls(BattleMode.getReplayControlRect());
		}
		float num3 = (float)Screen.height * 0.03f;
		float num4 = num3 * 1.39f;
		Rect rect3;
		rect3..ctor(-num4, -num3, num4 + num4, num3 + num3);
		GUI.color = new Color(1f, 1f, 1f, 0.75f);
		if (GUI.Button(rect3, string.Empty, this.battleUISkin.box))
		{
			this.menu.toggleMenu();
		}
		bool flag2 = rect3.Contains(GUIUtil.getScreenMousePos());
		Texture2D texture2D = ResourceManager.LoadTexture("ChatUI/arrow_" + ((!flag2) ? string.Empty : "mo"));
		float num5 = num3 * 0.6f;
		GUI.DrawTexture(new Rect((num4 - num5) * 0.25f, (num3 - num5) * 0.25f, num5, num5), texture2D);
		GUI.color = Color.white;
		if (!this.gameType.isTutorial() && !this.endGameScreen.isInited())
		{
			if (this.showClock)
			{
				float num6 = (this.roundTimer != -999f) ? Mathf.Floor(App.Clocks.battleModeClock.getTime() - this.roundTimer) : 0f;
				int seconds = Mathf.Max(0, (int)((float)this.roundTime - num6));
				this.clock.renderTime(Mathf.CeilToInt((float)this.currentTurn / 2f), seconds);
			}
			else
			{
				this.clock.renderTime(Mathf.CeilToInt((float)this.currentTurn / 2f), -1);
			}
		}
		this.menu.canShowMenu = true;
		if (this.showUserDebugInfo)
		{
			string text = string.Concat(new object[]
			{
				string.Empty,
				this.gameId,
				"-",
				this.leftColor,
				"@",
				this.gameServerAddress,
				":",
				this.gameServerPort
			});
			GUI.TextField(new Rect(0f, 0f, 300f, 100f), text);
		}
		if (this.endGameScreen.isInited())
		{
			this.menu.canShowMenu = false;
		}
		GUI.skin = this.labelSkin;
		int i = this.serverMessages.Count - 1;
		int num7 = 0;
		while (i >= 0)
		{
			BattleMode.ServerMessage serverMessage = this.serverMessages[i];
			float num8 = 3f - (Time.time - serverMessage.startTime);
			serverMessage.skin.label.normal.textColor = new Color(0f, 0f, 0f, num8);
			GUI.Label(new Rect((float)Screen.width * 0.2f + 2f, (float)Screen.height * 0.45f - (float)(num7 * 32) + 2f, (float)Screen.width * 0.6f, (float)Screen.height * 0.1f), serverMessage.text, serverMessage.skin.label);
			serverMessage.skin.label.normal.textColor = new Color(1f, 1f, 1f, num8);
			GUI.Label(new Rect((float)Screen.width * 0.2f, (float)Screen.height * 0.45f - (float)(num7 * 32), (float)Screen.width * 0.6f, (float)Screen.height * 0.1f), serverMessage.text, serverMessage.skin.label);
			if (num8 < 0f)
			{
				this.serverMessages.RemoveAt(i);
				break;
			}
			i--;
			num7++;
		}
		this.OnGUI_drawBattleChat();
		if (this.showToolTip && (this.mouseLabelHead != string.Empty || this.mouseLabelBody != string.Empty) && Time.time - this.mouseLabelTimer > 0.5f)
		{
			GUI.skin = this.labelSkin;
			GUIContent guicontent = new GUIContent(this.mouseLabelBody);
			Vector2 vector2;
			vector2..ctor(250f, this.labelSkin.GetStyle("label").CalcHeight(guicontent, 250f));
			float num9 = Input.mousePosition.x + 15f;
			if (Input.mousePosition.x > (float)(Screen.width / 2))
			{
				num9 = Input.mousePosition.x - 260f;
			}
			float num10 = (float)Screen.height - Input.mousePosition.y;
			if (Input.mousePosition.y < (float)(Screen.height / 2))
			{
				num10 = (float)Screen.height - Input.mousePosition.y - (vector2.y - 15f);
			}
			GUI.Label(new Rect(num9, num10 - 15f, 240f, 15f), this.mouseLabelHead);
			this.labelSkin.label.fontSize = 14;
			this.labelSkin.label.fontStyle = 0;
			GUI.Label(new Rect(num9, num10, 240f, vector2.y), this.mouseLabelBody);
		}
	}

	// Token: 0x06000317 RID: 791 RVA: 0x000282CC File Offset: 0x000264CC
	private void OnGUI_drawSpectatorCount()
	{
		if (this.currentSpectators <= 0)
		{
			return;
		}
		GUISkin skin = GUI.skin;
		GUI.skin = this.regularUI;
		GUI.color = new Color(1f, 1f, 1f, 0.5f);
		GUI.Box(new Rect((float)Screen.width * 0.95f, (float)Screen.height * 0.005f, (float)Screen.width * 0.045f, (float)Screen.height * 0.03f), string.Empty);
		GUI.color = Color.white;
		Rect rect;
		rect..ctor((float)Screen.width * 0.95f, (float)Screen.height * 0.004f, (float)Screen.height * 0.03f, (float)Screen.height * 0.03f);
		if (GUI.Button(rect, string.Empty))
		{
			if (this.spectatorListIsShowing)
			{
				this.spectatorListIsShowing = false;
			}
			else
			{
				this.comm.send(new SpectateGetSpectatorsMessage());
			}
		}
		GUI.DrawTexture(rect, ResourceManager.LoadTexture("Shared/spectate_eye"));
		int fontSize = GUI.skin.label.fontSize;
		GUI.skin.label.fontSize = Screen.height / 30;
		TextAnchor alignment = GUI.skin.label.alignment;
		GUI.skin.label.alignment = 4;
		GUIUtil.drawShadowText(new Rect((float)Screen.width * 0.95f + (float)Screen.height * 0.03f, (float)Screen.height * 0.004f, (float)Screen.width * 0.045f - (float)Screen.height * 0.03f, (float)Screen.height * 0.03f), string.Empty + this.currentSpectators, Color.white);
		GUI.skin.label.alignment = alignment;
		GUI.skin.label.fontSize = fontSize;
		GUI.skin = skin;
	}

	// Token: 0x06000318 RID: 792 RVA: 0x000284B8 File Offset: 0x000266B8
	private void OnGUI_drawSpectatorList()
	{
		if (!this.spectatorListIsShowing)
		{
			return;
		}
		float num = (float)Screen.height * 0.2f;
		float num2 = (float)Screen.width - num - (float)Screen.height * 0.01f;
		float num3 = (float)Screen.height * 0.04f;
		int num4 = Screen.width / 64;
		float num5 = (float)(1 + Math.Min(16, this.spectators.Count)) * (1.5f * (float)num4);
		Rect rect;
		rect..ctor(num2, num3, num, num5);
		Rect rect2 = GeomUtil.resizeCentered(rect, rect.width - (float)num4, rect.height - (float)num4);
		GUISkin skin = GUI.skin;
		GUI.skin = this.battleChat;
		int fontSize = GUI.skin.label.fontSize;
		GUI.skin.label.fontSize = num4;
		GUI.Box(rect, string.Empty);
		GUILayout.BeginArea(rect2);
		this.specScroll = GUILayout.BeginScrollView(this.specScroll, new GUILayoutOption[]
		{
			GUILayout.Width(rect2.width),
			GUILayout.Height(rect2.height)
		});
		for (int i = 0; i < this.spectators.Count; i++)
		{
			GUILayout.Label(this.spectators[i], new GUILayoutOption[0]);
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		GUI.skin.label.fontSize = fontSize;
		GUI.skin = skin;
	}

	// Token: 0x06000319 RID: 793 RVA: 0x00028628 File Offset: 0x00026828
	private static Rect getReplayControlRect()
	{
		float num = (float)Screen.height * 0.001f;
		float num2 = 90f * num;
		float num3 = 8f * num2;
		Rect result;
		result..ctor(0f, 0f, num3, num2);
		result.y = 990f * num - num2 - App.ChatUI.GetHeight();
		result.x = ((float)Screen.width - num3) * 0.5f;
		return result;
	}

	// Token: 0x0600031A RID: 794 RVA: 0x00028698 File Offset: 0x00026898
	private void OnGUI_drawReplayControls(Rect rect)
	{
		if (!this.showReplayControls)
		{
			return;
		}
		GUI.Box(rect, string.Empty, this.battleChat.box);
		GUI.Box(rect, string.Empty, this.battleChat.box);
		Rect rect2;
		rect2..ctor(0f, rect.y + rect.height * 0.2f, rect.height * 0.5f, rect.height * 0.5f);
		int num = 4;
		bool flag = this.replayNexts == 0;
		float num2 = rect2.width * 0.5f;
		float num3 = rect2.width + num2;
		float num4 = (float)num * rect2.width + (float)(num - 1) * num2;
		float num5 = rect.x + (rect.width - num4) * 0.5f;
		Rect rect3 = GeomUtil.cropShare(rect, new Rect(0.025f, 0.8f, 0.95f, 0.1f));
		rect3.height = rect.height * 0.05f;
		GUI.color = new Color(0.4f, 0.35f, 0.3f, 1f);
		GUI.DrawTexture(rect3, ResourceManager.LoadTexture("ChatUI/white"));
		float num6 = this.effects.Progress();
		GUI.color = ColorUtil.FromHex24(12359768u);
		GUI.DrawTexture(GeomUtil.cropShare(rect3, new Rect(0f, --0f, num6, 1f)), ResourceManager.LoadTexture("ChatUI/white"));
		GUI.color = Color.white;
		GUIContent guicontent = new GUIContent();
		rect2.x = num5 + 0f * num3;
		if (GUI.Button(rect2, guicontent, this.btnRestartStyle))
		{
			this.restartReplay();
		}
		rect2.x = num5 + 1f * num3;
		GUI.enabled = !flag;
		if (GUI.Button(rect2, guicontent, this.btnPauseStyle))
		{
			this.replayNexts = 0;
		}
		GUI.enabled = true;
		GUI.enabled = (flag || App.GlobalMessageHandler.getTimeScale() != 1f);
		rect2.x = num5 + 2f * num3;
		if (GUI.Button(rect2, guicontent, this.btnPlayStyle))
		{
			this.replayNexts = int.MaxValue;
			App.GlobalMessageHandler.setTimeScale(1f);
		}
		GUI.enabled = !flag;
		rect2.x = num5 + 3f * num3;
		this.replayFastForward |= GUI.RepeatButton(rect2, guicontent, this.btnFfStyle);
		GUI.enabled = true;
	}

	// Token: 0x0600031B RID: 795 RVA: 0x0002892C File Offset: 0x00026B2C
	private void OnGUI_drawBattleChat()
	{
		if (!this.showChat || (this.gameType.isTutorial() && this.isScriptedTutorial))
		{
			return;
		}
		GUISkin skin = GUI.skin;
		GUI.skin = this.battleChat;
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.chatOpacity);
		Rect rect;
		rect..ctor((float)Screen.width * this.chatPosition.x, (float)Screen.height * this.chatPosition.y, (float)(Screen.width / 4), (float)Screen.height / 5.9f);
		GUI.Box(rect, string.Empty);
		Rect rect2;
		rect2..ctor(rect.x + 10f, rect.y + 14f, rect.width - 24f, rect.height - 28f);
		GUILayout.BeginArea(rect2);
		this.chatScroll = GUILayout.BeginScrollView(this.chatScroll, new GUILayoutOption[]
		{
			GUILayout.Width(rect2.width),
			GUILayout.Height(rect2.height)
		});
		GUILayout.Label(this.chatLog, new GUILayoutOption[0]);
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		if (this.showChatInput)
		{
			GUI.SetNextControlName("chatFrame");
			this.chatString = GUI.TextField(new Rect((float)Screen.width * this.chatPosition.x, (float)Screen.height * this.chatPosition.y + rect.height * 1.005f, (float)(Screen.width / 4), 36f), this.chatString);
			if (GUI.GetNameOfFocusedControl() != "chatFrame")
			{
				GUI.FocusControl("chatFrame");
			}
		}
		GUI.skin = skin;
		if (this.showChatInput && GUI.Button(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), string.Empty))
		{
			this.chatLastMessageSentAtTime = -1000f;
			this.showChatInput = false;
			this.fadeChat = true;
		}
		if (this.fadeChat && this.chatOpacity > 0f && Time.time - this.chatLastMessageSentAtTime > 6f)
		{
			this.chatOpacity -= Time.deltaTime * 3f;
			if (this.chatOpacity <= 0f)
			{
				this.chatOpacity = 0f;
				this.setChatActive(false, false);
			}
		}
	}

	// Token: 0x0600031C RID: 796 RVA: 0x00028BD0 File Offset: 0x00026DD0
	private IEnumerator fadeMulliganButton(float to)
	{
		float from = this.mulliganAlpha;
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime * 2f;
			if (t > 1f)
			{
				t = 1f;
			}
			float factor = t * t * (3f - 2f * t);
			this.mulliganAlpha = from * (1f - factor) + to * factor;
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600031D RID: 797 RVA: 0x00028BFC File Offset: 0x00026DFC
	public void addGoldText(Idol i)
	{
		if (this.rewardForIdolKill <= 0)
		{
			return;
		}
		this.animatedTexts.Add(new BattleMode.AnimatedText(base.camera.WorldToScreenPoint(i.getPosition()) + new Vector3(0f, 30f, 0f), "x" + this.rewardForIdolKill, "#ccaa22"));
	}

	// Token: 0x0600031E RID: 798 RVA: 0x00028C6C File Offset: 0x00026E6C
	private void debugShowCreatedObjects()
	{
		GUILayout.Label("All " + Resources.FindObjectsOfTypeAll(typeof(Object)).Length, new GUILayoutOption[0]);
		GUILayout.Label("Textures " + Resources.FindObjectsOfTypeAll(typeof(Texture)).Length, new GUILayoutOption[0]);
		GUILayout.Label("AudioClips " + Resources.FindObjectsOfTypeAll(typeof(AudioClip)).Length, new GUILayoutOption[0]);
		GUILayout.Label("Meshes " + Resources.FindObjectsOfTypeAll(typeof(Mesh)).Length, new GUILayoutOption[0]);
		GUILayout.Label("Materials " + Resources.FindObjectsOfTypeAll(typeof(Material)).Length, new GUILayoutOption[0]);
		GUILayout.Label("GameObjects " + Resources.FindObjectsOfTypeAll(typeof(GameObject)).Length, new GUILayoutOption[0]);
		GUILayout.Label("Components " + Resources.FindObjectsOfTypeAll(typeof(Component)).Length, new GUILayoutOption[0]);
		GUILayout.Label("EffectPlayers " + Resources.FindObjectsOfTypeAll(typeof(EffectPlayer)).Length, new GUILayoutOption[0]);
		GUILayout.Label("MonoBehaviour " + Resources.FindObjectsOfTypeAll(typeof(MonoBehaviour)).Length, new GUILayoutOption[0]);
	}

	// Token: 0x0600031F RID: 799 RVA: 0x0000441F File Offset: 0x0000261F
	private bool checkDestroyCardRule()
	{
		if (!this.tutorial.allowHideCardView())
		{
			return false;
		}
		if (this.cardRule != null)
		{
			Object.Destroy(this.cardRule);
		}
		return true;
	}

	// Token: 0x06000320 RID: 800 RVA: 0x00028DFC File Offset: 0x00026FFC
	private void cardClicked(CardView card, int mouseButton)
	{
		this.audioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
		this.checkDestroyCardRule();
		if (mouseButton == 0)
		{
			if (this.gameMode == GameMode.Play)
			{
				if (this.subState == BattleMode.SubState.Sift)
				{
					this.handManager.SelectCard(card.getCardInfo().getId(), this, new ResourceType[0], false, true, false, this.alwaysWild);
					this.handManager.AddSelectedCardConfirm(CardConfirmType.Sift);
					return;
				}
				bool flag = this.tutorial.allowPlayCard(card.getCardInfo());
				if (this.tutorial.onCardClicked(this, card.getCardInfo()))
				{
					if (this.tutorial.shouldZoomCard(card.getCardInfo()))
					{
						this.showCardRule(card.getCardInfo(), card.profileId());
					}
					this.nextTutorialSlide();
				}
				bool canSacrificeForCards = !this.customSettings.has(RuleSettings.NoCardSacrifice) && this.sacrificer.canSacrifice(ResourceType.CARDS);
				ResourceType[] array = (!this.customSettings.has(RuleSettings.NoResourceSacrifice)) ? this.sacrificer.getSacrificable(this.resTypes).ToArray() : new ResourceType[0];
				CardView cardView = this.handManager.SelectCard(card.getCardInfo().getId(), this, array, canSacrificeForCards, this.activeColor == this.leftColor, false, this.alwaysWild);
				if (cardView != null && (flag || this.tutorial.allowPlayCard(cardView.getCardInfo())))
				{
					this.sendBattleRequest(new PlayCardInfoMessage(card.getCardInfo()));
				}
				ResourceGroup availableResources = this.battleUI.GetResources(this.isLeftColor(this.activeColor)).availableResources;
				if (cardView && availableResources.canAfford(cardView.getCardInfo()))
				{
					Card cardInfo = cardView.getCardInfo();
					ResourceGroup resourceGroup = new ResourceGroup(this.battleUI.GetLeftPlayerResources());
					int profileId = (this.gameMode != GameMode.Play) ? this.getActivePlayer().profileId : this.leftPlayer.profileId;
					resourceGroup.sub(cardInfo.getResourceType(), this.getCostForCard(cardInfo, profileId));
					this.battleUI.SetTempReduction(resourceGroup);
					this.handManager.SetReductedResources(resourceGroup);
					this.handManager.RefreshCardAffordabilities(availableResources);
				}
			}
			else if (this.sacrificer != null)
			{
				this.handManager.SelectCard(card.getCardInfo().getId(), this, this.sacrificer.getSacrificable(this.resTypes).ToArray(), false, this.activeColor == this.leftColor, true, this.alwaysWild);
			}
			this.deselectAllTiles();
		}
		if (this.activeColor != this.leftColor)
		{
			return;
		}
		if (mouseButton == 1)
		{
			this.handManager.DeselectCard();
			this.HideCardView();
			this.deselectAllTiles();
		}
	}

	// Token: 0x06000321 RID: 801 RVA: 0x000290D8 File Offset: 0x000272D8
	public float IncreaseMultiplier(int rarity)
	{
		float num = this.tierRewardMultiplierDelta[rarity];
		num = Mathf.Min(num, this.maxTierRewardMultiplier - this.accumulatedTierRewardMultiplier);
		this.accumulatedTierRewardMultiplier += num;
		return num;
	}

	// Token: 0x06000322 RID: 802 RVA: 0x00004450 File Offset: 0x00002650
	public void SpecificCardDeselected(CardView card)
	{
		this.resetTempReductions();
	}

	// Token: 0x06000323 RID: 803 RVA: 0x00029114 File Offset: 0x00027314
	private void resetTempReductions()
	{
		this.battleUI.ResetTempReductions();
		ResourceGroup availableResources = this.battleUI.GetResources(true).availableResources;
		this.handManager.RefreshCardAffordabilities(availableResources);
	}

	// Token: 0x06000324 RID: 804 RVA: 0x00004458 File Offset: 0x00002658
	public void magnifyCard(CardView card)
	{
		this.handManager.MagnifySelected();
	}

	// Token: 0x06000325 RID: 805 RVA: 0x00004465 File Offset: 0x00002665
	public void confirmPlayCard(CardView card)
	{
		this.confirmPlayCard(card, null);
	}

	// Token: 0x06000326 RID: 806 RVA: 0x0000446F File Offset: 0x0000266F
	public void confirmPlayCard(CardView card, List<TilePosition> tiles)
	{
		if (!this.isMyTurn())
		{
			return;
		}
		if (this.subState == BattleMode.SubState.Normal)
		{
			this.sendBattleRequest(new PlayCardMessage(card.getCardInfo(), tiles));
			this.handManager.RemoveSelectedCard();
			this.resetTempReductions();
		}
	}

	// Token: 0x06000327 RID: 807 RVA: 0x000044AB File Offset: 0x000026AB
	private void setSubState(BattleMode.SubState newState)
	{
		this.subState = newState;
	}

	// Token: 0x06000328 RID: 808 RVA: 0x000044B4 File Offset: 0x000026B4
	public Rect getSacrificeDestRect(ResourceType resource)
	{
		return this.battleUI.GetResourceIconRect(resource, true);
	}

	// Token: 0x06000329 RID: 809 RVA: 0x000044C3 File Offset: 0x000026C3
	private bool isMyTurn()
	{
		return this.activeColor == this.leftColor;
	}

	// Token: 0x0600032A RID: 810 RVA: 0x0002914C File Offset: 0x0002734C
	public void sacrificeCard(CardView card, ResourceType resource)
	{
		if (!this.isMyTurn())
		{
			return;
		}
		try
		{
			this.sacrificer.sacrifice(resource);
			this.sendBattleRequest(new SacrificeCardMessage(card.getCardInfo(), resource));
			if (resource.isResource())
			{
				this.addEffect(new EMTweenResource());
			}
			this.deselectAllTiles();
			this.resetTempReductions();
		}
		catch (Exception ex)
		{
			Log.info("Error: " + ex);
		}
	}

	// Token: 0x0600032B RID: 811 RVA: 0x000044D3 File Offset: 0x000026D3
	public void effectDoneSoon(float time)
	{
		base.StartCoroutine(this._EffectDoneSoon(time));
	}

	// Token: 0x0600032C RID: 812 RVA: 0x000291D0 File Offset: 0x000273D0
	private IEnumerator _EffectDoneSoon(float time)
	{
		yield return new WaitForSeconds(time);
		this.effectDone();
		yield break;
	}

	// Token: 0x0600032D RID: 813 RVA: 0x000044E3 File Offset: 0x000026E3
	public void killObject(Transform target)
	{
		Object.DestroyImmediate(target.gameObject);
	}

	// Token: 0x0600032E RID: 814 RVA: 0x000291FC File Offset: 0x000273FC
	public void effectDone()
	{
		if (this.lastStartedEffectSequenceId < this.lastFinishedEffectSequenceId)
		{
			Log.error(string.Concat(new object[]
			{
				"Started effect id < finished effect id! ",
				this.lastStartedEffectSequenceId,
				" < ",
				this.lastFinishedEffectSequenceId
			}));
			return;
		}
		if (this.lastStartedEffectSequenceId == this.lastFinishedEffectSequenceId)
		{
			return;
		}
		if (this.currentEffect == null)
		{
			Log.error("Calling effectDone without an effect running: " + SystemUtil.getStackTrace());
			return;
		}
		Log.info("effect done (" + this.currentEffect.type + ")");
		this.lastFinishedEffectSequenceId = this.currentEffect.sequenceId();
		this.currentEffect = null;
	}

	// Token: 0x0600032F RID: 815 RVA: 0x000292C0 File Offset: 0x000274C0
	private void playBlastAnim(Transform t, int strength)
	{
		if (strength <= 0)
		{
			return;
		}
		TilePosition approximateTilePosition = this.getApproximateTilePosition(t.position);
		int animId = Mathf.Clamp(strength - 1, 0, 9);
		AnimConf conf = this.animConf(approximateTilePosition, t).Bundle("melee-blast").AnimId(animId).KScale(0.9f).FlipX(this.isRightColor(approximateTilePosition.color)).ForwardOffset(new Vector3(0.11f, 0f, -0.1f));
		this.playAnimation(conf);
	}

	// Token: 0x06000330 RID: 816 RVA: 0x00029344 File Offset: 0x00027544
	private void battlePopup(TilePosition position, string s)
	{
		Unit unit = this.getUnit(position);
		if (unit != null)
		{
			unit.damage(2);
		}
		this.battlePopup(this.getTile(position).transform.position, new Vector3(0f, 1.15f, 0f), s);
	}

	// Token: 0x06000331 RID: 817 RVA: 0x00029398 File Offset: 0x00027598
	private void battlePopup(Vector3 position, Vector3 offset, string s)
	{
		GameObject gameObject = new GameObject("BattleText_" + s);
		UnityUtil.addChild(base.gameObject, gameObject);
		gameObject.transform.position = position;
		gameObject.transform.Translate(offset);
		BattleTextPopup battleTextPopup = gameObject.AddComponent<BattleTextPopup>();
		battleTextPopup.init(s);
	}

	// Token: 0x06000332 RID: 818 RVA: 0x000293EC File Offset: 0x000275EC
	private float getScreenShakeAmount(int amount)
	{
		if (amount <= 0)
		{
			return 0f;
		}
		float num = Math.Min((float)amount, 8f) / 8f;
		return 0.3f + this.damageShakeMultiplier * Mathf.Sqrt(num);
	}

	// Token: 0x06000333 RID: 819 RVA: 0x0002942C File Offset: 0x0002762C
	private void runEffect()
	{
		if (this.currentEffect != null)
		{
			return;
		}
		if (this.replayNexts <= 0)
		{
			return;
		}
		EffectMessage effectMessage = this.effects.PopFirst();
		if (effectMessage == null)
		{
			return;
		}
		this.replayNexts--;
		this.forceRunEffect(effectMessage);
	}

	// Token: 0x06000334 RID: 820 RVA: 0x0002947C File Offset: 0x0002767C
	private void handleDamageUnit(EMDamageUnit m)
	{
		if (m is EMTerminateUnit && m.damageType != DamageType.TERMINAL)
		{
			return;
		}
		Unit unit = this.getUnit(m.targetTile);
		unit.setHitPoints(m.hp);
		unit.playAnimation("Damage");
		if (m.amount < 0)
		{
			return;
		}
		if (m.damageType == DamageType.MAGICAL || m.damageType == DamageType.SUPERIOR)
		{
			unit.damage(2);
		}
		if (m.attackType == AttackType.MELEE)
		{
			this.playBlastAnim(unit.transform, m.amount);
		}
		if (m.damageType == DamageType.TERMINAL)
		{
			this.addNumPop("red_kill", unit.transform.position);
		}
		else
		{
			this.addNumPop(m.amount, unit.transform.position, "red");
		}
		if (m.sourceCard == null || m.attackType != AttackType.BALLISTIC)
		{
			float screenShakeAmount = this.getScreenShakeAmount(m.amount);
			this.cameraShake(screenShakeAmount, this.damageYRatio * screenShakeAmount);
		}
		if (m is EMTerminateUnit)
		{
			unit.damage(2);
		}
	}

	// Token: 0x06000335 RID: 821 RVA: 0x00029598 File Offset: 0x00027798
	private void forceRunEffect(EffectMessage effectMessage)
	{
		this.currentEffect = effectMessage;
		this.currentEffectCount++;
		this.currentEffectStartTime = App.Clocks.battleModeClock.getTime();
		this.lastStartedEffectSequenceId = effectMessage.stampSequenceId();
		if (this.lastStartedEffectSequenceId <= this.lastFinishedEffectSequenceId)
		{
			Log.error(string.Concat(new object[]
			{
				"Started effect id <= finished effect id! ",
				this.lastStartedEffectSequenceId,
				" <= ",
				this.lastFinishedEffectSequenceId
			}));
			return;
		}
		string type = effectMessage.type;
		Log.info(string.Concat(new object[]
		{
			"Running effect (",
			this.effects.Count,
			"): ",
			type,
			" -- ",
			effectMessage,
			"\n",
			effectMessage.getRawText()
		}));
		try
		{
			string text = type;
			if (text != null)
			{
				if (BattleMode.<>f__switch$map10 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(41);
					dictionary.Add("Delay", 0);
					dictionary.Add("BattleText", 1);
					dictionary.Add("EndGame", 2);
					dictionary.Add("ResourcesUpdate", 3);
					dictionary.Add("TweenResource", 4);
					dictionary.Add("CardSacrificed", 5);
					dictionary.Add("TurnBegin", 6);
					dictionary.Add("SiegeAttackTiles", 7);
					dictionary.Add("UnitAttackTile", 8);
					dictionary.Add("UnitAttackIdol", 9);
					dictionary.Add("UnitAttackDone", 10);
					dictionary.Add("SummonUnit", 11);
					dictionary.Add("UnsummonUnit", 12);
					dictionary.Add("MoveUnit", 13);
					dictionary.Add("TeleportUnits", 14);
					dictionary.Add("DamageIdol", 15);
					dictionary.Add("HealIdol", 16);
					dictionary.Add("TerminateUnit", 17);
					dictionary.Add("DamageUnit", 18);
					dictionary.Add("EffectDelay", 19);
					dictionary.Add("RemoveUnit", 20);
					dictionary.Add("SurrenderIdolEffect", 21);
					dictionary.Add("ChatEffect", 22);
					dictionary.Add("EnchantUnit", 23);
					dictionary.Add("HealUnit", 24);
					dictionary.Add("Feedback", 25);
					dictionary.Add("UnitActivateAbility", 26);
					dictionary.Add("StatsUpdate", 27);
					dictionary.Add("CardStackUpdate", 28);
					dictionary.Add("IdolUpdate", 29);
					dictionary.Add("CardPlayed", 30);
					dictionary.Add("RuleAdded", 31);
					dictionary.Add("RuleUpdate", 32);
					dictionary.Add("RuleRemoved", 33);
					dictionary.Add("SelectedTiles", 34);
					dictionary.Add("TargetTiles", 35);
					dictionary.Add("SiftUpdate", 36);
					dictionary.Add("SiftClose", 37);
					dictionary.Add("HandUpdate", 38);
					dictionary.Add("CostUpdate", 39);
					dictionary.Add("GameState", 40);
					BattleMode.<>f__switch$map10 = dictionary;
				}
				int num;
				if (BattleMode.<>f__switch$map10.TryGetValue(text, ref num))
				{
					switch (num)
					{
					case 0:
					{
						EMDelay emdelay = (EMDelay)effectMessage;
						if (!emdelay.opponentOnly || this.activeColor != this.leftColor)
						{
							this.delayTimerStart = App.Clocks.battleModeClock.getTime();
							this.delayTime = emdelay.delayTime;
						}
						else
						{
							this.effectDone();
						}
						goto IL_1BB1;
					}
					case 1:
					{
						EMBattleText embattleText = (EMBattleText)effectMessage;
						BattleText battleText = new BattleText();
						battleText.init(embattleText.text, embattleText.pos, embattleText.startInSeconds, this);
						this._hasStartedGame = true;
						goto IL_1BB1;
					}
					case 2:
					{
						this._hasActiveGame = false;
						EMEndGame endGameStatistics = (EMEndGame)effectMessage;
						this.audioScript.StopSound(this.battleMusic, true);
						string name = this.leftPlayer.name;
						string name2 = this.rightPlayer.name;
						Avatar leftAvatar = this.battleUI.GetLeftAvatar();
						Avatar rightAvatar = this.battleUI.GetRightAvatar();
						this.closeSift();
						this.endGameScreen.init(this.gameMode, this.comm, this.gameType, this.leftColor, endGameStatistics, this.battleUI.GetLeftAvatar(), this.battleUI.GetRightAvatar(), this.leftPlayer.name, this.rightPlayer.name, Mathf.CeilToInt((float)this.currentTurn / 2f));
						this.setShowUnitStatsNoPersistence(false);
						this.battleUI.MoveOutAvatars();
						this.disableMulligan();
						this.effectDone();
						this.effects.endGame();
						goto IL_1BB1;
					}
					case 3:
					{
						EMResourcesUpdate emresourcesUpdate = (EMResourcesUpdate)effectMessage;
						this.updateResources(emresourcesUpdate.getAssets(this.leftColor), emresourcesUpdate.getAssets(this.rightColor));
						this.handManager.RefreshCardAffordabilities(this.battleUI.GetLeftPlayerResources());
						this.effectDone();
						goto IL_1BB1;
					}
					case 4:
						base.StartCoroutine(this._EffectDoneSoon(0.5f));
						goto IL_1BB1;
					case 5:
					{
						EMCardSacrificed emcardSacrificed = (EMCardSacrificed)effectMessage;
						if (!this.isRealPlayer(emcardSacrificed.color))
						{
							this.showMessage(this.getPlayer(emcardSacrificed.color).name + " sacrifices for " + ((!emcardSacrificed.isForCards()) ? "resources" : "scrolls"));
							this.battleUI.markSacrificedFor(this.isLeftColor(emcardSacrificed.color), emcardSacrificed.resource);
							this.insertDelay(0.75f, true);
						}
						this.effectDone();
						if (this.tutorial.onCardSacrificed(emcardSacrificed.resource))
						{
							this.nextTutorialSlide();
						}
						goto IL_1BB1;
					}
					case 6:
					{
						EMTurnBegin emturnBegin = (EMTurnBegin)effectMessage;
						this.currentTurn = emturnBegin.turn;
						this.audioScript.PlaySFX("Sounds/hyperduck/UI/ui_end_turn_00");
						this.numPopInfo.Clear();
						this.cleanupBoard();
						this.sacrificer = this.createSacrificer();
						this.activeColor = emturnBegin.color;
						this.setupTurnBegin(true);
						if (this.mulliganAvailable && this.isRealPlayer(this.activeColor) && this.currentTurn <= 2)
						{
							base.StartCoroutine(this.fadeMulliganButton(1f));
						}
						List<Transform> spellListFor = this.getSpellListFor(this.activeColor);
						float num2 = (float)((!this.isLeftColor(this.activeColor)) ? 5 : -5);
						for (int i = 0; i < spellListFor.Count; i++)
						{
							iTween.MoveTo(spellListFor[i].gameObject, iTween.Hash(new object[]
							{
								"x",
								2.43f,
								"y",
								5.53f,
								"z",
								num2,
								"time",
								0.3f,
								"easetype",
								iTween.EaseType.easeInExpo,
								"oncompletetarget",
								base.gameObject,
								"delay",
								(float)i * 0.1f,
								"oncomplete",
								"killObject",
								"oncompleteparams",
								spellListFor[i]
							}));
						}
						this.getSelectionHistory(this.activeColor).Clear();
						spellListFor.Clear();
						foreach (Unit unit in this.rightUnitsArr)
						{
							this.updateChargeAnimation(unit.getTilePosition());
						}
						foreach (Unit unit2 in this.leftUnitsArr)
						{
							this.updateChargeAnimation(unit2.getTilePosition());
						}
						this.effectDone();
						if (this.isMyTurn() && emturnBegin.sendEndPhase)
						{
							this.endPhase(EndPhaseMessage.Phase.PreMain);
						}
						this.setSecondsLeft(emturnBegin.secondsLeft);
						if (emturnBegin.isFake && emturnBegin.hasSacrificed)
						{
							this.sacrificer.sacrifice(ResourceType.CARDS);
						}
						this.battleUI.clearSacrificedFor(this.activeColor == this.leftColor);
						if (this.activeColor == this.leftColor && this.tutorial.onTurnBegin(this.currentTurn))
						{
							this.nextTutorialSlide();
						}
						goto IL_1BB1;
					}
					case 7:
					{
						EMSiegeAttackTiles emsiegeAttackTiles = (EMSiegeAttackTiles)effectMessage;
						this.getTile(emsiegeAttackTiles.source).setChargeAnimation(-1);
						float num3 = (!this.isLeftColor(emsiegeAttackTiles.source.color)) ? 0.8f : -0.8f;
						Vector3 vector = default(Vector3);
						foreach (TilePosition p in emsiegeAttackTiles.targets)
						{
							Tile tile = this.getTile(p);
							vector += tile.transform.position;
						}
						float num4 = 1f / (float)emsiegeAttackTiles.targets.Length;
						vector.Scale(new Vector3(num4, num4, num4));
						Vector3 vector2 = vector;
						Vector3 attackTo;
						attackTo..ctor(vector2.x, vector2.y, vector2.z + num3);
						this.getUnit(emsiegeAttackTiles.source).unitAttack(attackTo, num3);
						goto IL_1BB1;
					}
					case 8:
					{
						EMUnitAttackTile emunitAttackTile = (EMUnitAttackTile)effectMessage;
						this.getTile(emunitAttackTile.source).setChargeAnimation(-1);
						Tile tile2 = this.getTile(emunitAttackTile.target);
						Vector3 position = tile2.transform.position;
						float num5 = (!this.isLeftColor(emunitAttackTile.source.color)) ? 0.8f : -0.8f;
						Vector3 attackTo2;
						attackTo2..ctor(position.x, position.y, position.z + num5);
						Unit unit3 = this.getUnit(emunitAttackTile.source);
						this.currentAttackingUnit = unit3;
						unit3.unitAttack(attackTo2, num5);
						goto IL_1BB1;
					}
					case 9:
					{
						EMUnitAttackIdol emunitAttackIdol = (EMUnitAttackIdol)effectMessage;
						this.getTile(emunitAttackIdol.attacker).setChargeAnimation(-1);
						TileColor color = emunitAttackIdol.attacker.color.otherColor();
						Idol idol = this.getIdol(color, emunitAttackIdol.idol);
						Vector3 position2 = idol.getPosition();
						float num6 = (!this.isLeftColor(emunitAttackIdol.attacker.color)) ? 0.6f : -0.6f;
						Vector3 attackTo3;
						attackTo3..ctor(position2.x, position2.y, position2.z + num6);
						Unit unit4 = this.getUnit(emunitAttackIdol.attacker);
						this.currentAttackingUnit = unit4;
						unit4.unitAttack(attackTo3, num6);
						goto IL_1BB1;
					}
					case 10:
					{
						EMUnitAttackDone emunitAttackDone = (EMUnitAttackDone)effectMessage;
						Unit unit5 = this.getUnit(emunitAttackDone.source);
						unit5.attackDone();
						unit5.attackFullyCompleted();
						this.currentAttackingUnit = null;
						this.effectDone();
						goto IL_1BB1;
					}
					case 11:
					{
						EMSummonUnit emsummonUnit = (EMSummonUnit)effectMessage;
						Unit unit6 = this.summonUnit(emsummonUnit.card, App.Communicator.GetCardDownloadURL(), emsummonUnit.target);
						foreach (EMSelectedTiles emselectedTiles in this.getSelectionHistory(this.activeColor))
						{
							if (emselectedTiles.card.id == emsummonUnit.card.id)
							{
								emselectedTiles.units.Add(unit6);
							}
						}
						if (this.tutorial.onSummonUnit(unit6))
						{
							this.nextTutorialSlide();
						}
						goto IL_1BB1;
					}
					case 12:
					{
						EMUnsummonUnit emunsummonUnit = (EMUnsummonUnit)effectMessage;
						if (emunsummonUnit.target != null)
						{
							this.removeUnit(emunsummonUnit.target, RemovalType.Unsummon);
						}
						this.effectDone();
						goto IL_1BB1;
					}
					case 13:
					{
						EMMoveUnit emmoveUnit = (EMMoveUnit)effectMessage;
						if (this.hasUnit(emmoveUnit.from))
						{
							this.moveUnit(emmoveUnit.from, emmoveUnit.to, true);
							if (this.tutorial.onMoveUnit(this.getUnit(emmoveUnit.from)))
							{
								this.nextTutorialSlide();
							}
						}
						else
						{
							this.effectDone();
						}
						goto IL_1BB1;
					}
					case 14:
					{
						EMTeleportUnits emteleportUnits = (EMTeleportUnits)effectMessage;
						this.teleportUnits(emteleportUnits.units);
						foreach (TeleportInfo teleportInfo in emteleportUnits.units)
						{
							this.updateChargeAnimation(teleportInfo.to);
						}
						this.effectDone();
						goto IL_1BB1;
					}
					case 15:
					{
						EMDamageIdol emdamageIdol = (EMDamageIdol)effectMessage;
						Idol idol2 = this.getIdol(emdamageIdol.idol);
						this.effectDone();
						if (idol2.getHitPoints() <= 0)
						{
							return;
						}
						if (emdamageIdol.attackType == AttackType.MELEE)
						{
							this.playBlastAnim(idol2.transform, emdamageIdol.amount);
						}
						idol2.setHitPoints(emdamageIdol.idol.hp, this.isRealOpponent(emdamageIdol.idol.color), true);
						this.battleUI.Hurt(emdamageIdol.amount, this.isLeftColor(emdamageIdol.idol.color));
						float screenShakeAmount = this.getScreenShakeAmount(emdamageIdol.amount);
						this.cameraShake(screenShakeAmount, this.damageYRatio * screenShakeAmount, new Vector2(0f, (float)((!this.isRightColor(emdamageIdol.idol.color)) ? 1 : -1)));
						this.addNumPop(emdamageIdol.amount, idol2.transform.position, "red");
						goto IL_1BB1;
					}
					case 16:
					{
						EMHealIdol emhealIdol = (EMHealIdol)effectMessage;
						this.effectDone();
						Idol idol3 = this.getIdol(emhealIdol.idol);
						if (idol3.getHitPoints() <= 0)
						{
							return;
						}
						idol3.setHitPoints(emhealIdol.idol.hp, false, true);
						this.addNumPop(emhealIdol.amount, idol3.getPosition(), "green");
						this.createEffectAnimation("heal1", 94000, idol3.getPosition(), 1, new Vector3(0.25f, 0.25f, 0.25f), new Vector3(51f, 270f, 0f), false);
						goto IL_1BB1;
					}
					case 17:
						this.handleDamageUnit((EMTerminateUnit)effectMessage);
						this.effectDone();
						goto IL_1BB1;
					case 18:
					{
						EMDamageUnit emdamageUnit = (EMDamageUnit)effectMessage;
						this.handleDamageUnit(emdamageUnit);
						if (!emdamageUnit.isFake())
						{
							this.effectDone();
						}
						goto IL_1BB1;
					}
					case 19:
					{
						EMEffectDelay emeffectDelay = (EMEffectDelay)effectMessage;
						this.createEffectAnimation(emeffectDelay.name, 94000, this.getAllUnitsCopy()[0].transform.position, 1, new Vector3(0.4f, 0.4f, 0.4f), new Vector3(51f, 270f, 0f), false);
						goto IL_1BB1;
					}
					case 20:
					{
						EMRemoveUnit emremoveUnit = (EMRemoveUnit)effectMessage;
						this.removeUnit(emremoveUnit.tile, RemovalType.Kill);
						this.effectDone();
						goto IL_1BB1;
					}
					case 21:
					{
						EMSurrenderIdolEffect emsurrenderIdolEffect = (EMSurrenderIdolEffect)effectMessage;
						Idol idol4 = this.getIdol(emsurrenderIdolEffect.color, emsurrenderIdolEffect.idolId);
						if (!idol4.alive())
						{
							this.effectDone();
							goto IL_1BB1;
						}
						idol4.surrender();
						this.cameraShake(0.2f);
						int num7 = 94898 + emsurrenderIdolEffect.idolId;
						this.createEffectAnimation("transp", num7, idol4.transform.position, 1, new Vector3(0.25f, 0.25f, 0.25f), new Vector3(51f, 270f, 0f), false);
						this.createEffectAnimation("MushroomExplosion1/back", num7 + 1, idol4.transform.position, 1, new Vector3(0.25f, 0.25f, 0.25f), new Vector3(51f, 270f, 0f), false);
						this.createEffectAnimation("MushroomExplosion1/mid", num7 + 2, idol4.transform.position, 1, new Vector3(0.25f, 0.25f, 0.25f), new Vector3(51f, 270f, 0f), false);
						this.createEffectAnimation("MushroomExplosion1/front", num7 + 3, idol4.transform.position, 1, new Vector3(0.25f, 0.25f, 0.25f), new Vector3(51f, 270f, 0f), false);
						this.effectDoneSoon(0.15f);
						goto IL_1BB1;
					}
					case 22:
					{
						EMChatEffect emchatEffect = (EMChatEffect)effectMessage;
						this.handleGameChatMessage(emchatEffect.message);
						this.effectDone();
						goto IL_1BB1;
					}
					case 23:
					{
						EMEnchantUnit emenchantUnit = (EMEnchantUnit)effectMessage;
						this.getUnit(emenchantUnit.target).playEnchantmentAnim(emenchantUnit.tags());
						this.effectDone();
						goto IL_1BB1;
					}
					case 24:
					{
						EMHealUnit emhealUnit = (EMHealUnit)effectMessage;
						Unit unit7 = this.getUnit(emhealUnit.target);
						if (emhealUnit.showAnimation())
						{
							unit7.setHitPoints(emhealUnit.hp);
							unit7.heal();
							GameObject gameObject = new GameObject();
							gameObject.AddComponent<MeshRenderer>();
							gameObject.AddComponent<EffectPlayer>();
							EffectPlayer component = gameObject.GetComponent<EffectPlayer>();
							component.init("heal1", 1, this, 90000 + unit7.getTilePosition().row + 1, new Vector3(0.5f, 0.5f, 0.5f), 0);
							gameObject.transform.position = new Vector3(unit7.transform.position.x - 0.2f, unit7.transform.position.y + 0.2f, unit7.transform.position.z);
							gameObject.transform.eulerAngles = new Vector3(39f, 270f, 0f);
							gameObject.transform.parent = unit7.transform;
						}
						if (emhealUnit.showPopup())
						{
							this.addNumPop(emhealUnit.amount, unit7.transform.position, "green");
						}
						this.effectDone();
						goto IL_1BB1;
					}
					case 25:
					{
						float num8 = 0f;
						bool flag = true;
						EffectMessage msg = ((EMFeedback)effectMessage).msg;
						if (msg is EMUnitActivateAbility)
						{
							EMUnitActivateAbility emunitActivateAbility = (EMUnitActivateAbility)msg;
							this.battlePopup(emunitActivateAbility.unit, emunitActivateAbility.name);
							this.playUnitAnimation(emunitActivateAbility.unit, "ability_activate");
							flag = false;
						}
						if (msg is EMSelectedTiles)
						{
							EMSelectedTiles emselectedTiles2 = (EMSelectedTiles)msg;
							this.getSelectionHistory(emselectedTiles2.color).Add(emselectedTiles2);
							bool flag2 = this.lastActionType == BattleMode.ActionType.ActivateAbility;
							bool flag3 = !this.isRealPlayer(emselectedTiles2.color) && !flag2 && emselectedTiles2.tiles.Count > 0 && emselectedTiles2.area.isTileTarget() && emselectedTiles2.card.getCardType().kind.isSorcery();
							if (emselectedTiles2.area.isTileTarget())
							{
								foreach (TilePosition tilePosition in emselectedTiles2.tiles)
								{
									if (flag3)
									{
										this.battlePopup(tilePosition, emselectedTiles2.card.getName());
									}
									if (flag2)
									{
										this.playUnitAnimation(tilePosition, "ability_target");
									}
								}
								if (emselectedTiles2.tiles.Count > 0 && flag2)
								{
									num8 = 0.5f;
								}
								CardType cardType = emselectedTiles2.card.getCardType();
								string anim = null;
								if (cardType.getTag<string>("anim_cardplayed", ref anim))
								{
									TilePosition tilePosition2 = emselectedTiles2.tiles[0];
									int renderQueue = Unit.getRowRenderQueue(tilePosition2.row) + 1;
									this.playTileAnimation(tilePosition2, anim, renderQueue);
									flag = false;
								}
							}
						}
						if (flag)
						{
							if (num8 > 0f)
							{
								this.effectDoneSoon(num8);
							}
							else
							{
								this.effectDone();
							}
						}
						goto IL_1BB1;
					}
					case 26:
					{
						EMUnitActivateAbility emunitActivateAbility2 = (EMUnitActivateAbility)effectMessage;
						if (!emunitActivateAbility2.isMoveLike())
						{
							this.getUnit(emunitActivateAbility2.unit).playAnimation("ActivateAbility");
						}
						else
						{
							this.effectDone();
						}
						goto IL_1BB1;
					}
					case 27:
					{
						EMStatsUpdate emstatsUpdate = (EMStatsUpdate)effectMessage;
						Unit unit8 = this.getUnit(emstatsUpdate.target);
						unit8.setHitPoints(emstatsUpdate.hp);
						unit8.setAttackPower(emstatsUpdate.ap);
						unit8.setBuffs(emstatsUpdate.buffs);
						unit8.setAttackCounter(emstatsUpdate.ac);
						this.updateChargeAnimation(emstatsUpdate.target);
						this.effectDone();
						goto IL_1BB1;
					}
					case 28:
					{
						EMCardStackUpdate emcardStackUpdate = (EMCardStackUpdate)effectMessage;
						this.battleUI.UpdateStackSize(emcardStackUpdate.librarySize, emcardStackUpdate.graveyardSize, this.isLeftColor(emcardStackUpdate.color));
						this.effectDone();
						goto IL_1BB1;
					}
					case 29:
						this.effectDone();
						goto IL_1BB1;
					case 30:
					{
						EMCardPlayed emcardPlayed = (EMCardPlayed)effectMessage;
						Card card = emcardPlayed.card;
						int profileId = this.getActivePlayer().profileId;
						CardView cardView = this.createCardObject(card, profileId);
						cardView.setRenderQueue(0);
						Transform transform = cardView.gameObject.transform;
						transform.name = "PlayedCard";
						cardView.setStartFlying();
						CardType cardType2 = card.getCardType();
						if (cardType2.hasAnyTag(new string[]
						{
							"shake_cardplayed_x",
							"shake_cardplayed_y",
							"shake_cardplayed"
						}))
						{
							float tag = cardType2.getTag<float>("shake_cardplayed", 0f);
							this.cameraShake(cardType2.getTag<float>("shake_cardplayed_x", tag), cardType2.getTag<float>("shake_cardplayed_y", tag), Vector2.zero, cardType2.getTag<float>("shake_cardplayed_duration", 0.5f));
						}
						int num9 = (this.leftColor != this.activeColor) ? 1 : -1;
						Vector3 position3;
						position3..ctor(this.GUIObject.transform.position.x + 1.52f, this.GUIObject.transform.position.y - 3.13f, this.GUIObject.transform.position.z + 5.55f * (float)num9);
						transform.eulerAngles = new Vector3(39f, 90f, 0f);
						transform.localScale = new Vector3(0.145f, 0.001f, 0.2376f);
						transform.position = position3;
						this.getSpellListFor(this.activeColor).Add(transform);
						CardView.OpCastInfo opCastInfo = new CardView.OpCastInfo(this, num9);
						if (this.isRealPlayer(this.activeColor))
						{
							iTween.RotateTo(transform.gameObject, iTween.Hash(new object[]
							{
								"y",
								90f,
								"time",
								0.6f,
								"easetype",
								iTween.EaseType.easeOutExpo
							}));
							transform.gameObject.GetComponent<CardView>().animateOpCast(opCastInfo);
						}
						else
						{
							iTween.MoveTo(transform.gameObject, iTween.Hash(new object[]
							{
								"x",
								3.6f,
								"y",
								4.65f,
								"z",
								0,
								"time",
								0.6f,
								"easetype",
								iTween.EaseType.easeOutExpo,
								"oncompletetarget",
								transform.gameObject,
								"oncomplete",
								"animateOpCast",
								"oncompleteparams",
								opCastInfo
							}));
						}
						if (this.tutorial.onCardPlayed(card))
						{
							this.nextTutorialSlide();
						}
						goto IL_1BB1;
					}
					case 31:
						this.getActivePlayer().rules().add((EMRuleAdded)effectMessage);
						this.effectDone();
						goto IL_1BB1;
					case 32:
						this.getPlayer(TileColor.white).rules().update((EMRuleUpdate)effectMessage);
						this.getPlayer(TileColor.black).rules().update((EMRuleUpdate)effectMessage);
						this.effectDone();
						goto IL_1BB1;
					case 33:
						this.getPlayer(TileColor.white).rules().remove((EMRuleRemoved)effectMessage);
						this.getPlayer(TileColor.black).rules().remove((EMRuleRemoved)effectMessage);
						this.effectDone();
						goto IL_1BB1;
					case 34:
					{
						EMSelectedTiles emselectedTiles3 = (EMSelectedTiles)effectMessage;
						foreach (TilePosition p2 in emselectedTiles3.tiles)
						{
							Unit unit9 = this.getUnit(p2);
							if (unit9 != null)
							{
								emselectedTiles3.units.Add(unit9);
							}
						}
						this.effectDone();
						goto IL_1BB1;
					}
					case 35:
					{
						EMTargetTiles emtargetTiles = (EMTargetTiles)effectMessage;
						if (emtargetTiles.area == TargetArea.SEQUENTIAL)
						{
							foreach (EMSelectedTiles emselectedTiles4 in this.getSelectionHistory(this.activeColor))
							{
								if (emselectedTiles4.card.id == emtargetTiles.card.id)
								{
									foreach (TilePosition tilePosition3 in emtargetTiles.targets)
									{
										if (!emselectedTiles4.tiles.Contains(tilePosition3))
										{
											emselectedTiles4.tiles.AddRange(emtargetTiles.targets);
										}
									}
								}
							}
						}
						TileColor color2 = emtargetTiles.targets[0].color;
						this.playCardEffect(emtargetTiles.area, color2, emtargetTiles.animationType, this.getTiles(emtargetTiles.targets));
						goto IL_1BB1;
					}
					case 36:
						this.setSubState(BattleMode.SubState.Sift);
						this.handleSiftUpdate(((EMSiftUpdate)effectMessage).cards);
						this.effectDone();
						goto IL_1BB1;
					case 37:
						this.closeSift();
						this.effectDone();
						goto IL_1BB1;
					case 38:
						TimedLog.SetThreshold(10000L);
						TimedLog.Begin();
						this.handleHandUpdate((EMHandUpdate)effectMessage);
						TimedLog.End();
						this.effectDone();
						goto IL_1BB1;
					case 39:
						this.handleCostUpdate((EMCostUpdate)effectMessage);
						this.effectDone();
						goto IL_1BB1;
					case 40:
					{
						EMGameState emgameState = (EMGameState)effectMessage;
						this.handleGameState(emgameState.state);
						this.effectDone();
						goto IL_1BB1;
					}
					}
				}
			}
			if (effectMessage is MovedOutLogic_Effect)
			{
				MovedOutLogic_Effect movedOutLogic_Effect = effectMessage as MovedOutLogic_Effect;
				EffectDone ed = new EffectDone(this, this.lastStartedEffectSequenceId);
				movedOutLogic_Effect.eval(this, ed);
			}
			else
			{
				this.effectDone();
			}
			IL_1BB1:;
		}
		catch (Exception ex)
		{
			Log.error(string.Concat(new object[]
			{
				"EFFECT ERROR: ",
				ex,
				"\n::",
				effectMessage.getRawText()
			}));
		}
	}

	// Token: 0x06000336 RID: 822 RVA: 0x0002B23C File Offset: 0x0002943C
	private void closeSift()
	{
		SiftOverlay component = base.gameObject.GetComponent<SiftOverlay>();
		if (component == null)
		{
			return;
		}
		this.setSubState(BattleMode.SubState.Normal);
		component.close();
	}

	// Token: 0x06000337 RID: 823 RVA: 0x0002B270 File Offset: 0x00029470
	public void playHitAnimation()
	{
		EffectMessage effectMessage = this.effects.PeekFirst();
		Log.warning("PEEK: " + effectMessage);
		if (!(effectMessage is EMDamageUnit))
		{
			return;
		}
		this.effects.PushFirst(((EMDamageUnit)effectMessage).createFakeHit());
		this.effectDone();
	}

	// Token: 0x06000338 RID: 824 RVA: 0x0002B2C4 File Offset: 0x000294C4
	private void handleSiftUpdate(Card[] cards)
	{
		bool allowInput = this.gameMode == GameMode.Play;
		base.gameObject.AddComponent<SiftOverlay>().init(cards, this, allowInput);
	}

	// Token: 0x06000339 RID: 825 RVA: 0x0002B2F0 File Offset: 0x000294F0
	private void handleHandUpdate(EMHandUpdate m)
	{
		bool flag = this.isSpectateOrReplay();
		if (flag)
		{
			if (m.profileId == this.getPlayer(this.activeColor).profileId)
			{
				ResourceGroup availableResources = this.battleUI.GetResources(this.isLeftColor(this.activeColor)).availableResources;
				this.handManager.SetHand(m.cards, availableResources, m.profileId);
			}
		}
		else if (this.isPlayer(m.profileId))
		{
			this.lastHandSize = m.cards.Length;
			this.handManager.SetHand(m.cards, this.battleUI.GetLeftPlayerResources(), m.profileId);
			if (this.tutorial.onHandUpdate())
			{
				this.nextTutorialSlide();
			}
		}
	}

	// Token: 0x0600033A RID: 826 RVA: 0x0002B3B8 File Offset: 0x000295B8
	private void addNumPop(int amount, Vector3 pos, string colorType)
	{
		float numpopDelayFor = this.getNumpopDelayFor(pos);
		GameObject gameObject = new GameObject();
		NumPop numPop = gameObject.AddComponent<NumPop>();
		numPop.init(amount, pos, colorType, numpopDelayFor);
		this.numPopInfo.Add(new NumPop.PosTime(pos, Time.time + numpopDelayFor));
	}

	// Token: 0x0600033B RID: 827 RVA: 0x0002B3FC File Offset: 0x000295FC
	private void addNumPop(string resourceName, Vector3 pos)
	{
		float numpopDelayFor = this.getNumpopDelayFor(pos);
		GameObject gameObject = new GameObject();
		NumPop numPop = gameObject.AddComponent<NumPop>();
		numPop.init(resourceName, pos, numpopDelayFor);
		this.numPopInfo.Add(new NumPop.PosTime(pos, Time.time + numpopDelayFor));
	}

	// Token: 0x0600033C RID: 828 RVA: 0x0002B440 File Offset: 0x00029640
	private float getNumpopDelayFor(Vector3 p)
	{
		for (int i = this.numPopInfo.Count - 1; i >= 0; i--)
		{
			NumPop.PosTime posTime = this.numPopInfo[i];
			if (posTime.position == p && posTime.time + 0.5f >= Time.time)
			{
				return posTime.time + 0.5f - Time.time;
			}
		}
		return 0f;
	}

	// Token: 0x0600033D RID: 829 RVA: 0x000028DF File Offset: 0x00000ADF
	private void removeParticleSystem()
	{
	}

	// Token: 0x0600033E RID: 830 RVA: 0x0002B4B8 File Offset: 0x000296B8
	private void pstest(TilePosition src, TilePosition[] dstTiles)
	{
		Vector3 position = this.getUnit(src).transform.position;
		foreach (TilePosition p in dstTiles)
		{
			GameObject gameObject = Object.Instantiate(this.PSModifierAffect, position, Quaternion.identity) as GameObject;
			ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
			component.Play();
			Vector3 position2 = this.getUnit(p).transform.position;
			float magnitude = (position2 - position).magnitude;
			iTween.MoveTo(gameObject, iTween.Hash(new object[]
			{
				"position",
				position2,
				"easetype",
				iTween.EaseType.linear,
				"time",
				this.PSLifeTime * Mathf.Sqrt(0.5f + magnitude),
				"oncompletetarget",
				base.gameObject,
				"oncomplete",
				"removeParticleSystem"
			}));
		}
	}

	// Token: 0x0600033F RID: 831 RVA: 0x0002B5BC File Offset: 0x000297BC
	private void updateChargeAnimation(TilePosition p)
	{
		int chargeAnimation = this.calculateChargeAnimation(p);
		this.getTile(p).setChargeAnimation(chargeAnimation);
	}

	// Token: 0x06000340 RID: 832 RVA: 0x0002B5E0 File Offset: 0x000297E0
	private int calculateChargeAnimation(TilePosition p)
	{
		Unit unit = this.getUnit(p);
		if (unit == null || unit.getOrigAttackInterval() <= 0)
		{
			return -1;
		}
		int attackInterval = unit.getAttackInterval();
		if (p.color == this.activeColor)
		{
			return (attackInterval != 0) ? -1 : 1;
		}
		if (this.isLeftColor(p.color) && this.gameMode != GameMode.Spectate)
		{
			return -1;
		}
		if (attackInterval == 0 || (attackInterval == 1 && unit.isCountingDownAutomatically()))
		{
			return 0;
		}
		if (unit.hasTag("attack_every_round"))
		{
			return 0;
		}
		return -1;
	}

	// Token: 0x06000341 RID: 833 RVA: 0x0002B684 File Offset: 0x00029884
	private void showUnitRule(Unit unit)
	{
		this.deselectAllTiles();
		if (unit == this.currentRuleShown)
		{
			this.HideCardView();
			return;
		}
		this.unitRuleShowing = true;
		this.currentRuleShown = unit;
		if (this.checkDestroyCardRule())
		{
			this.showCardRule(unit, this.getPlayer(unit.getTilePosition().color).profileId);
		}
	}

	// Token: 0x06000342 RID: 834 RVA: 0x0002B6E8 File Offset: 0x000298E8
	private void showCardRule(object cardOrUnit, int profileId)
	{
		if (!this.checkDestroyCardRule())
		{
			Object.Destroy(this.cardRule);
		}
		this.cardRule = PrimitiveFactory.createPlane();
		this.cardRule.name = "CardRule";
		CardView cardView = this.cardRule.AddComponent<CardView>();
		if (this.gameType.isTutorial())
		{
			cardView.setShader(Shaders.fnMilkBurn);
		}
		if (cardOrUnit is Card)
		{
			cardView.overrideCost(this.getCostForCard((Card)cardOrUnit, profileId));
			cardView.init(this, (Card)cardOrUnit, 190);
		}
		else
		{
			if (!(cardOrUnit is Unit))
			{
				throw new ArgumentException("cardOrUnit must be of type Card or Unit");
			}
			cardView.overrideCost(this.getCostForCard(((Unit)cardOrUnit).getCard(), profileId));
			cardView.init(this, (Unit)cardOrUnit, 190);
		}
		cardView.enableShowHelp();
		cardView.applyHighResTexture();
		cardView.setLayer(10);
		cardView.setRaycastCamera(this.uiCamera);
		Vector3 vector = CardView.CardLocalScale();
		float num = (float)Screen.height * 0.65f;
		float num2 = num * vector.x / vector.z;
		Rect dst;
		dst..ctor((float)Screen.width - num2, (float)Screen.height * 0.05f, num2, num);
		this.uiGui.DrawObject(dst, this.cardRule);
	}

	// Token: 0x06000343 RID: 835 RVA: 0x000028DF File Offset: 0x00000ADF
	public void createTutorialFrame()
	{
	}

	// Token: 0x06000344 RID: 836 RVA: 0x000044F0 File Offset: 0x000026F0
	public void aiDelay()
	{
		this.aiDelay(0.5f);
	}

	// Token: 0x06000345 RID: 837 RVA: 0x000044FD File Offset: 0x000026FD
	public void aiDelay(float delay)
	{
		if (this.isReplay())
		{
			this.addDelay(delay);
		}
		else if (this.gameType.isSinglePlayer())
		{
			this.opponentDelay(delay);
		}
	}

	// Token: 0x06000346 RID: 838 RVA: 0x0000452D File Offset: 0x0000272D
	public void opponentDelay()
	{
		this.opponentDelay(0.5f);
	}

	// Token: 0x06000347 RID: 839 RVA: 0x0000453A File Offset: 0x0000273A
	public void opponentDelay(float delay)
	{
		this.addDelay(delay, true);
	}

	// Token: 0x06000348 RID: 840 RVA: 0x00004544 File Offset: 0x00002744
	private void addDelay(float delay)
	{
		this.addDelay(delay, false);
	}

	// Token: 0x06000349 RID: 841 RVA: 0x0000454E File Offset: 0x0000274E
	private void addDelay(float delay, bool opponentOnly)
	{
		this.effects.Add(new EMDelay(delay, opponentOnly));
	}

	// Token: 0x0600034A RID: 842 RVA: 0x00004562 File Offset: 0x00002762
	public void insertDelay(float delay)
	{
		this.insertDelay(delay, false);
	}

	// Token: 0x0600034B RID: 843 RVA: 0x0000456C File Offset: 0x0000276C
	public void insertDelay(float delay, bool opponentOnly)
	{
		this.effects.PushFirst(new EMDelay(delay, opponentOnly));
	}

	// Token: 0x0600034C RID: 844 RVA: 0x00004580 File Offset: 0x00002780
	private void fadeOutCardRule()
	{
		if (this.cardRule == null)
		{
			return;
		}
		if (!this.tutorial.allowHideCardView())
		{
			return;
		}
		this.cardRule.GetComponent<CardView>().fadeOut();
	}

	// Token: 0x0600034D RID: 845 RVA: 0x000045B5 File Offset: 0x000027B5
	public void HideCardView()
	{
		if (!this.checkDestroyCardRule())
		{
			return;
		}
		this.unitRuleShowing = false;
		this.currentRuleShown = null;
	}

	// Token: 0x0600034E RID: 846 RVA: 0x000045D1 File Offset: 0x000027D1
	public void ActivateTriggeredAbility(string id, TilePosition pos)
	{
		if (!this.isMyTurn())
		{
			return;
		}
		this.activeAbilityPosition = pos;
		this.sendBattleRequest(new ActivateAbilityInfoMessage(id, pos));
		if (!ActiveAbility.isMoveLike(id))
		{
			this.deselectAllTiles();
		}
	}

	// Token: 0x0600034F RID: 847 RVA: 0x00004604 File Offset: 0x00002804
	private void showMessage(string mess)
	{
		this.serverMessages.Add(new BattleMode.ServerMessage(mess));
	}

	// Token: 0x06000350 RID: 848 RVA: 0x00004617 File Offset: 0x00002817
	private void runWatchdogs()
	{
		this.runEffectWatchdog();
	}

	// Token: 0x06000351 RID: 849 RVA: 0x0002B840 File Offset: 0x00029A40
	private void runEffectWatchdog()
	{
		if (this.currentEffect == null)
		{
			return;
		}
		float num = App.Clocks.battleModeClock.getTime() - this.currentEffectStartTime;
		if (num >= this.currentEffect.timeoutSeconds())
		{
			this.effectDone();
		}
	}

	// Token: 0x06000352 RID: 850 RVA: 0x0000461F File Offset: 0x0000281F
	private void FixedUpdate()
	{
		this.runWatchdogs();
	}

	// Token: 0x06000353 RID: 851 RVA: 0x0002B888 File Offset: 0x00029A88
	private void Update()
	{
		ComponentAttacher<TutorialTicker.Tag> componentAttacher = this.historyBlinker;
		List<TutorialTicker.Tag> list = new List<TutorialTicker.Tag>();
		list.Add(TutorialTicker.Tag.Blink_Unit);
		componentAttacher.update(list);
		float num = Time.deltaTime / 0.2f;
		if (this.isLeftColor(this.activeColor))
		{
			this.leftNameBorderAlpha += num;
			this.rightNameBorderAlpha -= num;
		}
		else if (this.isRightColor(this.activeColor))
		{
			this.leftNameBorderAlpha -= num;
			this.rightNameBorderAlpha += num;
		}
		this.leftNameBorderAlpha = Mathf.Clamp01(this.leftNameBorderAlpha);
		this.rightNameBorderAlpha = Mathf.Clamp01(this.rightNameBorderAlpha);
		if (this.delayTime != 0f)
		{
			float time = App.Clocks.battleModeClock.getTime();
			if (Input.GetKey(304))
			{
				Log.warning(string.Concat(new object[]
				{
					"delay: ",
					this.delayTime,
					", ",
					time,
					";  ",
					this.delayTimerStart
				}));
			}
			if (time >= this.delayTimerStart + this.delayTime)
			{
				this.delayTime = 0f;
				this.effectDone();
			}
		}
		if (this.handManager != null && this.handManager.GetCardViewsInHand().Count > 0 && !this.tutorialStarted && !this.handManager.IsAnyCardMoving())
		{
			this.createTutorialFrame();
			this.tutorialStarted = true;
		}
		this._updateTutorial();
		this.handleKeyInput();
		if (this.isSpectate())
		{
			this.handManager.RaiseCards(App.ChatUI.IsShown());
		}
		if (this.isReplay())
		{
			this.handManager.RaiseCards((!App.ChatUI.IsShown()) ? 0f : 0.38f);
		}
		if (this.isReplay())
		{
			Time.timeScale = (float)((!Input.GetKey(102)) ? 1 : 99);
			if (Input.GetKeyDown(49))
			{
				this.replayNexts = 0;
			}
			if (Input.GetKeyDown(50) && this.replayNexts < 2147483647)
			{
				this.replayNexts++;
			}
			if (Input.GetKeyDown(51))
			{
				this.replayNexts = int.MaxValue;
			}
		}
		if (this.gameMode == GameMode.Play && (Input.GetKeyDown(13) || Input.GetKeyDown(271)) && !this.menu.showMenu)
		{
			if (this.showChatInput && this.chatString.Length > 0)
			{
				this.closeChatInput();
			}
			else if (this.showChat && this.showChatInput)
			{
				this.setChatActive(false);
			}
			else if (this.showChat && !this.showChatInput)
			{
				this.setChatActive(true, true);
			}
			else
			{
				this.setChatActive(true, true);
			}
		}
		if (Input.GetKeyDown(27))
		{
			if (this.showChatInput)
			{
				this.closeChatInput(false);
			}
			else
			{
				this.menu.toggleMenu();
			}
		}
		bool flag = true;
		if (flag)
		{
			this.handleMouseInput();
		}
		List<BattleMode.AnimatedText> list2 = new List<BattleMode.AnimatedText>();
		for (int i = 0; i < this.animatedTexts.Count; i++)
		{
			BattleMode.AnimatedText animatedText = this.animatedTexts[i];
			animatedText.t += Time.deltaTime / 2f;
			if (animatedText.t > 1f)
			{
				list2.Add(animatedText);
			}
		}
		foreach (BattleMode.AnimatedText animatedText2 in list2)
		{
			this.animatedTexts.Remove(animatedText2);
		}
		this.runEffect();
	}

	// Token: 0x06000354 RID: 852 RVA: 0x0002BCB0 File Offset: 0x00029EB0
	private void handleKeyInput()
	{
		this.handleEditorKeyInput();
		if (Input.GetKeyDown(306) || Input.GetKeyDown(305))
		{
			this.toggleUnitStats();
		}
		if (Input.GetKeyDown(308) || Input.GetKeyDown(307))
		{
			this.toggleUnitStats();
		}
		if (Input.GetKeyUp(308) || Input.GetKeyUp(307))
		{
			this.toggleUnitStats();
		}
		if (Input.GetKeyUp(283))
		{
			this.showUserDebugInfo = !this.showUserDebugInfo;
		}
		this.handleReplayKeyInput();
	}

	// Token: 0x06000355 RID: 853 RVA: 0x0002BD54 File Offset: 0x00029F54
	private void handleReplayKeyInput()
	{
		if (!this.isReplay())
		{
			return;
		}
		if (App.ChatUI.IsShown())
		{
			return;
		}
		if (this.replayFastForward)
		{
			App.GlobalMessageHandler.setTimeScale(1000f);
		}
		else
		{
			App.GlobalMessageHandler.setTimeScale(1f);
		}
		this.replayFastForward = false;
		if (Input.GetKeyUp(284))
		{
			this.showReplayControls = !this.showReplayControls;
		}
		if (Input.GetKeyUp(276))
		{
			this.restartReplay();
		}
		if (Input.GetKeyUp(273))
		{
			this.replayNexts = ((this.replayNexts == 0) ? int.MaxValue : 0);
		}
		if (Input.GetKeyUp(275))
		{
			this.replayNexts = int.MaxValue;
			App.GlobalMessageHandler.setTimeScale(1f);
		}
		this.replayFastForward |= Input.GetKey(275);
	}

	// Token: 0x06000356 RID: 854 RVA: 0x0002BE50 File Offset: 0x0002A050
	private void handleEditorKeyInput()
	{
		if (!Application.isEditor)
		{
			return;
		}
		if (Input.GetKeyDown(281))
		{
			this.setupBackground(null, this.gameId + (long)this.bgIndexOffset++);
		}
		if (Input.GetKeyDown(113))
		{
			this.blastAnimStrength = Mathf.Clamp(this.blastAnimStrength + 1, 0, 9);
		}
		if (Input.GetKeyDown(122))
		{
			this.blastAnimStrength = Mathf.Clamp(this.blastAnimStrength - 1, 0, 9);
		}
		if (Input.GetKeyDown(52))
		{
			foreach (Unit unit in this.getAllUnitsCopy())
			{
				unit.playAnimation("Attack");
			}
		}
		if (Input.GetKeyDown(103))
		{
			List<Unit> allUnitsCopy = this.getAllUnitsCopy();
			if (allUnitsCopy.Count > 0)
			{
				this.playUnitParticles(RandomUtil.choice<Unit>(allUnitsCopy).getTilePosition(), "Explosion 01");
			}
		}
		if (UnityUtil.GetKeyDown(114, new KeyCode[]
		{
			308
		}))
		{
			this.gotoMockEndScreen();
		}
		if (Input.GetKeyDown(122))
		{
			this.getActivePlayer().rules().clear();
		}
		if (Input.GetKeyDown(120))
		{
			EMRuleAdded emruleAdded = new EMRuleAdded();
			CardType type = RandomUtil.choice<CardType>(CardTypeManager.getInstance().getAll());
			emruleAdded.card = new Card((long)(RandomUtil.random() * 1E+09f), type);
			emruleAdded.color = this.activeColor;
			emruleAdded.roundsLeft = (int)(10f * RandomUtil.random());
			this.getActivePlayer().rules().add(emruleAdded);
		}
		if (Input.GetKeyDown(97))
		{
			foreach (Unit unit2 in this.getUnitsFor(this.leftColor))
			{
				this.playBlastAnim(unit2.transform, this.blastAnimStrength);
			}
		}
		if (Input.GetKeyDown(100))
		{
			foreach (Unit unit3 in this.getUnitsFor(this.rightColor))
			{
				this.playBlastAnim(unit3.transform, this.blastAnimStrength);
			}
		}
		if (Input.GetKeyDown(119))
		{
			foreach (Unit unit4 in this.getUnitsFor(this.leftColor))
			{
				this.playUnitAnimation(unit4.getTilePosition(), "ability_activate");
			}
			EMFeedback msg = new EMFeedback(new EMUnitActivateAbility(this.getUnitsFor(this.rightColor)[0], "Monger"));
			this.addEffect(msg);
		}
		if (Input.GetKeyDown(115))
		{
			this.shake.shake(8f, 8f);
		}
		if (Input.GetKeyDown(119))
		{
			TilePosition p = new TilePosition(this.leftColor, 2, 1);
			Tile tile = this.getTile(p);
			this.playTileAnimation(p, "gunpowder_explosion_1");
			FlashingLight.Create(tile.transform.position, Color.yellow, 0.5f);
		}
		if (Input.GetKeyDown(109))
		{
			TargetArea targetArea = TargetArea.RADIUS_7;
			TileColor color = this.leftColor;
			TargetAreaAnimationType animationType = TargetAreaAnimationType.DEFAULT;
			List<Tile> list = new List<Tile>();
			list.Add(this.getTile(new TilePosition(this.leftColor, 2, 1)));
			this.playCardEffect(targetArea, color, animationType, list);
		}
		if (Input.GetKeyDown(285))
		{
			this.restartReplay();
		}
	}

	// Token: 0x06000357 RID: 855 RVA: 0x0002C244 File Offset: 0x0002A444
	private void gotoMockEndScreen()
	{
		while (this.effects.Count > 0)
		{
			this.effects.PopFirst();
		}
		EMEndGame emendGame = new EMEndGame();
		emendGame.setRawText(string.Empty);
		emendGame.winner = TileColor.white;
		emendGame.whiteStats = new GameStatistics();
		emendGame.whiteStats.profileId = 40039;
		emendGame.whiteStats.idolDamage = 46;
		emendGame.whiteStats.unitDamage = 31;
		emendGame.whiteStats.unitsPlayed = 20;
		emendGame.whiteStats.scrollsDrawn = 67;
		emendGame.whiteStats.totalMs = 269910L;
		emendGame.whiteStats.mostDamageUnit = 4;
		emendGame.whiteStats.mostDamageUnitId = 14489161;
		emendGame.blackStats = new GameStatistics();
		emendGame.blackStats.profileId = 40055;
		emendGame.blackStats.idolDamage = 0;
		emendGame.blackStats.unitDamage = 15;
		emendGame.blackStats.unitsPlayed = 9;
		emendGame.blackStats.spellsPlayed = 3;
		emendGame.blackStats.scrollsDrawn = 53;
		emendGame.blackStats.totalMs = 844L;
		emendGame.blackStats.mostDamageUnit = 3;
		emendGame.blackStats.mostDamageUnitId = 8371;
		emendGame.whiteGoldReward = new GameRewardStatistics();
		emendGame.whiteGoldReward.matchReward = 42;
		emendGame.whiteGoldReward.tierMatchReward = 21;
		emendGame.whiteGoldReward.matchCompletionReward = 25;
		emendGame.whiteGoldReward.idolsDestroyedReward = 0;
		emendGame.cardRewards = new Card[]
		{
			new Card(14878326L, CardTypeManager.getInstance().get(371))
		};
		this.addEffect(emendGame);
	}

	// Token: 0x06000358 RID: 856 RVA: 0x00004627 File Offset: 0x00002827
	private void setBattleText(string text, Vector3 pos, float startInSeconds)
	{
		this.addEffect(new EMBattleText(text, pos, startInSeconds));
	}

	// Token: 0x06000359 RID: 857 RVA: 0x00004637 File Offset: 0x00002837
	private void cancelScrollAndAbility()
	{
		this.HideCardView();
		this.handManager.DeselectCard();
		this.deselectAllTiles();
	}

	// Token: 0x0600035A RID: 858 RVA: 0x00004650 File Offset: 0x00002850
	public bool allowEndTurn()
	{
		return this.tutorial.allowEndTurn();
	}

	// Token: 0x0600035B RID: 859 RVA: 0x0000465D File Offset: 0x0000285D
	public void endturnPressed()
	{
		if (!this.isMyTurn())
		{
			return;
		}
		this.endTurn();
		this.handManager.DeselectCard();
	}

	// Token: 0x0600035C RID: 860 RVA: 0x0000467C File Offset: 0x0000287C
	public bool isInputBlocked()
	{
		return this.tutorialBlocker;
	}

	// Token: 0x0600035D RID: 861 RVA: 0x0002C3FC File Offset: 0x0002A5FC
	private void onMousePressed(int button)
	{
		if (button == 0)
		{
			bool flag = false;
			if (this.gameMode == GameMode.Play || !App.ChatUI.IsHovered())
			{
				CardView cardView = this.handManager.RayCast(out flag);
				if (cardView != null)
				{
					this.cardClicked(cardView, 0);
					return;
				}
			}
			if (flag)
			{
				return;
			}
			RaycastHit raycastHitForDefault = this.getRaycastHitForDefault();
			if (raycastHitForDefault.collider == null)
			{
				return;
			}
			if (raycastHitForDefault.collider.gameObject != this.cardRule && this.cardRule != null && raycastHitForDefault.collider.transform.parent != this.cardRule.transform)
			{
				this.HideCardView();
			}
			if (!this.tutorialBlocker)
			{
				Tile component = raycastHitForDefault.collider.gameObject.GetComponent<Tile>();
				if (component != null)
				{
					this.tileClicked(component);
				}
			}
		}
		if (button == 1)
		{
			Collider collider = new Collider();
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] array = Physics.RaycastAll(ray);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].collider.gameObject.name == "Card")
				{
					collider = array[i].collider;
					break;
				}
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].collider.gameObject.name == "Card" && array[i].collider.transform.position.z > collider.transform.position.z)
				{
					collider = array[i].collider;
				}
			}
			if (collider != null && !this.handManager.IsAnyCardMoving() && !this.gameType.isTutorial())
			{
				this.cardClicked(collider.gameObject.GetComponent<CardView>(), 1);
			}
			else
			{
				this.cancelScrollAndAbility();
			}
		}
	}

	// Token: 0x0600035E RID: 862 RVA: 0x0002C64C File Offset: 0x0002A84C
	private bool canClickThrough(Vector2 p)
	{
		if (this.isReplay())
		{
			Rect replayControlRect = BattleMode.getReplayControlRect();
			replayControlRect.yMax = (float)Screen.height;
			if (replayControlRect.Contains(p))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600035F RID: 863 RVA: 0x0002C688 File Offset: 0x0002A888
	private void handleMouseInput()
	{
		if (this.showChatInput || this.endGameScreen.isInited() || this.menu.showMenu)
		{
			return;
		}
		if (!this.canClickThrough(GUIUtil.getScreenMousePos()))
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			this.onMousePressed(0);
		}
		else if (Input.GetMouseButtonDown(1))
		{
			this.onMousePressed(1);
		}
		else if (this.gameMode == GameMode.Play || !App.ChatUI.IsHovered())
		{
			bool flag;
			CardView cardView = this.handManager.RayCast(out flag);
			if (cardView == null && this.subState != BattleMode.SubState.Sift && !this.handleRayCast(this.getRaycastHitForDefault()) && !this.unitRuleShowing)
			{
				this.fadeOutCardRule();
			}
		}
		else
		{
			this.handManager.DehoverCard();
		}
	}

	// Token: 0x06000360 RID: 864 RVA: 0x0002C778 File Offset: 0x0002A978
	private bool handleRayCast(Ray ray, int layerMask)
	{
		RaycastHit hit = default(RaycastHit);
		Physics.Raycast(ray, ref hit, float.PositiveInfinity, layerMask);
		return this.handleRayCast(hit);
	}

	// Token: 0x06000361 RID: 865 RVA: 0x00004684 File Offset: 0x00002884
	private RaycastHit getRaycastHitForDefault()
	{
		return UnityUtil.getRaycastHitFor(new RayInfo[]
		{
			new RayInfo(this.uiCamera, Input.mousePosition, 1536),
			new RayInfo(Camera.main, Input.mousePosition, -5)
		});
	}

	// Token: 0x06000362 RID: 866 RVA: 0x0002C7A4 File Offset: 0x0002A9A4
	private bool handleRayCast(RaycastHit hit)
	{
		if (hit.collider == null)
		{
			return false;
		}
		bool flag = false;
		Collider collider = null;
		Collider collider2 = null;
		string name = hit.collider.gameObject.name;
		if (name != null)
		{
			if (BattleMode.<>f__switch$map11 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
				dictionary.Add("CardRule", 0);
				dictionary.Add("Trigger_Ability_Button", 1);
				dictionary.Add("PlayedCard", 2);
				dictionary.Add("LingeringSpell", 3);
				dictionary.Add("Idol", 4);
				BattleMode.<>f__switch$map11 = dictionary;
			}
			int num;
			if (BattleMode.<>f__switch$map11.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
				{
					collider2 = hit.collider;
					CardView component = this.cardRule.GetComponent<CardView>();
					component.setTransparency(1f);
					if (component.getCardId() == this.lastHitPlayedCardId)
					{
						flag = true;
					}
					break;
				}
				case 1:
				{
					collider2 = hit.collider.transform.parent.collider;
					CardView component2 = this.cardRule.GetComponent<CardView>();
					component2.setTransparency(1f);
					break;
				}
				case 2:
				{
					CardView component3 = hit.collider.gameObject.GetComponent<CardView>();
					if (!component3.isFlying())
					{
						flag = true;
						collider = hit.collider;
						if (this.cardRule == null || (this.cardRule != null && this.cardRule.GetComponent<CardView>().getCardId() != component3.getCardId()))
						{
							this.lastHitPlayedCardId = component3.getCardId();
							this.handleSelectionHistory(component3.getCardId());
							this.showCardRule(component3.getCardInfo(), component3.profileId());
						}
					}
					break;
				}
				case 3:
				{
					PersistentRuleCardView component4 = hit.collider.gameObject.GetComponent<PersistentRuleCardView>();
					collider2 = hit.collider;
					if (this.cardRule == null || (this.cardRule != null && this.cardRule.GetComponent<CardView>().getCardId() != component4.card().getId()))
					{
						this.showCardRule(component4.card(), this.getPlayer(component4.playerColor()).profileId);
					}
					break;
				}
				}
			}
		}
		if (!flag)
		{
			this.historyBlinker.setUnits(new List<GameObject>());
			if (this.lastHitPlayedCardId != -1L)
			{
				foreach (Tile tile in this.allTiles)
				{
					tile.unmark();
				}
				this.lastHitPlayedCardId = -1L;
			}
		}
		if (this.cardRule != null && hit.collider.gameObject.name.StartsWith("3dPassive_"))
		{
			collider2 = hit.collider.transform.parent.collider;
			CardView component5 = this.cardRule.GetComponent<CardView>();
			component5.setTransparency(1f);
		}
		return collider != null || collider2 != null;
	}

	// Token: 0x06000363 RID: 867 RVA: 0x0002CAF8 File Offset: 0x0002ACF8
	private void handleSelectionHistory(long cardId)
	{
		EMSelectedTiles emselectedTiles = Enumerable.FirstOrDefault<EMSelectedTiles>(this.leftTargets, (EMSelectedTiles x) => x.card.id == cardId);
		if (emselectedTiles == null)
		{
			emselectedTiles = Enumerable.FirstOrDefault<EMSelectedTiles>(this.rightTargets, (EMSelectedTiles x) => x.card.id == cardId);
		}
		if (emselectedTiles == null)
		{
			return;
		}
		if (emselectedTiles.tiles.Count == 0 && emselectedTiles.units.Count == 0)
		{
			return;
		}
		List<GameObject> list = new List<GameObject>();
		if (emselectedTiles.area.isTileTarget())
		{
			foreach (Unit unit in emselectedTiles.units)
			{
				if (!unit.isDestroyed() && !(unit != this.getUnit(unit.getTilePosition())))
				{
					list.Add(unit.gameObject);
				}
			}
			if (list.Count == 0)
			{
				this.getTile(emselectedTiles.tiles[0]).mark(Tile.SelectionType.Target);
			}
		}
		else
		{
			List<TilePosition> list2 = new List<TilePosition>();
			foreach (TilePosition p in emselectedTiles.tiles)
			{
				list2.AddRange(emselectedTiles.area.getTargets(p));
			}
			foreach (Tile tile in this.getTiles(list2))
			{
				tile.mark(Tile.SelectionType.Target);
			}
		}
		this.historyBlinker.setUnits(list);
	}

	// Token: 0x06000364 RID: 868 RVA: 0x0002CCE8 File Offset: 0x0002AEE8
	private void _updateTutorial()
	{
		if (!this.gameType.isTutorial())
		{
			return;
		}
		if (this.pendingMoveNextTutorialSlide)
		{
			this.tutorial.next();
			this.updateTutorialBlink(true);
			this.pendingMoveNextTutorialSlide = false;
		}
		if (this.tutorialBlinker != null)
		{
			this.updateTutorialBlink(false);
		}
	}

	// Token: 0x06000365 RID: 869 RVA: 0x0002CD3C File Offset: 0x0002AF3C
	private EffectPlayer createGuiAnimation(string animation, string animationId, Vector2 pos)
	{
		Gui3D gui3D = new Gui3D(this.uiCamera);
		Vector3 baseScale = Vector3.one * ((float)Screen.height * 0.046f);
		GameObject gameObject = BattleMode.createEffectAnimation(null, animation, 98100, Vector3.zero, 1, baseScale, Vector3.zero, false);
		gui3D.DrawObject(pos.x, pos.y, gameObject);
		gameObject.transform.localEulerAngles = Vector3.zero;
		EffectPlayer component = gameObject.GetComponent<EffectPlayer>();
		component.layer = 9;
		if (animationId != null)
		{
			component.getAnimPlayer().setAnimationId(animationId);
		}
		return component;
	}

	// Token: 0x06000366 RID: 870 RVA: 0x0002CDD0 File Offset: 0x0002AFD0
	public bool allowPlayCard()
	{
		CardView selectedCard = this.handManager.GetSelectedCard();
		return selectedCard != null && this.tutorial.allowPlayCard(selectedCard.getCardInfo());
	}

	// Token: 0x06000367 RID: 871 RVA: 0x0002CE0C File Offset: 0x0002B00C
	public bool allowSacrifice(ResourceType resource)
	{
		if (!this.tutorial.allowSacrifice(resource))
		{
			return false;
		}
		if (resource.isCards())
		{
			return this.handManager.GetHandSize() <= this.maxScrollsForCycle;
		}
		if (this.gameType.hasWildResources() && resource == ResourceType.SPECIAL && !this.alwaysWild)
		{
			ResourceGroup outputResources = this.battleUI.GetResources(true).outputResources;
			int num = outputResources.get(ResourceType.SPECIAL) + 1;
			foreach (ResourceType resourceType in this.resTypes)
			{
				if (resourceType != ResourceType.SPECIAL)
				{
					if (1 * outputResources.get(resourceType) < num)
					{
						return false;
					}
				}
			}
			return true;
		}
		return true;
	}

	// Token: 0x06000368 RID: 872 RVA: 0x0002CEFC File Offset: 0x0002B0FC
	public string getResourceTooltip(ResourceType resource)
	{
		if (this.gameType.isTutorial())
		{
			return null;
		}
		if (this.allowSacrifice(resource))
		{
			return null;
		}
		if (resource.isCards())
		{
			return "You can only sacrifice for scrolls if you have " + this.maxScrollsForCycle + " or less scrolls in hand.";
		}
		if (resource == ResourceType.SPECIAL)
		{
			return "Wild cannot be increased above your other resources.";
		}
		return "can not sacrifice. report this error!";
	}

	// Token: 0x06000369 RID: 873 RVA: 0x000046BD File Offset: 0x000028BD
	private static string animName(ResourceType r)
	{
		if (r == ResourceType.SPECIAL)
		{
			return "wild";
		}
		return r.ToString().ToLower();
	}

	// Token: 0x0600036A RID: 874 RVA: 0x0002CF64 File Offset: 0x0002B164
	public void resourceTweenComplete(ResourceType resource)
	{
		this.battleUI.markSacrificedFor(true, resource);
		if (resource.isCards())
		{
			return;
		}
		string animationId = "resource_fx_" + BattleMode.animName(resource);
		Vector2 center = this.getSacrificeDestRect(resource).center;
		this.createGuiAnimation("sacrifice_glow_icon", animationId, center);
	}

	// Token: 0x0600036B RID: 875 RVA: 0x0002CFBC File Offset: 0x0002B1BC
	public void glowResourceIcon(ResourceType resource, Vector3 worldPos)
	{
		string animationId = "res_sac_fx_" + BattleMode.animName(resource);
		Vector2 screenMousePos = GUIUtil.getScreenMousePos(this.uiCamera.WorldToScreenPoint(worldPos));
		this.createGuiAnimation("sacrifice_glow_icon_0", animationId, screenMousePos);
	}

	// Token: 0x0600036C RID: 876 RVA: 0x0002D000 File Offset: 0x0002B200
	private void OnGUI_updateTutorial()
	{
		if (!this.tutorial.isRunning())
		{
			return;
		}
		if (this.isScriptedTutorial && this.endGameScreen.isInited())
		{
			return;
		}
		this.tutorialBlocker = false;
		if (this.tutorial.isBlocking())
		{
			this.tutorialBlocker = true;
			if (GUI.Button(this.lastFrameOKButton, string.Empty))
			{
				this.nextTutorialSlide();
			}
			if (GUI.Button(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), string.Empty) && !this.tutorial.isRunning())
			{
				this.setBattleText("BattleStart", new Vector3(-7f, 0.75f, -5f), 2f);
				this.tutorialBlocker = false;
			}
		}
		if (this.tutorial.getText() != null)
		{
			this.OnGUI_drawTutorialText(this.tutorial.getText(), this.tutorialBlocker);
		}
		GUI.color = Color.white;
	}

	// Token: 0x0600036D RID: 877 RVA: 0x0002D10C File Offset: 0x0002B30C
	private void OnGUI_drawTutorialText(string text, bool block)
	{
		int fontSize = Screen.height / 30;
		GUI.skin.label.fontSize = fontSize;
		GUI.skin.label.normal.textColor = new Color(0.23f, 0.16f, 0.125f);
		GUI.skin.label.alignment = 4;
		float num = 1469f * ((float)Screen.height / 1700f);
		float num2 = 309f * ((float)Screen.height / 1700f);
		Rect rect;
		rect..ctor(((float)Screen.width - num) / 2f, (float)Screen.height * 0.01f, num, num2);
		if (this.gameType.isTutorial() && !this.isScriptedTutorial)
		{
			rect = GeomUtil.cropShare(rect, new Rect(0f, -0.05f, 1f, 0.5f));
		}
		Rect rect2 = GeomUtil.cropShare(rect, new Rect(0.05f, 0.025f, 0.9f, 0.95f));
		this.tutorialLabelStyle.fontSize = Screen.height / 30;
		new ScrollsFrame(rect).AddNinePatch(ScrollsFrame.Border.LIGHT_CURVED, NinePatch.Patches.CENTER).Draw();
		GUI.Label(rect2, text, this.tutorialLabelStyle);
		if (block)
		{
			GUISkin skin = GUI.skin;
			GUI.skin = this.regularUI;
			Rect rect3;
			rect3..ctor(rect2.x + rect2.width / 2f - (float)Screen.height * 0.065f, rect2.yMax - (float)Screen.height * 0.02f, (float)Screen.height * 0.13f, (float)Screen.height * 0.05f);
			GUI.Box(rect3, string.Empty);
			this.lastFrameOKButton = new Rect(rect2.x + rect2.width / 2f - (float)Screen.height * 0.06f, rect2.yMax - (float)Screen.height * 0.015f, (float)Screen.height * 0.12f, (float)Screen.height * 0.04f);
			int fontSize2 = GUI.skin.button.fontSize;
			GUI.skin.button.fontSize = Screen.height / 40;
			if (GUI.Button(this.lastFrameOKButton, "Continue"))
			{
			}
			GUI.skin.button.fontSize = fontSize2;
			GUI.skin = skin;
		}
	}

	// Token: 0x0600036E RID: 878 RVA: 0x0002D36C File Offset: 0x0002B56C
	private void OnGUI_drawNameBoxes()
	{
		GUISkin skin = GUI.skin;
		int num = Screen.height / 30;
		GUI.skin.label.fontSize = num;
		float num2 = Mathf.Max(GUI.skin.label.CalcSize(new GUIContent(this.leftPlayer.name)).x, GUI.skin.label.CalcSize(new GUIContent(this.rightPlayer.name)).x);
		float num3 = Mathf.Max((float)Screen.height * 0.28f, num2 + (float)Screen.height * 0.13f);
		GUI.color = new Color(1f, 1f, 1f, 0.75f);
		Rect rect;
		rect..ctor((float)Screen.width * 0.5f - num3, (float)Screen.height * 0.027f, num3, (float)Screen.height * 0.046f);
		Rect rect2;
		rect2..ctor((float)Screen.width * 0.5f, (float)Screen.height * 0.027f, num3, (float)Screen.height * 0.046f);
		GUI.Box(rect, string.Empty, this.battleUISkin.box);
		GUI.Box(rect2, string.Empty, this.battleUISkin.box);
		if (this.leftNameBorderAlpha > 0f)
		{
			GUI.color = new Color(1f, 1f, 1f, 0.75f * this.leftNameBorderAlpha);
			GUI.Box(rect, string.Empty, this.battleUIBoxStyleSelected);
		}
		if (this.rightNameBorderAlpha > 0f)
		{
			GUI.color = new Color(1f, 1f, 1f, 0.75f * this.rightNameBorderAlpha);
			GUI.Box(rect2, string.Empty, this.battleUIBoxStyleSelected);
		}
		GUI.color = Color.white;
		float num4 = (float)Screen.width * 0.5f - num3 + (float)Screen.height * 0.05f;
		float num5 = (float)Screen.width * 0.5f + num3 - (float)Screen.height * 0.05f;
		float num6 = (float)Screen.height * 0.029f;
		float num7 = (float)num * 1.2f;
		Color textColor;
		textColor..ctor(1f, 0.9f, 0.8f);
		GUI.skin.label.alignment = 3;
		GUIUtil.drawBorderedText(new Rect(num4, num6, num3, num7), this.leftPlayer.name, textColor);
		GUI.skin.label.alignment = 5;
		GUIUtil.drawBorderedText(new Rect(num5 - num3, num6, num3, num7), this.rightPlayer.name, textColor);
	}

	// Token: 0x0600036F RID: 879 RVA: 0x0002D614 File Offset: 0x0002B814
	private bool drawBox(Rect rect, int fontSize, string text, Texture icon, bool textIsLeft)
	{
		GUISkin skin = GUI.skin;
		GUI.skin = this.battleUISkin;
		GUI.color = new Color(1f, 1f, 1f, 0.75f);
		GUI.Box(rect, string.Empty);
		GUI.color = Color.white;
		GUI.skin = skin;
		GUI.skin.label.fontSize = fontSize;
		GUI.skin.label.alignment = 4;
		Rect rect2 = GeomUtil.cropShare(rect, new Rect(0.4f, 0f, 0.4f, 1f));
		Rect rect3 = GeomUtil.cropShare(rect, new Rect(-0.4f, -0.25f, 0.8f, 1.5f));
		if (textIsLeft)
		{
			rect2 = GeomUtil.cropShare(rect, new Rect(0.2f, 0f, 0.4f, 1f));
			rect3 = GeomUtil.cropShare(rect, new Rect(0.6f, -0.25f, 0.8f, 1.5f));
		}
		GUI.Label(rect2, text);
		return GUI.Button(rect3, icon);
	}

	// Token: 0x06000370 RID: 880 RVA: 0x000046DC File Offset: 0x000028DC
	private void moveUnit(TilePosition from, TilePosition to, bool tween)
	{
		this.moveUnit(this.getUnit(from), from, to, tween);
	}

	// Token: 0x06000371 RID: 881 RVA: 0x0002D724 File Offset: 0x0002B924
	private void moveUnit(Unit unit, TilePosition from, TilePosition to, bool tween)
	{
		this.getTile(from).setChargeAnimation(-1);
		Vector3 vector;
		vector..ctor(0.15f, 0f, (!this.isLeftColor(to.color)) ? -0.05f : 0.05f);
		Vector3 position = this.getTile(to).transform.position + vector;
		position.y = unit.transform.position.y;
		unit.setTilePosition(to);
		unit.setZPos(position.z);
		if (!tween)
		{
			unit.gameObject.transform.position = position;
			this.updateChargeAnimation(to);
			return;
		}
		unit.moveUnitStart();
		iTween.MoveTo(unit.gameObject, iTween.Hash(new object[]
		{
			"x",
			position.x,
			"z",
			position.z,
			"time",
			this.movementTweenTime,
			"easetype",
			this.movementEaseType.ToString(),
			"oncompletetarget",
			base.gameObject,
			"oncomplete",
			"moveUnitComplete",
			"oncompleteparams",
			unit
		}));
	}

	// Token: 0x06000372 RID: 882 RVA: 0x000046EE File Offset: 0x000028EE
	private void moveUnitComplete(Unit unit)
	{
		unit.moveUnitComplete();
		this.updateChargeAnimation(unit.getTilePosition());
	}

	// Token: 0x06000373 RID: 883 RVA: 0x0002D884 File Offset: 0x0002BA84
	private void teleportUnit(TeleportInfo teleport)
	{
		Unit unit = (!(teleport.unit != null)) ? this.getUnit(teleport.from) : teleport.unit;
		this.playTileAnimation(teleport.from, "Unsummon");
		this.moveUnit(unit, teleport.from, teleport.to, false);
		this.playTileAnimation(teleport.to, "Unsummon");
	}

	// Token: 0x06000374 RID: 884 RVA: 0x0002D8F4 File Offset: 0x0002BAF4
	private void teleportUnits(TeleportInfo[] teleports)
	{
		foreach (TeleportInfo teleportInfo in teleports)
		{
			teleportInfo.unit = this.getUnit(teleportInfo.from);
		}
		foreach (TeleportInfo teleport in teleports)
		{
			this.teleportUnit(teleport);
		}
	}

	// Token: 0x06000375 RID: 885 RVA: 0x0002D958 File Offset: 0x0002BB58
	private Sacrificer createSacrificer()
	{
		return this._createSacrificer();
	}

	// Token: 0x06000376 RID: 886 RVA: 0x0002D970 File Offset: 0x0002BB70
	private Sacrificer _createSacrificer()
	{
		if (this.alwaysWild)
		{
			return new AlwaysWildSacrificer();
		}
		bool multiColorSacrifice = false;
		return new SacrificeSet(multiColorSacrifice);
	}

	// Token: 0x06000377 RID: 887 RVA: 0x00004702 File Offset: 0x00002902
	public void siftCard(Card card)
	{
		this.sendBattleRequest(new SiftCardMessage(card));
		this.handManager.RemoveSelectedCard();
	}

	// Token: 0x06000378 RID: 888 RVA: 0x0000471B File Offset: 0x0000291B
	public CardView create(Card card)
	{
		return this.createCardObject(card, this.getActivePlayer().profileId).enableShowHelp();
	}

	// Token: 0x06000379 RID: 889 RVA: 0x00004734 File Offset: 0x00002934
	public bool allowInviteNotification(InviteManager.WaitingInvite w)
	{
		return this.gameMode != GameMode.Play;
	}

	// Token: 0x0400017A RID: 378
	private const int ROWS = 5;

	// Token: 0x0400017B RID: 379
	private const int COLS = 3;

	// Token: 0x0400017C RID: 380
	private const int NO_OF_IDOLS = 5;

	// Token: 0x0400017D RID: 381
	private const int STATE_TILE_SELECT = 0;

	// Token: 0x0400017E RID: 382
	private const int STATE_RESOURCE_SELECT = 1;

	// Token: 0x0400017F RID: 383
	private const int STATE_RESOURCE_OUTPUT_SELECT = 2;

	// Token: 0x04000180 RID: 384
	private const float CHAT_FADEOUT_DELAY = 6f;

	// Token: 0x04000181 RID: 385
	private const float RoundTimeNull = -999f;

	// Token: 0x04000182 RID: 386
	private Vector2 chatPosition = new Vector2(0.01f, 0.656f);

	// Token: 0x04000183 RID: 387
	private iTween.EaseType movementEaseType = iTween.EaseType.easeInOutSine;

	// Token: 0x04000184 RID: 388
	private float movementTweenTime = 0.26f;

	// Token: 0x04000185 RID: 389
	[SerializeField]
	private Camera uiCamera;

	// Token: 0x04000186 RID: 390
	[SerializeField]
	private GameObject PSModifierAffect;

	// Token: 0x04000187 RID: 391
	public ScreenShake shake;

	// Token: 0x04000188 RID: 392
	private float ExplosionScale = 0.015f;

	// Token: 0x04000189 RID: 393
	private Gui3D uiGui;

	// Token: 0x0400018A RID: 394
	public Color overColor = ColorUtil.FromInts(249, 225, 116);

	// Token: 0x0400018B RID: 395
	public Color underColor = ColorUtil.FromInts(209, 173, 246);

	// Token: 0x0400018C RID: 396
	public static int LAYER_NOLIGHT = 20;

	// Token: 0x0400018D RID: 397
	public Transform tile;

	// Token: 0x0400018E RID: 398
	private MiniCommunicator comm;

	// Token: 0x0400018F RID: 399
	private bool _hasActiveGame = true;

	// Token: 0x04000190 RID: 400
	private bool showClock;

	// Token: 0x04000191 RID: 401
	private GameType gameType;

	// Token: 0x04000192 RID: 402
	private GameMode gameMode;

	// Token: 0x04000193 RID: 403
	private ITutorial tutorial = new EmptyTutorial();

	// Token: 0x04000194 RID: 404
	private bool isScriptedTutorial;

	// Token: 0x04000195 RID: 405
	private Blinker tutorialBlinker;

	// Token: 0x04000196 RID: 406
	private Color currentResourceColor;

	// Token: 0x04000197 RID: 407
	private GUISkin battleModeSkin;

	// Token: 0x04000198 RID: 408
	private GUISkin battleChat;

	// Token: 0x04000199 RID: 409
	private GUISkin battleUISkin;

	// Token: 0x0400019A RID: 410
	private GUIStyle battleUIBoxStyleSelected;

	// Token: 0x0400019B RID: 411
	private GUIStyle tutorialLabelStyle;

	// Token: 0x0400019C RID: 412
	private List<NumPop.PosTime> numPopInfo = new List<NumPop.PosTime>();

	// Token: 0x0400019D RID: 413
	private string activeAbilityId;

	// Token: 0x0400019E RID: 414
	private TilePosition activeAbilityPosition = new TilePosition(TileColor.white, 0, 0);

	// Token: 0x0400019F RID: 415
	private int replayNexts = 99999999;

	// Token: 0x040001A0 RID: 416
	private bool replayFastForward;

	// Token: 0x040001A1 RID: 417
	private TileColor playerColor = TileColor.unknown;

	// Token: 0x040001A2 RID: 418
	private TileColor leftColor;

	// Token: 0x040001A3 RID: 419
	private TileColor rightColor = TileColor.black;

	// Token: 0x040001A4 RID: 420
	private TileColor activeColor = TileColor.unknown;

	// Token: 0x040001A5 RID: 421
	private List<IEnumerable<TilePosition>> activeTileGroups = new List<IEnumerable<TilePosition>>();

	// Token: 0x040001A6 RID: 422
	private List<Unit> leftUnitsArr = new List<Unit>();

	// Token: 0x040001A7 RID: 423
	private List<Unit> rightUnitsArr = new List<Unit>();

	// Token: 0x040001A8 RID: 424
	private List<List<Tile>> leftTileArr = new List<List<Tile>>();

	// Token: 0x040001A9 RID: 425
	private List<List<Tile>> rightTileArr = new List<List<Tile>>();

	// Token: 0x040001AA RID: 426
	private List<Tile> allTiles = new List<Tile>();

	// Token: 0x040001AB RID: 427
	private List<Idol> rightIdolsArr = new List<Idol>();

	// Token: 0x040001AC RID: 428
	private List<Idol> leftIdolsArr = new List<Idol>();

	// Token: 0x040001AD RID: 429
	private List<Transform> leftSpellListArr = new List<Transform>();

	// Token: 0x040001AE RID: 430
	private List<Transform> rightSpellListArr = new List<Transform>();

	// Token: 0x040001AF RID: 431
	private List<EMSelectedTiles> leftTargets = new List<EMSelectedTiles>();

	// Token: 0x040001B0 RID: 432
	private List<EMSelectedTiles> rightTargets = new List<EMSelectedTiles>();

	// Token: 0x040001B1 RID: 433
	private GameObject goField;

	// Token: 0x040001B2 RID: 434
	private GameObject goLeftBoard;

	// Token: 0x040001B3 RID: 435
	private GameObject goLeftIdols;

	// Token: 0x040001B4 RID: 436
	private GameObject goLeftTiles;

	// Token: 0x040001B5 RID: 437
	private GameObject goRightBoard;

	// Token: 0x040001B6 RID: 438
	private GameObject goRightIdols;

	// Token: 0x040001B7 RID: 439
	private GameObject goRightTiles;

	// Token: 0x040001B8 RID: 440
	private TileSelector tileSelector = new TileSelector();

	// Token: 0x040001B9 RID: 441
	private List<BattleMode.ServerMessage> serverMessages = new List<BattleMode.ServerMessage>();

	// Token: 0x040001BA RID: 442
	private HandManager handManager;

	// Token: 0x040001BB RID: 443
	private int playerProfileId;

	// Token: 0x040001BC RID: 444
	private long gameId = -1L;

	// Token: 0x040001BD RID: 445
	private string gameServerAddress = string.Empty;

	// Token: 0x040001BE RID: 446
	private int gameServerPort;

	// Token: 0x040001BF RID: 447
	private bool showUnitStats;

	// Token: 0x040001C0 RID: 448
	private Unit currentRuleShown;

	// Token: 0x040001C1 RID: 449
	private GameObject lightSource;

	// Token: 0x040001C2 RID: 450
	private GameObject lightSource2;

	// Token: 0x040001C3 RID: 451
	private float roundTimer = -999f;

	// Token: 0x040001C4 RID: 452
	private int roundTime = -1;

	// Token: 0x040001C5 RID: 453
	private Sacrificer sacrificer;

	// Token: 0x040001C6 RID: 454
	private bool tutorialStarted;

	// Token: 0x040001C7 RID: 455
	private string mouseLabelHead;

	// Token: 0x040001C8 RID: 456
	private string mouseLabelBody;

	// Token: 0x040001C9 RID: 457
	private float mouseLabelTimer;

	// Token: 0x040001CA RID: 458
	private GUISkin labelSkin;

	// Token: 0x040001CB RID: 459
	private GUISkin regularUI;

	// Token: 0x040001CC RID: 460
	private List<ResourceType> resTypes = new List<ResourceType>();

	// Token: 0x040001CD RID: 461
	private GUIStyle btnRestartStyle;

	// Token: 0x040001CE RID: 462
	private GUIStyle btnFfStyle;

	// Token: 0x040001CF RID: 463
	private GUIStyle btnPlayStyle;

	// Token: 0x040001D0 RID: 464
	private GUIStyle btnPauseStyle;

	// Token: 0x040001D1 RID: 465
	private BMPlayer leftPlayer = new BMPlayer(true);

	// Token: 0x040001D2 RID: 466
	private BMPlayer rightPlayer = new BMPlayer(false);

	// Token: 0x040001D3 RID: 467
	private BackgroundData currentBgData;

	// Token: 0x040001D4 RID: 468
	private EndGameScreen endGameScreen;

	// Token: 0x040001D5 RID: 469
	private bool showToolTip;

	// Token: 0x040001D6 RID: 470
	private GUIBattleModeMenu menu;

	// Token: 0x040001D7 RID: 471
	private GameObject GUIObject;

	// Token: 0x040001D8 RID: 472
	private GUIClock clock;

	// Token: 0x040001D9 RID: 473
	private AudioScript audioScript;

	// Token: 0x040001DA RID: 474
	private int rewardForIdolKill;

	// Token: 0x040001DB RID: 475
	public BattleModeUI battleUI;

	// Token: 0x040001DC RID: 476
	private int currentTurn;

	// Token: 0x040001DD RID: 477
	private FpsCounter fps = new FpsCounter();

	// Token: 0x040001DE RID: 478
	private string battleMusic;

	// Token: 0x040001DF RID: 479
	private bool spectatorListIsShowing;

	// Token: 0x040001E0 RID: 480
	private List<string> spectators = new List<string>();

	// Token: 0x040001E1 RID: 481
	private Tags customSettings = new Tags();

	// Token: 0x040001E2 RID: 482
	private bool showReplayControls = true;

	// Token: 0x040001E3 RID: 483
	private float showReplayAlpha = 1f;

	// Token: 0x040001E4 RID: 484
	private BattleMode.SubState subState;

	// Token: 0x040001E5 RID: 485
	private BattleMode.ActionType lastActionType;

	// Token: 0x040001E6 RID: 486
	private Card lastCardPlayed;

	// Token: 0x040001E7 RID: 487
	private EMUnitActivateAbility lastActivatedAbility;

	// Token: 0x040001E8 RID: 488
	private Blinker historyBlinker;

	// Token: 0x040001E9 RID: 489
	private Tile lastTileClicked;

	// Token: 0x040001EA RID: 490
	public float accumulatedTierRewardMultiplier;

	// Token: 0x040001EB RID: 491
	public float maxTierRewardMultiplier;

	// Token: 0x040001EC RID: 492
	public float[] tierRewardMultiplierDelta;

	// Token: 0x040001ED RID: 493
	private int maxScrollsForCycle;

	// Token: 0x040001EE RID: 494
	private List<BattleMode.AnimatedText> animatedTexts = new List<BattleMode.AnimatedText>();

	// Token: 0x040001EF RID: 495
	private int currentSpectators;

	// Token: 0x040001F0 RID: 496
	private int bgIndexOffset;

	// Token: 0x040001F1 RID: 497
	private List<GameObject> bgObjects = new List<GameObject>();

	// Token: 0x040001F2 RID: 498
	private bool _hasStartedGame;

	// Token: 0x040001F3 RID: 499
	private bool boardSet;

	// Token: 0x040001F4 RID: 500
	private int lastHandSize = -1;

	// Token: 0x040001F5 RID: 501
	private List<EffectMessage> _tmpEffects;

	// Token: 0x040001F6 RID: 502
	private int _tmpEffectId = -1;

	// Token: 0x040001F7 RID: 503
	private bool requestedGameState;

	// Token: 0x040001F8 RID: 504
	private bool _serverRestarted;

	// Token: 0x040001F9 RID: 505
	private bool showChat;

	// Token: 0x040001FA RID: 506
	private bool showChatInput;

	// Token: 0x040001FB RID: 507
	private bool fadeChat;

	// Token: 0x040001FC RID: 508
	private string chatLog = string.Empty;

	// Token: 0x040001FD RID: 509
	private string chatString = string.Empty;

	// Token: 0x040001FE RID: 510
	private float chatOpacity;

	// Token: 0x040001FF RID: 511
	private float chatLastMessageSentAtTime;

	// Token: 0x04000200 RID: 512
	private bool showUserDebugInfo;

	// Token: 0x04000201 RID: 513
	private bool pendingMoveNextTutorialSlide;

	// Token: 0x04000202 RID: 514
	private long lastTutorialBlinkTime = long.MinValue;

	// Token: 0x04000203 RID: 515
	private Vector2 chatScroll = new Vector2(0f, float.PositiveInfinity);

	// Token: 0x04000204 RID: 516
	private Vector2 specScroll = new Vector2(0f, float.PositiveInfinity);

	// Token: 0x04000205 RID: 517
	private bool mulliganAvailable = true;

	// Token: 0x04000206 RID: 518
	private float mulliganAlpha;

	// Token: 0x04000207 RID: 519
	private Unit currentAttackingUnit;

	// Token: 0x04000208 RID: 520
	private EffectList effects = new EffectList();

	// Token: 0x04000209 RID: 521
	private EffectMessage currentEffect;

	// Token: 0x0400020A RID: 522
	private int currentEffectCount;

	// Token: 0x0400020B RID: 523
	private int currentEffectGroup;

	// Token: 0x0400020C RID: 524
	private int lastStartedEffectSequenceId = -1;

	// Token: 0x0400020D RID: 525
	private int lastFinishedEffectSequenceId = -1;

	// Token: 0x0400020E RID: 526
	private float currentEffectStartTime;

	// Token: 0x0400020F RID: 527
	private float delayTimerStart;

	// Token: 0x04000210 RID: 528
	private float delayTime;

	// Token: 0x04000211 RID: 529
	public float damageShakeMultiplier = 1f;

	// Token: 0x04000212 RID: 530
	public float damageYRatio = 0.5f;

	// Token: 0x04000213 RID: 531
	[SerializeField]
	private float PSLifeTime = 0.4f;

	// Token: 0x04000214 RID: 532
	private GameObject cardRule;

	// Token: 0x04000215 RID: 533
	private bool unitRuleShowing;

	// Token: 0x04000216 RID: 534
	private int blastAnimStrength = 1;

	// Token: 0x04000217 RID: 535
	private bool tutorialBlocker;

	// Token: 0x04000218 RID: 536
	private long lastHitPlayedCardId = -1L;

	// Token: 0x04000219 RID: 537
	private Rect lastFrameOKButton = default(Rect);

	// Token: 0x0400021A RID: 538
	private float leftNameBorderAlpha;

	// Token: 0x0400021B RID: 539
	private float rightNameBorderAlpha;

	// Token: 0x0400021C RID: 540
	private bool alwaysWild;

	// Token: 0x02000043 RID: 67
	public enum ActionType
	{
		// Token: 0x04000220 RID: 544
		None,
		// Token: 0x04000221 RID: 545
		CardPlayed,
		// Token: 0x04000222 RID: 546
		ActivateAbility
	}

	// Token: 0x02000044 RID: 68
	public enum SubState
	{
		// Token: 0x04000224 RID: 548
		Normal,
		// Token: 0x04000225 RID: 549
		Sift
	}

	// Token: 0x02000045 RID: 69
	private class AnimatedText
	{
		// Token: 0x0600037A RID: 890 RVA: 0x0002D998 File Offset: 0x0002BB98
		public AnimatedText(Vector3 pos, string text, string color)
		{
			this.startPos = pos;
			this.targetPos = pos + new Vector3(0f, 80f, 0f);
			this.text = text;
			this.color = color;
			this.t = 0f;
		}

		// Token: 0x04000226 RID: 550
		public Vector3 startPos;

		// Token: 0x04000227 RID: 551
		public Vector3 targetPos;

		// Token: 0x04000228 RID: 552
		public string color;

		// Token: 0x04000229 RID: 553
		public string text;

		// Token: 0x0400022A RID: 554
		public float t;
	}

	// Token: 0x02000046 RID: 70
	private class ServerMessage
	{
		// Token: 0x0600037B RID: 891 RVA: 0x0002D9EC File Offset: 0x0002BBEC
		public ServerMessage(string message)
		{
			this.text = message;
			this.startTime = Time.time;
			this.skin = ScriptableObject.CreateInstance<GUISkin>();
			this.skin.label.wordWrap = true;
			this.skin.label.font = (Font)ResourceManager.Load("Fonts/HoneyMeadBB_bold", typeof(Font));
			this.skin.label.fontSize = 32;
			this.skin.label.alignment = 1;
			this.skin.label.normal.textColor = new Color(1f, 1f, 1f, 1f);
		}

		// Token: 0x0400022B RID: 555
		public string text;

		// Token: 0x0400022C RID: 556
		public float startTime;

		// Token: 0x0400022D RID: 557
		public GUISkin skin;
	}
}
