using System;

// Token: 0x020002F7 RID: 759
public class TowerLevel
{
	// Token: 0x06001301 RID: 4865 RVA: 0x0000E3C2 File Offset: 0x0000C5C2
	public string getHeader()
	{
		return (this.title == null) ? StringUtil.capitalize(this.difficulty.ToString()) : this.title;
	}

	// Token: 0x06001302 RID: 4866 RVA: 0x0000E3EF File Offset: 0x0000C5EF
	public bool isTutorial()
	{
		return this.difficulty == AiDifficulty.TUTORIAL;
	}

	// Token: 0x06001303 RID: 4867 RVA: 0x0000E3FA File Offset: 0x0000C5FA
	public bool isEasy()
	{
		return this.difficulty == AiDifficulty.EASY;
	}

	// Token: 0x06001304 RID: 4868 RVA: 0x0000E405 File Offset: 0x0000C605
	public bool isMedium()
	{
		return this.difficulty == AiDifficulty.MEDIUM;
	}

	// Token: 0x06001305 RID: 4869 RVA: 0x0000E410 File Offset: 0x0000C610
	public bool isHard()
	{
		return this.difficulty == AiDifficulty.HARD;
	}

	// Token: 0x06001306 RID: 4870 RVA: 0x0000E41B File Offset: 0x0000C61B
	public bool isDemoLocked()
	{
		return this.difficulty > AiDifficulty.EASY;
	}

	// Token: 0x06001307 RID: 4871 RVA: 0x0000E426 File Offset: 0x0000C626
	public bool isTutorialLocked(TowerLevel[] levels)
	{
		if (this._tutorialLock != null)
		{
			return this._tutorialLock.Value;
		}
		this._tutorialLock = new bool?(this._checkTutorialLock(levels));
		return this._tutorialLock.Value;
	}

	// Token: 0x06001308 RID: 4872 RVA: 0x00078558 File Offset: 0x00076758
	private bool _checkTutorialLock(TowerLevel[] levels)
	{
		if (this.type != TowerLevel.Type.TUTORIAL || this.getHeader() != "Tutorial")
		{
			return false;
		}
		int num = CollectionUtil.indexOf<TowerLevel>(levels, this);
		if (num <= 0)
		{
			return false;
		}
		TowerLevel towerLevel = levels[num - 1];
		return towerLevel != null && towerLevel.type == TowerLevel.Type.TUTORIAL && !towerLevel.isCompleted;
	}

	// Token: 0x04000FD8 RID: 4056
	public int id = -1;

	// Token: 0x04000FD9 RID: 4057
	public string name = string.Empty;

	// Token: 0x04000FDA RID: 4058
	public string description = string.Empty;

	// Token: 0x04000FDB RID: 4059
	public bool isCompleted;

	// Token: 0x04000FDC RID: 4060
	public bool borrowDeck;

	// Token: 0x04000FDD RID: 4061
	public int goldReward;

	// Token: 0x04000FDE RID: 4062
	public int cardRewardCount;

	// Token: 0x04000FDF RID: 4063
	public AiDifficulty difficulty;

	// Token: 0x04000FE0 RID: 4064
	public string flavour = string.Empty;

	// Token: 0x04000FE1 RID: 4065
	public TowerLevel.Type type;

	// Token: 0x04000FE2 RID: 4066
	public string title;

	// Token: 0x04000FE3 RID: 4067
	private bool? _tutorialLock;

	// Token: 0x020002F8 RID: 760
	public enum Type
	{
		// Token: 0x04000FE5 RID: 4069
		TRIAL,
		// Token: 0x04000FE6 RID: 4070
		TUTORIAL,
		// Token: 0x04000FE7 RID: 4071
		DAILY
	}
}
