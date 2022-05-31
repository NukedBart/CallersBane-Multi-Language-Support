using System;

// Token: 0x02000413 RID: 1043
public class SelectableTargets
{
	// Token: 0x17000121 RID: 289
	// (get) Token: 0x06001716 RID: 5910 RVA: 0x0001097F File Offset: 0x0000EB7F
	public int Count
	{
		get
		{
			return (this.tileSets == null) ? 0 : this.tileSets.Length;
		}
	}

	// Token: 0x04001483 RID: 5251
	public TilePosition[][] tileSets;
}
