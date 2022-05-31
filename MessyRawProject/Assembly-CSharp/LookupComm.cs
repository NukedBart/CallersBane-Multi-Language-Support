using System;

// Token: 0x0200015E RID: 350
internal class LookupComm : MiniCommunicator
{
	// Token: 0x06000AFA RID: 2810 RVA: 0x00050F18 File Offset: 0x0004F118
	protected override bool preHandleMessage(Message msg)
	{
		if (msg is LobbyLookupMessage)
		{
			LobbyLookupMessage lobbyLookupMessage = (LobbyLookupMessage)msg;
			this.lobbyAddress = new IpPort(lobbyLookupMessage.ip, lobbyLookupMessage.port);
		}
		return true;
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x000028DF File Offset: 0x00000ADF
	public override void OnDisconnect(GameSocket s)
	{
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x000093A3 File Offset: 0x000075A3
	protected override void onStateTransition(MiniCommunicator.State newState)
	{
		if (newState == MiniCommunicator.State.ServerResponded)
		{
			this.send(new LobbyLookupMessage());
			return;
		}
		base.onStateTransition(newState);
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x000093C0 File Offset: 0x000075C0
	public IpPort getLobbyAddress()
	{
		return this.lobbyAddress;
	}

	// Token: 0x0400087A RID: 2170
	private IpPort lobbyAddress;
}
