using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x02000116 RID: 278
public class Tags : IEnumerable<string>, IEnumerable
{
	// Token: 0x060008BE RID: 2238 RVA: 0x00007985 File Offset: 0x00005B85
	public Tags()
	{
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x00007998 File Offset: 0x00005B98
	public Tags(Dictionary<string, object> d)
	{
		this.update(d);
	}

	// Token: 0x060008C0 RID: 2240 RVA: 0x000079B2 File Offset: 0x00005BB2
	public Tags(ICollection<string> collection)
	{
		this.update(collection);
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x000079CC File Offset: 0x00005BCC
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.map.Keys.GetEnumerator();
	}

	// Token: 0x060008C2 RID: 2242 RVA: 0x000079E3 File Offset: 0x00005BE3
	public void clear()
	{
		this.map.Clear();
	}

	// Token: 0x060008C3 RID: 2243 RVA: 0x00045864 File Offset: 0x00043A64
	public void update(Dictionary<string, object> d)
	{
		foreach (KeyValuePair<string, object> keyValuePair in d)
		{
			this.map[keyValuePair.Key.ToLower()] = keyValuePair.Value;
		}
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x000079F0 File Offset: 0x00005BF0
	public void update(Tags tags)
	{
		this.update(tags.map);
	}

	// Token: 0x060008C5 RID: 2245 RVA: 0x000458D0 File Offset: 0x00043AD0
	public void update(ICollection<string> collection)
	{
		foreach (string text in collection)
		{
			this.map[text.ToLower()] = true;
		}
	}

	// Token: 0x060008C6 RID: 2246 RVA: 0x000079FE File Offset: 0x00005BFE
	public void setTo(Dictionary<string, object> d)
	{
		this.clear();
		this.update(d);
	}

	// Token: 0x060008C7 RID: 2247 RVA: 0x00007A0D File Offset: 0x00005C0D
	public bool has(string tag)
	{
		return this.map.ContainsKey(tag.ToLower());
	}

	// Token: 0x060008C8 RID: 2248 RVA: 0x00007A20 File Offset: 0x00005C20
	public bool get<T>(string tag, ref T value)
	{
		if (!this.has(tag))
		{
			return false;
		}
		value = this.get<T>(tag, value);
		return true;
	}

	// Token: 0x060008C9 RID: 2249 RVA: 0x00045934 File Offset: 0x00043B34
	public T get<T>(string tag, T orDefault)
	{
		tag = tag.ToLower();
		if (!this.map.ContainsKey(tag))
		{
			return orDefault;
		}
		object obj = this.map[tag];
		Type type = obj.GetType();
		if (typeof(T) == typeof(float))
		{
			if (type == typeof(double))
			{
				return (T)((object)((float)((double)obj)));
			}
			if (type == typeof(int))
			{
				return (T)((object)((float)((int)obj)));
			}
		}
		return (!(obj is T)) ? orDefault : ((T)((object)obj));
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x000459E4 File Offset: 0x00043BE4
	public override string ToString()
	{
		string text = string.Empty;
		foreach (string text2 in this)
		{
			string text3 = text;
			text = string.Concat(new object[]
			{
				text3,
				"k, v: ",
				text2,
				", ",
				this.map[text2],
				"\n"
			});
		}
		return text;
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x000079CC File Offset: 0x00005BCC
	public IEnumerator<string> GetEnumerator()
	{
		return this.map.Keys.GetEnumerator();
	}

	// Token: 0x0400067B RID: 1659
	private Dictionary<string, object> map = new Dictionary<string, object>();
}
