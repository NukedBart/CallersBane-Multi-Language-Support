using System;

// Token: 0x020002FF RID: 767
internal class UpdateWeeklyWinnersMessage : Message
{
	// Token: 0x04000FF9 RID: 4089
	[ServerToClient]
	public WeeklyWinner[] weeklyWinners;
}
