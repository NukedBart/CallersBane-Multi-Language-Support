using System;
using UnityEngine;

// Token: 0x02000129 RID: 297
public interface ICardView : ICardViewInfo
{
	// Token: 0x06000990 RID: 2448
	void updateGraphics(Card card);

	// Token: 0x06000991 RID: 2449
	void forceUpdateGraphics(Card card);

	// Token: 0x06000992 RID: 2450
	void setLocked(bool locked, bool useLargeLock);

	// Token: 0x06000993 RID: 2451
	void renderAsEnabled(bool enabled, float time);

	// Token: 0x06000994 RID: 2452
	Transform getTransform();
}
