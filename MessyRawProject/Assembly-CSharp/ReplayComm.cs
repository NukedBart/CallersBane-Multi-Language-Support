using System;

// Token: 0x02000460 RID: 1120
internal class ReplayComm : MiniCommunicator
{
	// Token: 0x060018F9 RID: 6393 RVA: 0x000122DD File Offset: 0x000104DD
	public ReplayComm Init(IpPort serverAddress, long gameId)
	{
		base.connect(serverAddress);
		this.gameId = gameId;
		return this;
	}

	// Token: 0x060018FA RID: 6394 RVA: 0x00093CF8 File Offset: 0x00091EF8
	protected override bool preHandleMessage(Message msg)
	{
		if (msg is GetReplayLogMessage)
		{
			GetReplayLogMessage getReplayLogMessage = (GetReplayLogMessage)msg;
			this.replayLog = getReplayLogMessage.log;
		}
		return true;
	}

	// Token: 0x060018FB RID: 6395 RVA: 0x000028DF File Offset: 0x00000ADF
	public override void OnDisconnect(GameSocket s)
	{
	}

	// Token: 0x060018FC RID: 6396 RVA: 0x000122EF File Offset: 0x000104EF
	protected override void onStateTransition(MiniCommunicator.State newState)
	{
		if (newState == MiniCommunicator.State.ServerResponded)
		{
			this.send(new GetReplayLogMessage(this.gameId));
			return;
		}
		base.onStateTransition(newState);
	}

	// Token: 0x060018FD RID: 6397 RVA: 0x00012312 File Offset: 0x00010512
	public string getReplayLog()
	{
		return this.replayLog;
	}

	// Token: 0x060018FE RID: 6398 RVA: 0x0001231A File Offset: 0x0001051A
	public bool isDone()
	{
		return this.replayLog != null || this.error != null;
	}

	// Token: 0x04001566 RID: 5478
	private long gameId;

	// Token: 0x04001567 RID: 5479
	private string replayLog;

	// Token: 0x04001568 RID: 5480
	private string error;
}
