using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200010B RID: 267
public class Pages
{
	// Token: 0x06000886 RID: 2182 RVA: 0x00007744 File Offset: 0x00005944
	public Pages.Page Current()
	{
		return this._current;
	}

	// Token: 0x17000071 RID: 113
	// (get) Token: 0x06000887 RID: 2183 RVA: 0x0000774C File Offset: 0x0000594C
	public int Count
	{
		get
		{
			return this._pages.Count;
		}
	}

	// Token: 0x06000888 RID: 2184 RVA: 0x00007759 File Offset: 0x00005959
	public void Next()
	{
		this._step(1);
	}

	// Token: 0x06000889 RID: 2185 RVA: 0x00044BB0 File Offset: 0x00042DB0
	private void _step(int j)
	{
		this._index = (this._index + j) % this.Count;
		if (this._index < 0)
		{
			this._index += this.Count;
		}
		Pages.Page page = this._pages[this._index];
		if (page == this._current)
		{
			return;
		}
		if (this._current != null)
		{
			this._current.g.SetActive(false);
		}
		if (page != null)
		{
			page.g.SetActive(true);
		}
		this._current = page;
	}

	// Token: 0x0600088A RID: 2186 RVA: 0x00044C48 File Offset: 0x00042E48
	public void Add(GameObject g)
	{
		Pages.Page page = new Pages.Page(g, this.Count);
		if (this._current == null)
		{
			this._current = page;
		}
		else
		{
			page.g.SetActive(false);
		}
		this._pages.Add(page);
	}

	// Token: 0x0600088B RID: 2187 RVA: 0x00044C94 File Offset: 0x00042E94
	public void Remove(GameObject g)
	{
		int num = this._pages.FindIndex((Pages.Page p) => p.g == g);
		if (num < 0)
		{
			return;
		}
		this._pages.RemoveAt(num);
		this._step(0);
	}

	// Token: 0x04000649 RID: 1609
	private List<Pages.Page> _pages = new List<Pages.Page>();

	// Token: 0x0400064A RID: 1610
	private Pages.Page _current;

	// Token: 0x0400064B RID: 1611
	private int _index;

	// Token: 0x0200010C RID: 268
	public class Page
	{
		// Token: 0x0600088C RID: 2188 RVA: 0x00007762 File Offset: 0x00005962
		public Page(GameObject g, int index)
		{
			this.g = g;
			this.index = index;
		}

		// Token: 0x0400064C RID: 1612
		public readonly int index;

		// Token: 0x0400064D RID: 1613
		public GameObject g;
	}
}
