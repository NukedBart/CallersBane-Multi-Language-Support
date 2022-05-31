using System;

// Token: 0x020002A7 RID: 679
public class GetCardStatsMessage : Message
{
	// Token: 0x06001267 RID: 4711 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public GetCardStatsMessage()
	{
	}

	// Token: 0x06001268 RID: 4712 RVA: 0x0000DDB9 File Offset: 0x0000BFB9
	public GetCardStatsMessage(long cardId)
	{
		this.cardId = cardId;
	}

	// Token: 0x06001269 RID: 4713 RVA: 0x0000DDC8 File Offset: 0x0000BFC8
	public override ServerRole allowedServerRoles()
	{
		return ServerRole.LOBBY | ServerRole.GAME;
	}

	// Token: 0x04000F43 RID: 3907
	public long cardId;

	// Token: 0x04000F44 RID: 3908
	public CardStat cardStat;

	// Token: 0x04000F45 RID: 3909
	public CardHistory[] history;

	// Token: 0x020002A8 RID: 680
	public interface ICardStatsReceiver
	{
		// Token: 0x0600126A RID: 4714
		void onCostUpdate(EMCostUpdate m);

		// Token: 0x0600126B RID: 4715
		void onCardStatsReceived(GetCardStatsMessage m);

		// Token: 0x0600126C RID: 4716
		void onCardTypeUpdated(CardTypeUpdateMessage update);
	}
}
