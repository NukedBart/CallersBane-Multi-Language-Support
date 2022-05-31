using System;
using System.Collections.Generic;

// Token: 0x02000137 RID: 311
public class ChatHistory
{
	// Token: 0x06000A12 RID: 2578 RVA: 0x000086D2 File Offset: 0x000068D2
	public ChatHistory(int maxSize)
	{
		this.maxSize = maxSize;
		this.reset();
	}

	// Token: 0x06000A13 RID: 2579 RVA: 0x000086F2 File Offset: 0x000068F2
	public void add(string line)
	{
		this.lines.Add(line);
		if (this.lines.Count > this.maxSize)
		{
			this.lines.RemoveAt(0);
		}
		this.reset();
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x00008728 File Offset: 0x00006928
	public bool previous()
	{
		if (this.index < 0)
		{
			this.index = this.lines.Count;
		}
		return this.setIndex(this.index - 1);
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x00008755 File Offset: 0x00006955
	public bool next()
	{
		return this.setIndex((this.index >= 0) ? (this.index + 1) : this.lines.Count);
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x00008781 File Offset: 0x00006981
	private bool inRange(int x)
	{
		return x >= 0 && x < this.lines.Count;
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x0000879B File Offset: 0x0000699B
	private bool setIndex(int x)
	{
		this.index = ((this.lines.Count != 0) ? Mth.clamp(x, 0, this.lines.Count - 1) : -1);
		return this.inRange(x);
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x000087D4 File Offset: 0x000069D4
	public string get()
	{
		return (!this.inRange(this.index)) ? string.Empty : this.lines[this.index];
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x00008802 File Offset: 0x00006A02
	public void reset()
	{
		this.index = -99999;
	}

	// Token: 0x040007BA RID: 1978
	private int maxSize;

	// Token: 0x040007BB RID: 1979
	private List<string> lines = new List<string>();

	// Token: 0x040007BC RID: 1980
	private int index;
}
