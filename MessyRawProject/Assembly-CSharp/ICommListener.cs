using System;

// Token: 0x02000167 RID: 359
public interface ICommListener
{
	// Token: 0x06000B2D RID: 2861
	void handleMessage(Message msg);

	// Token: 0x06000B2E RID: 2862
	void onConnect(OnConnectData data);
}
