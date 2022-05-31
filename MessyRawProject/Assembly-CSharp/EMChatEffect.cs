using System;

// Token: 0x0200023D RID: 573
public class EMChatEffect : EffectMessage
{
	// Token: 0x0600118F RID: 4495 RVA: 0x0000D636 File Offset: 0x0000B836
	public EMChatEffect()
	{
	}

	// Token: 0x06001190 RID: 4496 RVA: 0x0000D656 File Offset: 0x0000B856
	public EMChatEffect(GameChatMessageMessage m)
	{
		this.message = m;
	}

	// Token: 0x04000E10 RID: 3600
	public GameChatMessageMessage message;
}
