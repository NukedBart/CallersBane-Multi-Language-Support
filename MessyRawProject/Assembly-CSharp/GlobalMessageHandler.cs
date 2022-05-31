using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001C5 RID: 453
public class GlobalMessageHandler : AbstractCommListener, IOnPostHandler
{
	// Token: 0x14000005 RID: 5
	// (add) Token: 0x06000E4A RID: 3658 RVA: 0x0000B74C File Offset: 0x0000994C
	// (remove) Token: 0x06000E4B RID: 3659 RVA: 0x0000B765 File Offset: 0x00009965
	public event GlobalMessageHandler.EmptyDelegate LimitedRewardEvent;

	// Token: 0x06000E4C RID: 3660 RVA: 0x0000B77E File Offset: 0x0000997E
	public void Start()
	{
		this.regularStyle = new GUIStyle(((GUISkin)ResourceManager.Load("_GUISkins/RegularUI")).label);
		this.regularStyle.alignment = 3;
	}

	// Token: 0x06000E4D RID: 3661 RVA: 0x0000B7AB File Offset: 0x000099AB
	public void init()
	{
		App.Communicator.addListener(this);
		App.Communicator.setOnPostHandler(this);
		SceneLoader.OnSceneHasLoaded += this.HandleSceneLoaderOnSceneHasLoaded;
	}

	// Token: 0x06000E4E RID: 3662 RVA: 0x0000B7D5 File Offset: 0x000099D5
	private void Update()
	{
		this.handleMouseInput();
		this.handleKeyInput();
	}

	// Token: 0x06000E4F RID: 3663 RVA: 0x0000B7E3 File Offset: 0x000099E3
	private void OnGUI()
	{
		GUI.depth = 6;
		this.OnGUI_updateLoadingBattle();
	}

	// Token: 0x06000E50 RID: 3664 RVA: 0x0006107C File Offset: 0x0005F27C
	private void OnGUI_updateLoadingBattle()
	{
		if (this.regularStyle == null)
		{
			return;
		}
		float num = Time.time - this.enterBattleTime;
		if (num >= 5f)
		{
			return;
		}
		if (SceneLoader.getScene() != "_Lobby")
		{
			return;
		}
		GUI.color = new Color(1f, 1f, 1f, 0.7f);
		GUI.DrawTexture(GUIUtil.screen(), ResourceManager.LoadTexture("Login/black"));
		this.regularStyle.fontSize = Screen.height / 14;
		string[] array = new string[]
		{
			"Loading.",
			"Loading..",
			"Loading..."
		};
		Vector2 vector = this.regularStyle.CalcSize(new GUIContent(array[2]));
		Rect centered = GeomUtil.getCentered(new Rect(0f, 0f, vector.x, vector.y), true, true);
		string text = array[(int)(Time.time * 3f) % 3];
		GUI.Label(centered, text, this.regularStyle);
	}

	// Token: 0x06000E51 RID: 3665 RVA: 0x0000B7F1 File Offset: 0x000099F1
	private void handleKeyInput()
	{
		this.handleEditorKeyInput();
	}

	// Token: 0x06000E52 RID: 3666 RVA: 0x00061180 File Offset: 0x0005F380
	private void handleEditorKeyInput()
	{
		if (!Application.isEditor)
		{
			return;
		}
		if (Input.GetKeyDown(270))
		{
			App.Communicator.debug_reconnect_boolean_only();
		}
		if (Input.GetKeyDown(269))
		{
			App.Communicator.debug_disconnect_no_reconnect();
		}
		if (UnityUtil.GetKeyDown(117, new KeyCode[]
		{
			306
		}) && App.MyProfile.ProfileInfo.featureType.isDemo())
		{
			App.Communicator.send(new TestMessageMessage());
		}
	}

	// Token: 0x06000E53 RID: 3667 RVA: 0x0000B7F9 File Offset: 0x000099F9
	private void handleMouseInput()
	{
		this.handleEditorMouseInput();
	}

	// Token: 0x06000E54 RID: 3668 RVA: 0x00061210 File Offset: 0x0005F410
	private void handleEditorMouseInput()
	{
		if (!Application.isEditor)
		{
			return;
		}
		if (!Input.GetKey(308))
		{
			return;
		}
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (axis == 0f)
		{
			return;
		}
		float num = this.timeScale;
		float num2 = (axis <= 0f) ? 0.8333333f : 1.2f;
		float num3 = Mathf.Clamp(this.timeScale * num2, 0.01f, 10000f);
		this.setTimeScale(num3);
	}

