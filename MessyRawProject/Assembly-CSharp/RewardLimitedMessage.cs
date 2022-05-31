using System;
using System.Collections.Generic;

// Token: 0x020002EE RID: 750
public class RewardLimitedMessage : Message
{
	// Token: 0x060012EF RID: 4847 RVA: 0x00078448 File Offset: 0x00076648
	public static RewardLimitedMessage getTestReward(Reward reward)
	{
		RewardLimitedMessage rewardLimitedMessage = new RewardLimitedMessage();
		rewardLimitedMessage.reward = reward;
		rewardLimitedMessage.deck = "TestDeck";
		int num = 0;
		List<Card> list = new List<Card>();
		foreach (CardType type in CardTypeManager.getInstance().getAll())
		{
			list.Add(new Card((long)(++num), type));
		}
		rewardLimitedMessage.cards = list.ToArray();
		return rewardLimitedMessage;
	}

	// Token: 0x04000FC5 RID: 4037
	public string deck;

	// Token: 0x04000FC6 RID: 4038
	public Card[] cards;

	// Token: 0x04000FC7 RID: 4039
	public Reward reward;

	// Token: 0x04000FC8 RID: 4040
	public int gamesWon;

	// Token: 0x04000FC9 RID: 4041
	public int gamesLeft;

	// Token: 0x04000FCA RID: 4042
	public int gamesTotal;
}
