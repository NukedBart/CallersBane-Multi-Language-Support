using System;
using CommConfig;
using UnityEngine;

// Token: 0x0200001D RID: 29
public class App : MonoBehaviour
{
	// Token: 0x1700000A RID: 10
	// (get) Token: 0x06000189 RID: 393 RVA: 0x0000341A File Offset: 0x0000161A
	public static Communicator Communicator
	{
		get
		{
			return App.instance.communicator;
		}
	}

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x0600018A RID: 394 RVA: 0x00003426 File Offset: 0x00001626
	public static string PurchaseEncryptionPublicKey
	{
		get
		{
			return App.instance.purchaseEncryptionPublicKey;
		}
	}

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x0600018B RID: 395 RVA: 0x00003432 File Offset: 0x00001632
	public static RSACrypt Crypt
	{
		get
		{
			return App.instance.crypt;
		}
	}

	// Token: 0x0600018C RID: 396 RVA: 0x0000343E File Offset: 0x0000163E
	public static bool Debug_UseDefaultAssetPaths()
	{
		return App.instance.debugUseDefaultAssetPaths;
	}

	// Token: 0x0600018D RID: 397 RVA: 0x0000344A File Offset: 0x0000164A
	public static bool useExternalResources()
	{
		return App.instance.externalResources;
	}

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x0600018E RID: 398 RVA: 0x00003456 File Offset: 0x00001656
	// (set) Token: 0x0600018F RID: 399 RVA: 0x00003462 File Offset: 0x00001662
	public static bool IsBorderlessWindow
	{
		get
		{
			return App.instance.isBorderlessWindow;
		}
		set
		{
			App.instance.isBorderlessWindow = value;
		}
	}

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x06000190 RID: 400 RVA: 0x0000346F File Offset: 0x0000166F
	public static AudioScript AudioScript
	{
		get
		{
			return App.instance.audioScript;
		}
	}

	// Token: 0x1700000F RID: 15
	// (get) Token: 0x06000191 RID: 401 RVA: 0x0000347B File Offset: 0x0000167B
	public static ApplicationController ApplicationController
	{
		get
		{
			return App.instance.applicationController;
		}
	}