	// Token: 0x06000E55 RID: 3669 RVA: 0x0000B801 File Offset: 0x00009A01
	public float getTimeScale()
	{
		return this.timeScale;
	}

	// Token: 0x06000E56 RID: 3670 RVA: 0x00061290 File Offset: 0x0005F490
	public void setTimeScale(float scale)
	{
		if (scale == this.timeScale)
		{
			return;
		}
		this.timeScale = scale;
		App.Clocks.tweenClock.setSpeed(scale);
		App.Clocks.animClock.setSpeed(scale);
		App.Clocks.battleModeClock.setSpeed(scale);
		iTween.setSharedTimer(App.Clocks.tweenClock);
	}

	// Token: 0x06000E57 RID: 3671 RVA: 0x0000B809 File Offset: 0x00009A09
	public void handleMessage(GameMatchQueueStatusMessage m)
	{
		App.LobbyMenu.UpdateQueueStatus(m.gameType, m.inQueue);
	}

	// Token: 0x06000E58 RID: 3672 RVA: 0x0000B821 File Offset: 0x00009A21
	public void handleMessage(GetTowerInfoMessage m)
	{
		App.TowerChallengeInfo.SetTowerChallengeInfo(m);
	}

	// Token: 0x06000E59 RID: 3673 RVA: 0x0000B82E File Offset: 0x00009A2E
	public void handleMessage(GetTutorialInfoMessage m)
	{
		App.TutorialChallengeInfo.SetTowerChallengeInfo(m);
	}

	// Token: 0x06000E5A RID: 3674 RVA: 0x0000B83B File Offset: 0x00009A3B
	public void handleMessage(GetCustomGamesSpMessage m)
	{
		m.init();
		App.CustomMatchSingleplayerInfo = m;
	}

	// Token: 0x06000E5B RID: 3675 RVA: 0x0000B849 File Offset: 0x00009A49
	public void handleMessage(GetCustomGamesMpMessage m)
	{
		m.init();
		App.CustomMatchMultiplayerInfo = m;
	}

	// Token: 0x06000E5C RID: 3676 RVA: 0x0000B857 File Offset: 0x00009A57
	public void handleMessage(RewardLimitedMessage m)
	{
		App.WaitingReward = m;
		if (this.LimitedRewardEvent != null)
		{
			this.LimitedRewardEvent();
		}
	}

	// Token: 0x06000E5D RID: 3677 RVA: 0x0000B875 File Offset: 0x00009A75
	public void handleMessage(ServerSettingsMessage m)
	{
		App.ServerSettings = m;
	}

	// Token: 0x06000E5E RID: 3678 RVA: 0x0000B87D File Offset: 0x00009A7D
	public void handleMessage(GetGameLogMessage m)
	{
		App.SceneValues.battleMode.setupForReplay(m.log);
		SceneLoader.loadScene("_BattleModeView");
	}

	// Token: 0x06000E5F RID: 3679 RVA: 0x000612F4 File Offset: 0x0005F4F4
	public void handleMessage(MessageMessage m)
	{
		if (m.type == MessageMessage.Type.SOLD_MARKET_SCROLLS)
		{
			App.InviteManager.addInvite("Scrolls sold on the Black Market!", "You have unclaimed rewards in the Black Market.", Invite.InviteType.SOLD_MARKET_SCROLLS);
		}
		if (m.isStarterDeck())
		{
			if (SceneLoader.isScene(new string[]
			{
				"_LoginView"
			}))
			{
				return;
			}
			App.SceneValues.selectPreconstructed.Add(m.type);
			SceneLoader.loadScene("_SelectPreconstructed");
		}
	}

	// Token: 0x06000E60 RID: 3680 RVA: 0x00061368 File Offset: 0x0005F568
	public void handleMessage(FatalFailMessage m)
	{
		string text = (m.op == null) ? string.Empty : (" (" + m.op + ")");
		string message = "This was the error reported by the server" + text + ":\n\n" + m.info;
		App.SignOutWithMessage("Something went wrong!", message);
	}

	// Token: 0x06000E61 RID: 3681 RVA: 0x000613C4 File Offset: 0x0005F5C4
	public override void handleMessage(Message msg)
	{
		if (msg is FailMessage)
		{
			FailMessage failMessage = (FailMessage)msg;
			if (failMessage.isTypes(new Type[]
			{
				typeof(ConnectMessage),
				typeof(FirstConnectMessage)
			}) && failMessage.info == "Invalid access token")
			{
				App.SignOutWithMessage("Authentication failed", "You need to re-login.");
			}
			if (failMessage is DemoFailMessage)
			{
				App.Popups.ShowDemoOk();
			}
			if (failMessage is RestrictedFailMessage)
			{
				App.Popups.ShowOk(new OkVoidCallback(), "restricted", "Restricted", failMessage.info, "Ok");
			}
			if (failMessage.GetType() == typeof(FailMessage) && failMessage.isTypes(new Type[]
			{
				typeof(GameChallengeAcceptMessage),
				typeof(GameChallengeRequestMessage)
			}))
			{
				App.Popups.ShowOk(new OkVoidCallback(), "challengefail", "Challenge failed", failMessage.info, "Ok");
			}
		}
	}

