using System;
using Gui;
using UnityEngine;

// Token: 0x020001E0 RID: 480
public class NinePatch
{
	// Token: 0x06000F1A RID: 3866 RVA: 0x00064428 File Offset: 0x00062628
	public NinePatch(Rect screenRect, Texture2D image, int borderTop, int borderRight, int borderBottom, int borderLeft, NinePatch.Patches excludePatchFlags)
	{
		this.screenRect = screenRect;
		this.image = image;
		this.borderTop = borderTop;
		this.borderRight = borderRight;
		this.borderBottom = borderBottom;
		this.borderLeft = borderLeft;
		this.excludePatchFlags = excludePatchFlags;
		this.gui = UnityGui2D.getInstance();
		this.SetRect(screenRect);
	}

	// Token: 0x06000F1B RID: 3867 RVA: 0x00064498 File Offset: 0x00062698
	public NinePatch(Texture2D image, int borderTop, int borderRight, int borderBottom, int borderLeft, NinePatch.Patches excludePatchFlags) : this(default(Rect), image, borderTop, borderRight, borderBottom, borderLeft, excludePatchFlags)
	{
	}

	// Token: 0x06000F1C RID: 3868 RVA: 0x0000C2B2 File Offset: 0x0000A4B2
	public void SetRect(Rect screenRect)
	{
		this.screenRect = screenRect;
		this.UpdateRects();
	}

	// Token: 0x06000F1D RID: 3869 RVA: 0x0000C2C1 File Offset: 0x0000A4C1
	public NinePatch IncludeFlags(NinePatch.Patches include)
	{
		this.excludePatchFlags = (NinePatch.Patches.TOP_LEFT | NinePatch.Patches.TOP | NinePatch.Patches.TOP_RIGHT | NinePatch.Patches.LEFT | NinePatch.Patches.CENTER | NinePatch.Patches.RIGHT | NinePatch.Patches.BOTTOM_LEFT | NinePatch.Patches.BOTTOM | NinePatch.Patches.BOTTOM_RIGHT);
		this.excludePatchFlags &= ~include;
		return this;
	}

	// Token: 0x06000F1E RID: 3870 RVA: 0x0000C2DE File Offset: 0x0000A4DE
	public NinePatch SetGui(IGui gui)
	{
		this.gui = gui;
		return this;
	}

	// Token: 0x06000F1F RID: 3871 RVA: 0x000644C0 File Offset: 0x000626C0
	public void Draw()
	{
		if ((this.excludePatchFlags & NinePatch.Patches.TOP_LEFT) == NinePatch.Patches.NONE)
		{
			this.gui.DrawTextureWithTexCoords(this.topLeft, this.image, this.topLeftTX);
		}
		if ((this.excludePatchFlags & NinePatch.Patches.TOP) == NinePatch.Patches.NONE)
		{
			this.gui.DrawTextureWithTexCoords(this.top, this.image, this.topTX);
		}
		if ((this.excludePatchFlags & NinePatch.Patches.TOP_RIGHT) == NinePatch.Patches.NONE)
		{
			this.gui.DrawTextureWithTexCoords(this.topRight, this.image, this.topRightTX);
		}
		if ((this.excludePatchFlags & NinePatch.Patches.LEFT) == NinePatch.Patches.NONE)
		{
			this.gui.DrawTextureWithTexCoords(this.left, this.image, this.leftTX);
		}
		if ((this.excludePatchFlags & NinePatch.Patches.CENTER) == NinePatch.Patches.NONE)
		{
			this.gui.DrawTextureWithTexCoords(this.mid, this.image, this.midTX);
		}
		if ((this.excludePatchFlags & NinePatch.Patches.RIGHT) == NinePatch.Patches.NONE)
		{
			this.gui.DrawTextureWithTexCoords(this.right, this.image, this.rightTX);
		}
		if ((this.excludePatchFlags & NinePatch.Patches.BOTTOM_LEFT) == NinePatch.Patches.NONE)
		{
			this.gui.DrawTextureWithTexCoords(this.bottomLeft, this.image, this.bottomLeftTX);
		}
		if ((this.excludePatchFlags & NinePatch.Patches.BOTTOM) == NinePatch.Patches.NONE)
		{
			this.gui.DrawTextureWithTexCoords(this.bottom, this.image, this.bottomTX);
		}
		if ((this.excludePatchFlags & NinePatch.Patches.BOTTOM_RIGHT) == NinePatch.Patches.NONE)
		{
			this.gui.DrawTextureWithTexCoords(this.bottomRight, this.image, this.bottomRightTX);
		}
	}

