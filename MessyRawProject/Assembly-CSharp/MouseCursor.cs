using System;
using UnityEngine;

// Token: 0x02000379 RID: 889
public class MouseCursor : MonoBehaviour
{
	// Token: 0x060013DB RID: 5083 RVA: 0x0000EB45 File Offset: 0x0000CD45
	private void Start()
	{
		this.cursorNormal = ResourceManager.LoadTexture("Cursor/cursor");
		this.cursorResizeVert = ResourceManager.LoadTexture("Cursor/cursor_adjust_vertical");
	}

	// Token: 0x060013DC RID: 5084 RVA: 0x00079028 File Offset: 0x00077228
	public void SetCursor(MouseCursor.CursorType newCursor)
	{
		if (this.cursor != newCursor)
		{
			Vector2 zero = Vector2.zero;
			Texture2D texture2D;
			if (newCursor == MouseCursor.CursorType.NORMAL || newCursor != MouseCursor.CursorType.RESIZE_VERTICAL)
			{
				zero..ctor(10f, 7f);
				texture2D = this.cursorNormal;
			}
			else
			{
				zero..ctor(16f, 16f);
				texture2D = this.cursorResizeVert;
			}
			Cursor.SetCursor(texture2D, zero, 0);
		}
		this.cursor = newCursor;
	}

	// Token: 0x04001115 RID: 4373
	private MouseCursor.CursorType cursor = MouseCursor.CursorType.NORMAL;

	// Token: 0x04001116 RID: 4374
	private Texture2D cursorNormal;

	// Token: 0x04001117 RID: 4375
	private Texture2D cursorResizeVert;

	// Token: 0x0200037A RID: 890
	public enum CursorType
	{
		// Token: 0x04001119 RID: 4377
		UNDEFINED,
		// Token: 0x0400111A RID: 4378
		NORMAL,
		// Token: 0x0400111B RID: 4379
		RESIZE_VERTICAL
	}
}
