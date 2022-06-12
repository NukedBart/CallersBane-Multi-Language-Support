using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001EE RID: 494
public class IntentionManager : AbstractCommListener, IOkCallback, ICancelCallback, IOkCancelCallback
{
	// Token: 0x06000F83 RID: 3971 RVA: 0x0000C6B2 File Offset: 0x0000A8B2
	private void Awake()
	{
		this._setIntention(Intention.Lobby);
		App.Communicator.addListener(this);
	}

	// Token: 0x06000F84 RID: 3972 RVA: 0x0000C6C7 File Offset: 0x0000A8C7
	private void _setIntention(Intention newIntention)
	{
		this.intention = newIntention;
		this.lastIntention = newIntention;
	}

	// Token: 0x06000F85 RID: 3973 RVA: 0x0000C6D7 File Offset: 0x0000A8D7
	private void clearBattleIntention()
	{
		this.intention = Intention.None;
	}

	// Token: 0x06000F86 RID: 3974 RVA: 0x0000C6E0 File Offset: 0x0000A8E0
	public void registerIntention_Lobby()
	{
		this._setIntention(Intention.Lobby);
	}

	// Token: 0x06000F87 RID: 3975 RVA: 0x0000C6E9 File Offset: 0x0000A8E9
	private void Update()
	{
		this._handleSpecSetup();
	}

	// Token: 0x06000F88 RID: 3976 RVA: 0x00066BB4 File Offset: 0x00064DB4
	private void _handleSpecSetup()
	{
		if (this.spectateSetup == null)
		{
			return;
		}
		if (!this.spectateSetup.isDone())
		{
			return;
		}
		if (this.spectateSetup.success())
		{
			GameObject pausedCommAndTakeOwnership = this.spectateSetup.getPausedCommAndTakeOwnership();
			App.SceneValues.battleMode.specCommGameObject = pausedCommAndTakeOwnership;
			App.SceneValues.battleMode.msg = (this.spectateSetup.getTerminationMessage() as GameInfoMessage);
			Object.DontDestroyOnLoad(pausedCommAndTakeOwnership);
			this.startBattleMode(false);
		}
		else
		{
			FailMessage failMessage = this.spectateSetup.getTerminationMessage() as FailMessage;
			if (failMessage != null)
			{
				this.handleMessage(failMessage);
			}
		}
		this.spectateSetup.Destroy();
		this.spectateSetup = null;
	}

	// Token: 0x06000F89 RID: 3977 RVA: 0x00066C6C File Offset: 0x00064E6C
	public void handleMessage(GameInfoMessage m)
	{
		if (SceneLoader.isScene(new string[]
		{
			"_BattleModeView"
		}))
		{
			long num = (App.SceneValues.battleMode.msg == null) ? -1L : App.SceneValues.battleMode.msg.gameId;
			if (num == m.gameId)
			{
				return;
			}
		}
		App.SceneValues.battleMode.msg = m;
		this.startBattleMode(true);
	}

	// Token: 0x06000F8A RID: 3978 RVA: 0x0000C6F1 File Offset: 0x0000A8F1
	private void startBattleMode(bool orReload)
	{
		if (SceneLoader.isScene(new string[]
		{
			"_BattleModeView"
		}) && !orReload)
		{
			return;
		}
		App.InviteManager.clearInviteList();
		SceneLoader.loadScene("_BattleModeView");
		this.clearBattleIntention();
	}

	// Token: 0x06000F8B RID: 3979 RVA: 0x0000C72C File Offset: 0x0000A92C
	public void handleMessage(BattleRedirectMessage m)
	{
		this._setIntention(Intention.Game);
		App.SceneValues.battleMode.setupForGame(m);
	}

	// Token: 0x06000F8C RID: 3980 RVA: 0x00066CE8 File Offset: 0x00064EE8
	public void handleMessage(SpectateRedirectMessage m)
	{
		this._setIntention(Intention.Spectate);
		App.SceneValues.battleMode.setupForSpectate(m);
		if (this.spectateSetup != null)
		{
			this.spectateSetup.Destroy();
		}
		IpPort ipPort = new IpPort(m.ip, m.port);
		if (ipPort.Equals(App.Communicator.getAddress()))
		{
			App.Communicator.send(new SpectateGameMessage(m.gameId));
		}
		else
		{
			this.spectateSetup = new CommSetup(ipPort, new SpectateGameMessage(m.gameId), new CommSetup.RecvCompleteCondition(typeof(GameInfoMessage)));
		}
	}

