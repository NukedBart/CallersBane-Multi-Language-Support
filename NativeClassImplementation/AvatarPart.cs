using System;

// Token: 0x0200032F RID: 815
public class AvatarPart
{
	// Token: 0x06001364 RID: 4964 RVA: 0x00078B1C File Offset: 0x00076D1C
	public AvatarPart(AvatarPartName type, string filename, string set)
	{
		this.id = (int)(-(int)(1 + Math.Abs(set.GetHashCode() % 19773) + type));
		this.part = type;
		this.filename = filename;
		this.set = set;
	}

	// Token: 0x06001365 RID: 4965 RVA: 0x0000E658 File Offset: 0x0000C858
	public string getFullFilename()
	{
		return this.set + "/" + this.filename;
	}

	// Token: 0x06001366 RID: 4966 RVA: 0x00078B74 File Offset: 0x00076D74
	public string getSuffix()
	{
		int num = this.filename.LastIndexOf("_");
		if (num >= 0)
		{
			return this.filename.Substring(num + 1).ToLower();
		}
		return "none";
	}

	// Token: 0x06001367 RID: 4967 RVA: 0x00078BB4 File Offset: 0x00076DB4
	public string getPartNameString()
	{
		switch (this.part)
		{
		case AvatarPartName.ARM_BACK:
			return "Arm";
		case AvatarPartName.LEG:
			return "Leg";
		case AvatarPartName.BODY:
			return "Body";
		case AvatarPartName.HEAD:
			return "Head";
		case AvatarPartName.ARM_FRONT:
			return "Arm";
		default:
			return "Invalid";
		}
	}

	// Token: 0x06001368 RID: 4968 RVA: 0x0000E670 File Offset: 0x0000C870
	public string getSetString()
	{
		if (this.set == "MALE_1")
		{
			return "male";
		}
		if (this.set == "FEMALE_1")
		{
			return "female";
		}
		return "other";
	}

	// Token: 0x0400104B RID: 4171
	public int id;

	// Token: 0x0400104C RID: 4172
	public AvatarPartRarity type;

	// Token: 0x0400104D RID: 4173
	public AvatarPartName part = AvatarPartName.INVALID;

	// Token: 0x0400104E RID: 4174
	public string filename;

	// Token: 0x0400104F RID: 4175
	public string set = AvatarPartTypeManager.getDefaultSet();
}
