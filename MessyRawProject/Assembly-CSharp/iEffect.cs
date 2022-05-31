using System;
using Irrelevant.Assets;

// Token: 0x02000466 RID: 1126
public interface iEffect
{
	// Token: 0x06001935 RID: 6453
	void effectAnimDone(EffectPlayer effect, bool loop);

	// Token: 0x06001936 RID: 6454
	void locator(EffectPlayer effect, AnimLocator loc);
}
