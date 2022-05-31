using System;
using System.Collections.Generic;

// Token: 0x020002A3 RID: 675
public class DeckListMessage : LobbyMessage
{
	// Token: 0x0600125E RID: 4702 RVA: 0x00077D68 File Offset: 0x00075F68
	public List<DeckInfo> GetAllDecks()
	{
		List<DeckInfo> list = new List<DeckInfo>(this.decks);
		if (this.labEntryInfo != null)
		{
			list.Add(this.labEntryInfo);
		}
		return list;
	}

	// Token: 0x04000F39 RID: 3897
	public DeckInfo[] decks;

	// Token: 0x04000F3A RID: 3898
	public LabEntryInfo labEntryInfo;
}
