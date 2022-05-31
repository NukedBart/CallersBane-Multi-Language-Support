using System;
using UnityEngine;

// Token: 0x02000071 RID: 113
public class TimedRemoveGameObject : TimedTrigger
{
	// Token: 0x0600045A RID: 1114 RVA: 0x00004D62 File Offset: 0x00002F62
	protected override void trigger()
	{
		Object.Destroy(base.gameObject);
	}
}
