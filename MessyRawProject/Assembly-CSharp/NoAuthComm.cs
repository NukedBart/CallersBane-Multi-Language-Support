using System;

// Token: 0x02000085 RID: 133
internal class NoAuthComm : MiniCommunicator
{
	// Token: 0x06000504 RID: 1284 RVA: 0x0000535A File Offset: 0x0000355A
	protected override void onStateTransition(MiniCommunicator.State newState)
	{
		if (newState == MiniCommunicator.State.ServerResponded)
		{
			base._setState(MiniCommunicator.State.LoggedIn);
		}
		else
		{
			base.onStateTransition(newState);
		}
	}
}
