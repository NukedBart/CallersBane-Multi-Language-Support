using System;
using System.Collections.Generic;

namespace NSCampaign
{
	// Token: 0x020000E0 RID: 224
	public class Level
	{
		// Token: 0x0600078C RID: 1932 RVA: 0x00006BBD File Offset: 0x00004DBD
		public Level(Field field)
		{
			this.field = field;
		}

		// Token: 0x0600078D RID: 1933 RVA: 0x00006BE2 File Offset: 0x00004DE2
		public void addPlayer(Player player)
		{
			this.players.Add(player);
		}

		// Token: 0x0600078E RID: 1934 RVA: 0x00006BF0 File Offset: 0x00004DF0
		public void addMob(Mob mob)
		{
			this.mobs.Add(mob);
		}

		// Token: 0x0400059E RID: 1438
		public Field field;

		// Token: 0x0400059F RID: 1439
		public List<Player> players = new List<Player>();

		// Token: 0x040005A0 RID: 1440
		public List<Mob> mobs = new List<Mob>();
	}
}
