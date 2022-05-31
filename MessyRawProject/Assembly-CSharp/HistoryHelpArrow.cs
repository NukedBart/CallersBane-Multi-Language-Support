using System;
using UnityEngine;

// Token: 0x0200006B RID: 107
public class HistoryHelpArrow : HelpArrow
{
	// Token: 0x0600043C RID: 1084 RVA: 0x00031154 File Offset: 0x0002F354
	protected override void setupPosition(Vector2 p)
	{
		p.y -= 0.15f * (float)Screen.height;
		this.gui.DrawObject(p.x, p.y, this.pointer);
		this.pointer.transform.localEulerAngles = new Vector3(0f, 270f, 90f);
	}
}
