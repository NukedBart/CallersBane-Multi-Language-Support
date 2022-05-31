using System;

// Token: 0x020002E2 RID: 738
public class DeleteLabDeckMessage : Message
{
	// Token: 0x060012DF RID: 4831 RVA: 0x0000E249 File Offset: 0x0000C449
	public DeleteLabDeckMessage(string deckName)
	{
		this.name = deckName;
	}

	// Token: 0x04000FB0 RID: 4016
	public string name;
}
