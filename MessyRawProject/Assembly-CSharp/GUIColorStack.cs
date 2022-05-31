using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200043B RID: 1083
public class GUIColorStack
{
	// Token: 0x060017F6 RID: 6134 RVA: 0x0001130A File Offset: 0x0000F50A
	public GUIColorStack push()
	{
		this.stack.Push(GUI.color);
		if (this.stack.Count == 1)
		{
			this.first = new Color?(this.stack.Peek());
		}
		return this;
	}

	// Token: 0x060017F7 RID: 6135 RVA: 0x00011344 File Offset: 0x0000F544
	public GUIColorStack push(Color s)
	{
		this.push();
		GUI.color = s;
		return this;
	}

	// Token: 0x060017F8 RID: 6136 RVA: 0x00091960 File Offset: 0x0008FB60
	public Color pop()
	{
		Color color = this.stack.Pop();
		GUI.color = color;
		return color;
	}

	// Token: 0x060017F9 RID: 6137 RVA: 0x00011354 File Offset: 0x0000F554
	public void reset()
	{
		if (this.first != null)
		{
			GUI.color = this.first.Value;
		}
	}

	// Token: 0x0400150A RID: 5386
	public static GUIColorStack statics = new GUIColorStack();

	// Token: 0x0400150B RID: 5387
	private Stack<Color> stack = new Stack<Color>();

	// Token: 0x0400150C RID: 5388
	private Color? first;
}
