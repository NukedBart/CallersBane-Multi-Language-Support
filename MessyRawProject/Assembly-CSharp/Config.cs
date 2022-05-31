using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000176 RID: 374
public class Config : AbstractCommListener
{
	// Token: 0x06000B84 RID: 2948 RVA: 0x000098BD File Offset: 0x00007ABD
	private static string getSaveFilename()
	{
		return Application.persistentDataPath + "/config.txt";
	}

	// Token: 0x06000B85 RID: 2949 RVA: 0x000098CE File Offset: 0x00007ACE
	private static string getSaveFilename(string profile)
	{
		return Application.persistentDataPath + "/config_" + profile + ".txt";
	}

	// Token: 0x06000B86 RID: 2950 RVA: 0x000098E5 File Offset: 0x00007AE5
	private void Awake()
	{
		this.loadGlobalSettings();
		App.Communicator.addListener(this);
	}

	// Token: 0x06000B87 RID: 2951 RVA: 0x000098F9 File Offset: 0x00007AF9
	public void handleMessage(ProfileInfoMessage m)
	{
		if (this.hasLoadedUserSettings)
		{
			return;
		}
		this.hasLoadedUserSettings = true;
		this.loadUserSettings(m.profile.id.ToString());
		App.AudioScript.OnUserSettingsLoaded();
	}

	// Token: 0x06000B88 RID: 2952 RVA: 0x0000992E File Offset: 0x00007B2E
	public void SetResolution(int width, int height, bool fullscreen)
	{
		this.globalSettings.graphics.resolution = new VideoMode(width, height, fullscreen);
		this.ApplyResolution();
	}

	// Token: 0x06000B89 RID: 2953 RVA: 0x0000994E File Offset: 0x00007B4E
	public bool HasLoadedUserSettings()
	{
		return this.hasLoadedUserSettings;
	}

	// Token: 0x06000B8A RID: 2954 RVA: 0x00052A38 File Offset: 0x00050C38
	public void ApplyResolution()
	{
		VideoMode videoMode = this.globalSettings.graphics.resolution;
		if (App.IsBorderlessWindow)
		{
			videoMode = VideoMode.getHighest();
		}
		else if (!VideoMode.isAllowed(videoMode))
		{
			videoMode = VideoMode.getDefault();
		}
		Screen.SetResolution(videoMode.width, videoMode.height, videoMode.fullscreen);
		base.StartCoroutine(this.InitSoon());
	}

	// Token: 0x06000B8B RID: 2955 RVA: 0x00052AB0 File Offset: 0x00050CB0
	private IEnumerator InitSoon()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		App.LobbyMenu.AdjustForResolution();
		App.ChatUI.AdjustToResolution();
		yield break;
	}

	// Token: 0x06000B8C RID: 2956 RVA: 0x00052AC4 File Offset: 0x00050CC4
	private void loadGlobalSettings()
	{
		this.globalSettings = new GlobalSettings();
		if (this._tryLoadOldConfig())
		{
			return;
		}
		string text = FileUtil.readFileContents(Config.getSaveFilename());
		if (text != null)
		{
			this.globalSettings = SettingsSerializer.Read<GlobalSettings>(text);
		}
	}

	// Token: 0x06000B8D RID: 2957 RVA: 0x00052B08 File Offset: 0x00050D08
	private void loadUserSettings(string profile)
	{
		UserSettings userSettings = this.readUserSettings(profile);
		if (userSettings != null)
		{
			this.settings = userSettings;
			this.settings.onLoaded();
		}
	}

	// Token: 0x06000B8E RID: 2958 RVA: 0x00052B38 File Offset: 0x00050D38
	private UserSettings readUserSettings(string profile)
	{
		UserSettings result = new UserSettings();
		if (this.readSettingsFromFilename(result, Config.getSaveFilename(profile)))
		{
			return result;
		}
		if (this.readSettingsFromFilename(result, Config.getSaveFilename()))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06000B8F RID: 2959 RVA: 0x00052B74 File Offset: 0x00050D74
	private bool readSettingsFromFilename(object settings, string filename)
	{
		string text = FileUtil.readFileContents(filename);
		if (text == null)
		{
			return false;
		}
		text = text.Replace("cardlist.marketplace.", "cardlist.marketplace_");
		text = text.Replace("cardlist.store.", "cardlist.store_");
		text = text.Replace("cardlist.trade.", "cardlist.trade_");
		SettingsSerializer.ReadInto(text, settings);
		return true;
	}

	// Token: 0x06000B90 RID: 2960 RVA: 0x00009956 File Offset: 0x00007B56
	public void flushSettings()
	{
		this.flushSettings(App.MyProfile.ProfileInfo.id.ToString());
	}

	// Token: 0x06000B91 RID: 2961 RVA: 0x00052BCC File Offset: 0x00050DCC
	private void flushSettings(string profile)
	{
		if (profile != null && profile != "0")
		{
			List<Room> list = App.ArenaChat.ChatRooms.FilterPersistentRooms();
			App.Config.settings.chat.rooms.set(Enumerable.Select<Room, string>(list, (Room room) => room.name));
			this.serializeSettings(this.settings, Config.getSaveFilename(profile));
		}
		this.serializeSettings(this.globalSettings, Config.getSaveFilename());
	}

	// Token: 0x06000B92 RID: 2962 RVA: 0x00009972 File Offset: 0x00007B72
	private void serializeSettings(object o, string filename)
	{
		FileUtil.writeFileContents(filename, SettingsSerializer.Write(o));
	}

	// Token: 0x06000B93 RID: 2963 RVA: 0x00009980 File Offset: 0x00007B80
	private void OnApplicationQuit()
	{
		this.flushSettings();
	}

	// Token: 0x06000B94 RID: 2964 RVA: 0x00052C60 File Offset: 0x00050E60
	private bool _tryLoadOldConfig()
	{
		string text = FileUtil.readFileContents(Config.getSaveFilename());
		if (text == null)
		{
			return false;
		}
		bool result = false;
		this.globalSettings = new GlobalSettings();
		string[] array = text.Split(new char[]
		{
			'\n'
		});
		foreach (string text2 in array)
		{
			if (text2.Contains("="))
			{
				string[] array3 = text2.Split(new char[]
				{
					'='
				});
				int num;
				if (int.TryParse(array3[1], ref num))
				{
					string text3 = array3[0].ToLower().Trim();
					if (text3 == "width")
					{
						this.globalSettings.graphics.resolution.width.value = num;
					}
					if (text3 == "height")
					{
						this.globalSettings.graphics.resolution.height.value = num;
					}
					if (text3 == "fullscreen")
					{
						this.globalSettings.graphics.resolution.fullscreen.value = (num != 0);
						result = true;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x040008CF RID: 2255
	public GlobalSettings globalSettings;

	// Token: 0x040008D0 RID: 2256
	public UserSettings settings = new UserSettings();

	// Token: 0x040008D1 RID: 2257
	private bool hasLoadedUserSettings;
}
