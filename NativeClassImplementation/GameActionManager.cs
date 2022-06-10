using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001C3 RID: 451
public class GameActionManager : AbstractCommListener, IOkCallback, ICancelCallback, IOkCancelCallback, IOkStringCallback, IOkStringCancelCallback, IDeckCallback
{
	// Token: 0x06000E2C RID: 3628 RVA: 0x0000B529 File Offset: 0x00009729
	public static GameActionManager.StartType fromMpGameType(GameType gameType)
	{
		if (gameType == GameType.MP_QUICKMATCH)
		{
			return GameActionManager.StartType.START_MULTIPLAYER_QUICK;
		}
		if (gameType == GameType.MP_LIMITED)
		{
			return GameActionManager.StartType.START_MULTIPLAYER_LIMITED;
		}
		if (gameType == GameType.MP_RANKED)
		{
			return GameActionManager.StartType.START_MULTIPLAYER_RANKED;
		}
		throw new ArgumentException(gameType + " not implemented");
	}

	// Token: 0x06000E2D RID: 3629 RVA: 0x00060A14 File Offset: 0x0005EC14
	public void StartGame(GameActionManager.StartType startType)
	{
		this.startType = startType;
		switch (startType)
		{
		case GameActionManager.StartType.START_TUTORIAL:
			App.Communicator.send(new GetTutorialInfoMessage());
			this.startGameBuilder = new PlayGameBuilder(new PlaySinglePlayerTutorialMessage());
			this.ChooseTutorialChallenge();
			break;
		case GameActionManager.StartType.START_SINGLEPLAYER_QUICK:
			this.startGameBuilder = new PlayGameBuilder(new PlaySinglePlayerQuickMatchMessage());
			App.Popups.ShowMultibutton(this, "difficulty", "Choose opponent", new GUIContent[]
			{
				new GUIContent("Easy"),
				new GUIContent("Medium"),
				new GUIContent("Hard").lockDemo(),
				new GUIContent("Custom rules").lockDemo()
			});
			break;
		case GameActionManager.StartType.START_SINGLEPLAYER_ADVENTURE:
			this.startGameBuilder = new PlayGameBuilder(new PlaySinglePlayerAdventureMatchMessage());
			this.ChooseDeck();
			break;
		case GameActionManager.StartType.START_MULTIPLAYER_QUICK:
			this.startGameBuilder = new PlayGameBuilder(new PlayMultiPlayerQuickMatchMessage());
			this.ChooseDeck();
			break;
		case GameActionManager.StartType.START_MULTIPLAYER_RANKED:
			this.startGameBuilder = new PlayGameBuilder(new PlayMultiPlayerRankedMatchMessage());
			this.ChooseDeck();
			break;
		case GameActionManager.StartType.START_MULTIPLAYER_LIMITED:
			this.startGameBuilder = new PlayGameBuilder(new PlayMultiPlayerLimitedMessage());
			this.ChooseDeckLimited();
			break;
		case GameActionManager.StartType.START_DEV_GAME:
			this.startGameBuilder = new PlayGameBuilder(new PlayDevGameMessage());
			this.ChooseDeck();
			break;
		case GameActionManager.StartType.ACCEPT_MULTIPLAYER_CHALLENGE:
			this.startGameBuilder = new PlayGameBuilder(new GameChallengeAcceptMessage());
			this.startGameBuilder.setOpponent(this.challengeFromUserId);
			this.ChooseDeckOrFinish(this.chooseDeck);
			break;
		case GameActionManager.StartType.START_TOWER_CHALLENGE:
			App.Communicator.send(new GetTowerInfoMessage());
			this.startGameBuilder = new PlayGameBuilder(new PlaySinglePlayerTowerMatchMessage());
			this.ChooseTowerChallenge();
			break;
		}
	}

	// Token: 0x06000E2E RID: 3630 RVA: 0x00060BDC File Offset: 0x0005EDDC
	public void StartLimited(string deckName)
	{
		this.startType = GameActionManager.StartType.START_MULTIPLAYER_LIMITED;
		this.startGameBuilder = new PlayGameBuilder(new PlayMultiPlayerLimitedMessage());
		Log.info("Playing limited with deck: " + deckName);
		this.startGameBuilder.setDeck(deckName);
		this.BuildAndDispatchMessage();
		App.InviteManager.declineAllNonActiveInvites();
	}

