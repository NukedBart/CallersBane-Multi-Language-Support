using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000461 RID: 1121
internal class SpectatableGameInfoSorter : IComparer<SpectatableGameInfo>
{
	// Token: 0x06001901 RID: 6401 RVA: 0x00012349 File Offset: 0x00010549
	public SpectatableGameInfoSorter addSorter(Comparison<SpectatableGameInfo> c)
	{
		this.sorters.Add(c);
		return this;
	}

	// Token: 0x06001902 RID: 6402 RVA: 0x00012358 File Offset: 0x00010558
	public void clear()
	{
		this.sorters.Clear();
	}

	// Token: 0x06001903 RID: 6403 RVA: 0x00093DBC File Offset: 0x00091FBC
	public int Compare(SpectatableGameInfo a, SpectatableGameInfo b)
	{
		foreach (Comparison<SpectatableGameInfo> comparison in this.sorters)
		{
			int num = comparison.Invoke(a, b);
			if (num != 0)
			{
				return (!this.isReversed) ? num : (-num);
			}
		}
		return SpectatableGameInfoSorter.ByTimestamp.Invoke(a, b);
	}

	// Token: 0x06001904 RID: 6404 RVA: 0x00012365 File Offset: 0x00010565
	public void reverse(bool reverse)
	{
		this.isReversed = reverse;
	}

	// Token: 0x06001905 RID: 6405 RVA: 0x0001236E File Offset: 0x0001056E
	public SpectatableGameInfoSorter byRating()
	{
		return this.addSorter(SpectatableGameInfoSorter.ByRating);
	}

	// Token: 0x06001906 RID: 6406 RVA: 0x0001237B File Offset: 0x0001057B
	public SpectatableGameInfoSorter byPopularity()
	{
		return this.addSorter(SpectatableGameInfoSorter.ByPopularity);
	}

	// Token: 0x06001907 RID: 6407 RVA: 0x00012388 File Offset: 0x00010588
	public SpectatableGameInfoSorter byTimestamp()
	{
		return this.addSorter(SpectatableGameInfoSorter.ByTimestamp);
	}

	// Token: 0x06001908 RID: 6408 RVA: 0x00012395 File Offset: 0x00010595
	public SpectatableGameInfoSorter byStarted()
	{
		return this.addSorter(SpectatableGameInfoSorter.ByStarted);
	}

	// Token: 0x04001569 RID: 5481
	private List<Comparison<SpectatableGameInfo>> sorters = new List<Comparison<SpectatableGameInfo>>();

	// Token: 0x0400156A RID: 5482
	private bool isReversed;

	// Token: 0x0400156B RID: 5483
	public static Comparison<SpectatableGameInfo> ByRating = delegate(SpectatableGameInfo a, SpectatableGameInfo b)
	{
		int rating = a.whitePlayer.rating;
		int rating2 = a.blackPlayer.rating;
		int rating3 = b.whitePlayer.rating;
		int rating4 = b.blackPlayer.rating;
		return Mth.firstNonZero(new int[]
		{
			rating3 * rating4 - rating * rating2,
			Mathf.Max(rating3, rating4) - Mathf.Max(rating, rating2)
		});
	};

	// Token: 0x0400156C RID: 5484
	public static Comparison<SpectatableGameInfo> ByPopularity = (SpectatableGameInfo a, SpectatableGameInfo b) => -a.spectators.Value.CompareTo(b.spectators.Value);

	// Token: 0x0400156D RID: 5485
	public static Comparison<SpectatableGameInfo> ByTimestamp = (SpectatableGameInfo a, SpectatableGameInfo b) => -a.gameId.CompareTo(b.gameId);

	// Token: 0x0400156E RID: 5486
	public static Comparison<SpectatableGameInfo> ByStarted = (SpectatableGameInfo a, SpectatableGameInfo b) => ((!b.started.Value) ? 0 : 1) - ((!a.started.Value) ? 0 : 1);
}
