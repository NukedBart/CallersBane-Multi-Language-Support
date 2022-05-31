using System;

// Token: 0x020000A3 RID: 163
public class FilterData
{
	// Token: 0x060005D2 RID: 1490 RVA: 0x000059F1 File Offset: 0x00003BF1
	public FilterData(BattleMode game, Card card)
	{
		this.game = game;
		this.card = card;
	}

	// Token: 0x060005D3 RID: 1491 RVA: 0x00005A07 File Offset: 0x00003C07
	public FilterData(BattleMode game, Unit unit) : this(game, unit, null)
	{
	}

	// Token: 0x060005D4 RID: 1492 RVA: 0x00005A12 File Offset: 0x00003C12
	public FilterData(BattleMode game, Unit unit, string ability)
	{
		this.game = game;
		this.unit = unit;
		this.ability = ability;
	}

	// Token: 0x060005D5 RID: 1493 RVA: 0x00005A2F File Offset: 0x00003C2F
	public FilterData setupFor(TilePosition pos)
	{
		this.tile = this.game.getTile(pos);
		return this;
	}

	// Token: 0x060005D6 RID: 1494 RVA: 0x00005A44 File Offset: 0x00003C44
	public Unit unitAt()
	{
		return this.game.getUnit(this.tile.tilePosition());
	}

	// Token: 0x0400041F RID: 1055
	public BattleMode game;

	// Token: 0x04000420 RID: 1056
	public Card card;

	// Token: 0x04000421 RID: 1057
	public string ability;

	// Token: 0x04000422 RID: 1058
	public Unit unit;

	// Token: 0x04000423 RID: 1059
	public Tile tile;
}
