using System;

// Token: 0x02000266 RID: 614
public class EMTurnBegin : EffectMessage
{
	// Token: 0x060011EC RID: 4588 RVA: 0x0000D97B File Offset: 0x0000BB7B
	public EMTurnBegin()
	{
	}

	// Token: 0x060011ED RID: 4589 RVA: 0x000777D8 File Offset: 0x000759D8
	private EMTurnBegin(TileColor color, int turn, bool hasSacrificed, int secondsLeft)
	{
		this.color = color;
		this.turn = turn;
		this.hasSacrificed = hasSacrificed;
		this.secondsLeft = secondsLeft;
		this.showText = false;
		this.sendEndPhase = false;
		this.isFake = true;
	}

	// Token: 0x060011EE RID: 4590 RVA: 0x0000D991 File Offset: 0x0000BB91
	public static EMTurnBegin Fake(TileColor color, int turn, bool hasSacrificed, int secondsLeft)
	{
		return new EMTurnBegin(color, turn, hasSacrificed, secondsLeft);
	}

	// Token: 0x04000E9C RID: 3740
	public TileColor color;

	// Token: 0x04000E9D RID: 3741
	public int turn;

	// Token: 0x04000E9E RID: 3742
	public bool showText = true;

	// Token: 0x04000E9F RID: 3743
	public bool sendEndPhase = true;

	// Token: 0x04000EA0 RID: 3744
	public bool isFake;

	// Token: 0x04000EA1 RID: 3745
	public bool hasSacrificed;

	// Token: 0x04000EA2 RID: 3746
	public int secondsLeft;
}
