using System;
using System.Collections.Generic;

// Token: 0x0200025C RID: 604
public class EMSelectedTiles : EffectMessage
{
	// Token: 0x04000E80 RID: 3712
	public Card card;

	// Token: 0x04000E81 RID: 3713
	public TileColor color;

	// Token: 0x04000E82 RID: 3714
	public List<TilePosition> tiles = new List<TilePosition>();

	// Token: 0x04000E83 RID: 3715
	public TargetArea area;

	// Token: 0x04000E84 RID: 3716
	public EMUnitActivateAbility lastAbility;

	// Token: 0x04000E85 RID: 3717
	public BattleMode.ActionType lastActionType;

	// Token: 0x04000E86 RID: 3718
	public List<Unit> units = new List<Unit>();
}
