using System;

// Token: 0x0200033E RID: 830
public class RemoveCreditCardMessage : Message
{
	// Token: 0x0600137C RID: 4988 RVA: 0x0000E7B1 File Offset: 0x0000C9B1
	public RemoveCreditCardMessage(string cardId)
	{
		this.cardId = cardId;
	}

	// Token: 0x04001091 RID: 4241
	public string cardId;
}
