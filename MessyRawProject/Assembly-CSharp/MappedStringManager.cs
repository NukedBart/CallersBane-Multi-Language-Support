using System;
using System.Collections.Generic;

// Token: 0x0200020F RID: 527
public class MappedStringManager
{
	// Token: 0x060010CA RID: 4298 RVA: 0x0000D008 File Offset: 0x0000B208
	public static MappedStringManager getInstance()
	{
		return MappedStringManager._instance;
	}

	// Token: 0x060010CB RID: 4299 RVA: 0x0000D00F File Offset: 0x0000B20F
	public void reset()
	{
		this._map.Clear();
	}

	// Token: 0x060010CC RID: 4300 RVA: 0x0000D01C File Offset: 0x0000B21C
	public void feed(MappedString m)
	{
		if (m == null)
		{
			return;
		}
		this._map[m.key.ToLower()] = m;
	}

	// Token: 0x060010CD RID: 4301 RVA: 0x000717C8 File Offset: 0x0006F9C8
	public void feed(MappedString[] strings)
	{
		foreach (MappedString m in strings)
		{
			this.feed(m);
		}
	}

	// Token: 0x060010CE RID: 4302 RVA: 0x000717F8 File Offset: 0x0006F9F8
	public MappedString get(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			Log.error("empty key! null? " + (key == null));
			return null;
		}
		key = key.ToLower();
		MappedString result;
		if (this._map.TryGetValue(key.ToLower(), ref result))
		{
			return result;
		}
		if (key.EndsWith("s"))
		{
			return this.get(key.Substring(0, key.Length - 1));
		}
		return null;
	}

	// Token: 0x060010CF RID: 4303 RVA: 0x00071874 File Offset: 0x0006FA74
	public MappedString getWithWarning(string key)
	{
		MappedString mappedString = this.get(key);
		if (mappedString == null)
		{
			Log.warning("MappedStringManager::get. Couldn't find string with key " + key);
		}
		return mappedString;
	}

	// Token: 0x060010D0 RID: 4304 RVA: 0x0000D03C File Offset: 0x0000B23C
	public int size()
	{
		return this._map.Count;
	}

	// Token: 0x04000D36 RID: 3382
	private Dictionary<string, MappedString> _map = new Dictionary<string, MappedString>();

	// Token: 0x04000D37 RID: 3383
	private static MappedStringManager _instance = new MappedStringManager();
}
