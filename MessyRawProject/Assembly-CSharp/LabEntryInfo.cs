using System;

// Token: 0x020001B6 RID: 438
public class LabEntryInfo : DeckInfo
{
	// Token: 0x04000AC2 RID: 2754
	public string profileName;

	// Token: 0x04000AC3 RID: 2755
	public int wins;

	// Token: 0x04000AC4 RID: 2756
	public int losses;

	// Token: 0x04000AC5 RID: 2757
	public int lossesRemoveAfter;

	// Token: 0x04000AC6 RID: 2758
	public LabEntryInfo.LabEntryState state;

	// Token: 0x020001B7 RID: 439
	public enum LabEntryState
	{
		// Token: 0x04000AC8 RID: 2760
		CREATED,
		// Token: 0x04000AC9 RID: 2761
		ACTIVE,
		// Token: 0x04000ACA RID: 2762
		USED
	}
}
