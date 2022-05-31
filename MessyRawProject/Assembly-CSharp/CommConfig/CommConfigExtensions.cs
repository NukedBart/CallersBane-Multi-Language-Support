using System;

namespace CommConfig
{
	// Token: 0x02000155 RID: 341
	public static class CommConfigExtensions
	{
		// Token: 0x06000ABF RID: 2751 RVA: 0x00050758 File Offset: 0x0004E958
		public static IpPort ip(this Host host)
		{
			string ip = string.Empty;
			int port = 8081;
			switch (host)
			{
			case Host.Localhost:
				ip = "127.0.0.1";
				break;
			case Host.Amazon:
				ip = "107.21.58.31";
				break;
			case Host.Development_Stable:
				ip = "server.internal.mojang";
				break;
			case Host.Development_Volatile:
				ip = "server.internal.mojang";
				port = 8091;
				break;
			case Host.Aron:
				ip = "10.0.3.49";
				break;
			case Host.Jon:
				ip = "10.0.3.91";
				break;
			case Host.Mans:
				ip = "10.0.3.79";
				break;
			case Host.Random_IP_Replace_At_Will:
				ip = "54.208.12.210";
				break;
			case Host.Office:
				ip = "office.mojang.com";
				break;
			case Host.Amazon_Test:
				ip = "107.23.16.25";
				break;
			default:
				throw new ArgumentException("Unknown host: " + host);
			}
			return new IpPort(ip, port);
		}
	}
}
