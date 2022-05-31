using System;
using System.Collections.Generic;

// Token: 0x020000A2 RID: 162
public class EmptyTutorial : ITutorial
{
	// Token: 0x060005BD RID: 1469 RVA: 0x00004AAC File Offset: 0x00002CAC
	public bool allowSacrifice(ResourceType resource)
	{
		return true;
	}

	// Token: 0x060005BE RID: 1470 RVA: 0x00004AAC File Offset: 0x00002CAC
	public bool allowEndTurn()
	{
		return true;
	}

	// Token: 0x060005BF RID: 1471 RVA: 0x00004AAC File Offset: 0x00002CAC
	public bool allowMove()
	{
		return true;
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x00004AAC File Offset: 0x00002CAC
	public bool allowPlayCard(Card card)
	{
		return true;
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x00004AAC File Offset: 0x00002CAC
	public bool allowHideCardView()
	{
		return true;
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x000059E4 File Offset: 0x00003BE4
	public virtual bool isRunning()
	{
		return false;
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x000059E4 File Offset: 0x00003BE4
	public bool isBlocking()
	{
		return false;
	}

	// Token: 0x060005C4 RID: 1476 RVA: 0x000038F2 File Offset: 0x00001AF2
	public virtual string getText()
	{
		return string.Empty;
	}

	// Token: 0x060005C5 RID: 1477 RVA: 0x000059E4 File Offset: 0x00003BE4
	public bool shouldZoomCard(Card card)
	{
		return false;
	}

	// Token: 0x060005C6 RID: 1478 RVA: 0x000059E4 File Offset: 0x00003BE4
	public bool onTileClicked(TilePosition pos, Unit unit)
	{
		return false;
	}

	// Token: 0x060005C7 RID: 1479 RVA: 0x000059E4 File Offset: 0x00003BE4
	public bool onEndTurn()
	{
		return false;
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x000059E4 File Offset: 0x00003BE4
	public bool onTurnBegin(int round)
	{
		return false;
	}

	// Token: 0x060005C9 RID: 1481 RVA: 0x000059E4 File Offset: 0x00003BE4
	public bool onCardSacrificed(ResourceType res)
	{
		return false;
	}

	// Token: 0x060005CA RID: 1482 RVA: 0x000059E4 File Offset: 0x00003BE4
	public bool onHandUpdate()
	{
		return false;
	}

	// Token: 0x060005CB RID: 1483 RVA: 0x000059E4 File Offset: 0x00003BE4
	public bool onSummonUnit(Unit unit)
	{
		return false;
	}

	// Token: 0x060005CC RID: 1484 RVA: 0x000059E4 File Offset: 0x00003BE4
	public bool onMoveUnit(Unit unit)
	{
		return false;
	}

	// Token: 0x060005CD RID: 1485 RVA: 0x000059E4 File Offset: 0x00003BE4
	public bool onCardClicked(BattleMode m, Card card)
	{
		return false;
	}

	// Token: 0x060005CE RID: 1486 RVA: 0x000059E4 File Offset: 0x00003BE4
	public bool onCardPlayed(Card card)
	{
		return false;
	}

	// Token: 0x060005CF RID: 1487 RVA: 0x000028DF File Offset: 0x00000ADF
	public virtual void next()
	{
	}

	// Token: 0x060005D0 RID: 1488 RVA: 0x000059E7 File Offset: 0x00003BE7
	public TilePosition[] filter(TilePosition[] p, FilterData data)
	{
		return p;
	}

	// Token: 0x060005D1 RID: 1489 RVA: 0x000059EA File Offset: 0x00003BEA
	public List<TutorialTicker.Tag> getTags()
	{
		return new List<TutorialTicker.Tag>();
	}
}
