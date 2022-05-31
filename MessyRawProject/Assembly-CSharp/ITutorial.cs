using System;
using System.Collections.Generic;

// Token: 0x020000A1 RID: 161
public interface ITutorial
{
	// Token: 0x060005A7 RID: 1447
	bool allowSacrifice(ResourceType resource);

	// Token: 0x060005A8 RID: 1448
	bool allowEndTurn();

	// Token: 0x060005A9 RID: 1449
	bool allowMove();

	// Token: 0x060005AA RID: 1450
	bool allowHideCardView();

	// Token: 0x060005AB RID: 1451
	bool allowPlayCard(Card card);

	// Token: 0x060005AC RID: 1452
	bool isRunning();

	// Token: 0x060005AD RID: 1453
	bool isBlocking();

	// Token: 0x060005AE RID: 1454
	string getText();

	// Token: 0x060005AF RID: 1455
	bool shouldZoomCard(Card card);

	// Token: 0x060005B0 RID: 1456
	bool onTileClicked(TilePosition pos, Unit unit);

	// Token: 0x060005B1 RID: 1457
	bool onEndTurn();

	// Token: 0x060005B2 RID: 1458
	bool onTurnBegin(int round);

	// Token: 0x060005B3 RID: 1459
	bool onCardSacrificed(ResourceType res);

	// Token: 0x060005B4 RID: 1460
	bool onHandUpdate();

	// Token: 0x060005B5 RID: 1461
	bool onMoveUnit(Unit unit);

	// Token: 0x060005B6 RID: 1462
	bool onSummonUnit(Unit unit);

	// Token: 0x060005B7 RID: 1463
	bool onCardClicked(BattleMode m, Card card);

	// Token: 0x060005B8 RID: 1464
	bool onCardPlayed(Card card);

	// Token: 0x060005B9 RID: 1465
	void next();

	// Token: 0x060005BA RID: 1466
	List<TutorialTicker.Tag> getTags();

	// Token: 0x060005BB RID: 1467
	TilePosition[] filter(TilePosition[] pos, FilterData data);
}
