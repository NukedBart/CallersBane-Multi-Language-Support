using System;

namespace NSCampaign
{
	// Token: 0x020000E2 RID: 226
	public class Mob
	{
		// Token: 0x06000793 RID: 1939 RVA: 0x00006C3D File Offset: 0x00004E3D
		public Mob() : this(0, 0)
		{
		}

		// Token: 0x06000794 RID: 1940 RVA: 0x00006C47 File Offset: 0x00004E47
		public Mob(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		// Token: 0x040005A5 RID: 1445
		public int x;

		// Token: 0x040005A6 RID: 1446
		public int y;

		// Token: 0x040005A7 RID: 1447
		public int id;

		// Token: 0x040005A8 RID: 1448
		public MobPrototype mobPrototype;

		// Token: 0x040005A9 RID: 1449
		public bool isMobile;

		// Token: 0x040005AA RID: 1450
		public int mobTypeId;
	}
}
