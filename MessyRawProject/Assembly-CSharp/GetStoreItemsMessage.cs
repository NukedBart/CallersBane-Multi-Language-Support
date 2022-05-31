using System;

// Token: 0x02000364 RID: 868
[Update(new Type[]
{
	typeof(ProfileDataInfoMessage)
})]
public class GetStoreItemsMessage : LobbyMessage
{
	// Token: 0x060013C3 RID: 5059 RVA: 0x000059E4 File Offset: 0x00003BE4
	public override bool shouldLog()
	{
		return false;
	}

	// Token: 0x040010EF RID: 4335
	public Items[] items;

	// Token: 0x040010F0 RID: 4336
	public int[] cardSellbackGold;
}
