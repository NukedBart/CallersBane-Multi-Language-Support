using System;

// Token: 0x0200032D RID: 813
public class AvatarSaveMessage : Message
{
	// Token: 0x06001361 RID: 4961 RVA: 0x00078AB4 File Offset: 0x00076CB4
	public AvatarSaveMessage(AvatarConfig cfg)
	{
		this.armBack = cfg.arm.getId();
		this.leg = cfg.leg.getId();
		this.body = cfg.body.getId();
		this.head = cfg.head.getId();
		this.armFront = cfg.arm2.getId();
	}

	// Token: 0x04001045 RID: 4165
	public int armBack;

	// Token: 0x04001046 RID: 4166
	public int leg;

	// Token: 0x04001047 RID: 4167
	public int body;

	// Token: 0x04001048 RID: 4168
	public int head;

	// Token: 0x04001049 RID: 4169
	public int armFront;
}
