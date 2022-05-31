using System;
using System.Collections.Generic;

// Token: 0x0200012F RID: 303
public interface ICardListCallback
{
	// Token: 0x060009AE RID: 2478
	void ButtonClicked(CardListPopup popup, ECardListButton button);

	// Token: 0x060009AF RID: 2479
	void ButtonClicked(CardListPopup popup, ECardListButton button, List<Card> selectedCards);

	// Token: 0x060009B0 RID: 2480
	void ItemHovered(CardListPopup popup, Card card);

	// Token: 0x060009B1 RID: 2481
	void ItemClicked(CardListPopup popup, Card card);

	// Token: 0x060009B2 RID: 2482
	void ItemButtonClicked(CardListPopup popup, Card card);
}
