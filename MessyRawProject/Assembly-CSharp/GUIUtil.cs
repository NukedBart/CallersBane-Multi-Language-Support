using System;
using UnityEngine;

// Token: 0x0200043D RID: 1085
public static class GUIUtil
{
	// Token: 0x06001800 RID: 6144 RVA: 0x000113F8 File Offset: 0x0000F5F8
	public static Rect screen()
	{
		return new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
	}

	// Token: 0x06001801 RID: 6145 RVA: 0x00011415 File Offset: 0x0000F615
	public static Rect screen(Rect unitRect)
	{
		return GeomUtil.cropShare(GUIUtil.screen(), unitRect);
	}

	// Token: 0x06001802 RID: 6146 RVA: 0x00011422 File Offset: 0x0000F622
	public static Rect centeredScreen(float unitWidth, float unitHeight)
	{
		return GUIUtil.screen(new Rect(0.5f * (1f - unitWidth), 0.5f * (1f - unitHeight), unitWidth, unitHeight));
	}

	// Token: 0x06001803 RID: 6147 RVA: 0x000919A0 File Offset: 0x0008FBA0
	public static bool isMouseOnScreen()
	{
		Vector2 screenMousePos = GUIUtil.getScreenMousePos();
		return screenMousePos.x >= 0f && screenMousePos.x < (float)Screen.width && screenMousePos.y >= 0f && screenMousePos.y < (float)Screen.height;
	}

