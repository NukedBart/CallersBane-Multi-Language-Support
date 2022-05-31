using System;
using System.Collections.Generic;

// Token: 0x020002C9 RID: 713
public abstract class GetCustomGamesMessage : Message
{
	// Token: 0x060012B1 RID: 4785 RVA: 0x00078190 File Offset: 0x00076390
	public void init()
	{
		HashSet<int> hashSet = new HashSet<int>(this.completed ?? new int[0]);
		foreach (CustomGameInfo customGameInfo in this.popular)
		{
			customGameInfo.init(hashSet.Contains(customGameInfo.id.Value));
		}
		foreach (CustomGameInfo customGameInfo2 in this.recent)
		{
			customGameInfo2.init(hashSet.Contains(customGameInfo2.id.Value));
		}
		foreach (CustomGameInfo customGameInfo3 in this.mine)
		{
			customGameInfo3.init(hashSet.Contains(customGameInfo3.id.Value));
		}
	}

	// Token: 0x04000F70 RID: 3952
	public CustomGameInfo[] popular = new CustomGameInfo[0];

	// Token: 0x04000F71 RID: 3953
	public CustomGameInfo[] recent = new CustomGameInfo[0];

	// Token: 0x04000F72 RID: 3954
	public CustomGameInfo[] mine = new CustomGameInfo[0];

	// Token: 0x04000F73 RID: 3955
	public int[] completed;

	// Token: 0x04000F74 RID: 3956
	public string search;
}
