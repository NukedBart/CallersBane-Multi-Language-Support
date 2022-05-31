using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CommConfig;
using UnityEngine;

// Token: 0x0200015A RID: 346
public class Communicator : MiniCommunicator
{
	// Token: 0x06000ACF RID: 2767 RVA: 0x00050A2C File Offset: 0x0004EC2C
	public string GetCardDownloadURL()
	{
		if (base.getAddress() == null)
		{
			return string.Empty;
		}
		return string.Concat(new string[]
		{
			"http://",
			base.getAddress().ip,
			":",
			8082.ToString(),
			"/"
		});
	}

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x06000AD0 RID: 2768 RVA: 0x000091B6 File Offset: 0x000073B6
	public bool ReadyToUse
	{
		get
		{
			return this.lobbyAddress != null && base._isServerReady();
		}
	}

	// Token: 0x06000AD1 RID: 2769 RVA: 0x00050A8C File Offset: 0x0004EC8C
	private IEnumerator Start()
	{
		Application.targetFrameRate = 60;
		if (!App.IsStandalone)
		{
			IpPort lookupAddress = this.getLookupAddress();
			yield return base.StartCoroutine(this.connectToLookup(lookupAddress));
		}
		yield break;
	}

	// Token: 0x06000AD2 RID: 2770 RVA: 0x00050AA8 File Offset: 0x0004ECA8
	public IEnumerator connectToLookup(IpPort lookupAddress)
	{
		if (!lookupAddress.Equals(Host.Amazon.ip()) && !lookupAddress.Equals(Host.Amazon_Test.ip()))
		{
			this.AuthUrl = new Uri("http://auth.yggdrasil-staging.mojang.com:8080");
			this.ApiURL = "http://api-staging.mojang.com:8080/user";
		}
		base.init(true);
		LookupComm lookupComm = base.gameObject.AddComponent<LookupComm>();
		lookupComm.setMaxReconnectTries(this.maxReconnectTries);
		this.lastLookupConnectResult = new bool?(lookupComm.connect(lookupAddress));
		while (lookupComm.getLobbyAddress() == null)
		{
			yield return new WaitForEndOfFrame();
		}
		this.lobbyAddress = lookupComm.getLobbyAddress();
		Object.Destroy(lookupComm);
		this.lastConnectResult = new bool?(base.connect(this.lobbyAddress));
		yield break;
	}

	// Token: 0x06000AD3 RID: 2771 RVA: 0x000091CC File Offset: 0x000073CC
	public string getAuthUrlForCall(string call)
	{
		return new Uri(this.AuthUrl, call).AbsoluteUri;
	}

	// Token: 0x06000AD4 RID: 2772 RVA: 0x00050AD4 File Offset: 0x0004ECD4
	private IpPort getLookupAddress()
	{
		string text = Application.persistentDataPath + "/server_ip.txt";
		Log.info(text);
		if (File.Exists(text) && this.UseHost != Host.Amazon_Test && !Application.isEditor)
		{
			string[] array = File.ReadAllText(text).Split(new char[]
			{
				' '
			});
			short port = Convert.ToInt16(array[1]);
			return new IpPort(array[0], (int)port);
		}
		return this.UseHost.ip();
	}

