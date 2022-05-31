using System;

// Token: 0x020002B4 RID: 692
public class SetAcceptChallengesMessage : GameChallengeBaseMessage
{
	// Token: 0x0600127E RID: 4734 RVA: 0x0000DE79 File Offset: 0x0000C079
	public SetAcceptChallengesMessage(bool isAccepting)
	{
		this.acceptChallenges = isAccepting;
	}

	// Token: 0x04000F59 RID: 3929
	public bool acceptChallenges;
}
