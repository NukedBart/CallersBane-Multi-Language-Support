using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000220 RID: 544
public class SettingsMenu : AbstractCommListener
{
	// Token: 0x06001156 RID: 4438 RVA: 0x00076244 File Offset: 0x00074444
	private void Start()
	{
		this.settingsSkin = (GUISkin)ResourceManager.Load("_GUISkins/Settings");
		this.regularUI = (GUISkin)ResourceManager.Load("_GUISkins/RegularUI");
		App.Communicator.addListener(this);
		Application.targetFrameRate = 60;
		this.Init();
		App.LobbyMenu.fadeInScene();
		this.stringToSpectateEnum.Add("Allow", SpectatePermission.ALLOW_CHAT);
		this.stringToSpectateEnum.Add("Allow, but hide chat", SpectatePermission.ALLOW);
		this.stringToSpectateEnum.Add("Block", SpectatePermission.DISALLOW);
		this.spectateDrop = new GameObject("Dropdown").AddComponent<Dropdown>();
		this.spectateDrop.Init(Enumerable.ToArray<string>(this.stringToSpectateEnum.Keys), 4f, true, false, 14);
		this.spectateDrop.SetEnabled(true);
		this.spectateDrop.SetSkin(this.regularUI);
		this.spectateDrop.DropdownChangedEvent += this.DropdownChangedEvent;
		this.battleBgDrop = new GameObject("BattleBgDropdown").AddComponent<Dropdown>();
		List<string> list = new List<string>();
		list.Add("-- Shuffle --");
		list.AddRange(Enumerable.ToArray<string>(BackgroundData.getAllNames()));
		this.battleBgDrop.Init(list.ToArray(), 4f, true, false, 14);
		this.battleBgDrop.SetEnabled(true);
		this.battleBgDrop.SetSkin(this.regularUI);
		this.battleBgDrop.DropdownChangedEvent += this.BattleBgDropdownChangedEvent;
		this.LoadBattleBgSetting();
		App.Communicator.send(new SpectateGetPermissionMessage());
		App.ChatUI.Show(false);
	}

	// Token: 0x06001157 RID: 4439 RVA: 0x000763E4 File Offset: 0x000745E4
	private void LoadBattleBgSetting()
	{
		BackgroundData background = App.Config.settings.battle.getBackground();
		if (background == null)
		{
			this.battleBgDrop.SetSelectedItem(0);
		}
		else
		{
			this.battleBgDrop.SetSelectedItem(background.name);
		}
	}

