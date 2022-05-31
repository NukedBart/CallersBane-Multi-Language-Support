using System;

// Token: 0x020002CC RID: 716
public class CustomGameInfo
{
	// Token: 0x060012B6 RID: 4790 RVA: 0x0000E0DE File Offset: 0x0000C2DE
	public CustomGameInfo()
	{
	}

	// Token: 0x060012B7 RID: 4791 RVA: 0x0000E107 File Offset: 0x0000C307
	public CustomGameInfo(string name, bool isSinglePlayer, string code)
	{
		this.name = name;
		this.isSinglePlayer = isSinglePlayer;
		this.code = code;
	}

	// Token: 0x060012B8 RID: 4792 RVA: 0x00078270 File Offset: 0x00076470
	public void init(bool isCompleted)
	{
		this._rating = ((!this.isRated()) ? -1f : ((float)((double)this.totalRating / (double)this.totalRates)));
		this._ratingString = this._rating.ToString("0.##");
		this.isCompleted = isCompleted;
	}

	// Token: 0x060012B9 RID: 4793 RVA: 0x0000E145 File Offset: 0x0000C345
	public float rating()
	{
		return this._rating;
	}

	// Token: 0x060012BA RID: 4794 RVA: 0x0000E14D File Offset: 0x0000C34D
	public string ratingString()
	{
		return this._ratingString;
	}

	// Token: 0x060012BB RID: 4795 RVA: 0x0000E155 File Offset: 0x0000C355
	public bool isRated()
	{
		return this.totalRates > 1;
	}

	// Token: 0x04000F75 RID: 3957
	public int? id;

	// Token: 0x04000F76 RID: 3958
	public bool isSinglePlayer;

	// Token: 0x04000F77 RID: 3959
	[ServerToClient]
	public int profileId;

	// Token: 0x04000F78 RID: 3960
	public string profileName;

	// Token: 0x04000F79 RID: 3961
	public string name;

	// Token: 0x04000F7A RID: 3962
	[ServerToClient]
	public string flavor = string.Empty;

	// Token: 0x04000F7B RID: 3963
	public string code;

	// Token: 0x04000F7C RID: 3964
	public string descriptionP1;

	// Token: 0x04000F7D RID: 3965
	public string descriptionP2;

	// Token: 0x04000F7E RID: 3966
	[ServerToClient]
	public string deckP1 = string.Empty;

	// Token: 0x04000F7F RID: 3967
	[ServerToClient]
	public string deckP2 = string.Empty;

	// Token: 0x04000F80 RID: 3968
	[ServerToClient]
	public int totalRates;

	// Token: 0x04000F81 RID: 3969
	[ServerToClient]
	public int totalRating;

	// Token: 0x04000F82 RID: 3970
	public string bet;

	// Token: 0x04000F83 RID: 3971
	public string timer;

	// Token: 0x04000F84 RID: 3972
	[ServerToClient]
	public bool chooseDeckP1;

	// Token: 0x04000F85 RID: 3973
	[ServerToClient]
	public bool chooseDeckP2;

	// Token: 0x04000F86 RID: 3974
	[ServerToClient]
	public bool isPuzzle;

	// Token: 0x04000F87 RID: 3975
	[ServerToClient]
	public bool isCampaign;

	// Token: 0x04000F88 RID: 3976
	[ServerToClient]
	public bool chooseDifficulty;

	// Token: 0x04000F89 RID: 3977
	[ServerToClient]
	private float _rating;

	// Token: 0x04000F8A RID: 3978
	[ServerToClient]
	private string _ratingString;

	// Token: 0x04000F8B RID: 3979
	[Transient]
	public bool isCompleted;
}
