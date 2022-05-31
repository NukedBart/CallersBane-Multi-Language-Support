using System;
using Irrelevant.Assets;
using UnityEngine;

// Token: 0x02000467 RID: 1127
public class DefaultIEffectCallback : iEffect
{
	// Token: 0x06001937 RID: 6455 RVA: 0x00002DDA File Offset: 0x00000FDA
	private DefaultIEffectCallback()
	{
	}

	// Token: 0x06001938 RID: 6456 RVA: 0x0001259B File Offset: 0x0001079B
	public static DefaultIEffectCallback instance()
	{
		if (DefaultIEffectCallback._instance == null)
		{
			DefaultIEffectCallback._instance = new DefaultIEffectCallback();
		}
		return DefaultIEffectCallback._instance;
	}

	// Token: 0x06001939 RID: 6457 RVA: 0x000125B6 File Offset: 0x000107B6
	public void effectAnimDone(EffectPlayer effect, bool loop)
	{
		if (loop)
		{
			effect.playEffect();
		}
		else
		{
			Object.Destroy(effect.gameObject);
		}
	}

	// Token: 0x0600193A RID: 6458 RVA: 0x000028DF File Offset: 0x00000ADF
	public void locator(EffectPlayer effect, AnimLocator loc)
	{
	}

	// Token: 0x040015A0 RID: 5536
	private static DefaultIEffectCallback _instance;
}
