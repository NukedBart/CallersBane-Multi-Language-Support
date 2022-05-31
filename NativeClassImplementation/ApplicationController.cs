using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using CommConfig;
using UnityEngine;

// Token: 0x0200001E RID: 30
public class ApplicationController : MonoBehaviour, IOkCallback
{
	// Token: 0x060001C0 RID: 448 RVA: 0x0000372B File Offset: 0x0000192B
	private void Start()
	{
		this.hasBeenStarted = true;
		this.CheckForUpdate();
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x0000373A File Offset: 0x0000193A
	private void OnApplicationFocus(bool focus)
	{
		if (this.hasBeenStarted)
		{
			this.PauseRendering(!focus);
		}
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x00003751 File Offset: 0x00001951
	private void OnApplicationPause(bool pause)
	{
		if (this.hasBeenStarted)
		{
			this.PauseRendering(pause);
		}
		if (pause)
		{
			ConnectionRegistry.resetDisconnectedTime();
		}
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x00003770 File Offset: 0x00001970
	private void PauseRendering(bool pause)
	{
		this.isPaused = pause;
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x00003779 File Offset: 0x00001979
	private void LateUpdate()
	{
		if (this.isPaused)
		{
			Thread.Sleep(50);
		}
		this.updateDisconnectCheck();
		this.updateFps();
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x00003799 File Offset: 0x00001999
	private void updateFps()
	{
		this.fps.Update();
		if (this.fps.HasUpdatedFps())
		{
			Log.info("FPS: " + this.fps.Fps());
		}
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x00020030 File Offset: 0x0001E230
	private void updateDisconnectCheck()
	{
		bool flag = this.connectionCheckEnabled && ConnectionRegistry.updateDisconnectCheck();
		if (flag == this.wasDisconnected)
		{
			return;
		}
		this.wasDisconnected = flag;
		if (flag)
		{
			Log.warning("Connection: DOWN. Showing reconnect popup");
			App.Popups.ShowReconnectPopup();
		}
		else
		{
			Log.warning("Connection: OK. Killing reconnect popup");
			App.Popups.KillReconnectPopup();
		}
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x000037D5 File Offset: 0x000019D5
	public void CheckForUpdate()
	{
		if (Application.isEditor)
		{
			return;
		}
		base.StartCoroutine(this.CheckForUpdateCoroutine());
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x00020098 File Offset: 0x0001E298
	public IEnumerator CheckForUpdateCoroutine()
	{
		if (App.IsStandalone)
		{
			yield break;
		}
		long since = TimeUtil.CurrentTimeMillis() - this.lastUpdateCheck;
		if (since > 10000L)
		{
			yield return base.StartCoroutine(this._getLatestVersion());
		}
		yield return null;
		yield break;
	}

	// Token: 0x060001C9 RID: 457 RVA: 0x000200B4 File Offset: 0x0001E2B4
	private void ShowUpdateScreen()
	{
		if (OsSpec.getOS() == OSType.Linux)
		{
			App.Popups.ShowOk(this, "downloadlinux", I18n.Text("Your version of {GAME_NAME} is outdated"), I18n.Text("Your {GAME_NAME} client needs to be updated. Press the \"Ok\" button to close {GAME_NAME} and download the latest experimental Linux client."), "Ok");
		}
		else
		{
			App.Popups.ShowOk(this, "update", I18n.Text("Your version of {GAME_NAME} is outdated"), I18n.Text("Your {GAME_NAME} client needs to be updated. Press the \"Ok\" button to close {GAME_NAME} and start the update process."), "Ok");
		}
	}

	// Token: 0x060001CA RID: 458 RVA: 0x00020124 File Offset: 0x0001E324
	private void CallUpdater()
	{
		string text = "Scrolls.exe";
		string text2 = "scrolls.sh";
		if (App.Communicator.UseHost == Host.Amazon_Test)
		{
			text = "ScrollsTest.exe";
			text2 = "scrollstest.sh";
		}
		OSType os = OsSpec.getOS();
		string fileName;
		if (os != OSType.Windows)
		{
			if (os != OSType.OSX)
			{
				return;
			}
			fileName = OsSpec.getInstalledPath() + "/" + text2;
		}
		else
		{
			fileName = OsSpec.getInstalledPath() + "/" + text;
		}
		new Process
		{
			StartInfo = 
			{
				FileName = fileName,
				Arguments = "--install"
			}
		}.Start();
		Application.Quit();
	}

	// Token: 0x060001CB RID: 459 RVA: 0x000201D4 File Offset: 0x0001E3D4
	private IEnumerator _getLatestVersion()
	{
		string url = "http://download.scrolls.com/client/";
		if (App.Communicator.UseHost == Host.Amazon_Test)
		{
			url = "http://download.scrolls.com/clienttest/";
		}
		url = url + OsSpec.getOSSuffix().ToLower() + "/version";
		WWW www = new WWW(url);
		yield return www;
		if (www.error == null)
		{
			Version latestVersion = new Version(www.text);
			this.compareVersions(latestVersion);
			this.lastUpdateCheck = TimeUtil.CurrentTimeMillis();
		}
		else
		{
			Log.info("ERROR reading version file");
		}
		yield break;
	}

	// Token: 0x060001CC RID: 460 RVA: 0x000037EF File Offset: 0x000019EF
	public bool NeedUpdate()
	{
		return this.needUpdate;
	}

	// Token: 0x060001CD RID: 461 RVA: 0x000037F7 File Offset: 0x000019F7
	public void setConnectionCheckEnabled(bool enabled)
	{
		this.connectionCheckEnabled = enabled;
	}

	// Token: 0x060001CE RID: 462 RVA: 0x000201F0 File Offset: 0x0001E3F0
	private void compareVersions(Version latestVersion)
	{
		this.needUpdate = SharedConstants.getGameVersion().isLowerThan(latestVersion);
		Log.info(string.Concat(new object[]
		{
			"ApplicationController: Versions - latest, game, outdated: ",
			latestVersion.ToString(),
			", ",
			SharedConstants.getGameVersion().ToString(),
			", ",
			this.needUpdate
		}));
		if (this.needUpdate)
		{
			this.ShowUpdateScreen();
		}
	}

	// Token: 0x060001CF RID: 463 RVA: 0x0002026C File Offset: 0x0001E46C
	public void PopupOk(string popupType)
	{
		if (popupType != null)
		{
			if (ApplicationController.<>f__switch$mapD == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("update", 0);
				dictionary.Add("downloadlinux", 1);
				ApplicationController.<>f__switch$mapD = dictionary;
			}
			int num;
			if (ApplicationController.<>f__switch$mapD.TryGetValue(popupType, ref num))
			{
				if (num != 0)
				{
					if (num == 1)
					{
						string text = "client";
						if (App.Communicator.UseHost == Host.Amazon_Test)
						{
							text = "clienttest";
						}
						Application.OpenURL("http://download.scrolls.com/" + text + "/linux.tar.gz");
						Application.Quit();
					}
				}
				else
				{
					this.CallUpdater();
				}
			}
		}
	}

	// Token: 0x040000D3 RID: 211
	private bool hasBeenStarted;

	// Token: 0x040000D4 RID: 212
	private bool isPaused;

	// Token: 0x040000D5 RID: 213
	private bool needUpdate;

	// Token: 0x040000D6 RID: 214
	private long lastUpdateCheck = -99999L;

	// Token: 0x040000D7 RID: 215
	private FpsCounter fps = new FpsCounter();

	// Token: 0x040000D8 RID: 216
	private bool connectionCheckEnabled = true;

	// Token: 0x040000D9 RID: 217
	private bool wasDisconnected;
}
