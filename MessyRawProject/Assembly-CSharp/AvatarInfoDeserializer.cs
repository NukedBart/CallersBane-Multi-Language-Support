using System;

// Token: 0x0200039B RID: 923
public class AvatarInfoDeserializer
{
	// Token: 0x060014AD RID: 5293 RVA: 0x0000F361 File Offset: 0x0000D561
	public AvatarInfo getAvatarInfo()
	{
		return AvatarInfo.Builder().Head(this.head).Body(this.body).Leg(this.leg).ArmBack(this.armBack).ArmFront(this.armFront);
	}

	// Token: 0x040011DF RID: 4575
	public int head = -1;

	// Token: 0x040011E0 RID: 4576
	public int body = -1;

	// Token: 0x040011E1 RID: 4577
	public int leg = -1;

	// Token: 0x040011E2 RID: 4578
	public int armBack = -1;

	// Token: 0x040011E3 RID: 4579
	public int armFront = -1;

	// Token: 0x040011E4 RID: 4580
	public int profileId;

	// Token: 0x040011E5 RID: 4581
	public string image;

	// Token: 0x040011E6 RID: 4582
	public string name;

	// Token: 0x040011E7 RID: 4583
	public string description;
}
