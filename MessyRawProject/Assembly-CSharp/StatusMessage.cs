using System;
using System.Linq;

// Token: 0x020002C7 RID: 711
public class StatusMessage : Message
{
	// Token: 0x060012AC RID: 4780 RVA: 0x0000E02F File Offset: 0x0000C22F
	public bool isType(Type t)
	{
		return t.ToString().Equals(this.op) || t.ToString().Equals(this.op + "Message");
	}

	// Token: 0x060012AD RID: 4781 RVA: 0x0000E065 File Offset: 0x0000C265
	public bool isTypes(params Type[] types)
	{
		return Enumerable.Any<Type>(types, (Type t) => this.isType(t));
	}

	// Token: 0x04000F6E RID: 3950
	public string op;
}
