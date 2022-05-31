using System;
using System.Collections.Generic;
using Gui;
using UnityEngine;

// Token: 0x020001E2 RID: 482
public class ScrollsFrame
{
	// Token: 0x06000F22 RID: 3874 RVA: 0x0000C2E8 File Offset: 0x0000A4E8
	public ScrollsFrame(Rect rect) : this(rect, ScrollsFrame.Background.DARK)
	{
	}

	// Token: 0x06000F23 RID: 3875 RVA: 0x0000C2F2 File Offset: 0x0000A4F2
	public ScrollsFrame(Rect rect, ScrollsFrame.Background bg)
	{
		this.SetRect(rect);
		this.gui = UnityGui2D.getInstance();
		this.bgString = ((bg != ScrollsFrame.Background.DARK) ? "Store/checkout_paperbg" : "ChatUI/bg_texture");
	}

	// Token: 0x06000F24 RID: 3876 RVA: 0x00064BA8 File Offset: 0x00062DA8
	public ScrollsFrame() : this(default(Rect))
	{
	}

	// Token: 0x06000F25 RID: 3877 RVA: 0x00064BC4 File Offset: 0x00062DC4
	public ScrollsFrame AddNinePatch(ScrollsFrame.Border frame, NinePatch.Patches excludeFlags)
	{
		int num = 202;
		string filename = string.Empty;
		switch (frame)
		{
		case ScrollsFrame.Border.DARK_DOUBLE_CURVED:
			filename = "BattleUI/battlegui_border_rounded";
			num = 90;
			break;
		case ScrollsFrame.Border.DARK_DOUBLE_SHARP:
			filename = "BattleUI/battlegui_border_sharp";
			num = 90;
			break;
		case ScrollsFrame.Border.DARK_CURVED:
			filename = "ChatUI/dark_curved_box";
			break;
		case ScrollsFrame.Border.LIGHT_CURVED:
			filename = "ChatUI/light_curved_box";
			break;
		case ScrollsFrame.Border.DARK_SHARP:
			filename = "ChatUI/dark_sharp_box";
			break;
		case ScrollsFrame.Border.LIGHT_SHARP:
			filename = "ChatUI/light_sharp_box";
			break;
		}
		this.patches.Add(new NinePatch(this.borderArea, ResourceManager.LoadTexture(filename), num, num, num, num, excludeFlags).SetGui(this.gui));
		return this;
	}

	// Token: 0x06000F26 RID: 3878 RVA: 0x0000C332 File Offset: 0x0000A532
	public ScrollsFrame AddNinePatchInclude(ScrollsFrame.Border frame, NinePatch.Patches includeFlags)
	{
		this.AddNinePatch(frame, NinePatch.Patches.NONE);
		this.patches[this.patches.Count - 1].IncludeFlags(includeFlags);
		return this;
	}

	// Token: 0x06000F27 RID: 3879 RVA: 0x0000C35D File Offset: 0x0000A55D
	public ScrollsFrame AddNinePatch(ScrollsFrame.Border frame, NinePatch.Patches excludeFlags, IGui gui)
	{
		this.AddNinePatch(frame, excludeFlags);
		this.patches[this.patches.Count - 1].SetGui(gui);
		return this;
	}

	// Token: 0x06000F28 RID: 3880 RVA: 0x0000C388 File Offset: 0x0000A588
	public Rect GetRect()
	{
		return this.borderArea;
	}

	// Token: 0x06000F29 RID: 3881 RVA: 0x00064C7C File Offset: 0x00062E7C
	public void SetRect(Rect rect)
	{
		this.borderArea = rect;
		float num = (float)Screen.height * 0.015f;
		this.bgArea = new Rect(rect.x + num, rect.y + num, rect.width - num * 2f, rect.height - num * 2f);
		this.bgAreaTX = new Rect(0f, 0f, 6f * this.bgArea.width / (float)Screen.height, 6f * this.bgArea.height / (float)Screen.height);
		foreach (NinePatch ninePatch in this.patches)
		{
			ninePatch.SetRect(rect);
		}
	}

	// Token: 0x06000F2A RID: 3882 RVA: 0x0000C390 File Offset: 0x0000A590
	public ScrollsFrame SetGui(IGui gui)
	{
		this.gui = gui;
		return this;
	}

	// Token: 0x06000F2B RID: 3883 RVA: 0x0000C39A File Offset: 0x0000A59A
	public void Draw()
	{
		this.Draw(Color.white);
	}

	// Token: 0x06000F2C RID: 3884 RVA: 0x00064D6C File Offset: 0x00062F6C
	public void Draw(Color bgColor)
	{
		Color color = GUI.color;
		GUI.color = bgColor;
		this.gui.DrawTextureWithTexCoords(this.bgArea, ResourceManager.LoadTexture(this.bgString), this.bgAreaTX);
		GUI.color = color;
		foreach (NinePatch ninePatch in this.patches)
		{
			ninePatch.Draw();
		}
	}

	// Token: 0x04000BC1 RID: 3009
	private Rect borderArea;

	// Token: 0x04000BC2 RID: 3010
	private Rect bgArea;

	// Token: 0x04000BC3 RID: 3011
	private Rect bgAreaTX;

	// Token: 0x04000BC4 RID: 3012
	private NinePatch patch;

	// Token: 0x04000BC5 RID: 3013
	private IGui gui;

	// Token: 0x04000BC6 RID: 3014
	private List<NinePatch> patches = new List<NinePatch>();

	// Token: 0x04000BC7 RID: 3015
	private string bgString;

	// Token: 0x020001E3 RID: 483
	public enum Border
	{
		// Token: 0x04000BC9 RID: 3017
		DARK_DOUBLE_CURVED,
		// Token: 0x04000BCA RID: 3018
		DARK_DOUBLE_SHARP,
		// Token: 0x04000BCB RID: 3019
		DARK_CURVED,
		// Token: 0x04000BCC RID: 3020
		LIGHT_CURVED,
		// Token: 0x04000BCD RID: 3021
		DARK_SHARP,
		// Token: 0x04000BCE RID: 3022
		LIGHT_SHARP
	}

	// Token: 0x020001E4 RID: 484
	public enum Background
	{
		// Token: 0x04000BD0 RID: 3024
		DARK,
		// Token: 0x04000BD1 RID: 3025
		LIGHT
	}
}
