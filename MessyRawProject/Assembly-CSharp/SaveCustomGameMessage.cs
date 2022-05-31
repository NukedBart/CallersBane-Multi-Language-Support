using System;
using System.Collections.Generic;

// Token: 0x020002CD RID: 717
public class SaveCustomGameMessage : Message
{
	// Token: 0x060012BC RID: 4796 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public SaveCustomGameMessage()
	{
	}

	// Token: 0x060012BD RID: 4797 RVA: 0x0000E160 File Offset: 0x0000C360
	public SaveCustomGameMessage(bool isSinglePlayer, string name, string setupCode, bool compileOnly)
	{
		this.customGame = new CustomGameInfo(name, isSinglePlayer, setupCode);
		this.compileOnly = compileOnly;
	}

	// Token: 0x04000F8C RID: 3980
	public bool compileOnly;

	// Token: 0x04000F8D RID: 3981
	public List<ErrorMsg> errors;

	// Token: 0x04000F8E RID: 3982
	public CustomGameInfo customGame;
}
