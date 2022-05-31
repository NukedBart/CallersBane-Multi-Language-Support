using System;
using System.Text.RegularExpressions;

// Token: 0x02000136 RID: 310
public class ArenaChat : AbstractCommListener
{
	// Token: 0x06000A08 RID: 2568 RVA: 0x0000391A File Offset: 0x00001B1A
	private void Start()
	{
		App.Communicator.addListener(this);
	}

	// Token: 0x17000086 RID: 134
	// (get) Token: 0x06000A09 RID: 2569 RVA: 0x0000864A File Offset: 0x0000684A
	public ChatRooms ChatRooms
	{
		get
		{
			return this.chatRooms;
		}
	}

	// Token: 0x06000A0A RID: 2570 RVA: 0x0004CD74 File Offset: 0x0004AF74
	public string AdjustIfReplyMessage(string message)
	{
		string text = message.ToLower().Trim();
		if (text.StartsWith("/resetsystempassword"))
		{
			Group group = StringUtil.removeWords(message, 1);
			if (group != null)
			{
				message = message.Substring(0, group.Index) + LoginMessage.hashPassword(group.Value);
			}
		}
		if (text.StartsWith("/resetpassword"))
		{
			Group group2 = StringUtil.removeWords(message, 2);
			if (group2 != null)
			{
				message = message.Substring(0, group2.Index) + LoginMessage.hashPassword(group2.Value);
			}
		}
		if ((message.StartsWith("/r ") || message.StartsWith("/R ")) && this.lastWhisperFrom != null)
		{
			return "/w " + this.lastWhisperFrom + " " + message.Substring(3);
		}
		return message;
	}

	// Token: 0x06000A0B RID: 2571 RVA: 0x0004CE50 File Offset: 0x0004B050
	public override void handleMessage(Message msg)
	{
		if (msg is RoomEnterMessage)
		{
			this.chatRooms.SetCurrentRoom(((RoomEnterMessage)msg).roomName);
		}
		if (msg is RoomEnterMultiMessage)
		{
		}
		if (msg is RoomChatMessageMessage)
		{
			this.chatRooms.ChatMessage((RoomChatMessageMessage)msg);
		}
		if (msg is WhisperMessage)
		{
			WhisperMessage whisperMessage = (WhisperMessage)msg;
			if (whisperMessage.from != App.MyProfile.ProfileInfo.name)
			{
				this.lastWhisperFrom = whisperMessage.from;
			}
			this.chatRooms.ChatMessage(whisperMessage);
		}
		if (msg is CliResponseMessage)
		{
			this.chatRooms.ChatMessage((CliResponseMessage)msg);
		}
		if (msg is RoomInfoMessage)
		{
			this.chatRooms.SetRoomInfo((RoomInfoMessage)msg);
		}
		if (msg is RoomsListMessage)
		{
			this.chatRooms.SetRoomsList((RoomsListMessage)msg);
		}
	}

	// Token: 0x06000A0C RID: 2572 RVA: 0x0004CF44 File Offset: 0x0004B144
	public void InitiateOrRejoinAllActive()
	{
		if (this.chatRooms.GetRejoinableRoomNames().Count == 0)
		{
			this.RoomEnterFree("General");
		}
		else
		{
			App.Communicator.send(new RoomEnterMultiMessage(this.chatRooms.GetRejoinableRoomNames()));
		}
	}

	// Token: 0x06000A0D RID: 2573 RVA: 0x00008652 File Offset: 0x00006852
	public void GetRoomsList()
	{
		App.Communicator.send(new RoomsListMessage());
	}

	// Token: 0x06000A0E RID: 2574 RVA: 0x00008664 File Offset: 0x00006864
	public void RoomEnter(string roomName)
	{
		App.Communicator.send(new RoomEnterMessage(roomName));
	}

	// Token: 0x06000A0F RID: 2575 RVA: 0x00008677 File Offset: 0x00006877
	public void RoomEnterFree(string roomName)
	{
		App.Communicator.send(new RoomEnterFreeMessage(roomName));
	}

	// Token: 0x06000A10 RID: 2576 RVA: 0x0000868A File Offset: 0x0000688A
	public void RoomExit(Room room, bool retainHistory)
	{
		if (!retainHistory)
		{
			this.chatRooms.LeaveRoom(room.name);
		}
		if (room.type == RoomType.ChatRoom)
		{
			App.Communicator.send(new RoomExitMessage(room.name));
		}
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x000086C4 File Offset: 0x000068C4
	public void OpenWhisperRoom(string username)
	{
		this.chatRooms.OpenWhisperRoom(username);
	}

	// Token: 0x040007B8 RID: 1976
	private ChatRooms chatRooms = new ChatRooms();

	// Token: 0x040007B9 RID: 1977
	private string lastWhisperFrom;
}
