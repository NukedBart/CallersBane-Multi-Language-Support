using System;

// Token: 0x02000298 RID: 664
public class AdventureStartMessage : Message
{
	// Token: 0x06001246 RID: 4678 RVA: 0x0000DC9D File Offset: 0x0000BE9D
	public AdventureStartMessage(string mapName)
	{
		this.mapName = mapName;
	}

	// Token: 0x04000F27 RID: 3879
	public string mapName;
}