	// Token: 0x06000E2F RID: 3631 RVA: 0x0000B55B File Offset: 0x0000975B
	public void AcceptChallenge(int userId, bool chooseDeck)
	{
		this.challengeFromUserId = userId;
		this.chooseDeck = chooseDeck;
		this.StartGame(GameActionManager.StartType.ACCEPT_MULTIPLAYER_CHALLENGE);
	}

	// Token: 0x06000E30 RID: 3632 RVA: 0x0000B572 File Offset: 0x00009772
	public void Initiate()
	{
		App.Communicator.addListener(this);
		App.Communicator.send(new DeckListMessage());
	}

	// Token: 0x06000E31 RID: 3633 RVA: 0x00060C30 File Offset: 0x0005EE30
	private void CustomSetupCode(bool isSinglePlayer, string challengee)
	{
		SceneValues.SV_CustomGames customGames = App.SceneValues.customGames;
		if (isSinglePlayer != customGames.isSinglePlayer)
		{
			customGames.lastSearch = string.Empty;
		}
		if (string.IsNullOrEmpty(customGames.lastSearch))
		{
			this.RefreshCustomGamesList(isSinglePlayer, null);
		}
		customGames.isSinglePlayer = isSinglePlayer;
		customGames.challengee = challengee;
		SceneLoader.loadScene("_CustomGames");
	}

	// Token: 0x06000E32 RID: 3634 RVA: 0x00060C90 File Offset: 0x0005EE90
	public void RefreshCustomGamesList(bool isSinglePlayer, string search)
	{
		search = ((!string.IsNullOrEmpty(search)) ? search : null);
		if (isSinglePlayer)
		{
			App.Communicator.send(new GetCustomGamesSpMessage(search));
		}
		else
		{
			App.Communicator.send(new GetCustomGamesMpMessage(search));
		}
	}

	// Token: 0x06000E33 RID: 3635 RVA: 0x0000B590 File Offset: 0x00009790
	private void ChooseDeck()
	{
		App.Popups.ShowDeckSelector(this, this, this.decks, false, false);
		App.Communicator.send(new DeckListMessage());
	}

	// Token: 0x06000E34 RID: 3636 RVA: 0x0000B5B6 File Offset: 0x000097B6
	private void ChooseDeckLimited()
	{
		this.decks.Clear();
		App.Popups.UpdateDecks(this.decks);
		App.Popups.ShowDeckSelector(this, this, this.decks, false, false);
	}

	// Token: 0x06000E35 RID: 3637 RVA: 0x0000B5E7 File Offset: 0x000097E7
	private void ChooseTowerChallenge()
	{
		App.Popups.ShowTowerChallengeSelector(this, this);
	}

	// Token: 0x06000E36 RID: 3638 RVA: 0x0000B5F5 File Offset: 0x000097F5
	private void ChooseTutorialChallenge()
	{
		App.Popups.ShowTutorialChallengeSelector(this, this);
	}

	// Token: 0x06000E37 RID: 3639 RVA: 0x00060CE0 File Offset: 0x0005EEE0
	public override void handleMessage(Message message)
	{
		if (message is DeckListMessage)
		{
			this.decks = new List<DeckInfo>(((DeckListMessage)message).decks);
			App.InviteManager.setDeckList(this.decks);
			App.Popups.UpdateDecks(this.decks);
		}
		if (message is OkMessage)
		{
			OkMessage okMessage = message as OkMessage;
			if (okMessage.isType(typeof(JoinLobbyMessage)) && App.SceneValues.lobby != null && App.SceneValues.lobby.nextGame != null)
			{
				this.pendingCustomGame = App.SceneValues.lobby.nextGame;
				App.SceneValues.lobby.nextGame = null;
				CustomGamesImpl.ShowGameDetailsPopup(this, "next_custom_game", this.pendingCustomGame, false);
			}
		}
	}

	// Token: 0x06000E38 RID: 3640 RVA: 0x0000B603 File Offset: 0x00009803
	public void ChallengeUser(ChatUser user)
	{
		this.startGameBuilder = new PlayGameBuilder(new GameChallengeRequestMessage(user.profileId));
		this.ChooseDeck();
	}

	// Token: 0x06000E39 RID: 3641 RVA: 0x0000B621 File Offset: 0x00009821
	public void CustomChallengeUser(ChatUser user)
	{
		this.startGameBuilder = new PlayGameBuilder(new GameChallengeRequestMessage(user.profileId));
		this.CustomSetupCode(false, user.name);
	}

