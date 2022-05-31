using System;

// Token: 0x02000151 RID: 337
public class IpPort
{
	// Token: 0x06000ABB RID: 2747 RVA: 0x00009098 File Offset: 0x00007298
	public IpPort(string ip, int port)
	{
		this.ip = ip;
		this.port = port;
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x000090AE File Offset: 0x000072AE
	public bool Equals(IpPort p)
	{
		return p != null && this.ip == p.ip && this.port == p.port;
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x000090DF File Offset: 0x000072DF
	public override string ToString()
	{
		return this.ip + ":" + this.port;
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x00050710 File Offset: 0x0004E910
	public static IpPort fromString(string address, int defaultPort)
	{
		string text = address;
		int num = defaultPort;
		int num2 = address.LastIndexOf(':');
		int num3;
		if (num2 >= 0 && int.TryParse(address.Substring(num2 + 1), ref num3))
		{
			text = address.Substring(0, num2);
			num = num3;
		}
		return new IpPort(text, num);
	}

	// Token: 0x04000842 RID: 2114
	public readonly string ip;

	// Token: 0x04000843 RID: 2115
	public readonly int port;
}
