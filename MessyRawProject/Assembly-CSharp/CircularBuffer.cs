using System;

// Token: 0x0200018F RID: 399
public class CircularBuffer<T>
{
	// Token: 0x06000C63 RID: 3171 RVA: 0x0000A15A File Offset: 0x0000835A
	public CircularBuffer(int size)
	{
		if (size <= 0)
		{
			throw new ArgumentException("CircularBuffer size must be >= 0");
		}
		this._arr = new T[size];
	}

	// Token: 0x06000C64 RID: 3172 RVA: 0x000574AC File Offset: 0x000556AC
	public void add(T elem)
	{
		this._arr[this._index++] = elem;
		if (this._index == this.size())
		{
			this._index = 0;
		}
	}

	// Token: 0x06000C65 RID: 3173 RVA: 0x0000A180 File Offset: 0x00008380
	public int size()
	{
		return this._arr.Length;
	}

	// Token: 0x06000C66 RID: 3174 RVA: 0x000574F0 File Offset: 0x000556F0
	public void clear()
	{
		for (int i = 0; i < this.size(); i++)
		{
			this._arr[i] = default(T);
		}
	}

	// Token: 0x04000993 RID: 2451
	protected T[] _arr;

	// Token: 0x04000994 RID: 2452
	private int _index;
}
