using System;
using System.Collections;
using Gui;
using UnityEngine;

// Token: 0x020001F6 RID: 502
public class GoldCountup : GoldPile
{
	// Token: 0x06000FBA RID: 4026 RVA: 0x0000C86B File Offset: 0x0000AA6B
	public void init(Gui3D gui, int goldEarned, int layer, bool playAnimation)
	{
		this.goldEarned = goldEarned;
		base.init(gui, layer, playAnimation);
		this.labelStyle = new GUIStyle(((GUISkin)ResourceManager.Load("_GUISkins/RegularUI")).label);
		this.labelStyle.alignment = 1;
	}

	// Token: 0x06000FBB RID: 4027 RVA: 0x0000C8A9 File Offset: 0x0000AAA9
	public override void run()
	{
		base.StartCoroutine(this._run());
		base.run();
	}

	// Token: 0x06000FBC RID: 4028 RVA: 0x00069728 File Offset: 0x00067928
	private IEnumerator _run()
	{
		this.loopGoldSound = true;
		if (this.playAnimation)
		{
			base.StartCoroutine(this.GoldSound());
			yield return base.StartCoroutine(this.countupGold());
		}
		this.goldEarnedCountup = this.goldEarned;
		this.loopGoldSound = false;
		yield break;
	}

	// Token: 0x06000FBD RID: 4029 RVA: 0x0000C8BE File Offset: 0x0000AABE
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.stopSound();
	}

	// Token: 0x06000FBE RID: 4030 RVA: 0x0000C8CC File Offset: 0x0000AACC
	public void skip()
	{
		this.goldEarnedCountup = this.goldEarned;
		this._countupValue = 1f;
		this.stopSound();
	}

	// Token: 0x06000FBF RID: 4031 RVA: 0x00069744 File Offset: 0x00067944
	private IEnumerator GoldSound()
	{
		App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_coin_tally_loop", true);
		App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_coin_tally_loop", true);
		while (this.loopGoldSound)
		{
			yield return null;
		}
		this.stopSound();
		App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_coin_tally_end");
		yield break;
	}

	// Token: 0x06000FC0 RID: 4032 RVA: 0x0000C8EB File Offset: 0x0000AAEB
	private void stopSound()
	{
		this.loopGoldSound = false;
		App.AudioScript.StopSound("Sounds/hyperduck/UI/ui_coin_tally_loop", false);
	}

	// Token: 0x06000FC1 RID: 4033 RVA: 0x00069760 File Offset: 0x00067960
	private IEnumerator countupGold()
	{
		this._countupValue = 0f;
		float timeStarted = Time.time;
		float duration = Mathf.Min(1.5f, (float)this.goldEarned / 200f);
		while (this._countupValue < 1f)
		{
			this._countupValue = Mathf.Min((Time.time - timeStarted) / duration, 1f);
			this.goldEarnedCountup = Mathf.RoundToInt(Mathf.Lerp(0f, (float)this.goldEarned, this._countupValue));
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000FC2 RID: 4034 RVA: 0x0006977C File Offset: 0x0006797C
	protected void OnGUI()
	{
		GUI.depth = 10;
		this.labelStyle.fontSize = (int)base.scaleY((float)Screen.height / 4f);
		float num = (float)Screen.width * this.unitX - (float)Screen.width * 0.2f;
		GUI.Label(base.transformRect(new Rect(num, (float)Screen.height * 0.41f + (float)Screen.height * 0.007f, (float)Screen.width * 0.4f, (float)Screen.height * 0.3f)), "<color=#000000>" + this.goldEarnedCountup + "</color>", this.labelStyle);
		GUI.Label(base.transformRect(new Rect(num, (float)Screen.height * 0.41f, (float)Screen.width * 0.4f, (float)Screen.height * 0.3f)), "<color=#ffcc44>" + this.goldEarnedCountup + "</color>", this.labelStyle);
		this.labelStyle.fontSize = (int)base.scaleY((float)Screen.height / 10.5f);
		GUI.Label(base.transformRect(new Rect(num, (float)Screen.height * 0.375f + (float)Screen.height * 0.004f, (float)Screen.width * 0.4f, (float)Screen.height * 0.3f)), "<color=#000000>Gold</color>", this.labelStyle);
		GUI.Label(base.transformRect(new Rect(num, (float)Screen.height * 0.375f, (float)Screen.width * 0.4f, (float)Screen.height * 0.3f)), "<color=#ee9922>Gold</color>", this.labelStyle);
	}

	// Token: 0x04000C40 RID: 3136
	private int goldEarned;

	// Token: 0x04000C41 RID: 3137
	private int goldEarnedCountup;

	// Token: 0x04000C42 RID: 3138
	private float _countupValue;

	// Token: 0x04000C43 RID: 3139
	private GUIStyle labelStyle;

	// Token: 0x04000C44 RID: 3140
	private bool loopGoldSound;
}