	// Token: 0x17000010 RID: 16
	// (get) Token: 0x06000192 RID: 402 RVA: 0x00003487 File Offset: 0x00001687
	public static GlobalMessageHandler GlobalMessageHandler
	{
		get
		{
			return App.instance.globalMessageHandler;
		}
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x06000193 RID: 403 RVA: 0x00003493 File Offset: 0x00001693
	public static IntentionManager Intention
	{
		get
		{
			return App.instance.intentionManager;
		}
	}

	// Token: 0x17000012 RID: 18
	// (get) Token: 0x06000194 RID: 404 RVA: 0x0000349F File Offset: 0x0000169F
	public static ArenaChat ArenaChat
	{
		get
		{
			return App.instance.arenaChat;
		}
	}

	// Token: 0x17000013 RID: 19
	// (get) Token: 0x06000195 RID: 405 RVA: 0x000034AB File Offset: 0x000016AB
	// (set) Token: 0x06000196 RID: 406 RVA: 0x000034B7 File Offset: 0x000016B7
	public static ProfileContainer MyProfile
	{
		get
		{
			return App.instance.myProfile;
		}
		set
		{
			App.instance.myProfile = value;
		}
	}

	// Token: 0x06000197 RID: 407 RVA: 0x000034C4 File Offset: 0x000016C4
	public static bool IsPremium()
	{
		return !App.IsDemo();
	}

	// Token: 0x06000198 RID: 408 RVA: 0x000034CE File Offset: 0x000016CE
	public static bool IsDemo()
	{
		return App.MyProfile != null && App.MyProfile.ProfileInfo.featureType.isDemo();
	}

	// Token: 0x06000199 RID: 409 RVA: 0x000034F1 File Offset: 0x000016F1
	public static bool IsLocked(bool isLockedInDemo)
	{
		return App.IsDemo() && isLockedInDemo;
	}

	// Token: 0x17000014 RID: 20
	// (get) Token: 0x0600019A RID: 410 RVA: 0x00003501 File Offset: 0x00001701
	// (set) Token: 0x0600019B RID: 411 RVA: 0x0000350D File Offset: 0x0000170D
	public static SceneValues SceneValues
	{
		get
		{
			return App.instance.sceneValues;
		}
		set
		{
			App.instance.sceneValues = value;
		}
	}

	// Token: 0x17000015 RID: 21
	// (get) Token: 0x0600019C RID: 412 RVA: 0x0000351A File Offset: 0x0000171A
	// (set) Token: 0x0600019D RID: 413 RVA: 0x00003526 File Offset: 0x00001726
	public static ChatUI ChatUI
	{
		get
		{
			return App.instance.chatUI;
		}
		set
		{
			App.instance.chatUI = value;
		}
	}

	// Token: 0x17000016 RID: 22
	// (get) Token: 0x0600019E RID: 414 RVA: 0x00003533 File Offset: 0x00001733
	// (set) Token: 0x0600019F RID: 415 RVA: 0x0000353F File Offset: 0x0000173F
	public static InviteManager InviteManager
	{
		get
		{
			return App.instance.inviteManager;
		}
		set
		{
			App.instance.inviteManager = value;
		}
	}

	// Token: 0x17000017 RID: 23
	// (get) Token: 0x060001A0 RID: 416 RVA: 0x0000354C File Offset: 0x0000174C
	// (set) Token: 0x060001A1 RID: 417 RVA: 0x00003558 File Offset: 0x00001758
	public static Popups Popups
	{
		get
		{
			return App.instance.popups;
		}
		set
		{
			App.instance.popups = value;
		}
	}

	// Token: 0x17000018 RID: 24
	// (get) Token: 0x060001A2 RID: 418 RVA: 0x00003565 File Offset: 0x00001765
	public static GameActionManager GameActionManager
	{
		get
		{
			return App.instance.gameActionManager;
		}
	}

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x060001A3 RID: 419 RVA: 0x00003571 File Offset: 0x00001771
	public static MouseCursor MouseCursor
	{
		get
		{
			return App.instance.mouseCursor;
		}
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x060001A4 RID: 420 RVA: 0x0000357D File Offset: 0x0000177D
	public static Clocks Clocks
	{
		get
		{
			return App.instance.clocks;
		}
	}

	// Token: 0x1700001B RID: 27
	// (get) Token: 0x060001A5 RID: 421 RVA: 0x00003589 File Offset: 0x00001789
	public static LobbyMenu LobbyMenu
	{
		get
		{
			return App.instance.lobbyMenu;
		}
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x060001A6 RID: 422 RVA: 0x00003595 File Offset: 0x00001795
	public static AssetLoader AssetLoader
	{
		get
		{
			return App.instance.assetLoader;
		}
	}

	// Token: 0x1700001D RID: 29
	// (get) Token: 0x060001A7 RID: 423 RVA: 0x000035A1 File Offset: 0x000017A1
	public static TowerChallengeInfo TowerChallengeInfo
	{
		get
		{
			return App.instance.towerChallengeInfo;
		}
	}

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x060001A8 RID: 424 RVA: 0x000035AD File Offset: 0x000017AD
	public static TowerChallengeInfo TutorialChallengeInfo
	{
		get
		{
			return App.instance.tutorialChallengeInfo;
		}
	}

	// Token: 0x1700001F RID: 31
	// (get) Token: 0x060001A9 RID: 425 RVA: 0x000035B9 File Offset: 0x000017B9
	// (set) Token: 0x060001AA RID: 426 RVA: 0x000035C5 File Offset: 0x000017C5
	public static GetCustomGamesMessage CustomMatchMultiplayerInfo
	{
		get
		{
			return App.instance.customMatchMultiplayerInfo;
		}
		set
		{
			App.instance.customMatchMultiplayerInfo = value;
		}
	}

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x060001AB RID: 427 RVA: 0x000035D2 File Offset: 0x000017D2
	// (set) Token: 0x060001AC RID: 428 RVA: 0x000035DE File Offset: 0x000017DE
	public static GetCustomGamesMessage CustomMatchSingleplayerInfo
	{
		get
		{
			return App.instance.customMatchSingleplayerInfo;
		}
		set
		{
			App.instance.customMatchSingleplayerInfo = value;
		}
	}

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x060001AD RID: 429 RVA: 0x000035EB File Offset: 0x000017EB
	public static Config Config
	{
		get
		{
			return App.instance.config;
		}
	}

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x060001AE RID: 430 RVA: 0x000035F7 File Offset: 0x000017F7
	public static FriendList FriendList
	{
		get
		{
			return App.instance.friendList;
		}
	}

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x060001AF RID: 431 RVA: 0x00003603 File Offset: 0x00001803
	// (set) Token: 0x060001B0 RID: 432 RVA: 0x0000360F File Offset: 0x0000180F
	public static RewardLimitedMessage WaitingReward
	{
		get
		{
			return App.instance.rewardLimited;
		}
		set
		{
			App.instance.rewardLimited = value;
		}
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x060001B1 RID: 433 RVA: 0x0000361C File Offset: 0x0000181C
	// (set) Token: 0x060001B2 RID: 434 RVA: 0x00003628 File Offset: 0x00001828
	public static GUIWrap GUI
	{
		get
		{
			return App.instance.gui;
		}
		set
		{
			App.instance.gui = value;
		}
	}

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x060001B3 RID: 435 RVA: 0x00003635 File Offset: 0x00001835
	// (set) Token: 0x060001B4 RID: 436 RVA: 0x00003641 File Offset: 0x00001841
	public static Tooltip Tooltip
	{
		get
		{
			return App.instance.tooltip;
		}
		set
		{
			App.instance.tooltip = value;
		}
	}

	// Token: 0x060001B5 RID: 437 RVA: 0x0000364E File Offset: 0x0000184E
	public static bool IsParentalConsentNeeded()
	{
		return App.MyProfile.ProfileInfo.isParentalConsentNeeded;
	}

	// Token: 0x060001B6 RID: 438 RVA: 0x0000365F File Offset: 0x0000185F
	public static bool IsChatAllowed()
	{
		return !App.IsParentalConsentNeeded();
	}

	// Token: 0x060001B7 RID: 439 RVA: 0x00003669 File Offset: 0x00001869
	public static void PostCardImagesLoaded()
	{
		App.LobbyMenu.SetButtonsEnabled(true);
		App.ChatUI.SetCanOpenContextMenu(true);
	}

	// Token: 0x060001B8 RID: 440 RVA: 0x00003681 File Offset: 0x00001881
	private static void clearSignoutMessage()
	{
		App.signoutHeader = (App.signoutMessage = null);
	}

	// Token: 0x060001B9 RID: 441 RVA: 0x0000368F File Offset: 0x0000188F
	public static void SignOutWithMessage(string header, string message)
	{
		App.SignOut();
		App.signoutHeader = header;
		App.signoutMessage = message;
	}

	// Token: 0x060001BA RID: 442 RVA: 0x000036A2 File Offset: 0x000018A2
	public static void SignOut()
	{
		App.clearSignoutMessage();
		App.PostSignout();
		App.Config.flushSettings();
		App.RestartApplication();
		SceneLoader.loadScene("_LoginView");
	}

	// Token: 0x060001BB RID: 443 RVA: 0x0001FDC0 File Offset: 0x0001DFC0
	private static void RestartApplication()
	{
		foreach (Object @object in Object.FindObjectsOfType(typeof(GameObject)))
		{
			Object.Destroy(@object);
		}
	}

	// Token: 0x060001BC RID: 444 RVA: 0x0001FDFC File Offset: 0x0001DFFC
	private static void PostSignout()
	{
		string accessToken = Login.getAccessToken();
		if (accessToken != null)
		{
			new InvalidateRequest(accessToken);
		}
		Login.clearAccessToken();
	}

	// Token: 0x060001BD RID: 445 RVA: 0x0001FE24 File Offset: 0x0001E024
	private void Awake()
	{
		if (App.instance == null)
		{
			App.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			this.clocks = base.gameObject.AddComponent<Clocks>();
			this.config = base.GetComponent<Config>();
			this.communicator = base.GetComponent<Communicator>();
			this.crypt = RSACrypt.fromXmlKey("<Modulus>mFCubVhPGG+euHuVQbNObqod/Ji0kRe+oh2OCFR7aV09xYiOklqFQ8jgIgAHvyCcM1JowqfFeJ5jV9up0Lh0eIiv3FPRu14aQS35kMdBLMebSW2DNBkfVsOF3l498WWQS9/THIqIaxbqwRDUxba5btBLTN0/A2y6WWiXl05Xu1c=</Modulus><Exponent>AQAB</Exponent>");
			this.commandLine = base.GetComponent<CommandLine>();
			if (this.communicator.UsePurchaseEnvironment == PurchaseEnvironment.Production)
			{
				this.purchaseEncryptionPublicKey = "MIIBCgKCAQEAyYuC3tnCCcwQYyoPTqYAxMgNN6gpmamO1BmYZjQUDwtZo/VhorlnKZgpQKfcsTniHTSgJV6jOdg1YHzPNuRd4pjgw61hKN+cqfNkV4SFqU29xghJ8fEA2bMoJn+lDiMWdMDI2ruHH73bVIVxoOpYYdUmKMjyRFr4OX3ERMACn2u6QnOe8vMbtBr6ZJaXmjW0x+AT4Vy5nS6LuJuMNU27g/tYED2wMTesxpeYqyzE6B75IrfMs51Ur/FfRMB4f3ih57v5SXWm96KgjhEZbCQJiqMjPWeyEE1Sflaapw5x716UcSsZEqe/znA5UipcDz1zqOYiXG+yuPf/bGhMq6QA8wIDAQAB";
			}
			if (this.communicator.UsePurchaseEnvironment == PurchaseEnvironment.Staging)
			{
				this.purchaseEncryptionPublicKey = "MIIBCgKCAQEA3tWE56GMDUX5nPNWphqQb9X42/foLckwvk/Xy/45702ZBlrObegqvnuqTEAosByb8gCcOYQ6SKP2XZEGTGcP/trYK9XCoX1P5uOPjdDUojLcVubtnw9G1hjGtMhkEqr/fIjHYrOtiP/zV1JPLb1ZN/SXnJOTlWzqg3cK7WRdB/IlAtyZ0NjUuHdhjV0+MLml7bNA1ybD2fConSswlS/GOFrxUNrl4mAgosqVRN6NZfQYSWJy5QoSOPBgSqx8FD8HWxdsEFlgzXVHwbK1oVizepQt+SCY6EAOrgMo16YqdMs1I/UAAtuK2tnIyi4dFG4Rv4P3pORuNoKzwq46OlMAOQIDAQAB";
			}
			this.globalMessageHandler = base.gameObject.AddComponent<GlobalMessageHandler>();
			this.globalMessageHandler.init();
			if (this.externalResources)
			{
				this.audioScript = base.gameObject.AddComponent<AddonAudioScript>();
			}
			else
			{
				this.audioScript = base.gameObject.AddComponent<AudioScript>();
			}
			this.applicationController = base.GetComponent<ApplicationController>();
			this.arenaChat = base.GetComponent<ArenaChat>();
			this.sceneValues = new SceneValues();
			this.tooltip = base.gameObject.AddComponent<Tooltip>();
			this.chatUI = base.gameObject.GetComponent<ChatUI>();
			this.inviteManager = base.gameObject.GetComponent<InviteManager>();
			this.popups = base.transform.GetComponentInChildren<Popups>();
			this.popups.transform.parent = base.transform;
			this.gameActionManager = base.GetComponent<GameActionManager>();
			this.intentionManager = base.gameObject.AddComponent<IntentionManager>();
			this.mouseCursor = base.GetComponent<MouseCursor>();
			this.lobbyMenu = base.GetComponent<LobbyMenu>();
			this.friendList = base.GetComponent<FriendList>();
			if (App.IsStandalone)
			{
				this.assetLoader = base.gameObject.AddComponent<StandaloneAssetLoader>();
			}
			else
			{
				this.assetLoader = base.gameObject.AddComponent<DownloadAssetLoader>();
			}
			this.sceneLoader = base.gameObject.AddComponent<SceneLoader>();
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060001BE RID: 446 RVA: 0x000036C7 File Offset: 0x000018C7
	public static void ShowSignoutReasonIfAny()
	{
		if (App.signoutHeader != null && App.signoutMessage != null)
		{
			App.Popups.ShowOk(new OkVoidCallback(), "signout-message", App.signoutHeader, App.signoutMessage, "Ok");
			App.clearSignoutMessage();
		}
	}

	// Token: 0x040000AB RID: 171
	private const string publicKeyXml = "<Modulus>mFCubVhPGG+euHuVQbNObqod/Ji0kRe+oh2OCFR7aV09xYiOklqFQ8jgIgAHvyCcM1JowqfFeJ5jV9up0Lh0eIiv3FPRu14aQS35kMdBLMebSW2DNBkfVsOF3l498WWQS9/THIqIaxbqwRDUxba5btBLTN0/A2y6WWiXl05Xu1c=</Modulus><Exponent>AQAB</Exponent>";

	// Token: 0x040000AC RID: 172
	private static App instance = null;

	// Token: 0x040000AD RID: 173
	private Communicator communicator;

	// Token: 0x040000AE RID: 174
	private string purchaseEncryptionPublicKey;

	// Token: 0x040000AF RID: 175
	private RSACrypt crypt;

	// Token: 0x040000B0 RID: 176
	[SerializeField]
	private bool debugUseDefaultAssetPaths;

	// Token: 0x040000B1 RID: 177
	[SerializeField]
	private bool externalResources;

	// Token: 0x040000B2 RID: 178
	private bool isBorderlessWindow;

	// Token: 0x040000B3 RID: 179
	public static bool StartedWithLauncher = false;

	// Token: 0x040000B4 RID: 180
	public static bool HasTriedFirstLogin = false;

	// Token: 0x040000B5 RID: 181
	private AudioScript audioScript;

	// Token: 0x040000B6 RID: 182
	private ApplicationController applicationController;

	// Token: 0x040000B7 RID: 183
	private GlobalMessageHandler globalMessageHandler;

	// Token: 0x040000B8 RID: 184
	private IntentionManager intentionManager;

	// Token: 0x040000B9 RID: 185
	private ArenaChat arenaChat;

	// Token: 0x040000BA RID: 186
	private ProfileContainer myProfile = new ProfileContainer();

	// Token: 0x040000BB RID: 187
	private SceneValues sceneValues;

	// Token: 0x040000BC RID: 188
	private ChatUI chatUI;

	// Token: 0x040000BD RID: 189
	private InviteManager inviteManager;

	// Token: 0x040000BE RID: 190
	private Popups popups;

	// Token: 0x040000BF RID: 191
	private GameActionManager gameActionManager;

	// Token: 0x040000C0 RID: 192
	private MouseCursor mouseCursor;

	// Token: 0x040000C1 RID: 193
	private Clocks clocks;

	// Token: 0x040000C2 RID: 194
	private LobbyMenu lobbyMenu;

	// Token: 0x040000C3 RID: 195
	private AssetLoader assetLoader;

	// Token: 0x040000C4 RID: 196
	private SceneLoader sceneLoader;

	// Token: 0x040000C5 RID: 197
	private TowerChallengeInfo towerChallengeInfo = new TowerChallengeInfo();

	// Token: 0x040000C6 RID: 198
	private TowerChallengeInfo tutorialChallengeInfo = new TowerChallengeInfo();

	// Token: 0x040000C7 RID: 199
	private GetCustomGamesMessage customMatchMultiplayerInfo = new GetCustomGamesMpMessage();

	// Token: 0x040000C8 RID: 200
	private GetCustomGamesMessage customMatchSingleplayerInfo = new GetCustomGamesSpMessage();

	// Token: 0x040000C9 RID: 201
	private Config config;

	// Token: 0x040000CA RID: 202
	private FriendList friendList;

	// Token: 0x040000CB RID: 203
	private RewardLimitedMessage rewardLimited;

	// Token: 0x040000CC RID: 204
	private GUIWrap gui = new GUIWrap();

	// Token: 0x040000CD RID: 205
	private Tooltip tooltip;

	// Token: 0x040000CE RID: 206
	public static ServerSettingsMessage ServerSettings = new ServerSettingsMessage();

	// Token: 0x040000CF RID: 207
	private CommandLine commandLine;

	// Token: 0x040000D0 RID: 208
	public static readonly bool IsStandalone = true;

	// Token: 0x040000D1 RID: 209
	private static string signoutHeader;

	// Token: 0x040000D2 RID: 210
	private static string signoutMessage;
}
