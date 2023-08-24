using System;
using System.Collections.Generic;

// Token: 0x0200040E RID: 1038
public class TilePosition
{
	// Token: 0x060016FE RID: 5886 RVA: 0x00002DDA File Offset: 0x00000FDA
	public TilePosition()
	{
	}

	// Token: 0x060016FF RID: 5887 RVA: 0x000107E7 File Offset: 0x0000E9E7
	public TilePosition(TileColor color, string pos)
	{
		TilePosition.assertValidColor(color);
		this.color = color;
		this._parsePosition(pos);
	}

	// Token: 0x06001700 RID: 5888 RVA: 0x00010803 File Offset: 0x0000EA03
	public TilePosition(TileColor color, int row, int column)
	{
		this.row = row;
		this.column = column;
		TilePosition.assertValidColor(color);
		this.color = color;
	}

	// Token: 0x17000120 RID: 288
	// (get) Token: 0x06001701 RID: 5889 RVA: 0x0008F18C File Offset: 0x0008D38C
	// (set) Token: 0x06001702 RID: 5890 RVA: 0x00010826 File Offset: 0x0000EA26
	public string position
	{
		get
		{
			string text = this.color.ToString().Substring(0, 1).ToUpper() + ",";
			return string.Concat(new object[]
			{
				text,
				this.row,
				",",
				this.column
			});
		}
		set
		{
			this._parsePosition(value);
		}
	}

	// Token: 0x06001703 RID: 5891 RVA: 0x0001082F File Offset: 0x0000EA2F
	public TilePosition copy()
	{
		return new TilePosition(this.color, this.row, this.column);
	}

	// Token: 0x06001704 RID: 5892 RVA: 0x00010848 File Offset: 0x0000EA48
	public TilePosition copy(int row, int column)
	{
		return new TilePosition(this.color, row, column);
	}

	// Token: 0x06001705 RID: 5893 RVA: 0x00010857 File Offset: 0x0000EA57
	public TilePosition copyRow(int column)
	{
		return new TilePosition(this.color, this.row, column);
	}

	// Token: 0x06001706 RID: 5894 RVA: 0x0001086B File Offset: 0x0000EA6B
	public TilePosition copyColumn(int row)
	{
		return new TilePosition(this.color, row, this.column);
	}

	// Token: 0x06001707 RID: 5895 RVA: 0x0008F1F4 File Offset: 0x0008D3F4
	private void _parsePosition(string p)
	{
		string[] array = p.Split(new char[]
		{
			','
		});
		this.row = int.Parse(array[0]);
		this.column = int.Parse(array[1]);
	}

	// Token: 0x06001708 RID: 5896 RVA: 0x0001087F File Offset: 0x0000EA7F
	private static void assertValidColor(TileColor color)
	{
		if (color != TileColor.white && color != TileColor.black)
		{
			throw new Exception("color in TilePosition not valid! : " + color);
		}
	}

	// Token: 0x06001709 RID: 5897 RVA: 0x000108A4 File Offset: 0x0000EAA4
	public override string ToString()
	{
		return this.position;
	}

	// Token: 0x0600170A RID: 5898 RVA: 0x000108AC File Offset: 0x0000EAAC
	public override bool Equals(object o)
	{
		return o is TilePosition && this.Equals((TilePosition)o);
	}

	// Token: 0x0600170B RID: 5899 RVA: 0x000108C7 File Offset: 0x0000EAC7
	public bool Equals(TilePosition other)
	{
		return other != null && this.row == other.row && this.column == other.column && this.color == other.color;
	}

	// Token: 0x0600170C RID: 5900 RVA: 0x00010902 File Offset: 0x0000EB02
	public override int GetHashCode()
	{
		return (this.color.GetHashCode() << 16) + (this.row << 8) + this.column;
	}

	// Token: 0x0600170D RID: 5901 RVA: 0x00010927 File Offset: 0x0000EB27
	public static string[] positionStrings(List<TilePosition> tps)
	{
		return (tps != null) ? TilePosition.positionStrings(tps.ToArray()) : null;
	}

	// Token: 0x0600170E RID: 5902 RVA: 0x0008F230 File Offset: 0x0008D430
	public static string[] positionStrings(TilePosition[] tps)
	{
		if (tps == null)
		{
			return null;
		}
		string[] array = new string[tps.Length];
		for (int i = 0; i < tps.Length; i++)
		{
			array[i] = tps[i].position;
		}
		return array;
	}

	// Token: 0x0600170F RID: 5903 RVA: 0x0008F270 File Offset: 0x0008D470
	public static bool areAdjacent(TilePosition a, TilePosition b)
	{
		if (a.row == b.row)
		{
			return Math.Abs(a.column - b.column) == 1;
		}
		if (Math.Abs(a.row - b.row) > 1)
		{
			return false;
		}
		float num = (a.row % 2 != 0) ? 0.5f : -0.5f;
		return Math.Abs(num + (float)a.column - (float)b.column) <= 0.51f;
	}

	// Token: 0x04001478 RID: 5240
	public int row;

	// Token: 0x04001479 RID: 5241
	public int column;

	// Token: 0x0400147A RID: 5242
	public TileColor color;
}
