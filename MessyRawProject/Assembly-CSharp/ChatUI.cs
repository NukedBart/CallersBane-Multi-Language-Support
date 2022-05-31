using System;
using System.Collections;
using System.Collections.Generic;
using Gui;
using UnityEngine;

// Token: 0x0200013B RID: 315
public class ChatUI : AbstractCommListener, IOverlayClickCallback, IOkCallback, ICancelCallback, IJoinRoomCallback
{
	// Token: 0x14000003 RID: 3
	// (add) Token: 0x06000A42 RID: 2626 RVA: 0x00008A18 File Offset: 0x00006C18
	// (remove) Token: 0x06000A43 RID: 2627 RVA: 0x00008A31 File Offset: 0x00006C31
	public event ChatUI.ChatRoomDelegate ChatRoomLeftEvent;

	// Token: 0x06000A44 RID: 2628 RVA: 0x000028DF File Offset: 0x00000ADF
	public void OverlayClicked()
	{
	}

	// Token: 0x06000A45 RID: 2629 RVA: 0x00008A4A File Offset: 0x00006C4A
	public void SetLocked(bool locked)
	{
		this.SetLocked(locked, (float)Screen.height * 0.3f);
	}

	// Token: 0x06000A46 RID: 2630 RVA: 0x00008A5F File Offset: 0x00006C5F
	public void SetLocked(bool locked, float height)
	{
		this.locked = locked;
		this.targetChatHeight = height;
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x0004D7A8 File Offset: 0x0004B9A8
	public void SetContextMenuItems(params ContextMenu<ChatUser>.Item[] itemMask)
	{
		this.contextItems = (ContextMenu<ChatUser>.Item)0;
		foreach (ContextMenu<ChatUser>.Item item in itemMask)
		{
			this.contextItems |= item;
		}
	}

	// Token: 0x06000A48 RID: 2632 RVA: 0x00008A6F File Offset: 0x00006C6F
	public void SetCanOpenContextMenu(bool canOpenContextMenu)
	{
		if (!canOpenContextMenu)
		{
			this.CloseUserMenu();
		}
		this.canOpenContextMenu = canOpenContextMenu;
	}

	// Token: 0x06000A49 RID: 2633 RVA: 0x00008A84 File Offset: 0x00006C84
	public void SetSendMessageHandler(Action<Room, string> handler)
	{
		this.sendMessageHandler = handler;
	}

	// Token: 0x06000A4A RID: 2634 RVA: 0x0004D7E4 File Offset: 0x0004B9E4
	protected void Awake()
	{
		this.chatSkin = (GUISkin)ResourceManager.Load("_GUISkins/ChatUI");
		this.chatMinMaxSkin = (GUISkin)ResourceManager.Load("_GUISkins/ChatUIMinMax");
		this.chatTabActiveSkin = (GUISkin)ResourceManager.Load("_GUISkins/ChatTabActive");
		this.chatTabInactiveSkin = (GUISkin)ResourceManager.Load("_GUISkins/ChatTabInactive");
		this.chatButtonSkin = (GUISkin)ResourceManager.Load("_GUISkins/ChatButton");
		this.chatRooms = App.ArenaChat.ChatRooms;
		App.Communicator.addListener(this);
		this.anchor = new GameObject("Chat Anchor").GetComponent<Transform>();
		this.anchor.position = Vector3.zero;
		Object.DontDestroyOnLoad(this.anchor);
		this.overlay = new GameObject("ChatBGOverlay").AddComponent<GUIBlackOverlayButton>();
		Object.DontDestroyOnLoad(this.overlay);
		this.AdjustToResolution();
		this.overlay.enabled = false;
		this.show = false;
		this.anchor.transform.position = this.anchorDown;
		this.SetEnabled(false);
		this.chatRooms.ChatHighlight += this.ChatHighlight;
		this.chatRooms.ChatMessageReceived += this.ChatMessageReceived;
		this.SetMode(OnlineState.LOBBY);
	}

	// Token: 0x06000A4B RID: 2635 RVA: 0x0004D934 File Offset: 0x0004BB34
	public void AdjustToResolution()
	{
		this.chatHeight = (float)Screen.height * 0.3f;
		this.BORDER_WIDTH = (float)Screen.height * 0.01f;
		this.targetChatHeight = this.chatHeight;
		this.chatLogStyle = new GUIStyle(this.chatSkin.label);
		this.chatLogStyle.fontSize = 10 + Screen.height / 72;
		this.chatLogStyle.alignment = 0;
		this.chatLogStyle.wordWrap = true;
		this.timeStampStyle = new GUIStyle(this.chatLogStyle);
		this.timeStampStyle.fontSize = 8 + Screen.height / 80;
		this.timeStampStyle.margin.right = 2;
		this.timeStampStyle.margin.left = 6;
		this.timeStampStyle.margin.top = 1 + (int)(10f + (float)Screen.height / 72f - 8f - (float)Screen.height / 80f);
		this.timeStampStyle.alignment = 0;
		this.timeStampStyle.wordWrap = false;
		this.tabAndUserStyle = new GUIStyle(this.chatLogStyle);
		this.tabAndUserStyle.alignment = 3;
		this.tabAndUserStyle.padding.left = 6;
		this.tabAndUserStyle.wordWrap = false;
		this.chatMsgStyle = new GUIStyle(this.chatSkin.textField);
		this.RefreshAreas();
		this.overlay.Init(this, 14, this.fullArea, true);
		this.leftBorder = new ScrollsFrame().AddNinePatch(ScrollsFrame.Border.DARK_SHARP, NinePatch.Patches.TOP_LEFT | NinePatch.Patches.LEFT | NinePatch.Patches.CENTER).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.TOP | NinePatch.Patches.TOP_RIGHT | NinePatch.Patches.CENTER | NinePatch.Patches.RIGHT | NinePatch.Patches.BOTTOM_LEFT | NinePatch.Patches.BOTTOM | NinePatch.Patches.BOTTOM_RIGHT);
		this.midBorder = new ScrollsFrame().AddNinePatch(ScrollsFrame.Border.LIGHT_SHARP, NinePatch.Patches.CENTER);
		this.rightBorder = new ScrollsFrame().AddNinePatch(ScrollsFrame.Border.DARK_SHARP, NinePatch.Patches.TOP_RIGHT | NinePatch.Patches.CENTER | NinePatch.Patches.RIGHT).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.TOP_LEFT | NinePatch.Patches.TOP | NinePatch.Patches.LEFT | NinePatch.Patches.CENTER | NinePatch.Patches.BOTTOM_LEFT | NinePatch.Patches.BOTTOM | NinePatch.Patches.BOTTOM_RIGHT);
		this.anchorUp = Vector3.zero;
		this.anchorDown = this.GetAnchorDownPosition();
		this.ScrollDownChat(true);
		if (!this.isAtBottomAnchor)
		{
			this.MoveTo(this.anchorUp);
		}
		else
		{
			this.MoveTo(this.anchorDown);
		}
	}

	// Token: 0x06000A4C RID: 2636 RVA: 0x0004DB50 File Offset: 0x0004BD50
	public override void OnDestroy()
	{
		base.OnDestroy();
		if (this.chatRooms != null)
		{
			this.chatRooms.ChatHighlight -= this.ChatHighlight;
			this.chatRooms.ChatMessageReceived -= this.ChatMessageReceived;
		}
	}

	// Token: 0x06000A4D RID: 2637 RVA: 0x00008A8D File Offset: 0x00006C8D
	private void ChatHighlight(string roomName)
	{
		this.activeRoomNotifications.Add(roomName);
		if (App.AudioScript.GetSoundToggle(AudioScript.ESoundToggle.CHAT_HIGHLIGHT) && !this.AllChatSoundsMuted())
		{
			App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_chat_highlight");
		}
	}

	// Token: 0x06000A4E RID: 2638 RVA: 0x00008AC6 File Offset: 0x00006CC6
	private void ChatMessageReceived(string roomName)
	{
		if (App.AudioScript.GetSoundToggle(AudioScript.ESoundToggle.CHAT) && !this.AllChatSoundsMuted())
		{
			App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_chat");
		}
	}

	// Token: 0x06000A4F RID: 2639 RVA: 0x00008AF2 File Offset: 0x00006CF2
	private bool AllChatSoundsMuted()
	{
		return Application.loadedLevelName == "_SelectPreconstructed" || this.isBattling;
	}

	// Token: 0x06000A50 RID: 2640 RVA: 0x00008B11 File Offset: 0x00006D11
	public void Initiate()
	{
		this.initiatePersistentChatRooms();
		App.ArenaChat.InitiateOrRejoinAllActive();
		App.ArenaChat.GetRoomsList();
		this.SetEnabled(true);
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x0004DB9C File Offset: 0x0004BD9C
	private void initiatePersistentChatRooms()
	{
		List<string> value = App.Config.settings.chat.rooms.value;
		foreach (string text in value)
		{
			Log.warning("------> " + text);
			App.Communicator.send(new RoomEnterMessage(text));
		}
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x0004DC28 File Offset: 0x0004BE28
	public override void handleMessage(Message msg)
	{
		if (msg is RoomChatMessageMessage && ((RoomChatMessageMessage)msg).roomName == this.chatRooms.GetCurrentRoomName())
		{
			this.ScrollDownChat(false);
		}
		if (msg is WhisperMessage && ((WhisperMessage)msg).GetChatroomName() == this.chatRooms.GetCurrentRoomName())
		{
			this.ScrollDownChat(false);
		}
		if (msg is CliResponseMessage)
		{
			this.ScrollDownChat(true);
		}
		if (msg is UpdateWeeklyWinnersMessage)
		{
			App.ChatUI.SetWeeklyWinners(((UpdateWeeklyWinnersMessage)msg).weeklyWinners);
		}
		if (msg is RoomInfoMessage)
		{
			RoomInfoMessage roomInfoMessage = (RoomInfoMessage)msg;
			foreach (RoomInfoProfile roomInfoProfile in roomInfoMessage.updated)
			{
				if (roomInfoProfile.profileId == App.MyProfile.ProfileInfo.id)
				{
					this.SetAcceptChallenges(roomInfoProfile.acceptChallenges);
					this.SetAcceptTrades(roomInfoProfile.acceptTrades);
				}
			}
		}
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x00008B34 File Offset: 0x00006D34
	public void SetAcceptChallenges(bool acceptChallenges)
	{
		this.acceptChallenges = acceptChallenges;
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x00008B3D File Offset: 0x00006D3D
	public void SetAcceptTrades(bool acceptTrades)
	{
		this.acceptTrades = acceptTrades;
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x0004DD30 File Offset: 0x0004BF30
	public void ShowByUserAction(bool show)
	{
		if (show && App.IsParentalConsentNeeded())
		{
			string description = "You need parental consent to access the chat.\nThis is not currently set up on your account.";
			App.Popups.ShowOk(this, "parental_consent", "Notice", description, "Set up parental consent");
			App.Popups.AddCloseButton();
			return;
		}
		this.Show(show);
	}

	// Token: 0x06000A56 RID: 2646 RVA: 0x0004DD80 File Offset: 0x0004BF80
	public void Show(bool show)
	{
		if (base.enabled && !App.IsChatAllowed())
		{
			return;
		}
		if (this.show == show)
		{
			return;
		}
		if (!show && this.overlay != null)
		{
			this.overlay.enabled = false;
		}
		if (!show)
		{
			this.CloseUserMenu();
		}
		if (!show)
		{
			this.MoveTo(this.anchorDown);
		}
		else
		{
			this.isAtBottomAnchor = false;
			this.MoveTo(this.anchorUp);
		}
		this.show = show;
	}

	// Token: 0x06000A57 RID: 2647 RVA: 0x00008B46 File Offset: 0x00006D46
	public bool IsShown()
	{
		return this.show;
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x00008B4E File Offset: 0x00006D4E
	public void SetEnabled(bool enabled)
	{
		base.enabled = enabled;
		if (!enabled)
		{
			this.CloseUserMenu();
			this.overlay.enabled = false;
		}
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x0004DE10 File Offset: 0x0004C010
	public float GetHeight()
	{
		return this.chatHeight - this.anchor.position.y;
	}

	// Token: 0x06000A5A RID: 2650 RVA: 0x00008B6F File Offset: 0x00006D6F
	private Vector3 GetAnchorDownPosition()
	{
		return new Vector3(0f, this.chatHeight - this.BORDER_WIDTH, 0f);
	}

	// Token: 0x06000A5B RID: 2651 RVA: 0x0004DE38 File Offset: 0x0004C038
	private void MoveTo(Vector3 position)
	{
		if (this.anchor != null)
		{
			iTween.MoveTo(this.anchor.gameObject, iTween.Hash(new object[]
			{
				"x",
				position.x,
				"y",
				position.y,
				"z",
				position.z,
				"easetype",
				iTween.EaseType.easeInOutQuad,
				"time",
				0.5f,
				"oncompletetarget",
				base.gameObject,
				"oncomplete",
				"OnFinishedMove"
			}));
		}
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x00008B8D File Offset: 0x00006D8D
	public void OnFinishedMove()
	{
		if (this.show && this.overlay != null)
		{
			this.overlay.enabled = true;
		}
		if (!this.show)
		{
			this.isAtBottomAnchor = true;
		}
	}

	// Token: 0x06000A5D RID: 2653 RVA: 0x00008BC9 File Offset: 0x00006DC9
	public bool IsHovered()
	{
		return this.IsHovered(GUIUtil.getScreenMousePos());
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x00008BD6 File Offset: 0x00006DD6
	public bool IsHovered(Vector2 position)
	{
		return this.overlay.IsMouseOverArea(position) || this.minMaxArea.Contains(position);
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x00008BF8 File Offset: 0x00006DF8
	private void SwitchRooms(string room)
	{
		this.chatRooms.SetCurrentRoom(room);
		this.maxScroll = 0f;
		this.ScrollDownChat(true);
		this.ScrollUpUsers();
	}

	// Token: 0x06000A60 RID: 2656 RVA: 0x00008C1E File Offset: 0x00006E1E
	public void ScrollDownChat(bool forceScroll)
	{
		if (forceScroll || this.scrollLocked)
		{
			this.chatScroll = new Vector2(0f, float.PositiveInfinity);
			this.scrollLocked = true;
		}
	}

	// Token: 0x06000A61 RID: 2657 RVA: 0x00008C4D File Offset: 0x00006E4D
	private void ScrollUpUsers()
	{
		this.userScroll = new Vector2(0f, 0f);
	}

	// Token: 0x06000A62 RID: 2658 RVA: 0x00008C64 File Offset: 0x00006E64
	private void ScrollUpTabs()
	{
		this.tabScroll = new Vector2(0f, 0f);
	}

	// Token: 0x06000A63 RID: 2659 RVA: 0x00008C7B File Offset: 0x00006E7B
	private void FixedUpdate()
	{
		if (this.show && this.chatHeight != this.targetChatHeight)
		{
			this.chatHeight += (this.targetChatHeight - this.chatHeight) * 0.1f;
		}
	}

	// Token: 0x06000A64 RID: 2660 RVA: 0x0004DF04 File Offset: 0x0004C104
	private void handleChatHistory()
	{
		bool flag = false;
		if (Input.GetKeyDown(273))
		{
			flag |= this.chatHistory.previous();
		}
		if (Input.GetKeyDown(274))
		{
			flag |= this.chatHistory.next();
		}
		if (!flag)
		{
			return;
		}
		this.chatMsg = this.chatHistory.get();
	}

	// Token: 0x06000A65 RID: 2661 RVA: 0x0004DF68 File Offset: 0x0004C168
	private void Update()
	{
		if (Input.GetKeyDown(9))
		{
			this.performTabCompletion = true;
		}
		if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && this.userContextMenu != null && !this.userContextMenu.containsMouse())
		{
			this.CloseUserMenu();
		}
		this.handleChatHistory();
		this.anchorDown = this.GetAnchorDownPosition();
		if (this.show && !this.locked)
		{
			Vector2 vector;
			vector..ctor(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
			bool flag = this.show && this.resizeArea.Contains(vector) && !this.minMaxArea.Contains(vector);
			if (flag || this.dragging)
			{
				App.MouseCursor.SetCursor(MouseCursor.CursorType.RESIZE_VERTICAL);
			}
			else
			{
				App.MouseCursor.SetCursor(MouseCursor.CursorType.NORMAL);
			}
			if (flag && Input.GetMouseButtonDown(0))
			{
				this.dragging = true;
				this.dragDiff = Input.mousePosition.y - this.chatHeight;
			}
			if (Input.GetMouseButtonUp(0))
			{
				this.dragging = false;
			}
		}
		if (this.dragging)
		{
			this.ScrollDownChat(true);
			this.chatHeight = Mathf.Clamp(Input.mousePosition.y - this.dragDiff, (float)Screen.height * 0.25f, (float)Screen.height * 0.75f);
			this.targetChatHeight = this.chatHeight;
		}
	}

	// Token: 0x06000A66 RID: 2662 RVA: 0x00008CB9 File Offset: 0x00006EB9
	public void SetLeftRightWidths(float left, float right)
	{
		this.leftSide = left;
		this.rightSide = right;
	}

	// Token: 0x06000A67 RID: 2663 RVA: 0x0004E108 File Offset: 0x0004C308
	private void RefreshAreas()
	{
		this.fullArea = new Rect((float)Screen.width * this.MARGIN_SIDES, (float)Screen.height - this.chatHeight + this.anchor.position.y, (float)Screen.width - (float)(2 * Screen.width) * this.MARGIN_SIDES, this.chatHeight);
		if (this.overlay != null)
		{
			this.overlay.SetArea(this.fullArea);
		}
		this.marginS = (float)Screen.height * 0.015f;
		this.marginL = (float)Screen.height * 0.02f;
		float num = 40f;
		float num2 = (float)Screen.height * this.leftSide;
		float num3 = (float)Screen.height * this.rightSide;
		Rect rect;
		rect..ctor(this.fullArea.x, this.fullArea.y, num2, this.fullArea.height);
		Rect rect2;
		rect2..ctor(this.fullArea.xMax - num3, this.fullArea.y, num3, this.fullArea.height);
		Rect rect3;
		rect3..ctor(this.fullArea.x + num2, this.fullArea.y, this.fullArea.width - num2 - num3, this.fullArea.height);
		if (this.leftBorder != null)
		{
			this.leftBorder.SetRect(rect);
			this.rightBorder.SetRect(rect2);
			this.midBorder.SetRect(rect3);
		}
		this.tabArea = new Rect(this.fullArea.x + 1.5f * this.marginL, this.fullArea.y + this.marginL, num2 - 2.5f * this.marginL, this.fullArea.height - num - 2.2f * this.marginL);
		this.tabAreaInner = new Rect(this.tabArea.x + 5f, this.tabArea.y + 5f, this.tabArea.width - 10f, this.tabArea.height - 10f);
		this.usersArea = new Rect(this.fullArea.xMax - num3 + this.marginL, this.fullArea.y + this.marginL, num3 - 2.5f * this.marginL, this.fullArea.height - 2f * this.marginL);
		this.usersAreaInner = new Rect(this.usersArea.x + 5f, this.usersArea.y + 5f, this.usersArea.width - 10f, this.usersArea.height - 10f);
		this.chatlogArea = new Rect(this.fullArea.x + num2 + this.marginL, this.fullArea.y + this.marginL, this.fullArea.width - num2 - num3 - 2f * this.marginL, this.fullArea.height - num - 2.2f * this.marginL);
		this.chatlogAreaInner = new Rect(this.chatlogArea.x + 5f, this.chatlogArea.y + 5f, this.chatlogArea.width - 10f, this.chatlogArea.height - 10f);
		this.tabControlArea = new Rect(this.tabArea.x, (float)Screen.height - num + this.anchor.position.y - this.marginL, this.tabArea.width, num);
		this.chatInputArea = new Rect(this.chatlogArea.x, (float)Screen.height - num + this.anchor.position.y - this.marginL, this.chatlogArea.width * 0.85f, num);
		this.settingsArea = new Rect(this.chatlogArea.x + this.chatInputArea.width, (float)Screen.height - num + this.anchor.position.y - this.marginL, this.chatlogArea.width - this.chatInputArea.width, num);
		this.minMaxArea = new Rect((float)Screen.width * 0.5f - (float)Screen.height * 0.04f, this.fullArea.y - (float)Screen.height * 0.014f, (float)Screen.height * 0.08f, (float)Screen.height * 0.027f);
		this.resizeArea = new Rect(this.fullArea.x, this.fullArea.y, this.fullArea.width, this.BORDER_WIDTH);
	}

	// Token: 0x06000A68 RID: 2664 RVA: 0x0004E610 File Offset: 0x0004C810
	private void OnGUI()
	{
		GUI.depth = 12;
		GUI.skin = this.chatSkin;
		this.RefreshAreas();
		if (this.showChatTabs)
		{
			this.leftBorder.Draw();
		}
		this.midBorder.Draw();
		if (this.showUserList)
		{
			this.rightBorder.Draw();
		}
		if (this.userContextMenu != null)
		{
			this.userContextMenu.OnGUI_First();
		}
		if (this.isAtBottomAnchor && this.activeRoomNotifications.Count > 0)
		{
			GUI.color = new Color(1f, 1f, 1f, 0.4f + Mathf.Sin(Time.time * 4f) / 2.5f);
			Rect rect;
			rect..ctor(this.fullArea.x + (float)Screen.height * 0.018f, this.fullArea.y + (float)Screen.height * 0.0035f, this.fullArea.width - (float)(2 * Screen.height) * 0.018f, this.marginL);
			GUI.DrawTexture(rect, ResourceManager.LoadTexture("ChatUI/white"));
		}
		GUI.color = new Color(1f, 1f, 1f, 0.3f);
		GUI.Box(this.chatlogArea, string.Empty);
		GUI.color = Color.white;
		RoomLog currentRoomChatLog = this.chatRooms.GetCurrentRoomChatLog();
		if (currentRoomChatLog != null)
		{
			GUI.color = new Color(1f, 1f, 1f, 0.5f);
			GUI.Box(new Rect(this.chatlogAreaInner.xMax - 15f, this.chatlogAreaInner.y, 15f, this.chatlogAreaInner.height), string.Empty);
			GUI.color = Color.white;
			GUILayout.BeginArea(this.chatlogAreaInner);
			Vector2 vector = this.chatScroll;
			this.chatScroll = GUILayout.BeginScrollView(this.chatScroll, new GUILayoutOption[]
			{
				GUILayout.Width(this.chatlogAreaInner.width),
				GUILayout.Height(this.chatlogAreaInner.height)
			});
			if (this.chatScroll.y < vector.y && vector.y != float.PositiveInfinity)
			{
				this.scrollLocked = false;
			}
			else if (this.chatScroll.y > vector.y && this.maxScroll - this.chatScroll.y <= (float)(10 + Screen.height / 60))
			{
				this.scrollLocked = true;
			}
			if (this.chatScroll.y != float.PositiveInfinity)
			{
				this.maxScroll = Mathf.Max(this.maxScroll, this.chatScroll.y);
			}
			foreach (RoomLog.ChatLine chatLine in currentRoomChatLog.GetLines())
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label(chatLine.timestamp, this.timeStampStyle, new GUILayoutOption[]
				{
					GUILayout.Width(20f + (float)Screen.height * 0.042f)
				});
				GUI.color = new Color(1f, 1f, 1f, 0.65f);
				bool flag = false;
				if (this.weeklyWinners != null && this.weeklyWinners.Length >= 4)
				{
					for (int i = 0; i < 4; i++)
					{
						if (chatLine.from == this.weeklyWinners[i].userName)
						{
							GUILayout.Label(ResourceManager.LoadTexture(this.weeklyWinners[i].getIcon()), new GUILayoutOption[]
							{
								GUILayout.Width((float)this.chatLogStyle.fontSize),
								GUILayout.Height((float)this.chatLogStyle.fontSize)
							});
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					Texture texture = null;
					if (chatLine.senderAdminRole == AdminRole.Mojang)
					{
						texture = ResourceManager.LoadTexture("ChatUI/mojang_icon");
					}
					else if (chatLine.senderAdminRole == AdminRole.Admin)
					{
						texture = ResourceManager.LoadTexture("ChatUI/admin_icon");
					}
					else if (chatLine.senderAdminRole == AdminRole.Moderator)
					{
						texture = ResourceManager.LoadTexture("ChatUI/moderator_icon");
					}
					if (texture != null)
					{
						GUILayout.Label(texture, new GUILayoutOption[]
						{
							GUILayout.Width((float)this.chatLogStyle.fontSize),
							GUILayout.Height((float)this.chatLogStyle.fontSize)
						});
					}
				}
				GUI.color = Color.white;
				string text = chatLine.text;
				GUILayout.Label(text, this.chatLogStyle, new GUILayoutOption[]
				{
					GUILayout.Width(this.chatlogAreaInner.width - (float)Screen.height * 0.1f - 20f)
				});
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}
		Rect rect2;
		rect2..ctor(this.chatInputArea.x + 45f, this.chatInputArea.y, this.chatInputArea.width * 0.88f - 45f, this.chatInputArea.height);
		GUI.color = new Color(1f, 1f, 1f, 0.6f);
		GUI.Box(this.chatInputArea, string.Empty);
		GUI.color = Color.white;
		GUI.SetNextControlName("chatInput");
		string text2 = this.chatMsg;
		this.chatMsg = GUI.TextField(rect2, this.chatMsg, this.chatMsgStyle);
		if (this.chatMsg != text2)
		{
			this.lastTabCompletionSearch = string.Empty;
			this.lastTabCompletionName = string.Empty;
			this.chatHistory.reset();
		}
		this.chatMsg = this.chatMsg.Substring(0, Mathf.Min(this.chatMsg.Length, 512));
		TextAnchor alignment = GUI.skin.label.alignment;
		GUI.skin.label.alignment = 3;
		GUI.Label(new Rect(this.chatInputArea.x + this.chatInputArea.height * 0.22f, this.chatInputArea.y, this.chatInputArea.width, this.chatInputArea.height), "Say: ", this.chatMsgStyle);
		GUI.skin.label.alignment = alignment;
		bool flag2 = false;
		GUI.skin = this.chatButtonSkin;
		Rect rect3;
		rect3..ctor(this.chatInputArea.x + this.chatInputArea.width * 0.88f, this.chatInputArea.y + this.chatInputArea.height * 0.12f, this.chatInputArea.width * 0.12f - this.chatInputArea.height * 0.12f, this.chatInputArea.height - this.chatInputArea.height * 0.24f);
		int fontSize = GUI.skin.button.fontSize;
		GUI.skin.button.fontSize = Screen.height / 40;
		if (GUI.Button(rect3, "Send") && this.chatMsg.Length > 0)
		{
			flag2 = true;
		}
		GUI.skin.button.fontSize = fontSize;
		GUI.skin = this.chatSkin;
		if (App.Popups != null && !App.Popups.IsShowingPopup())
		{
			if ((Input.GetKeyDown(13) || Input.GetKeyDown(271)) && GUI.GetNameOfFocusedControl() == "chatInput" && this.chatMsg.Length > 0)
			{
				flag2 = true;
			}
			if (flag2)
			{
				this.SendChat();
			}
		}
		this.TabCompletion();
		if (this.showUserList)
		{
			GUI.color = new Color(1f, 1f, 1f, 0.3f);
			GUI.Box(this.usersArea, string.Empty);
			GUI.color = Color.white;
			GUILayout.Space(5f);
			List<ChatUser> currentRoomUsers = this.chatRooms.GetCurrentRoomUsers();
			float lineHeight = this.tabAndUserStyle.lineHeight;
			if (currentRoomUsers != null)
			{
				GUI.color = new Color(1f, 1f, 1f, 0.5f);
				GUI.Box(new Rect(this.usersAreaInner.xMax - 15f, this.usersAreaInner.y, 15f, this.usersAreaInner.height), string.Empty);
				GUI.color = Color.white;
				this.userScroll = GUI.BeginScrollView(this.usersAreaInner, this.userScroll, new Rect(0f, 0f, this.usersAreaInner.width - 20f, (float)currentRoomUsers.Count * lineHeight));
				for (int j = 0; j < currentRoomUsers.Count; j++)
				{
					ChatUser chatUser = currentRoomUsers[j];
					string text3 = chatUser.name;
					if (!chatUser.acceptChallenges)
					{
						GUI.color = new Color(1f, 1f, 1f, 0.3f);
					}
					GUI.DrawTexture(new Rect(5f, lineHeight * (float)j + 3f, lineHeight - 5f, lineHeight - 5f), ResourceManager.LoadTexture("ChatUI/challenge_icon"));
					GUI.color = Color.white;
					if (chatUser.featureType.isDemo())
					{
						GUI.DrawTexture(new Rect(5f + lineHeight - 2f, lineHeight * (float)j + 3f, lineHeight - 5f, lineHeight - 5f), ResourceManager.LoadTexture("ChatUI/demo_icon"));
					}
					else
					{
						if (!chatUser.acceptTrades)
						{
							GUI.color = new Color(1f, 1f, 1f, 0.3f);
						}
						GUI.DrawTexture(new Rect(5f + lineHeight - 2f, lineHeight * (float)j + 3f, lineHeight - 5f, lineHeight - 5f), ResourceManager.LoadTexture("ChatUI/trade_icon"));
						GUI.color = Color.white;
					}
					float num = 5f + 2f * lineHeight - 7f;
					Texture2D texture2D = null;
					bool flag3 = false;
					if (this.weeklyWinners != null && this.weeklyWinners.Length >= 4)
					{
						for (int k = 0; k < 4; k++)
						{
							if (chatUser.profileId == this.weeklyWinners[k].profileId)
							{
								texture2D = ResourceManager.LoadTexture(this.weeklyWinners[k].getIcon());
								text3 = "<color=#ebbbaf>" + text3 + "</color>";
								flag3 = true;
								break;
							}
						}
					}
					if (!flag3)
					{
						if (chatUser.featureType.isDemo())
						{
							text3 = "<color=#a59585>" + text3 + "</color>";
						}
						else if (chatUser.adminRole == AdminRole.Mojang)
						{
							texture2D = ResourceManager.LoadTexture("ChatUI/mojang_icon");
							text3 = "<color=#f9a851>" + text3 + "</color>";
						}
						else if (chatUser.adminRole == AdminRole.Admin)
						{
							texture2D = ResourceManager.LoadTexture("ChatUI/admin_icon");
							text3 = "<color=#f9a851>" + text3 + "</color>";
						}
						else if (chatUser.adminRole == AdminRole.Moderator)
						{
							texture2D = ResourceManager.LoadTexture("ChatUI/moderator_icon");
							text3 = "<color=#ddcc88>" + text3 + "</color>";
						}
					}
					if (texture2D != null)
					{
						Rect rect4;
						rect4..ctor(num + 3f, lineHeight * (float)j + 3f, lineHeight - 5f, lineHeight - 5f);
						GUI.DrawTexture(rect4, texture2D);
						num += lineHeight - 3f;
					}
					if (GUI.Button(new Rect(num, lineHeight * (float)j, this.usersAreaInner.width - num - 20f, lineHeight), text3, this.tabAndUserStyle) && App.MyProfile.ProfileInfo.id != chatUser.profileId && this.allowSendingChallenges && this.userContextMenu == null)
					{
						this.CreateUserMenu(chatUser);
						App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
					}
				}
				GUI.EndScrollView();
			}
		}
		if (this.showChatTabs)
		{
			GUI.color = new Color(1f, 1f, 1f, 0.3f);
			GUI.Box(this.tabArea, string.Empty);
			GUI.color = Color.white;
			float num2 = this.tabAndUserStyle.lineHeight + 12f;
			float num3 = num2 + 3f;
			float num4 = num3 * (float)this.chatRooms.GetAllRooms().Count;
			float num5 = this.tabAreaInner.width - 16f;
			GUI.color = new Color(1f, 1f, 1f, 0.5f);
			GUI.Box(new Rect(this.tabAreaInner.xMax - 15f, this.tabAreaInner.y, 15f, this.tabAreaInner.height), string.Empty);
			GUI.color = Color.white;
			this.tabScroll = GUI.BeginScrollView(this.tabAreaInner, this.tabScroll, new Rect(0f, 0f, num5, num4));
			GUISkin skin = GUI.skin;
			int num6 = 0;
			Room room = null;
			foreach (Room room2 in this.chatRooms.GetAllRooms())
			{
				string name = room2.name;
				bool flag4 = room2 == this.chatRooms.GetCurrentRoom();
				if (flag4)
				{
					GUI.skin = this.chatTabActiveSkin;
				}
				else
				{
					GUI.skin = this.chatTabInactiveSkin;
				}
				Rect rect5;
				rect5..ctor(0f, num3 * (float)num6, num5 - 3f, num2);
				if (flag4)
				{
					bool flag5 = true;
					long num7 = App.SceneValues.battleMode.gameId();
					string spectateRoomName = ChatRooms.GetSpectateRoomName(num7);
					if (num7 > 0L && room2.name.ToLower() == spectateRoomName.ToLower() && SceneLoader.isScene(new string[]
					{
						"_BattleModeView"
					}))
					{
						flag5 = false;
					}
					GUI.Box(rect5, string.Empty);
					if (flag5 && GUI.Button(new Rect(rect5.width - num2 * 6.5f / 8f, rect5.y + num2 / 7.5f, num2 * 6f / 8f, num2 * 6f / 8f), string.Empty))
					{
						App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
						room = room2;
					}
				}
				else if (GUI.Button(rect5, string.Empty))
				{
					this.SwitchRooms(name);
				}
				if (this.activeRoomNotifications.Contains(name))
				{
					if (!this.isAtBottomAnchor && this.chatRooms.GetCurrentRoom() == room2)
					{
						this.activeRoomNotifications.Remove(name);
					}
					else
					{
						GUI.color = new Color(1f, 1f, 1f, 0.08f + Mathf.Sin(Time.time * 3f) * 0.08f);
						GUI.DrawTexture(rect5, ResourceManager.LoadTexture("ChatUI/white"));
						GUI.color = Color.white;
					}
				}
				string text4 = (room2.log.allRead && this.chatRooms.GetCurrentRoom() != room2) ? "<color=#777460>" : "<color=white>";
				if (room2.type == RoomType.WhisperRoom)
				{
					text4 = ((room2.log.allRead && this.chatRooms.GetCurrentRoom() != room2) ? "<color=#997730>[W] " : "<color=#ffcc60>[W] ");
				}
				GUI.Label(new Rect(0f, rect5.y, rect5.width * ((!flag4) ? 1f : 0.8f), rect5.height), text4 + ((name.Length >= 17) ? (name.Substring(0, 16) + "...") : name) + "</color>", this.tabAndUserStyle);
				num6++;
			}
			if (room != null)
			{
				App.ArenaChat.RoomExit(room, false);
				this.chatScroll = new Vector2(0f, float.PositiveInfinity);
				if (this.ChatRoomLeftEvent != null)
				{
					this.ChatRoomLeftEvent(room.name);
				}
			}
			GUI.skin = skin;
			GUI.EndScrollView();
			GUI.skin = this.chatButtonSkin;
			Rect rect6;
			rect6..ctor(this.tabControlArea.x, this.tabControlArea.y, this.tabControlArea.width, this.tabControlArea.height);
			if (GUI.Button(rect6, "Join room"))
			{
				App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
				App.ArenaChat.GetRoomsList();
				App.Popups.ShowJoinRoom(this, this.chatRooms.GetListedRooms());
			}
			GUI.Label(rect6, "Join room");
		}
		if (this.showAcceptInvites)
		{
			float num8 = this.settingsArea.height * 0.1f;
			Rect rect7;
			rect7..ctor(this.settingsArea.x + num8, this.settingsArea.y, this.settingsArea.width / 2f - num8, this.settingsArea.height);
			if (GUI.Button(rect7, string.Empty))
			{
				App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
				App.Communicator.send(new SetAcceptChallengesMessage(!this.acceptChallenges));
			}
			Rect rect8;
			rect8..ctor(this.settingsArea.x + this.settingsArea.width / 2f + num8, this.settingsArea.y, this.settingsArea.width / 2f - num8, this.settingsArea.height);
			if (GUI.Button(rect8, string.Empty))
			{
				App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
				App.Communicator.send(new SetAcceptTradesMessage(!this.acceptTrades));
			}
			float num9 = this.settingsArea.height * 0.1f;
			GUI.color = ((!this.acceptChallenges) ? new Color(1f, 1f, 1f, 0.3f) : Color.white);
			GUI.Label(new Rect(rect7.x, rect7.y + num9, rect7.width, rect7.height - 2f * num9), ResourceManager.LoadTexture("ChatUI/challenge_icon"));
			GUI.color = ((!this.acceptTrades) ? new Color(1f, 1f, 1f, 0.3f) : Color.white);
			GUI.Label(new Rect(rect8.x, rect8.y + num9, rect8.width, rect8.height - 2f * num9), ResourceManager.LoadTexture("ChatUI/trade_icon"));
			GUI.color = Color.white;
			Vector2 screenMousePos = GUIUtil.getScreenMousePos();
			TextAnchor alignment2 = GUI.skin.box.alignment;
			GUI.skin.box.alignment = 4;
			float num10 = 150f;
			float num11 = 40f;
			if (rect7.Contains(screenMousePos))
			{
				GUI.Box(new Rect(screenMousePos.x - num10 * 1.05f, screenMousePos.y - num11 * 1.05f, num10, num11), "Accept challenges?");
			}
			if (rect8.Contains(screenMousePos))
			{
				GUI.Box(new Rect(screenMousePos.x - num10 * 1.05f, screenMousePos.y - num11 * 1.05f, num10, num11), "Accept trade invites?");
			}
			GUI.skin.box.alignment = alignment2;
		}
		GUI.skin = this.chatMinMaxSkin;
		if (!this.locked && GUI.Button(this.minMaxArea, string.Empty))
		{
			App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
			this.ShowByUserAction(!this.show);
		}
		GUI.skin = this.chatButtonSkin;
		if (this.userContextMenu != null)
		{
			this.userContextMenu.OnGUI_Last();
		}
	}

	// Token: 0x06000A69 RID: 2665 RVA: 0x0004FB2C File Offset: 0x0004DD2C
	private void SendChat()
	{
		string text = this.chatMsg.Substring(0, Mathf.Min(this.chatMsg.Length, 512));
		if (text.Trim().Length == 0)
		{
			return;
		}
		Room currentRoom = this.chatRooms.GetCurrentRoom();
		text = App.ArenaChat.AdjustIfReplyMessage(text);
		switch (currentRoom.type)
		{
		case RoomType.ChatRoom:
			this.ScrollDownChat(true);
			this.OnSendMessage(currentRoom, text);
			break;
		case RoomType.WhisperRoom:
			this.ScrollDownChat(true);
			App.Communicator.send(new WhisperMessage(currentRoom.name, text));
			break;
		}
		this.chatHistory.add(this.chatMsg);
		this.chatMsg = string.Empty;
		this.lastTabCompletionName = string.Empty;
		this.lastTabCompletionSearch = string.Empty;
	}

	// Token: 0x06000A6A RID: 2666 RVA: 0x00008CC9 File Offset: 0x00006EC9
	public void PopupJoinRoom(string popupType, string roomName, bool autoIncrement)
	{
		if (!string.IsNullOrEmpty(roomName))
		{
			if (autoIncrement)
			{
				App.ArenaChat.RoomEnterFree(roomName);
			}
			else
			{
				App.ArenaChat.RoomEnter(roomName);
			}
		}
	}

	// Token: 0x06000A6B RID: 2667 RVA: 0x000028DF File Offset: 0x00000ADF
	public void PopupCancel(string popupType)
	{
	}

	// Token: 0x06000A6C RID: 2668 RVA: 0x0004FC10 File Offset: 0x0004DE10
	private void CreateUserMenu(ChatUser user)
	{
		if (!this.canOpenContextMenu)
		{
			return;
		}
		this.userContextMenu = new ContextMenu<ChatUser>(user);
		if (this.HasContextItem(ContextMenu<ChatUser>.Item.Challenge) && user.acceptChallenges)
		{
			this.userContextMenu.add("Challenge", new ContextMenu<ChatUser>.URCMCallback(this.ChallengeUser));
		}
		if (this.HasContextItem(ContextMenu<ChatUser>.Item.CustomChallenge) && user.acceptChallenges)
		{
			this.userContextMenu.add("Custom Match", new ContextMenu<ChatUser>.URCMCallback(this.CustomChallengeUser));
		}
		if (this.HasContextItem(ContextMenu<ChatUser>.Item.Trade) && user.acceptTrades && user.featureType.isPremium())
		{
			this.userContextMenu.add(new GUIContent("Trade").lockDemo(), new ContextMenu<ChatUser>.URCMCallback(this.TradeUser));
		}
		if (!App.IsParentalConsentNeeded() && this.HasContextItem(ContextMenu<ChatUser>.Item.Whisper))
		{
			this.userContextMenu.add("Whisper", new ContextMenu<ChatUser>.URCMCallback(this.WhisperUser));
		}
		if (this.HasContextItem(ContextMenu<ChatUser>.Item.Profile))
		{
			this.userContextMenu.add("Profile", new ContextMenu<ChatUser>.URCMCallback(this.ProfileUser));
		}
		if (App.ServerSettings.friendsListEnabled && !App.FriendList.IsFriend(user.name) && !App.FriendList.IsFriendPending(user.name) && this.HasContextItem(ContextMenu<ChatUser>.Item.AddFriend))
		{
			this.userContextMenu.add("Add Friend", new ContextMenu<ChatUser>.URCMCallback(this.FriendUser));
		}
		if (!App.FriendList.IsBlocked(user.name))
		{
			if (this.HasContextItem(ContextMenu<ChatUser>.Item.Ignore))
			{
				this.userContextMenu.add("Ignore", new ContextMenu<ChatUser>.URCMCallback(this.BlockUser));
			}
		}
		else if (this.HasContextItem(ContextMenu<ChatUser>.Item.Unignore))
		{
			this.userContextMenu.add("Unignore", new ContextMenu<ChatUser>.URCMCallback(this.UnblockUser));
		}
		Vector3 mousePosition = Input.mousePosition;
		int num = this.userContextMenu.size();
		Rect rect;
		rect..ctor(Mathf.Min((float)(Screen.width - 135), mousePosition.x), Mathf.Min((float)(Screen.height - 30 * num - 5), (float)Screen.height - mousePosition.y), 130f, 30f);
		this.userContextMenu.setRect(rect);
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x00008CF7 File Offset: 0x00006EF7
	private bool HasContextItem(ContextMenu<ChatUser>.Item item)
	{
		return (this.contextItems & item) != (ContextMenu<ChatUser>.Item)0;
	}

	// Token: 0x06000A6E RID: 2670 RVA: 0x00008D07 File Offset: 0x00006F07
	private void CloseUserMenu()
	{
		this.userContextMenu = null;
		base.StartCoroutine(this.EnableChallengesInOneFrame());
	}

	// Token: 0x06000A6F RID: 2671 RVA: 0x00008D1D File Offset: 0x00006F1D
	private void ChallengeUser(ChatUser user)
	{
		this.CloseUserMenu();
		App.GameActionManager.ChallengeUser(user);
	}

	// Token: 0x06000A70 RID: 2672 RVA: 0x00008D30 File Offset: 0x00006F30
	private void CustomChallengeUser(ChatUser user)
	{
		this.CloseUserMenu();
		App.GameActionManager.CustomChallengeUser(user);
	}

	// Token: 0x06000A71 RID: 2673 RVA: 0x00008D43 File Offset: 0x00006F43
	private void TradeUser(ChatUser user)
	{
		this.CloseUserMenu();
		App.GameActionManager.TradeUser(user);
	}

	// Token: 0x06000A72 RID: 2674 RVA: 0x00008D56 File Offset: 0x00006F56
	private void ProfileUser(ChatUser user)
	{
		this.CloseUserMenu();
		App.GameActionManager.ProfileUser(user);
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x00008D69 File Offset: 0x00006F69
	private void FriendUser(ChatUser user)
	{
		this.CloseUserMenu();
		App.FriendList.AddFriend(user.name);
	}

	// Token: 0x06000A74 RID: 2676 RVA: 0x00008D81 File Offset: 0x00006F81
	private void WhisperUser(ChatUser user)
	{
		this.CloseUserMenu();
		App.ArenaChat.OpenWhisperRoom(user.name);
	}

	// Token: 0x06000A75 RID: 2677 RVA: 0x00008D99 File Offset: 0x00006F99
	private void BlockUser(ChatUser user)
	{
		this.CloseUserMenu();
		App.FriendList.BlockUser(user.name);
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x00008DB1 File Offset: 0x00006FB1
	private void UnblockUser(ChatUser user)
	{
		this.CloseUserMenu();
		App.FriendList.UnblockUser(user.name);
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x0004FE7C File Offset: 0x0004E07C
	private IEnumerator EnableChallengesInOneFrame()
	{
		this.allowSendingChallenges = false;
		yield return null;
		this.allowSendingChallenges = true;
		yield break;
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x00008DC9 File Offset: 0x00006FC9
	private void OnSendMessage(Room room, string message)
	{
		if (this.sendMessageHandler != null)
		{
			this.sendMessageHandler.Invoke(room, message);
		}
		else
		{
			App.Communicator.send(new RoomChatMessageMessage(room.name, message));
		}
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x0004FE98 File Offset: 0x0004E098
	private void TabCompletion()
	{
		if (GUI.GetNameOfFocusedControl() == "chatInput" && this.performTabCompletion)
		{
			this.performTabCompletion = false;
			string text = this.chatMsg.TrimEnd(null);
			if (text.EndsWith(":"))
			{
				text = text.Substring(0, text.Length - 1);
			}
			if (text.Length > 0)
			{
				int num = text.LastIndexOfAny(ChatUI.wordDelimiters) + 1;
				string text2 = text.Substring(num).ToLower();
				if (string.IsNullOrEmpty(this.lastTabCompletionSearch))
				{
					this.lastTabCompletionSearch = text2;
				}
				List<ChatUser> currentRoomUsers = this.chatRooms.GetCurrentRoomUsers();
				int num2 = 0;
				for (int i = 0; i < currentRoomUsers.Count; i++)
				{
					if (currentRoomUsers[i].name.ToLower() == this.lastTabCompletionName)
					{
						num2 = i + 1;
						break;
					}
				}
				for (int j = num2; j < num2 + currentRoomUsers.Count; j++)
				{
					ChatUser chatUser = currentRoomUsers[j % currentRoomUsers.Count];
					if (chatUser.name.ToLower().StartsWith((!string.IsNullOrEmpty(this.lastTabCompletionSearch)) ? this.lastTabCompletionSearch : text2))
					{
						this.chatMsg = text.Remove(num) + chatUser.name;
						if (num == 0)
						{
							this.chatMsg += ":";
						}
						this.chatMsg += " ";
						this.lastTabCompletionName = chatUser.name.ToLower();
						try
						{
							TextEditor textEditor = GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl) as TextEditor;
							textEditor.pos = this.chatMsg.Length;
							textEditor.selectPos = this.chatMsg.Length;
						}
						catch (Exception ex)
						{
							Log.error("Unable to update caret position. Perhaps the Unity version is lacking support for this. Error: " + ex.Message);
						}
						break;
					}
				}
			}
		}
	}

	// Token: 0x06000A7A RID: 2682 RVA: 0x00008DFF File Offset: 0x00006FFF
	public void SetIsBattling(bool isBattling)
	{
		this.isBattling = isBattling;
	}

	// Token: 0x06000A7B RID: 2683 RVA: 0x000500C4 File Offset: 0x0004E2C4
	public void SetMode(OnlineState state)
	{
		if (state == OnlineState.LOBBY)
		{
			this.SetContextMenuItems(new ContextMenu<ChatUser>.Item[]
			{
				ContextMenu<ChatUser>.Item.Challenge,
				ContextMenu<ChatUser>.Item.CustomChallenge,
				ContextMenu<ChatUser>.Item.Trade,
				ContextMenu<ChatUser>.Item.Whisper,
				ContextMenu<ChatUser>.Item.Profile,
				ContextMenu<ChatUser>.Item.AddFriend,
				ContextMenu<ChatUser>.Item.Ignore,
				ContextMenu<ChatUser>.Item.Unignore
			});
			this.MARGIN_SIDES = 0.01f;
			this.SetLeftRightWidths(0.3f, 0.3f);
			this.midBorder = new ScrollsFrame().AddNinePatch(ScrollsFrame.Border.LIGHT_SHARP, NinePatch.Patches.CENTER);
			this.showChatTabs = (this.showUserList = (this.showAcceptInvites = true));
			this.SetSendMessageHandler(null);
		}
		else if (state == OnlineState.SPECTATE)
		{
			this.SetContextMenuItems(new ContextMenu<ChatUser>.Item[]
			{
				ContextMenu<ChatUser>.Item.Whisper,
				ContextMenu<ChatUser>.Item.Ignore,
				ContextMenu<ChatUser>.Item.Unignore
			});
		}
	}

	// Token: 0x06000A7C RID: 2684 RVA: 0x00008E08 File Offset: 0x00007008
	public void SetWeeklyWinners(WeeklyWinner[] _weeklyWinners)
	{
		this.weeklyWinners = _weeklyWinners;
	}

	// Token: 0x06000A7D RID: 2685 RVA: 0x00050180 File Offset: 0x0004E380
	public void PopupOk(string popupType)
	{
		if (popupType == "parental_consent")
		{
			string profileUuid = App.MyProfile.ProfileInfo.profileUuid;
			Application.OpenURL("https://accounts.mojang.com/account/requestConsent/" + profileUuid);
		}
	}

	// Token: 0x040007CF RID: 1999
	private const float DefaultSideWidth = 0.3f;

	// Token: 0x040007D0 RID: 2000
	private static char[] wordDelimiters = new char[]
	{
		' ',
		'\t',
		',',
		'.',
		'!',
		'?',
		'(',
		')',
		'[',
		']',
		'@'
	};

	// Token: 0x040007D1 RID: 2001
	private string lastTabCompletionName = string.Empty;

	// Token: 0x040007D2 RID: 2002
	private string lastTabCompletionSearch = string.Empty;

	// Token: 0x040007D3 RID: 2003
	private bool performTabCompletion;

	// Token: 0x040007D4 RID: 2004
	private float BORDER_WIDTH;

	// Token: 0x040007D5 RID: 2005
	private float MARGIN_SIDES = 0.01f;

	// Token: 0x040007D6 RID: 2006
	private ChatRooms chatRooms;

	// Token: 0x040007D7 RID: 2007
	private GUISkin chatSkin;

	// Token: 0x040007D8 RID: 2008
	private GUISkin chatMinMaxSkin;

	// Token: 0x040007D9 RID: 2009
	private GUISkin chatTabActiveSkin;

	// Token: 0x040007DA RID: 2010
	private GUISkin chatTabInactiveSkin;

	// Token: 0x040007DB RID: 2011
	private GUISkin chatButtonSkin;

	// Token: 0x040007DC RID: 2012
	private Vector2 chatScroll = new Vector2(0f, float.PositiveInfinity);

	// Token: 0x040007DD RID: 2013
	private float maxScroll;

	// Token: 0x040007DE RID: 2014
	private bool scrollLocked = true;

	// Token: 0x040007DF RID: 2015
	private Vector2 userScroll = new Vector2(0f, 0f);

	// Token: 0x040007E0 RID: 2016
	private Vector2 tabScroll = new Vector2(0f, 0f);

	// Token: 0x040007E1 RID: 2017
	private string chatMsg = string.Empty;

	// Token: 0x040007E2 RID: 2018
	private ChatHistory chatHistory = new ChatHistory(10);

	// Token: 0x040007E3 RID: 2019
	private bool acceptChallenges = true;

	// Token: 0x040007E4 RID: 2020
	private bool acceptTrades = true;

	// Token: 0x040007E5 RID: 2021
	private bool allowSendingChallenges = true;

	// Token: 0x040007E6 RID: 2022
	private ContextMenu<ChatUser> userContextMenu;

	// Token: 0x040007E7 RID: 2023
	private ContextMenu<ChatUser>.Item contextItems;

	// Token: 0x040007E8 RID: 2024
	private Action<Room, string> sendMessageHandler;

	// Token: 0x040007E9 RID: 2025
	private GUIBlackOverlayButton overlay;

	// Token: 0x040007EA RID: 2026
	private Rect fullArea;

	// Token: 0x040007EB RID: 2027
	private Rect tabArea;

	// Token: 0x040007EC RID: 2028
	private Rect tabAreaInner;

	// Token: 0x040007ED RID: 2029
	private Rect tabControlArea;

	// Token: 0x040007EE RID: 2030
	private Rect usersArea;

	// Token: 0x040007EF RID: 2031
	private Rect usersAreaInner;

	// Token: 0x040007F0 RID: 2032
	private Rect settingsArea;

	// Token: 0x040007F1 RID: 2033
	private Rect chatlogArea;

	// Token: 0x040007F2 RID: 2034
	private Rect chatlogAreaInner;

	// Token: 0x040007F3 RID: 2035
	private Rect chatInputArea;

	// Token: 0x040007F4 RID: 2036
	private Rect minMaxArea;

	// Token: 0x040007F5 RID: 2037
	private Rect resizeArea;

	// Token: 0x040007F6 RID: 2038
	private float chatHeight;

	// Token: 0x040007F7 RID: 2039
	private float targetChatHeight;

	// Token: 0x040007F8 RID: 2040
	private Transform anchor;

	// Token: 0x040007F9 RID: 2041
	private Vector3 anchorUp;

	// Token: 0x040007FA RID: 2042
	private Vector3 anchorDown;

	// Token: 0x040007FB RID: 2043
	private bool show;

	// Token: 0x040007FC RID: 2044
	private bool showChatTabs = true;

	// Token: 0x040007FD RID: 2045
	private bool showUserList = true;

	// Token: 0x040007FE RID: 2046
	private bool showAcceptInvites = true;

	// Token: 0x040007FF RID: 2047
	private bool isAtBottomAnchor = true;

	// Token: 0x04000800 RID: 2048
	private GUIStyle chatLogStyle;

	// Token: 0x04000801 RID: 2049
	private GUIStyle tabAndUserStyle;

	// Token: 0x04000802 RID: 2050
	private GUIStyle timeStampStyle;

	// Token: 0x04000803 RID: 2051
	private GUIStyle chatMsgStyle;

	// Token: 0x04000804 RID: 2052
	private bool dragging;

	// Token: 0x04000805 RID: 2053
	private float dragDiff;

	// Token: 0x04000806 RID: 2054
	private ScrollsFrame leftBorder;

	// Token: 0x04000807 RID: 2055
	private ScrollsFrame midBorder;

	// Token: 0x04000808 RID: 2056
	private ScrollsFrame rightBorder;

	// Token: 0x04000809 RID: 2057
	private bool locked;

	// Token: 0x0400080A RID: 2058
	protected bool canOpenContextMenu = true;

	// Token: 0x0400080B RID: 2059
	private bool isBattling;

	// Token: 0x0400080C RID: 2060
	private HashSet<string> activeRoomNotifications = new HashSet<string>();

	// Token: 0x0400080D RID: 2061
	private WeeklyWinner[] weeklyWinners;

	// Token: 0x0400080E RID: 2062
	private float leftSide = 0.3f;

	// Token: 0x0400080F RID: 2063
	private float rightSide = 0.3f;

	// Token: 0x04000810 RID: 2064
	private float marginS;

	// Token: 0x04000811 RID: 2065
	private float marginL;

	// Token: 0x0200013C RID: 316
	private enum WinType
	{
		// Token: 0x04000814 RID: 2068
		FIRST,
		// Token: 0x04000815 RID: 2069
		SECOND,
		// Token: 0x04000816 RID: 2070
		THIRD,
		// Token: 0x04000817 RID: 2071
		MOST_WINS,
		// Token: 0x04000818 RID: 2072
		COUNT
	}

	// Token: 0x0200013D RID: 317
	// (Invoke) Token: 0x06000A7F RID: 2687
	public delegate void ChatRoomDelegate(string roomName);
}
