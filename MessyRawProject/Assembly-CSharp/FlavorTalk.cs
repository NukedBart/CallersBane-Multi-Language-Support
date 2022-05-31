using System;

// Token: 0x020000CB RID: 203
internal class FlavorTalk
{
	// Token: 0x060006F9 RID: 1785 RVA: 0x0000666B File Offset: 0x0000486B
	private FlavorTalk(bool valid)
	{
		this.valid = valid;
	}

	// Token: 0x060006FA RID: 1786 RVA: 0x0000667A File Offset: 0x0000487A
	private FlavorTalk Greeting(string s)
	{
		this.greeting = s;
		return this;
	}

	// Token: 0x060006FB RID: 1787 RVA: 0x00006684 File Offset: 0x00004884
	private FlavorTalk Attack(string s)
	{
		this.attack = s;
		return this;
	}

	// Token: 0x1700005F RID: 95
	// (get) Token: 0x060006FC RID: 1788 RVA: 0x0000668E File Offset: 0x0000488E
	public static FlavorTalk empty
	{
		get
		{
			return new FlavorTalk(false);
		}
	}

	// Token: 0x060006FD RID: 1789 RVA: 0x0003F228 File Offset: 0x0003D428
	public static FlavorTalk get(CardType ct)
	{
		DateUtil.DateQuery utcToday = DateUtil.utcToday;
		if (ct.name == "Harvester" && (utcToday.isMonthAndDay(5, 5) || utcToday.isMonthAndDay(10, 12)))
		{
			return new FlavorTalk(true).Greeting("No hablo\nIngles").Attack("Como\nestaaa!");
		}
		return FlavorTalk.empty;
	}

	// Token: 0x060006FE RID: 1790 RVA: 0x00006696 File Offset: 0x00004896
	public static implicit operator bool(FlavorTalk ft)
	{
		return ft != null && ft.valid;
	}

	// Token: 0x04000509 RID: 1289
	public readonly bool valid;

	// Token: 0x0400050A RID: 1290
	public string greeting;

	// Token: 0x0400050B RID: 1291
	public string attack;
}
