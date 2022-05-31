using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000189 RID: 393
public class AudioTestScene : DeckBuilder2, Unit.ICallback
{
	// Token: 0x06000C37 RID: 3127 RVA: 0x00056B20 File Offset: 0x00054D20
	public new void Update()
	{
		base.Update();
		if (Input.GetKeyDown(9))
		{
			this.show = !this.show;
		}
		if (Input.GetKeyDown(114))
		{
			this.reloadSounds(true);
		}
		if (!this.show)
		{
			return;
		}
		this.updateReplay();
	}

	// Token: 0x06000C38 RID: 3128 RVA: 0x00056B74 File Offset: 0x00054D74
	public new void OnGUI()
	{
		base.OnGUI();
		if (!this.show)
		{
			return;
		}
		GUI.skin = null;
		Rect area = this.unitCalc.r(0f, 0.65f, 0.5f, 0.25f);
		Func<float, float, float, float, Rect> func = (float x, float y, float w, float h) => GeomUtil.cropShare(area, new Rect(x, y, w, h));
		GUI.Box(area, string.Empty);
		bool flag = this.repeatingAnimation;
		this.repeatingAnimation = GUI.Toggle(func.Invoke(0f, 0f, 0.2f, 0.3f), this.repeatingAnimation, "Repeat");
		if (this.repeatingAnimation != flag)
		{
			this.playCount = 0;
		}
		float num = (float)Screen.height * 0.05f;
		GUI.enabled = (this.currentCard != null);
		if (GUI.Button(new Rect(this.rectRight.x, (float)Screen.height - num, this.rectRight.width, num), "Create TAG file"))
		{
			this.currentCard.getCardType().writeDefaultTagsToDisk();
		}
		GUI.enabled = true;
	}

	// Token: 0x06000C39 RID: 3129 RVA: 0x00056C94 File Offset: 0x00054E94
	private void updateReplay()
	{
		if (base.getUnit() == null)
		{
			return;
		}
		if (base.getUnit().isAnimationDone() && (this.repeatingAnimation || --this.playCount >= 0))
		{
			this.replayAnimation();
		}
	}

	// Token: 0x06000C3A RID: 3130 RVA: 0x00056CEC File Offset: 0x00054EEC
	private void updateReloadSounds()
	{
		long num = TimeUtil.CurrentTimeMillis();
		long num2 = num - this.lastUpdateReloadSounds;
		if (num2 < 1000L)
		{
			return;
		}
		this.reloadSounds(false);
		this.lastUpdateReloadSounds = num;
	}

	// Token: 0x06000C3B RID: 3131 RVA: 0x00009FD7 File Offset: 0x000081D7
	protected override void showCardUnit(Card card)
	{
		base.showCardUnit(card);
		if (base.getUnit() == null)
		{
			return;
		}
		base.getUnit().callBackTarget = this;
		this.reloadSounds(true);
		this.playCount = 0;
	}

	// Token: 0x06000C3C RID: 3132 RVA: 0x00056D24 File Offset: 0x00054F24
	private List<string> getCurrentSounds()
	{
		return new List<string>();
	}

	// Token: 0x06000C3D RID: 3133 RVA: 0x00056D38 File Offset: 0x00054F38
	private void reloadSounds(bool force)
	{
		foreach (string fn in this.getCurrentSounds())
		{
			string path = AudioTestScene.getPath(fn);
			if (path != null)
			{
				FileInfo fileInfo = new FileInfo(path);
				DateTime dateTime;
				this.lastModificationTimes.TryGetValue(path, ref dateTime);
				if (force || !(dateTime == fileInfo.LastWriteTimeUtc))
				{
					this.reloadSound(fn);
					this.lastModificationTimes[path] = fileInfo.LastWriteTimeUtc;
					this.playCount = 3;
				}
			}
		}
	}

	// Token: 0x06000C3E RID: 3134 RVA: 0x0000A00C File Offset: 0x0000820C
	private void reloadSound(string fn)
	{
		base.StartCoroutine(this._reloadSound(fn));
	}

	// Token: 0x06000C3F RID: 3135 RVA: 0x0000A01C File Offset: 0x0000821C
	public static string getPath(string fn)
	{
		return AudioTestScene.getPath(fn, ".wav");
	}

	// Token: 0x06000C40 RID: 3136 RVA: 0x0000A029 File Offset: 0x00008229
	public static string getPath(string fn, string ext)
	{
		return StorageEnvironment.getAddonPath("sounds/") + fn + ext;
	}

	// Token: 0x06000C41 RID: 3137 RVA: 0x00056DF0 File Offset: 0x00054FF0
	private IEnumerator _reloadSound(string fn)
	{
		string fullfn = "file://" + AudioTestScene.getPath(fn);
		WWW p = new WWW(fullfn);
		yield return p;
		App.AudioScript.InjectSound(fn, p.audioClip);
		yield break;
	}

	// Token: 0x06000C42 RID: 3138 RVA: 0x0000A03C File Offset: 0x0000823C
	private void replayAnimation()
	{
		base.playAttackAnimation();
	}

	// Token: 0x06000C43 RID: 3139 RVA: 0x000028DF File Offset: 0x00000ADF
	public void cameraShake(float strength)
	{
	}

	// Token: 0x06000C44 RID: 3140 RVA: 0x000028DF File Offset: 0x00000ADF
	public void effectDone()
	{
	}

	// Token: 0x06000C45 RID: 3141 RVA: 0x000028DF File Offset: 0x00000ADF
	public void playHitAnimation()
	{
	}

	// Token: 0x04000974 RID: 2420
	private bool show;

	// Token: 0x04000975 RID: 2421
	private MockupCalc unitCalc = new MockupCalc(1, 1);

	// Token: 0x04000976 RID: 2422
	private bool repeatingAnimation;

	// Token: 0x04000977 RID: 2423
	private long lastUpdateReloadSounds = TimeUtil.CurrentTimeMillis();

	// Token: 0x04000978 RID: 2424
	private Dictionary<string, DateTime> lastModificationTimes = new Dictionary<string, DateTime>();

	// Token: 0x04000979 RID: 2425
	private int playCount;
}
