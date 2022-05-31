using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x020003A3 RID: 931
public class IdolTypeManager
{
	// Token: 0x060014E5 RID: 5349 RVA: 0x0000F5FC File Offset: 0x0000D7FC
	public static IdolTypeManager getInstance()
	{
		return IdolTypeManager._instance;
	}

	// Token: 0x060014E6 RID: 5350 RVA: 0x0000F603 File Offset: 0x0000D803
	public void reset()
	{
		this._idToType.Clear();
	}

	// Token: 0x060014E7 RID: 5351 RVA: 0x0000F610 File Offset: 0x0000D810
	private void feed(IdolType t)
	{
		if (t == null)
		{
			return;
		}
		this._idToType[t.id] = t;
	}

	// Token: 0x060014E8 RID: 5352 RVA: 0x00080984 File Offset: 0x0007EB84
	public void feed(IdolType[] types)
	{
		foreach (IdolType t in types)
		{
			this.feed(t);
		}
	}

	// Token: 0x060014E9 RID: 5353 RVA: 0x000809B4 File Offset: 0x0007EBB4
	public IdolType get(short id)
	{
		IdolType result;
		if (this._idToType.TryGetValue(id, ref result))
		{
			return result;
		}
		Log.warning("IdolType::get. Couldn't find IdolType with id " + id);
		return null;
	}

	// Token: 0x060014EA RID: 5354 RVA: 0x000809EC File Offset: 0x0007EBEC
	public IdolType[] getAvailable(ICollection<short> unlockedIds)
	{
		HashSet<short> hashSet = new HashSet<short>(unlockedIds);
		foreach (KeyValuePair<short, IdolType> keyValuePair in this._idToType)
		{
			if (keyValuePair.Value.type == IdolRarity.COMMON)
			{
				hashSet.Add(keyValuePair.Value.id);
			}
		}
		return Enumerable.ToArray<IdolType>(Enumerable.Where<IdolType>(Enumerable.Select<short, IdolType>(hashSet, (short id) => this.get(id)), (IdolType a) => a != null));
	}

	// Token: 0x060014EB RID: 5355 RVA: 0x0000F62B File Offset: 0x0000D82B
	public List<IdolType> getAll()
	{
		return new List<IdolType>(this._idToType.Values);
	}

	// Token: 0x060014EC RID: 5356 RVA: 0x0000F63D File Offset: 0x0000D83D
	public int size()
	{
		return this._idToType.Count;
	}

	// Token: 0x040011FC RID: 4604
	private Dictionary<short, IdolType> _idToType = new Dictionary<short, IdolType>();

	// Token: 0x040011FD RID: 4605
	private static IdolTypeManager _instance = new IdolTypeManager();
}
