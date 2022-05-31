using System;

// Token: 0x020002B0 RID: 688
public class GameChallengeMessage : Message
{
	// Token: 0x0600127A RID: 4730 RVA: 0x0000DE38 File Offset: 0x0000C038
	public bool isCustom()
	{
		return this.customGame != null;
	}

	// Token: 0x04000F4D RID: 3917
	public ProfileInfo from;

	// Token: 0x04000F4E RID: 3918
	public CustomGameInfo customGame;
}
