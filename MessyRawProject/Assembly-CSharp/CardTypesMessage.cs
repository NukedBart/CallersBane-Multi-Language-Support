using System;

// Token: 0x0200029A RID: 666
public class CardTypesMessage : Message
{
	// Token: 0x06001249 RID: 4681 RVA: 0x000059E4 File Offset: 0x00003BE4
	public override bool shouldLog()
	{
		return false;
	}

	// Token: 0x0600124A RID: 4682 RVA: 0x00077C34 File Offset: 0x00075E34
	public void consume()
	{
		if (this.consumed)
		{
			return;
		}
		foreach (CardType cardType in this.cardTypes)
		{
			cardType.onLoaded();
		}
		this.consumed = true;
	}

	// Token: 0x04000F29 RID: 3881
	public CardType[] cardTypes;

	// Token: 0x04000F2A RID: 3882
	private bool consumed;
}