	// Token: 0x06000E62 RID: 3682 RVA: 0x0000B89E File Offset: 0x00009A9E
	public void handleMessage(SetFeatureTypeMessage m)
	{
		App.MyProfile.ProfileInfo.SetFeatureType(m.featureType);
	}

	// Token: 0x06000E63 RID: 3683 RVA: 0x0000B8B5 File Offset: 0x00009AB5
	public void handleMessage(AchievementTypesMessage m)
	{
		AchievementTypeManager.getInstance().reset();
		AchievementTypeManager.getInstance().feed(m.achievementTypes);
	}

	// Token: 0x06000E64 RID: 3684 RVA: 0x0000B8D1 File Offset: 0x00009AD1
	public void handleMessage(MappedStringsMessage msg)
	{
		MappedStringManager.getInstance().reset();
		MappedStringManager.getInstance().feed(msg.strings);
	}

	// Token: 0x06000E65 RID: 3685 RVA: 0x000614DC File Offset: 0x0005F6DC
	public void handleMessage(CardTypesMessage msg)
	{
		CardTypeManager.getInstance().feed(msg.cardTypes);
		msg.consume();
		DateTime now = DateTime.Now;
		int num = this.cleanAssetFiles();
		TimeSpan timeSpan = DateTime.Now.Subtract(now);
	}

	// Token: 0x06000E66 RID: 3686 RVA: 0x0006151C File Offset: 0x0005F71C
	public void handleMessage(CardTypeUpdateMessage msg)
	{
		foreach (GetCardStatsMessage.ICardStatsReceiver cardStatsReceiver in this.cardStatsListeners)
		{
			cardStatsReceiver.onCardTypeUpdated(msg);
		}
	}

	// Token: 0x06000E67 RID: 3687 RVA: 0x00061578 File Offset: 0x0005F778
	private int cleanAssetFiles()
	{
		AssetCleaner assetCleaner = new AssetCleaner(CardTypeManager.getInstance().getAll());
		base.StartCoroutine(assetCleaner.cleanAllCoroutine());
		return 0;
	}

	// Token: 0x06000E68 RID: 3688 RVA: 0x0000B8ED File Offset: 0x00009AED
	public void handleMessage(ProfileDataInfoMessage msg)
	{
		App.MyProfile.ProfileData = msg.profileData;
	}

	// Token: 0x06000E69 RID: 3689 RVA: 0x0000B8FF File Offset: 0x00009AFF
	public void handleMessage(ProfileInfoMessage msg)
	{
		App.MyProfile.ProfileInfo = msg;
	}

	// Token: 0x06000E6A RID: 3690 RVA: 0x000615A4 File Offset: 0x0005F7A4
	public void handleMessage(ProfilePageInfoMessage msg)
	{
		if (msg.unlockedAvatarTypes == null)
		{
			return;
		}
		foreach (int id in msg.unlockedAvatarTypes)
		{
			AvatarPartTypeManager.getInstance().unlock(id);
		}
	}

	// Token: 0x06000E6B RID: 3691 RVA: 0x000615E8 File Offset: 0x0005F7E8
	public void handleMessage(OkMessage m)
	{
		if (m.isType(typeof(JoinLobbyMessage)) && App.SceneValues.lobby != null && App.SceneValues.lobby.enterBattleMessage != null)
		{
			App.Communicator.send(App.SceneValues.lobby.enterBattleMessage);
			App.SceneValues.lobby.enterBattleMessage = null;
			this.enterBattleTime = Time.time;
		}
	}

	// Token: 0x06000E6C RID: 3692 RVA: 0x00061664 File Offset: 0x0005F864
	public void handleMessage(IdolTypesMessage msg)
	{
		IdolTypeManager instance = IdolTypeManager.getInstance();
		instance.reset();
		instance.feed(msg.types);
	}

