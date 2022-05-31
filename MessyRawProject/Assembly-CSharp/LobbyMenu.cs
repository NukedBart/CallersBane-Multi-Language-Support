using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gui;
using UnityEngine;

// Token: 0x0200020A RID: 522
public class LobbyMenu : MonoBehaviour, IOkCallback, ICancelCallback, IOkCancelCallback, IOkStringCallback, IOkStringCancelCallback
{
	// Token: 0x0600107D RID: 4221 RVA: 0x0006F44C File Offset: 0x0006D64C
	private void Start()
	{
		this.audioScript = App.AudioScript;
		this.guiSkinClear = ScriptableObject.CreateInstance<GUISkin>();
		this.guiSkin = (GUISkin)ResourceManager.Load("_GUISkins/LobbyMenu");
		this.guiSkinDisabled = (GUISkin)ResourceManager.Load("_GUISkins/DisabledButtons");
		this.chatButtonSkin = (GUISkin)ResourceManager.Load("_GUISkins/ChatButton");
		this.regularUI = (GUISkin)ResourceManager.Load("_GUISkins/RegularUI");
		this.demoStyle = new GUIStyle(this.guiSkin.label);
		this.demoStyle.alignment = 1;
		this.demoStyle.normal.textColor = new Color(0.6f, 0.53f, 0.3f, 1f);
		this.buttons.Add(new LobbyMenu.Button(0, "_HomeScreen", LobbyMenu.createButtonSkin("ChatUI/arrow_")).noHelp());
		this.buttons.Add(new LobbyMenu.Button(1, "_Lobby", LobbyMenu.createButtonSkin("LobbyMenu/arena_")));
		this.buttons.Add(new LobbyMenu.Button(2, "_DeckBuilderView", LobbyMenu.createButtonSkin("LobbyMenu/deckbuilder_")));
		this.buttons.Add(new LobbyMenu.Button(3, "_CraftingView", LobbyMenu.createButtonSkin("LobbyMenu/crafting_")));
		this.buttons.Add(new LobbyMenu.Button(4, "_Store", LobbyMenu.createButtonSkin("LobbyMenu/store_")));
		this.buttons.Add(new LobbyMenu.Button(5, "_Settings", LobbyMenu.createButtonSkin("LobbyMenu/settings_")).noHelp());
		this.buttons.Add(new LobbyMenu.Button(6, "_Profile", LobbyMenu.createButtonSkin("LobbyMenu/profile_")));
		this.AdjustForResolution();
		this.comm = App.Communicator;
		base.enabled = false;
	}

	// Token: 0x0600107E RID: 4222 RVA: 0x0000CCEB File Offset: 0x0000AEEB
	private static GUISkin createButtonSkin(string basefn)
	{
		return LobbyMenu.createButtonSkin(basefn, basefn + "mo", basefn + "md");
	}

	// Token: 0x0600107F RID: 4223 RVA: 0x0006F610 File Offset: 0x0006D810
	private static GUISkin createButtonSkin(string basefn, string mouseoverfn, string mousedownfn)
	{
		GUISkin guiskin = ScriptableObject.CreateInstance<GUISkin>();
		guiskin.button.normal.background = ResourceManager.LoadTexture(basefn);
		guiskin.button.hover.background = ResourceManager.LoadTexture(mouseoverfn);
		guiskin.button.active.background = ResourceManager.LoadTexture(mousedownfn);
		return guiskin;
	}

	// Token: 0x06001080 RID: 4224 RVA: 0x0006F668 File Offset: 0x0006D868
	public void AdjustForResolution()
	{
		List<Vector2> list = new List<Vector2>();
		foreach (LobbyMenu.Button button in this.buttons)
		{
			GUISkin skin = button.skin;
			Vector2 vector;
			vector..ctor(40f, 20f);
			if (skin.button.normal.background != null)
			{
				vector..ctor((float)skin.button.normal.background.width, (float)skin.button.normal.background.height);
			}
			list.Add(vector);
		}
		MockupCalc c = new MockupCalc(2048, 1536);
		float spacingBetween = 100f;
		if (AspectRatio.now.isNarrower(AspectRatio._5_4))
		{
			spacingBetween = 50f;
		}
		else if (AspectRatio.now.isNarrower(AspectRatio._4_3))
		{
			spacingBetween = 56f;
		}
		else if (AspectRatio.now.isNarrower(AspectRatio._3_2))
		{
			spacingBetween = 70f;
		}
		else if (AspectRatio.now.isNarrower(AspectRatio._16_10))
		{
			spacingBetween = 95f;
		}
		this._headerPositioner = LobbyMenu.getButtonPositioner(c, list.ToArray(), spacingBetween);
	}

	// Token: 0x06001081 RID: 4225 RVA: 0x0006F7D8 File Offset: 0x0006D9D8
	public void ShowFriendList()
	{
		if (!App.ServerSettings.friendsListEnabled)
		{
			return;
		}
		this.showFriendsList = true;
		this.friendsListBorder = new ScrollsFrame().AddNinePatch(ScrollsFrame.Border.LIGHT_SHARP, NinePatch.Patches.CENTER);
		this.friendsListScroll = new Vector2((float)Screen.height * 0.35f - 25f, 0f);
	}

	// Token: 0x06001082 RID: 4226 RVA: 0x0000CD09 File Offset: 0x0000AF09
	public void SetEnabled(bool enabled)
	{
		base.enabled = enabled;
		this.canOpenContextMenu = enabled;
	}

	// Token: 0x06001083 RID: 4227 RVA: 0x0000CD19 File Offset: 0x0000AF19
	public void SetButtonsEnabled(bool enabled)
	{
		this.buttonsEnabled = enabled;
		this.canOpenContextMenu = enabled;
	}

