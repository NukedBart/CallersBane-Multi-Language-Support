using System;

// Token: 0x020003FB RID: 1019
public static class ItemTypeExtension
{
	// Token: 0x06001663 RID: 5731 RVA: 0x000102CC File Offset: 0x0000E4CC
	public static bool isCard(this Items.Type type)
	{
		return type >= Items.Type.CARD_FACE_DOWN && type <= Items.Type.CARD_ENERGY;
	}
}
