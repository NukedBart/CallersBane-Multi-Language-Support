using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gui
{
	// Token: 0x020001C8 RID: 456
	public class ContextMenu<T>
	{
		// Token: 0x06000E81 RID: 3713 RVA: 0x0000B9C3 File Offset: 0x00009BC3
		public ContextMenu(T user, Rect rect)
		{
			this.param = user;
			this.rect = rect;
		}

		// Token: 0x06000E82 RID: 3714 RVA: 0x00061BC0 File Offset: 0x0005FDC0
		public ContextMenu(T user)
		{
			this.param = user;
			this.rect = default(Rect);
		}

		// Token: 0x06000E83 RID: 3715 RVA: 0x0000B9E4 File Offset: 0x00009BE4
		public void setRect(Rect rect)
		{
			this.rect = rect;
		}

		// Token: 0x06000E84 RID: 3716 RVA: 0x0000B9ED File Offset: 0x00009BED
		public int size()
		{
			return this.items.Count;
		}

		// Token: 0x06000E85 RID: 3717 RVA: 0x0000B9FA File Offset: 0x00009BFA
		public void add(string title, ContextMenu<T>.URCMCallback cb)
		{
			this.add(new GUIContent(title), cb);
		}

		// Token: 0x06000E86 RID: 3718 RVA: 0x0000BA09 File Offset: 0x00009C09
		public void add(GUIContent content, ContextMenu<T>.URCMCallback cb)
		{
			this.items.Add(new KeyValuePair<GUIContent, ContextMenu<T>.URCMCallback>(content, cb));
		}

		// Token: 0x06000E87 RID: 3719 RVA: 0x0000BA1D File Offset: 0x00009C1D
		public void OnGUI_First()
		{
			this.draw(true);
		}

		// Token: 0x06000E88 RID: 3720 RVA: 0x0000BA26 File Offset: 0x00009C26
		public void OnGUI_Last()
		{
			this.draw(false);
		}

		// Token: 0x06000E89 RID: 3721 RVA: 0x00061BF4 File Offset: 0x0005FDF4
		private void draw(bool buttonsEnabled)
		{
			GUI.Box(new Rect(this.rect.x - 5f, this.rect.y - 5f, this.rect.width + 10f, this.rect.height * (float)this.items.Count + 10f), string.Empty);
			for (int i = 0; i < this.items.Count; i++)
			{
				KeyValuePair<GUIContent, ContextMenu<T>.URCMCallback> keyValuePair = this.items[i];
				Rect rect;
				rect..ctor(this.rect.x, this.rect.y + (float)i * this.rect.height, this.rect.width, this.rect.height);
				Rect rect2;
				rect2..ctor(rect.x, rect.y, rect.width, rect.height);
				if (App.GUI.Button(rect2, new GUIContent(keyValuePair.Key)) && buttonsEnabled)
				{
					keyValuePair.Value(this.param);
					App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
				}
			}
		}

		// Token: 0x06000E8A RID: 3722 RVA: 0x00061D34 File Offset: 0x0005FF34
		public bool containsMouse()
		{
			Rect rect;
			rect..ctor(this.rect.x, this.rect.y, this.rect.width, this.rect.height * (float)this.items.Count);
			return rect.Contains(GUIUtil.getScreenMousePos());
		}

		// Token: 0x04000B2D RID: 2861
		private T param;

		// Token: 0x04000B2E RID: 2862
		private Rect rect;

		// Token: 0x04000B2F RID: 2863
		private List<KeyValuePair<GUIContent, ContextMenu<T>.URCMCallback>> items = new List<KeyValuePair<GUIContent, ContextMenu<T>.URCMCallback>>();

		// Token: 0x020001C9 RID: 457
		[Flags]
		public enum Item
		{
			// Token: 0x04000B31 RID: 2865
			Challenge = 1,
			// Token: 0x04000B32 RID: 2866
			Trade = 2,
			// Token: 0x04000B33 RID: 2867
			Whisper = 4,
			// Token: 0x04000B34 RID: 2868
			Profile = 8,
			// Token: 0x04000B35 RID: 2869
			AddFriend = 16,
			// Token: 0x04000B36 RID: 2870
			Ignore = 32,
			// Token: 0x04000B37 RID: 2871
			Unignore = 64,
			// Token: 0x04000B38 RID: 2872
			CustomChallenge = 128
		}

		// Token: 0x020001CA RID: 458
		// (Invoke) Token: 0x06000E8C RID: 3724
		public delegate void URCMCallback(T user);
	}
}
