using System;
using UnityEngine;

namespace NSCampaign
{
	// Token: 0x020000E6 RID: 230
	public class GraphicsField : MonoBehaviour
	{
		// Token: 0x060007AD RID: 1965 RVA: 0x00006D3F File Offset: 0x00004F3F
		public void init(int width, int height)
		{
			this.width = width;
			this.height = height;
			this.tiles = new GraphicsTile[width, height];
		}

		// Token: 0x060007AE RID: 1966 RVA: 0x00006D5C File Offset: 0x00004F5C
		public void destroy()
		{
			this.destroyVisualBoard();
			Object.Destroy(this);
		}

		// Token: 0x060007AF RID: 1967 RVA: 0x00043608 File Offset: 0x00041808
		private void destroyVisualBoard()
		{
			if (this.tiles == null)
			{
				return;
			}
			for (int i = 0; i < this.width; i++)
			{
				for (int j = 0; j < this.height; j++)
				{
					Object.Destroy(this.tiles[i, j].gameObject);
				}
			}
			this.tiles = null;
		}

		// Token: 0x040005C1 RID: 1473
		public int width;

		// Token: 0x040005C2 RID: 1474
		public int height;

		// Token: 0x040005C3 RID: 1475
		public GraphicsTile[,] tiles;
	}
}
