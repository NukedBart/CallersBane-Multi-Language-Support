using System;

// Token: 0x02000170 RID: 368
internal interface IServerMessageSplitter
{
	// Token: 0x06000B65 RID: 2917
	void clear();

	// Token: 0x06000B66 RID: 2918
	void feed(string s);

	// Token: 0x06000B67 RID: 2919
	string getNextMessage();
}
