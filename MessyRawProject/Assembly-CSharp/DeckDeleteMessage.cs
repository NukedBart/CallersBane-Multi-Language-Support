using System;

// Token: 0x020002A2 RID: 674
public class DeckDeleteMessage : Message
{
	// Token: 0x0600125C RID: 4700 RVA: 0x0000DD7A File Offset: 0x0000BF7A
	public DeckDeleteMessage(string deckName)
	{
		this.name = deckName;
	}

	// Token: 0x04000F38 RID: 3896
	public string name;
}