	// Token: 0x06000E3A RID: 3642 RVA: 0x0000B646 File Offset: 0x00009846
	public void TradeUser(ChatUser user)
	{
		App.Communicator.send(new TradeInviteMessage(user.profileId));
	}

	// Token: 0x06000E3B RID: 3643 RVA: 0x0000B65E File Offset: 0x0000985E
	public void ProfileUser(ChatUser user)
	{
		App.SceneValues.profilePage = new SceneValues.SV_ProfilePage(user.profileId);
		SceneLoader.loadScene("_Profile");
	}

	// Token: 0x06000E3C RID: 3644 RVA: 0x0000B67F File Offset: 0x0000987F
	public void SpectateUser(ProfileInfo profile)
	{
		App.Communicator.send(new SpectateUserRequestMessage(profile));
	}

	// Token: 0x06000E3D RID: 3645 RVA: 0x0000B692 File Offset: 0x00009892
	private void BuildAndDispatchMessage()
	{
		if (Time.time - this.lastDispatchTime >= 0.2f)
		{
			App.Communicator.send(this.startGameBuilder);
			this.lastDispatchTime = Time.time;
		}
		this.startType = GameActionManager.StartType.START_UNKNOWN;
	}

	// Token: 0x06000E3E RID: 3646 RVA: 0x00060DB0 File Offset: 0x0005EFB0
	public void CustomGameChosen(int levelId, bool chooseDeck, bool chooseDifficulty)
	{
		this.startGameBuilder.setCustomGameId(levelId);
		this.chooseDeck = chooseDeck;
		if (chooseDifficulty)
		{
			App.Popups.ShowMultibutton(this, "custom_difficulty", "Choose opponent", new GUIContent[]
			{
				new GUIContent("Easy"),
				new GUIContent("Medium"),
				new GUIContent("Hard").lockDemo()
			});
		}
		else
		{
			this.ChooseDeckOrFinish(chooseDeck);
		}
	}

	// Token: 0x06000E3F RID: 3647 RVA: 0x00060E2C File Offset: 0x0005F02C
	public void PopupTowerChallengeChosen(int levelId, bool borrowDeck)
	{
		this.startGameBuilder.setLevelId(levelId);
		bool flag = !borrowDeck && this.startType != GameActionManager.StartType.START_TUTORIAL;
		this.ChooseDeckOrFinish(flag);
	}

	// Token: 0x06000E40 RID: 3648 RVA: 0x0000B6D2 File Offset: 0x000098D2
	public void PopupDeckChosen(DeckInfo deck)
	{
		if (!deck.valid)
		{
			return;
		}
		this.startGameBuilder.setDeck(deck.name);
		this.Commit();
	}

	// Token: 0x06000E41 RID: 3649 RVA: 0x0000B6F8 File Offset: 0x000098F8
	private void ChooseDeckOrFinish(bool chooseDeck)
	{
		if (chooseDeck)
		{
			this.ChooseDeck();
		}
		else
		{
			this.Commit();
		}
	}

	// Token: 0x06000E42 RID: 3650 RVA: 0x0000B711 File Offset: 0x00009911
	private void Commit()
	{
		this.BuildAndDispatchMessage();
		App.InviteManager.declineAllNonActiveInvites();
	}

	// Token: 0x06000E43 RID: 3651 RVA: 0x000028DF File Offset: 0x00000ADF
	public void PopupDeckDeleted(DeckInfo deck)
	{
	}

