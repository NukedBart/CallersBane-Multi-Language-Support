using System;
using System.Collections.Generic;

// Token: 0x02000073 RID: 115
public class ListQueue<T> where T : class
{
	// Token: 0x17000048 RID: 72
	// (get) Token: 0x06000461 RID: 1121 RVA: 0x00004DBB File Offset: 0x00002FBB
	public int Count
	{
		get
		{
			return this._list.Count;
		}
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x00004DC8 File Offset: 0x00002FC8
	public int TotalCount()
	{
		return this.totalCount;
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x00004DD0 File Offset: 0x00002FD0
	private int UsedCount()
	{
		return this.TotalCount() - this.Count;
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x00004DDF File Offset: 0x00002FDF
	public float Progress()
	{
		return (float)((double)this.UsedCount() / (double)this.TotalCount());
	}

	// Token: 0x06000465 RID: 1125 RVA: 0x00004DF1 File Offset: 0x00002FF1
	public void Add(T e)
	{
		this.totalCount++;
		this._list.Add(e);
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x00004E0D File Offset: 0x0000300D
	public bool Remove(T e)
	{
		this.totalCount--;
		return this._list.Remove(e);
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x000317A0 File Offset: 0x0002F9A0
	public int LastIndexOf(T e)
	{
		for (int i = this._list.Count - 1; i >= 0; i--)
		{
			if (this._list[i] == e)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x000317EC File Offset: 0x0002F9EC
	public int LastIndexOf(Type type)
	{
		for (int i = this._list.Count - 1; i >= 0; i--)
		{
			T t = this._list[i];
			if (t.GetType() == type)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x0003183C File Offset: 0x0002FA3C
	public void InsertAfter(T e, Type afterAllOf)
	{
		this.totalCount++;
		int num = this.LastIndexOf(afterAllOf);
		if (ListQueue<T>.has(num))
		{
			this._list.Insert(num + 1, e);
		}
		else
		{
			this._list.Add(e);
		}
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x0003188C File Offset: 0x0002FA8C
	public void InsertBefore(T e, T following)
	{
		this.totalCount++;
		int num = this.LastIndexOf(following);
		if (ListQueue<T>.has(num))
		{
			this._list.Insert(num, e);
		}
		else
		{
			this._list.Add(e);
		}
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x00004E29 File Offset: 0x00003029
	public bool IsEmpty()
	{
		return this._list.Count == 0;
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x00004E39 File Offset: 0x00003039
	public T PeekFirst()
	{
		return (this._list.Count <= 0) ? ((T)((object)null)) : this._list[0];
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x000318D8 File Offset: 0x0002FAD8
	public T PopFirst()
	{
		T t = this.PeekFirst();
		if (t != null)
		{
			this._list.RemoveAt(0);
		}
		return t;
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x00004E63 File Offset: 0x00003063
	public void PushFirst(T e)
	{
		this.totalCount++;
		this._list.Insert(0, e);
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x00004E80 File Offset: 0x00003080
	private static bool has(int index)
	{
		return index >= 0;
	}

	// Token: 0x040002DD RID: 733
	protected List<T> _list = new List<T>();

	// Token: 0x040002DE RID: 734
	private int totalCount;
}
