using System;
using System.Collections.Generic;

// Token: 0x0200016F RID: 367
public interface IMessageParser
{
	// Token: 0x06000B62 RID: 2914
	void feed(string data);

	// Token: 0x06000B63 RID: 2915
	Message nextMessage();

	// Token: 0x06000B64 RID: 2916
	void pushMessages(List<Message> messages);
}
