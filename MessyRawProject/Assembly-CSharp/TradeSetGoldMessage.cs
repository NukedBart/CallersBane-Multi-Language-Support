using System;

// Token: 0x02000373 RID: 883
public class TradeSetGoldMessage : Message
{
	// Token: 0x060013D4 RID: 5076 RVA: 0x0000EB27 File Offset: 0x0000CD27
	public TradeSetGoldMessage(int gold)
	{
		this.gold = gold;
	}

	// Token: 0x04001107 RID: 4359
	public int gold;
}