	// Token: 0x06000E44 RID: 3652 RVA: 0x00060E64 File Offset: 0x0005F064
	public void PopupOk(string popupType, string choice)
	{
		if (popupType != null)
		{
			if (GameActionManager.<>f__switch$map16 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("custom_difficulty", 0);
				dictionary.Add("difficulty", 0);
				GameActionManager.<>f__switch$map16 = dictionary;
			}
			int num;
			if (GameActionManager.<>f__switch$map16.TryGetValue(popupType, ref num))
			{
				if (num == 0)
				{
					AiDifficulty aiDifficulty = AiDifficulty.EASY;
					if (choice == "Medium")
					{
						aiDifficulty = AiDifficulty.MEDIUM;
					}
					else if (choice == "Hard")
					{
						aiDifficulty = AiDifficulty.HARD;
					}
					else
					{
						if (choice == "Custom rules")
						{
							this.startGameBuilder = new PlayGameBuilder(new PlaySinglePlayerCustomQuickmatchMessage());
							this.CustomSetupCode(true, null);
							return;
						}
						if (choice.ToLowerInvariant().Contains("player"))
						{
							this.startGameBuilder = new PlayGameBuilder(new PlaySinglePlayerLabMatchMessage());
							this.ChooseDeck();
							return;
						}
					}
					Log.info("Robot Chosen: " + aiDifficulty);
					this.startGameBuilder.setRobotName(aiDifficulty);
					if (popupType == "custom_difficulty")
					{
						this.ChooseDeckOrFinish(this.chooseDeck);
					}
					else
					{
						this.ChooseDeck();
					}
				}
			}
		}
	}

	// Token: 0x06000E45 RID: 3653 RVA: 0x00060F98 File Offset: 0x0005F198
	public void PopupOk(string popupType)
	{
		if (popupType == "delete_ruleset")
		{
			App.Communicator.send(new DeleteCustomGameMessage(this._deleteCustomGameId));
		}
		if (popupType == "next_custom_game")
		{
			this.startType = GameActionManager.StartType.START_SINGLEPLAYER_QUICK;
			this.startGameBuilder = new PlayGameBuilder(new PlaySinglePlayerCustomQuickmatchMessage());
			this.CustomGameChosen(this.pendingCustomGame.id.Value, this.pendingCustomGame.chooseDeckP1, this.pendingCustomGame.chooseDifficulty);
		}
	}

	// Token: 0x06000E46 RID: 3654 RVA: 0x00061020 File Offset: 0x0005F220
	public void PopupCancel(string popupType)
	{
		if (popupType != null)
		{
			if (GameActionManager.<>f__switch$map17 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
				dictionary.Add("difficulty", 0);
				GameActionManager.<>f__switch$map17 = dictionary;
			}
			int num;
			if (GameActionManager.<>f__switch$map17.TryGetValue(popupType, ref num))
			{
				if (num != 0)
				{
				}
			}
		}
	}

	// Token: 0x06000E47 RID: 3655 RVA: 0x000028DF File Offset: 0x00000ADF
	public void SaveCustomGame(bool isSinglePlayer, string name, string setupCode, bool isCompileOnly)
	{
	}

	// Token: 0x06000E48 RID: 3656 RVA: 0x000028DF File Offset: 0x00000ADF
	public void DeleteCustomGame(int customGameId)
	{
	}

	// Token: 0x04000B05 RID: 2821
	private PlayGameBuilder startGameBuilder;

	// Token: 0x04000B06 RID: 2822
	private List<DeckInfo> decks = new List<DeckInfo>();

	// Token: 0x04000B07 RID: 2823
	private int challengeFromUserId;

	// Token: 0x04000B08 RID: 2824
	private CustomGameInfo pendingCustomGame;

	// Token: 0x04000B09 RID: 2825
	private bool chooseDeck;

	// Token: 0x04000B0A RID: 2826
	private GameActionManager.StartType startType;

	// Token: 0x04000B0B RID: 2827
	private float lastDispatchTime = -999f;

	// Token: 0x04000B0C RID: 2828
	private int _deleteCustomGameId = -1;

	// Token: 0x020001C4 RID: 452
	public enum StartType
	{
		// Token: 0x04000B10 RID: 2832
		START_UNKNOWN,
		// Token: 0x04000B11 RID: 2833
		START_TUTORIAL,
		// Token: 0x04000B12 RID: 2834
		START_SINGLEPLAYER_QUICK,
		// Token: 0x04000B13 RID: 2835
		START_SINGLEPLAYER_ADVENTURE,
		// Token: 0x04000B14 RID: 2836
		START_MULTIPLAYER_QUICK,
		// Token: 0x04000B15 RID: 2837
		START_MULTIPLAYER_RANKED,
		// Token: 0x04000B16 RID: 2838
		START_MULTIPLAYER_LIMITED,
		// Token: 0x04000B17 RID: 2839
		START_DEV_GAME,
		// Token: 0x04000B18 RID: 2840
		ACCEPT_MULTIPLAYER_CHALLENGE,
		// Token: 0x04000B19 RID: 2841
		START_TOWER_CHALLENGE
	}
}