	// Token: 0x06001804 RID: 6148 RVA: 0x000919FC File Offset: 0x0008FBFC
	public static Vector2 getScreenMousePos()
	{
		return GUIUtil.getScreenMousePos(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
	}

	// Token: 0x06001805 RID: 6149 RVA: 0x0001144A File Offset: 0x0000F64A
	public static Vector2 getScreenMousePos(Vector2 p)
	{
		return new Vector2(p.x, (float)Screen.height - p.y);
	}

	// Token: 0x06001806 RID: 6150 RVA: 0x00011466 File Offset: 0x0000F666
	public static Rect createUV01(float u0, float v0, float u1, float v1)
	{
		return GUIUtil.createUVwh(u0, v0, u1 - u0, v1 - v0);
	}

	// Token: 0x06001807 RID: 6151 RVA: 0x00011475 File Offset: 0x0000F675
	public static Rect createUVwh(float u0, float v0, float du, float dv)
	{
		return new Rect(u0, 1f - v0 - dv, du, dv);
	}

	// Token: 0x06001808 RID: 6152 RVA: 0x00091A30 File Offset: 0x0008FC30
	public static Matrix4x4 getVirtualResolutionMatrix(float width, float height)
	{
		Quaternion quaternion = Quaternion.AngleAxis(0f, new Vector3(0f, 1f, 0f));
		Vector3 vector;
		vector..ctor((float)Screen.height / (width * height / width), (float)Screen.height / height, 1f);
		return Matrix4x4.TRS(Vector3.zero, quaternion, vector);
	}

	// Token: 0x06001809 RID: 6153 RVA: 0x00011488 File Offset: 0x0000F688
	public static void setupVirtualResolution(float width, float height)
	{
		GUI.matrix = GUIUtil.getVirtualResolutionMatrix(width, height);
	}

	// Token: 0x0600180A RID: 6154 RVA: 0x00011496 File Offset: 0x0000F696
	public static void resetVirtualResolution()
	{
		GUIUtil.setupVirtualResolution((float)Screen.width, (float)Screen.height);
	}

	// Token: 0x0600180B RID: 6155 RVA: 0x000114A9 File Offset: 0x0000F6A9
	public static float HorizontalScrollbar(Rect position, float value, float size, float leftValue, float rightValue)
	{
		return GUI.HorizontalScrollbar(position, value, size, leftValue, rightValue + size);
	}

	// Token: 0x0600180C RID: 6156 RVA: 0x00091A88 File Offset: 0x0008FC88
	public static void setScissorScreenRect(Camera cam, Rect screenRect)
	{
		Rect r = GUIUtil.createUVwh(screenRect.x / (float)Screen.width, screenRect.y / (float)Screen.height, screenRect.width / (float)Screen.width, screenRect.height / (float)Screen.height);
		GUIUtil.setScissorRect(cam, r);
	}

	// Token: 0x0600180D RID: 6157 RVA: 0x00091ADC File Offset: 0x0008FCDC
	public static void setScissorRect(Camera cam, Rect r)
	{
		if (r.x < 0f)
		{
			r.width += r.x;
			r.x = 0f;
		}
		if (r.y < 0f)
		{
			r.height += r.y;
			r.y = 0f;
		}
		r.width = Mathf.Min(1f - r.x, r.width);
		r.height = Mathf.Min(1f - r.y, r.height);
		cam.rect = new Rect(0f, 0f, 1f, 1f);
		cam.ResetProjectionMatrix();
		Matrix4x4 projectionMatrix = cam.projectionMatrix;
		cam.rect = r;
		Matrix4x4 matrix4x = Matrix4x4.TRS(new Vector3(1f / r.width - 1f, 1f / r.height - 1f, 0f), Quaternion.identity, new Vector3(1f / r.width, 1f / r.height, 1f));
		Matrix4x4 matrix4x2 = Matrix4x4.TRS(new Vector3(-r.x * 2f / r.width, -r.y * 2f / r.height, 0f), Quaternion.identity, Vector3.one);
		cam.projectionMatrix = matrix4x2 * matrix4x * projectionMatrix;
	}

	// Token: 0x0600180E RID: 6158 RVA: 0x000114B8 File Offset: 0x0000F6B8
	public static string RtSize(string text, int size)
	{
		return string.Concat(new object[]
		{
			"<size=",
			size,
			">",
			text,
			"</size>"
		});
	}

	// Token: 0x0600180F RID: 6159 RVA: 0x000114EA File Offset: 0x0000F6EA
	public static string RtColor(string text, Color color)
	{
		return string.Concat(new string[]
		{
			"<color=",
			ColorUtil.ToHexString(color),
			">",
			text,
			"</color>"
		});
	}

	// Token: 0x06001810 RID: 6160 RVA: 0x0001151C File Offset: 0x0000F71C
	public static string RtSizeColor(string text, int size, Color color)
	{
		return GUIUtil.RtSize(GUIUtil.RtColor(text, color), size);
	}

	// Token: 0x06001811 RID: 6161 RVA: 0x00091C78 File Offset: 0x0008FE78
	public static void drawShadowText(Rect rect, string text, Color color, int shiftX, int shiftY)
	{
		Rect rect2;
		rect2..ctor(rect);
		Color textColor = GUI.skin.label.normal.textColor;
		GUI.skin.label.normal.textColor = Color.black;
		rect2.x += (float)shiftX;
		rect2.y += (float)shiftY;
		GUI.Label(rect2, text);
		GUI.skin.label.normal.textColor = color;
		rect2.x -= (float)shiftX;
		rect2.y -= (float)shiftY;
		GUI.Label(rect2, text);
		GUI.skin.label.normal.textColor = textColor;
	}

	// Token: 0x06001812 RID: 6162 RVA: 0x0001152B File Offset: 0x0000F72B
	public static void drawShadowText(Rect rect, string text, Color color)
	{
		GUIUtil.drawShadowText(rect, text, color, 2, 2);
	}

	// Token: 0x06001813 RID: 6163 RVA: 0x00091D38 File Offset: 0x0008FF38
	public static void drawShadowText(Rect rect, string text, Color color, int fontSize)
	{
		int fontSize2 = GUI.skin.label.fontSize;
		GUI.skin.label.fontSize = fontSize;
		GUIUtil.drawShadowText(rect, text, color);
		GUI.skin.label.fontSize = fontSize2;
	}

	// Token: 0x06001814 RID: 6164 RVA: 0x00091D80 File Offset: 0x0008FF80
	public static void drawBorderedText(Rect rect, string text, Color textColor)
	{
		Rect rect2;
		rect2..ctor(rect);
		Color textColor2 = GUI.skin.label.normal.textColor;
		GUI.skin.label.normal.textColor = Color.black;
		rect2.x = rect.x + 2f;
		GUI.Label(rect2, text);
		rect2.x = rect.x - 2f;
		GUI.Label(rect2, text);
		rect2.x = rect.x;
		rect2.y = rect.y + 2f;
		GUI.Label(rect2, text);
		rect2.y = rect.y - 2f;
		GUI.Label(rect2, text);
		GUI.skin.label.normal.textColor = textColor;
		GUI.Label(rect, text);
		GUI.skin.label.normal.textColor = textColor2;
	}

	// Token: 0x06001815 RID: 6165 RVA: 0x00091E70 File Offset: 0x00090070
	public static int getFittingFontSize(string name, GUIStyle style, float allowedWidth)
	{
		GUIStyle guistyle = new GUIStyle(style);
		for (float x = guistyle.CalcSize(new GUIContent(name)).x; x > allowedWidth; x = guistyle.CalcSize(new GUIContent(name)).x)
		{
			guistyle.fontSize--;
		}
		return guistyle.fontSize;
	}

	// Token: 0x06001816 RID: 6166 RVA: 0x00011537 File Offset: 0x0000F737
	public static Rect getTooltipRect(float width, float height)
	{
		return GUIUtil.getTooltipRect(width, height, GUIUtil.getScreenMousePos());
	}

	// Token: 0x06001817 RID: 6167 RVA: 0x00091ED0 File Offset: 0x000900D0
	public static Rect getTooltipRect(float width, float height, Vector2 p)
	{
		p.x = Mth.clamp(p.x, 0f, (float)Screen.width - width);
		p.y = Mth.clamp(p.y - height, 0f, (float)Screen.height - height);
		return new Rect(p.x, p.y, width, height);
	}

	// Token: 0x06001818 RID: 6168 RVA: 0x00011545 File Offset: 0x0000F745
	public static void drawToolTip(GUIContent tooltip)
	{
		GUIUtil.drawToolTip(tooltip, (float)Screen.width * 0.2f);
	}

	// Token: 0x06001819 RID: 6169 RVA: 0x00011559 File Offset: 0x0000F759
	public static void drawToolTip(GUIContent tooltip, float labelWidth)
	{
		GUIUtil.drawToolTip(tooltip, labelWidth, GUIUtil.getScreenMousePos());
	}

	// Token: 0x0600181A RID: 6170 RVA: 0x00091F34 File Offset: 0x00090134
	public static void drawToolTip(GUIContent tooltip, float labelWidth, Vector2 p)
	{
		GUISkin skin = GUI.skin;
		GUI.skin = (GUISkin)ResourceManager.Load("_GUISkins/Tooltip");
		int fontSize = GUI.skin.label.fontSize;
		GUI.skin.label.fontSize = Screen.height / 40;
		Vector2 vector;
		vector..ctor(labelWidth, GUI.skin.label.CalcHeight(tooltip, labelWidth));
		GUI.Label(GUIUtil.getTooltipRect(vector.x, vector.y, p), tooltip);
		GUI.skin.label.fontSize = fontSize;
		GUI.skin = skin;
	}

	// Token: 0x0600181B RID: 6171 RVA: 0x00091FCC File Offset: 0x000901CC
	public static GUIStyle createButtonStyle(string fn)
	{
		GUIStyle guistyle = new GUIStyle();
		guistyle.normal.background = ResourceManager.LoadTexture(fn);
		guistyle.hover.background = ResourceManager.LoadTexture(fn + "mo");
		guistyle.active.background = ResourceManager.LoadTexture(fn + "md");
		if (guistyle.active.background == null)
		{
			ResourceManager.LoadTexture(fn + "d");
		}
		return guistyle;
	}
}
