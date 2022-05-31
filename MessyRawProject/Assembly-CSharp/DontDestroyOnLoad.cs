using System;
using UnityEngine;

// Token: 0x020001BA RID: 442
public class DontDestroyOnLoad : MonoBehaviour
{
	// Token: 0x06000DE5 RID: 3557 RVA: 0x0000AF76 File Offset: 0x00009176
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}
}
