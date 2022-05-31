using System;

// Token: 0x02000245 RID: 581
public class EMEnchantUnit : EffectMessage
{
	// Token: 0x0600119E RID: 4510 RVA: 0x00004AAC File Offset: 0x00002CAC
	public override bool isAction()
	{
		return true;
	}

	// Token: 0x0600119F RID: 4511 RVA: 0x0000D6C5 File Offset: 0x0000B8C5
	public Tags tags()
	{
		return new Tags(this.enchantTags);
	}

	// Token: 0x04000E2E RID: 3630
	public TilePosition target;

	// Token: 0x04000E2F RID: 3631
	public string[] enchantTags = new string[0];
}
