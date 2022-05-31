using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000424 RID: 1060
public static class ChangeDetectors
{
	// Token: 0x0600177E RID: 6014 RVA: 0x00010E8F File Offset: 0x0000F08F
	public static IsValueChanged resolution()
	{
		return new FuncValueChangeDetector<KeyValuePair<int, int>>(() => new KeyValuePair<int, int>(Screen.width, Screen.height));
	}
}
