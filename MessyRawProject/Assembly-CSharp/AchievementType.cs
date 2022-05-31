using System;

// Token: 0x02000226 RID: 550
public class AchievementType
{
	// Token: 0x0600116E RID: 4462 RVA: 0x00077388 File Offset: 0x00075588
	public string getPart()
	{
		if (this._strPartType != null)
		{
			return this._strPartType;
		}
		if (this.partType == null)
		{
			return null;
		}
		if (this.partType.Value == AchievementType.PartType.MP_QUICK_RANKED)
		{
			this._strPartType = "In a MP Quick or Ranked";
		}
		else if (this.partType.Value == AchievementType.PartType.EASY_AI)
		{
			this._strPartType = "In a match vs Easy AI";
		}
		else if (this.partType.Value == AchievementType.PartType.HARD_AI)
		{
			this._strPartType = "In a match vs Hard AI";
		}
		return this._strPartType;
	}

	// Token: 0x0600116F RID: 4463 RVA: 0x0007741C File Offset: 0x0007561C
	public string getIcon()
	{
		return this.icon.Substring(0, this.icon.IndexOf("."));
	}

	// Token: 0x04000DDE RID: 3550
	public short id;

	// Token: 0x04000DDF RID: 3551
	public string name;

	// Token: 0x04000DE0 RID: 3552
	public string description;

	// Token: 0x04000DE1 RID: 3553
	public string icon = "Misc.png";

	// Token: 0x04000DE2 RID: 3554
	public int goldReward;

	// Token: 0x04000DE3 RID: 3555
	public short sortId;

	// Token: 0x04000DE4 RID: 3556
	public short group;

	// Token: 0x04000DE5 RID: 3557
	public AchievementType.PartType? partType;

	// Token: 0x04000DE6 RID: 3558
	private string _strPartType;

	// Token: 0x02000227 RID: 551
	public enum PartType
	{
		// Token: 0x04000DE8 RID: 3560
		EASY_AI,
		// Token: 0x04000DE9 RID: 3561
		HARD_AI,
		// Token: 0x04000DEA RID: 3562
		MP_QUICK_RANKED
	}
}
