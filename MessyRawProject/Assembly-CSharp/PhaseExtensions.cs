using System;

// Token: 0x02000230 RID: 560
public static class PhaseExtensions
{
	// Token: 0x06001179 RID: 4473 RVA: 0x0000D58A File Offset: 0x0000B78A
	public static bool isPreGame(this EndPhaseMessage.Phase p)
	{
		return p == EndPhaseMessage.Phase.Init;
	}

	// Token: 0x0600117A RID: 4474 RVA: 0x000050EB File Offset: 0x000032EB
	public static bool isPostGame(this EndPhaseMessage.Phase p)
	{
		return p == EndPhaseMessage.Phase.End;
	}

	// Token: 0x0600117B RID: 4475 RVA: 0x0000D590 File Offset: 0x0000B790
	public static bool isGame(this EndPhaseMessage.Phase p)
	{
		return p == EndPhaseMessage.Phase.PreMain || p == EndPhaseMessage.Phase.Main;
	}
}
