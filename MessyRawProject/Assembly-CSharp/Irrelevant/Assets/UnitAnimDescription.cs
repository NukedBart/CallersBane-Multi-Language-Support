using System;
using System.Collections.Generic;
using System.Linq;
using Animation.Serialization;
using UnityEngine;

namespace Irrelevant.Assets
{
	// Token: 0x02000018 RID: 24
	public class UnitAnimDescription
	{
		// Token: 0x0600016C RID: 364 RVA: 0x0001F2CC File Offset: 0x0001D4CC
		public UnitAnimDescription(AtlasAnimsBundle bundle)
		{
			this.bundle = bundle;
			this.desc = new UnitAtlas(bundle.atlas);
			this.data = new List<UnitAnimDescription.AnimDataObj>(bundle.anims.animations.Count);
			foreach (PD_Anim pd_Anim in bundle.anims.animations)
			{
				if (pd_Anim.frames != null && pd_Anim.frames.Count != 0)
				{
					this.data.Add(new UnitAnimDescription.AnimDataObj(new AnimData(pd_Anim)));
				}
			}
			this.postLoad();
		}

		// Token: 0x0600016D RID: 365 RVA: 0x0000327C File Offset: 0x0000147C
		internal bool hasAnimation(int id)
		{
			return id >= 0 && id < this.data.Count;
		}

		// Token: 0x0600016E RID: 366 RVA: 0x00003296 File Offset: 0x00001496
		internal AnimData getAnimation(int id)
		{
			if (!this.hasAnimation(id))
			{
				return null;
			}
			return this.data[id].assureLoaded();
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0001F39C File Offset: 0x0001D59C
		internal int getAnimationId(string p)
		{
			p = p.ToLower();
			for (int i = 0; i < this.data.Count; i++)
			{
				if (this.data[i].animData.name.ToLower() == p)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06000170 RID: 368 RVA: 0x000032B7 File Offset: 0x000014B7
		internal string[] getAnimationNames()
		{
			return Enumerable.ToArray<string>(Enumerable.Select<UnitAnimDescription.AnimDataObj, string>(this.data, (UnitAnimDescription.AnimDataObj d) => d.animData.name));
		}

		// Token: 0x06000171 RID: 369 RVA: 0x0001F3F8 File Offset: 0x0001D5F8
		public void nameAnimations(string name)
		{
			foreach (UnitAnimDescription.AnimDataObj animDataObj in this.data)
			{
				animDataObj.animData.id = name + ";" + animDataObj.animData.name;
			}
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0001F46C File Offset: 0x0001D66C
		private void postLoad()
		{
			this.desc.postLoad();
			foreach (UnitAnimDescription.AnimDataObj animDataObj in this.data)
			{
				if (animDataObj.animData.frames != null)
				{
					foreach (PD_AnimFrame pd_AnimFrame in animDataObj.animData.frames)
					{
						foreach (PD_AnimFramePart pd_AnimFramePart in pd_AnimFrame.parts)
						{
							pd_AnimFramePart.mesh = this.desc.getImageMesh((int)pd_AnimFramePart.meshId);
						}
					}
				}
			}
		}

		// Token: 0x0400009E RID: 158
		private const bool LazyLoad = true;

		// Token: 0x0400009F RID: 159
		private UnitAtlas desc;

		// Token: 0x040000A0 RID: 160
		private List<UnitAnimDescription.AnimDataObj> data;

		// Token: 0x040000A1 RID: 161
		public Texture2D textureReference;

		// Token: 0x040000A2 RID: 162
		public AtlasAnimsBundle bundle;

		// Token: 0x02000019 RID: 25
		private class AnimDataObj
		{
			// Token: 0x06000174 RID: 372 RVA: 0x000032F3 File Offset: 0x000014F3
			public AnimDataObj(AnimData animData)
			{
				this.animData = animData;
			}

			// Token: 0x06000175 RID: 373 RVA: 0x00003302 File Offset: 0x00001502
			internal AnimData assureLoaded()
			{
				if (!this.isLoaded)
				{
					this.animData.postLoad();
					this.isLoaded = true;
				}
				return this.animData;
			}

			// Token: 0x040000A4 RID: 164
			private bool isLoaded;

			// Token: 0x040000A5 RID: 165
			public AnimData animData;
		}
	}
}
