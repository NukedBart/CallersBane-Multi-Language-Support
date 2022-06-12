using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001F3 RID: 499
public class InviteManager : AbstractCommListener, IOkCallback, ICancelCallback, IOkCancelCallback
{
	// Token: 0x06000F9A RID: 3994 RVA: 0x000672B8 File Offset: 0x000654B8
	private void Start()
	{
		this.showAll = false;
		this.inviteSize = new Rect(0f, 0f, (float)Screen.width * 0.15f, (float)Screen.width * 0.15f * 0.324f);
		this.inviteSkin = (GUISkin)ResourceManager.Load("_GUISkins/InviteSkin");
		this.inviteSkin.label.fontSize = Screen.height / 25;
		this.inviteCloseSkin = (GUISkin)ResourceManager.Load("_GUISkins/InviteCloseSkin");
		this.chatSkin = (GUISkin)ResourceManager.Load("_GUISkins/ChatButton");
		this.emptyStyle = new GUIStyle();
		this._invites = new List<Invite>();
		App.Communicator.addListener(this);
	}

	// Token: 0x06000F9B RID: 3995 RVA: 0x0000C7DF File Offset: 0x0000A9DF
	public void addInvite(string label, string fullText)
	{
		this.addInvite(label, fullText, Invite.InviteType.DEFAULT);
	}

	// Token: 0x06000F9C RID: 3996 RVA: 0x00067378 File Offset: 0x00065578
	public void addInvite(string label, string fullText, Invite.InviteType type)
	{
		InviteManager.WaitingInvite waitingInvite = new InviteManager.WaitingInvite();
		waitingInvite.label = label;
		waitingInvite.fullText = fullText;
		waitingInvite.inviteType = type;
		this._waitingInvites.Add(waitingInvite);
	}

	// Token: 0x06000F9D RID: 3997 RVA: 0x000673AC File Offset: 0x000655AC
	private void addInvite(Message msg)
	{
		InviteManager.WaitingInvite waitingInvite = new InviteManager.WaitingInvite();
		waitingInvite.message = msg;
		this._waitingInvites.Add(waitingInvite);
	}

	// Token: 0x06000F9E RID: 3998 RVA: 0x000673D4 File Offset: 0x000655D4
	private bool allowInvite(IEnumerable<IAllowNotification> listeners, InviteManager.WaitingInvite w)
	{
		foreach (IAllowNotification allowNotification in listeners)
		{
			if (!allowNotification.allowInviteNotification(w))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000F9F RID: 3999 RVA: 0x00067438 File Offset: 0x00065638
	private void popWaitingInvites()
	{
		if (SceneLoader.isScene(new string[]
		{
			"_LoginView"
		}))
		{
			return;
		}
		List<IAllowNotification> listenersOfType = App.Communicator.getListenersOfType<IAllowNotification>();
		List<InviteManager.WaitingInvite> list = new List<InviteManager.WaitingInvite>();
		foreach (InviteManager.WaitingInvite waitingInvite in this._waitingInvites)
		{
			if (!this.allowInvite(listenersOfType, waitingInvite))
			{
				list.Add(waitingInvite);
			}
			else if (waitingInvite.message == null)
			{
				this.startInvite(waitingInvite.label, waitingInvite.fullText, waitingInvite.inviteType);
			}
			else
			{
				this.startInvite(waitingInvite.message);
			}
		}
		this._waitingInvites.Clear();
		this._waitingInvites.AddRange(list);
	}

	// Token: 0x06000FA0 RID: 4000 RVA: 0x0006751C File Offset: 0x0006571C
	public void startInvite(string label, string fullText, Invite.InviteType type)
	{
		Invite invite = this.createInvite();
		invite.init(label, fullText, type);
		App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_notification");
	}

	// Token: 0x06000FA1 RID: 4001 RVA: 0x00067548 File Offset: 0x00065748
	public void startInvite(Message msg)
	{
		Rect size = this.inviteSize;
		if (msg is AchievementUnlockedMessage)
		{
			float num = 1.85f * (AspectRatio._4_3.ratio / AspectRatio.now.ratio);
			size.width *= num;
		}
		Invite invite = this.createInvite(size);
		invite.init(msg);
		if (msg is GameMatchMessage)
		{
			App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_notification_important");
		}
		else
		{
			App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_notification");
		}
	}

	// Token: 0x06000FA2 RID: 4002 RVA: 0x0000C7EA File Offset: 0x0000A9EA
	private Invite createInvite()
	{
		return this.createInvite(this.inviteSize);
	}

	// Token: 0x06000FA3 RID: 4003 RVA: 0x000675D0 File Offset: 0x000657D0
	private Invite createInvite(Rect size)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "Invite";
		Invite invite = gameObject.AddComponent<Invite>();
		invite.size = size;
		this._invites.Add(invite);
		this.setInvitePosition();
		Object.DontDestroyOnLoad(gameObject);
		return invite;
	}

	// Token: 0x06000FA4 RID: 4004 RVA: 0x00067618 File Offset: 0x00065818
	private void Update()
	{
		this.popInvitesTime += Time.deltaTime;
		if (this.popInvitesTime > 0.2f)
		{
			this.popWaitingInvites();
			this.popInvitesTime = 0f;
		}
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < this._invites.Count; i++)
		{
			float startTime = this._invites[i].startTime;
			if (Time.time - startTime > this._invites[i].timeOut && this._invites[i].timeOut >= 0f)
			{
				Object.Destroy(this._invites[i].gameObject);
				this._invites.RemoveAt(i);
				i--;
				flag2 = true;
			}
			if (Time.time - startTime > 10f)
			{
				flag = true;
			}
		}
		if (flag || flag2)
		{
			this.setInvitePosition();
		}
	}

