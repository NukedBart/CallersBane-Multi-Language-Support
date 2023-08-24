using System;
using System.Collections.Generic;

// Token: 0x0200009A RID: 154
public static class TargetAreaExtensions
{
	// Token: 0x06000574 RID: 1396 RVA: 0x00005754 File Offset: 0x00003954
	public static bool isTileTarget(this TargetArea t)
	{
		return t == TargetArea.UNDEFINED || TargetArea.TILE == t;
	}

	// Token: 0x06000575 RID: 1397 RVA: 0x0003962C File Offset: 0x0003782C
	private static bool isAny(this TargetArea t, params TargetArea[] types)
	{
		for (int i = 0; i < types.Length; i++)
		{
			if (types[i] == t)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000576 RID: 1398 RVA: 0x00005764 File Offset: 0x00003964
	private static bool isRadius(this TargetArea t)
	{
		return t.isAny(new TargetArea[]
		{
			TargetArea.RADIUS_3,
			TargetArea.RADIUS_4,
			TargetArea.RADIUS_7
		});
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x00005781 File Offset: 0x00003981
	private static bool isValid(int row, int column)
	{
		return column >= 0 && column < 3 && row >= 0 && row < 5;
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x0000579F File Offset: 0x0000399F
	private static void addValid(TilePosition p, IList<TilePosition> positions)
	{
		if (TargetAreaExtensions.isValid(p.row, p.column))
		{
			positions.Add(p);
		}
	}

	// Token: 0x06000579 RID: 1401 RVA: 0x0003965C File Offset: 0x0003785C
	public static Tile.SelectionType selectionType(this TargetArea t)
	{
		if (t.isRadius() || t.isAny(new TargetArea[]
		{
			TargetArea.SEQUENTIAL
		}))
		{
			return Tile.SelectionType.Target;
		}
		if (t.isAny(new TargetArea[]
		{
			TargetArea.FORWARD,
			TargetArea.ROW_SIDE,
			TargetArea.ROW_SIDE_IDOLS,
			TargetArea.ROW_FULL,
			TargetArea.ROW_FULL_IDOLS
		}))
		{
			return Tile.SelectionType.Path;
		}
		return Tile.SelectionType.None;
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x000396B8 File Offset: 0x000378B8
	public static List<TilePosition> getTargets(this TargetArea t, TilePosition p)
	{
		if (p.color == TileColor.unknown)
		{
			throw new ArgumentException("Unknown TileColor in " + t + ".getTargets()");
		}
		if (t.isAny(new TargetArea[]
		{
			TargetArea.RADIUS_3
		}))
		{
			throw new ArgumentException("TargetArea." + t + " not implemented!");
		}
		int row = p.row;
		int column = p.column;
		int column2 = (row % 2 != 0) ? (column - 1) : (column + 1);
		List<TilePosition> list = new List<TilePosition>();
		switch (t)
		{
		case TargetArea.FORWARD:
		case TargetArea.ROW_SIDE:
		case TargetArea.ROW_SIDE_IDOLS:
			for (int i = 0; i < 3; i++)
			{
				list.Add(p.copy(p.row, i));
			}
			return list;
		case TargetArea.RADIUS_4:
			TargetAreaExtensions.addValid(p, list);
			TargetAreaExtensions.addValid(p.copyRow(column2), list);
			TargetAreaExtensions.addValid(p.copyColumn(row - 1), list);
			TargetAreaExtensions.addValid(p.copyColumn(row + 1), list);
			return list;
		case TargetArea.RADIUS_7:
			TargetAreaExtensions.addValid(p, list);
			TargetAreaExtensions.addValid(p.copyRow(column - 1), list);
			TargetAreaExtensions.addValid(p.copyRow(column + 1), list);
			TargetAreaExtensions.addValid(p.copyColumn(row - 1), list);
			TargetAreaExtensions.addValid(p.copyColumn(row + 1), list);
			if (row % 2 == 0)
			{
				TargetAreaExtensions.addValid(p.copy(row - 1, column - 1), list);
				TargetAreaExtensions.addValid(p.copy(row + 1, column - 1), list);
			}
			else
			{
				TargetAreaExtensions.addValid(p.copy(row - 1, column + 1), list);
				TargetAreaExtensions.addValid(p.copy(row + 1, column + 1), list);
			}
			return list;
		case TargetArea.ROW_FULL_IDOLS:
		case TargetArea.ROW_FULL:
		{
			TileColor color = p.color.otherColor();
			for (int j = 0; j < 3; j++)
			{
				list.Add(new TilePosition(p.color, p.row, j));
				list.Add(new TilePosition(color, p.row, j));
			}
			return list;
		}
		}
		TargetAreaExtensions.addValid(p, list);
		return list;
	}

	// Token: 0x040003F6 RID: 1014
	private const int Rows = 5;

	// Token: 0x040003F7 RID: 1015
	private const int Columns = 3;
}
