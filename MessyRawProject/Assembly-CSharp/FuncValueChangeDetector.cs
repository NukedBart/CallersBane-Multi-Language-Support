using System;

// Token: 0x02000423 RID: 1059
public class FuncValueChangeDetector<T> : IsValueChanged
{
	// Token: 0x0600177C RID: 6012 RVA: 0x00010E80 File Offset: 0x0000F080
	public FuncValueChangeDetector(Func<T> func)
	{
		this.getValue = func;
	}

	// Token: 0x0600177D RID: 6013 RVA: 0x00090CD8 File Offset: 0x0008EED8
	public bool IsChanged()
	{
		T t = this.getValue.Invoke();
		bool result = !t.Equals(this.old);
		this.old = t;
		return result;
	}

	// Token: 0x040014E3 RID: 5347
	private Func<T> getValue;

	// Token: 0x040014E4 RID: 5348
	private T old;
}
