using System;
using UnityEngine;

// Token: 0x020001E6 RID: 486
public class Tooltip : MonoBehaviour
{
	// Token: 0x06000F35 RID: 3893 RVA: 0x00064F80 File Offset: 0x00063180
	public void Start()
	{
		GUISkin guiskin = (GUISkin)ResourceManager.Load("_GUISkins/Tooltip");
		this.style = new GUIStyle(guiskin.label);
		this.calcStyle = new GUIStyle(this.style);
		this.calcStyle.wordWrap = false;
	}

	// Token: 0x06000F36 RID: 3894 RVA: 0x0000C40A File Offset: 0x0000A60A
	public void Update()
	{
		this.frames--;
	}

	// Token: 0x06000F37 RID: 3895 RVA: 0x0000C41A File Offset: 0x0000A61A
	public void setText(string text)
	{
		this.setText(text, 0);
	}

	// Token: 0x06000F38 RID: 3896 RVA: 0x0000C424 File Offset: 0x0000A624
	public void setText(string text, int priority)
	{
		this.set(new GUIContent(text), priority);
	}

	// Token: 0x06000F39 RID: 3897 RVA: 0x0000C433 File Offset: 0x0000A633
	public void set(GUIContent content)
	{
		this.set(content, 0);
	}

	// Token: 0x06000F3A RID: 3898 RVA: 0x0000C43D File Offset: 0x0000A63D
	public void set(GUIContent content, int priority)
	{
		this.set(content, priority, true);
	}

	// Token: 0x06000F3B RID: 3899 RVA: 0x0000C448 File Offset: 0x0000A648
	private void setButHideIfPopupActive(GUIContent content, int priority)
	{
		this.set(content, priority, false);
	}

	// Token: 0x06000F3C RID: 3900 RVA: 0x0000C453 File Offset: 0x0000A653
	private void set(GUIContent content, int priority, bool showIfPopupActive)
	{
		if (priority < this.priority)
		{
			return;
		}
		this.content = content;
		this.priority = priority;
		this.frames = 2;
		this.showIfPopupActive = showIfPopupActive;
	}

	// Token: 0x06000F3D RID: 3901 RVA: 0x00064FCC File Offset: 0x000631CC
	public void OnGUI()
	{
		if (!this.showTooltip())
		{
			return;
		}
		if (!this.showIfPopupActive && App.Popups.IsShowingPopup())
		{
			return;
		}
		this.style.fontSize = Screen.height / 40;
		GUIContent guicontent = this.content;
		float num = (float)Screen.width * 0.005f;
		float num2 = Mathf.Min(this.calcStyle.CalcSize(guicontent).x * 1.5f, (float)Screen.width * 0.2f) + num;
		Vector2 vector;
		vector..ctor(num2, this.style.CalcHeight(guicontent, num2));
		GUI.Label(GUIUtil.getTooltipRect(vector.x, vector.y), guicontent, this.style);
	}

	// Token: 0x06000F3E RID: 3902 RVA: 0x0006508C File Offset: 0x0006328C
	private bool showTooltip()
	{
		return this.frames > 0 && this.content != null && (this.content.image != null || this.content.text != null);
	}

	// Token: 0x04000BD8 RID: 3032
	private int frames;

	// Token: 0x04000BD9 RID: 3033
	private int priority;

	// Token: 0x04000BDA RID: 3034
	private bool showIfPopupActive;

	// Token: 0x04000BDB RID: 3035
	private GUIContent content = new GUIContent("Tooltip!");

	// Token: 0x04000BDC RID: 3036
	private GUIStyle style;

	// Token: 0x04000BDD RID: 3037
	private GUIStyle calcStyle;
}
