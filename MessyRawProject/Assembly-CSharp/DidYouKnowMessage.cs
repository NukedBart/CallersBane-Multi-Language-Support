using System;

// Token: 0x020002E0 RID: 736
public class DidYouKnowMessage : Message
{
	// Token: 0x060012DC RID: 4828 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public DidYouKnowMessage()
	{
	}

	// Token: 0x060012DD RID: 4829 RVA: 0x0000E224 File Offset: 0x0000C424
	public DidYouKnowMessage(int id)
	{
		this.id = id;
	}

	// Token: 0x04000FAC RID: 4012
	public int id;

	// Token: 0x04000FAD RID: 4013
	public string hint;
}
