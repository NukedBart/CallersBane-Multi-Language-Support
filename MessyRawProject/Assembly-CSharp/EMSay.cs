using System;

// Token: 0x0200025B RID: 603
public class EMSay : EffectMessage, MovedOutLogic_Effect
{
	// Token: 0x060011DB RID: 4571 RVA: 0x0000D90B File Offset: 0x0000BB0B
	public void eval(IGame game, EffectDone ed)
	{
		game.getUnit(this.tile).say(this.text);
		ed.callDone();
	}

	// Token: 0x04000E7E RID: 3710
	public TilePosition tile;

	// Token: 0x04000E7F RID: 3711
	public string text;
}
