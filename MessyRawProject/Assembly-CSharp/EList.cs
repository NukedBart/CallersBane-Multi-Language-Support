using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x0200042B RID: 1067
public class EList<T> : IEnumerable, IEnumerable<T>
{
	// Token: 0x060017A5 RID: 6053 RVA: 0x00010FF4 File Offset: 0x0000F1F4
	public EList()
	{
		this.list = new List<T>();
	}

	// Token: 0x060017A6 RID: 6054 RVA: 0x0001102A File Offset: 0x0000F22A
	private EList(List<T> list)
	{
		this.list = list;
	}

	// Token: 0x14000009 RID: 9
	// (add) Token: 0x060017A7 RID: 6055 RVA: 0x0001105C File Offset: 0x0000F25C
	// (remove) Token: 0x060017A8 RID: 6056 RVA: 0x00011075 File Offset: 0x0000F275
	public event EList<T>.EListDelegate onUpdate = delegate(EList<T> A_0)
	{
	};

	// Token: 0x060017A9 RID: 6057 RVA: 0x0001108E File Offset: 0x0000F28E
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.list.GetEnumerator();
	}

	// Token: 0x060017AA RID: 6058 RVA: 0x000110A0 File Offset: 0x0000F2A0
	public void Clear()
	{
		this.list.Clear();
		this._onUpdate();
	}

	// Token: 0x060017AB RID: 6059 RVA: 0x000110B3 File Offset: 0x0000F2B3
	public void Add(T item)
	{
		this.list.Add(item);
		this._onUpdate();
	}

	// Token: 0x060017AC RID: 6060 RVA: 0x000110C7 File Offset: 0x0000F2C7
	public void AddRange(IEnumerable<T> collection)
	{
		this.list.AddRange(collection);
		this._onUpdate();
	}

	// Token: 0x060017AD RID: 6061 RVA: 0x000110DB File Offset: 0x0000F2DB
	public void Sort(IComparer<T> comparer)
	{
		this.list.Sort(comparer);
		this._onUpdate();
	}

	// Token: 0x060017AE RID: 6062 RVA: 0x000110EF File Offset: 0x0000F2EF
	public void RemoveAt(int index)
	{
		this.list.RemoveAt(index);
		this._onUpdate();
	}

	// Token: 0x060017AF RID: 6063 RVA: 0x00011103 File Offset: 0x0000F303
	public List<T> toList()
	{
		return new List<T>(this.list);
	}

	// Token: 0x1700012C RID: 300
	// (get) Token: 0x060017B0 RID: 6064 RVA: 0x00011110 File Offset: 0x0000F310
	public int Count
	{
		get
		{
			return this.list.Count;
		}
	}

	// Token: 0x060017B1 RID: 6065 RVA: 0x0001111D File Offset: 0x0000F31D
	public static EList<T> fromMutableList(List<T> list)
	{
		return new EList<T>(list);
	}

	// Token: 0x060017B2 RID: 6066 RVA: 0x00011125 File Offset: 0x0000F325
	private void _onUpdate()
	{
		this.onUpdate(this);
	}

	// Token: 0x1700012D RID: 301
	public T this[int key]
	{
		get
		{
			return this.list[key];
		}
		set
		{
			this.list[key] = value;
			this._onUpdate();
		}
	}

	// Token: 0x060017B5 RID: 6069 RVA: 0x0001108E File Offset: 0x0000F28E
	public IEnumerator<T> GetEnumerator()
	{
		return this.list.GetEnumerator();
	}

	// Token: 0x040014E9 RID: 5353
	private List<T> list;

	// Token: 0x0200042C RID: 1068
	// (Invoke) Token: 0x060017B8 RID: 6072
	public delegate void EListDelegate(EList<T> e);
}
