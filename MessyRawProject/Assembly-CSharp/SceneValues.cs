using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003BA RID: 954
public class SceneValues
{
	// Token: 0x04001283 RID: 4739
	public string deckName;

	// Token: 0x04001284 RID: 4740
	public bool loadTradeSystem;

	// Token: 0x04001285 RID: 4741
	public TradeResponseMessage tradeResponseMessage;

	// Token: 0x04001286 RID: 4742
	public SceneValues.SV_BattleMode battleMode = new SceneValues.SV_BattleMode(GameMode.Undefined);

	// Token: 0x04001287 RID: 4743
	public SceneValues.SV_DeckBuilder deckBuilder;

	// Token: 0x04001288 RID: 4744
	public SceneValues.SV_ProfilePage profilePage = new SceneValues.SV_ProfilePage(0);

	// Token: 0x04001289 RID: 4745
	public SceneValues.SV_Limited limited;

	// Token: 0x0400128A RID: 4746
	public SceneValues.SV_Lobby lobby;

	// Token: 0x0400128B RID: 4747
	public SceneValues.SV_Store store = new SceneValues.SV_Store();

	// Token: 0x0400128C RID: 4748
	public SceneValues.SV_Marketplace marketplace;

	// Token: 0x0400128D RID: 4749
	public SceneValues.SV_CustomGames customGames = new SceneValues.SV_CustomGames();

	// Token: 0x0400128E RID: 4750
	public SceneValues.SV_Watch watch = new SceneValues.SV_Watch();

	// Token: 0x0400128F RID: 4751
	public SceneValues.SV_SelectPreconstructed selectPreconstructed = new SceneValues.SV_SelectPreconstructed();

	// Token: 0x020003BB RID: 955
	public class SV_BattleMode
	{
		// Token: 0x06001554 RID: 5460 RVA: 0x0000F971 File Offset: 0x0000DB71
		public SV_BattleMode(GameMode gameMode)
		{
			this.gameMode = gameMode;
		}

		// Token: 0x06001555 RID: 5461 RVA: 0x0000F987 File Offset: 0x0000DB87
		public bool isReplay()
		{
			return this.gameMode == GameMode.Replay;
		}

		// Token: 0x06001556 RID: 5462 RVA: 0x0000F992 File Offset: 0x0000DB92
		public bool isSpectate()
		{
			return this.gameMode == GameMode.Spectate;
		}

		// Token: 0x06001557 RID: 5463 RVA: 0x0000F99D File Offset: 0x0000DB9D
		public void setupForSpectate(SpectateRedirectMessage m)
		{
			this.gameMode = GameMode.Spectate;
			this.address = new IpPort(m.ip, m.port);
		}

		// Token: 0x06001558 RID: 5464 RVA: 0x0000F9BD File Offset: 0x0000DBBD
		public void setupForGame(BattleRedirectMessage m)
		{
			this.gameMode = GameMode.Play;
			this.address = new IpPort(m.ip, m.port);
		}

		// Token: 0x06001559 RID: 5465 RVA: 0x0000F9DD File Offset: 0x0000DBDD
		public void setupForReplay(string data)
		{
			this.gameMode = GameMode.Replay;
			this.gameLog = data;
			this.msg = null;
		}

		// Token: 0x0600155A RID: 5466 RVA: 0x0000F9F4 File Offset: 0x0000DBF4
		public long gameId()
		{
			return (this.msg == null || this.gameMode == GameMode.Undefined) ? -1L : this.msg.gameId;
		}

		// Token: 0x04001290 RID: 4752
		public GameMode gameMode = GameMode.Play;

		// Token: 0x04001291 RID: 4753
		public GameInfoMessage msg;

		// Token: 0x04001292 RID: 4754
		public EMEndGame end;

		// Token: 0x04001293 RID: 4755
		public GameObject specCommGameObject;

		// Token: 0x04001294 RID: 4756
		public IpPort address;

		// Token: 0x04001295 RID: 4757
		public string gameLog;
	}

	// Token: 0x020003BC RID: 956
	public class SV_DeckBuilder
	{
		// Token: 0x0600155C RID: 5468 RVA: 0x0000FA1E File Offset: 0x0000DC1E
		public void markRead()
		{
			this.isLimitedMode = false;
			this.limitedDeckInfo = null;
		}

		// Token: 0x04001296 RID: 4758
		public DeckCardsMessage tableState;

		// Token: 0x04001297 RID: 4759
		public bool isLimitedMode;

		// Token: 0x04001298 RID: 4760
		public LimitedDeckInfo limitedDeckInfo;
	}

