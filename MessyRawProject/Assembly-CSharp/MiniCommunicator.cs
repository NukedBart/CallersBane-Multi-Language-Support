using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using UnityEngine;

// Token: 0x0200016D RID: 365
public class MiniCommunicator : MonoBehaviour, ISocketListener
{
	// Token: 0x06000B3E RID: 2878 RVA: 0x000095EA File Offset: 0x000077EA
	public static void clearAllCredentials()
	{
		MiniCommunicator.accessToken = null;
		MiniCommunicator.username = null;
		MiniCommunicator.password = null;
		MiniCommunicator.authHash = null;
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x00009604 File Offset: 0x00007804
	protected void Awake()
	{
		this._init();
		this._nextReconnectTime = Time.time;
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x00009617 File Offset: 0x00007817
	protected void OnDestroy()
	{
		ConnectionRegistry.unregister(this);
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x0000961F File Offset: 0x0000781F
	public MiniCommunicator setEnabled(bool sendEnabled, bool receiveEnabled)
	{
		this.doSend = sendEnabled;
		this.doReceive = receiveEnabled;
		return this;
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x00009630 File Offset: 0x00007830
	public void init(bool initialSignIn)
	{
		this.firstConnection = initialSignIn;
		this._init();
	}

	// Token: 0x06000B43 RID: 2883 RVA: 0x00051BE0 File Offset: 0x0004FDE0
	private void _init()
	{
		if (this.gsocket != null)
		{
			return;
		}
		this._commId = ++MiniCommunicator._runningCommid;
		this.gsocket = base.gameObject.AddComponent<GameSocket>();
		this.gsocket.Init(this);
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x0000963F File Offset: 0x0000783F
	protected void _setState(MiniCommunicator.State newState)
	{
		this.state = newState;
		this.onStateTransition(newState);
	}

	// Token: 0x06000B45 RID: 2885 RVA: 0x0000964F File Offset: 0x0000784F
	protected virtual void onStateTransition(MiniCommunicator.State newState)
	{
		if (newState == MiniCommunicator.State.ServerResponded)
		{
			this.send(this._getLoginMessage());
		}
		if (newState == MiniCommunicator.State.LoggedIn)
		{
			this._dispatchOnConnect(OnConnectData.ConnectType.Connect, this.listeners.ToArray());
			this.firstConnection = false;
		}
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x000028DF File Offset: 0x00000ADF
	protected virtual void onConnect(OnConnectData data)
	{
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x00051C30 File Offset: 0x0004FE30
	public bool connect(IpPort address)
	{
		ConnectionRegistry.register(this);
		if (this.isLoggedIn() && address.Equals(this.getAddress()))
		{
			Log.warning(this._commId + " calling connect, but already connected! " + address);
			this._dispatchOnConnect(OnConnectData.ConnectType.AlreadyConnected, this.listeners.ToArray());
			return true;
		}
		Log.warning(this._commId + " calling connect! " + address);
		if (this.gsocket.Connect(address))
		{
			this._reconnectTries = 0;
			this._setState(MiniCommunicator.State.Connected);
			return true;
		}
		return false;
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x00051CCC File Offset: 0x0004FECC
	public virtual bool send(Message m)
	{
		if (!this.doSend)
		{
			return true;
		}
		if (m.shouldLogC2S())
		{
			Log.info(string.Concat(new object[]
			{
				"Sending > ",
				m.msg,
				"\n",
				m
			}));
		}
		if (!this._isServerReady())
		{
			Log.warning("Server NOT ready!");
			return false;
		}
		if (!m.isAllowedForServerRole(this.serverRoles))
		{
			Log.warning("Not allowed for server roles: " + this.serverRoles);
			return false;
		}
		if (m is ConnectMessage)
		{
			Log.warning(string.Concat(new object[]
			{
				"-> ",
				++MiniCommunicator.n,
				" > ",
				this is Communicator
			}));
		}
		return this.gsocket.Send(m);
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x00009685 File Offset: 0x00007885
	public void pushMessages(List<Message> messages)
	{
		this.gsocket.GetMessageParser().pushMessages(messages);
	}

	// Token: 0x06000B4A RID: 2890 RVA: 0x00051DC0 File Offset: 0x0004FFC0
	protected void Update()
	{
		if (this.getAddress() == null)
		{
			return;
		}
		if (this.state == MiniCommunicator.State.Unconnected || this.state == MiniCommunicator.State.Disconnected)
		{
			this._updateReconnect();
		}
		if (this.state == MiniCommunicator.State.Connected || this._isServerReady())
		{
			this._updateReceive();
			this._updatePing();
		}
	}

	// Token: 0x06000B4B RID: 2891 RVA: 0x00051E1C File Offset: 0x0005001C
	protected void _updateReceive()
	{
		if (!this.doReceive)
		{
			return;
		}
		try
		{
			Message message;
			do
			{
				message = this.gsocket.Receive();
				this._handleMessage(message);
			}
			while (message != null);
		}
		catch (SocketException)
		{
		}
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x00009698 File Offset: 0x00007898
	private void _handleMessage(Message msg)
	{
		if (msg == null)
		{
			return;
		}
		this._internalHandleMessage(msg);
		if (this.preHandleMessage(msg))
		{
			this._dispatchMessageToListeners(msg);
		}
		this.postHandleMessage(msg);
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x00051E6C File Offset: 0x0005006C
	private void _updateReconnect()
	{
		if (this.maxReconnectTries >= 0 && this._reconnectTries >= this.maxReconnectTries)
		{
			return;
		}
		if (Time.time < this._nextReconnectTime)
		{
			return;
		}
		float num = (float)Math.Pow(2.0, (double)(0.6f * (float)this._reconnectTries - 1f));
		float num2 = Mathf.Clamp(num * this.reconnectWaitSeconds, 1f, 60f);
		this._nextReconnectTime = Time.time + num2;
		this._reconnectTries++;
		Log.warning("Waiting " + num2 + " seconds.");
		this.connect(this.getAddress());
	}

	// Token: 0x06000B4E RID: 2894 RVA: 0x000096C2 File Offset: 0x000078C2
	public void setMaxReconnectTries(int maxTries)
	{
		this.maxReconnectTries = maxTries;
	}

	// Token: 0x06000B4F RID: 2895 RVA: 0x00051F28 File Offset: 0x00050128
	protected void _updatePing()
	{
		if ((DateTime.Now - this._lastPingTime).TotalSeconds < 10.0)
		{
			return;
		}
		this.send(new PingMessage());
		this._lastPingTime = DateTime.Now;
	}

	// Token: 0x06000B50 RID: 2896 RVA: 0x00051F74 File Offset: 0x00050174
	private void _internalHandleMessage(Message msg)
	{
		if (msg is ServerInfoMessage)
		{
			this.serverRoles = ((ServerInfoMessage)msg).serverRoles();
			this._setState(MiniCommunicator.State.ServerResponded);
		}
		if (msg is FailMessage)
		{
			Log.warning(((FailMessage)msg).str());
		}
		if (msg is OkMessage)
		{
			OkMessage okMessage = (OkMessage)msg;
			if (okMessage.isType(typeof(FirstConnectMessage)) || okMessage.isType(typeof(ConnectMessage)))
			{
				this._setState(MiniCommunicator.State.LoggedIn);
			}
		}
	}

	// Token: 0x06000B51 RID: 2897 RVA: 0x00004AAC File Offset: 0x00002CAC
	protected virtual bool preHandleMessage(Message msg)
	{
		return true;
	}

	// Token: 0x06000B52 RID: 2898 RVA: 0x000028DF File Offset: 0x00000ADF
	protected virtual void postHandleMessage(Message msg)
	{
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x000096CB File Offset: 0x000078CB
	public virtual void OnDisconnect(GameSocket s)
	{
		this._setState(MiniCommunicator.State.Disconnected);
		this._nextReconnectTime = Time.time + 0.2f;
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x00052004 File Offset: 0x00050204
	public void storeUserCredentials(string username, string password)
	{
		if (App.IsStandalone)
		{
			MiniCommunicator.username = username;
			MiniCommunicator.password = ((password == null) ? null : LoginMessage.hashPassword(password));
		}
		else
		{
			MiniCommunicator.username = ((username == null) ? null : App.Crypt.encrypt64(username));
			MiniCommunicator.password = ((password == null) ? null : App.Crypt.encrypt64(LoginMessage.hashPassword(password)));
		}
	}

	// Token: 0x06000B55 RID: 2901 RVA: 0x000096E5 File Offset: 0x000078E5
	public void storeAuthHash(string password)
	{
		MiniCommunicator.authHash = ((password == null) ? null : Hash.sha256(password));
	}

	// Token: 0x06000B56 RID: 2902 RVA: 0x000096FE File Offset: 0x000078FE
	public bool copyCredentialsFrom(MiniCommunicator rhs)
	{
		return MiniCommunicator.username != null && MiniCommunicator.password != null;
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x0005207C File Offset: 0x0005027C
	protected LoginMessage _getLoginMessage()
	{
		LoginMessage loginMessage = (!this.firstConnection) ? new ConnectMessage() : new FirstConnectMessage();
		loginMessage.accessToken = MiniCommunicator.accessToken;
		loginMessage.setCredentials(MiniCommunicator.username, MiniCommunicator.password);
		loginMessage.authHash = MiniCommunicator.authHash;
		return loginMessage;
	}

	// Token: 0x06000B58 RID: 2904 RVA: 0x00009718 File Offset: 0x00007918
	public bool hasServerRole(ServerRole role)
	{
		return Message.hasRole(this.serverRoles, role);
	}

	// Token: 0x06000B59 RID: 2905 RVA: 0x00009726 File Offset: 0x00007926
	public MiniCommunicator addListener(ICommListener listener)
	{
		if (!this.hasListener(listener))
		{
			this._dispatchOnConnect(OnConnectData.ConnectType.ListenerAdded, new ICommListener[]
			{
				listener
			});
			this.listeners.Add(listener);
		}
		return this;
	}

	// Token: 0x06000B5A RID: 2906 RVA: 0x00009752 File Offset: 0x00007952
	public bool hasListener(ICommListener listener)
	{
		return this.listeners.Contains(listener);
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x00009760 File Offset: 0x00007960
	public void removeListener(ICommListener listener)
	{
		this.listeners.Remove(listener);
	}

	// Token: 0x06000B5C RID: 2908 RVA: 0x000520CC File Offset: 0x000502CC
	private void _dispatchMessageToListeners(Message msg)
	{
		msg.comm = this;
		foreach (ICommListener commListener in this.listeners.ToArray())
		{
			MethodInfo method = commListener.GetType().GetMethod("handleMessage", new Type[]
			{
				msg.GetType()
			});
			method.Invoke(commListener, new object[]
			{
				msg
			});
		}
	}

	// Token: 0x06000B5D RID: 2909 RVA: 0x00052138 File Offset: 0x00050338
	private void _dispatchOnConnect(OnConnectData.ConnectType type, params ICommListener[] listeners)
	{
		if (this.state != MiniCommunicator.State.LoggedIn)
		{
			return;
		}
		OnConnectData data = new OnConnectData(this, type);
		foreach (ICommListener commListener in listeners)
		{
			commListener.onConnect(data);
		}
		if (type != OnConnectData.ConnectType.ListenerAdded)
		{
			this.onConnect(data);
		}
	}

	// Token: 0x06000B5E RID: 2910 RVA: 0x0000976F File Offset: 0x0000796F
	public bool isLoggedIn()
	{
		return this.state == MiniCommunicator.State.LoggedIn;
	}

	// Token: 0x06000B5F RID: 2911 RVA: 0x0000977A File Offset: 0x0000797A
	public bool isDisconnected()
	{
		return this.state == MiniCommunicator.State.Disconnected || this.state == MiniCommunicator.State.Debug_Unconnected || !this.gsocket.IsConnected();
	}

	// Token: 0x06000B60 RID: 2912 RVA: 0x000097A5 File Offset: 0x000079A5
	public IpPort getAddress()
	{
		return this.gsocket.getAddress();
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x000097B2 File Offset: 0x000079B2
	protected bool _isServerReady()
	{
		return this.state == MiniCommunicator.State.ServerResponded || this.state == MiniCommunicator.State.LoggedIn;
	}

	// Token: 0x0400089F RID: 2207
	public static AccessToken accessToken;

	// Token: 0x040008A0 RID: 2208
	private static string username;

	// Token: 0x040008A1 RID: 2209
	private static string password;

	// Token: 0x040008A2 RID: 2210
	private static string authHash;

	// Token: 0x040008A3 RID: 2211
	protected GameSocket gsocket;

	// Token: 0x040008A4 RID: 2212
	protected List<ICommListener> listeners = new List<ICommListener>();

	// Token: 0x040008A5 RID: 2213
	private ServerRole serverRoles;

	// Token: 0x040008A6 RID: 2214
	private bool firstConnection;

	// Token: 0x040008A7 RID: 2215
	private bool doReceive = true;

	// Token: 0x040008A8 RID: 2216
	private bool doSend = true;

	// Token: 0x040008A9 RID: 2217
	private int _commId = 2;

	// Token: 0x040008AA RID: 2218
	private static int _runningCommid = 5;

	// Token: 0x040008AB RID: 2219
	protected float reconnectWaitSeconds = 2f;

	// Token: 0x040008AC RID: 2220
	private int _reconnectTries;

	// Token: 0x040008AD RID: 2221
	private float _nextReconnectTime;

	// Token: 0x040008AE RID: 2222
	protected int maxReconnectTries = -1;

	// Token: 0x040008AF RID: 2223
	private MiniCommunicator.State state;

	// Token: 0x040008B0 RID: 2224
	private static int n;

	// Token: 0x040008B1 RID: 2225
	private DateTime _lastPingTime = DateTime.Now;

	// Token: 0x0200016E RID: 366
	protected enum State
	{
		// Token: 0x040008B3 RID: 2227
		Unconnected,
		// Token: 0x040008B4 RID: 2228
		Connected,
		// Token: 0x040008B5 RID: 2229
		ServerResponded,
		// Token: 0x040008B6 RID: 2230
		LoggedIn,
		// Token: 0x040008B7 RID: 2231
		Disconnected,
		// Token: 0x040008B8 RID: 2232
		Debug_Unconnected
	}
}
