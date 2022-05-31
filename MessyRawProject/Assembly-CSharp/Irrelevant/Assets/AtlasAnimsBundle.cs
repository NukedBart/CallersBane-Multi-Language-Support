using System;
using Animation.Serialization;

namespace Irrelevant.Assets
{
	// Token: 0x0200001B RID: 27
	public class AtlasAnimsBundle
	{
		// Token: 0x0600017B RID: 379 RVA: 0x00003391 File Offset: 0x00001591
		public AtlasAnimsBundle(PD_Atlas atlas, PD_AnimCollection anims)
		{
			this.atlas = atlas;
			this.anims = anims;
		}

		// Token: 0x040000A9 RID: 169
		public readonly PD_Atlas atlas;

		// Token: 0x040000AA RID: 170
		public readonly PD_AnimCollection anims;
	}
}
