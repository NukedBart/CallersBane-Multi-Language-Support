using System;

// Token: 0x020003A5 RID: 933
public class Ranks
{
	// Token: 0x0600150D RID: 5389 RVA: 0x0000F6AF File Offset: 0x0000D8AF
	public static Rank Get(int rankIndex)
	{
		if (rankIndex >= 0 && rankIndex < Ranks.ranks.Length)
		{
			return Ranks.ranks[rankIndex];
		}
		return null;
	}

	// Token: 0x04001237 RID: 4663
	private static Rank[] ranks = new Rank[]
	{
		new Rank("Adventurer", "Profile/ranks/rank_novice_1"),
		new Rank("Adept", "Profile/ranks/rank_novice_2"),
		new Rank("Trickster", "Profile/ranks/rank_novice_3"),
		new Rank("Sorcerer", "Profile/ranks/rank_novice_4"),
		new Rank("Apprentice Caller", "Profile/ranks/rank_ordinary_1"),
		new Rank("Caller", "Profile/ranks/rank_ordinary_2"),
		new Rank("Accomplished Caller", "Profile/ranks/rank_ordinary_3"),
		new Rank("Master Caller", "Profile/ranks/rank_ordinary_4"),
		new Rank("Exalted Caller", "Profile/ranks/rank_master_1"),
		new Rank("Grand Master", "Profile/ranks/rank_master_2"),
		new Rank("Aspect-Commander", "Profile/ranks/rank_master_3"),
		new Rank("Ascendant", "Profile/ranks/rank_master_4")
	};
}