	// Token: 0x06001084 RID: 4228 RVA: 0x0006F834 File Offset: 0x0006DA34
	private void OnGUI()
	{
		if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && this.userContextMenu != null && !this.userContextMenu.containsMouse())
		{
			this.CloseUserMenu();
		}
		GUI.depth = 13;
		this.menuGUI(App.MyProfile.ProfileData.gold, App.MyProfile.ProfileData.rating, App.MyProfile.ProfileData.shards);
		if (this.showFriendsList)
		{
			App.GUI.Blocker(this.friendsListBorder.GetRect());
		}
	}

	// Token: 0x06001085 RID: 4229 RVA: 0x0006F8D4 File Offset: 0x0006DAD4
	private void Update()
	{
		if (Input.GetKeyDown(27))
		{
			if (App.Popups.IsShowingPopup())
			{
				App.Popups.RequestPopupClose();
			}
			else
			{
				App.Popups.ShowOkCancel(this, "exit", I18n.Text("Quitting {GAME_NAME}"), I18n.Text("Are you sure you want to quit {GAME_NAME}?"), "Quit", "Cancel");
			}
		}
	}

	// Token: 0x06001086 RID: 4230 RVA: 0x0006F93C File Offset: 0x0006DB3C
	public void PopupOk(string popupType)
	{
		if (popupType != null)
		{
			if (LobbyMenu.<>f__switch$map1B == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
				dictionary.Add("exit", 0);
				dictionary.Add("leavequeue", 1);
				dictionary.Add("removefriend", 2);
				LobbyMenu.<>f__switch$map1B = dictionary;
			}
			int num;
			if (LobbyMenu.<>f__switch$map1B.TryGetValue(popupType, ref num))
			{
				switch (num)
				{
				case 0:
					Application.Quit();
					break;
				case 1:
					App.Communicator.send(new ExitMultiPlayerQueueMessage());
					break;
				case 2:
					if (App.FriendList.Friends.Count == 1)
					{
						this.editingFriendsList = false;
					}
					App.FriendList.RemoveFriend(this.friendToRemove);
					this.friendToRemove = null;
					break;
				}
			}
		}
	}

	// Token: 0x06001087 RID: 4231 RVA: 0x0006FA0C File Offset: 0x0006DC0C
	public void PopupOk(string popupType, string value)
	{
		if (popupType != null)
		{
			if (LobbyMenu.<>f__switch$map1C == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
				dictionary.Add("addfriend", 0);
				LobbyMenu.<>f__switch$map1C = dictionary;
			}
			int num;
			if (LobbyMenu.<>f__switch$map1C.TryGetValue(popupType, ref num))
			{
				if (num == 0)
				{
					App.FriendList.AddFriend(value);
				}
			}
		}
	}

	// Token: 0x06001088 RID: 4232 RVA: 0x000028DF File Offset: 0x00000ADF
	public void PopupCancel(string popupType)
	{
	}

	// Token: 0x06001089 RID: 4233 RVA: 0x0006FA74 File Offset: 0x0006DC74
	public static Rect getMenuRect()
	{
		float num = 0.0875f * (float)Screen.height;
		return new Rect(0f, num / 3f, (float)Screen.width, num);
	}

	// Token: 0x0600108A RID: 4234 RVA: 0x0006FAA8 File Offset: 0x0006DCA8
	public void menuGUI(int noOfGolds, int playerRating, int noOfShards)
	{
		if (this.overlayAlpha > 0f)
		{
			GUI.color = new Color(1f, 1f, 1f, this.overlayAlpha);
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), ResourceManager.LoadTexture("Shared/blackFiller"));
			GUI.color = new Color(1f, 1f, 1f, 1f);
		}
		if (this.userContextMenu != null)
		{
			this.userContextMenu.OnGUI_First();
		}
		Rect menuRect = LobbyMenu.getMenuRect();
		float y = menuRect.y;
		float num = y - (float)Screen.height * 0.0875f / 3f;
		GUI.BeginGroup(new Rect(0f, num, (float)Screen.width, (float)Screen.height));
		float num2 = y - num;
		GUI.DrawTexture(new Rect(menuRect.x - (float)Screen.width * 0.01f, num2, menuRect.width + (float)Screen.width * 0.02f, menuRect.height), ResourceManager.LoadTexture("ChatUI/menu_bar"));
		this.menuButtons();
		GUISkin skin = GUI.skin;
		float num3 = 0.0875f * (float)Screen.height;
		GUI.skin = this.chatButtonSkin;
		List<Person> friends = App.FriendList.Friends;
		List<Request> requests = App.FriendList.Requests;
		GUI.skin.button.alignment = 3;
		int fontSize = GUI.skin.button.fontSize;
		GUI.skin.button.fontSize = Screen.height / 28;
		Rect rect;
		rect..ctor((float)Screen.width - (float)Screen.height * 0.04f - (float)Screen.height * 0.065f - (float)Screen.height * 0.022f, num3 * 0.575f, (float)Screen.height * 0.06f, (float)Screen.height * 0.048f);
		if (App.ServerSettings.friendsListEnabled && this.GUIButton(rect, " " + App.FriendList.OnlineFriendCount()))
		{
			this.showFriendsList = !this.showFriendsList;
			if (!this.showFriendsList)
			{
				this.CloseUserMenu();
			}
			this.friendsListBorder = new ScrollsFrame().AddNinePatch(ScrollsFrame.Border.LIGHT_SHARP, NinePatch.Patches.CENTER);
			this.friendsListScroll = new Vector2((float)Screen.height * 0.35f - 25f, 0f);
		}
		if (App.FriendList.IncomingRequestCount() > 0)
		{
			GUI.color = new Color(1f, 0.8f, 0.4f, 0.1f + Mathf.Sin(Time.time * 4f) / 12f);
			GUI.DrawTexture(rect, ResourceManager.LoadTexture("ChatUI/white"));
			GUI.color = Color.white;
		}
		GUI.skin.button.fontSize = fontSize;
		GUI.skin.button.alignment = 4;
		if (App.ServerSettings.friendsListEnabled)
		{
			GUI.DrawTexture(new Rect((float)Screen.width - (float)Screen.height * 0.04f - (float)Screen.height * 0.065f + (float)Screen.height * 0.005f, num3 * 0.505f + (float)Screen.height * 0.012f, (float)Screen.height * 0.04f, (float)Screen.height * 0.04f * 0.863f), ResourceManager.LoadTexture("LobbyMenu/friends"));
		}
		GUI.skin = skin;
		if (this.showFriendsList)
		{
			float num4 = (float)Screen.height * 0.35f;
			float num5 = (float)Screen.height * 0.3f;
			Rect rect2;
			rect2..ctor((float)Screen.width - (float)Screen.height * 0.3f, menuRect.y + menuRect.height, num5, num4);
			this.friendsListBorder.SetRect(rect2);
			this.friendsListBorder.Draw();
			GUI.BeginGroup(rect2);
			GUISkin skin2 = GUI.skin;
			GUI.skin = (GUISkin)ResourceManager.Load("_GUISkins/ChatUI");
			TextAnchor alignment = GUI.skin.button.alignment;
			GUI.skin.button.alignment = 3;
			TextAnchor alignment2 = GUI.skin.label.alignment;
			GUI.skin.label.alignment = 6;
			int fontSize2 = GUI.skin.button.fontSize;
			int fontSize3 = GUI.skin.label.fontSize;
			GUI.skin.button.fontSize = Screen.height / 40;
			GUI.skin.label.fontSize = Screen.height / 34;
			float num6 = (float)Screen.height * 0.04f;
			GUI.Box(new Rect(num5 - 20f - 5f, 10f, 15f, num4 - (float)Screen.height * 0.08f), string.Empty);
			this.friendsListScroll = GUI.BeginScrollView(new Rect(10f, 10f, num5 - 20f, num4 - (float)Screen.height * 0.08f - 5f), this.friendsListScroll, new Rect(0f, 0f, num5 - 60f, (float)(friends.Count + requests.Count + 1) * num6), false, false);
			int num7 = 0;
			if (requests.Count > 0)
			{
				GUI.Label(new Rect((float)Screen.height * 0.01f, num6 * (float)num7, num5 - 40f - num6 * 2f, num6), "Friend requests");
				num7++;
			}
			for (int i = 0; i < requests.Count; i++)
			{
				GUI.color = new Color(1f, 1f, 1f, 0.4f);
				GUI.Box(new Rect(5f, num6 * (float)num7, num5 - 50f, num6), string.Empty);
				GUI.color = Color.white;
				GUI.skin.button.normal.textColor = Color.white;
				if (requests[i].from.profile.id == App.MyProfile.ProfileInfo.id)
				{
					if (GUI.Button(new Rect(15f, num6 * (float)num7, num5 - 50f - num6 * 2f, num6), "[Pending] " + requests[i].to.profile.name))
					{
					}
				}
				else
				{
					if (GUI.Button(new Rect(15f, num6 * (float)num7, num5 - 50f - num6 * 2f, num6), string.Empty + requests[i].from.profile.name))
					{
					}
					GUISkin skin3 = GUI.skin;
					GUI.skin = this.regularUI;
					if (this.GUIButton(new Rect(5f + num5 - 50f - num6 * 2f + 7f, num6 * (float)num7 + 3f, num6 - 6f, num6 - 6f), ResourceManager.LoadTexture("LobbyMenu/tickmark")))
					{
						App.FriendList.AcceptRequest(requests[i]);
					}
					if (this.GUIButton(new Rect(5f + num5 - 50f - num6 + 3f, num6 * (float)num7 + 3f, num6 - 6f, num6 - 6f), ResourceManager.LoadTexture("LobbyMenu/crossmark")))
					{
						App.FriendList.DeclineRequest(requests[i]);
					}
					GUI.skin = skin3;
				}
				num7++;
			}
			if (friends.Count > 0)
			{
				GUI.Label(new Rect((float)Screen.height * 0.01f, num6 * (float)num7, num5 - 50f - num6 * 2f, num6), "Friends");
				num7++;
			}
			for (int j = 0; j < friends.Count; j++)
			{
				GUI.color = new Color(1f, 1f, 1f, 0.4f);
				GUI.Box(new Rect(5f, num6 * (float)num7, num5 - 50f, num6), string.Empty);
				GUI.color = Color.white;
				if (!friends[j].online())
				{
					GUI.skin.button.normal.textColor = Color.grey;
					if (GUI.Button(new Rect(15f, num6 * (float)num7, num5 - 20f - num6 - 40f, num6), "[Offline] " + friends[j].profile.name))
					{
						this.CreateUserMenu(friends[j], false);
					}
				}
				else
				{
					GUI.skin.button.normal.textColor = Color.white;
					float num8 = (!this.editingFriendsList) ? 5f : (num6 + 5f);
					if (this.GUIButton(new Rect(15f + (float)Screen.height * 0.03f, num6 * (float)num7, num5 - 20f - (float)Screen.height * 0.03f - num8 - 40f, num6), friends[j].prefixString() + friends[j].profile.name))
					{
						this.CreateUserMenu(friends[j], true);
					}
					Texture2D texture2D = ResourceManager.LoadTexture("Logos/scrolls_icon_32");
					GUI.DrawTexture(new Rect(10f, num6 * (float)num7 + (float)Screen.height * 0.003f, (float)Screen.height * 0.03f, (float)Screen.height * 0.03f), texture2D);
				}
				GUISkin skin4 = GUI.skin;
				GUI.skin = this.regularUI;
				if (this.editingFriendsList && this.GUIButton(new Rect(5f + num5 - 50f - num6 + 3f, num6 * (float)num7 + 3f, num6 - 6f, num6 - 6f), ResourceManager.LoadTexture("LobbyMenu/crossmark")))
				{
					App.Popups.ShowOkCancel(this, "removefriend", "Remove friend", "Are you sure you want to remove " + friends[j].profile.name + " from your friends list?", "Remove", "Cancel");
					this.friendToRemove = friends[j];
				}
				GUI.skin = skin4;
				num7++;
			}
			GUI.EndScrollView();
			GUI.skin.button.normal.textColor = Color.white;
			GUI.skin.button.alignment = alignment;
			GUI.skin = (GUISkin)ResourceManager.Load("_GUISkins/FriendsList");
			float num9 = num5 * 0.4f;
			float num10 = num5 * 0.08f;
			if (!this.editingFriendsList)
			{
				if (this.GUIButton(new Rect(num10, num4 - (float)Screen.height * 0.06f, num9, (float)Screen.height * 0.04f), "Add Friend"))
				{
					App.Popups.ShowTextEntry(this, "addfriend", "Add friend", "Enter friend name", "Add", "Cancel");
				}
				if (this.GUIButton(new Rect(num5 - num9 - num10, num4 - (float)Screen.height * 0.06f, num9, (float)Screen.height * 0.04f), "Edit Friends"))
				{
					this.editingFriendsList = true;
				}
			}
			else if (this.GUIButton(new Rect(num5 - num9 - num10, num4 - (float)Screen.height * 0.06f, num9, (float)Screen.height * 0.04f), "Back"))
			{
				this.editingFriendsList = false;
			}
			GUI.EndGroup();
			GUI.skin.label.fontSize = fontSize3;
			GUI.skin.button.fontSize = fontSize2;
			GUI.skin.label.alignment = alignment2;
			GUI.skin = skin2;
		}
		GUI.skin = this.guiSkin;
		float num11 = (float)Screen.height * 0.35f;
		float num12 = (float)Screen.width - num11 - (float)Screen.width * 0.1f;
		if (num12 < this.buttonMaxX)
		{
			num12 = this.buttonMaxX;
		}
		float num13 = (float)Screen.width * 0.022f + num12;
		float num14 = num13 + num11 * 0.35f;
		float num15 = num14 + num11 * 0.25f;
		float num16 = (float)Screen.height * 0.017f;
		float num17 = num11 / 3.142f;
		if (AspectRatio.now.isNarrower(AspectRatio._3_2))
		{
			num16 = (float)Screen.height * 0.026f;
			num17 = num11 / 3.742f;
		}
		GUI.DrawTexture(new Rect(num12, num16, num11, num17), ResourceManager.LoadTexture("ChatUI/profile"));
		GUI.skin.label.alignment = 0;
		GUI.skin.label.fontSize = Screen.height / 28;
		LobbyMenu.drawShadowText(new Rect(num13, (float)Screen.height * 0.04f, 250f, (float)Screen.height * 0.08f), App.MyProfile.ProfileInfo.name, Color.white);
		GUI.skin.label.fontSize = Screen.height / 36;
		LobbyMenu.drawShadowText(new Rect(num13, (float)Screen.height * 0.068f, 250f, (float)Screen.height * 0.04f), "Rating " + playerRating, Color.yellow);
		GUI.DrawTexture(new Rect(num14, (float)Screen.height * 0.068f + (float)Screen.height * 0.005f, (float)Screen.height * 0.02f, (float)Screen.height * 0.02f), ResourceManager.LoadTexture("Shared/gold_icon_small"));
		LobbyMenu.drawShadowText(new Rect(num14 + (float)Screen.height * 0.02f, (float)Screen.height * 0.068f, 250f, (float)Screen.height * 0.04f), " " + noOfGolds, Color.yellow);
		if (App.ServerSettings.shardsEnabled)
		{
			GUI.DrawTexture(new Rect(num15, (float)Screen.height * 0.068f + (float)Screen.height * 0.005f, (float)Screen.height * 0.02f, (float)Screen.height * 0.02f), ResourceManager.LoadTexture("Shared/voidshard_icon_small"));
			LobbyMenu.drawShadowText(new Rect(num15 + (float)Screen.height * 0.02f, (float)Screen.height * 0.068f, 250f, (float)Screen.height * 0.04f), " " + noOfShards, Color.yellow);
		}
		GUI.skin.label.normal.textColor = new Color(0.73f, 0.664f, 0.273f);
		TextAnchor alignment3 = GUI.skin.label.alignment;
		GUI.skin.label.alignment = 5;
		GUI.Label(new Rect((float)(Screen.width - 220), (float)Screen.height * 0.003f, 200f, (float)Screen.height * 0.03f), string.Empty + DateTime.Now.ToShortTimeString());
		GUI.skin.label.alignment = alignment3;
		if (this.isQueued())
		{
			GUI.skin = this.guiSkinClear;
			GUI.skin.button.font = (Font)ResourceManager.Load("Fonts/HoneyMeadBB_bold", typeof(Font));
			GUI.skin.button.alignment = 3;
			GUI.skin.button.normal.textColor = new Color(1f, 0.98f, 0.86f, 1f);
			GUI.skin.button.fontSize = Screen.height / 40;
			string text = string.Empty;
			int num18 = 0;
			while ((float)num18 < Time.time % 3f)
			{
				text += ".";
				num18++;
			}
			string text2 = "You are queued for a multiplayer game. Click to leave queue.";
			if (GUI.Button(new Rect((float)Screen.width * 0.01f, (float)Screen.height * 0.002f, 250f, 20f), new GUIContent("Looking for a " + this.queueString + " match" + text, text2)))
			{
				App.Popups.ShowOkCancel(this, "leavequeue", "Leaving queue", "Are you sure you want to leave the queue?", "Leave", "Stay");
				App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
			}
		}
		if (GUI.tooltip != string.Empty)
		{
			GUI.skin = (GUISkin)ResourceManager.Load("_GUISkins/Tooltip");
			GUI.skin.label.fontSize = Screen.height / 40;
			GUISkin skin5 = GUI.skin;
			float num19 = skin5.GetStyle("label").CalcHeight(new GUIContent(GUI.tooltip), 200f);
			GUI.Label(new Rect(Input.mousePosition.x + 20f, (float)Screen.height - Input.mousePosition.y, 200f, num19), GUI.tooltip);
		}
		GUI.skin = this.chatButtonSkin;
		if (this.userContextMenu != null)
		{
			this.userContextMenu.OnGUI_Last();
		}
		GUI.EndGroup();
	}

	// Token: 0x0600108B RID: 4235 RVA: 0x0000B150 File Offset: 0x00009350
	private bool GUIButton(Rect r, string text)
	{
		if (GUI.Button(r, text))
		{
			App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
			return true;
		}
		return false;
	}

	// Token: 0x0600108C RID: 4236 RVA: 0x0000CD29 File Offset: 0x0000AF29
	private bool GUIButton(Rect r, Texture2D texture)
	{
		if (GUI.Button(r, texture))
		{
			App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
			return true;
		}
		return false;
	}

	// Token: 0x0600108D RID: 4237 RVA: 0x00070C78 File Offset: 0x0006EE78
	public static void drawShadowText(Rect rect, string text, Color color)
	{
		Rect rect2;
		rect2..ctor(rect);
		Color textColor = GUI.skin.label.normal.textColor;
		GUI.skin.label.normal.textColor = Color.black;
		GUI.Label(rect2, text);
		GUI.skin.label.normal.textColor = color;
		rect2.x -= 2f;
		rect2.y -= 2f;
		GUI.Label(rect2, text);
		GUI.skin.label.normal.textColor = textColor;
	}

	// Token: 0x0600108E RID: 4238 RVA: 0x00070D1C File Offset: 0x0006EF1C
	public Rect getSubMenuRect(float alpha)
	{
		Rect menuRect = LobbyMenu.getMenuRect();
		float num = (float)(Screen.height * 120 / 1536);
		float num2 = menuRect.y + menuRect.height * 0.7f;
		float num3 = num2 - num;
		return new Rect(0f, num3 + (num2 - num3) * alpha, (float)Screen.width, num);
	}

	// Token: 0x0600108F RID: 4239 RVA: 0x0000CD49 File Offset: 0x0000AF49
	public GUIPositioner getSubMenuPositioner(float alpha, int numButtons)
	{
		return this.getSubMenuPositioner(alpha, numButtons, 200f);
	}

	// Token: 0x06001090 RID: 4240 RVA: 0x00070D74 File Offset: 0x0006EF74
	public GUIPositioner getSubMenuPositioner(float alpha, int numButtons, float bWidth)
	{
		GUIPositioner buttonPositioner = LobbyMenu.getButtonPositioner(1f, 2f, bWidth);
		Rect subMenuRect = this.getSubMenuRect(alpha);
		float num = subMenuRect.y + subMenuRect.height * 0.35f;
		buttonPositioner.setOffset(new Vector2(this._headerPositioner.getButtonRect(1f, 0f).x, num));
		return buttonPositioner;
	}

	// Token: 0x06001091 RID: 4241 RVA: 0x0000CD58 File Offset: 0x0000AF58
	public static bool drawButton(Rect rect, string title)
	{
		return LobbyMenu.drawButton(rect, new GUIContent(title));
	}

	// Token: 0x06001092 RID: 4242 RVA: 0x0000CD66 File Offset: 0x0000AF66
	public static bool drawButton(Rect rect, GUIContent content)
	{
		return LobbyMenu.drawButton(rect, content, GUI.skin.button, GUI.skin.label);
	}

	// Token: 0x06001093 RID: 4243 RVA: 0x0000CD83 File Offset: 0x0000AF83
	public static bool drawButton(Rect rect, string title, GUISkin skin)
	{
		return LobbyMenu.drawButton(rect, new GUIContent(title), skin.button, skin.label);
	}

	// Token: 0x06001094 RID: 4244 RVA: 0x0000CD9D File Offset: 0x0000AF9D
	public static bool drawButton(Rect rect, GUIContent content, GUISkin skin)
	{
		return LobbyMenu.drawButton(rect, content, skin.button, skin.label);
	}

	// Token: 0x06001095 RID: 4245 RVA: 0x0000CDB2 File Offset: 0x0000AFB2
	public static bool drawButton(Rect rect, string title, GUIStyle buttonStyle, GUIStyle labelStyle)
	{
		return LobbyMenu.drawButton(rect, new GUIContent(title), buttonStyle, labelStyle);
	}

	// Token: 0x06001096 RID: 4246 RVA: 0x00070DDC File Offset: 0x0006EFDC
	public static bool drawButton(Rect rect, GUIContent content, GUIStyle buttonStyle, GUIStyle labelStyle)
	{
		Rect rect2;
		rect2..ctor(rect);
		bool flag = App.GUI.Button(rect2, content, buttonStyle);
		rect2.x -= 1f;
		rect2.y -= 1f;
		App.GUI.Label(rect2, content, labelStyle);
		if (flag)
		{
			App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
		}
		return flag;
	}

	// Token: 0x06001097 RID: 4247 RVA: 0x00070E48 File Offset: 0x0006F048
	public static GUIPositioner getButtonPositioner(float numButtons, float spacingBetween, float bWidth, float bHeight)
	{
		MockupCalc mockupCalc = new MockupCalc(1920, 1080);
		Rect rect = mockupCalc.rAspectH(0f, 0f, bWidth, bHeight);
		return new GUIPositioner(numButtons, mockupCalc.X(spacingBetween), rect.width, rect.height);
	}

	// Token: 0x06001098 RID: 4248 RVA: 0x0000CDC2 File Offset: 0x0000AFC2
	public static GUIPositioner getButtonPositioner(float numButtons, float spacingBetween, float bWidth)
	{
		return LobbyMenu.getButtonPositioner(numButtons, spacingBetween, bWidth, 40f);
	}

	// Token: 0x06001099 RID: 4249 RVA: 0x0000CDD1 File Offset: 0x0000AFD1
	public static GUIPositioner getButtonPositioner(float numButtons, float spacingBetween)
	{
		return LobbyMenu.getButtonPositioner(numButtons, spacingBetween, 200f);
	}

	// Token: 0x0600109A RID: 4250 RVA: 0x0000CDDF File Offset: 0x0000AFDF
	public static GUIPositioner2 getButtonPositioner(MockupCalc c, Vector2[] sizes, float spacingBetween)
	{
		return new GUIPositioner2(c, sizes, spacingBetween);
	}

	// Token: 0x0600109B RID: 4251 RVA: 0x0000CDE9 File Offset: 0x0000AFE9
	private void fadeOutScene()
	{
		base.StopCoroutine("fadeToTransparent");
		base.StopCoroutine("fadeToBlack");
		base.StartCoroutine("fadeToBlack");
	}

	// Token: 0x0600109C RID: 4252 RVA: 0x0000CE0D File Offset: 0x0000B00D
	public void fadeInScene()
	{
		base.StopCoroutine("fadeToTransparent");
		base.StopCoroutine("fadeToBlack");
		base.StartCoroutine("fadeToTransparent");
	}

	// Token: 0x0600109D RID: 4253 RVA: 0x0000CE31 File Offset: 0x0000B031
	public void loadSceneWithFade(string scene)
	{
		this._sceneToLoad = scene;
		this.fadeOutScene();
	}

	// Token: 0x0600109E RID: 4254 RVA: 0x00070E94 File Offset: 0x0006F094
	private IEnumerator fadeToBlack()
	{
		while (this.overlayAlpha < 1f)
		{
			this.overlayAlpha += Time.fixedDeltaTime * 2.5f;
			yield return new WaitForEndOfFrame();
		}
		SceneLoader.loadScene(this._sceneToLoad);
		this._sceneToLoad = null;
		yield break;
	}

	// Token: 0x0600109F RID: 4255 RVA: 0x00070EB0 File Offset: 0x0006F0B0
	private IEnumerator fadeToTransparent()
	{
		while (this.overlayAlpha > 0f)
		{
			this.overlayAlpha -= Time.fixedDeltaTime * 2.5f;
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	// Token: 0x060010A0 RID: 4256 RVA: 0x00070ECC File Offset: 0x0006F0CC
	private void drawHeaderButtons()
	{
		this.drawCurrentSceneHighlight();
		this._hoverButtonInside = false;
		foreach (LobbyMenu.Button button in this.buttons)
		{
			this.drawHeaderButton(button.index, button.scene);
		}
		this.buttonMaxX = this._headerPositioner.getButtonRect(6f, 0f).x;
		if (!this._hoverButtonInside)
		{
			this._hoverButtonIndex = -1;
		}
	}

	// Token: 0x060010A1 RID: 4257 RVA: 0x00070F74 File Offset: 0x0006F174
	private void drawCurrentSceneHighlight()
	{
		string scene = SceneLoader.getScene();
		foreach (LobbyMenu.Button button in this.buttons)
		{
			if (button.scene == scene)
			{
				this.drawCurrentSceneHighlight(button);
				break;
			}
		}
	}

	// Token: 0x060010A2 RID: 4258 RVA: 0x00070FEC File Offset: 0x0006F1EC
	private void drawCurrentSceneHighlight(LobbyMenu.Button button)
	{
		Rect buttonRect = this._headerPositioner.getButtonRect((float)button.index, (float)Screen.height * 0.023f);
		float num = (float)Screen.height * 0.1f;
		float num2 = 0.7297297f * num;
		Rect rect;
		rect..ctor(buttonRect.x - 0.5f * num2, buttonRect.y, num2, num);
		Rect rect2;
		rect2..ctor(buttonRect.xMax - 0.55f * num2, buttonRect.y, num2, num);
		Rect rect3;
		rect3..ctor(rect.x, rect.y, rect2.xMax - rect.x, rect.height);
		Rect rect4;
		rect4..ctor(rect3);
		rect4.height *= 0.82f;
		bool flag = rect4.Contains(GUIUtil.getScreenMousePos());
		GUI.DrawTexture(rect, ResourceManager.LoadTexture("LobbyMenu/menu_bar_tab_marker_left"));
		GUI.DrawTexture(rect2, (!button.hasHelp) ? ResourceManager.LoadTexture("LobbyMenu/menu_bar_tab_marker_right_no_help") : ((!flag) ? ResourceManager.LoadTexture("LobbyMenu/menu_bar_tab_marker_right") : ResourceManager.LoadTexture("LobbyMenu/menu_bar_tab_marker_right_mo")));
		if (GUI.Button(rect4, string.Empty, this.guiSkinClear.label))
		{
			FirstTimeHelp.showFirstTimeHelpFor(button.scene, true);
		}
	}

	// Token: 0x060010A3 RID: 4259 RVA: 0x0007113C File Offset: 0x0006F33C
	private bool drawHeaderButton(int index, string sceneToLoad)
	{
		float y = (float)Screen.height * ((index != 0) ? 0.0565f : 0.06f);
		LobbyMenu.Button button = this.buttons[index];
		GUI.skin = button.skin;
		Rect buttonRect = this._headerPositioner.getButtonRect((float)index, y);
		bool flag = this.buttonsEnabled && buttonRect.Contains(GUIUtil.getScreenMousePos());
		if (this.buttonsEnabled && flag && !button.isCurrentScene())
		{
			if (this._hoverButtonIndex != index)
			{
				this._hoverButtonStartTime = Time.time;
				this._hoverButtonIndex = index;
			}
			this._hoverButtonInside = true;
			float num = Time.time - this._hoverButtonStartTime;
			float num2 = Mathf.Min(1f, 10f * num) * (0.8f - 0.2f * Mathf.Sin(4f * Time.time));
			float kx = 1f + this.mockupJunk.Y(80f) / buttonRect.width;
			Rect rect = GeomUtil.resizeCentered(buttonRect, (float)Screen.height * 0.24f, buttonRect.height);
			rect.height = LobbyMenu.getMenuRect().height * 0.68f;
			rect.y -= rect.height * 0.26f;
			Rect rect2 = GeomUtil.scaleCentered(buttonRect, kx, 1.5f);
			GUI.DrawTexture(rect, ResourceManager.LoadTexture("ChatUI/menu_select_black"));
			GUI.color = new Color(1f, 1f, 1f, num2);
			GUI.DrawTexture(rect2, ResourceManager.LoadTexture("ChatUI/menu_select_white"));
			GUI.color = Color.white;
		}
		GUI.enabled = this.buttonsEnabled;
		GUIContent content = this.emptyContent;
		if (App.Config.settings.showTutorialArrows() && sceneToLoad == "_Lobby" && !SceneLoader.isScene(new string[]
		{
			"_Lobby"
		}))
		{
			content = this.helpArrowContent;
		}
		bool flag2 = LobbyMenu.drawButton(buttonRect, content) && this.buttonsEnabled;
		GUI.enabled = true;
		if (flag2 && this.isSceneJumpValid(sceneToLoad))
		{
			this._sceneToLoad = sceneToLoad;
			this.audioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
			this.fadeOutScene();
		}
		return flag2;
	}

	// Token: 0x060010A4 RID: 4260 RVA: 0x00071398 File Offset: 0x0006F598
	private bool isSceneJumpValid(string scene)
	{
		if (scene == "_Profile" && SceneLoader.isScene(new string[]
		{
			scene
		}))
		{
			return !App.SceneValues.profilePage.wasMe;
		}
		return !SceneLoader.isScene(new string[]
		{
			scene
		});
	}

	// Token: 0x060010A5 RID: 4261 RVA: 0x000713F0 File Offset: 0x0006F5F0
	public void menuButtons()
	{
		int buttonFontSize = LobbyMenu.getButtonFontSize();
		if (this.buttonsEnabled)
		{
			GUI.skin = this.guiSkin;
		}
		else
		{
			GUI.skin = this.guiSkinDisabled;
		}
		GUI.skin.label.fontSize = buttonFontSize;
		GUI.skin.button.fontSize = buttonFontSize;
		this.drawDemoSticker(0.01f);
		this.drawHeaderButtons();
	}

	// Token: 0x060010A6 RID: 4262 RVA: 0x0007145C File Offset: 0x0006F65C
	private void drawDemoSticker(float yp)
	{
		if (!App.IsDemo())
		{
			return;
		}
		float num = 0.04f * (float)Screen.height;
		Rect rect;
		rect..ctor(0f, -0.005f * (float)Screen.height, (float)Screen.width, num);
		this.demoStyle.fontSize = Screen.height / 30;
		GUI.Label(rect, "SCROLLS DEMO", this.demoStyle);
	}

	// Token: 0x060010A7 RID: 4263 RVA: 0x0000CE40 File Offset: 0x0000B040
	public static int getButtonFontSize()
	{
		return (int)(0.5f + (float)Screen.height / 40f);
	}

	// Token: 0x060010A8 RID: 4264 RVA: 0x0000CE55 File Offset: 0x0000B055
	private void SetCanOpenContextMenu(bool canOpenContextMenu)
	{
		if (!canOpenContextMenu)
		{
			this.CloseUserMenu();
		}
		this.canOpenContextMenu = canOpenContextMenu;
	}

	// Token: 0x060010A9 RID: 4265 RVA: 0x0000CE6A File Offset: 0x0000B06A
	public void FriendUpdated(string username)
	{
		if (this.userContextMenu != null && this.lastUserClicked.profile.name == username)
		{
			this.CloseUserMenu();
		}
	}

	// Token: 0x060010AA RID: 4266 RVA: 0x0000CE98 File Offset: 0x0000B098
	private void CloseUserMenu()
	{
		this.userContextMenu = null;
	}

	// Token: 0x060010AB RID: 4267 RVA: 0x000714C4 File Offset: 0x0006F6C4
	private void CreateUserMenu(Person user, bool isOnline)
	{
		if (!this.canOpenContextMenu)
		{
			return;
		}
		this.lastUserClicked = user;
		Vector3 mousePosition = Input.mousePosition;
		Rect rect;
		rect..ctor(Mathf.Min((float)(Screen.width - 105), mousePosition.x), Mathf.Min((float)(Screen.height - 90 - 5), (float)Screen.height - mousePosition.y), 100f, 30f);
		this.userContextMenu = new ContextMenu<Person>(user, rect);
		if (isOnline)
		{
			if (user.isInLobby())
			{
				this.userContextMenu.add("Challenge", new ContextMenu<Person>.URCMCallback(this.ChallengeUser));
				this.userContextMenu.add(new GUIContent("Trade").lockDemo(), new ContextMenu<Person>.URCMCallback(this.TradeUser));
				this.userContextMenu.add("Custom", new ContextMenu<Person>.URCMCallback(this.CustomChallengeUser));
			}
			if (user.isPlaying() || user.isSpectating())
			{
				this.userContextMenu.add("Spectate", new ContextMenu<Person>.URCMCallback(this.SpectateUser));
			}
			if (!App.IsParentalConsentNeeded())
			{
				this.userContextMenu.add("Whisper", new ContextMenu<Person>.URCMCallback(this.WhisperUser));
			}
		}
		this.userContextMenu.add("Profile", new ContextMenu<Person>.URCMCallback(this.ProfileUser));
	}

	// Token: 0x060010AC RID: 4268 RVA: 0x0000CEA1 File Offset: 0x0000B0A1
	private void ChallengeUser(Person user)
	{
		this.CloseUserMenu();
		App.GameActionManager.ChallengeUser(ChatUser.FromPerson(user));
	}

	// Token: 0x060010AD RID: 4269 RVA: 0x0000CEB9 File Offset: 0x0000B0B9
	private void CustomChallengeUser(Person user)
	{
		this.CloseUserMenu();
		App.GameActionManager.CustomChallengeUser(ChatUser.FromPerson(user));
	}

	// Token: 0x060010AE RID: 4270 RVA: 0x0000CED1 File Offset: 0x0000B0D1
	private void TradeUser(Person user)
	{
		this.CloseUserMenu();
		App.GameActionManager.TradeUser(ChatUser.FromPerson(user));
	}

	// Token: 0x060010AF RID: 4271 RVA: 0x0000CEE9 File Offset: 0x0000B0E9
	private void ProfileUser(Person user)
	{
		this.CloseUserMenu();
		App.GameActionManager.ProfileUser(ChatUser.FromPerson(user));
	}

	// Token: 0x060010B0 RID: 4272 RVA: 0x0000CF01 File Offset: 0x0000B101
	private void WhisperUser(Person user)
	{
		this.CloseUserMenu();
		App.ArenaChat.OpenWhisperRoom(user.profile.name);
	}

	// Token: 0x060010B1 RID: 4273 RVA: 0x0000CF1E File Offset: 0x0000B11E
	private void SpectateUser(Person user)
	{
		this.CloseUserMenu();
		App.GameActionManager.SpectateUser(user.profile);
	}

	// Token: 0x060010B2 RID: 4274 RVA: 0x0000CF36 File Offset: 0x0000B136
	public void ClearQueueStatuses()
	{
		this.queueStatuses.Clear();
		this.queueString = string.Empty;
	}

	// Token: 0x060010B3 RID: 4275 RVA: 0x00071620 File Offset: 0x0006F820
	public void UpdateQueueStatus(GameType gameType, bool inQueue)
	{
		if (inQueue)
		{
			this.queueStatuses.Add(gameType);
		}
		else
		{
			this.queueStatuses.Remove(gameType);
		}
		List<GameType> list = Enumerable.ToList<GameType>(this.queueStatuses);
		list.Sort();
		this.queueString = StringUtil.coordinate("or", Enumerable.ToList<string>(Enumerable.Select<GameType, string>(list, (GameType gt) => gt.getPrefix(false))));
	}

	// Token: 0x060010B4 RID: 4276 RVA: 0x0000CF4E File Offset: 0x0000B14E
	private bool isQueued()
	{
		return this.queueStatuses.Count > 0;
	}

	// Token: 0x060010B5 RID: 4277 RVA: 0x0000CF5E File Offset: 0x0000B15E
	public bool isQueuedIn(GameType gameType)
	{
		return this.queueStatuses.Contains(gameType);
	}

	// Token: 0x04000D0B RID: 3339
	private GUISkin guiSkin;

	// Token: 0x04000D0C RID: 3340
	private GUISkin guiSkinDisabled;

	// Token: 0x04000D0D RID: 3341
	private GUISkin guiSkinClear;

	// Token: 0x04000D0E RID: 3342
	private GUISkin chatButtonSkin;

	// Token: 0x04000D0F RID: 3343
	private GUISkin regularUI;

	// Token: 0x04000D10 RID: 3344
	private GUIStyle demoStyle;

	// Token: 0x04000D11 RID: 3345
	private Communicator comm;

	// Token: 0x04000D12 RID: 3346
	private AudioScript audioScript;

	// Token: 0x04000D13 RID: 3347
	private MockupCalc mockupJunk = new MockupCalc(1920, 1080);

	// Token: 0x04000D14 RID: 3348
	private List<LobbyMenu.Button> buttons = new List<LobbyMenu.Button>();

	// Token: 0x04000D15 RID: 3349
	private Person friendToRemove;

	// Token: 0x04000D16 RID: 3350
	private bool editingFriendsList;

	// Token: 0x04000D17 RID: 3351
	private Person lastUserClicked;

	// Token: 0x04000D18 RID: 3352
	private string queueString = string.Empty;

	// Token: 0x04000D19 RID: 3353
	private HashSet<GameType> queueStatuses = new HashSet<GameType>();

	// Token: 0x04000D1A RID: 3354
	private ContextMenu<Person> userContextMenu;

	// Token: 0x04000D1B RID: 3355
	private bool buttonsEnabled = true;

	// Token: 0x04000D1C RID: 3356
	private bool showFriendsList;

	// Token: 0x04000D1D RID: 3357
	private ScrollsFrame friendsListBorder;

	// Token: 0x04000D1E RID: 3358
	private Vector2 friendsListScroll;

	// Token: 0x04000D1F RID: 3359
	private string _sceneToLoad;

	// Token: 0x04000D20 RID: 3360
	private float overlayAlpha;

	// Token: 0x04000D21 RID: 3361
	private int _hoverButtonIndex = -1;

	// Token: 0x04000D22 RID: 3362
	private bool _hoverButtonInside;

	// Token: 0x04000D23 RID: 3363
	private float _hoverButtonStartTime;

	// Token: 0x04000D24 RID: 3364
	private float buttonMaxX;

	// Token: 0x04000D25 RID: 3365
	private GUIPositioner2 _headerPositioner;

	// Token: 0x04000D26 RID: 3366
	private GUIContent emptyContent = new GUIContent();

	// Token: 0x04000D27 RID: 3367
	private GUIContent helpArrowContent = new GUIContent().helpArrow();

	// Token: 0x04000D28 RID: 3368
	private bool canOpenContextMenu = true;

	// Token: 0x0200020B RID: 523
	private class Button
	{
		// Token: 0x060010B7 RID: 4279 RVA: 0x0000CF75 File Offset: 0x0000B175
		public Button(int index, string scene, GUISkin skin)
		{
			this.index = index;
			this.scene = scene;
			this.skin = skin;
		}

		// Token: 0x060010B8 RID: 4280 RVA: 0x0000CF99 File Offset: 0x0000B199
		public LobbyMenu.Button noHelp()
		{
			this.hasHelp = false;
			return this;
		}

		// Token: 0x060010B9 RID: 4281 RVA: 0x0000CFA3 File Offset: 0x0000B1A3
		public bool isCurrentScene()
		{
			return this.scene == SceneLoader.getScene();
		}

		// Token: 0x04000D2C RID: 3372
		public readonly int index;

		// Token: 0x04000D2D RID: 3373
		public readonly GUISkin skin;

		// Token: 0x04000D2E RID: 3374
		public readonly string scene;

		// Token: 0x04000D2F RID: 3375
		public bool hasHelp = true;
	}
}
