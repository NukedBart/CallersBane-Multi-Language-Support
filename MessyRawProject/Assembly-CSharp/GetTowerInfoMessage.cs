using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x020002F5 RID: 757
public class GetTowerInfoMessage : LobbyMessage
{
	// Token: 0x060012FC RID: 4860 RVA: 0x000784E0 File Offset: 0x000766E0
	public TowerLevel[] getSortedLevels()
	{
		List<TowerLevel> list = new List<TowerLevel>();
		list.AddRange(Enumerable.Where<TowerLevel>(this.levels, (TowerLevel l) => l.type == TowerLevel.Type.DAILY));
		list.AddRange(Enumerable.Where<TowerLevel>(this.levels, (TowerLevel l) => l.type != TowerLevel.Type.DAILY));
		return list.ToArray();
	}

	// Token: 0x04000FD5 RID: 4053
	public TowerLevel[] levels;
}
