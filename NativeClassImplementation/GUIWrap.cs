using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001D8 RID: 472
public class GUIWrap
{
	// Token: 0x06000EC6 RID: 3782 RVA: 0x0000BD5E File Offset: 0x00009F5E
	public GUIWrap()
	{
		this.clearStyle = new GUIStyle();
	}

	// Token: 0x06000EC8 RID: 3784 RVA: 0x000636FC File Offset: 0x000618FC
	public void BeginGroup(Rect rect)
	{
		GUI.BeginGroup(rect);
		Vector2 vector;
		vector..ctor(rect.x, rect.y);
		this.groupOffsets.Add(vector);
		this.groupOffset += vector;
	}

	// Token: 0x06000EC9 RID: 3785 RVA: 0x00063744 File Offset: 0x00061944
	public void EndGroup()
	{
		GUI.EndGroup();
		Vector2 vector = this.groupOffsets[this.groupOffsets.Count - 1];
		this.groupOffsets.RemoveAt(this.groupOffsets.Count - 1);
		this.groupOffset -= vector;
	}

	// Token: 0x06000ECA RID: 3786 RVA: 0x0000BDB2 File Offset: 0x00009FB2
	public void BeginResettedGroup()
	{
		this.BeginGroup(GeomUtil.getTranslated(GUIUtil.screen(), -this.groupOffset.x, -this.groupOffset.y));
	}

