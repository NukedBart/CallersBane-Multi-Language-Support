using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gui;
using UnityEngine;

// Token: 0x020003A4 RID: 932
public class ProfileMenu : AbstractCommListener
{
	// Token: 0x060014F1 RID: 5361 RVA: 0x00080B9C File Offset: 0x0007ED9C
	private void Start()
	{
		Application.targetFrameRate = 60;
		App.ChatUI.Show(false);
		this._emptySkin = (GUISkin)ResourceManager.Load("_GUISkins/EmptySkin");
		this._buttonSkin = (GUISkin)ResourceManager.Load("_GUISkins/Lobby");
		this._regularUI = (GUISkin)ResourceManager.Load("_GUISkins/RegularUI");
		GUIStyle label = this._buttonSkin.label;
		int buttonFontSize = LobbyMenu.getButtonFontSize();
		this._buttonSkin.button.fontSize = buttonFontSize;
		label.fontSize = buttonFontSize;
		this.guiCloseButton = (GUISkin)ResourceManager.Load("_GUISkins/CloseButton");
		this.achievementTween = new GameObject();
		this.keyStyle = new GUIStyle();
		this.keyStyle.wordWrap = true;
		this.keyStyle.font = (Font)ResourceManager.Load("Fonts/HoneyMeadBB_bold", typeof(Font));
		this.keyStyle.fontSize = 24;
		this.keyStyle.alignment = 0;
		this.keyStyle.normal.textColor = ColorUtil.FromHex24(16181972u);
		this.valueStyle = new GUIStyle(this.keyStyle);
		this.valueStyle.alignment = 2;
		this.centeredStyle = new GUIStyle(this.keyStyle);
		this.centeredStyle.alignment = 1;
		this.centeredStyle.fontSize = Screen.height / 30;
		this.middleCenterStyle = new GUIStyle(this.keyStyle);
		this.middleCenterStyle.alignment = 4;
		this.partIdStyle = new GUIStyle(this.valueStyle);
		this.partIdStyle.alignment = 4;
		this.partNameStyle = new GUIStyle(this.partIdStyle);
		this.partNameStyle.fontSize = Screen.height / 36;
		this.partNameStyle.normal.textColor = ColorUtil.FromHex24(8616813u);
		this.usernameStyle = new GUIStyle(this.keyStyle);
		this.usernameStyle.alignment = 4;
		this.usernameStyle.normal.textColor = ColorUtil.FromHex24(16181972u);
		this.titleStyle = new GUIStyle(this.usernameStyle);
		this.titleStyle.fontSize = Screen.height / 28;
		this.titleStyle.alignment = 1;
		this.titleStyle.normal.textColor = ColorUtil.FromHex24(15326921u);
		this.guiSkin = (GUISkin)ResourceManager.Load("_GUISkins/Lobby");
		this.guiSkinDisabled = (GUISkin)ResourceManager.Load("_GUISkins/DisabledButtons");
		this.guiSkinClear = ScriptableObject.CreateInstance<GUISkin>();
		this.guiArrowSkin = (GUISkin)ResourceManager.Load("_GUISkins/Lobby");
		this.guiArrowSkin.button.font = (Font)ResourceManager.Load("Fonts/HoneyMeadBB_bold", typeof(Font));
		this.guiArrowSkin.label.font = (Font)ResourceManager.Load("Fonts/HoneyMeadBB_bold", typeof(Font));
		this.guiSkinButtonLeft = ScriptableObject.CreateInstance<GUISkin>();
		this.guiSkinButtonLeft.button.normal.background = ResourceManager.LoadTexture("ChatUI/arrow_");
		this.guiSkinButtonLeft.button.hover.background = ResourceManager.LoadTexture("ChatUI/arrow_mo");
		this.guiSkinButtonLeft.button.active.background = ResourceManager.LoadTexture("ChatUI/arrow_md");
		this.arrowSize = new Vector2((float)this.guiSkinButtonLeft.button.normal.background.width, (float)this.guiSkinButtonLeft.button.normal.background.height);
		this.guiSkinButtonRight = ScriptableObject.CreateInstance<GUISkin>();
		this.guiSkinButtonRight.button.normal.background = ResourceManager.LoadTexture("ChatUI/arrow_right");
		this.guiSkinButtonRight.button.hover.background = ResourceManager.LoadTexture("ChatUI/arrow_right_mo");
		this.guiSkinButtonRight.button.active.background = ResourceManager.LoadTexture("ChatUI/arrow_right_md");
		this.bg = PrimitiveFactory.createPlane(false);
		this.bg.renderer.material = this.unlitMaterial;
		this.bg.renderer.material.mainTexture = ResourceManager.LoadTexture("DeckBuilder/bg");
		this.avatarGui = new Gui3D(Camera.main).setDefaultMaterial(this.unlitMaterial);
		this.avatarShadowGui = new Gui3D(Camera.main).setBaseDepth(this.avatarGui.getDepth(true) + 5f);
		new Gui3D(Camera.main).setBaseDepth(50f).DrawObject(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), this.bg);
		this.comm = App.Communicator;
		this.comm.addListener(this);
		if (App.SceneValues.profilePage.isMe())
		{
			this.comm.send(new ProfilePageInfoMessage());
		}
		else
		{
			this.comm.send(new ProfilePageInfoMessage(App.SceneValues.profilePage.profileId));
		}
		if (App.SceneValues.profilePage.showAchievementId > 0)
		{
			this._showAchievementId = new int?(App.SceneValues.profilePage.showAchievementId);
			this.showAchievementFrame(true);
		}
		App.SceneValues.profilePage.wasMe = App.SceneValues.profilePage.isMe();
		App.SceneValues.profilePage.clear();
		App.LobbyMenu.fadeInScene();
	}

	// Token: 0x060014F2 RID: 5362 RVA: 0x00081130 File Offset: 0x0007F330
	private void setupIdols(short[] unlockedIds, short[] chosenIds)
	{
		IdolType[] available = IdolTypeManager.getInstance().getAvailable(unlockedIds);
		this.idolConfig = new StepConfig<short>(Enumerable.ToArray<short>(Enumerable.Select<IdolType, short>(available, (IdolType t) => t.id)));
		this.idolConfig.setId(chosenIds[0]);
	}

	// Token: 0x060014F3 RID: 5363 RVA: 0x0008118C File Offset: 0x0007F38C
	private void setupSets()
	{
		this.setConfig = new SetConfig(AvatarPartTypeManager.getInstance().getPublicSets());
		for (int i = 0; i < this.setConfig.size(); i++)
		{
			string id = this.setConfig.getId(i);
			this.avatarConfigs[id] = AvatarConfig.createConfig(id, false);
		}
		this.setUpdated();
	}

	// Token: 0x060014F4 RID: 5364 RVA: 0x000811F0 File Offset: 0x0007F3F0
	private void setupAvatar(AvatarInfo avatarInfo)
	{
		this.avatarConfig = AvatarConfig.loadConfig(avatarInfo, !this.isSelf);
		if (this.avatarConfig.set != null)
		{
			this.avatarConfigs[this.avatarConfig.set] = this.avatarConfig;
		}
		this.avatar = Avatar.ProfilePageAvatar(null);
		this.setConfig.setId(this.avatarConfig.set);
		Rect rect = this.avatar.getRect((float)Screen.height * 0.83f);
		rect.x = (float)Screen.width * 0.5f - rect.width * 0.38f;
		rect.y = (float)Screen.height * 0.98f - rect.height;
		this.avatarRect = rect;
		this.avatarShadowRect = GeomUtil.cropShare(this.avatarRect, new Rect(0f, 0.63f, 1f, 0.3f));
	}

	// Token: 0x060014F5 RID: 5365 RVA: 0x000812E8 File Offset: 0x0007F4E8
	private void setupAchievements(short[] unlockedTypes)
	{
		Object.Destroy(this.achievementList);
		this.achievementList = base.gameObject.AddComponent<AchievementList>();
		this.achievementList.init(new Rect(this.avatarRect), 20f, unlockedTypes);
		if (this._showAchievementId != null)
		{
			this.achievementList.scrollTo(this._showAchievementId.Value);
			this._showAchievementId = default(int?);
		}
	}

	// Token: 0x060014F6 RID: 5366 RVA: 0x00081364 File Offset: 0x0007F564
	private void Update()
	{
		if (Input.GetKeyDown(97) && Input.GetKey(306) && Input.GetKey(304))
		{
			this.showAvatarIds = !this.showAvatarIds;
		}
		if (this.avatar != null)
		{
			this.avatar.update();
		}
	}

	// Token: 0x060014F7 RID: 5367 RVA: 0x000813C0 File Offset: 0x0007F5C0
	private void FixedUpdate()
	{
		this.fadeIn = Mathf.Max(0f, this.fadeIn - 0.03f);
		if (this.showEdit)
		{
			if (this.editFrameAlpha < 1f)
			{
				this.editFrameAlpha = Mathf.Min(this.editFrameAlpha + 0.25f, 1f);
			}
		}
		else if (this.editFrameAlpha > 0f)
		{
			this.editFrameAlpha = Mathf.Max(this.editFrameAlpha - 0.25f, 0f);
		}
	}

	// Token: 0x060014F8 RID: 5368 RVA: 0x00081454 File Offset: 0x0007F654
	private void OnGUI()
	{
		GUI.depth = 21;
		this.drawInfoPane();
		this.drawRightSide();
		this.drawAvatar();
		if (this.achievementList != null)
		{
			this.drawAchievementFrame();
		}
		GUI.color = new Color(1f, 1f, 1f, this.fadeIn);
		GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), ResourceManager.LoadTexture("Shared/blackFiller"));
		GUI.color = new Color(1f, 1f, 1f, 1f);
		if (this.avatarConfig != null && this.showAvatarIds)
		{
			this.printAvatarIds();
		}
	}

	// Token: 0x060014F9 RID: 5369 RVA: 0x00081514 File Offset: 0x0007F714
	private void drawAvatar()
	{
		if (this.avatar == null)
		{
			return;
		}
		if (Event.current.type.Equals(7))
		{
			this.avatarShadowGui.frameBegin();
			this.avatar.draw(this.avatarShadowGui, this.avatarShadowRect, Color.black, true);
			this.avatarShadowGui.frameEnd();
			this.avatarGui.frameBegin();
			this.avatar.draw(this.avatarGui, this.avatarRect, Color.white, true);
			this.avatarGui.frameEnd();
		}
	}

	// Token: 0x060014FA RID: 5370 RVA: 0x000815B4 File Offset: 0x0007F7B4
	private void drawAchievementFrame()
	{
		Rect rect = this.leftPaneRect;
		float num = (float)Screen.width - rect.xMax;
		rect.width = num * 0.84f;
		float num2 = (num - rect.width) / 2f;
		rect.x = (float)Screen.width - (rect.width + num2) * this.achievementTween.transform.position.x;
		Rect r = rect;
		Rect rect2 = GeomUtil.inflate(r, -20f, -20f);
		this.achievementList.setRect(rect2);
	}

	// Token: 0x060014FB RID: 5371 RVA: 0x00081648 File Offset: 0x0007F848
	private void drawInfoPane()
	{
		if (this.profileInfo == null)
		{
			return;
		}
		Rect rr;
		rr..ctor((float)ProfileMenu.frameBoxBorder, (float)ProfileMenu.frameBoxY, 508f, ProfileMenu.frameMockRect.height);
		Rect rect = ProfileMenu.mockJunk1280.rAspectH(rr);
		Rect rect2 = rect;
		this.leftPaneRect = rect;
		rect.height *= 0.7f;
		ScrollsFrame scrollsFrame = new ScrollsFrame(rect).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.CENTER);
		scrollsFrame.Draw();
		rect2.yMin += rect2.height * 0.72f;
		new ScrollsFrame(rect2).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.CENTER).Draw();
		Rect buttonRect = this.getButtonRect(0f);
		buttonRect.width *= 1.25f;
		buttonRect.x = rect2.x + (rect2.width - buttonRect.width) / 2f;
		buttonRect.y = rect2.yMax - buttonRect.height * 1.5f;
		int num = this.profileInfo.unlockedAchievementTypes.Length;
		int num2 = Math.Min(3, num);
		float num3 = rect2.width / 3f;
		float num4 = buttonRect.y - rect2.y;
		float x = rect2.x;
		float y = rect2.y;
		Vector2 screenMousePos = GUIUtil.getScreenMousePos();
		if (this.achievementList != null)
		{
			this.achievementList.hovered = null;
		}
		for (int i = 0; i < num2; i++)
		{
			int num5 = i;
			short id = this.profileInfo.unlockedAchievementTypes[num5];
			float num6 = x + (float)i * num3;
			Rect rect3 = GeomUtil.scaleCentered(new Rect(num6, y, num3, num4), 0.6f);
			AchievementType achievementType = AchievementTypeManager.getInstance().get(id);
			AchievementList.drawAchievementIcon(rect3, achievementType);
			if (this.achievementList != null && rect3.Contains(screenMousePos))
			{
				this.achievementList.hovered = achievementType;
			}
		}
		if (LobbyMenu.drawButton(buttonRect, ((!this._showAchievementFrame) ? "Show" : "Hide") + " achievements", this.guiSkin))
		{
			this.showAchievementFrame(!this._showAchievementFrame);
		}
		rect.height = ProfileMenu.mockJunk1280.Y(170f);
		this.usernameStyle.fontSize = Screen.height / 20;
		Rect rect4;
		rect4..ctor(rect.x, rect.y - ProfileMenu.mockJunk1280.Y(30f), rect.width, rect.height);
		GUI.Label(rect4, this.profileInfo.name, this.usernameStyle);
		int num7 = 420;
		int num8 = 44;
		float num9 = ProfileMenu.mockJunk1280.Y((float)num8);
		int num10 = 260;
		float num11 = rect.x + ProfileMenu.mockJunk1280.Y(30f);
		float num12 = rect.width - ProfileMenu.mockJunk1280.Y(60f);
		float num13 = num12 - ProfileMenu.mockJunk1280.Y(30f);
		float num14 = num13 * 183f / 512f;
		Rect rect5;
		rect5..ctor(num11 + (num12 - num13) / 2f, ProfileMenu.mockJunk1280.Y((float)(80 + num10)), num13, num14);
		Rect rect6;
		rect6..ctor(num11, ProfileMenu.mockJunk1280.Y((float)(215 + num10)), num12, (float)num8);
		Rect rect7;
		rect7..ctor(num11, ProfileMenu.mockJunk1280.Y((float)(150 + num10)), num12, (float)num8);
		GUIStyle guistyle = this.keyStyle;
		int fontSize = Screen.height / 30;
		this.valueStyle.fontSize = fontSize;
		guistyle.fontSize = fontSize;
		GUIStyleState normal = this.keyStyle.normal;
		Color textColor = Color.white;
		this.valueStyle.normal.textColor = textColor;
		normal.textColor = textColor;
		if (this.currentRank != null)
		{
			GUI.DrawTexture(rect5, ResourceManager.LoadTexture(this.currentRank.imagePath));
			string text = string.Concat(new object[]
			{
				"Rating: ",
				this.profileInfo.rating,
				" - Ranking: #",
				this.profileInfo.ranking
			});
			GUI.Label(rect6, this.currentRank.name, this.centeredStyle);
			GUIStyle guistyle2 = new GUIStyle(this.centeredStyle);
			guistyle2.fontSize = Screen.height / 50;
			GUI.Label(new Rect(rect6.x, rect6.y + ProfileMenu.mockJunk1280.Y(35f), rect6.width, rect6.height), (!this.isSelf) ? string.Empty : text, guistyle2);
		}
		else
		{
			string text2 = (!this.isSelf) ? "<color=#998877>This player is not currently ranked.</color>" : ("<color=#998877>Win</color> " + ProfileMenu.numberToWord(this.profileInfo.winsForRank) + " <color=#998877>more ranked matches\n to earn your caller rank.\n\n(Not enough current match history)</color>");
			GUI.Label(rect7, text2, this.middleCenterStyle);
		}
		Rect rect8;
		rect8..ctor(num11, ProfileMenu.mockJunk1280.Y((float)num7 + 140.8f), num12, (float)num8);
		Rect rect9;
		rect9..ctor(rect8);
		float num15 = 0.4f * num9;
		GUIStyle guistyle3 = this.keyStyle;
		fontSize = Screen.height / 42;
		this.valueStyle.fontSize = fontSize;
		guistyle3.fontSize = fontSize;
		GUIStyleState normal2 = this.keyStyle.normal;
		textColor = new Color(0.9f, 0.85f, 0.7f);
		this.valueStyle.normal.textColor = textColor;
		normal2.textColor = textColor;
		num9 = ProfileMenu.mockJunk1280.Y(32f);
		GUI.Box(new Rect(rect8.x, rect8.y - num15, rect9.xMax - rect8.x, num15 * 2f + 9.8f * num9), string.Empty, this._regularUI.box);
		rect8.x += ProfileMenu.mockJunk1280.X(20f);
		rect9.x -= ProfileMenu.mockJunk1280.X(20f);
		GUI.Label(rect8, "Games played", this.keyStyle);
		GUI.Label(rect9, this.profileInfo.gamesPlayed.ToString(), this.valueStyle);
		rect8.y += num9;
		rect9.y += num9;
		GUI.Label(rect8, "Games won", this.keyStyle);
		GUI.Label(rect9, this.profileInfo.gamesWon.ToString(), this.valueStyle);
		rect8.y += num9;
		rect9.y += num9;
		GUI.Label(rect8, "Ranked won", this.keyStyle);
		GUI.Label(rect9, this.profileInfo.rankedWon.ToString(), this.valueStyle);
		rect8.y += num9;
		rect9.y += num9;
		GUI.Label(rect8, "Judgement won", this.keyStyle);
		GUI.Label(rect9, this.profileInfo.limitedWon.ToString(), this.valueStyle);
		rect8.y += num9;
		rect9.y += num9;
		GUI.Label(rect8, "Last game", this.keyStyle);
		GUI.Label(rect9, this.profileInfo.lastGamePlayed, this.valueStyle);
		rect8.y += 1.8f * num9;
		rect9.y += 1.8f * num9;
		GUI.Label(rect8, "Scrolls", this.keyStyle);
		GUI.Label(rect9, this.profileInfo.getScrollsTotal().ToString(), this.valueStyle);
		rect8.y += num9;
		rect9.y += num9;
		GUI.Label(rect8, "Unique types", this.keyStyle);
		int uniqueTypes = this.profileInfo.uniqueTypes;
		int num16 = CardTypeManager.getInstance().size();
		int num17 = 100 * uniqueTypes / num16;
		string text3 = string.Concat(new object[]
		{
			uniqueTypes,
			" of ",
			num16,
			" (",
			num17,
			"%)"
		});
		GUI.Label(rect9, text3, this.valueStyle);
		rect8.y += num9;
		rect9.y += num9;
		GUI.Label(rect8, "Rarities C/U/R", this.keyStyle);
		string text4 = string.Concat(new object[]
		{
			string.Empty,
			this.profileInfo.scrollsCommon,
			"/",
			this.profileInfo.scrollsUncommon,
			"/",
			this.profileInfo.scrollsRare
		});
		GUI.Label(rect9, text4, this.valueStyle);
		rect8.y += num9;
		rect9.y += num9;
		GUI.Label(rect8, "Gold", this.keyStyle);
		GUI.Label(rect9, this.profileInfo.gold.ToString(), this.valueStyle);
	}

	// Token: 0x060014FC RID: 5372 RVA: 0x00081FF0 File Offset: 0x000801F0
	private static string numberToWord(int i)
	{
		switch (i)
		{
		case 0:
			return "zero";
		case 1:
			return "one";
		case 2:
			return "two";
		case 3:
			return "three";
		case 4:
			return "four";
		case 5:
			return "five";
		case 6:
			return "six";
		case 7:
			return "seven";
		case 8:
			return "eight";
		case 9:
			return "nine";
		case 10:
			return "ten";
		default:
			return i.ToString();
		}
	}

	// Token: 0x060014FD RID: 5373 RVA: 0x00082080 File Offset: 0x00080280
	private void showAchievementFrame(bool show)
	{
		this._showAchievementFrame = show;
		iTween.Stop(this.achievementTween);
		float x = this.achievementTween.transform.position.x;
		if (!this._showAchievementFrame)
		{
			float num = x;
			Hashtable args = iTween.Hash(new object[]
			{
				"x",
				-0.1f,
				"time",
				num,
				"easetype",
				iTween.EaseType.easeInOutQuart
			});
			iTween.MoveTo(this.achievementTween, args);
		}
		else
		{
			float num2 = 1f - x;
			Hashtable args2 = iTween.Hash(new object[]
			{
				"x",
				1,
				"time",
				num2,
				"easetype",
				iTween.EaseType.easeInOutQuart
			});
			iTween.MoveTo(this.achievementTween, args2);
		}
	}

	// Token: 0x060014FE RID: 5374 RVA: 0x00082170 File Offset: 0x00080370
	private void drawIdolButton(float i, int partId)
	{
		IdolType idolType = IdolTypeManager.getInstance().get(this.idolConfig.getId());
		GUIContent content = new GUIContent(ResourceManager.LoadTexture(idolType.getFullHealthFilename()));
		this.drawButton<short>(content, 760 + (int)(i * 92f), this.idolConfig);
	}

	// Token: 0x060014FF RID: 5375 RVA: 0x000821C0 File Offset: 0x000803C0
	private void drawAvatarButton(string text, int i, int partId)
	{
		AvatarConfigPart configPart = this.avatarConfig.getConfigPart(partId);
		this.drawButton<int>(new GUIContent(text), 310 + i * 82, configPart);
	}

	// Token: 0x06001500 RID: 5376 RVA: 0x000821F4 File Offset: 0x000803F4
	private void drawSetButton()
	{
		int index = this.setConfig.getIndex();
		this.drawButton<string>(new GUIContent("Set"), 638, this.setConfig);
		if (index != this.setConfig.getIndex())
		{
			this.setUpdated();
		}
	}

	// Token: 0x06001501 RID: 5377 RVA: 0x00082240 File Offset: 0x00080440
	private bool drawButton<T>(GUIContent content, int y, StepConfig<T> p)
	{
		Rect rect;
		rect..ctor(1540f, (float)y, 36f, 32f);
		Rect rect2;
		rect2..ctor(1540f, (float)y, 206f, 32f);
		Rect rect3 = ProfileMenu.mockJunk.r(rect);
		bool flag = false;
		if (LobbyMenu.drawButton(ProfileMenu.mockJunk.r(GeomUtil.getTranslated(rect, 0f, 0f)), "<", this.guiSkin))
		{
			p.prev();
			flag = true;
		}
		if (LobbyMenu.drawButton(ProfileMenu.mockJunk.r(GeomUtil.getTranslated(rect, 170f, 0f)), ">", this.guiSkin))
		{
			p.next();
			flag = true;
		}
		Rect rect4 = rect2;
		if (content.image != null)
		{
			rect4 = GeomUtil.scaleCentered(rect4, 3f);
		}
		GUI.Label(ProfileMenu.mockJunk.r(rect4), content, this.partNameStyle);
		if (flag)
		{
			this.isSaved = false;
		}
		return flag;
	}

	// Token: 0x06001502 RID: 5378 RVA: 0x00082344 File Offset: 0x00080544
	private void printAvatarIds()
	{
		string text = string.Concat(new object[]
		{
			"avatar(",
			this.avatarConfig.head.getId(),
			",",
			this.avatarConfig.body.getId(),
			",",
			this.avatarConfig.leg.getId(),
			",",
			this.avatarConfig.arm.getId(),
			",",
			AvatarPartTypeManager.getInstance().getFrontArmIdForBackArm(this.avatarConfig.arm.getId()),
			")"
		});
		GUI.TextField(GeomUtil.getTranslated(this.frameRect, 0f, (float)(-(float)Screen.height) * 0.04f), text, this.valueStyle);
	}

	// Token: 0x06001503 RID: 5379 RVA: 0x0008243C File Offset: 0x0008063C
	private void drawEditBox()
	{
		if (this.editFrameAlpha <= 0f)
		{
			return;
		}
		GUI.color = new Color(1f, 1f, 1f, this.editFrameAlpha);
		new ScrollsFrame(this.frameRect).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.NONE).Draw();
		GUI.Label(GeomUtil.getTranslated(this.frameRect, 0f, (float)Screen.height * 0.03f), "Avatar", this.titleStyle);
		string[] array = new string[]
		{
			"Head",
			"Arms",
			"Torso",
			"Legs"
		};
		AvatarPartName[] array2 = new AvatarPartName[]
		{
			AvatarPartName.HEAD,
			AvatarPartName.ARM_BACK,
			AvatarPartName.BODY,
			AvatarPartName.LEG
		};
		for (int i = 0; i < 4; i++)
		{
			this.drawAvatarButton(array[i], i, (int)array2[i]);
		}
		this.drawSetButton();
		GUI.Label(GeomUtil.getTranslated(this.frameRect, 0f, (float)Screen.height * 0.52f), "Idols", this.titleStyle);
		this.drawIdolButton(1f, 0);
		GUI.enabled = !this.isSaved;
		if (LobbyMenu.drawButton(this.getButtonRect(1.1f), new GUIContent("Save").lockDemo(), this.guiSkin))
		{
			this.comm.send(new IdolSetupSaveMessage(this.idolConfig.getId()));
			this.comm.send(new AvatarSaveMessage(this.avatarConfig));
		}
		GUI.enabled = true;
		if (LobbyMenu.drawButton(this.getButtonRect(-2.1f), "Randomize", this.guiSkin))
		{
			for (int j = 0; j < 4; j++)
			{
				this.avatarConfig.getConfigPart(j).random();
			}
			this.isSaved = false;
		}
		this.buttonColor = new Color(1f, 1f, 1f, 0.3f);
		GUI.color = Color.white;
	}

	// Token: 0x06001504 RID: 5380 RVA: 0x00082638 File Offset: 0x00080838
	private void updateAvatar()
	{
		for (int i = 0; i < 5; i++)
		{
			if (i != 4)
			{
				this.avatar.set(i, this.avatarConfig.getConfigPart(i).getId());
			}
		}
	}

	// Token: 0x06001505 RID: 5381 RVA: 0x0000F653 File Offset: 0x0000D853
	private void drawRightSide()
	{
		if (this.avatar == null)
		{
			return;
		}
		this.updateAvatar();
		if (!this.isSelf)
		{
			return;
		}
		this.drawEditButton();
		this.drawEditBox();
		GUI.color = Color.white;
	}

	// Token: 0x06001506 RID: 5382 RVA: 0x0008267C File Offset: 0x0008087C
	private void drawEditButton()
	{
		if (this.editFrameAlpha >= 1f)
		{
			return;
		}
		bool enabled = GUI.enabled;
		GUI.enabled = (this.setConfig != null);
		GUI.color = new Color(1f, 1f, 1f, 1f - this.editFrameAlpha);
		if (LobbyMenu.drawButton(this.getButtonRect(-4f), "Edit avatar", this.guiSkin))
		{
			this.showEdit = true;
		}
		GUI.enabled = enabled;
	}

	// Token: 0x06001507 RID: 5383 RVA: 0x0000F689 File Offset: 0x0000D889
	private void setUpdated()
	{
		this.avatarConfig = this.avatarConfigs[this.setConfig.getId()];
	}

	// Token: 0x06001508 RID: 5384 RVA: 0x00082704 File Offset: 0x00080904
	public override void handleMessage(Message msg)
	{
		if (msg is ProfilePageInfoMessage)
		{
			ProfilePageInfoMessage profilePageInfoMessage = (ProfilePageInfoMessage)msg;
			this.isSelf = (App.MyProfile.ProfileInfo.name == profilePageInfoMessage.name);
			this.profileInfo = profilePageInfoMessage;
			this.currentRank = Ranks.Get(this.profileInfo.rank);
			this.setupIdols(profilePageInfoMessage.unlockedIdolTypes, profilePageInfoMessage.idols.idols());
			this.setupSets();
			this.setupAvatar(profilePageInfoMessage.getAvatar());
			this.setupAchievements(profilePageInfoMessage.unlockedAchievementTypes);
		}
		if (msg is OkMessage && ((OkMessage)msg).isType(typeof(AvatarSaveMessage)))
		{
			this.isSaved = true;
		}
		base.handleMessage(msg);
	}

	// Token: 0x06001509 RID: 5385 RVA: 0x000827C8 File Offset: 0x000809C8
	private Rect getButtonRect(float i)
	{
		Rect result;
		result..ctor(0f, 0f, (float)Screen.height * 0.18f, (float)Screen.height * 0.055f);
		result.x = this.frameRect.x + (this.frameRect.width - result.width) / 2f;
		result.y = this.frameRect.y + this.frameRect.height - 2.8f * result.height;
		result.y += i * result.height + (i - 1f) * (float)Screen.height * 0.01f;
		return result;
	}

	// Token: 0x040011FF RID: 4607
	private const float achievementMargin = 20f;

	// Token: 0x04001200 RID: 4608
	private Communicator comm;

	// Token: 0x04001201 RID: 4609
	private GUISkin _emptySkin;

	// Token: 0x04001202 RID: 4610
	private GUISkin _buttonSkin;

	// Token: 0x04001203 RID: 4611
	private GUISkin _regularUI;

	// Token: 0x04001204 RID: 4612
	private string _sceneToLoad;

	// Token: 0x04001205 RID: 4613
	private ProfilePageInfoMessage profileInfo;

	// Token: 0x04001206 RID: 4614
	private Rank currentRank;

	// Token: 0x04001207 RID: 4615
	private static MockupCalc mockJunk = new MockupCalc(1920, 1080);

	// Token: 0x04001208 RID: 4616
	private static MockupCalc mockJunk1280 = new MockupCalc(1920, 1280);

	// Token: 0x04001209 RID: 4617
	private static MockupCalc mockUnit = new MockupCalc(1, 1);

	// Token: 0x0400120A RID: 4618
	private GUIStyle keyStyle;

	// Token: 0x0400120B RID: 4619
	private GUIStyle valueStyle;

	// Token: 0x0400120C RID: 4620
	private GUIStyle centeredStyle;

	// Token: 0x0400120D RID: 4621
	private GUIStyle middleCenterStyle;

	// Token: 0x0400120E RID: 4622
	private GUIStyle partIdStyle;

	// Token: 0x0400120F RID: 4623
	private GUIStyle partNameStyle;

	// Token: 0x04001210 RID: 4624
	private GUIStyle usernameStyle;

	// Token: 0x04001211 RID: 4625
	private GUIStyle titleStyle;

	// Token: 0x04001212 RID: 4626
	private GUISkin guiCloseButton;

	// Token: 0x04001213 RID: 4627
	private GUISkin guiSkin;

	// Token: 0x04001214 RID: 4628
	private GUISkin guiSkinDisabled;

	// Token: 0x04001215 RID: 4629
	private GUISkin guiSkinClear;

	// Token: 0x04001216 RID: 4630
	private GUISkin guiSkinButtonLeft;

	// Token: 0x04001217 RID: 4631
	private GUISkin guiSkinButtonRight;

	// Token: 0x04001218 RID: 4632
	private GUISkin guiArrowSkin;

	// Token: 0x04001219 RID: 4633
	private GameObject bg;

	// Token: 0x0400121A RID: 4634
	private Avatar avatar;

	// Token: 0x0400121B RID: 4635
	private bool isSaved = true;

	// Token: 0x0400121C RID: 4636
	private bool isSelf;

	// Token: 0x0400121D RID: 4637
	private float editFrameAlpha;

	// Token: 0x0400121E RID: 4638
	private bool _showAchievementFrame;

	// Token: 0x0400121F RID: 4639
	private int? _showAchievementId;

	// Token: 0x04001220 RID: 4640
	private GameObject achievementTween;

	// Token: 0x04001221 RID: 4641
	private AvatarConfig avatarConfig;

	// Token: 0x04001222 RID: 4642
	private SetConfig setConfig;

	// Token: 0x04001223 RID: 4643
	private StepConfig<short> idolConfig = new StepConfig<short>(new short[0]);

	// Token: 0x04001224 RID: 4644
	private Dictionary<string, AvatarConfig> avatarConfigs = new Dictionary<string, AvatarConfig>();

	// Token: 0x04001225 RID: 4645
	private Vector2 arrowSize;

	// Token: 0x04001226 RID: 4646
	private Gui3D avatarGui;

	// Token: 0x04001227 RID: 4647
	private Gui3D avatarShadowGui;

	// Token: 0x04001228 RID: 4648
	private AchievementList achievementList;

	// Token: 0x04001229 RID: 4649
	[SerializeField]
	private Color buttonColor = Color.white;

	// Token: 0x0400122A RID: 4650
	[SerializeField]
	private Material unlitMaterial;

	// Token: 0x0400122B RID: 4651
	private Rect avatarRect;

	// Token: 0x0400122C RID: 4652
	private Rect avatarShadowRect;

	// Token: 0x0400122D RID: 4653
	private Rect leftPaneRect;

	// Token: 0x0400122E RID: 4654
	private float fadeIn = 1f;

	// Token: 0x0400122F RID: 4655
	private bool showAvatarIds;

	// Token: 0x04001230 RID: 4656
	private static int frameBoxBorder = 100;

	// Token: 0x04001231 RID: 4657
	private static int frameBoxY = 260;

	// Token: 0x04001232 RID: 4658
	private static int border = 60;

	// Token: 0x04001233 RID: 4659
	private static Rect frameMockRect = new Rect((float)(1540 - ProfileMenu.border), (float)ProfileMenu.frameBoxY, (float)(206 + ProfileMenu.border * 2), (float)(840 + ProfileMenu.border * 2));

	// Token: 0x04001234 RID: 4660
	private Rect frameRect = ProfileMenu.mockJunk1280.r(ProfileMenu.frameMockRect);

	// Token: 0x04001235 RID: 4661
	private bool showEdit;
}