	// Token: 0x06000FA5 RID: 4005 RVA: 0x00067710 File Offset: 0x00065910
	private void setInvitePosition()
	{
		int num = 0;
		for (int i = this._invites.Count - 1; i >= 0; i--)
		{
			Invite invite = this._invites[i];
			if (this.showAll || Time.time - this._invites[i].startTime <= 10f || this._invites[i].startTime == 0f)
			{
				iTween.MoveTo(this._invites[i].gameObject, iTween.Hash(new object[]
				{
					"y",
					(float)Screen.height * 0.14f + (float)num * this.inviteSize.height,
					"x",
					(float)Screen.width - invite.size.width * 0.99f,
					"time",
					0.75f,
					"easetype",
					iTween.EaseType.easeInOutSine
				}));
				num++;
			}
			else
			{
				iTween.MoveTo(this._invites[i].gameObject, iTween.Hash(new object[]
				{
					"y",
					(float)Screen.height * 0.08f,
					"x",
					Screen.width,
					"time",
					0.25f,
					"easetype",
					iTween.EaseType.easeInOutSine
				}));
			}
		}
	}

	// Token: 0x06000FA6 RID: 4006 RVA: 0x000678B0 File Offset: 0x00065AB0
	public void clearActiveInvite()
	{
		for (int i = 0; i < this._invites.Count; i++)
		{
			if (this._invites[i].inviteActive)
			{
				Object.Destroy(this._invites[i].gameObject);
				this._invites.RemoveAt(i);
			}
		}
	}

	// Token: 0x06000FA7 RID: 4007 RVA: 0x00067914 File Offset: 0x00065B14
	public void removeRankedOrQuickMatchInvite()
	{
		for (int i = 0; i < this._invites.Count; i++)
		{
			if (this._invites[i].message is GameMatchMessage)
			{
				if (this._invites[i].inviteActive)
				{
					App.Popups.KillCurrentPopup();
				}
				Object.Destroy(this._invites[i].gameObject);
				this._invites.RemoveAt(i);
				break;
			}
		}
	}

	// Token: 0x06000FA8 RID: 4008 RVA: 0x000679A0 File Offset: 0x00065BA0
	public void removeChallengeInviteFromUser(int user)
	{
		for (int i = 0; i < this._invites.Count; i++)
		{
			if (this._invites[i].message is GameChallengeMessage && this._invites[i].inviterInfo.from.id == user)
			{
				if (this._invites[i].inviteActive)
				{
					App.Popups.KillCurrentPopup();
				}
				Object.Destroy(this._invites[i].gameObject);
				this._invites.RemoveAt(i);
				break;
			}
		}
	}

