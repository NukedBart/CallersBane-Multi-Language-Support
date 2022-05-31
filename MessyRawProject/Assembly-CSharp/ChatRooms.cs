using System;
using System.Collections.Generic;

// Token: 0x02000138 RID: 312
public class ChatRooms
{
	// Token: 0x14000001 RID: 1
	// (add) Token: 0x06000A1C RID: 2588 RVA: 0x0000884D File Offset: 0x00006A4D
	// (remove) Token: 0x06000A1D RID: 2589 RVA: 0x00008866 File Offset: 0x00006A66
	public event ChatRooms.ChatRoomNotification ChatHighlight;

	// Token: 0x14000002 RID: 2
	// (add) Token: 0x06000A1E RID: 2590 RVA: 0x0000887F File Offset: 0x00006A7F
	// (remove) Token: 0x06000A1F RID: 2591 RVA: 0x00008898 File Offset: 0x00006A98
	public event ChatRooms.ChatRoomNotification ChatMessageReceived;

	// Token: 0x06000A20 RID: 2592 RVA: 0x0004CF94 File Offset: 0x0004B194
	public void SetCurrentRoom(string roomName)
	{
		if (!this.rooms.Contains(roomName))
		{
			if (roomName == "[Notice]" || roomName.StartsWith(ChatRooms.ReplayRoomPrefix))
			{
				this.rooms.Add(roomName, RoomType.FakeRoom);
			}
			else
			{
				this.rooms.Add(roomName, RoomType.ChatRoom);
			}
		}
		this.currentRoom = this.rooms.Get(roomName);
		this.currentRoom.log.allRead = true;
	}

	// Token: 0x06000A21 RID: 2593 RVA: 0x0004D014 File Offset: 0x0004B214
	public void OpenWhisperRoom(string roomName)
	{
		if (!this.rooms.Contains(roomName))
		{
			this.rooms.Add(roomName, RoomType.WhisperRoom);
		}
		this.currentRoom = this.rooms.Get(roomName);
		this.currentRoom.log.allRead = true;
	}

	// Token: 0x06000A22 RID: 2594 RVA: 0x0004D064 File Offset: 0x0004B264
	public void LeaveRoom(string roomName)
	{
		if (this.rooms.Contains(roomName))
		{
			if (this.currentRoom == this.rooms.Get(roomName))
			{
				if (this.rooms.Count > 1)
				{
					if (this.rooms.IndexOf(roomName) == 0)
					{
						this.currentRoom = this.rooms[1];
					}
					else
					{
						this.currentRoom = this.rooms[this.rooms.IndexOf(roomName) - 1];
					}
				}
				else
				{
					this.currentRoom = null;
				}
			}
			this.rooms.Remove(roomName);
		}
	}

	// Token: 0x06000A23 RID: 2595 RVA: 0x000088B1 File Offset: 0x00006AB1
	public Room GetCurrentRoom()
	{
		return this.currentRoom;
	}

	// Token: 0x06000A24 RID: 2596 RVA: 0x0004D10C File Offset: 0x0004B30C
	public void SetRoomsList(RoomsListMessage m)
	{
		this.listedRooms.Clear();
		foreach (RoomsListMessage.RoomWithUsers roomWithUsers in m.rooms)
		{
			ChatRooms.JoinableRoomDesc joinableRoomDesc = new ChatRooms.JoinableRoomDesc();
			joinableRoomDesc.name = roomWithUsers.room.name;
			joinableRoomDesc.autoIncrement = roomWithUsers.room.autoIncrement;
			joinableRoomDesc.numUsers = roomWithUsers.numberOfUsers;
			this.listedRooms.Add(joinableRoomDesc);
		}
	}

	// Token: 0x06000A25 RID: 2597 RVA: 0x000088B9 File Offset: 0x00006AB9
	public List<ChatRooms.JoinableRoomDesc> GetListedRooms()
	{
		return this.listedRooms;
	}

	// Token: 0x06000A26 RID: 2598 RVA: 0x000088C1 File Offset: 0x00006AC1
	public List<Room> GetAllRooms()
	{
		return this.rooms.List();
	}

	// Token: 0x06000A27 RID: 2599 RVA: 0x000088CE File Offset: 0x00006ACE
	public List<string> GetRejoinableRoomNames()
	{
		return this.rooms.GetChatRoomNames();
	}

	// Token: 0x06000A28 RID: 2600 RVA: 0x000088DB File Offset: 0x00006ADB
	public void ChatMessage(RoomChatMessageMessage m)
	{
		this.ChatMessage(m.roomName, m.from, m.text, false);
	}