	// Token: 0x06000E6D RID: 3693 RVA: 0x0006168C File Offset: 0x0005F88C
	public void handleMessage(AvatarTypesMessage msg)
	{
		AvatarPartTypeManager instance = AvatarPartTypeManager.getInstance();
		instance.feed(msg.types);
		instance.feed(new AvatarPart(AvatarPartName.HEAD, "tutorial_weirdo", "MISC")
		{
			id = 1000000001
		});
		foreach (object obj in Enum.GetValues(typeof(AvatarPartName)))
		{
			AvatarPartName type = (AvatarPartName)((int)obj);
			instance.feed(new AvatarPart(type, "blank", "MISC")
			{
				id = GlobalMessageHandler.AvatarId_Blank(type)
			});
		}
		instance.hideSet("MISC");
	}

	// Token: 0x06000E6E RID: 3694 RVA: 0x00061760 File Offset: 0x0005F960
	public void handleMessage(GetCardStatsMessage msg)
	{
		foreach (GetCardStatsMessage.ICardStatsReceiver cardStatsReceiver in this.cardStatsListeners)
		{
			cardStatsReceiver.onCardStatsReceived(msg);
		}
	}

	// Token: 0x06000E6F RID: 3695 RVA: 0x0000B911 File Offset: 0x00009B11
	public static int AvatarId_Blank(AvatarPartName type)
	{
		return (int)((AvatarPartName)1000000000 - (1 + type));
	}

	// Token: 0x06000E70 RID: 3696 RVA: 0x0000B91C File Offset: 0x00009B1C
	public void registerCardStatsListener(GetCardStatsMessage.ICardStatsReceiver listener)
	{
		if (!this.cardStatsListeners.Contains(listener))
		{
			this.cardStatsListeners.Add(listener);
		}
	}

	// Token: 0x06000E71 RID: 3697 RVA: 0x0000B93B File Offset: 0x00009B3B
	public void unregisterCardStatsListener(GetCardStatsMessage.ICardStatsReceiver listener)
	{
		this.cardStatsListeners.Remove(listener);
	}

	// Token: 0x06000E72 RID: 3698 RVA: 0x000617BC File Offset: 0x0005F9BC
	public void onPostMessage(Message msg)
	{
		Message message = this.isPostHandlingMessage;
		this.isPostHandlingMessage = msg;
		foreach (Attribute attribute in Attribute.GetCustomAttributes(msg.GetType()))
		{
			if (attribute.GetType() == typeof(Update))
			{
				Update update = (Update)attribute;
				foreach (Type type in update.messageTypes)
				{
					if (type == typeof(ProfileDataInfoMessage))
					{
						if (message != null)
						{
							string text = message.msg + " -> " + msg.msg;
							Debug.LogWarning("Chain of PostMessage handlers, is this intended? " + text);
						}
						App.Communicator.send(new ProfileDataInfoMessage());
					}
				}
			}
		}
		this.isPostHandlingMessage = null;
	}

	// Token: 0x06000E73 RID: 3699 RVA: 0x00061898 File Offset: 0x0005FA98
	public void internalHandleMessage(EMCostUpdate m)
	{
		foreach (GetCardStatsMessage.ICardStatsReceiver cardStatsReceiver in this.cardStatsListeners)
		{
			cardStatsReceiver.onCostUpdate(m);
		}
	}

	// Token: 0x06000E74 RID: 3700 RVA: 0x000618F4 File Offset: 0x0005FAF4
	private void HandleSceneLoaderOnSceneHasLoaded(string sceneName, string previous)
	{
		if (previous == "_BattleModeView")
		{
			App.LobbyMenu.SetEnabled(true);
			if (App.SceneValues.lobby != null && App.SceneValues.lobby.enterBattleMessage != null)
			{
				this.enterBattleTime = Time.time;
			}
		}
	}

	// Token: 0x04000B1A RID: 2842
	private const int AvatarId_CustomStart = 1000000000;

	// Token: 0x04000B1B RID: 2843
	public const int AvatarId_Misc_TutorialMaster = 1000000001;

	// Token: 0x04000B1C RID: 2844
	private float enterBattleTime = -1000f;

	// Token: 0x04000B1D RID: 2845
	private GUIStyle regularStyle;

	// Token: 0x04000B1E RID: 2846
	private float timeScale = 1f;

	// Token: 0x04000B1F RID: 2847
	private List<GetCardStatsMessage.ICardStatsReceiver> cardStatsListeners = new List<GetCardStatsMessage.ICardStatsReceiver>();

	// Token: 0x04000B20 RID: 2848
	private Message isPostHandlingMessage;

	// Token: 0x020001C6 RID: 454
	// (Invoke) Token: 0x06000E76 RID: 3702
	public delegate void EmptyDelegate();
}