	// Token: 0x020003BD RID: 957
	public class SV_ProfilePage
	{
		// Token: 0x0600155D RID: 5469 RVA: 0x00002DDA File Offset: 0x00000FDA
		public SV_ProfilePage()
		{
		}

		// Token: 0x0600155E RID: 5470 RVA: 0x0000FA2E File Offset: 0x0000DC2E
		public SV_ProfilePage(int profileId)
		{
			this.profileId = profileId;
		}

		// Token: 0x0600155F RID: 5471 RVA: 0x0000FA3D File Offset: 0x0000DC3D
		public void clear()
		{
			this.profileId = 0;
			this.showAchievementId = -1;
		}

		// Token: 0x06001560 RID: 5472 RVA: 0x0000FA4D File Offset: 0x0000DC4D
		public bool isMe()
		{
			return this.profileId == 0 || this.profileId == App.MyProfile.ProfileInfo.id;
		}

		// Token: 0x04001299 RID: 4761
		public bool wasMe;

		// Token: 0x0400129A RID: 4762
		public int profileId;

		// Token: 0x0400129B RID: 4763
		public int showAchievementId;
	}

	// Token: 0x020003BE RID: 958
	public class SV_Limited
	{
		// Token: 0x0400129C RID: 4764
		public int cardsPerRow;

		// Token: 0x0400129D RID: 4765
		public int targetCollectionSize;
	}

	// Token: 0x020003BF RID: 959
	public class SV_Lobby
	{
		// Token: 0x0400129E RID: 4766
		public bool enterLimited;

		// Token: 0x0400129F RID: 4767
		public Message enterBattleMessage;

		// Token: 0x040012A0 RID: 4768
		public CustomGameInfo nextGame;
	}

	// Token: 0x020003C0 RID: 960
	public class SV_Store
	{
		// Token: 0x06001564 RID: 5476 RVA: 0x0000FA74 File Offset: 0x0000DC74
		public void reset()
		{
			this.openShardPurchasePopup = false;
			this.openBuyGamePopup = false;
		}

		// Token: 0x040012A1 RID: 4769
		public bool openShardPurchasePopup;

		// Token: 0x040012A2 RID: 4770
		public bool openBuyGamePopup;
	}

	// Token: 0x020003C1 RID: 961
	public class SV_Marketplace
	{
		// Token: 0x040012A3 RID: 4771
		public bool openSellView;
	}

	// Token: 0x020003C2 RID: 962
	public class SV_CustomGames
	{
		// Token: 0x040012A4 RID: 4772
		public bool isSinglePlayer;

		// Token: 0x040012A5 RID: 4773
		public string challengee;

		// Token: 0x040012A6 RID: 4774
		public string lastSearch = string.Empty;
	}

	// Token: 0x020003C3 RID: 963
	public class SV_Watch
	{
		// Token: 0x040012A7 RID: 4775
		public string profileName;
	}

	// Token: 0x020003C4 RID: 964
	public class SV_SelectPreconstructed
	{
		// Token: 0x06001569 RID: 5481 RVA: 0x0000FAAA File Offset: 0x0000DCAA
		public void Add(MessageMessage.Type type)
		{
			this._types.AddLast(type);
		}

		// Token: 0x0600156A RID: 5482 RVA: 0x00083028 File Offset: 0x00081228
		public MessageMessage.Type Pop()
		{
			if (this.IsEmpty())
			{
				throw new InvalidOperationException("Popping from empty queue!");
			}
			MessageMessage.Type value = this._types.First.Value;
			this._types.RemoveFirst();
			return value;
		}

		// Token: 0x0600156B RID: 5483 RVA: 0x0000FAB9 File Offset: 0x0000DCB9
		public static ResourceType toResource(MessageMessage.Type type)
		{
			if (type == MessageMessage.Type.DECAY_START_DECK)
			{
				return ResourceType.DECAY;
			}
			if (type == MessageMessage.Type.ORDER_START_DECK)
			{
				return ResourceType.ORDER;
			}
			if (type == MessageMessage.Type.ENERGY_START_DECK)
			{
				return ResourceType.ENERGY;
			}
			if (type == MessageMessage.Type.GROWTH_START_DECK)
			{
				return ResourceType.GROWTH;
			}
			return ResourceType.NONE;
		}

		// Token: 0x0600156C RID: 5484 RVA: 0x0000FADF File Offset: 0x0000DCDF
		public void Clear()
		{
			this._types.Clear();
		}

		// Token: 0x0600156D RID: 5485 RVA: 0x0000FAEC File Offset: 0x0000DCEC
		public bool IsEmpty()
		{
			return this._types.First == null;
		}

		// Token: 0x040012A8 RID: 4776
		private LinkedList<MessageMessage.Type> _types = new LinkedList<MessageMessage.Type>();
	}
}
