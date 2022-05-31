using System;
using System.Collections.Generic;

// Token: 0x02000174 RID: 372
public class ServerIdLookup
{
	// Token: 0x06000B7B RID: 2939 RVA: 0x00009897 File Offset: 0x00007A97
	public ServerIdLookup()
	{
	}

	// Token: 0x06000B7C RID: 2940 RVA: 0x00052610 File Offset: 0x00050810
	public ServerIdLookup(ServerIdAddress[] addresses)
	{
		foreach (ServerIdAddress serverIdAddress in addresses)
		{
			this.addresses[serverIdAddress.id] = new IpPort(serverIdAddress.ip, serverIdAddress.port);
		}
	}

	// Token: 0x06000B7D RID: 2941 RVA: 0x0005266C File Offset: 0x0005086C
	public IpPort get(string serverId)
	{
		IpPort result;
		this.addresses.TryGetValue(serverId, ref result);
		return result;
	}

	// Token: 0x040008C5 RID: 2245
	private readonly Dictionary<string, IpPort> addresses = new Dictionary<string, IpPort>();
}
