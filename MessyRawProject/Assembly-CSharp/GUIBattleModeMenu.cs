using System;
using UnityEngine;

// Token: 0x02000088 RID: 136
public class GUIBattleModeMenu : MonoBehaviour
{
	// Token: 0x06000511 RID: 1297 RVA: 0x000364F4 File Offset: 0x000346F4
	public void Start()
	{
		this.clearStyle = new GUIStyle();
		this.clearStyle.font = (Font)ResourceManager.Load("Fonts/HoneyMeadBB_bold", typeof(Font));
		this.clearStyle.fontStyle = 1;
		this.clearStyle.alignment = 4;
		this.clearStyle.normal.textColor = ColorUtil.FromHex24(11842445u);
		this.clearStyleHover = new GUIStyle(this.clearStyle);
		this.clearStyleHover.normal.textColor = ColorUtil.FromHex24(16709600u);
		this.buttonSkin = (GUISkin)ResourceManager.Load("_GUISkins/Lobby");
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x000053E2 File Offset: 0x000035E2
	public void init(GameMode gameMode, MiniCommunicator specComm)
	{
		this.audioScript = App.AudioScript;
		this.specComm = specComm;
		this.gameMode = gameMode;
		this.soundVolume = this.audioScript.GetVolume(AudioScript.ESoundType.SFX);
		this.musicVolume = this.audioScript.GetVolume(AudioScript.ESoundType.MUSIC);
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x00005421 File Offset: 0x00003621
	public void toggleMenu()
	{
		this.showMenu = !this.showMenu;
		if (this.showMenu)
		{
			this.changeMenuState(GUIBattleModeMenu.EMenuState.MAIN);
		}
	}

	// Token: 0x06000514 RID: 1300 RVA: 0x000365A4 File Offset: 0x000347A4
	public void Update()
	{
		if (Input.GetKeyUp(274))
		{
			this.down();
		}
		if (Input.GetKeyUp(273))
		{
			this.up();
		}
		if ((Input.GetKeyDown(13) || Input.GetKeyDown(271)) && this.showMenu)
		{
			this.menuPendingConfirm = true;
		}
	}

	// Token: 0x06000515 RID: 1301 RVA: 0x00036608 File Offset: 0x00034808
	public void OnGUI()
	{
		if (!this.canShowMenu)
		{
			this.showMenu = false;
		}
		if (this.showMenu)
		{
			this.menuBlackAlpha += Time.deltaTime / 0.25f;
		}
		else
		{
			this.menuBlackAlpha -= Time.deltaTime / 0.1f;
		}
		this.menuBlackAlpha = Mth.clamp(this.menuBlackAlpha, 0f, 0.5f);
		if (this.menuBlackAlpha > 0f)
		{
			Color color = GUI.color;
			this.drawMenuBase(this.menuBlackAlpha);
			Func<Rect, Rect> func = (Rect x) => GeomUtil.getCentered(x, true, false);
			if (this.menuState == GUIBattleModeMenu.EMenuState.MAIN)
			{
				GUI.DrawTexture(func.Invoke(this.mockJunk.rAspectH(819f, 240f, 282f, 41f)), ResourceManager.LoadTexture("EscMenu/escmenu__menu"));
				int fontSize = 60 * Screen.height / 1080;
				this.clearStyle.fontSize = fontSize;
				this.clearStyleHover.fontSize = fontSize;
				Rect rect = func.Invoke(this.mockJunk.r(745f, 354f, 430f, 49f));
				float num = 0.07f * (float)Screen.height;
				Rect r;
				r..ctor(rect);
				Vector2 vector;
				vector..ctor(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
				for (int i = 0; i < this.getNumMenuButtons(); i++)
				{
					if (GeomUtil.scaleCentered(r, 1f).Contains(vector))
					{
						this.menuCurrentSelection = i;
						break;
					}
					r.y += num;
				}
				if (this.menuCurrentSelection >= 0)
				{
					Rect rect2;
					rect2..ctor(rect);
					rect2.y += (float)this.menuCurrentSelection * num;
					GUI.color = new Color(1f, 1f, 1f, 0.5f);
					GUI.DrawTexture(rect2, ResourceManager.LoadTexture("EscMenu/escmenu__ljusflash"));
					GUI.color = Color.white;
				}
				int num2 = -1;
				if (this.menuPendingConfirm)
				{
					num2 = this.menuCurrentSelection;
					this.menuPendingConfirm = false;
				}
				if (GUI.Button(rect, "Return to Game", (this.menuCurrentSelection != 0) ? this.clearStyle : this.clearStyleHover))
				{
					num2 = 0;
				}
				rect.y += num;
				if (GUI.Button(rect, "Settings", (this.menuCurrentSelection != 1) ? this.clearStyle : this.clearStyleHover))
				{
					num2 = 1;
				}
				if (this.gameMode == GameMode.Play)
				{
					rect.y += num;
					if (GUI.Button(rect, "Help", (this.menuCurrentSelection != 2) ? this.clearStyle : this.clearStyleHover))
					{
						num2 = 2;
					}
					rect.y += num;
					if (GUI.Button(rect, "Surrender", (this.menuCurrentSelection != 3) ? this.clearStyle : this.clearStyleHover))
					{
						num2 = 3;
					}
				}
				else
				{
					rect.y += num;
					if (GUI.Button(rect, "Leave", (this.menuCurrentSelection != 2) ? this.clearStyle : this.clearStyleHover))
					{
						num2 = 2;
					}
				}
				if (num2 == this.menuCurrentSelection)
				{
					switch (num2)
					{
					case 0:
						this.showMenu = false;
						break;
					case 1:
						this.changeMenuState(GUIBattleModeMenu.EMenuState.SETTINGS);
						break;
					case 2:
						if (this.gameMode == GameMode.Play)
						{
							this.changeMenuState(GUIBattleModeMenu.EMenuState.HELP);
						}
						else
						{
							this.specComm.send(new SpectateLeaveGameMessage());
							App.Communicator.joinLobby(true);
						}
						break;
					case 3:
						this.changeMenuState(GUIBattleModeMenu.EMenuState.QUIT);
						break;
					}
				}
				GUI.color = color;
			}
			else if (this.menuState == GUIBattleModeMenu.EMenuState.QUIT)
			{
				GUI.DrawTexture(func.Invoke(this.mockJunk.rAspectH(793f, 240f, 333f, 53f)), ResourceManager.LoadTexture("EscMenu/escmenu_quit"));
				int num3 = -1;
				if (this.menuPendingConfirm)
				{
					num3 = this.menuCurrentSelection;
					this.menuPendingConfirm = false;
				}
				Rect rect3 = func.Invoke(this.mockJunk.r(745f, 354f, 430f, 49f));
				float num4 = 0.07f * (float)Screen.height;
				Rect r2;
				r2..ctor(rect3);
				Vector2 vector2;
				vector2..ctor(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
				for (int j = 0; j < this.getNumMenuButtons(); j++)
				{
					if (GeomUtil.scaleCentered(r2, 1f).Contains(vector2))
					{
						this.menuCurrentSelection = j;
						break;
					}
					r2.y += num4;
				}
				if (this.menuCurrentSelection >= 0)
				{
					Rect rect4;
					rect4..ctor(rect3);
					rect4.y += (float)this.menuCurrentSelection * num4;
					GUI.color = new Color(1f, 1f, 1f, 0.5f);
					GUI.DrawTexture(rect4, ResourceManager.LoadTexture("EscMenu/escmenu__ljusflash"));
					GUI.color = Color.white;
				}
				if (GUI.Button(rect3, "Yes", (this.menuCurrentSelection != 0) ? this.clearStyle : this.clearStyleHover))
				{
					num3 = 0;
				}
				rect3.y += num4;
				if (GUI.Button(rect3, "No", (this.menuCurrentSelection != 1) ? this.clearStyle : this.clearStyleHover))
				{
					num3 = 1;
				}
				if (num3 == this.menuCurrentSelection)
				{
					int num5 = num3;
					if (num5 != 0)
					{
						if (num5 == 1)
						{
							this.changeMenuState(GUIBattleModeMenu.EMenuState.MAIN);
						}
					}
					else
					{
						this.showMenu = false;
						App.Communicator.sendRequest(new SurrenderMessage());
					}
				}
			}
			else if (this.menuState == GUIBattleModeMenu.EMenuState.HELP)
			{
				float num6 = 68f;
				GUI.DrawTexture(func.Invoke(new Rect(-num6, 0f, (float)Screen.width + 2f * num6, (float)Screen.height)), ResourceManager.LoadTexture("Tutorial/help_screen"));
				int buttonFontSize = LobbyMenu.getButtonFontSize();
				this.buttonSkin.label.fontSize = buttonFontSize;
				this.buttonSkin.button.fontSize = buttonFontSize;
				GUIPositioner buttonPositioner = LobbyMenu.getButtonPositioner(1f, 0f);
				if (LobbyMenu.drawButton(buttonPositioner.getButtonRect(0f, (float)Screen.width * 0.8f, (float)Screen.height * 0.9f), "Close", this.buttonSkin))
				{
					this.changeMenuState(GUIBattleModeMenu.EMenuState.MAIN);
				}
			}
			else if (this.menuState == GUIBattleModeMenu.EMenuState.SETTINGS)
			{
				int num7 = -1;
				Rect rect5 = func.Invoke(this.mockJunk.rAspectH(745f, 554f, 430f, 49f));
				float num8 = 0.07f * (float)Screen.height;
				Rect r3;
				r3..ctor(rect5);
				Vector2 vector3;
				vector3..ctor(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
				for (int k = 0; k < this.getNumMenuButtons(); k++)
				{
					if (GeomUtil.scaleCentered(r3, 1f).Contains(vector3))
					{
						this.menuCurrentSelection = k;
						break;
					}
					r3.y += num8;
				}
				if (this.menuCurrentSelection >= 0)
				{
					Rect rect6;
					rect6..ctor(rect5);
					rect6.y += (float)this.menuCurrentSelection * num8;
					GUI.color = new Color(1f, 1f, 1f, 0.5f);
					GUI.DrawTexture(rect6, ResourceManager.LoadTexture("EscMenu/escmenu__ljusflash"));
					GUI.color = Color.white;
				}
				int fontSize2 = GUI.skin.label.fontSize;
				TextAnchor alignment = GUI.skin.label.alignment;
				GUI.skin.label.fontSize = (int)((float)Screen.height * 0.03f);
				GUI.skin.label.alignment = 6;
				Rect rect7 = func.Invoke(this.mockJunk.r(745f, 300f, 430f, 50f));
				GUI.Label(rect7, "SFX volume: " + (this.soundVolume * 100f).ToString("N0") + "%");
				float num9 = this.mockJunk.Y(45f);
				rect7.y += num9;
				this.soundVolume = GUI.HorizontalSlider(rect7, this.soundVolume, 0f, 1f);
				if (!Mathf.Approximately(App.AudioScript.GetVolume(AudioScript.ESoundType.SFX), this.soundVolume))
				{
					this.audioScript.SetVolume(AudioScript.ESoundType.SFX, this.soundVolume);
				}
				Rect rect8 = func.Invoke(this.mockJunk.r(745f, 430f, 430f, 50f));
				GUI.Label(rect8, "Music volume: " + (this.musicVolume * 100f).ToString("N0") + "%");
				rect8.y += num9;
				this.musicVolume = GUI.HorizontalSlider(rect8, this.musicVolume, 0f, 1f);
				if (!Mathf.Approximately(App.AudioScript.GetVolume(AudioScript.ESoundType.MUSIC), this.musicVolume))
				{
					this.audioScript.SetVolume(AudioScript.ESoundType.MUSIC, this.musicVolume);
				}
				GUI.skin.label.fontSize = fontSize2;
				GUI.skin.label.alignment = alignment;
				if (GUI.Button(rect5, "Back", (this.menuCurrentSelection != 0) ? this.clearStyle : this.clearStyleHover))
				{
					num7 = 0;
				}
				if (num7 == this.menuCurrentSelection)
				{
					int num5 = num7;
					if (num5 == 0)
					{
						this.changeMenuState(GUIBattleModeMenu.EMenuState.MAIN);
					}
				}
			}
		}
	}

	// Token: 0x06000516 RID: 1302 RVA: 0x000370C4 File Offset: 0x000352C4
	private void drawMenuBase(float alpha)
	{
		GUI.color = new Color(1f, 1f, 1f, this.menuBlackAlpha);
		GUI.DrawTexture(new Rect(-1f, -1f, (float)(Screen.width + 2), (float)(Screen.height + 2)), ResourceManager.LoadTexture("Login/black"));
		GUI.color = Color.white;
		GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), ResourceManager.LoadTexture("EscMenu/escmenu__alfaboll"));
		float num = Time.time * -7.5f;
		Vector2 vector;
		vector..ctor(0.5f * (float)Screen.width, 0.43f * (float)Screen.height);
		GUIUtility.RotateAroundPivot(num, vector);
		float num2 = (float)Screen.width * 0.2f;
		Vector2 vector2;
		vector2..ctor(num2, num2);
		GUI.color = new Color(1f, 1f, 1f, 0.25f);
		GUI.DrawTexture(new Rect(vector.x - vector2.x, vector.y - vector2.y, vector2.x + vector2.x, vector2.y + vector2.y), ResourceManager.LoadTexture("Login/_0002_ring"));
		GUIUtility.RotateAroundPivot(-num, vector);
		GUI.color = Color.white;
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x0003721C File Offset: 0x0003541C
	private void down()
	{
		if (++this.menuCurrentSelection >= this.getNumMenuButtons())
		{
			this.menuCurrentSelection = 0;
		}
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x0003724C File Offset: 0x0003544C
	private void up()
	{
		if (--this.menuCurrentSelection < 0)
		{
			this.menuCurrentSelection = this.getNumMenuButtons() - 1;
		}
	}

	// Token: 0x06000519 RID: 1305 RVA: 0x00005444 File Offset: 0x00003644
	private void changeMenuState(GUIBattleModeMenu.EMenuState toState)
	{
		this.menuState = toState;
		this.menuCurrentSelection = 0;
	}

	// Token: 0x0600051A RID: 1306 RVA: 0x00037280 File Offset: 0x00035480
	private int getNumMenuButtons()
	{
		switch (this.menuState)
		{
		case GUIBattleModeMenu.EMenuState.MAIN:
			return (this.gameMode != GameMode.Play) ? 3 : 4;
		case GUIBattleModeMenu.EMenuState.QUIT:
			return 2;
		case GUIBattleModeMenu.EMenuState.HELP:
			return 2;
		case GUIBattleModeMenu.EMenuState.SETTINGS:
			return 1;
		default:
			return 0;
		}
	}

	// Token: 0x04000381 RID: 897
	public bool canShowMenu = true;

	// Token: 0x04000382 RID: 898
	public bool showMenu;

	// Token: 0x04000383 RID: 899
	private GUIBattleModeMenu.EMenuState menuState;

	// Token: 0x04000384 RID: 900
	private int menuCurrentSelection;

	// Token: 0x04000385 RID: 901
	private bool menuPendingConfirm;

	// Token: 0x04000386 RID: 902
	private AudioScript audioScript;

	// Token: 0x04000387 RID: 903
	private MiniCommunicator specComm;

	// Token: 0x04000388 RID: 904
	private GameMode gameMode;

	// Token: 0x04000389 RID: 905
	private float soundVolume;

	// Token: 0x0400038A RID: 906
	private float musicVolume;

	// Token: 0x0400038B RID: 907
	private float menuBlackAlpha;

	// Token: 0x0400038C RID: 908
	private MockupCalc mockJunk = new MockupCalc(1920, 1080);

	// Token: 0x0400038D RID: 909
	private GUIStyle clearStyle;

	// Token: 0x0400038E RID: 910
	private GUIStyle clearStyleHover;

	// Token: 0x0400038F RID: 911
	private GUISkin buttonSkin;

	// Token: 0x02000089 RID: 137
	private enum EMenuState
	{
		// Token: 0x04000392 RID: 914
		NONE,
		// Token: 0x04000393 RID: 915
		MAIN,
		// Token: 0x04000394 RID: 916
		QUIT,
		// Token: 0x04000395 RID: 917
		HELP,
		// Token: 0x04000396 RID: 918
		SETTINGS
	}
}
