using System;

// Token: 0x020003F9 RID: 1017
public class Items
{
	// Token: 0x06001662 RID: 5730 RVA: 0x000102BF File Offset: 0x0000E4BF
	public AvatarInfo getAvatar()
	{
		return this.avatar.getAvatarInfo();
	}

	// Token: 0x04001398 RID: 5016
	public int itemId;

	// Token: 0x04001399 RID: 5017
	public string itemName;

	// Token: 0x0400139A RID: 5018
	public Items.Type itemType;

	// Token: 0x0400139B RID: 5019
	public int costGold;

	// Token: 0x0400139C RID: 5020
	public int costShards;

	// Token: 0x0400139D RID: 5021
	public bool isPurchased;

	// Token: 0x0400139E RID: 5022
	public bool isPublic;

	// Token: 0x0400139F RID: 5023
	public string description;

	// Token: 0x040013A0 RID: 5024
	public string expires;

	// Token: 0x040013A1 RID: 5025
	public string deckName;

	// Token: 0x040013A2 RID: 5026
	public string deckDescription;

	// Token: 0x040013A3 RID: 5027
	public int cardTypeId;

	// Token: 0x040013A4 RID: 5028
	public long[] cardTypeIds = new long[0];

	// Token: 0x040013A5 RID: 5029
	public AvatarInfoDeserializer avatar;

	// Token: 0x040013A6 RID: 5030
	public short avatarPart;

	// Token: 0x040013A7 RID: 5031
	public short idolId;

	// Token: 0x020003FA RID: 1018
	public enum Type
	{
		// Token: 0x040013A9 RID: 5033
		CARD_FACE_DOWN,
		// Token: 0x040013AA RID: 5034
		CARD_FACE_UP,
		// Token: 0x040013AB RID: 5035
		CARD_DECAY,
		// Token: 0x040013AC RID: 5036
		CARD_ORDER,
		// Token: 0x040013AD RID: 5037
		CARD_GROWTH,
		// Token: 0x040013AE RID: 5038
		CARD_ENERGY,
		// Token: 0x040013AF RID: 5039
		CARD_PACK,
		// Token: 0x040013B0 RID: 5040
		CARD_PACK_NEW,
		// Token: 0x040013B1 RID: 5041
		DECK,
		// Token: 0x040013B2 RID: 5042
		IDOL,
		// Token: 0x040013B3 RID: 5043
		BUNDLE,
		// Token: 0x040013B4 RID: 5044
		AVATAR,
		// Token: 0x040013B5 RID: 5045
		AVATAR_OUTFIT
	}
}
