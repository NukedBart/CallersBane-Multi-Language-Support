using System;
using System.Collections.Generic;

// Token: 0x020003B0 RID: 944
public class Room : IComparable
{
	// Token: 0x06001524 RID: 5412 RVA: 0x000829C8 File Offset: 0x00080BC8
	public Room(string name, RoomType type)
	{
		this.name = name;
		this.lowerName = name.ToLower();
		this.type = type;
		this.timestamp = DateTime.Now.Ticks;
	}

	// Token: 0x06001525 RID: 5413 RVA: 0x0000F7B7 File Offset: 0x0000D9B7
	public void RoomInfo(RoomInfoMessage m)
	{
		if (m.reset)
		{
			this.users = new List<ChatUser>();
		}
		this.UsersUpdated(m.updated);
		this.UsersRemoved(m.removed);
	}

	// Token: 0x06001526 RID: 5414 RVA: 0x00082A20 File Offset: 0x00080C20
	public int CompareTo(object obj)
	{
		Room room = obj as Room;
		if (this.type == RoomType.ChatRoom && room.type != RoomType.ChatRoom)
		{
			return 1;
		}
		if (this.type != RoomType.ChatRoom && room.type == RoomType.ChatRoom)
		{
			return -1;
		}
		return (int)(room.timestamp - this.timestamp);
	}

	// Token: 0x06001527 RID: 5415 RVA: 0x00082A74 File Offset: 0x00080C74
	private void UsersUpdated(RoomInfoProfile[] profiles)
	{
		foreach (RoomInfoProfile roomInfoProfile in profiles)
		{
			ChatUser chatUser = ChatUser.FromRoomInfoProfile(roomInfoProfile);
			bool flag = false;
			for (int j = 0; j < this.users.Count; j++)
			{
				if (this.users[j].name == roomInfoProfile.name)
				{
					this.users[j] = chatUser;
					flag = true;
				}
			}
			if (!flag)
			{
				this.users.Add(chatUser);
			}
		}
		this.users.Sort();
	}

	// Token: 0x06001528 RID: 5416 RVA: 0x00082B18 File Offset: 0x00080D18
	private void UsersRemoved(RoomInfoProfile[] profiles)
	{
		foreach (RoomInfoProfile roomInfoProfile in profiles)
		{
			for (int j = 0; j < this.users.Count; j++)
			{
				if (this.users[j].name == roomInfoProfile.name)
				{
					this.users.RemoveAt(j);
					break;
				}
			}
		}
	}

	// Token: 0x04001259 RID: 4697
	public RoomType type;

	// Token: 0x0400125A RID: 4698
	public string name;

	// Token: 0x0400125B RID: 4699
	public string lowerName;

	// Token: 0x0400125C RID: 4700
	public List<ChatUser> users = new List<ChatUser>();

	// Token: 0x0400125D RID: 4701
	public RoomLog log = new RoomLog();

	// Token: 0x0400125E RID: 4702
	private long timestamp;
}
