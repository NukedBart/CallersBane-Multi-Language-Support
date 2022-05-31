using System;
using System.Collections.Generic;

// Token: 0x020001A4 RID: 420
public class DeckSorter : IComparer<Card>, IComparer<DeckCard>
{
	// Token: 0x06000D26 RID: 3366 RVA: 0x0000A7FA File Offset: 0x000089FA
	public DeckSorter addSorter(Comparison<Card> c)
	{
		this.sorters.Add(c);
		this.dirty = true;
		return this;
	}

	// Token: 0x06000D27 RID: 3367 RVA: 0x0000A810 File Offset: 0x00008A10
	public void clear()
	{
		this.dirty = true;
		this.sorters.Clear();
	}

	// Token: 0x06000D28 RID: 3368 RVA: 0x0005D234 File Offset: 0x0005B434
	public int Compare(DeckCard a, DeckCard b)
	{
		if (a == null || a.card == null)
		{
			return -1;
		}
		if (b == null || b.card == null)
		{
			return 1;
		}
		return this.Compare(a.card.getCardInfo(), b.card.getCardInfo());
	}

	// Token: 0x06000D29 RID: 3369 RVA: 0x0005D284 File Offset: 0x0005B484
	public int Compare(Card a, Card b)
	{
		if (this.dirty)
		{
			this.rebuildCache();
		}
		foreach (Comparison<Card> comparison in this.cachedSorters)
		{
			int num = comparison.Invoke(a, b);
			if (num != 0)
			{
				return (!this.isReversed) ? num : (-num);
			}
		}
		return 0;
	}

	// Token: 0x06000D2A RID: 3370 RVA: 0x0005D314 File Offset: 0x0005B514
	private void rebuildCache()
	{
		this.cachedSorters.Clear();
		this.cachedSorters.AddRange(this.sorters);
		if (!this.cachedSorters.Contains(DeckSorter.ById))
		{
			this.cachedSorters.Add(DeckSorter.ById);
		}
		this.dirty = false;
	}

	// Token: 0x06000D2B RID: 3371 RVA: 0x0000A824 File Offset: 0x00008A24
	public void reverse(bool reverse)
	{
		this.isReversed = reverse;
	}

	// Token: 0x06000D2C RID: 3372 RVA: 0x0000A82D File Offset: 0x00008A2D
	public DeckSorter byName()
	{
		return this.addSorter(DeckSorter.ByName);
	}

	// Token: 0x06000D2D RID: 3373 RVA: 0x0000A83A File Offset: 0x00008A3A
	public DeckSorter byNameDesc()
	{
		return this.addSorter(DeckSorter.ByNameDesc);
	}

	// Token: 0x06000D2E RID: 3374 RVA: 0x0000A847 File Offset: 0x00008A47
	public DeckSorter byTradable()
	{
		return this.addSorter(DeckSorter.ByTradable);
	}

	// Token: 0x06000D2F RID: 3375 RVA: 0x0000A854 File Offset: 0x00008A54
	public DeckSorter byLevel()
	{
		return this.addSorter(DeckSorter.ByLevel);
	}

	// Token: 0x06000D30 RID: 3376 RVA: 0x0000A861 File Offset: 0x00008A61
	public DeckSorter byLevelAscending()
	{
		return this.addSorter(DeckSorter.ByLevelAscending);
	}

	// Token: 0x06000D31 RID: 3377 RVA: 0x0000A86E File Offset: 0x00008A6E
	public DeckSorter byId()
	{
		return this.addSorter(DeckSorter.ById);
	}

	// Token: 0x06000D32 RID: 3378 RVA: 0x0000A87B File Offset: 0x00008A7B
	public DeckSorter byResourceCount()
	{
		return this.addSorter(DeckSorter.ByResourceCount);
	}

	// Token: 0x06000D33 RID: 3379 RVA: 0x0000A888 File Offset: 0x00008A88
	public DeckSorter byResourceCountDesc()
	{
		return this.addSorter(DeckSorter.ByResourceCountDesc);
	}

	// Token: 0x06000D34 RID: 3380 RVA: 0x0000A895 File Offset: 0x00008A95
	public DeckSorter byType()
	{
		return this.addSorter(DeckSorter.ByType);
	}

	// Token: 0x06000D35 RID: 3381 RVA: 0x0000A8A2 File Offset: 0x00008AA2
	public DeckSorter byColor()
	{
		return this.addSorter(DeckSorter.ByColor);
	}

	// Token: 0x06000D36 RID: 3382 RVA: 0x0000A8AF File Offset: 0x00008AAF
	public DeckSorter byData<T>() where T : IComparable<T>
	{
		return this.addSorter(delegate(Card a, Card b)
		{
			T t = a.dataAs<T>();
			return t.CompareTo(b.dataAs<T>());
		});
	}

	// Token: 0x06000D37 RID: 3383 RVA: 0x0000A8C3 File Offset: 0x00008AC3
	public DeckSorter byDataDesc<T>() where T : IComparable<T>
	{
		return this.addSorter(delegate(Card a, Card b)
		{
			T t = b.dataAs<T>();
			return t.CompareTo(a.dataAs<T>());
		});
	}

	// Token: 0x04000A3A RID: 2618
	private List<Comparison<Card>> sorters = new List<Comparison<Card>>();

	// Token: 0x04000A3B RID: 2619
	private List<Comparison<Card>> cachedSorters = new List<Comparison<Card>>();

	// Token: 0x04000A3C RID: 2620
	private bool isReversed;

	// Token: 0x04000A3D RID: 2621
	private bool dirty = true;

	// Token: 0x04000A3E RID: 2622
	public static Comparison<Card> ByName = (Card a, Card b) => a.getName().CompareTo(b.getName());

	// Token: 0x04000A3F RID: 2623
	public static Comparison<Card> ByNameDesc = (Card a, Card b) => b.getName().CompareTo(a.getName());

	// Token: 0x04000A40 RID: 2624
	public static Comparison<Card> ByTradable = (Card a, Card b) => -a.tradable.CompareTo(b.tradable);

	// Token: 0x04000A41 RID: 2625
	public static Comparison<Card> ByLevel = (Card a, Card b) => -a.level.CompareTo(b.level);

	// Token: 0x04000A42 RID: 2626
	public static Comparison<Card> ByLevelAscending = (Card a, Card b) => a.level.CompareTo(b.level);

	// Token: 0x04000A43 RID: 2627
	public static Comparison<Card> ById = (Card a, Card b) => a.getId().CompareTo(b.getId());

	// Token: 0x04000A44 RID: 2628
	public static Comparison<Card> ByResourceCount = (Card a, Card b) => a.getCostTotal().CompareTo(b.getCostTotal());

	// Token: 0x04000A45 RID: 2629
	public static Comparison<Card> ByResourceCountDesc = (Card a, Card b) => b.getCostTotal().CompareTo(a.getCostTotal());

	// Token: 0x04000A46 RID: 2630
	public static Comparison<Card> ByType = delegate(Card a, Card b)
	{
		int num = a.getPieceKind().CompareTo(b.getPieceKind());
		if (num != 0)
		{
			return num;
		}
		return a.getPieceKind().CompareTo(b.getPieceKind());
	};

	// Token: 0x04000A47 RID: 2631
	public static Comparison<Card> ByColor = (Card a, Card b) => a.getCardType().getResource().CompareTo(b.getCardType().getResource());
}
