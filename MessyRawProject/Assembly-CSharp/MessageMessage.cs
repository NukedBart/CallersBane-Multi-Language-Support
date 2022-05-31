using System;

// Token: 0x0200031C RID: 796
public class MessageMessage : Message
{
	// Token: 0x06001341 RID: 4929 RVA: 0x0000E572 File Offset: 0x0000C772
	public bool isStarterDeck()
	{
		return this.type == MessageMessage.Type.DECAY_START_DECK || this.type == MessageMessage.Type.ENERGY_START_DECK || this.type == MessageMessage.Type.GROWTH_START_DECK || this.type == MessageMessage.Type.ORDER_START_DECK;
	}

	// Token: 0x04001032 RID: 4146
	public MessageMessage.Type type;

	// Token: 0x0200031D RID: 797
	public enum Type
	{
		// Token: 0x04001034 RID: 4148
		DECAY_START_DECK,
		// Token: 0x04001035 RID: 4149
		ENERGY_START_DECK,
		// Token: 0x04001036 RID: 4150
		GROWTH_START_DECK,
		// Token: 0x04001037 RID: 4151
		ORDER_START_DECK,
		// Token: 0x04001038 RID: 4152
		COLLECT_LAB_REWARD,
		// Token: 0x04001039 RID: 4153
		SOLD_MARKET_SCROLLS
	}
}
