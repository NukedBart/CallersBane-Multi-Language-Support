using System;

// Token: 0x02000122 RID: 290
public class CardViewCached : AbstractCardView
{
	// Token: 0x0600096D RID: 2413 RVA: 0x00008115 File Offset: 0x00006315
	public void init(Card card)
	{
		this.card = card;
		base.renderer.material = CardView2.material();
		CardViewManager.getInstance().get(card.getCardType()).apply(base.renderer);
	}

	// Token: 0x0600096E RID: 2414 RVA: 0x000028DF File Offset: 0x00000ADF
	public override void setLocked(bool locked, bool useLargeLock)
	{
	}

	// Token: 0x0600096F RID: 2415 RVA: 0x00008149 File Offset: 0x00006349
	public override void renderAsEnabled(bool enabled, float time)
	{
		base.renderer.material.color = ((!enabled) ? AbstractCardView.DisabledColor : AbstractCardView.EnabledColor);
	}
}
