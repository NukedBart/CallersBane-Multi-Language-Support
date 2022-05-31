using System;
using System.Collections.Generic;

// Token: 0x02000425 RID: 1061
public static class CollectionUtil
{
	// Token: 0x06001781 RID: 6017 RVA: 0x00090D18 File Offset: 0x0008EF18
	public static int getSortedInsertionIndex<T>(List<T> collection, T item, IComparer<T> comp)
	{
		for (int i = 0; i < collection.Count; i++)
		{
			if (comp.Compare(item, collection[i]) < 0)
			{
				return i;
			}
		}
		return collection.Count;
	}

	// Token: 0x06001782 RID: 6018 RVA: 0x00090D58 File Offset: 0x0008EF58
	public static void extendList<T>(List<T> list, int newSize)
	{
		int num = newSize - list.Count;
		if (num <= 0)
		{
			return;
		}
		list.AddRange(new T[num]);
	}

	// Token: 0x06001783 RID: 6019 RVA: 0x00090D84 File Offset: 0x0008EF84
	public static void rotateLeft<T>(List<T> list, int count)
	{
		count = Math.Min(count, list.Count);
		if (count <= 0 || count == list.Count)
		{
			return;
		}
		List<T> range = list.GetRange(0, count);
		list.RemoveRange(0, count);
		list.AddRange(range);
	}

	// Token: 0x06001784 RID: 6020 RVA: 0x00090DCC File Offset: 0x0008EFCC
	public static void rotateRight<T>(List<T> list, int count)
	{
		count = Math.Min(count, list.Count);
		if (count <= 0 || count == list.Count)
		{
			return;
		}
		int num = list.Count - count;
		List<T> range = list.GetRange(num, count);
		list.RemoveRange(num, count);
		list.InsertRange(0, range);
	}

	// Token: 0x06001785 RID: 6021 RVA: 0x00090E20 File Offset: 0x0008F020
	public static void updateDict<K, V>(Dictionary<K, V> src, Dictionary<K, V> with)
	{
		foreach (KeyValuePair<K, V> keyValuePair in with)
		{
			src[keyValuePair.Key] = keyValuePair.Value;
		}
	}

	// Token: 0x06001786 RID: 6022 RVA: 0x00090E84 File Offset: 0x0008F084
	public static T[] enumValues<T>() where T : struct, IConvertible
	{
		Type typeFromHandle = typeof(T);
		if (CollectionUtil._enumValuesMap.ContainsKey(typeFromHandle))
		{
			return (T[])CollectionUtil._enumValuesMap[typeFromHandle];
		}
		if (!typeFromHandle.IsEnum)
		{
			throw new ArgumentException("Type " + typeFromHandle + " is not an enum");
		}
		T[] array = (T[])Enum.GetValues(typeFromHandle);
		CollectionUtil._enumValuesMap.Add(typeFromHandle, array);
		return array;
	}

	// Token: 0x06001787 RID: 6023 RVA: 0x00090EF8 File Offset: 0x0008F0F8
	public static T getMinElement<T>(IEnumerable<T> collection, Func<T, float> f)
	{
		T result = default(T);
		float num = float.MaxValue;
		foreach (T t in collection)
		{
			float num2 = f.Invoke(t);
			if (num2 < num)
			{
				num = num2;
				result = t;
			}
		}
		return result;
	}

	// Token: 0x06001788 RID: 6024 RVA: 0x00090F6C File Offset: 0x0008F16C
	public static T getMaxElement<T>(IEnumerable<T> collection, Func<T, float> f)
	{
		T result = default(T);
		float num = float.MinValue;
		foreach (T t in collection)
		{
			float num2 = f.Invoke(t);
			if (num2 > num)
			{
				num = num2;
				result = t;
			}
		}
		return result;
	}

	// Token: 0x06001789 RID: 6025 RVA: 0x00090FE0 File Offset: 0x0008F1E0
	public static bool isOrdered<T>(IList<T> collection, IComparer<T> comparer)
	{
		if (collection.Count <= 1)
		{
			return true;
		}
		int count = collection.Count;
		for (int i = 1; i < count; i++)
		{
			if (comparer.Compare(collection[i - 1], collection[i]) > 0)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600178A RID: 6026 RVA: 0x00091034 File Offset: 0x0008F234
	public static Dictionary<T, int> countInstances<T>(List<T> items)
	{
		Dictionary<T, int> dictionary = new Dictionary<T, int>();
		foreach (T t in items)
		{
			if (dictionary.ContainsKey(t))
			{
				Dictionary<T, int> dictionary3;
				Dictionary<T, int> dictionary2 = dictionary3 = dictionary;
				T t3;
				T t2 = t3 = t;
				int num = dictionary3[t3];
				dictionary2[t2] = num + 1;
			}
			else
			{
				dictionary[t] = 1;
			}
		}
		return dictionary;
	}

	// Token: 0x0600178B RID: 6027 RVA: 0x000910BC File Offset: 0x0008F2BC
	public static V getOrDefault<K, V>(Dictionary<K, V> dict, K key, V defaultValue)
	{
		V result;
		if (dict.TryGetValue(key, ref result))
		{
			return result;
		}
		return defaultValue;
	}

	// Token: 0x0600178C RID: 6028 RVA: 0x000910DC File Offset: 0x0008F2DC
	public static int count<T>(IEnumerable<T> collection, T element)
	{
		int num = 0;
		foreach (T t in collection)
		{
			if (t.Equals(element))
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x0600178D RID: 6029 RVA: 0x00091148 File Offset: 0x0008F348
	public static int indexOf<T>(T[] array, T element) where T : class
	{
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == element)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x040014E6 RID: 5350
	private static Dictionary<Type, object> _enumValuesMap = new Dictionary<Type, object>();

	// Token: 0x02000426 RID: 1062
	public class Counter<T>
	{
		// Token: 0x0600178F RID: 6031 RVA: 0x00010EE3 File Offset: 0x0000F0E3
		public void Add(T e)
		{
			this.counts[e] = this.Count(e) + 1;
		}

		// Token: 0x06001790 RID: 6032 RVA: 0x00091184 File Offset: 0x0008F384
		public int Count(T e)
		{
			int result;
			this.counts.TryGetValue(e, ref result);
			return result;
		}

		// Token: 0x040014E7 RID: 5351
		private Dictionary<T, int> counts = new Dictionary<T, int>();
	}
}
