using System;
using System.Collections.Generic;

// Token: 0x020001A7 RID: 423
internal class FrontCardList
{
	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x06000D4C RID: 3404 RVA: 0x0000AA04 File Offset: 0x00008C04
	public int Count
	{
		get
		{
			return this.size;
		}
	}

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x06000D4D RID: 3405 RVA: 0x0000AA0C File Offset: 0x00008C0C
	public int CountTotal
	{
		get
		{
			return this.list.Count;
		}
	}

	// Token: 0x170000A8 RID: 168
	public DeckCard this[int i]
	{
		get
		{
			DeckCard result;
			try
			{
				result = this.list[i];
			}
			catch (Exception)
			{
				Log.error(string.Concat(new object[]
				{
					"size: ",
					this.size,
					", ",
					this.CountTotal
				}));
				result = null;
			}
			return result;
		}
	}

	// Token: 0x06000D4F RID: 3407 RVA: 0x0000AA19 File Offset: 0x00008C19
	public void RemoveAt(int i)
	{
		if (!this.isSameTypeAsNeighbour(this.list[i].type, i))
		{
			this.size--;
		}
		this.list.RemoveAt(i);
	}

	// Token: 0x06000D50 RID: 3408 RVA: 0x0005D54C File Offset: 0x0005B74C
	private int getIndex(DeckCard c)
	{
		for (int i = 0; i < this.list.Count; i++)
		{
			if (c == this.list[i])
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000D51 RID: 3409 RVA: 0x0005D58C File Offset: 0x0005B78C
	public bool Remove(DeckCard c)
	{
		int index = this.getIndex(c);
		if (index < 0)
		{
			return false;
		}
		if (!this.isSameTypeAsNeighbour(c.type, index))
		{
			this.size--;
		}
		this.list.RemoveAt(index);
		return true;
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x0000AA52 File Offset: 0x00008C52
	public void Add(DeckCard c)
	{
		this.list.Add(c);
		if (!this.isSameTypeAsNeighbour(c.type, this.list.Count - 1))
		{
			this.size++;
		}
	}

	// Token: 0x06000D53 RID: 3411 RVA: 0x0000AA8C File Offset: 0x00008C8C
	public void Insert(int i, DeckCard c)
	{
		this.list.Insert(i, c);
		if (!this.isSameTypeAsNeighbour(c.type, i))
		{
			this.size++;
		}
	}

	// Token: 0x06000D54 RID: 3412 RVA: 0x0000AABB File Offset: 0x00008CBB
	public List<DeckCard> RemoveTypeLeft()
	{
		if (this.list.Count == 0)
		{
			return null;
		}
		return this.RemoveTypeLeft(this.list[0].card.getCardInfo().getType());
	}

	// Token: 0x06000D55 RID: 3413 RVA: 0x0005D5D8 File Offset: 0x0005B7D8
	public List<DeckCard> RemoveTypeLeft(int type)
	{
		List<DeckCard> list = new List<DeckCard>();
		while (this.list.Count > 0 && type == this.list[0].card.getCardInfo().getType())
		{
			list.Add(this.list[0]);
			this.list.RemoveAt(0);
		}
		if (list.Count > 0)
		{
			this.size--;
		}
		return list;
	}

	// Token: 0x06000D56 RID: 3414 RVA: 0x0005D65C File Offset: 0x0005B85C
	public List<DeckCard> RemoveTypeRight()
	{
		if (this.list.Count == 0)
		{
			return null;
		}
		return this.RemoveTypeRight(this.list[this.list.Count - 1].card.getCardInfo().getType());
	}

	// Token: 0x06000D57 RID: 3415 RVA: 0x0005D6A8 File Offset: 0x0005B8A8
	public List<DeckCard> RemoveTypeRight(int type)
	{
		List<DeckCard> list = new List<DeckCard>();
		while (this.list.Count > 0 && type == this.list[this.list.Count - 1].card.getCardInfo().getType())
		{
			list.Add(this.list[this.list.Count - 1]);
			this.list.RemoveAt(this.list.Count - 1);
		}
		if (list.Count > 0)
		{
			this.size--;
		}
		return list;
	}

	// Token: 0x06000D58 RID: 3416 RVA: 0x0005D750 File Offset: 0x0005B950
	public void listTypes()
	{
		for (int i = 0; i < this.list.Count; i++)
		{
			Log.info(string.Concat(new object[]
			{
				"i: ",
				i,
				"; ",
				this.list[i].type
			}));
		}
	}

	// Token: 0x06000D59 RID: 3417 RVA: 0x0005D7BC File Offset: 0x0005B9BC
	public int getTypeIndex(int type)
	{
		int num = -1;
		int i = 0;
		int num2 = 0;
		while (i < this.list.Count)
		{
			int type2 = this.list[i].card.getCardInfo().getType();
			if (type2 != num)
			{
				if (type2 == type)
				{
					return num2;
				}
				num = type2;
				num2++;
			}
			i++;
		}
		return -1;
	}

	// Token: 0x06000D5A RID: 3418 RVA: 0x0005D81C File Offset: 0x0005BA1C
	public int getCardIndexForTypeIndex(int type)
	{
		int num = -1;
		int i = 0;
		int num2 = 0;
		while (i < this.list.Count)
		{
			int type2 = this.list[i].type;
			if (type2 != num)
			{
				if (num2 == type)
				{
					return i;
				}
				num = type2;
				num2++;
			}
			i++;
		}
		return -1;
	}

	// Token: 0x06000D5B RID: 3419 RVA: 0x0005D874 File Offset: 0x0005BA74
	private bool isSameTypeAsNeighbour(int type, int i)
	{
		return (i - 1 >= 0 && this.list[i - 1].type == type) || (i + 1 < this.list.Count && this.list[i + 1].type == type);
	}

	// Token: 0x06000D5C RID: 3420 RVA: 0x0000AAF0 File Offset: 0x00008CF0
	public List<DeckCard> getList()
	{
		return new List<DeckCard>(this.list);
	}

	// Token: 0x04000A55 RID: 2645
	private int size;

	// Token: 0x04000A56 RID: 2646
	private List<DeckCard> list = new List<DeckCard>();
}
