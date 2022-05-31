using System;
using UnityEngine;

// Token: 0x02000447 RID: 1095
public class LabelUtil
{
	// Token: 0x06001858 RID: 6232 RVA: 0x00092980 File Offset: 0x00090B80
	public static void HelpLabel(Rect r, string label, float labelSize, Color labelColor)
	{
		TextAnchor alignment = GUI.skin.label.alignment;
		int fontSize = GUI.skin.label.fontSize;
		GUI.skin.label.alignment = 3;
		GUI.skin.label.fontSize = 6 + Screen.height / 64;
		Color textColor = GUI.skin.label.normal.textColor;
		GUI.skin.label.normal.textColor = labelColor;
		GUI.Label(new Rect(r.x - labelSize, r.y, labelSize, r.height), label);
		GUI.skin.label.normal.textColor = textColor;
		GUI.skin.label.alignment = alignment;
		GUI.skin.label.fontSize = fontSize;
	}
}
