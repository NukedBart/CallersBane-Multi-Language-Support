using System;

// Token: 0x020002C3 RID: 707
public class FailMessage : StatusMessage
{
	// Token: 0x060012A7 RID: 4775 RVA: 0x0007814C File Offset: 0x0007634C
	public string str()
	{
		string text = (this.info == null) ? string.Empty : ("\n::info:: " + this.info);
		return "Operation Failed! op: " + this.op + text;
	}

	// Token: 0x04000F6D RID: 3949
	public string info;
}
