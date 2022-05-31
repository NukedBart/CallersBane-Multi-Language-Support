using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

// Token: 0x0200016B RID: 363
public static class ICommListenerDispatcher
{
	// Token: 0x06000B32 RID: 2866 RVA: 0x000519B4 File Offset: 0x0004FBB4
	public static void dispatch(ICommListener listener, string data)
	{
		long num = TimeUtil.CurrentTimeMillis();
		JsonMessageSplitter jsonMessageSplitter = new JsonMessageSplitter();
		jsonMessageSplitter.feed(data);
		for (;;)
		{
			string nextMessage = jsonMessageSplitter.getNextMessage();
			if (nextMessage == null)
			{
				break;
			}
			Message message = MessageFactory.create(nextMessage);
			if (message != null)
			{
				ICommListenerDispatcher.dispatch(listener, message);
			}
		}
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x00051A00 File Offset: 0x0004FC00
	public static IEnumerator enumeratorDispatch(ICommListener listener, string data)
	{
		long st = TimeUtil.CurrentTimeMillis();
		JsonMessageSplitter splitter = new JsonMessageSplitter();
		splitter.feed(data);
		for (;;)
		{
			string json = splitter.getNextMessage();
			if (json == null)
			{
				break;
			}
			Message msg = MessageFactory.create(json);
			if (msg != null)
			{
				ICommListenerDispatcher.dispatch(listener, msg);
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000B34 RID: 2868 RVA: 0x00051A30 File Offset: 0x0004FC30
	public static void dispatch(ICommListener listener, List<Message> msgs)
	{
		foreach (Message msg in msgs)
		{
			ICommListenerDispatcher.dispatch(listener, msg);
		}
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x00051A88 File Offset: 0x0004FC88
	public static void dispatch(ICommListener listener, Message msg)
	{
		MethodInfo method = listener.GetType().GetMethod("handleMessage", new Type[]
		{
			msg.GetType()
		});
		method.Invoke(listener, new object[]
		{
			msg
		});
	}
}
