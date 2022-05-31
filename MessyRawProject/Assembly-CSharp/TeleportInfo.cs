using System;

// Token: 0x02000265 RID: 613
public class TeleportInfo
{
	// Token: 0x060011E9 RID: 4585 RVA: 0x00002DDA File Offset: 0x00000FDA
	public TeleportInfo()
	{
	}

	// Token: 0x060011EA RID: 4586 RVA: 0x0000D94F File Offset: 0x0000BB4F
	public TeleportInfo(TilePosition from, TilePosition to)
	{
		this.from = from;
		this.to = to;
	}

	// Token: 0x060011EB RID: 4587 RVA: 0x0000D965 File Offset: 0x0000BB65
	public TeleportInfo(Unit unit, TilePosition to) : this(unit.getTilePosition(), to)
	{
		this.unit = unit;
	}

	// Token: 0x04000E99 RID: 3737
	public TilePosition from;

	// Token: 0x04000E9A RID: 3738
	public TilePosition to;

	// Token: 0x04000E9B RID: 3739
	public Unit unit;
}