	// Token: 0x06000FA9 RID: 4009 RVA: 0x00067A4C File Offset: 0x00065C4C
	public void removeTradeInviteFromUser(int user)
	{
		for (int i = 0; i < this._invites.Count; i++)
		{
			if (this._invites[i].message is TradeInviteForwardMessage && this._invites[i].inviterInfo.from.id == user)
			{
				MonoBehaviour.print("Is it active? " + this._invites[i].inviteActive);
				if (this._invites[i].inviteActive)
				{
					App.Popups.KillCurrentPopup();
				}
				Object.Destroy(this._invites[i].gameObject);
				this._invites.RemoveAt(i);
				break;
			}
		}
	}

	// Token: 0x06000FAA RID: 4010 RVA: 0x00067B20 File Offset: 0x00065D20
	private void OnGUI()
	{
		GUI.depth = 11;
		float num = 0.0875f * (float)Screen.height;
		float num2 = LobbyMenu.getMenuRect().y - num / 3f;
		GUI.BeginGroup(new Rect(0f, num2, (float)Screen.width, (float)Screen.height));
		this.oldSkin = GUI.skin;
		GUI.skin = this.inviteSkin;
		if (!SceneLoader.isScene(new string[]
		{
			"_LoginView",
			"_BattleModeView"
		}))
		{
			Rect rect;
			rect..ctor((float)Screen.width - (float)Screen.height * 0.04f - (float)Screen.height * 0.065f - (float)Screen.height * 0.022f + (float)Screen.height * 0.065f, num * 0.575f, (float)Screen.height * 0.06f, (float)Screen.height * 0.048f);
			GUISkin skin = GUI.skin;
			GUI.skin = this.chatSkin;
			GUI.skin.button.alignment = 3;
			int fontSize = GUI.skin.button.fontSize;
			GUI.skin.button.fontSize = Screen.height / 28;
			if (GUI.Button(rect, " " + this._invites.Count))
			{
				App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
				this.showAll = !this.showAll;
				this.setInvitePosition();
			}
			GUI.skin.button.fontSize = fontSize;
			if (this._invites.Count > 0)
			{
				GUI.color = new Color(1f, 0.8f, 0.4f, 0.1f + Mathf.Sin(Time.time * 4f) / 12f);
				GUI.DrawTexture(rect, ResourceManager.LoadTexture("ChatUI/white"));
				GUI.color = Color.white;
			}
			GUI.skin.button.alignment = 4;
			GUI.DrawTexture(new Rect((float)Screen.width - (float)Screen.height * 0.04f - (float)Screen.height * 0.065f + (float)Screen.height * 0.005f + (float)Screen.height * 0.065f, num * 0.505f + (float)Screen.height * 0.012f, (float)Screen.height * 0.04f, (float)Screen.height * 0.04f * 0.863f), ResourceManager.LoadTexture("InviteManager/notification"));
			GUI.skin = skin;
			TextAnchor alignment = GUI.skin.label.alignment;
			GUI.skin.label.alignment = 4;
			GUI.skin.label.alignment = alignment;
		}
		List<Invite> list = new List<Invite>();
		for (int i = 0; i < this._invites.Count; i++)
		{
			Invite invite = this._invites[i];
			int num3 = 0;
			float num4 = this._invites[i].transform.position.x + 10f + (float)num3;
			if (this.showAll || this._invites[i].transform.position.y > (float)Screen.height * 0.1f)
			{
				Rect rect2;
				rect2..ctor((float)num3 + this._invites[i].transform.position.x + invite.size.width - invite.size.width / 7.5f, this._invites[i].transform.position.y + invite.size.height / 10f, invite.size.width / 10f, invite.size.width / 10f);
				if (GUI.Button(rect2, ResourceManager.LoadTexture("InviteManager/close_x"), string.Empty))
				{
					if (this._invites[i].message is GameMatchMessage)
					{
						App.Communicator.send(new GameMatchDeclineMessage());
					}
					else if (this._invites[i].message is GameChallengeMessage)
					{
						App.Communicator.send(new GameChallengeDeclineMessage(this._invites[i].inviterInfo.from.id));
					}
					else if (this._invites[i].message is TradeInviteForwardMessage)
					{
						App.Communicator.send(new TradeDeclineMessage(this._invites[i].inviterInfo.from.id));
					}
					Object.Destroy(this._invites[i].gameObject);
					this._invites.RemoveAt(i);
					this.setInvitePosition();
					break;
				}
				Rect rect3;
				rect3..ctor((float)num3 + this._invites[i].transform.position.x, this._invites[i].transform.position.y, invite.size.width, invite.size.height);
				GUIStyle button = GUI.skin.button;
				if (invite.message is AchievementUnlockedMessage)
				{
					Rect rect4 = rect3;
					rect4.xMin -= rect4.height * 0.5f;
					GUI.DrawTexture(rect4, ResourceManager.LoadTexture("ChatUI/NotificationTab"));
					button = this.emptyStyle;
				}
				if (GUI.Button(rect3, string.Empty, button))
				{
					App.Popups.KillCurrentPopup();
					for (int j = 0; j < this._invites.Count; j++)
					{
						this._invites[j].inviteActive = false;
					}
					this._invites[i].inviteActive = true;
					if (this._invites[i].message is GameMatchMessage)
					{
						string prefix = this._invites[i].gameType.getPrefix(false);
						App.Popups.ShowOkCancel(this, "joingame", "Join game", "A " + prefix + " match has been found", "Join", "Decline");
					}
					else
					{
						if (invite.message is AchievementUnlockedMessage)
						{
							AchievementUnlockedMessage achievementUnlockedMessage = (AchievementUnlockedMessage)invite.message;
							this._invites.RemoveAt(i--);
							App.SceneValues.profilePage.showAchievementId = (int)achievementUnlockedMessage.typeId;
							SceneLoader.loadScene("_Profile");
							return;
						}
						if (this._invites[i].message is GameChallengeMessage)
						{
							GameChallengeMessage gameChallengeMessage = this._invites[i].message as GameChallengeMessage;
							this.challengeUserId = this._invites[i].inviterInfo.from.id;
							string description = this._invites[i].inviterInfo.from.name + " asked you to play a match";
							string text = (gameChallengeMessage.customGame != null && !gameChallengeMessage.customGame.chooseDeckP2) ? "challenge" : "challenge_choosedeck";
							Log.warning("popupType: " + text);
							if (gameChallengeMessage.isCustom())
							{
								CustomGamesImpl.ShowGameDetailsPopup(this, text, gameChallengeMessage.customGame);
							}
							else
							{
								App.Popups.ShowOkCancel(this, text, "Challenge", description, "Accept", "Decline");
							}
						}
						else if (this._invites[i].message is TradeInviteForwardMessage)
						{
							this.tradeUserId = this._invites[i].inviterInfo.from.id;
							App.Popups.ShowOkCancel(this, "trade", "Trade", this._invites[i].inviterInfo.from.name + " asked you to trade", "Accept", "Decline");
						}
						else if (this._invites[i].message == null || this._invites[i].message is StringNotificationMessage)
						{
							if (this._invites[i].inviteType == Invite.InviteType.FRIEND_NEW_REQUEST)
							{
								App.LobbyMenu.ShowFriendList();
								this._invites.Remove(this._invites[i]);
								i--;
							}
							else
							{
								if (this._invites[i].inviteType == Invite.InviteType.SOLD_MARKET_SCROLLS)
								{
									Log.info("BM: Invite.InviteType.SOLD_MARKET_SCROLLS clicked");
									this._invites.Remove(this._invites[i]);
									i--;
									App.SceneValues.marketplace = new SceneValues.SV_Marketplace();
									App.SceneValues.marketplace.openSellView = true;
									SceneLoader.loadScene("_Marketplace");
									return;
								}
								this.lastClickedInvite = this._invites[i];
								App.Popups.ShowOk(this, "notification", this._invites[i].inviteLabel, this._invites[i].fullText, "Ok");
							}
						}
					}
				}
				GUI.skin = this.inviteCloseSkin;
				if (GUI.Button(new Rect((float)num3 + this._invites[i].transform.position.x + invite.size.width - invite.size.width / 7.5f, this._invites[i].transform.position.y + invite.size.height / 10f, invite.size.width / 10f, invite.size.width / 10f), string.Empty))
				{
				}
				GUI.skin = this.inviteSkin;
				int fontSize2 = GUI.skin.label.fontSize;
				GUI.skin.label.fontSize = Screen.height / 40;
				if (this._invites[i].message is GameMatchMessage)
				{
					GUI.DrawTexture(new Rect(this._invites[i].transform.position.x, this._invites[i].transform.position.y + invite.size.height / 10f, invite.size.width, invite.size.height), ResourceManager.LoadTexture("InviteManager/matchfound"));
				}
				else if (invite.message is AchievementUnlockedMessage)
				{
					AchievementType t = AchievementTypeManager.getInstance().get(((AchievementUnlockedMessage)invite.message).typeId);
					float num5 = 0f;
					float height = invite.size.height;
					Rect r = GeomUtil.scaleCentered(new Rect(invite.transform.position.x + num5, invite.transform.position.y + num5, height, height), 1.15f);
					float glow = -1f;
					float num6 = (Time.time - (invite.startTime + 0.65f)) / 0.37f;
					if (num6 > 0f && num6 < 1f)
					{
						num6 %= 1f;
						glow = (((double)num6 >= 0.5) ? (1f - num6) : num6) * 2.4f;
					}
					Color color = GUI.color;
					r.x += r.height * 0.05f - 0.0035f * (float)Screen.width;
					AchievementList.drawAchievementIcon(GeomUtil.scaleCentered(r, 0.9f), t, glow);
					GUI.color = invite.textColor;
					GUI.Label(new Rect(num4 + height * 0.8f, invite.transform.position.y, invite.size.width * 0.85f - height, invite.size.height), invite.inviteLabel);
					GUI.color = color;
				}
				else if (this._invites[i].message is GameChallengeMessage)
				{
					GUI.DrawTexture(new Rect(this._invites[i].transform.position.x, this._invites[i].transform.position.y, invite.size.width, invite.size.height), ResourceManager.LoadTexture("InviteManager/challenge"));
					GUI.Label(new Rect(this._invites[i].transform.position.x, this._invites[i].transform.position.y + invite.size.height / 10f, invite.size.width, invite.size.height), "Sent by: " + this._invites[i].inviterInfo.from.name);
				}
				else if (this._invites[i].message is TradeInviteForwardMessage)
				{
					GUI.DrawTexture(new Rect(this._invites[i].transform.position.x, this._invites[i].transform.position.y, invite.size.width, invite.size.height), ResourceManager.LoadTexture("InviteManager/trade"));
					GUI.Label(new Rect(this._invites[i].transform.position.x, this._invites[i].transform.position.y + invite.size.height / 10f, invite.size.width, invite.size.height), "Sent by: " + this._invites[i].inviterInfo.from.name);
				}
				else if (this._invites[i].message is GameChallengeResponseMessage && ((GameChallengeResponseMessage)this._invites[i].message).status == GameChallengeResponseMessage.Status.DECLINE)
				{
					if (this._invites[i].inviterInfo.from.id == App.MyProfile.ProfileInfo.id)
					{
						for (int k = 0; k < this._invites.Count; k++)
						{
							if (this._invites[k].message is GameChallengeMessage && ((GameChallengeMessage)this._invites[k].message).from.id == ((GameChallengeResponseMessage)this._invites[i].message).from.id)
							{
								list.Add(this._invites[k]);
								break;
							}
						}
						list.Add(this._invites[i]);
					}
					else
					{
						GUI.Label(new Rect(this._invites[i].transform.position.x + invite.size.width * 0.05f, this._invites[i].transform.position.y - invite.size.height / 10f, invite.size.width * 0.75f, invite.size.height), this._invites[i].inviterInfo.from.name + " declined challenge");
					}
				}
				else if (this._invites[i].message is TradeResponseMessage && ((TradeResponseMessage)this._invites[i].message).status == "DECLINE")
				{
					if (this._invites[i].inviterInfo.from.id == App.MyProfile.ProfileInfo.id)
					{
						for (int l = 0; l < this._invites.Count; l++)
						{
							if (this._invites[l].message is TradeInviteForwardMessage && this._invites[l].inviterInfo.from.id == ((TradeResponseMessage)this._invites[i].message).from.id)
							{
								list.Add(this._invites[l]);
								break;
							}
						}
						list.Add(this._invites[i]);
					}
					else
					{
						GUI.Label(new Rect(this._invites[i].transform.position.x + invite.size.width * 0.05f, this._invites[i].transform.position.y - invite.size.height / 10f, invite.size.width * 0.75f, invite.size.height), this._invites[i].inviterInfo.from.name + " declined trading");
					}
				}
				else
				{
					GUI.Label(new Rect(num4, this._invites[i].transform.position.y, invite.size.width * 0.8f - 10f, invite.size.height), this._invites[i].inviteLabel);
				}
				GUI.skin.label.fontSize = fontSize2;
			}
		}
		foreach (Invite invite2 in list)
		{
			this._invites.Remove(invite2);
		}
		GUI.skin = this.oldSkin;
		GUI.EndGroup();
	}

