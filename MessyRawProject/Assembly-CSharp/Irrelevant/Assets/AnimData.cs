using System;
using System.Collections.Generic;
using Animation.Serialization;
using UnityEngine;

namespace Irrelevant.Assets
{
	// Token: 0x02000011 RID: 17
	public class AnimData
	{
		// Token: 0x06000122 RID: 290 RVA: 0x0001E0D4 File Offset: 0x0001C2D4
		public AnimData(PD_Anim anim)
		{
			this.iid = ++AnimData.runningId;
			this._anim = anim;
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000124 RID: 292 RVA: 0x00002E85 File Offset: 0x00001085
		public float fps
		{
			get
			{
				return this._anim.fps;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000125 RID: 293 RVA: 0x00002E92 File Offset: 0x00001092
		public string name
		{
			get
			{
				return this._anim.name;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000126 RID: 294 RVA: 0x00002E9F File Offset: 0x0000109F
		public List<PD_AnimFrame> frames
		{
			get
			{
				return this._anim.frames;
			}
		}

		// Token: 0x06000127 RID: 295 RVA: 0x0001E128 File Offset: 0x0001C328
		private void updateMinMaxLayers()
		{
			foreach (PD_AnimFrame pd_AnimFrame in this._anim.frames)
			{
				if (this._firstFrame < 0 || pd_AnimFrame.frame < this._firstFrame)
				{
					this._firstFrame = pd_AnimFrame.frame;
				}
				if (this._lastFrame < 0 || pd_AnimFrame.frame > this._lastFrame)
				{
					this._lastFrame = pd_AnimFrame.frame;
				}
				foreach (PD_AnimFramePart pd_AnimFramePart in pd_AnimFrame.parts)
				{
					short num;
					bool flag = this._minLayers.TryGetValue(pd_AnimFrame.frame, ref num);
					if (!flag || pd_AnimFramePart.layer < num)
					{
						this._minLayers.Add(pd_AnimFrame.frame, pd_AnimFramePart.layer);
					}
				}
			}
		}

		// Token: 0x06000128 RID: 296 RVA: 0x0001E254 File Offset: 0x0001C454
		private void normalizeLayers()
		{
			foreach (PD_AnimFrame pd_AnimFrame in this._anim.frames)
			{
				foreach (PD_AnimFramePart pd_AnimFramePart in pd_AnimFrame.parts)
				{
					PD_AnimFramePart pd_AnimFramePart2 = pd_AnimFramePart;
					pd_AnimFramePart2.layer -= this._minLayers[pd_AnimFrame.frame];
				}
			}
		}

		// Token: 0x06000129 RID: 297 RVA: 0x0001E310 File Offset: 0x0001C510
		private void buildFrameMeshes()
		{
			this._frameMeshes = new Mesh[this._lastFrame + 1];
			for (int i = this._firstFrame; i < this._lastFrame + 1; i++)
			{
				this.buildAndCacheFrameMesh(i);
			}
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00002EAC File Offset: 0x000010AC
		public void postLoad()
		{
			this.updateMinMaxLayers();
			this.normalizeLayers();
			this.buildFrameMeshes();
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00002EC0 File Offset: 0x000010C0
		public List<PD_AnimLocator> getAnimLocators()
		{
			return this._anim.locators;
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00002ECD File Offset: 0x000010CD
		public int getNumFrames()
		{
			return this._lastFrame + 1;
		}

		// Token: 0x0600012D RID: 301 RVA: 0x0001E358 File Offset: 0x0001C558
		public Mesh getMeshForFrame(int frame)
		{
			if (this._frameMeshes == null || frame < 0 || frame >= this._frameMeshes.Length)
			{
				return null;
			}
			if (this._frameMeshes[frame] == null)
			{
				this.buildAndCacheFrameMesh(frame);
			}
			return this._frameMeshes[frame];
		}

		// Token: 0x0600012E RID: 302 RVA: 0x0001E3AC File Offset: 0x0001C5AC
		private void buildAndCacheFrameMesh(int frame)
		{
			List<PD_AnimFramePart> animPartDatasForFrame = this.getAnimPartDatasForFrame(frame);
			this._frameMeshes[frame] = ((animPartDatasForFrame.Count <= 0) ? AnimData.NullMesh : GraphicsUtils.combineMeshes(animPartDatasForFrame));
		}

		// Token: 0x0600012F RID: 303 RVA: 0x0001E3E8 File Offset: 0x0001C5E8
		public Mesh buildMeshForFrame(int frame, AnimPartDataModifier mod)
		{
			if (frame < 0 && frame >= this._frameMeshes.Length)
			{
				return null;
			}
			List<PD_AnimFramePart> animPartDatasForFrame = this.getAnimPartDatasForFrame(frame);
			if (mod != null)
			{
				foreach (PD_AnimFramePart part in animPartDatasForFrame)
				{
					mod.modify(part);
				}
			}
			return GraphicsUtils.combineMeshes(animPartDatasForFrame);
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00002ED7 File Offset: 0x000010D7
		public List<PD_AnimFramePart> getAnimPartDatasForFrame(int frame)
		{
			return this._anim.frames[frame].parts;
		}

		// Token: 0x04000076 RID: 118
		private static readonly Mesh NullMesh = new Mesh();

		// Token: 0x04000077 RID: 119
		private PD_Anim _anim;

		// Token: 0x04000078 RID: 120
		private Mesh[] _frameMeshes;

		// Token: 0x04000079 RID: 121
		private int _firstFrame;

		// Token: 0x0400007A RID: 122
		private int _lastFrame = -1;

		// Token: 0x0400007B RID: 123
		public string id = string.Empty;

		// Token: 0x0400007C RID: 124
		private static int runningId = 0;

		// Token: 0x0400007D RID: 125
		private int iid = -1;

		// Token: 0x0400007E RID: 126
		private Dictionary<int, short> _minLayers = new Dictionary<int, short>();
	}
}
