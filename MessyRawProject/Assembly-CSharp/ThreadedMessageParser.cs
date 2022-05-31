using System;
using System.Collections.Generic;
using System.Threading;

// Token: 0x02000172 RID: 370
public class ThreadedMessageParser : IMessageParser
{
	// Token: 0x06000B6E RID: 2926 RVA: 0x0005235C File Offset: 0x0005055C
	public ThreadedMessageParser()
	{
		this._parserThread = new Thread(new ThreadStart(this.runLoop));
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x0000982D File Offset: 0x00007A2D
	public ThreadedMessageParser start()
	{
		this._parserThread.Start();
		return this;
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x0000983B File Offset: 0x00007A3B
	public void abort()
	{
		this._parserThreadRun = false;
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x00009846 File Offset: 0x00007A46
	public void feed(string data)
	{
		this._messageReader.feed(data);
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x000523B0 File Offset: 0x000505B0
	public Message nextMessage()
	{
		this.update();
		if (!this.hasMessage())
		{
			return null;
		}
		LinkedList<Message> parsedMessages = this._parsedMessages;
		Message result;
		lock (parsedMessages)
		{
			Message value = this._parsedMessages.First.Value;
			this._parsedMessages.RemoveFirst();
			result = value;
		}
		return result;
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x00052420 File Offset: 0x00050620
	private void update()
	{
		string nextMessage = this._messageReader.getNextMessage();
		if (nextMessage == null)
		{
			return;
		}
		LinkedList<string> toParseMessages = this._toParseMessages;
		lock (toParseMessages)
		{
			this._toParseMessages.AddLast(nextMessage);
		}
	}

	// Token: 0x06000B74 RID: 2932 RVA: 0x00009854 File Offset: 0x00007A54
	public void clearAll()
	{
		this.clearUnparsed();
		this.clearParsed();
	}

	// Token: 0x06000B75 RID: 2933 RVA: 0x00052478 File Offset: 0x00050678
	public void pushMessages(List<Message> messages)
	{
		LinkedList<Message> parsedMessages = this._parsedMessages;
		lock (parsedMessages)
		{
			for (int i = messages.Count - 1; i >= 0; i--)
			{
				this._parsedMessages.AddFirst(messages[i]);
			}
		}
	}

	// Token: 0x06000B76 RID: 2934 RVA: 0x00009862 File Offset: 0x00007A62
	private bool hasMessage()
	{
		return this._parsedMessages.Count > 0;
	}

	// Token: 0x06000B77 RID: 2935 RVA: 0x00009872 File Offset: 0x00007A72
	private void clearUnparsed()
	{
		this._messageReader = new JsonMessageSplitter();
	}

	// Token: 0x06000B78 RID: 2936 RVA: 0x0000987F File Offset: 0x00007A7F
	private void clearParsed()
	{
		this._toParseMessages.Clear();
		this._parsedMessages.Clear();
	}

	// Token: 0x06000B79 RID: 2937 RVA: 0x000524DC File Offset: 0x000506DC
	private void runLoop()
	{
		while (this._parserThreadRun)
		{
			try
			{
				Thread.Sleep(5);
				if (this._toParseMessages.Count != 0)
				{
					string text = null;
					LinkedList<string> toParseMessages = this._toParseMessages;
					lock (toParseMessages)
					{
						text = this._toParseMessages.First.Value;
						this._toParseMessages.RemoveFirst();
					}
					string messageName = MessageFactory.getMessageName(text);
					Message message = MessageFactory.create(messageName, text);
					if (message == null)
					{
						Log.error("Failed creating message for " + messageName);
					}
					else
					{
						LinkedList<Message> parsedMessages = this._parsedMessages;
						lock (parsedMessages)
						{
							this._parsedMessages.AddLast(message);
						}
						if (message.shouldLogS2C())
						{
							Log.info("FROM SERVER (" + message.msg + ")" + message.getRawText());
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.error("RunLoop Exception: " + ex);
			}
		}
	}

	// Token: 0x040008BD RID: 2237
	private IServerMessageSplitter _messageReader = new JsonMessageSplitter();

	// Token: 0x040008BE RID: 2238
	private LinkedList<string> _toParseMessages = new LinkedList<string>();

	// Token: 0x040008BF RID: 2239
	private LinkedList<Message> _parsedMessages = new LinkedList<Message>();

	// Token: 0x040008C0 RID: 2240
	private Thread _parserThread;

	// Token: 0x040008C1 RID: 2241
	private volatile bool _parserThreadRun = true;
}
