using System;
using System.Collections.Generic;

// Token: 0x020001A5 RID: 421
internal class DepletingMultiMapQuery<K, T>
{
	// Token: 0x06000D45 RID: 3397 RVA: 0x0000A972 File Offset: 0x00008B72
	public void Add(K id, T obj)
	{
		if (!this.d.ContainsKey(id))
		{
			this.d.Add(id, new DepletingMultiMapQuery<K, T>.Item());
		}
		this.d[id].list.Add(obj);
	}

	// Token: 0x06000D46 RID: 3398 RVA: 0x0000A9AD File Offset: 0x00008BAD
	public T getNext(K id)
	{
		return this.d[id].getNext();
	}

	// Token: 0x06000D47 RID: 3399 RVA: 0x0005D474 File Offset: 0x0005B674
	public bool hasNext(K id)
	{
		DepletingMultiMapQuery<K, T>.Item item = null;
		return this.d.TryGetValue(id, ref item) && item.hasNext();
	}

	// Token: 0x04000A52 RID: 2642
	private Dictionary<K, DepletingMultiMapQuery<K, T>.Item> d = new Dictionary<K, DepletingMultiMapQuery<K, T>.Item>();

	// Token: 0x020001A6 RID: 422
	private class Item
	{
		// Token: 0x06000D49 RID: 3401 RVA: 0x0005D4A0 File Offset: 0x0005B6A0
		public T getNext()
		{
			return this.list[++this.index];
		}

		// Token: 0x06000D4A RID: 3402 RVA: 0x0000A9DA File Offset: 0x00008BDA
		public bool hasNext()
		{
			return this.index < this.list.Count - 1;
		}

		// Token: 0x04000A53 RID: 2643
		public int index = -1;

		// Token: 0x04000A54 RID: 2644
		public List<T> list = new List<T>();
	}
}