	// Token: 0x06000ECB RID: 3787 RVA: 0x0000BDDC File Offset: 0x00009FDC
	public Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect)
	{
		return GUI.BeginScrollView(position, scrollPosition, viewRect);
	}

	// Token: 0x06000ECC RID: 3788 RVA: 0x0000BDE6 File Offset: 0x00009FE6
	public void EndScrollView()
	{
		GUI.EndScrollView();
	}

	// Token: 0x06000ECD RID: 3789 RVA: 0x0000BDED File Offset: 0x00009FED
	public void Blank(Rect rect, GUIContent c)
	{
		this.drawOverlay(rect, c);
	}

	// Token: 0x06000ECE RID: 3790 RVA: 0x0000BDF7 File Offset: 0x00009FF7
	public void Box(Rect rect, GUIContent c)
	{
		this.Box(rect, c, GUI.skin.box);
	}

	// Token: 0x06000ECF RID: 3791 RVA: 0x0000BE0B File Offset: 0x0000A00B
	public void Box(Rect rect, GUIContent c, GUIStyle style)
	{
		GUI.Box(rect, c, style);
		this.Blank(rect, c);
	}

	// Token: 0x06000ED0 RID: 3792 RVA: 0x0000BE1D File Offset: 0x0000A01D
	public bool Blocker(Rect rect)
	{
		return GUI.Button(rect, string.Empty, this.clearStyle);
	}

	// Token: 0x06000ED1 RID: 3793 RVA: 0x0000BE30 File Offset: 0x0000A030
	public void DrawTexture(Rect rect, GUIContent c)
	{
		GUI.DrawTexture(rect, c.image);
		this.Blank(rect, c);
	}

	// Token: 0x06000ED2 RID: 3794 RVA: 0x0000BE46 File Offset: 0x0000A046
	public void Label(Rect rect, string s)
	{
		this._labelTempContent.text = s;
		this.Label(rect, this._labelTempContent);
	}

	// Token: 0x06000ED3 RID: 3795 RVA: 0x0000BE61 File Offset: 0x0000A061
	public void Label(Rect rect, GUIContent c)
	{
		this.Label(rect, c, GUI.skin.label);
	}

	// Token: 0x06000ED4 RID: 3796 RVA: 0x0000BE75 File Offset: 0x0000A075
	public void Label(Rect rect, string s, GUIStyle style)
	{
		this._labelTempContent.text = s;
		this.Label(rect, this._labelTempContent, style);
	}

	// Token: 0x06000ED5 RID: 3797 RVA: 0x0006379C File Offset: 0x0006199C
	public void Label(Rect rect, GUIContent c, GUIStyle style)
	{
		bool enabled = GUI.enabled;
		bool flag = GUIWrap.isLocked(c);
		GUI.enabled = !flag;
		GUI.Label(rect, c.clearedTagsCopy(), style);
		this.drawLock(rect, c, flag);
		GUI.enabled = enabled;
	}

	// Token: 0x06000ED6 RID: 3798 RVA: 0x0000BE91 File Offset: 0x0000A091
	public bool Button(Rect rect, string text)
	{
		this._buttonTempContent.text = text;
		return this.Button(rect, this._buttonTempContent);
	}

	// Token: 0x06000ED7 RID: 3799 RVA: 0x0000BEAC File Offset: 0x0000A0AC
	public bool Button(Rect rect, GUIContent c)
	{
		return this.Button(rect, c, GUI.skin.button);
	}

	// Token: 0x06000ED8 RID: 3800 RVA: 0x0000BEC0 File Offset: 0x0000A0C0
	public bool Button(Rect rect, string s, GUIStyle style)
	{
		this._buttonTempContent.text = s;
		return this.Button(rect, this._buttonTempContent, style);
	}

	// Token: 0x06000ED9 RID: 3801 RVA: 0x000637DC File Offset: 0x000619DC
	public bool Button(Rect rect, GUIContent c, GUIStyle style)
	{
		bool enabled = GUI.enabled;
		bool flag = GUIWrap.isLocked(c);
		Color color = GUI.color;
		if (flag)
		{
			GUI.color = color * 0.7f;
		}
		bool flag2 = GUI.Button(rect, c.clearedTagsCopy(), style);
		if (flag2)
		{
			if (flag)
			{
				if (c.hasTag(GUITags.Premium))
				{
					App.Popups.ShowDemoOk();
				}
				flag2 = false;
			}
			else
			{
				App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
			}
		}
		this.Blank(rect, c);
		GUI.enabled = enabled;
		GUI.color = color;
		return flag2;
	}

	// Token: 0x06000EDA RID: 3802 RVA: 0x0000BEDC File Offset: 0x0000A0DC
	private void drawOverlay(Rect rect, GUIContent c)
	{
		this.drawLock(rect, c, GUIWrap.isLocked(c));
		if (c.hasTag(GUITags.HelpArrow))
		{
			this.drawArrow(rect, c);
		}
	}

	// Token: 0x06000EDB RID: 3803 RVA: 0x0000BF04 File Offset: 0x0000A104
	public void clearTooltip()
	{
		this._currentTooltip = null;
	}

	// Token: 0x06000EDC RID: 3804 RVA: 0x00063874 File Offset: 0x00061A74
	public void drawTooltip()
	{
		if (this._currentTooltip == null)
		{
			return;
		}
		Vector2 screenMousePos = GUIUtil.getScreenMousePos();
		App.Tooltip.set(this._currentTooltip);
	}

	// Token: 0x06000EDD RID: 3805 RVA: 0x0000BF0D File Offset: 0x0000A10D
	private static bool isLocked(GUIContent c)
	{
		return c.hasTag(GUITags.AlwaysLock) || App.IsLocked(c.hasTag(GUITags.Premium));
	}

	// Token: 0x06000EDE RID: 3806 RVA: 0x000638A4 File Offset: 0x00061AA4
	public void drawArrow(Rect rect, GUIContent c)
	{
		Matrix4x4 matrix = GUI.matrix;
		rect = GUIWrap.screenSpaceRect(rect);
		this.BeginResettedGroup();
		Vector2 center = rect.center;
		Vector2 center2 = GUIUtil.screen().center;
		Vector2 vector = center - center2;
		float num = 57.29578f * Mathf.Atan2(vector.y, vector.x);
		Vector2 normalized = vector.normalized;
		Vector2 vector2 = center - normalized * ((float)Screen.height * (0.05f + 0.01f * Mathf.Sin(5f * Time.time)));
		GUIUtility.RotateAroundPivot(num, vector2);
		float num2 = (float)Screen.height * 0.05f;
		rect = GeomUtil.centerAt(rect, vector2);
		rect = GeomUtil.resizeCentered(rect, GeomUtil.getWidthFromHeight(num2, 500f, 368f), num2);
		GUI.DrawTexture(rect, ResourceManager.LoadTexture("Tutorial/Tutorial_arrow_noglow"));
		GUI.matrix = matrix;
		this.EndGroup();
	}

	// Token: 0x06000EDF RID: 3807 RVA: 0x00063994 File Offset: 0x00061B94
	public void drawLock(Rect rect, GUIContent c, bool isLocked)
	{
		if (!isLocked)
		{
			return;
		}
		bool enabled = GUI.enabled;
		GUI.enabled = true;
		this.drawLock(rect, c);
		GUI.enabled = enabled;
	}

	// Token: 0x06000EE0 RID: 3808 RVA: 0x000639C4 File Offset: 0x00061BC4
	public void drawLock(Rect rect, GUIContent c)
	{
		float num = Math.Min((float)Screen.height * 0.05f, rect.height);
		Rect rect2;
		rect2..ctor(rect.xMax - num, rect.y, num, num);
		if (c.hasTag(GUITags.Center))
		{
			rect2 = GeomUtil.resizeCentered(rect, rect.height);
		}
		GUI.DrawTexture(rect2, ResourceManager.LoadTexture("Store/lock_outline"));
		if (GUIWrap.screenSpaceRect(rect).Contains(GUIUtil.getScreenMousePos()))
		{
			if (c.hasTag(GUITags.Premium))
			{
				this._currentTooltip = GUIWrap._demoTooltip;
			}
			else if (c.hasTag(GUITags.Tutorial))
			{
				this._currentTooltip = GUIWrap._tutorialTooltip;
			}
			else
			{
				Log.warning("Unknown GUI Lock tag: " + c.tooltip);
			}
			if (c.hasTag(GUITags.Tooltip) || (c.hasTag(GUITags.TooltipIfNoPopup) && !App.Popups.IsShowingPopup() && !App.ChatUI.IsHovered(rect.center)))
			{
				this.drawTooltip();
			}
		}
	}

	// Token: 0x06000EE1 RID: 3809 RVA: 0x00063AEC File Offset: 0x00061CEC
	private static Rect screenSpaceRect(Rect rect)
	{
		Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(rect.x, rect.y));
		return new Rect(vector.x, vector.y, rect.width, rect.height);
	}

	// Token: 0x04000B71 RID: 2929
	private static GUIContent _demoTooltip = new GUIContent("You need to purchase the full game to use this feature.");

	// Token: 0x04000B72 RID: 2930
	private static GUIContent _tutorialTooltip = new GUIContent("You need to complete the previous tutorials first.");

	// Token: 0x04000B73 RID: 2931
	private GUIStyle clearStyle;

	// Token: 0x04000B74 RID: 2932
	private List<Vector2> groupOffsets = new List<Vector2>();

	// Token: 0x04000B75 RID: 2933
	private Vector2 groupOffset;

	// Token: 0x04000B76 RID: 2934
	private GUIContent _labelTempContent = new GUIContent();

	// Token: 0x04000B77 RID: 2935
	private GUIContent _buttonTempContent = new GUIContent();

	// Token: 0x04000B78 RID: 2936
	private GUIContent _currentTooltip;
}