	// Token: 0x06000FAB RID: 4011 RVA: 0x00068ED0 File Offset: 0x000670D0
	public void PopupOk(string popupType)
	{
		if (popupType != null)
		{
			if (InviteManager.<>f__switch$map19 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
				dictionary.Add("joingame", 0);
				dictionary.Add("challenge_choosedeck", 1);
				dictionary.Add("challenge", 2);
				dictionary.Add("trade", 3);
				dictionary.Add("notification", 4);
				InviteManager.<>f__switch$map19 = dictionary;
			}
			int num;
			if (InviteManager.<>f__switch$map19.TryGetValue(popupType, ref num))
			{
				switch (num)
				{
				case 0:
					App.Popups.ShowInfo("Waiting for opponent", "Waiting for opponent to accept match...");
					App.Communicator.send(new GameMatchAcceptMessage());
					break;
				case 1:
					this.acceptChallenge(this.challengeUserId, true);
					break;
				case 2:
					this.acceptChallenge(this.challengeUserId, false);
					break;
				case 3:
					this.acceptTradeInvite();
					break;
				case 4:
					this._invites.Remove(this.lastClickedInvite);
					break;
				}
			}
		}
	}

	// Token: 0x06000FAC RID: 4012 RVA: 0x00068FD8 File Offset: 0x000671D8
	public void PopupCancel(string popupType)
	{
		if (popupType != null)
		{
			if (InviteManager.<>f__switch$map1A == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
				dictionary.Add("joingame", 0);
				dictionary.Add("challenge", 1);
				dictionary.Add("trade", 2);
				InviteManager.<>f__switch$map1A = dictionary;
			}
			int num;
			if (InviteManager.<>f__switch$map1A.TryGetValue(popupType, ref num))
			{
				switch (num)
				{
				case 0:
					App.Communicator.send(new GameMatchDeclineMessage());
					App.InviteManager.clearActiveInvite();
					break;
				case 1:
					this.declineChallenge();
					break;
				case 2:
					this.declineTradeInvite();
					break;
				}
			}
		}
	}

