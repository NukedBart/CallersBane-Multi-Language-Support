using System;
using System.Collections.Generic;

// Token: 0x020001D1 RID: 465
public class GraphDataSet<T>
{
	// Token: 0x170000BA RID: 186
	public int this[T key]
	{
		get
		{
			if (!this.set.ContainsKey(key))
			{
				this.set.Add(key, 0);
			}
			return this.set[key];
		}
		set
		{
			this.set[key] = value;
		}
	}

	// Token: 0x170000BB RID: 187
	// (get) Token: 0x06000EAF RID: 3759 RVA: 0x0000BBC6 File Offset: 0x00009DC6
	public Dictionary<T, int>.KeyCollection Keys
	{
		get
		{
			return this.set.Keys;
		}
	}

	// Token: 0x170000BC RID: 188
	// (get) Token: 0x06000EB0 RID: 3760 RVA: 0x0000BBD3 File Offset: 0x00009DD3
	public Dictionary<T, int>.ValueCollection Values
	{
		get
		{
			return this.set.Values;
		}
	}

	// Token: 0x06000EB1 RID: 3761 RVA: 0x0000BBE0 File Offset: 0x00009DE0
	public Dictionary<T, int>.Enumerator GetEnumerator()
	{
		return this.set.GetEnumerator();
	}

	// Token: 0x04000B60 RID: 2912
	private Dictionary<T, int> set = new Dictionary<T, int>();
}
