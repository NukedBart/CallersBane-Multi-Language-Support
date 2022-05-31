using System;
using UnityEngine;

// Token: 0x020001C7 RID: 455
public class Checkbox
{
	// Token: 0x06000E79 RID: 3705 RVA: 0x0000B94A File Offset: 0x00009B4A
	public Checkbox(string label, Rect labelRect, Rect checkboxRect, GUISkin skin, float fontScale) : this(label, label, labelRect, checkboxRect, skin, fontScale)
	{
	}

	// Token: 0x06000E7A RID: 3706 RVA: 0x0006194C File Offset: 0x0005FB4C
	public Checkbox(string label, string labelDisabled, Rect labelRect, Rect checkboxRect, GUISkin skin, float fontScale)
	{
		this.label = label;
		this.labelDisabled = labelDisabled;
		this.labelRect = labelRect;
		this.checkboxRect = checkboxRect;
		this.skin = skin;
		this.fontScale = fontScale;
		this.emptyButtonStyle = new GUIStyle(skin.button);
		GUIStyleState active = this.emptyButtonStyle.active;
		Texture2D texture2D = null;
		this.emptyButtonStyle.hover.background = texture2D;
		texture2D = texture2D;
		this.emptyButtonStyle.normal.background = texture2D;
		active.background = texture2D;
		this.SetTextures(ResourceManager.LoadTexture("Arena/scroll_browser_button_cb_checked"), ResourceManager.LoadTexture("Arena/scroll_browser_button_cb"));
	}

	// Token: 0x06000E7B RID: 3707 RVA: 0x0000B95A File Offset: 0x00009B5A
	public Checkbox(Rect checkboxRect, GUISkin skin) : this(null, new Rect(0f, 0f, 1f, 1f), checkboxRect, skin, 1f)
	{
		this.hasLabel = false;
	}

	// Token: 0x06000E7C RID: 3708 RVA: 0x0000B98A File Offset: 0x00009B8A
	public Checkbox SetTextures(Texture enabled, Texture disabled)
	{
		this.texEnabled = enabled;
		this.texDisabled = disabled;
		return this;
	}

	// Token: 0x06000E7D RID: 3709 RVA: 0x0000B99B File Offset: 0x00009B9B
	public Checkbox SetAlignment(TextAnchor alignment)
	{
		this.alignment = new TextAnchor?(alignment);
		return this;
	}

	// Token: 0x06000E7E RID: 3710 RVA: 0x0000B9AA File Offset: 0x00009BAA
	private string labelText(bool state)
	{
		return (!state) ? this.labelDisabled : this.label;
	}

	// Token: 0x06000E7F RID: 3711 RVA: 0x000619F8 File Offset: 0x0005FBF8
	public bool Draw(SvBool state)
	{
		bool value = state.value;
		bool result = this.Draw(ref value);
		state.value = value;
		return result;
	}

	// Token: 0x06000E80 RID: 3712 RVA: 0x00061A20 File Offset: 0x0005FC20
	public bool Draw(ref bool state)
	{
		GUISkin guiskin = GUI.skin;
		GUI.skin = this.skin;
		bool result = false;
		if (this.hasLabel)
		{
			TextAnchor textAnchor = GUI.skin.label.alignment;
			int fontSize = GUI.skin.label.fontSize;
			GUI.skin.label.alignment = ((this.alignment == null) ? 3 : this.alignment.Value);
			GUI.skin.label.fontSize = (int)((float)(Screen.height / 40) * this.fontScale);
			if (GUI.Button(this.checkboxRect, string.Empty) || GUI.Button(this.labelRect, string.Empty, this.emptyButtonStyle))
			{
				App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
				state = !state;
				result = true;
			}
			GUI.Label(this.labelRect, this.labelText(state));
			GUI.skin.label.fontSize = fontSize;
			GUI.skin.label.alignment = textAnchor;
		}
		else if (GUI.Button(this.checkboxRect, string.Empty))
		{
			App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
			state = !state;
			result = true;
		}
		if (state)
		{
			if (this.texEnabled != null)
			{
				GUI.DrawTexture(this.checkboxRect, this.texEnabled);
			}
		}
		else if (this.texDisabled != null)
		{
			GUI.DrawTexture(this.checkboxRect, this.texDisabled);
		}
		GUI.skin = guiskin;
		return result;
	}

	// Token: 0x04000B22 RID: 2850
	private string label;

	// Token: 0x04000B23 RID: 2851
	private string labelDisabled;

	// Token: 0x04000B24 RID: 2852
	private Rect labelRect;

	// Token: 0x04000B25 RID: 2853
	private Rect checkboxRect;

	// Token: 0x04000B26 RID: 2854
	private GUISkin skin;

	// Token: 0x04000B27 RID: 2855
	private float fontScale;

	// Token: 0x04000B28 RID: 2856
	private GUIStyle emptyButtonStyle;

	// Token: 0x04000B29 RID: 2857
	private Texture texEnabled;

	// Token: 0x04000B2A RID: 2858
	private Texture texDisabled;

	// Token: 0x04000B2B RID: 2859
	private bool hasLabel = true;

	// Token: 0x04000B2C RID: 2860
	private TextAnchor? alignment;
}