	// Token: 0x06000FAD RID: 4013 RVA: 0x0000C7F8 File Offset: 0x0000A9F8
	public void setDeckList(List<DeckInfo> decks)
	{
		this.decks = decks;
	}

	// Token: 0x06000FAE RID: 4014 RVA: 0x00069088 File Offset: 0x00067288
	public void declineChallenge()
	{
		for (int i = 0; i < this._invites.Count; i++)
		{
			if (this._invites[i].inviteActive)
			{
				App.Communicator.send(new GameChallengeDeclineMessage(this._invites[i].inviterInfo.from.id));
				Object.Destroy(this._invites[i].gameObject);
				this._invites.RemoveAt(i);
				i = this._invites.Count;
			}
		}
	}

	// Token: 0x06000FAF RID: 4015 RVA: 0x0000C801 File Offset: 0x0000AA01
	public void acceptChallenge(int challengerId, bool chooseDeck)
	{
		Log.warning("in acceptChallenge() " + this.decks.Count);
		App.GameActionManager.AcceptChallenge(challengerId, chooseDeck);
	}

	// Token: 0x06000FB0 RID: 4016 RVA: 0x00069120 File Offset: 0x00067320
	public void acceptTradeInvite()
	{
		SceneLoader.loadScene("_Lobby");
		for (int i = 0; i < this._invites.Count; i++)
		{
			if (this._invites[i].inviteActive)
			{
				App.Communicator.send(new TradeAcceptMessage(this._invites[i].inviterInfo.from.id));
				Object.Destroy(this._invites[i].gameObject);
				this._invites.RemoveAt(i);
				i = this._invites.Count;
			}
		}
	}

