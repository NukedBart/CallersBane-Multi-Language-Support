using System;
using System.Collections.Generic;

// Token: 0x02000416 RID: 1046
public class TowerChallengeInfo
{
	// Token: 0x17000122 RID: 290
	// (get) Token: 0x06001720 RID: 5920 RVA: 0x00010A3A File Offset: 0x0000EC3A
	// (set) Token: 0x06001721 RID: 5921 RVA: 0x00010A42 File Offset: 0x0000EC42
	public TowerLevel[] levels
	{
		get
		{
			return this._levels;
		}
		private set
		{
			this.setLevels(value);
		}
	}

	// Token: 0x06001722 RID: 5922 RVA: 0x0008F334 File Offset: 0x0008D534
	public void setLevels(TowerLevel[] levels)
	{
		List<TowerLevel> list = new List<TowerLevel>();
		this.numSeparators = 0;
		bool flag = false;
		string text = "_";
		foreach (TowerLevel towerLevel in levels)
		{
			bool flag2 = false;
			flag2 |= (flag && towerLevel.type != TowerLevel.Type.DAILY);
			flag = (towerLevel.type == TowerLevel.Type.DAILY);
			flag2 |= (text != "_" && text != towerLevel.title);
			text = towerLevel.title;
			if (flag2)
			{
				list.Add(null);
				this.numSeparators++;
			}
			list.Add(towerLevel);
		}
		this._levels = list.ToArray();
	}

	// Token: 0x06001723 RID: 5923 RVA: 0x00010A4B File Offset: 0x0000EC4B
	public int separatorCount()
	{
		return this.numSeparators;
	}

	// Token: 0x06001724 RID: 5924 RVA: 0x00010A53 File Offset: 0x0000EC53
	public int levelCount()
	{
		return this._levels.Length - this.numSeparators;
	}

	// Token: 0x06001725 RID: 5925 RVA: 0x00010A64 File Offset: 0x0000EC64
	public void SetTowerChallengeInfo(GetTowerInfoMessage msg)
	{
		this.levels = msg.getSortedLevels();
	}

	// Token: 0x06001726 RID: 5926 RVA: 0x0008F3FC File Offset: 0x0008D5FC
	public int NextTutorialId(int refId)
	{
		int num = this.levels.Length;
		for (int i = 0; i < this.levels.Length; i++)
		{
			if (this.levels[i] != null && this.levels[i].id == refId)
			{
				num = i;
				break;
			}
		}
		for (int j = num + 1; j < this.levels.Length; j++)
		{
			if (TowerChallengeInfo.isRealTutorial(this.levels[j]))
			{
				return this.levels[j].id;
			}
		}
		return -1;
	}

	// Token: 0x06001727 RID: 5927 RVA: 0x00010A72 File Offset: 0x0000EC72
	private static bool isRealTutorial(TowerLevel level)
	{
		return level != null && level.type == TowerLevel.Type.TUTORIAL && level.getHeader() == "Tutorial";
	}

	// Token: 0x06001728 RID: 5928 RVA: 0x0008F490 File Offset: 0x0008D690
	public bool IsLastTutorial(int refId)
	{
		for (int i = this.levels.Length - 1; i >= 0; i--)
		{
			TowerLevel towerLevel = this.levels[i];
			if (towerLevel != null && towerLevel.id != refId && towerLevel.type == TowerLevel.Type.TUTORIAL && towerLevel.getHeader() == "Tutorial")
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0400148B RID: 5259
	private int numSeparators;

	// Token: 0x0400148C RID: 5260
	private TowerLevel[] _levels = new TowerLevel[0];

	// Token: 0x0400148D RID: 5261
	public int levelChosen;
}
