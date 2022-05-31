using System;
using Animation.Serialization;

namespace Irrelevant.Assets
{
	// Token: 0x02000013 RID: 19
	public class AnimLocator
	{
		// Token: 0x06000132 RID: 306 RVA: 0x00002EEF File Offset: 0x000010EF
		public AnimLocator(string name, int frame)
		{
			this.name = name;
			this.frame = frame;
		}

		// Token: 0x06000133 RID: 307 RVA: 0x0001E468 File Offset: 0x0001C668
		public AnimLocator(PD_AnimLocator locator)
		{
			this.name = locator.name;
			this.frame = locator.frame;
			PD_AnimLocator.Pos pos = locator.pos;
			this.pos = new AnimLocator.Pos(pos.x, pos.y, pos.rotation, pos.sx, pos.sy);
		}

		// Token: 0x0400007F RID: 127
		public string name;

		// Token: 0x04000080 RID: 128
		public int frame;

		// Token: 0x04000081 RID: 129
		public AnimLocator.Pos pos = new AnimLocator.Pos();

		// Token: 0x02000014 RID: 20
		public class Pos
		{
			// Token: 0x06000134 RID: 308 RVA: 0x00002F10 File Offset: 0x00001110
			public Pos() : this(0f, 0f, 0f)
			{
			}

			// Token: 0x06000135 RID: 309 RVA: 0x00002F27 File Offset: 0x00001127
			public Pos(float x, float y, float rotation) : this(x, y, rotation, 1f, 1f)
			{
			}

			// Token: 0x06000136 RID: 310 RVA: 0x00002F3C File Offset: 0x0000113C
			public Pos(float x, float y, float rotation, float sx, float sy)
			{
				this.x = 0.01f * x;
				this.y = 0.01f * y;
				this.rotation = rotation;
				this.sx = sx;
				this.sy = sy;
			}

			// Token: 0x04000082 RID: 130
			public float x;

			// Token: 0x04000083 RID: 131
			public float y;

			// Token: 0x04000084 RID: 132
			public float rotation;

			// Token: 0x04000085 RID: 133
			public float sx;

			// Token: 0x04000086 RID: 134
			public float sy;
		}
	}
}
