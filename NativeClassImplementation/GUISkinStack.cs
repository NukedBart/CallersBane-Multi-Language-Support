using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200043C RID: 1084
public class GUISkinStack
{
	// Token: 0x060017FC RID: 6140 RVA: 0x00011395 File Offset: 0x0000F595
	public GUISkinStack push()
	{
		this.stack.Push(GUI.skin);
		if (this.first == null)
		{
			this.first = this.stack.Peek();
		}
		return this;
	}

	// Token: 0x060017FD RID: 6141 RVA: 0x000113CA File Offset: 0x0000F5CA
	public GUISkinStack push(GUISkin s)
	{
		this.push();
		GUI.skin = s;
		return this;
	}

	// Token: 0x060017FE RID: 6142 RVA: 0x00091980 File Offset: 0x0008FB80
	public GUISkin pop()
	{
		GUISkin guiskin = this.stack.Pop();
		GUI.skin = guiskin;
		return guiskin;
	}

	// Token: 0x060017FF RID: 6143 RVA: 0x000113DA File Offset: 0x0000F5DA
	public void reset()
	{
		if (this.first != null)
		{
			GUI.skin = this.first;
		}
	}

	// Token: 0x0400150D RID: 5389
	public static GUISkinStack statics = new GUISkinStack();

	// Token: 0x0400150E RID: 5390
	private Stack<GUISkin> stack = new Stack<GUISkin>();

	// Token: 0x0400150F RID: 5391
	private GUISkin first;
}
