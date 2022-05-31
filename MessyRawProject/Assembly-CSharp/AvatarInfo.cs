using System;

// Token: 0x02000399 RID: 921
public class AvatarInfo
{
	// Token: 0x060014A2 RID: 5282 RVA: 0x0007FFD4 File Offset: 0x0007E1D4
	private AvatarInfo()
	{
		this.head = new AvatarInfo.Part(AvatarPartName.HEAD);
		this.body = new AvatarInfo.Part(AvatarPartName.BODY);
		this.leg = new AvatarInfo.Part(AvatarPartName.LEG);
		this.armBack = new AvatarInfo.Part(AvatarPartName.ARM_BACK);
		this.armFront = new AvatarInfo.Part(AvatarPartName.ARM_FRONT);
	}

	// Token: 0x060014A3 RID: 5283 RVA: 0x0000F286 File Offset: 0x0000D486
	public static AvatarInfo Builder()
	{
		return new AvatarInfo();
	}

	// Token: 0x060014A4 RID: 5284 RVA: 0x0000F28D File Offset: 0x0000D48D
	public static AvatarInfo CustomBuilder()
	{
		return AvatarInfo.Builder().ArmBack(GlobalMessageHandler.AvatarId_Blank(AvatarPartName.ARM_BACK)).ArmFront(GlobalMessageHandler.AvatarId_Blank(AvatarPartName.ARM_FRONT)).Body(GlobalMessageHandler.AvatarId_Blank(AvatarPartName.BODY)).Head(GlobalMessageHandler.AvatarId_Blank(AvatarPartName.HEAD)).Leg(GlobalMessageHandler.AvatarId_Blank(AvatarPartName.LEG));
	}

	// Token: 0x060014A5 RID: 5285 RVA: 0x0000F2CB File Offset: 0x0000D4CB
	public AvatarInfo Head(int id)
	{
		this.head.id = id;
		return this;
	}

	// Token: 0x060014A6 RID: 5286 RVA: 0x0000F2DA File Offset: 0x0000D4DA
	public AvatarInfo Body(int id)
	{
		this.body.id = id;
		return this;
	}

	// Token: 0x060014A7 RID: 5287 RVA: 0x0000F2E9 File Offset: 0x0000D4E9
	public AvatarInfo Leg(int id)
	{
		this.leg.id = id;
		return this;
	}

	// Token: 0x060014A8 RID: 5288 RVA: 0x0000F2F8 File Offset: 0x0000D4F8
	public AvatarInfo ArmBack(int id)
	{
		this.armBack.id = id;
		return this;
	}

	// Token: 0x060014A9 RID: 5289 RVA: 0x0000F307 File Offset: 0x0000D507
	public AvatarInfo ArmFront(int id)
	{
		this.armFront.id = id;
		return this;
	}

	// Token: 0x040011D7 RID: 4567
	public readonly AvatarInfo.Part head;

	// Token: 0x040011D8 RID: 4568
	public readonly AvatarInfo.Part body;

	// Token: 0x040011D9 RID: 4569
	public readonly AvatarInfo.Part leg;

	// Token: 0x040011DA RID: 4570
	public readonly AvatarInfo.Part armBack;

	// Token: 0x040011DB RID: 4571
	public readonly AvatarInfo.Part armFront;

	// Token: 0x0200039A RID: 922
	public class Part
	{
		// Token: 0x060014AA RID: 5290 RVA: 0x0000F316 File Offset: 0x0000D516
		public Part(AvatarPartName part) : this(part, null)
		{
		}

		// Token: 0x060014AB RID: 5291 RVA: 0x0000F320 File Offset: 0x0000D520
		public Part(AvatarPartName part, string set)
		{
			this.part = part;
			this.set = set;
		}

		// Token: 0x040011DC RID: 4572
		public readonly AvatarPartName part;

		// Token: 0x040011DD RID: 4573
		public readonly string set;

		// Token: 0x040011DE RID: 4574
		public int id;
	}
}
