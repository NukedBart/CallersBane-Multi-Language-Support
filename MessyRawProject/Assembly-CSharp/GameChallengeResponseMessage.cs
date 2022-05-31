using System;

// Token: 0x020002B2 RID: 690
public class GameChallengeResponseMessage : Message
{
	// Token: 0x04000F51 RID: 3921
	public ProfileInfo from;

	// Token: 0x04000F52 RID: 3922
	public ProfileInfo to;

	// Token: 0x04000F53 RID: 3923
	public GameChallengeResponseMessage.Status status;

	// Token: 0x020002B3 RID: 691
	public enum Status
	{
		// Token: 0x04000F55 RID: 3925
		ACCEPT,
		// Token: 0x04000F56 RID: 3926
		DECLINE,
		// Token: 0x04000F57 RID: 3927
		CANCEL,
		// Token: 0x04000F58 RID: 3928
		TIMEOUT
	}
}
