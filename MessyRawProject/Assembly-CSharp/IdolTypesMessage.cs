using System;

// Token: 0x02000333 RID: 819
public class IdolTypesMessage : Message
{
	// Token: 0x0600136B RID: 4971 RVA: 0x000059E4 File Offset: 0x00003BE4
	public override bool shouldLog()
	{
		return false;
	}

	// Token: 0x0400105C RID: 4188
	public IdolType[] types;
}
