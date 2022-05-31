using System;

// Token: 0x02000168 RID: 360
public struct OnConnectData
{
	// Token: 0x06000B2F RID: 2863 RVA: 0x000095A8 File Offset: 0x000077A8
	public OnConnectData(MiniCommunicator comm, OnConnectData.ConnectType type)
	{
		this.comm = comm;
		this.type = type;
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x000095B8 File Offset: 0x000077B8
	public bool isConnect()
	{
		return this.type == OnConnectData.ConnectType.Connect || this.type == OnConnectData.ConnectType.AlreadyConnected;
	}

	// Token: 0x0400088F RID: 2191
	public MiniCommunicator comm;

	// Token: 0x04000890 RID: 2192
	public OnConnectData.ConnectType type;

	// Token: 0x02000169 RID: 361
	public enum ConnectType
	{
		// Token: 0x04000892 RID: 2194
		Connect,
		// Token: 0x04000893 RID: 2195
		AlreadyConnected,
		// Token: 0x04000894 RID: 2196
		ListenerAdded
	}
}
