using System;

// Token: 0x0200039E RID: 926
public class StepConfig<T>
{
	// Token: 0x060014B6 RID: 5302 RVA: 0x0000F3CA File Offset: 0x0000D5CA
	internal StepConfig(T[] ids) : this(ids, 0)
	{
	}

	// Token: 0x060014B7 RID: 5303 RVA: 0x0000F3D4 File Offset: 0x0000D5D4
	internal StepConfig(T[] ids, int index)
	{
		this.ids = ids;
		this.index = index;
	}

	// Token: 0x060014B9 RID: 5305 RVA: 0x00080244 File Offset: 0x0007E444
	public void next()
	{
		if (++this.index >= this.ids.Length)
		{
			this.index = 0;
		}
	}

	// Token: 0x060014BA RID: 5306 RVA: 0x00080278 File Offset: 0x0007E478
	public void prev()
	{
		if (--this.index < 0)
		{
			this.index = this.ids.Length - 1;
		}
	}

	// Token: 0x060014BB RID: 5307 RVA: 0x0000F3F6 File Offset: 0x0000D5F6
	public void random()
	{
		this.index = StepConfig<T>.rnd.Next(this.ids.Length);
	}

	// Token: 0x060014BC RID: 5308 RVA: 0x0000F410 File Offset: 0x0000D610
	public int size()
	{
		return this.ids.Length;
	}

	// Token: 0x060014BD RID: 5309 RVA: 0x0000F41A File Offset: 0x0000D61A
	public T getId(int i)
	{
		return this.ids[i];
	}

	// Token: 0x060014BE RID: 5310 RVA: 0x0000F428 File Offset: 0x0000D628
	public T getId()
	{
		return this.getId(this.index);
	}

	// Token: 0x060014BF RID: 5311 RVA: 0x0000F436 File Offset: 0x0000D636
	public int getIndex()
	{
		return this.index;
	}

	// Token: 0x060014C0 RID: 5312 RVA: 0x000802AC File Offset: 0x0007E4AC
	public int getIndex(T id)
	{
		for (int i = 0; i < this.ids.Length; i++)
		{
			if (this.ids[i].Equals(id))
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060014C1 RID: 5313 RVA: 0x0000F43E File Offset: 0x0000D63E
	public void setIndex(int i)
	{
		this.index = i % this.ids.Length;
	}

	// Token: 0x060014C2 RID: 5314 RVA: 0x000802F8 File Offset: 0x0007E4F8
	public bool setId(T id)
	{
		int num = this.getIndex(id);
		if (num >= 0)
		{
			this.setIndex(num);
		}
		return num >= 0;
	}

	// Token: 0x040011F1 RID: 4593
	protected T[] ids;

	// Token: 0x040011F2 RID: 4594
	protected int index;

	// Token: 0x040011F3 RID: 4595
	private static Random rnd = new Random();
}
