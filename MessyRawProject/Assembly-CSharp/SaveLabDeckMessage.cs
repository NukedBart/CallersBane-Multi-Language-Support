using System;
using System.Linq;

// Token: 0x020002E4 RID: 740
public class SaveLabDeckMessage : Message
{
	// Token: 0x060012E1 RID: 4833 RVA: 0x0000E258 File Offset: 0x0000C458
	public SaveLabDeckMessage(string deckName, long[] cards)
	{
		this.name = deckName;
		this.cards = Enumerable.ToArray<string>(Enumerable.Select<long, string>(cards, (long id) => id.ToString()));
	}

	// Token: 0x04000FB2 RID: 4018
	public string name;

	// Token: 0x04000FB3 RID: 4019
	public string[] cards;
}
