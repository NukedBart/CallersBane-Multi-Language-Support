using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gui
{
	// Token: 0x020001DD RID: 477
	public class LargeGuiImage
	{
		// Token: 0x06000F0D RID: 3853 RVA: 0x0000C201 File Offset: 0x0000A401
		private LargeGuiImage()
		{
			this.gui = UnityGui2D.getInstance();
		}

		// Token: 0x06000F0E RID: 3854 RVA: 0x0000C21F File Offset: 0x0000A41F
		public void setGui(IGui gui)
		{
			this.gui = gui;
		}

		// Token: 0x06000F0F RID: 3855 RVA: 0x00064048 File Offset: 0x00062248
		private void feed(Rect rect, Texture2D tex)
		{
			this.parts.Add(new LargeGuiImage.Part(rect, tex));
			if (rect.xMax > this.totalWidth)
			{
				this.totalWidth = rect.xMax;
			}
			if (rect.yMax > this.totalHeight)
			{
				this.totalHeight = rect.yMax;
			}
		}

		// Token: 0x06000F10 RID: 3856 RVA: 0x0000C228 File Offset: 0x0000A428
		public void setTextureWrapMode(TextureWrapMode mode)
		{
			this.wrapMode = mode;
		}

		// Token: 0x06000F11 RID: 3857 RVA: 0x000640A8 File Offset: 0x000622A8
		public void Draw(Rect dst, Rect src)
		{
			if (this.wrapMode != null)
			{
				this._draw(dst, src);
			}
			else
			{
				Rect src2;
				src2..ctor(src);
				src2.x = src.x % this.totalWidth;
				src2.y = src.y % this.totalHeight;
				this._draw(dst, src2);
				this._draw(dst, new Rect(src2.x - this.totalWidth, src2.y, src2.width, src2.height));
			}
		}

		// Token: 0x06000F12 RID: 3858 RVA: 0x00064138 File Offset: 0x00062338
		public void _draw(Rect dst, Rect src)
		{
			float num = dst.width / src.width;
			float num2 = dst.height / src.height;
			foreach (LargeGuiImage.Part part in this.parts)
			{
				Rect rect = part.rect;
				Rect cropped = GeomUtil.getCropped(rect, src);
				if (cropped.width > 0f && cropped.height > 0f)
				{
					Rect dst2;
					dst2..ctor((cropped.x - src.x) * num + dst.x, (cropped.y - src.y) * num2 + dst.y, num * cropped.width, num2 * cropped.height);
					float u = (cropped.x - rect.x) / rect.width;
					float v = (cropped.y - rect.y) / rect.height;
					Rect texCoords = GUIUtil.createUVwh(u, v, cropped.width / rect.width, cropped.height / rect.height);
					this.gui.DrawTextureWithTexCoords(dst2, part.tex, texCoords);
				}
			}
		}

		// Token: 0x06000F13 RID: 3859 RVA: 0x000642AC File Offset: 0x000624AC
		public static LargeGuiImage fromFilenamesAndCutSize(string fnfmt, int totalWidth, int totalHeight, int cutWidth, int cutHeight)
		{
			LargeGuiImage largeGuiImage = new LargeGuiImage();
			int i = 0;
			int num = 0;
			while (i < totalHeight)
			{
				int j = 0;
				int num2 = 0;
				while (j < totalWidth)
				{
					string text = fnfmt.Replace("{x}", num2.ToString()).Replace("{y}", num.ToString());
					Texture2D texture2D = ResourceManager.LoadTexture(text);
					if (texture2D == null)
					{
						Log.error("Couldn't find image part: " + text);
					}
					else
					{
						float num3 = (float)Mathf.Min(cutWidth, totalWidth - j);
						float num4 = (float)Mathf.Min(cutHeight, totalHeight - i);
						largeGuiImage.feed(new Rect((float)j, (float)i, num3, num4), texture2D);
					}
					j += cutWidth;
					num2++;
				}
				i += cutHeight;
				num++;
			}
			return largeGuiImage;
		}

		// Token: 0x04000B90 RID: 2960
		private List<LargeGuiImage.Part> parts = new List<LargeGuiImage.Part>();

		// Token: 0x04000B91 RID: 2961
		private float totalWidth;

		// Token: 0x04000B92 RID: 2962
		private float totalHeight;

		// Token: 0x04000B93 RID: 2963
		private IGui gui;

		// Token: 0x04000B94 RID: 2964
		private TextureWrapMode wrapMode;

		// Token: 0x020001DE RID: 478
		private class Part
		{
			// Token: 0x06000F14 RID: 3860 RVA: 0x0000C231 File Offset: 0x0000A431
			public Part(Rect rect, Texture2D tex)
			{
				this.rect = rect;
				this.tex = tex;
			}

			// Token: 0x04000B95 RID: 2965
			public Rect rect;

			// Token: 0x04000B96 RID: 2966
			public Texture2D tex;
		}
	}
}
