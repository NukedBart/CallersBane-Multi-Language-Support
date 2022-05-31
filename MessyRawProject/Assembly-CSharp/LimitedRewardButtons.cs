using System;
using UnityEngine;

// Token: 0x02000207 RID: 519
public class LimitedRewardButtons : MonoBehaviour
{
	// Token: 0x06001059 RID: 4185 RVA: 0x0000CB57 File Offset: 0x0000AD57
	public void Init(LimitedReward limitedReward)
	{
		this.limitedReward = limitedReward;
	}

	// Token: 0x0600105A RID: 4186 RVA: 0x0000CB60 File Offset: 0x0000AD60
	private void Start()
	{
		this.emptySkin = (GUISkin)ResourceManager.Load("_GUISkins/EmptySkin");
		this.regularUI = (GUISkin)ResourceManager.Load("_GUISkins/RegularUI");
	}

	// Token: 0x0600105B RID: 4187 RVA: 0x0006DB88 File Offset: 0x0006BD88
	private void OnGUI()
	{
		GUI.depth = 5;
		GUI.skin = this.regularUI;
		float num = (float)Screen.height * 0.15f;
		float num2 = (float)Screen.height * 0.06f;
		float num3 = (float)Screen.height * 0.02f;
		GUI.skin = this.regularUI;
		int fontSize = GUI.skin.button.fontSize;
		GUI.skin.button.fontSize = Screen.height / 24;
		GUI.Box(new Rect((float)(Screen.width / 2) - num - num3 * 1.5f, (float)Screen.height * 0.85f - num3, num * 2f + num3 * 3f, num2 + num3 * 2f), string.Empty);
		GUI.enabled = this.buttonsEnabled;
		if (GUI.Button(new Rect((float)(Screen.width / 2) - num - num3 / 2f, (float)Screen.height * 0.85f, num, num2), "Pick"))
		{
			this.limitedReward.ConfirmScrollChoice(true);
		}
		if (GUI.Button(new Rect((float)(Screen.width / 2) + num3 / 2f, (float)Screen.height * 0.85f, num, num2), "Cancel"))
		{
			this.limitedReward.ConfirmScrollChoice(false);
		}
		GUI.skin = this.emptySkin;
		if (GUI.Button(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), string.Empty))
		{
			this.limitedReward.ConfirmScrollChoice(false);
		}
		GUI.enabled = true;
		GUI.skin.button.fontSize = fontSize;
	}

	// Token: 0x0600105C RID: 4188 RVA: 0x0000CB8C File Offset: 0x0000AD8C
	public void setButtonsEnabled(bool enabled)
	{
		this.buttonsEnabled = enabled;
	}

	// Token: 0x04000CE4 RID: 3300
	private LimitedReward limitedReward;

	// Token: 0x04000CE5 RID: 3301
	private GUISkin regularUI;

	// Token: 0x04000CE6 RID: 3302
	private GUISkin emptySkin;

	// Token: 0x04000CE7 RID: 3303
	private bool buttonsEnabled = true;
}
