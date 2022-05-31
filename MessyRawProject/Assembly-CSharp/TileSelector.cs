using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x0200009E RID: 158
internal class TileSelector
{
	// Token: 0x06000598 RID: 1432 RVA: 0x000058BA File Offset: 0x00003ABA
	public TileSelector() : this(null, TargetArea.UNDEFINED)
	{
	}

	// Token: 0x06000599 RID: 1433 RVA: 0x000058C4 File Offset: 0x00003AC4
	public TileSelector(List<TilePosition[]> groups, TargetArea targetArea)
	{
		this._available = ((groups == null) ? null : new List<TilePosition[]>(groups));
		this._targetArea = targetArea;
	}

	// Token: 0x0600059A RID: 1434 RVA: 0x000058F6 File Offset: 0x00003AF6
	public bool hasPickedAll()
	{
		return this._available.Count == 0;
	}

	// Token: 0x0600059B RID: 1435 RVA: 0x0003A268 File Offset: 0x00038468
	public bool pick(TilePosition tile)
	{
		if (this._available == null)
		{
			return false;
		}
		if (this._available.Count == 0)
		{
			Log.warning("No picks left!");
			return false;
		}
		if (!Enumerable.Contains<TilePosition>(this._available[0], tile))
		{
			Log.warning("Invalid tile!");
			return false;
		}
		this._picked.Add(tile);
		this._available.RemoveAt(0);
		return true;
	}

	// Token: 0x0600059C RID: 1436 RVA: 0x00005906 File Offset: 0x00003B06
	public List<TilePosition> getPickedTiles()
	{
		return this._picked;
	}

	// Token: 0x0600059D RID: 1437 RVA: 0x0000590E File Offset: 0x00003B0E
	public TilePosition[] getChoiceTiles()
	{
		if (this._available == null || this._available.Count == 0)
		{
			return new TilePosition[0];
		}
		return this._available[0];
	}

	// Token: 0x0600059E RID: 1438 RVA: 0x0003A2DC File Offset: 0x000384DC
	public bool isValid()
	{
		if (this._available == null)
		{
			return false;
		}
		if (this._picked.Count == 2)
		{
			TilePosition tilePosition = this._picked[0];
			TilePosition tilePosition2 = this._picked[1];
			if (tilePosition.Equals(tilePosition2))
			{
				return false;
			}
			if (this._targetArea == TargetArea.ROW_SIDE)
			{
				return tilePosition.row != tilePosition2.row;
			}
		}
		return true;
	}

	// Token: 0x0400041B RID: 1051
	private List<TilePosition[]> _available;

	// Token: 0x0400041C RID: 1052
	private List<TilePosition> _picked = new List<TilePosition>();

	// Token: 0x0400041D RID: 1053
	private TargetArea _targetArea;
}
