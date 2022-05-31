using System;
using System.Linq;
using System.Text;

// Token: 0x020002A5 RID: 677
public class DeckValidateMessage : Message
{
	// Token: 0x06001262 RID: 4706 RVA: 0x0000DDA5 File Offset: 0x0000BFA5
	public DeckValidateMessage()
	{
	}

	// Token: 0x06001263 RID: 4707 RVA: 0x00077DEC File Offset: 0x00075FEC
	public DeckValidateMessage(Card[] cards)
	{
		this.cards = Enumerable.ToArray<string>(Enumerable.Select<Card, string>(cards, (Card c) => c.getId().ToString()));
	}

	// Token: 0x06001264 RID: 4708 RVA: 0x00077E3C File Offset: 0x0007603C
	public string getErrorString(int maxErrors)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < this.errors.Length; i++)
		{
			if (i == maxErrors)
			{
				break;
			}
			if (i > 0)
			{
				stringBuilder.Append("\n");
			}
			stringBuilder.Append("* " + this.errors[i].msg);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x04000F3F RID: 3903
	public string[] cards;

	// Token: 0x04000F40 RID: 3904
	[ServerToClient]
	public ErrorMsg[] errors = new ErrorMsg[0];
}
