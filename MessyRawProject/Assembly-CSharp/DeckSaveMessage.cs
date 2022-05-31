using System;
using System.Linq;

// Token: 0x020002A4 RID: 676
public class DeckSaveMessage : Message
{
	// Token: 0x0600125F RID: 4703 RVA: 0x0000DD91 File Offset: 0x0000BF91
	public DeckSaveMessage(string deckName, long[] cards) : this(deckName, cards, null)
	{
	}

	// Token: 0x06001260 RID: 4704 RVA: 0x00077D9C File Offset: 0x00075F9C
	public DeckSaveMessage(string deckName, long[] cards, string metadata)
	{
		this.name = deckName;
		this.cards = Enumerable.ToArray<string>(Enumerable.Select<long, string>(cards, (long id) => id.ToString()));
		this.metadata = metadata;
	}

	// Token: 0x04000F3B RID: 3899
	public string name;

	// Token: 0x04000F3C RID: 3900
	public string[] cards;

	// Token: 0x04000F3D RID: 3901
	public string metadata;
}
