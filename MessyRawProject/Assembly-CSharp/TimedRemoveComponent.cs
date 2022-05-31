using System;
using UnityEngine;

// Token: 0x02000070 RID: 112
public class TimedRemoveComponent : TimedTrigger
{
	// Token: 0x06000458 RID: 1112 RVA: 0x00004D5A File Offset: 0x00002F5A
	protected override void trigger()
	{
		Object.Destroy(this);
	}
}
