using System;
using System.Collections.Generic;
using System.IO;

// Token: 0x0200002F RID: 47
public class FileModificationChecker
{
	// Token: 0x0600022D RID: 557 RVA: 0x0002128C File Offset: 0x0001F48C
	public bool isModified(string fn)
	{
		long? num = FileModificationChecker.timestamp(fn);
		long? num2 = default(long?);
		long num3;
		if (this._timestamps.TryGetValue(fn, ref num3))
		{
			num2 = new long?(num3);
		}
		if (num == num2)
		{
			return false;
		}
		if (num != null)
		{
			this._timestamps[fn] = num.Value;
		}
		else
		{
			this._timestamps.Remove(fn);
		}
		return true;
	}

	// Token: 0x0600022E RID: 558 RVA: 0x00003A2C File Offset: 0x00001C2C
	public bool hadFile(string fn)
	{
		return this._timestamps.ContainsKey(fn);
	}

	// Token: 0x0600022F RID: 559 RVA: 0x00003A3A File Offset: 0x00001C3A
	public void clear()
	{
		this._timestamps.Clear();
	}

	// Token: 0x06000230 RID: 560 RVA: 0x00021320 File Offset: 0x0001F520
	private static long? timestamp(string fn)
	{
		if (fn == null)
		{
			return default(long?);
		}
		if (!File.Exists(fn))
		{
			return default(long?);
		}
		return new long?(new FileInfo(fn).LastWriteTimeUtc.Ticks);
	}

	// Token: 0x0400011D RID: 285
	private Dictionary<string, long> _timestamps = new Dictionary<string, long>();
}
