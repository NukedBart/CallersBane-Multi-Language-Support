using System;

// Token: 0x020001BD RID: 445
public class EnchantmentInfo
{
	// Token: 0x06000E00 RID: 3584 RVA: 0x0000B170 File Offset: 0x00009370
	public string getUntaggedDescription()
	{
		if (this._untaggedDescription == null)
		{
			this._untaggedDescription = Keywords.clearFromTags(this.description);
		}
		return this._untaggedDescription;
	}

	// Token: 0x04000AE8 RID: 2792
	public string name;

	// Token: 0x04000AE9 RID: 2793
	public string description;

	// Token: 0x04000AEA RID: 2794
	public EnchantmentType type;

	// Token: 0x04000AEB RID: 2795
	private string _untaggedDescription;
}