	// Token: 0x06000FB1 RID: 4017 RVA: 0x000691C4 File Offset: 0x000673C4
	public void declineTradeInvite()
	{
		for (int i = 0; i < this._invites.Count; i++)
		{
			if (this._invites[i].inviteActive)
			{
				App.Communicator.send(new TradeDeclineMessage(this._invites[i].inviterInfo.from.id));
				Object.Destroy(this._invites[i].gameObject);
				this._invites.RemoveAt(i);
				i = this._invites.Count;
			}
		}
	}

	// Token: 0x06000FB2 RID: 4018 RVA: 0x0006925C File Offset: 0x0006745C
	public void declineAllNonActiveTradeInvites()
	{
		for (int i = 0; i < this._invites.Count; i++)
		{
			if (this._invites[i].message is TradeInviteForwardMessage && !this._invites[i].inviteActive)
			{
				App.Communicator.send(new TradeDeclineMessage(this._invites[i].inviterInfo.from.id));
				Object.Destroy(this._invites[i].gameObject);
				this._invites.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x06000FB3 RID: 4019 RVA: 0x00069308 File Offset: 0x00067508
	public void clearInviteList()
	{
		for (int i = 0; i < this._invites.Count; i++)
		{
			Object.Destroy(this._invites[i].gameObject);
			this._invites.RemoveAt(i);
			i--;
		}
		this._invites = new List<Invite>();
	}

	// Token: 0x06000FB4 RID: 4020 RVA: 0x00069364 File Offset: 0x00067564
	public void clearInviteListTyped(Invite.InviteType type)
	{
		for (int i = 0; i < this._invites.Count; i++)
		{
			if (this._invites[i].inviteType == type)
			{
				Object.Destroy(this._invites[i].gameObject);
				this._invites.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x06000FB5 RID: 4021 RVA: 0x000693CC File Offset: 0x000675CC
	public void declineAllNonActiveInvites()
	{
		for (int i = 0; i < this._invites.Count; i++)
		{
			if (!this._invites[i].inviteActive)
			{
				if (this._invites[i].message is GameMatchMessage)
				{
					App.Communicator.send(new GameMatchDeclineMessage());
					Object.Destroy(this._invites[i].gameObject);
					this._invites.RemoveAt(i);
					i--;
				}
				else if (this._invites[i].message is GameChallengeMessage)
				{
					App.Communicator.send(new GameChallengeDeclineMessage(this._invites[i].inviterInfo.from.id));
					Object.Destroy(this._invites[i].gameObject);
					this._invites.RemoveAt(i);
					i--;
				}
				else if (this._invites[i].message is TradeInviteForwardMessage)
				{
					App.Communicator.send(new TradeDeclineMessage(this._invites[i].inviterInfo.from.id));
					Object.Destroy(this._invites[i].gameObject);
					this._invites.RemoveAt(i);
					i--;
				}
			}
		}
	}

	// Token: 0x06000FB6 RID: 4022 RVA: 0x00069540 File Offset: 0x00067740
	public override void handleMessage(Message msg)
	{
		if (msg is GameChallengeMessage)
		{
			this.addInvite(msg);
		}
		if (msg is GameChallengeResponseMessage)
		{
			GameChallengeResponseMessage gameChallengeResponseMessage = (GameChallengeResponseMessage)msg;
			if (gameChallengeResponseMessage.status == GameChallengeResponseMessage.Status.CANCEL)
			{
				this.removeChallengeInviteFromUser(((GameChallengeResponseMessage)msg).from.id);
			}
			else if (gameChallengeResponseMessage.status == GameChallengeResponseMessage.Status.DECLINE)
			{
				this.addInvite(msg);
			}
		}
		if (msg is GameMatchMessage)
		{
			Log.warning("App.InviteManager: " + App.InviteManager);
			this.addInvite(msg);
		}
		if (msg is GameMatchCancelledMessage)
		{
			GameMatchCancelledMessage gameMatchCancelledMessage = (GameMatchCancelledMessage)msg;
			this.removeRankedOrQuickMatchInvite();
			App.Popups.ShowOk(this, "returntoqueue", "Cancelled", gameMatchCancelledMessage.info, "Ok");
		}
		if (msg is TradeInviteForwardMessage)
		{
			this.addInvite(msg);
		}
		if (msg is AchievementUnlockedMessage)
		{
			this.addInvite(msg);
		}
		if (msg is TradeResponseMessage)
		{
			TradeResponseMessage tradeResponseMessage = (TradeResponseMessage)msg;
			if (tradeResponseMessage.status == "ACCEPT")
			{
				App.SceneValues.loadTradeSystem = true;
				App.SceneValues.tradeResponseMessage = tradeResponseMessage;
				SceneLoader.loadScene("_Lobby");
			}
			else if (tradeResponseMessage.status == "DECLINE")
			{
				this.addInvite(msg);
			}
			else if (!(tradeResponseMessage.status == "TIMEOUT"))
			{
				if (tradeResponseMessage.status == "CANCEL_BARGAIN")
				{
					App.SceneValues.loadTradeSystem = false;
					App.SceneValues.tradeResponseMessage = null;
					Debug.LogError("TRADERESPONSE:CANCEL");
				}
				else if (tradeResponseMessage.status == "CANCEL")
				{
					this.removeTradeInviteFromUser(tradeResponseMessage.from.id);
				}
			}
		}
		if (msg is StringNotificationMessage)
		{
			this.addInvite(msg);
		}
	}

	// Token: 0x04000C28 RID: 3112
	private List<DeckInfo> decks;

	// Token: 0x04000C29 RID: 3113
	private List<Invite> _invites;

	// Token: 0x04000C2A RID: 3114
	private List<InviteManager.WaitingInvite> _waitingInvites = new List<InviteManager.WaitingInvite>();

	// Token: 0x04000C2B RID: 3115
	private bool showAll;

	// Token: 0x04000C2C RID: 3116
	private Rect inviteSize;

	// Token: 0x04000C2D RID: 3117
	private GUISkin inviteSkin;

	// Token: 0x04000C2E RID: 3118
	private GUISkin inviteCloseSkin;

	// Token: 0x04000C2F RID: 3119
	private GUISkin chatSkin;

	// Token: 0x04000C30 RID: 3120
	private GUIStyle emptyStyle;

	// Token: 0x04000C31 RID: 3121
	private Invite lastClickedInvite;

	// Token: 0x04000C32 RID: 3122
	private float popInvitesTime;

	// Token: 0x04000C33 RID: 3123
	private GUISkin oldSkin;

	// Token: 0x04000C34 RID: 3124
	private int challengeUserId;

	// Token: 0x04000C35 RID: 3125
	private int tradeUserId;

	// Token: 0x020001F4 RID: 500
	public class WaitingInvite
	{
		// Token: 0x04000C38 RID: 3128
		public string label;

		// Token: 0x04000C39 RID: 3129
		public string fullText;

		// Token: 0x04000C3A RID: 3130
		public Invite.InviteType inviteType;

		// Token: 0x04000C3B RID: 3131
		public Message message;
	}
}