	// Token: 0x06001158 RID: 4440 RVA: 0x0000D3E3 File Offset: 0x0000B5E3
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.spectateDrop.DropdownChangedEvent -= this.DropdownChangedEvent;
		this.battleBgDrop.DropdownChangedEvent -= this.BattleBgDropdownChangedEvent;
	}

	// Token: 0x06001159 RID: 4441 RVA: 0x00076430 File Offset: 0x00074630
	public void DropdownChangedEvent(int selectedIndex, string selection)
	{
		SpectatePermission permission = this.stringToSpectateEnum[selection];
		App.Communicator.send(new SpectateSetPermissionMessage(permission));
	}

	// Token: 0x0600115A RID: 4442 RVA: 0x0000D419 File Offset: 0x0000B619
	public void BattleBgDropdownChangedEvent(int selectedIndex, string selection)
	{
		App.Config.settings.battle.background.value = selection;
	}

	// Token: 0x0600115B RID: 4443 RVA: 0x0007645C File Offset: 0x0007465C
	private void Init()
	{
		this.GetResolution();
		this.soundVolume = App.AudioScript.GetVolume(AudioScript.ESoundType.SFX);
		this.musicVolume = App.AudioScript.GetVolume(AudioScript.ESoundType.MUSIC);
		this.chatToggle = App.AudioScript.GetSoundToggle(AudioScript.ESoundToggle.CHAT);
		this.chatHighlightToggle = App.AudioScript.GetSoundToggle(AudioScript.ESoundToggle.CHAT_HIGHLIGHT);
		this.buttonStyle = new GUIStyle(this.regularUI.button);
		this.buttonStyle.fontSize = Screen.height / 32;
		this.buttonStyle.padding.top = 4;
		this.buttonStyle.padding.bottom = 4;
		this.activeButtonStyle = new GUIStyle(this.buttonStyle);
		this.activeButtonStyle.normal.background = this.activeButtonStyle.hover.background;
		this.activeButtonStyle.active.background = this.activeButtonStyle.hover.background;
	}

	// Token: 0x0600115C RID: 4444 RVA: 0x00076550 File Offset: 0x00074750
	private void OnGUI()
	{
		GUI.depth = 21;
		GUI.skin = this.settingsSkin;
		GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), ResourceManager.LoadTexture("DeckBuilder/bg"));
		Rect rect;
		rect..ctor((float)Screen.width * 0.5f - (float)Screen.height * 0.52f, (float)Screen.height * 0.2f, (float)Screen.height * 0.5f, (float)Screen.height * 0.7f);
		new ScrollsFrame(rect).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.CENTER).Draw();
		Rect rect2;
		rect2..ctor((float)Screen.width * 0.5f + (float)Screen.height * 0.02f, (float)Screen.height * 0.2f, (float)Screen.height * 0.5f, (float)Screen.height * 0.7f);
		new ScrollsFrame(rect2).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.CENTER).Draw();
		GUI.skin.label.fontSize = Screen.height / 32;
		GUI.skin.label.alignment = 4;
		float num = (float)Screen.width * 0.5f - (float)Screen.height * 0.27f;
		float num2 = (float)Screen.height * 0.34f;
		if (!App.IsBorderlessWindow)
		{
			GUI.skin = this.regularUI;
			int fontSize = GUI.skin.label.fontSize;
			GUI.skin.label.fontSize = Screen.height / 32;
			GUI.Label(new Rect(num - num2 / 2f, rect2.y + (float)Screen.height * 0.03f, num2, (float)Screen.height * 0.05f), "Resolutions");
			GUI.skin.label.fontSize = fontSize;
			Rect rect3;
			rect3..ctor(num - num2 / 2f, rect2.y + (float)Screen.height * 0.1f, num2, (float)Screen.height * 0.15f);
			GUILayout.BeginArea(rect3);
			this.resolutionScroll = GUILayout.BeginScrollView(this.resolutionScroll, new GUILayoutOption[]
			{
				GUILayout.Width(rect3.width),
				GUILayout.Height(rect3.height)
			});
			foreach (Resolution resolution in Screen.resolutions)
			{
				bool flag = resolution.width == this.selected.width && resolution.height == this.selected.height;
				if (GUILayout.Button(resolution.width + " x " + resolution.height, (!flag) ? this.buttonStyle : this.activeButtonStyle, new GUILayoutOption[0]))
				{
					App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
					this.selected = resolution;
				}
				GUI.skin = this.regularUI;
			}
			GUILayout.EndScrollView();
			GUILayout.EndArea();
			Rect labelRect;
			labelRect..ctor(num - (float)Screen.height * 0.16f, (float)Screen.height * 0.46f, (float)Screen.height * 0.2f, (float)Screen.height * 0.04f);
			Rect checkboxRect;
			checkboxRect..ctor(num + (float)Screen.height * 0.13f, (float)Screen.height * 0.46f, (float)Screen.height * 0.04f, (float)Screen.height * 0.04f + 2f);
			if (new Checkbox("Full screen", labelRect, checkboxRect, GUI.skin, 1f).Draw(ref this.fullscreenToggle))
			{
			}
			int fontSize2 = GUI.skin.button.fontSize;
			GUI.skin.button.fontSize = Screen.height / 36;
			if (GUI.Button(new Rect(num - num2 / 4f, (float)Screen.height * 0.51f, num2 / 2f, (float)Screen.height * 0.05f), "Apply"))
			{
				this.ApplyResolution();
				base.StartCoroutine(this.InitSoon());
			}
			GUI.skin.button.fontSize = fontSize2;
		}
		else
		{
			GUI.skin = this.regularUI;
			TextAnchor alignment = GUI.skin.label.alignment;
			GUI.skin.label.alignment = 4;
			int fontSize3 = GUI.skin.label.fontSize;
			GUI.skin.label.fontSize = Screen.height / 42;
			GUI.Label(new Rect(num - num2 / 2f, rect2.y + (float)Screen.height * 0.03f, num2, (float)Screen.height * 0.2f), "Resolution settings are currently disabled.\n\nTo choose another resolution, restart the game from the launcher with the \"borderless window fullscreen\" option disabled.");
			GUI.skin.label.fontSize = fontSize3;
			GUI.skin.label.alignment = alignment;
		}
		if (App.IsStandalone)
		{
			int fontSize4 = GUI.skin.button.fontSize;
			GUI.skin.button.fontSize = Screen.height / 36;
			float num3 = num2 / 1.5f;
			if (GUI.Button(new Rect(num - 0.5f * num3, (float)Screen.height * 0.8f, num3, (float)Screen.height * 0.05f), "Change password"))
			{
				this.setPasswordScreen = new SetPasswordScreen();
			}
			GUI.skin.button.fontSize = fontSize4;
		}
		GUI.skin = this.settingsSkin;
		num = (float)Screen.width * 0.5f + (float)Screen.height * 0.27f;
		GUI.skin.horizontalSlider.fixedHeight = (float)(Screen.height / 30);
		GUI.skin.horizontalSliderThumb.fixedHeight = (float)(Screen.height / 30);
		Rect rect4;
		rect4..ctor(num - num2 / 2f, (float)Screen.height * 0.22f, num2, (float)Screen.height * 0.05f);
		GUI.Label(rect4, "SFX volume: " + (this.soundVolume * 100f).ToString("N0") + "%");
		rect4.y += (float)Screen.height * 0.05f;
		this.soundVolume = GUI.HorizontalSlider(rect4, this.soundVolume, 0f, 1f);
		if (!Mathf.Approximately(App.AudioScript.GetVolume(AudioScript.ESoundType.SFX), this.soundVolume))
		{
			App.AudioScript.SetVolume(AudioScript.ESoundType.SFX, this.soundVolume);
		}
		Rect rect5;
		rect5..ctor(num - num2 / 2f, (float)Screen.height * 0.31f, num2, (float)Screen.height * 0.05f);
		GUI.Label(rect5, "Music volume: " + (this.musicVolume * 100f).ToString("N0") + "%");
		rect5.y += (float)Screen.height * 0.05f;
		this.musicVolume = GUI.HorizontalSlider(rect5, this.musicVolume, 0f, 1f);
		if (!Mathf.Approximately(App.AudioScript.GetVolume(AudioScript.ESoundType.MUSIC), this.musicVolume))
		{
			App.AudioScript.SetVolume(AudioScript.ESoundType.MUSIC, this.musicVolume);
		}
		GUI.skin.label.fontSize = 10 + Screen.height / 72;
		GUI.skin.label.alignment = 3;
		float num4 = 0.37f * (float)Screen.height;
		float num5 = 0.05f * (float)Screen.height;
		num4 += num5;
		Rect labelRect2;
		labelRect2..ctor(num - (float)Screen.height * 0.16f, num4, (float)Screen.height * 0.2f, (float)Screen.height * 0.04f);
		Rect checkboxRect2;
		checkboxRect2..ctor(num + (float)Screen.height * 0.13f, num4, (float)Screen.height * 0.04f, (float)Screen.height * 0.04f + 2f);
		if (new Checkbox("Chat message sound", labelRect2, checkboxRect2, GUI.skin, 1f).Draw(ref this.chatToggle))
		{
			App.AudioScript.SetSoundToggle(AudioScript.ESoundToggle.CHAT, this.chatToggle);
		}
		num4 += num5;
		Rect labelRect3;
		labelRect3..ctor(num - (float)Screen.height * 0.16f, num4, (float)Screen.height * 0.2f, (float)Screen.height * 0.04f);
		Rect checkboxRect3;
		checkboxRect3..ctor(num + (float)Screen.height * 0.13f, num4, (float)Screen.height * 0.04f, (float)Screen.height * 0.04f + 2f);
		if (new Checkbox("Chat highlight sound", labelRect3, checkboxRect3, GUI.skin, 1f).Draw(ref this.chatHighlightToggle))
		{
			App.AudioScript.SetSoundToggle(AudioScript.ESoundToggle.CHAT_HIGHLIGHT, this.chatHighlightToggle);
		}
		num4 += num5;
		Rect labelRect4;
		labelRect4..ctor(num - (float)Screen.height * 0.16f, num4, (float)Screen.height * 0.26f, (float)Screen.height * 0.06f);
		Rect checkboxRect4;
		checkboxRect4..ctor(num + (float)Screen.height * 0.13f, num4, (float)Screen.height * 0.04f, (float)Screen.height * 0.04f + 2f);
		new Checkbox("Pause music when game goes into background", labelRect4, checkboxRect4, GUI.skin, 1f).Draw(App.Config.settings.music.pause_when_minimized);
		num4 += num5 + (float)Screen.height * 0.02f;
		Rect labelRect5;
		labelRect5..ctor(num - (float)Screen.height * 0.16f, num4, (float)Screen.height * 0.2f, (float)Screen.height * 0.04f);
		Rect checkboxRect5;
		checkboxRect5..ctor(num + (float)Screen.height * 0.13f, num4, (float)Screen.height * 0.04f, (float)Screen.height * 0.04f + 2f);
		new Checkbox("Upgraded scroll visuals", labelRect5, checkboxRect5, GUI.skin, 1f).Draw(App.Config.settings.preferences.tier_visuals);
		num4 += num5;
		Rect labelRect6;
		labelRect6..ctor(num - (float)Screen.height * 0.16f, num4, (float)Screen.height * 0.26f, (float)Screen.height * 0.06f);
		Rect checkboxRect6;
		checkboxRect6..ctor(num + (float)Screen.height * 0.13f, num4, (float)Screen.height * 0.04f, (float)Screen.height * 0.04f + 2f);
		new Checkbox("Enable chat profanity filter", labelRect6, checkboxRect6, GUI.skin, 1f).Draw(App.Config.settings.preferences.profanity_filter);
		num4 += num5;
		GUI.Label(new Rect(num - (float)Screen.height * 0.16f, num4, (float)Screen.height * 0.2f, (float)Screen.height * 0.04f), "Spectators");
		Rect rect6;
		rect6..ctor(num - (float)Screen.height * 0.06f, num4, (float)Screen.height * 0.23f, (float)Screen.height * 0.04f + 2f);
		this.spectateDrop.SetRect(rect6);
		num4 += num5;
		GUI.Label(new Rect(num - (float)Screen.height * 0.16f, num4, (float)Screen.height * 0.2f, (float)Screen.height * 0.04f), "Battlefield");
		Rect rect7;
		rect7..ctor(num - (float)Screen.height * 0.06f, num4, (float)Screen.height * 0.23f, (float)Screen.height * 0.04f + 2f);
		this.battleBgDrop.SetRect(rect7);
		GUI.color = new Color(1f, 1f, 1f, this.fadeIn);
		GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), ResourceManager.LoadTexture("Shared/blackFiller"));
		GUI.color = new Color(1f, 1f, 1f, 1f);
	}

	// Token: 0x0600115D RID: 4445 RVA: 0x00077170 File Offset: 0x00075370
	private IEnumerator InitSoon()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		this.Init();
		yield break;
	}

	// Token: 0x0600115E RID: 4446 RVA: 0x0000D435 File Offset: 0x0000B635
	private void ApplyResolution()
	{
		App.Config.SetResolution(this.selected.width, this.selected.height, this.fullscreenToggle);
	}

	// Token: 0x0600115F RID: 4447 RVA: 0x0007718C File Offset: 0x0007538C
	private void GetResolution()
	{
		this.selected = default(Resolution);
		this.selected.width = Screen.width;
		this.selected.height = Screen.height;
		this.fullscreenToggle = Screen.fullScreen;
	}

	// Token: 0x06001160 RID: 4448 RVA: 0x0000D45D File Offset: 0x0000B65D
	private void FixedUpdate()
	{
		if (this.fadeIn > 0f)
		{
			this.fadeIn -= 0.03f;
		}
	}

	// Token: 0x06001161 RID: 4449 RVA: 0x000771D4 File Offset: 0x000753D4
	public override void handleMessage(Message msg)
	{
		if (msg is SpectateGetPermissionMessage)
		{
			SpectateGetPermissionMessage spectateGetPermissionMessage = (SpectateGetPermissionMessage)msg;
			foreach (KeyValuePair<string, SpectatePermission> keyValuePair in this.stringToSpectateEnum)
			{
				if (keyValuePair.Value == spectateGetPermissionMessage.permission)
				{
					this.spectateDrop.SetSelectedItem(keyValuePair.Key);
					break;
				}
			}
		}
		if (msg is OkMessage)
		{
			OkMessage okMessage = (OkMessage)msg;
			if (okMessage.isType(typeof(SetPasswordMessage)) && this.setPasswordScreen != null)
			{
				App.Popups.KillCurrentPopup();
				App.Popups.ShowOk(new OkVoidCallback(), "password-ok", "Successful", "Password changed successfully", "Ok");
				this.setPasswordScreen = null;
			}
		}
		if (msg is FailMessage)
		{
			FailMessage failMessage = (FailMessage)msg;
			if (failMessage.isType(typeof(SetPasswordMessage)) && this.setPasswordScreen != null)
			{
				this.setPasswordScreen.setError(failMessage.info);
			}
		}
	}

	// Token: 0x04000DC4 RID: 3524
	private GUISkin settingsSkin;

	// Token: 0x04000DC5 RID: 3525
	private GUISkin regularUI;

	// Token: 0x04000DC6 RID: 3526
	private float soundVolume;

	// Token: 0x04000DC7 RID: 3527
	private float musicVolume;

	// Token: 0x04000DC8 RID: 3528
	private bool chatToggle;

	// Token: 0x04000DC9 RID: 3529
	private bool chatHighlightToggle;

	// Token: 0x04000DCA RID: 3530
	private bool fullscreenToggle;

	// Token: 0x04000DCB RID: 3531
	private Vector2 resolutionScroll;

	// Token: 0x04000DCC RID: 3532
	private Resolution selected;

	// Token: 0x04000DCD RID: 3533
	private float fadeIn = 1f;

	// Token: 0x04000DCE RID: 3534
	private GUIStyle buttonStyle;

	// Token: 0x04000DCF RID: 3535
	private GUIStyle activeButtonStyle;

	// Token: 0x04000DD0 RID: 3536
	private SetPasswordScreen setPasswordScreen;

	// Token: 0x04000DD1 RID: 3537
	private Dropdown spectateDrop;

	// Token: 0x04000DD2 RID: 3538
	private Dropdown battleBgDrop;

	// Token: 0x04000DD3 RID: 3539
	private Dictionary<string, SpectatePermission> stringToSpectateEnum = new Dictionary<string, SpectatePermission>();
}
