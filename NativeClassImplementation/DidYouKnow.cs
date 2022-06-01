using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001B8 RID: 440
public class DidYouKnow : AbstractCommListener
{
	// Token: 0x06000DD8 RID: 3544 RVA: 0x0000AF1B File Offset: 0x0000911B
	private void Start()
	{
		App.Communicator.addListener(this);
		this.regularUI = (GUISkin)ResourceManager.Load("_GUISkins/RegularUI");
		this.emptySkin = (GUISkin)ResourceManager.Load("_GUISkins/EmptySkin");
	}

	// Token: 0x06000DD9 RID: 3545 RVA: 0x0000AF53 File Offset: 0x00009153
	public void Init()
	{
		App.Communicator.send(new DidYouKnowMessage());
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x0005F68C File Offset: 0x0005D88C
	private void OnGUI()
	{
		GUI.depth = 6;
		if (this.didYouKnowString != null)
		{
			GUI.skin = this.regularUI;
			int fontSize = GUI.skin.label.fontSize;
			TextAnchor alignment = GUI.skin.label.alignment;
			GUI.skin.label.fontSize = Screen.height / 40;
			GUI.skin.label.alignment = 4;
			Rect rect;
			rect..ctor((float)Screen.width * 0.3f, (float)Screen.height * 0.8f, (float)Screen.width * 0.4f, (float)Screen.height * 0.09f);
			Rect rect2;
			rect2..ctor(rect.x + (float)Screen.height * 0.03f, rect.y + 5f, rect.width - (float)Screen.height * 0.06f, rect.height - 10f);
			GUI.skin = this.emptySkin;
			if (this.isActive && GUI.Button(new Rect(rect.x, rect.y, rect.width, rect.height * 1.5f), string.Empty))
			{
				App.Communicator.send(new DidYouKnowMessage(this.didYouKnowId + 1));
			}
			GUI.skin = this.regularUI;
			GUI.color = new Color(1f, 1f, 1f, this.alpha * 0.2f);
			GUI.Box(rect, string.Empty);
			GUI.color = new Color(1f, 1f, 1f, this.alpha);
			string text = ((int)(this.alpha * 255f * 0.8f)).ToString("X2").ToLower();
			string text2 = "<color=#ccbb95" + text + ">";
			GUI.Label(rect2, text2 + this.didYouKnowString + "</color>");
			string text3 = "<color=#887766" + text + ">";
			GUI.skin.label.fontSize = Screen.height / 50;
			GUI.Label(new Rect(rect2.x, rect2.yMax + 6f, rect2.width, rect2.height / 3f), text3 + "Click to view next hint</color>");
			GUI.color = Color.white;
			GUI.skin.label.alignment = 5;
			GUI.skin.label.fontSize = fontSize;
			GUI.skin.label.alignment = alignment;
		}
	}

	// Token: 0x06000DDB RID: 3547 RVA: 0x0005F930 File Offset: 0x0005DB30
	public void SetActive(bool isActive)
	{
		this.isActive = isActive;
		if (isActive && !string.IsNullOrEmpty(this.didYouKnowString))
		{
			base.StartCoroutine(this.Fade(1f));
		}
		if (!isActive)
		{
			base.StartCoroutine(this.Fade(0f));
		}
	}

	// Token: 0x06000DDC RID: 3548 RVA: 0x0005F984 File Offset: 0x0005DB84
	private IEnumerator Fade(float to)
	{
		float from = this.alpha;
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime * 4f;
			if (t > 1f)
			{
				t = 1f;
			}
			float factor = t * t * (3f - 2f * t);
			this.alpha = from * (1f - factor) + to * factor;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000DDD RID: 3549 RVA: 0x0005F9B0 File Offset: 0x0005DBB0
	public override void handleMessage(Message msg)
	{
		if (msg is DidYouKnowMessage)
		{
			DidYouKnowMessage didYouKnowMessage = (DidYouKnowMessage)msg;
			this.didYouKnowString = didYouKnowMessage.hint;
			this.didYouKnowId = didYouKnowMessage.id;
			if (this.isActive)
			{
				base.StartCoroutine(this.Fade(1f));
			}
		}
		base.handleMessage(msg);
	}

	// Token: 0x04000ACB RID: 2763
	private string didYouKnowString;

	// Token: 0x04000ACC RID: 2764
	private int didYouKnowId;

	// Token: 0x04000ACD RID: 2765
	private GUISkin regularUI;

	// Token: 0x04000ACE RID: 2766
	private GUISkin emptySkin;

	// Token: 0x04000ACF RID: 2767
	private bool isActive = true;

	// Token: 0x04000AD0 RID: 2768
	private float alpha;
}