	// Token: 0x06000F20 RID: 3872 RVA: 0x00064654 File Offset: 0x00062854
	private void UpdateRects()
	{
		float num = this.screenRect.width * 0.5f;
		float num2 = this.screenRect.height * 0.5f;
		float width = Math.Min((float)this.borderLeft, num);
		float width2 = Math.Min((float)this.borderRight, num);
		float height = Math.Min((float)this.borderTop, num2);
		float height2 = Math.Min((float)this.borderBottom, num2);
		this.topLeft = this.mockupCalc.prAspectH(Vector2.zero, width, height);
		this.topRight = this.mockupCalc.prAspectH(Vector2.zero, width2, height);
		this.bottomLeft = this.mockupCalc.prAspectH(Vector2.zero, width, height2);
		this.bottomRight = this.mockupCalc.prAspectH(Vector2.zero, width2, height2);
		this.topLeft.x = this.screenRect.x;
		this.topLeft.y = this.screenRect.y;
		this.topRight.x = this.screenRect.xMax - this.topRight.width;
		this.topRight.y = this.screenRect.y;
		this.bottomLeft.x = this.screenRect.x;
		this.bottomLeft.y = this.screenRect.yMax - this.bottomLeft.height;
		this.bottomRight.x = this.screenRect.xMax - this.bottomRight.width;
		this.bottomRight.y = this.screenRect.yMax - this.bottomRight.height;
		this.top = new Rect(this.topLeft.xMax, this.topLeft.y, this.screenRect.width - this.topLeft.width - this.topRight.width, this.topLeft.height);
		this.right = new Rect(this.topRight.x, this.topRight.yMax, this.topRight.width, this.screenRect.height - this.topRight.height - this.bottomRight.height);
		this.bottom = new Rect(this.topLeft.xMax, this.bottomLeft.y, this.screenRect.width - this.topLeft.width - this.topRight.width, this.bottomLeft.height);
		this.left = new Rect(this.topLeft.x, this.topLeft.yMax, this.topLeft.width, this.screenRect.height - this.topLeft.height - this.bottomLeft.height);
		this.mid = new Rect(this.topLeft.xMax, this.topLeft.yMax, this.screenRect.width - this.topLeft.width - this.topRight.width, this.screenRect.height - this.topLeft.height - this.bottomLeft.height);
		this.updateTexCoordRects(width, height, width2, height2);
	}

	// Token: 0x06000F21 RID: 3873 RVA: 0x000649B4 File Offset: 0x00062BB4
	private void updateTexCoordRects(float left, float top, float right, float bottom)
	{
		float num = left / (float)this.image.width;
		float num2 = top / (float)this.image.height;
		float num3 = right / (float)this.image.width;
		float num4 = bottom / (float)this.image.height;
		this.topLeftTX = new Rect(0f, 1f - num2, num, num2);
		this.topRightTX = new Rect(1f - num3, 1f - num2, num3, num2);
		this.bottomLeftTX = new Rect(0f, 0f, num, num4);
		this.bottomRightTX = new Rect(1f - num3, 0f, num3, num4);
		this.topTX = new Rect(this.topLeftTX.width, 1f - num2, 1f - this.topLeftTX.width - this.topRightTX.width, num2);
		this.rightTX = new Rect(1f - num3, this.topRightTX.height, num3, 1f - this.topRightTX.height - this.bottomRightTX.height);
		this.bottomTX = new Rect(this.topLeftTX.width, 0f, 1f - this.topLeftTX.width - this.topRightTX.width, num2);
		this.leftTX = new Rect(0f, this.topLeftTX.height, num, 1f - this.topLeftTX.height - this.bottomLeftTX.height);
		this.midTX = new Rect(this.topLeftTX.width, this.topLeftTX.height, 1f - this.topLeftTX.width - this.topRightTX.width, 1f - this.topLeftTX.width - this.bottomLeftTX.height);
	}