	// Token: 0x06000A29 RID: 2601 RVA: 0x000088F6 File Offset: 0x00006AF6
	public void ChatMessage(WhisperMessage m)
	{
		this.ChatMessage(m.GetChatroomName(), m.from, m.text, true);
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x0004D184 File Offset: 0x0004B384
	public void ChatMessage(GameChatMessageMessage m, long gameId, bool isReplay)
	{
		string roomName = (!isReplay) ? ChatRooms.GetMatchRoomName(gameId) : ChatRooms.GetReplayRoomName(gameId);
		this.ChatMessage(roomName, m.from, m.text, false);
	}

	// Token: 0x06000A2B RID: 2603 RVA: 0x0004D1C0 File Offset: 0x0004B3C0
	public void ChatMessage(SpectateChatMessageMessage m, long gameId)
	{
		string text = m.text;
		string text2 = m.from;
		if (m.fromPlayer)
		{
			text2 = "[Match] " + text2;
		}
		this.ChatMessage(ChatRooms.GetSpectateRoomName(gameId), text2, text, false);
	}

	// Token: 0x06000A2C RID: 2604 RVA: 0x00008911 File Offset: 0x00006B11
	public void ChatMessage(CliResponseMessage m)
	{
		if (this.currentRoom == null)
		{
			return;
		}
		this.ChatMessage(this.currentRoom.name, "System", m.text, false);
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x0004D204 File Offset: 0x0004B404
	private void ChatMessage(string roomName, string from, string text, bool isWhisper)
	{
		if (App.FriendList.IsBlocked(from))
		{
			return;
		}
		if (!this.rooms.Contains(roomName))
		{
			this.rooms.Add(roomName, (!isWhisper) ? RoomType.ChatRoom : RoomType.WhisperRoom);
			if (isWhisper && from != App.MyProfile.ProfileInfo.name)
			{
				this.ChatHighlight(roomName);
			}
			else
			{
				this.SetCurrentRoom(roomName);
			}
		}
		FeatureType featureTypeInRoom = this.GetFeatureTypeInRoom(from, roomName);
		string time = "<color=#777460>" + DateTime.Now.ToShortTimeString().Trim() + "\t</color>";
		string text2 = string.Empty;
		if (from == "System")
		{
			string text3 = text2;
			text2 = string.Concat(new string[]
			{
				text3,
				"<color=#777460>",
				from,
				": ",
				text,
				"</color>"
			});
		}
		else
		{
			string text4 = (!featureTypeInRoom.isDemo()) ? "<color=#b1ac80>" : "<color=#a59585>";
			bool flag = from == App.MyProfile.ProfileInfo.name;
			if (!flag && text.ToLowerInvariant().Contains(App.MyProfile.ProfileInfo.name.ToLowerInvariant()))
			{
				string text3 = text2;
				text2 = string.Concat(new string[]
				{
					text3,
					text4,
					from,
					": </color><color=#ffcc33ff>",
					text,
					"</color>"
				});
				this.ChatHighlight(roomName);
				if (!App.AudioScript.GetSoundToggle(AudioScript.ESoundToggle.CHAT_HIGHLIGHT))
				{
					this.ChatMessageReceived(roomName);
				}
			}
			else if (flag)
			{
				string text3 = text2;
				text2 = string.Concat(new string[]
				{
					text3,
					"<color=#aa803f>",
					from,
					": </color><color=#eeeac3>",
					text,
					"</color>"
				});
			}
			else
			{
				string text3 = text2;
				text2 = string.Concat(new string[]
				{
					text3,
					text4,
					from,
					": </color><color=#eeeac3>",
					text,
					"</color>"
				});
				this.ChatMessageReceived(roomName);
			}
		}
		this.rooms.Get(roomName).log.AddLine(new RoomLog.ChatLine(time, text2, from, this.GetUserAdminRoleInRoom(from, roomName), featureTypeInRoom));
		if (roomName == this.GetCurrentRoomName())
		{
			this.rooms.Get(roomName).log.allRead = true;
		}
		else
		{
			this.rooms.Get(roomName).log.allRead = false;
		}
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x0000893C File Offset: 0x00006B3C
	public string GetCurrentRoomName()
	{
		return (this.currentRoom != null) ? this.currentRoom.name : null;
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x0000895A File Offset: 0x00006B5A
	public RoomLog GetCurrentRoomChatLog()
	{
		return (this.currentRoom != null) ? this.currentRoom.log : null;
	}

	// Token: 0x06000A30 RID: 2608 RVA: 0x00008978 File Offset: 0x00006B78
	public List<ChatUser> GetCurrentRoomUsers()
	{
		return (this.currentRoom != null) ? this.currentRoom.users : null;
	}

	// Token: 0x06000A31 RID: 2609 RVA: 0x0004D4A4 File Offset: 0x0004B6A4
	public void SetRoomInfo(RoomInfoMessage m)
	{
		Room room = this.rooms.Get(m.roomName);
		room.RoomInfo(m);
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x00008996 File Offset: 0x00006B96
	private List<ChatUser> GetUsersInRoom(string roomName)
	{
		if (this.rooms.Contains(roomName))
		{
			return this.rooms.Get(roomName).users;
		}
		return null;
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x0004D4CC File Offset: 0x0004B6CC
	private AdminRole GetUserAdminRoleInRoom(string name, string roomName)
	{
		Room room = this.rooms.Get(roomName);
		if (room != null)
		{
			foreach (ChatUser chatUser in room.users)
			{
				if (chatUser.name == name)
				{
					return chatUser.adminRole;
				}
			}
			return AdminRole.None;
		}
		return AdminRole.None;
	}

	// Token: 0x06000A34 RID: 2612 RVA: 0x0004D554 File Offset: 0x0004B754
	public ChatUser GetUserInRoom(string name, string roomName)
	{
		Room room = this.rooms.Get(roomName);
		if (room == null)
		{
			return null;
		}
		if (room != null)
		{
			foreach (ChatUser chatUser in room.users)
			{
				if (chatUser.name == name)
				{
					return chatUser;
				}
			}
		}
		return null;
	}

	// Token: 0x06000A35 RID: 2613 RVA: 0x0004D5E0 File Offset: 0x0004B7E0
	private FeatureType GetFeatureTypeInRoom(string name, string roomName)
	{
		ChatUser userInRoom = this.GetUserInRoom(name, roomName);
		return (userInRoom == null) ? FeatureType.PREMIUM : userInRoom.featureType;
	}

	// Token: 0x06000A36 RID: 2614 RVA: 0x000089BC File Offset: 0x00006BBC
	public static string GetMatchRoomName(long gameId)
	{
		return ChatRooms.MatchRoomPrefix + gameId;
	}

	// Token: 0x06000A37 RID: 2615 RVA: 0x000089CE File Offset: 0x00006BCE
	public static string GetSpectateRoomName(long gameId)
	{
		return ChatRooms.SpectateRoomPrefix + gameId;
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x000089E0 File Offset: 0x00006BE0
	public static string GetReplayRoomName(long gameId)
	{
		return ChatRooms.ReplayRoomPrefix + gameId;
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x000089F2 File Offset: 0x00006BF2
	public List<Room> FilterPersistentRooms()
	{
		return ChatRooms.FilterPersistentRooms(this.GetAllRooms());
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x0004D608 File Offset: 0x0004B808
	public static List<Room> FilterPersistentRooms(List<Room> rooms)
	{
		List<Room> list = new List<Room>();
		foreach (Room room in rooms)
		{
			if (room.type == RoomType.ChatRoom)
			{
				string text = room.name.ToLower();
				if (!text.StartsWith("general-"))
				{
					if (!text.StartsWith(ChatRooms.MatchRoomPrefix) && !text.StartsWith(ChatRooms.SpectateRoomPrefix))
					{
						list.Add(room);
					}
				}
			}
		}
		return list;
	}

	// Token: 0x040007BD RID: 1981
	private const string colSystem = "<color=#777460>";

	// Token: 0x040007BE RID: 1982
	private const string colPremium = "<color=#b1ac80>";

	// Token: 0x040007BF RID: 1983
	private const string colDemo = "<color=#a59585>";

	// Token: 0x040007C0 RID: 1984
	private const string colText = "<color=#eeeac3>";

	// Token: 0x040007C1 RID: 1985
	private const string colHighlight = "<color=#ffcc33ff>";

	// Token: 0x040007C2 RID: 1986
	private const string colYou = "<color=#aa803f>";

	// Token: 0x040007C3 RID: 1987
	private const string colEnd = "</color>";

	// Token: 0x040007C4 RID: 1988
	private List<ChatRooms.JoinableRoomDesc> listedRooms = new List<ChatRooms.JoinableRoomDesc>();

	// Token: 0x040007C5 RID: 1989
	private SortedRoomSet rooms = new SortedRoomSet();

	// Token: 0x040007C6 RID: 1990
	private Room currentRoom;

	// Token: 0x040007C7 RID: 1991
	private static string MatchRoomPrefix = "match-";

	// Token: 0x040007C8 RID: 1992
	private static string SpectateRoomPrefix = "spec-";

	// Token: 0x040007C9 RID: 1993
	private static string ReplayRoomPrefix = "replay-";

	// Token: 0x02000139 RID: 313
	public class JoinableRoomDesc
	{
		// Token: 0x040007CC RID: 1996
		public string name;

		// Token: 0x040007CD RID: 1997
		public bool autoIncrement;

		// Token: 0x040007CE RID: 1998
		public int numUsers;
	}

	// Token: 0x0200013A RID: 314
	// (Invoke) Token: 0x06000A3D RID: 2621
	public delegate void ChatRoomNotification(string roomName);
}
