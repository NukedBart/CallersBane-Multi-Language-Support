using System;

// Token: 0x02000410 RID: 1040
public static class TileColorUtil
{
	// Token: 0x06001710 RID: 5904 RVA: 0x00010940 File Offset: 0x0000EB40
	public static TileColor otherColor(this TileColor c)
	{
		if (c.isWhite())
		{
			return TileColor.black;
		}
		if (c.isBlack())
		{
			return TileColor.white;
		}
		return TileColor.unknown;
	}

	// Token: 0x06001711 RID: 5905 RVA: 0x0000D58A File Offset: 0x0000B78A
	public static bool isWhite(this TileColor c)
	{
		return c == TileColor.white;
	}

	// Token: 0x06001712 RID: 5906 RVA: 0x00005376 File Offset: 0x00003576
	public static bool isBlack(this TileColor c)
	{
		return c == TileColor.black;
	}
}