	// Token: 0x04000B9B RID: 2971
	private Texture2D image;

	// Token: 0x04000B9C RID: 2972
	private int borderTop;

	// Token: 0x04000B9D RID: 2973
	private int borderRight;

	// Token: 0x04000B9E RID: 2974
	private int borderBottom;

	// Token: 0x04000B9F RID: 2975
	private int borderLeft;

	// Token: 0x04000BA0 RID: 2976
	private Rect screenRect;

	// Token: 0x04000BA1 RID: 2977
	private MockupCalc mockupCalc = new MockupCalc(2048, 1536);

	// Token: 0x04000BA2 RID: 2978
	private Rect topLeft;

	// Token: 0x04000BA3 RID: 2979
	private Rect top;

	// Token: 0x04000BA4 RID: 2980
	private Rect topRight;

	// Token: 0x04000BA5 RID: 2981
	private Rect left;

	// Token: 0x04000BA6 RID: 2982
	private Rect mid;

	// Token: 0x04000BA7 RID: 2983
	private Rect right;

	// Token: 0x04000BA8 RID: 2984
	private Rect bottomLeft;

	// Token: 0x04000BA9 RID: 2985
	private Rect bottom;

	// Token: 0x04000BAA RID: 2986
	private Rect bottomRight;

	// Token: 0x04000BAB RID: 2987
	private Rect topLeftTX;

	// Token: 0x04000BAC RID: 2988
	private Rect topTX;

	// Token: 0x04000BAD RID: 2989
	private Rect topRightTX;

	// Token: 0x04000BAE RID: 2990
	private Rect leftTX;

	// Token: 0x04000BAF RID: 2991
	private Rect midTX;

	// Token: 0x04000BB0 RID: 2992
	private Rect rightTX;

	// Token: 0x04000BB1 RID: 2993
	private Rect bottomLeftTX;

	// Token: 0x04000BB2 RID: 2994
	private Rect bottomTX;

	// Token: 0x04000BB3 RID: 2995
	private Rect bottomRightTX;

	// Token: 0x04000BB4 RID: 2996
	private IGui gui;

	// Token: 0x04000BB5 RID: 2997
	private NinePatch.Patches excludePatchFlags;

	// Token: 0x020001E1 RID: 481
	[Flags]
	public enum Patches
	{
		// Token: 0x04000BB7 RID: 2999
		NONE = 0,
		// Token: 0x04000BB8 RID: 3000
		TOP_LEFT = 1,
		// Token: 0x04000BB9 RID: 3001
		TOP = 2,
		// Token: 0x04000BBA RID: 3002
		TOP_RIGHT = 4,
		// Token: 0x04000BBB RID: 3003
		LEFT = 8,
		// Token: 0x04000BBC RID: 3004
		CENTER = 16,
		// Token: 0x04000BBD RID: 3005
		RIGHT = 32,
		// Token: 0x04000BBE RID: 3006
		BOTTOM_LEFT = 64,
		// Token: 0x04000BBF RID: 3007
		BOTTOM = 128,
		// Token: 0x04000BC0 RID: 3008
		BOTTOM_RIGHT = 256
	}
}
