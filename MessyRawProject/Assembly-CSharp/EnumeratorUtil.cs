using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000431 RID: 1073
public static class EnumeratorUtil
{
	// Token: 0x060017CA RID: 6090 RVA: 0x000914EC File Offset: 0x0008F6EC
	public static IEnumerator chain(params IEnumerator[] enumerators)
	{
		foreach (IEnumerator e in enumerators)
		{
			while (e.MoveNext())
			{
				object obj = e.Current;
				yield return obj;
			}
		}
		yield break;
	}

	// Token: 0x060017CB RID: 6091 RVA: 0x00091510 File Offset: 0x0008F710
	public static IEnumerator Func(Action action)
	{
		action.Invoke();
		yield break;
	}

	// Token: 0x060017CC RID: 6092 RVA: 0x00091534 File Offset: 0x0008F734
	public static IEnumerator Func(YieldInstruction action)
	{
		yield return action;
		yield break;
	}

	// Token: 0x02000432 RID: 1074
	public class QueryValue<T> where T : struct
	{
		// Token: 0x040014F3 RID: 5363
		public T? value;
	}

	// Token: 0x02000433 RID: 1075
	// (Invoke) Token: 0x060017CF RID: 6095
	public delegate IEnumerator FuncCaller(Action f);
}
