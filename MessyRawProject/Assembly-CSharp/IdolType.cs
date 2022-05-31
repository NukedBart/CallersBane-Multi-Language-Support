using System;

// Token: 0x02000334 RID: 820
public class IdolType
{
	// Token: 0x0600136D RID: 4973 RVA: 0x0000E6D5 File Offset: 0x0000C8D5
	public string getFullHealthFilename()
	{
		return "BattleMode/Crystals/" + this.filename + "01";
	}

	// Token: 0x0400105D RID: 4189
	public short id;

	// Token: 0x0400105E RID: 4190
	public string name;

	// Token: 0x0400105F RID: 4191
	public IdolRarity type;

	// Token: 0x04001060 RID: 4192
	public string filename;
}