	// Token: 0x06000F8D RID: 3981 RVA: 0x00066D8C File Offset: 0x00064F8C
	public void handleMessage(ActiveGameMessage m)
	{
		if (m.serverGameInfo == null)
		{
			return;
		}
		base.StartCoroutine(EnumeratorUtil.chain(new IEnumerator[]
		{
			EnumeratorUtil.Func(new WaitForSeconds(2f)),
			EnumeratorUtil.Func(delegate()
			{
				App.Popups.ShowOkCancel(this, "rejoin", "Game in progress", "Would you like to rejoin your current game?", "Ok", "Ignore");
			})
		}));
	}

	// Token: 0x06000F8E RID: 3982 RVA: 0x00066DE0 File Offset: 0x00064FE0
	public void handleMessage(FailMessage m)
	{
		if (m.isType(typeof(JoinBattleMessage)))
		{
			App.Popups.ShowOk(this, "gameconnectfailed", "Connection failed", "Could not connect to the game server. " + m.info, "Ok");
			App.Communicator.joinLobby(true);
		}
		if (m.isType(typeof(GameMatchAcceptMessage)))
		{
			App.Popups.ShowOk(this, "returntoqueue", "Match timed out", "The match timed out before both players accepted", "Ok");
		}
		if (m.op != null && m.op.StartsWith("Play") && m.op.EndsWith("Match"))
		{
			App.Popups.ShowOk(this, "fail", "Unable to start match", m.info, "Ok");
		}
		if (m.isType(typeof(SpectateGameMessage)))
		{
			App.Popups.ShowOk(this, "spec-failed", "Spectate failed", "Game not found on this server. It might have ended.", "Ok");
		}
	}

	// Token: 0x06000F8F RID: 3983 RVA: 0x00066EF4 File Offset: 0x000650F4
	public void PopupOk(string popupType)
	{
		if (popupType != null)
		{
			if (IntentionManager.<>f__switch$map18 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("spec-failed", 0);
				dictionary.Add("rejoin", 1);
				IntentionManager.<>f__switch$map18 = dictionary;
			}
			int num;
			if (IntentionManager.<>f__switch$map18.TryGetValue(popupType, ref num))
			{
				if (num != 0)
				{
					if (num == 1)
					{
						App.Communicator.send(new BattleRejoinMessage());
					}
				}
				else if (SceneLoader.getScene() == "_BattleModeView")
				{
					App.Communicator.joinLobby(true);
				}
			}
		}
	}

	// Token: 0x06000F90 RID: 3984 RVA: 0x000028DF File Offset: 0x00000ADF
	public void PopupCancel(string popupType)
	{
	}

	// Token: 0x06000F91 RID: 3985 RVA: 0x00066F98 File Offset: 0x00065198
	public override void onConnect(OnConnectData data)
	{
		if (data.type == OnConnectData.ConnectType.ListenerAdded)
		{
			return;
		}
		MiniCommunicator comm = data.comm;
		if (this.intention == Intention.Game && comm.hasServerRole(ServerRole.GAME))
		{
			comm.send(new JoinBattleMessage());
		}
		if (this.intention == Intention.Lobby && comm.hasServerRole(ServerRole.LOBBY))
		{
			comm.send(new JoinLobbyMessage());
		}
	}

	// Token: 0x06000F92 RID: 3986 RVA: 0x0000C745 File Offset: 0x0000A945
	public bool wasLastIntention(Intention intention)
	{
		return this.lastIntention == intention;
	}

	// Token: 0x04000C0E RID: 3086
	private CommSetup spectateSetup;

	// Token: 0x04000C0F RID: 3087
	private Intention intention;

	// Token: 0x04000C10 RID: 3088
	private Intention lastIntention;
}
