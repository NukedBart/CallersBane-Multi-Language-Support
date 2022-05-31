using System;

// Token: 0x02000300 RID: 768
public class WeeklyWinner
{
	// Token: 0x06001313 RID: 4883 RVA: 0x00078680 File Offset: 0x00076880
	public string getIcon()
	{
		if (this.winType == "GOLD")
		{
			return "ChatUI/win_first_icon";
		}
		if (this.winType == "SILVER")
		{
			return "ChatUI/win_second_icon";
		}
		if (this.winType == "BRONZE")
		{
			return "ChatUI/win_third_icon";
		}
		if (this.winType == "MOST_WINS")
		{
			return "ChatUI/win_most_icon";
		}
		return null;
	}

	// Token: 0x04000FFA RID: 4090
	public int profileId;

	// Token: 0x04000FFB RID: 4091
	public string winType;

	// Token: 0x04000FFC RID: 4092
	public string userName;

	// Token: 0x04000FFD RID: 4093
	public long? gameId;
}
