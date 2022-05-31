using System;
using System.Collections.Generic;

// Token: 0x0200024F RID: 591
public class EMRemoveUnit : EffectMessage
{
	// Token: 0x060011B5 RID: 4533 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override bool isAction()
	{
		return true;
	}

	// Token: 0x04000E58 RID: 3672
	public TilePosition tile;

	// Token: 0x04000E59 RID: 3673
	public EMRemoveUnit.RemovalType removalType;

	// Token: 0x04000E5A RID: 3674
	public List<ModifierAffect> modifierAffects = new List<ModifierAffect>();

	// Token: 0x02000250 RID: 592
	public enum RemovalType
	{
		// Token: 0x04000E5C RID: 3676
		DESTROY,
		// Token: 0x04000E5D RID: 3677
		REMOVE
	}
}
