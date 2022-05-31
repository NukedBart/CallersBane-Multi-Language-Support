using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000107 RID: 263
public class CardTypeManager
{
	// Token: 0x06000874 RID: 2164 RVA: 0x000448E0 File Offset: 0x00042AE0
	static CardTypeManager()
	{
		string text = "<Invalid>";
		CardTypeManager.IllegalCardType = new CardType();
		CardTypeManager.IllegalCardType.name = text;
		CardTypeManager.IllegalCardType.kind = CardType.Kind.NONE;
		CardTypeManager.IllegalCardType.subTypesStr = text;
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x00007683 File Offset: 0x00005883
	public static CardTypeManager getInstance()
	{
		return CardTypeManager._instance;
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x0000768A File Offset: 0x0000588A
	public void reset()
	{
		this._idToType.Clear();
		this._nameToType.Clear();
	}

	// Token: 0x06000877 RID: 2167 RVA: 0x00044928 File Offset: 0x00042B28
	public void feed(CardType t)
	{
		if (t == null)
		{
			return;
		}
		this._idToType[t.id] = t;
		this._nameToType[t.name.ToLower()] = t;
		if (App.useExternalResources())
		{
			t.writeDefaultTagsToDisk();
		}
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x00044978 File Offset: 0x00042B78
	public void feed(CardType[] types)
	{
		foreach (CardType t in types)
		{
			this.feed(t);
		}
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x000449A8 File Offset: 0x00042BA8
	public CardType get(int id)
	{
		CardType result;
		if (this._idToType.TryGetValue(id, ref result))
		{
			return result;
		}
		Log.warning("CardTypeManager::get. Couldn't find CardType with id " + id);
		return CardTypeManager.IllegalCardType;
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x000076A2 File Offset: 0x000058A2
	public List<CardType> getAll()
	{
		return new List<CardType>(this._idToType.Values);
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x000449E4 File Offset: 0x00042BE4
	public CardType get(string name)
	{
		CardType result;
		if (this._nameToType.TryGetValue(name.ToLower(), ref result))
		{
			return result;
		}
		return CardTypeManager.IllegalCardType;
	}

	// Token: 0x0600087C RID: 2172 RVA: 0x00044A10 File Offset: 0x00042C10
	public CardType debugGetRandom()
	{
		int num = Random.Range(0, this.getAll().Count - 1);
		return this.getAll()[num];
	}

	// Token: 0x0600087D RID: 2173 RVA: 0x000076B4 File Offset: 0x000058B4
	public int size()
	{
		return this._idToType.Count;
	}

	// Token: 0x0400063F RID: 1599
	private Dictionary<int, CardType> _idToType = new Dictionary<int, CardType>();

	// Token: 0x04000640 RID: 1600
	private Dictionary<string, CardType> _nameToType = new Dictionary<string, CardType>();

	// Token: 0x04000641 RID: 1601
	private static CardTypeManager _instance = new CardTypeManager();

	// Token: 0x04000642 RID: 1602
	private static CardType IllegalCardType;
}
