using System;
using System.Collections.Generic;
using UnityEngine;

namespace NSCampaign
{
	// Token: 0x020000DE RID: 222
	public class Field
	{
		// Token: 0x06000781 RID: 1921 RVA: 0x00006AE0 File Offset: 0x00004CE0
		public Field(int width, int height)
		{
			this.width = width;
			this.height = height;
			this.tiles = new FieldTile[width, height];
		}

		// Token: 0x06000782 RID: 1922 RVA: 0x00006B03 File Offset: 0x00004D03
		public Field(FieldTile[,] tiles)
		{
			this.tiles = tiles;
			this.width = tiles.GetLength(0);
			this.height = tiles.GetLength(1);
		}

		// Token: 0x06000783 RID: 1923 RVA: 0x00042730 File Offset: 0x00040930
		public Field(int width, int height, int[] tileIds)
		{
			this.width = width;
			this.height = height;
			this.tiles = new FieldTile[width, height];
			int i = 0;
			int num = 0;
			while (i < height)
			{
				int j = 0;
				while (j < width)
				{
					this.tiles[j, i] = new FieldTile(tileIds[num]);
					j++;
					num++;
				}
				i++;
			}
		}

		// Token: 0x06000784 RID: 1924 RVA: 0x00006B2C File Offset: 0x00004D2C
		public bool isDiscovered(int x, int y)
		{
			return this.tiles[x, y] != null;
		}

		// Token: 0x06000785 RID: 1925 RVA: 0x00006B41 File Offset: 0x00004D41
		public FieldTile discoverTile(int x, int y, int id)
		{
			if (this.tiles[x, y] == null)
			{
				this.tiles[x, y] = new FieldTile(id);
				return this.tiles[x, y];
			}
			return null;
		}

		// Token: 0x06000786 RID: 1926 RVA: 0x000427A0 File Offset: 0x000409A0
		public Vector4 getLimits()
		{
			int num = 0;
			int num2 = this.width;
			int num3 = 0;
			int num4 = this.height;
			for (int i = 0; i < this.height; i++)
			{
				for (int j = 0; j < this.width; j++)
				{
					FieldTile fieldTile = this.tiles[j, i];
					if (fieldTile != null && fieldTile.id > 0)
					{
						if (j < num2)
						{
							num2 = j;
						}
						if (j > num)
						{
							num = j;
						}
						if (i < num4)
						{
							num4 = i;
						}
						if (i > num3)
						{
							num3 = i;
						}
					}
				}
			}
			return new Vector4((float)num2, (float)num, (float)num4, (float)num3);
		}

		// Token: 0x06000787 RID: 1927 RVA: 0x00006B77 File Offset: 0x00004D77
		public bool inRange(int x, int y)
		{
			return x >= 0 && x < this.width && y >= 0 && y < this.height;
		}

		// Token: 0x06000788 RID: 1928 RVA: 0x00006B9F File Offset: 0x00004D9F
		public List<FieldTile> getNeighbours(int x, int y)
		{
			return this.getNeighbours(x, y, 255);
		}

		// Token: 0x06000789 RID: 1929 RVA: 0x00042854 File Offset: 0x00040A54
		public List<FieldTile> getNeighbours(int x, int y, int nmask)
		{
			List<FieldTile> list = new List<FieldTile>();
			if ((nmask & 1) != 0 && this.inRange(x - 1, y))
			{
				list.Add(this.tiles[x - 1, y]);
			}
			if ((nmask & 2) != 0 && this.inRange(x + 1, y))
			{
				list.Add(this.tiles[x + 1, y]);
			}
			if ((nmask & 4) != 0 && this.inRange(x, y - 1))
			{
				list.Add(this.tiles[x, y - 1]);
			}
			if ((nmask & 8) != 0 && this.inRange(x, y + 1))
			{
				list.Add(this.tiles[x, y + 1]);
			}
			int num = ((y & 1) == 0) ? -1 : 1;
			if ((nmask & 16) != 0 && this.inRange(x + num, y - 1))
			{
				list.Add(this.tiles[x + num, y - 1]);
			}
			if ((nmask & 32) != 0 && this.inRange(x + num, y + 1))
			{
				list.Add(this.tiles[x + num, y + 1]);
			}
			return list;
		}

		// Token: 0x0600078A RID: 1930 RVA: 0x0004298C File Offset: 0x00040B8C
		public static Field CreateRandomField(int seed, int width, int height)
		{
			if (seed < 0)
			{
				seed = (int)DateTime.UtcNow.Ticks;
			}
			Random random = new Random(seed);
			Field field = new Field(width, height);
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					field.tiles[j, i] = new FieldTile(random.Next(1, 6));
				}
			}
			return field;
		}

		// Token: 0x04000599 RID: 1433
		public FieldTile[,] tiles;

		// Token: 0x0400059A RID: 1434
		public int width;

		// Token: 0x0400059B RID: 1435
		public int height;
	}
}
