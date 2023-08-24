using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Token: 0x020003E4 RID: 996
public class SvList<T> : IEnumerable, IEnumerable<T>, ISettingsValue where T : class
{
	// Token: 0x060015E0 RID: 5600 RVA: 0x0008507C File Offset: 0x0008327C
	public SvList()
	{
		if (typeof(T) == typeof(int))
		{
			this.parser = new SvInt(0);
		}
		if (typeof(T) == typeof(bool))
		{
			this.parser = new SvBool(false);
		}
		if (typeof(T) == typeof(string))
		{
			this.parser = new SvString();
		}
		if (this.parser == null)
		{
			string text = "Unsupported type parameter " + typeof(T) + " in SvList";
			throw new InvalidOperationException(text);
		}
	}

	// Token: 0x060015E1 RID: 5601 RVA: 0x0000FFBD File Offset: 0x0000E1BD
	public SvList(params T[] p) : this()
	{
		this.set(new List<T>(p));
	}

	// Token: 0x060015E2 RID: 5602 RVA: 0x0000FFD1 File Offset: 0x0000E1D1
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	// Token: 0x17000103 RID: 259
	// (get) Token: 0x060015E3 RID: 5603 RVA: 0x0000FFD9 File Offset: 0x0000E1D9
	// (set) Token: 0x060015E4 RID: 5604 RVA: 0x0000FFE1 File Offset: 0x0000E1E1
	public List<T> value
	{
		get
		{
			return this._value;
		}
		set
		{
			this.set(value);
		}
	}

	// Token: 0x060015E5 RID: 5605 RVA: 0x00085134 File Offset: 0x00083334
	public void load(string s)
	{
		this._value = new List<T>();
		foreach (string text in Enumerable.Select<string, string>(s.Split(new char[]
		{
			';'
		}), (string e) => e.Trim()))
		{
			if (text.Length != 0)
			{
				this.parser.load(text);
				T value = ((SettingsValue<T>)this.parser).value;
				this._value.Add(value);
			}
		}
	}

	// Token: 0x060015E6 RID: 5606 RVA: 0x000851F8 File Offset: 0x000833F8
	public override string ToString()
	{
		if (this._value == null)
		{
			return string.Empty;
		}
		return string.Join("; ", Enumerable.ToArray<string>(Enumerable.Select<T, string>(this._value, (T e) => e.ToString())));
	}

	// Token: 0x060015E7 RID: 5607 RVA: 0x0000FFEA File Offset: 0x0000E1EA
	public void set(IEnumerable<T> v)
	{
		this._value = new List<T>(v);
	}

	// Token: 0x060015E8 RID: 5608 RVA: 0x0000FFF8 File Offset: 0x0000E1F8
	public IEnumerator<T> GetEnumerator()
	{
		return this.value.GetEnumerator();
	}

	// Token: 0x060015E9 RID: 5609 RVA: 0x0001000A File Offset: 0x0000E20A
	public static implicit operator List<T>(SvList<T> v)
	{
		return v.value;
	}

	// Token: 0x04001319 RID: 4889
	private ISettingsValue parser;

	// Token: 0x0400131A RID: 4890
	private List<T> _value = new List<T>();
}
