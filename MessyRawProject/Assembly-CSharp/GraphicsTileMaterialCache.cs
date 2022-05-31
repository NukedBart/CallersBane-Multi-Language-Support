using System;
using System.Collections.Generic;
using NSCampaign;
using UnityEngine;

// Token: 0x020000E9 RID: 233
internal class GraphicsTileMaterialCache
{
	// Token: 0x060007C5 RID: 1989 RVA: 0x000437C4 File Offset: 0x000419C4
	public static void setTileGraphics(int id, string bitmap)
	{
		int num = bitmap.LastIndexOf(".");
		string text = bitmap;
		if (num >= 0)
		{
			text = bitmap.Substring(0, num);
		}
		GraphicsTileMaterialCache.idFilenameMap[id] = text;
	}

	// Token: 0x060007C6 RID: 1990 RVA: 0x00006E40 File Offset: 0x00005040
	public static Texture2D getTexture(FieldTile t)
	{
		return ResourceManager.LoadTexture("Campaign/tiles/1/" + GraphicsTileMaterialCache.idFilenameMap[t.id]);
	}

	// Token: 0x060007C7 RID: 1991 RVA: 0x000437FC File Offset: 0x000419FC
	public static Material getMaterial(FieldTile t, int row)
	{
		Material material = null;
		GraphicsTileMaterialCache.Key key = new GraphicsTileMaterialCache.Key(t.id, row);
		if (GraphicsTileMaterialCache.materials.TryGetValue(key, ref material))
		{
			return material;
		}
		material = new Material(ResourceManager.LoadShader("Transparent/Diffuse"));
		material.renderQueue = CampaignRenderer.getRenderQueueForRow(row);
		material.mainTexture = ResourceManager.LoadTexture("Campaign/tiles/1/" + GraphicsTileMaterialCache.idFilenameMap[t.id]);
		GraphicsTileMaterialCache.materials.Add(key, material);
		return material;
	}

	// Token: 0x040005D0 RID: 1488
	private static Dictionary<GraphicsTileMaterialCache.Key, Material> materials = new Dictionary<GraphicsTileMaterialCache.Key, Material>();

	// Token: 0x040005D1 RID: 1489
	private static Dictionary<int, string> idFilenameMap = new Dictionary<int, string>();

	// Token: 0x020000EA RID: 234
	private class Key : IEquatable<GraphicsTileMaterialCache.Key>
	{
		// Token: 0x060007C8 RID: 1992 RVA: 0x00006E61 File Offset: 0x00005061
		public Key(int id, int row)
		{
			this.id = id;
			this.row = row;
		}

		// Token: 0x060007C9 RID: 1993 RVA: 0x00006E77 File Offset: 0x00005077
		public bool Equals(GraphicsTileMaterialCache.Key o)
		{
			return this.id == o.id && this.row == o.row;
		}

		// Token: 0x060007CA RID: 1994 RVA: 0x00006E9B File Offset: 0x0000509B
		public override int GetHashCode()
		{
			return this.id.GetHashCode() + this.row.GetHashCode();
		}

		// Token: 0x060007CB RID: 1995 RVA: 0x00006EB4 File Offset: 0x000050B4
		public override bool Equals(object o)
		{
			return this.Equals(o as GraphicsTileMaterialCache.Key);
		}

		// Token: 0x040005D2 RID: 1490
		public int id;

		// Token: 0x040005D3 RID: 1491
		public int row;
	}
}