	// Token: 0x06000AD5 RID: 2773 RVA: 0x000091DF File Offset: 0x000073DF
	public void setData(string log)
	{
		this.gsocket.SetFakeData(log);
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x00050B50 File Offset: 0x0004ED50
	protected override bool preHandleMessage(Message msg)
	{
		if (msg is ChatMessageMessage && !App.IsChatAllowed())
		{
			return false;
		}
		if (msg is CardTypesMessage)
		{
			this.lastCardTypesServerVersion = this.serverVersion;
		}
		if (msg is ServerInfoMessage)
		{
			ServerInfoMessage serverInfoMessage = (ServerInfoMessage)msg;
			this.serverVersion = serverInfoMessage.getVersion();
			App.AssetLoader.SetAssetURL(serverInfoMessage.assetURL);
			App.AssetLoader.SetNewsURL(serverInfoMessage.newsURL);
			if (serverInfoMessage.shouldCheckForUpdate())
			{
				App.ApplicationController.CheckForUpdate();
			}
		}
		if (msg is FailMessage)
		{
			FailMessage failMessage = (FailMessage)msg;
			if (failMessage.info != null && failMessage.info.ToLower().Contains("already logged in from another client"))
			{
				App.SignOutWithMessage("Already logged in", failMessage.info);
			}
		}
		return true;
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x00050C28 File Offset: 0x0004EE28
	protected override void postHandleMessage(Message msg)
	{
		if (msg is RedirectConnection)
		{
			RedirectConnection redirectConnection = (RedirectConnection)msg;
			this.lastConnectResult = new bool?(base.connect(new IpPort(redirectConnection.ip, redirectConnection.port)));
		}
		base.postHandleMessage(msg);
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x000091ED File Offset: 0x000073ED
	public Version getServerVersion()
	{
		return this.serverVersion;
	}

	// Token: 0x06000AD9 RID: 2777 RVA: 0x000091F5 File Offset: 0x000073F5
	protected override void onConnect(OnConnectData data)
	{
		this.loginWhenServerResponds = true;
		if (base.hasServerRole(ServerRole.LOBBY) && !SceneLoader.isScene(new string[]
		{
			"_LoginView"
		}))
		{
			App.ArenaChat.InitiateOrRejoinAllActive();
		}
	}

	// Token: 0x06000ADA RID: 2778 RVA: 0x0000922C File Offset: 0x0000742C
	protected override void onStateTransition(MiniCommunicator.State newState)
	{
		if (!this.loginWhenServerResponds && newState == MiniCommunicator.State.ServerResponded)
		{
			return;
		}
		base.onStateTransition(newState);
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x00009248 File Offset: 0x00007448
	public void setOnPostHandler(IOnPostHandler listener)
	{
		this.onPostHandler = listener;
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x00009251 File Offset: 0x00007451
	public bool sendRequest(Message msg)
	{
		return this.send(msg);
	}

	// Token: 0x06000ADD RID: 2781 RVA: 0x00050C70 File Offset: 0x0004EE70
	public override bool send(Message msg)
	{
		bool flag = base.send(msg);
		if (flag && this.onPostHandler != null)
		{
			this.onPostHandler.onPostMessage(msg);
		}
		return flag;
	}

	// Token: 0x06000ADE RID: 2782 RVA: 0x0000925A File Offset: 0x0000745A
	public void joinLobby(bool loadLobbyScene)
	{
		if (loadLobbyScene)
		{
			SceneLoader.loadScene("_Lobby");
		}
		App.Intention.registerIntention_Lobby();
		base.StartCoroutine(this.switchToLobbyServerCoroutine());
	}

	// Token: 0x06000ADF RID: 2783 RVA: 0x00050CA4 File Offset: 0x0004EEA4
	private IEnumerator switchToLobbyServerCoroutine()
	{
		this.lastConnectResult = new bool?(base.connect(this.lobbyAddress));
		yield break;
	}

	// Token: 0x06000AE0 RID: 2784 RVA: 0x00009283 File Offset: 0x00007483
	private void requestCardTypes()
	{
		if (!base._isServerReady())
		{
			return;
		}
		if (!this.lastCardTypesServerVersion.isLowerThan(this.serverVersion))
		{
			return;
		}
		this.onConnectedToNewerServer();
	}

	// Token: 0x06000AE1 RID: 2785 RVA: 0x000092AE File Offset: 0x000074AE
	private void onConnectedToNewerServer()
	{
		this.send(new MappedStringsMessage());
		this.send(new CardTypesMessage());
		this.send(new AvatarTypesMessage());
		this.send(new IdolTypesMessage());
	}

	// Token: 0x06000AE2 RID: 2786 RVA: 0x000092E0 File Offset: 0x000074E0
	public void sendLogin()
	{
		if (base._isServerReady())
		{
			this.send(base._getLoginMessage());
		}
		else
		{
			this.loginWhenServerResponds = true;
		}
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x00009306 File Offset: 0x00007506
	public void debug_disconnect_no_reconnect()
	{
		this.gsocket.CloseAndFakeDisconnect();
		base._setState(MiniCommunicator.State.Debug_Unconnected);
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x0000931A File Offset: 0x0000751A
	public void debug_reconnect_boolean_only()
	{
		base._setState(MiniCommunicator.State.Disconnected);
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x00009323 File Offset: 0x00007523
	public List<ICommListener> getListeners()
	{
		return new List<ICommListener>(this.listeners);
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x00050CC0 File Offset: 0x0004EEC0
	public List<T> getListenersOfType<T>()
	{
		List<T> list = new List<T>();
		foreach (ICommListener commListener in this.listeners)
		{
			if (commListener is T)
			{
				list.Add((T)((object)commListener));
			}
		}
		return list;
	}

	// Token: 0x04000860 RID: 2144
	private const int portCardDownload = 8082;

	// Token: 0x04000861 RID: 2145
	public Host UseHost = Host.Amazon;

	// Token: 0x04000862 RID: 2146
	public PurchaseEnvironment UsePurchaseEnvironment = PurchaseEnvironment.Staging;

	// Token: 0x04000863 RID: 2147
	private Uri AuthUrl = new Uri("https://authserver.mojang.com");

	// Token: 0x04000864 RID: 2148
	public string ApiURL = "https://api.mojang.com/user";

	// Token: 0x04000865 RID: 2149
	public bool? lastLookupConnectResult;

	// Token: 0x04000866 RID: 2150
	private bool? lastConnectResult;

	// Token: 0x04000867 RID: 2151
	private IpPort lobbyAddress;

	// Token: 0x04000868 RID: 2152
	private IpPort rejoinAddress;

	// Token: 0x04000869 RID: 2153
	private bool loginWhenServerResponds;

	// Token: 0x0400086A RID: 2154
	private IOnPostHandler onPostHandler;

	// Token: 0x0400086B RID: 2155
	private Version serverVersion = new Version();

	// Token: 0x0400086C RID: 2156
	private Version lastCardTypesServerVersion = new Version();
}
