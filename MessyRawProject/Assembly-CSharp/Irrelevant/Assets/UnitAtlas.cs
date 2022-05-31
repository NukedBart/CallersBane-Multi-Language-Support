using System;
using System.Collections.Generic;
using Animation.Serialization;
using UnityEngine;

namespace Irrelevant.Assets
{
	// Token: 0x0200001A RID: 26
	public class UnitAtlas
	{
		// Token: 0x06000176 RID: 374 RVA: 0x00003327 File Offset: 0x00001527
		public UnitAtlas(PD_Atlas atlas)
		{
			this.desc = atlas;
		}

		// Token: 0x06000177 RID: 375 RVA: 0x0001F584 File Offset: 0x0001D784
		public void postLoad()
		{
			float num = 1f / this.w();
			float num2 = 1f / this.h();
			foreach (PD_AtlasItem pd_AtlasItem in this.desc.atlasItems)
			{
				this._meshes[(int)pd_AtlasItem.id] = GraphicsUtils.createQuad((float)pd_AtlasItem.realW, (float)pd_AtlasItem.realH, (float)pd_AtlasItem.x * num, (float)pd_AtlasItem.y * num2, (float)pd_AtlasItem.w * num, (float)pd_AtlasItem.h * num2);
			}
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00003341 File Offset: 0x00001541
		public Mesh getImageMesh(int id)
		{
			if (!this._meshes.ContainsKey(id))
			{
				Log.info("Meshes doesn't contain: " + id);
			}
			return this._meshes[id];
		}

		// Token: 0x06000179 RID: 377 RVA: 0x00003375 File Offset: 0x00001575
		public float w()
		{
			return (float)this.desc.width;
		}

		// Token: 0x0600017A RID: 378 RVA: 0x00003383 File Offset: 0x00001583
		public float h()
		{
			return (float)this.desc.height;
		}

		// Token: 0x040000A6 RID: 166
		private PD_Atlas desc;

		// Token: 0x040000A7 RID: 167
		public Texture atlas;

		// Token: 0x040000A8 RID: 168
		private Dictionary<int, Mesh> _meshes = new Dictionary<int, Mesh>();
	}
}
