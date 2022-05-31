using System;
using System.Collections.Generic;

// Token: 0x020001D0 RID: 464
public class GraphData<T>
{
	// Token: 0x170000B7 RID: 183
	public GraphDataSet<T> this[int key]
	{
		get
		{
			if (!this.data.ContainsKey(key))
			{
				this.data[key] = new GraphDataSet<T>();
			}
			return this.data[key];
		}
		set
		{
			this.data[key] = value;
		}
	}

	// Token: 0x170000B8 RID: 184
	// (get) Token: 0x06000EA8 RID: 3752 RVA: 0x0000BB43 File Offset: 0x00009D43
	public Dictionary<int, GraphDataSet<T>>.KeyCollection Keys
	{
		get
		{
			return this.data.Keys;
		}
	}

	// Token: 0x170000B9 RID: 185
	// (get) Token: 0x06000EA9 RID: 3753 RVA: 0x0000BB50 File Offset: 0x00009D50
	public Dictionary<int, GraphDataSet<T>>.ValueCollection Values
	{
		get
		{
			return this.data.Values;
		}
	}

	// Token: 0x06000EAA RID: 3754 RVA: 0x0000BB5D File Offset: 0x00009D5D
	public Dictionary<int, GraphDataSet<T>>.Enumerator GetEnumerator()
	{
		return this.data.GetEnumerator();
	}

	// Token: 0x06000EAB RID: 3755 RVA: 0x0000BB6A File Offset: 0x00009D6A
	public bool ContainsKey(int key)
	{
		return this.data.ContainsKey(key);
	}

	// Token: 0x04000B5F RID: 2911
	private Dictionary<int, GraphDataSet<T>> data = new Dictionary<int, GraphDataSet<T>>();
}
