using System;

// Token: 0x02000389 RID: 905
public interface IDeckCallback
{
	// Token: 0x06001405 RID: 5125
	void PopupDeckChosen(DeckInfo deck);

	// Token: 0x06001406 RID: 5126
	void PopupDeckDeleted(DeckInfo deck);

	// Token: 0x06001407 RID: 5127
	void PopupOk(string popupType);
}
