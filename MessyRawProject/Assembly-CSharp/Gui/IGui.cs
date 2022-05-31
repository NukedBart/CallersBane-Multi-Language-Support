using System;
using UnityEngine;

namespace Gui
{
	// Token: 0x020001DC RID: 476
	public interface IGui
	{
		// Token: 0x06000F09 RID: 3849
		void SetColor(Color color);

		// Token: 0x06000F0A RID: 3850
		Color GetColor();

		// Token: 0x06000F0B RID: 3851
		void DrawTexture(Rect dst, Texture tex);

		// Token: 0x06000F0C RID: 3852
		void DrawTextureWithTexCoords(Rect dst, Texture tex, Rect texCoords);
	}
}
