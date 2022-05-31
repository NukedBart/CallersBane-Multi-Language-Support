using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x0200021A RID: 538
internal class LoginServer
{
	// Token: 0x06001134 RID: 4404 RVA: 0x0000D2B1 File Offset: 0x0000B4B1
	public LoginServer(string name, string address)
	{
		this.name = name;
		this.address = IpPort.fromString(address, 8081);
	}

	// Token: 0x06001135 RID: 4405 RVA: 0x00074F64 File Offset: 0x00073164
	public static LoginServer serverFromString(string s)
	{
		int num = s.IndexOf("#");
		return (num < 0) ? null : new LoginServer(s.Substring(0, num).Trim(), s.Substring(num + 1).Trim());
	}

	// Token: 0x06001136 RID: 4406 RVA: 0x00074FAC File Offset: 0x000731AC
	public static List<LoginServer> serverListFromString(string s)
	{
		return Enumerable.ToList<LoginServer>(Enumerable.Where<LoginServer>(Enumerable.Select<string, LoginServer>(s.Split(new char[]
		{
			'\n'
		}), (string line) => LoginServer.serverFromString(line)), (LoginServer server) => server != null));
	}

	// Token: 0x04000DA0 RID: 3488
	public readonly string name;

	// Token: 0x04000DA1 RID: 3489
	public readonly IpPort address;
}
