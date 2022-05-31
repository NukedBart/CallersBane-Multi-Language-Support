using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000156 RID: 342
public class CommSetup : ICommListener
{
	// Token: 0x06000AC0 RID: 2752 RVA: 0x00050840 File Offset: 0x0004EA40
	public CommSetup(IpPort address, Message onConnect, CommSetup.ICompleteCondition condition)
	{
		this._g = new GameObject("CommSetup " + address.ToString());
		this._comm = this._g.AddComponent<MiniCommunicator>();
		this._onConnected = onConnect;
		this._comm.addListener(this);
		this._comm.connect(address);
		this._condition = condition;
	}

	// Token: 0x06000AC1 RID: 2753 RVA: 0x000090FC File Offset: 0x000072FC
	public bool isDone()
	{
		return this._done;
	}

	// Token: 0x06000AC2 RID: 2754 RVA: 0x00009104 File Offset: 0x00007304
	public bool success()
	{
		return this._success;
	}

	// Token: 0x06000AC3 RID: 2755 RVA: 0x0000910C File Offset: 0x0000730C
	public Message getTerminationMessage()
	{
		return this._terminationMessage;
	}

	// Token: 0x06000AC4 RID: 2756 RVA: 0x00009114 File Offset: 0x00007314
	private void _setDone(bool success, Message terminationMessage)
	{
		this._done = true;
		this._success = success;
		this._terminationMessage = terminationMessage;
		this._comm.setEnabled(false, false);
		this._comm.removeListener(this);
	}

	// Token: 0x06000AC5 RID: 2757 RVA: 0x000508B4 File Offset: 0x0004EAB4
	public GameObject getPausedCommAndTakeOwnership()
	{
		if (this._g == null)
		{
			return null;
		}
		GameObject g = this._g;
		this._g = null;
		return g;
	}

	// Token: 0x06000AC6 RID: 2758 RVA: 0x00009145 File Offset: 0x00007345
	public void Destroy()
	{
		if (this._g != null)
		{
			Object.Destroy(this._g);
		}
	}

	// Token: 0x06000AC7 RID: 2759 RVA: 0x00009163 File Offset: 0x00007363
	public void onConnect(OnConnectData data)
	{
		if (data.type == OnConnectData.ConnectType.ListenerAdded)
		{
			return;
		}
		this._comm.send(this._onConnected);
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x000508E4 File Offset: 0x0004EAE4
	public void handleMessage(Message msg)
	{
		this._messages.Add(msg);
		if (this._condition.isComplete(msg))
		{
			this._setDone(true, msg);
			this._comm.pushMessages(Enumerable.ToList<Message>(new Message[]
			{
				msg
			}));
			this._messages.Clear();
		}
		if (msg is FailMessage)
		{
			FailMessage failMessage = (FailMessage)msg;
			if (failMessage.isTypes(new Type[]
			{
				typeof(ConnectMessage),
				typeof(FirstConnectMessage)
			}))
			{
				this._setDone(false, failMessage);
			}
			if (this._onConnected != null && failMessage.isType(this._onConnected.GetType()))
			{
				this._setDone(false, failMessage);
			}
		}
	}

	// Token: 0x04000856 RID: 2134
	private GameObject _g;

	// Token: 0x04000857 RID: 2135
	private MiniCommunicator _comm;

	// Token: 0x04000858 RID: 2136
	private bool _done;

	// Token: 0x04000859 RID: 2137
	private bool _success;

	// Token: 0x0400085A RID: 2138
	private Message _onConnected;

	// Token: 0x0400085B RID: 2139
	private Message _terminationMessage;

	// Token: 0x0400085C RID: 2140
	private CommSetup.ICompleteCondition _condition;

	// Token: 0x0400085D RID: 2141
	private List<Message> _messages = new List<Message>();

	// Token: 0x02000157 RID: 343
	public interface ICompleteCondition
	{
		// Token: 0x06000AC9 RID: 2761
		bool isComplete(Message msg);
	}

	// Token: 0x02000158 RID: 344
	public class OkCompleteCondition : CommSetup.ICompleteCondition
	{
		// Token: 0x06000ACA RID: 2762 RVA: 0x00009185 File Offset: 0x00007385
		public OkCompleteCondition(Type msg)
		{
			this._okType = msg;
		}

		// Token: 0x06000ACB RID: 2763 RVA: 0x000509AC File Offset: 0x0004EBAC
		public bool isComplete(Message msg)
		{
			OkMessage okMessage = msg as OkMessage;
			return okMessage != null && okMessage.isType(this._okType);
		}

		// Token: 0x0400085E RID: 2142
		private Type _okType;
	}

	// Token: 0x02000159 RID: 345
	public class RecvCompleteCondition : CommSetup.ICompleteCondition
	{
		// Token: 0x06000ACC RID: 2764 RVA: 0x00009194 File Offset: 0x00007394
		public RecvCompleteCondition(Type msg)
		{
			this._recvType = msg;
		}

		// Token: 0x06000ACD RID: 2765 RVA: 0x000091A3 File Offset: 0x000073A3
		public bool isComplete(Message msg)
		{
			return msg.GetType().Equals(this._recvType);
		}

		// Token: 0x0400085F RID: 2143
		private Type _recvType;
	}
}
