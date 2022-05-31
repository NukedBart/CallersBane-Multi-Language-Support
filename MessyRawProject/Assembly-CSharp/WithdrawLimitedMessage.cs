using System;

// Token: 0x020002F2 RID: 754
public class WithdrawLimitedMessage : Message
{
	// Token: 0x060012F7 RID: 4855 RVA: 0x0000E362 File Offset: 0x0000C562
	public WithdrawLimitedMessage(string deck)
	{
		this.deck = deck;
	}

	// Token: 0x060012F8 RID: 4856 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public WithdrawLimitedMessage()
	{
	}

	// Token: 0x04000FD3 RID: 4051
	public string deck;
}
