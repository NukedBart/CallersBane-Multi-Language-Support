using System;
using UnityEngine;

// Token: 0x02000050 RID: 80
public class BattleObj : MonoBehaviour
{
	// Token: 0x060003A2 RID: 930 RVA: 0x0002DED4 File Offset: 0x0002C0D4
	public void setHolder(GameObject parent)
	{
		GameObject gameObject = new GameObject("BattleObj_root" + ++BattleObj._runningId);
		UnityUtil.addChild(parent, base.gameObject);
	}

	// Token: 0x04000258 RID: 600
	private static int _runningId;
}
