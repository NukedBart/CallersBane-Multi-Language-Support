using System;
using UnityEngine;

namespace Gui
{
	// Token: 0x020001E7 RID: 487
	public class UnityGui2D : IGui
	{
		// Token: 0x06000F40 RID: 3904 RVA: 0x0000C47E File Offset: 0x0000A67E
		public void DrawTexture(Rect dst, Texture tex)
		{
			GUI.DrawTexture(dst, tex);
		}

		// Token: 0x06000F41 RID: 3905 RVA: 0x0000C487 File Offset: 0x0000A687
		public void DrawTextureWithTexCoords(Rect dst, Texture tex, Rect texCoords)
		{
			GUI.DrawTextureWithTexCoords(dst, tex, texCoords);
		}

		// Token: 0x06000F42 RID: 3906 RVA: 0x0000C491 File Offset: 0x0000A691
		public static IGui getInstance()
		{
			if (UnityGui2D._instance == null)
			{
				UnityGui2D._instance = new UnityGui2D();
			}
			return UnityGui2D._instance;
		}

		// Token: 0x06000F43 RID: 3907 RVA: 0x0000C4AC File Offset: 0x0000A6AC
		public void SetColor(Color color)
		{
			GUI.color = color;
		}

		// Token: 0x06000F44 RID: 3908 RVA: 0x0000C4B4 File Offset: 0x0000A6B4
		public Color GetColor()
		{
			return GUI.color;
		}

		// Token: 0x04000BDE RID: 3038
		private static IGui _instance;
	}
}
