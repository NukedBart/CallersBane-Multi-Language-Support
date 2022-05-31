using System;

// Token: 0x02000252 RID: 594
public class EMResourcesUpdate : EffectMessage
{
	// Token: 0x060011B8 RID: 4536 RVA: 0x0000D79F File Offset: 0x0000B99F
	public PlayerAssets getAssets(TileColor color)
	{
		if (color == TileColor.white)
		{
			return this.whiteAssets;
		}
		if (color == TileColor.black)
		{
			return this.blackAssets;
		}
		return null;
	}

	// Token: 0x04000E5F RID: 3679
	public PlayerAssets whiteAssets;

	// Token: 0x04000E60 RID: 3680
	public PlayerAssets blackAssets;
}
