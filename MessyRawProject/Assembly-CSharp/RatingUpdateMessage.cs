using System;

// Token: 0x0200028B RID: 651
public class RatingUpdateMessage : Message
{
	// Token: 0x04000EFE RID: 3838
	[ServerToClient]
	public int whiteNewRating;

	// Token: 0x04000EFF RID: 3839
	[ServerToClient]
	public int whiteRatingChange;

	// Token: 0x04000F00 RID: 3840
	[ServerToClient]
	public int blackNewRating;

	// Token: 0x04000F01 RID: 3841
	[ServerToClient]
	public int blackRatingChange;

	// Token: 0x04000F02 RID: 3842
	[ServerToClient]
	public int whiteNewRank;

	// Token: 0x04000F03 RID: 3843
	[ServerToClient]
	public int whiteOldRank;

	// Token: 0x04000F04 RID: 3844
	[ServerToClient]
	public int blackNewRank;

	// Token: 0x04000F05 RID: 3845
	[ServerToClient]
	public int blackOldRank;

	// Token: 0x04000F06 RID: 3846
	[ServerToClient]
	public int whiteWinsForRank;

	// Token: 0x04000F07 RID: 3847
	[ServerToClient]
	public int blackWinsForRank;
}
