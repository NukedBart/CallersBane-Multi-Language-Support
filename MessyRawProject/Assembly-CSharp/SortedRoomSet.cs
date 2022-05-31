using System;
using System.Collections.Generic;

// Token: 0x020003B5 RID: 949
internal class SortedRoomSet
{
	// Token: 0x06001532 RID: 5426 RVA: 0x0000F84B File Offset: 0x0000DA4B
	public bool Contains(string roomName)
	{
		return this.IndexOf(roomName) >= 0;
	}

	// Token: 0x06001533 RID: 5427 RVA: 0x00082CC8 File Offset: 0x00080EC8
	public Room Get(string roomName)
	{
		int num = this.IndexOf(roomName);
		if (num >= 0)
		{
			return this.rooms[num];
		}
		return null;
	}

	// Token: 0x06001534 RID: 5428 RVA: 0x0000F85A File Offset: 0x0000DA5A
	public void Add(string roomName, RoomType type)
	{
		if (!this.Contains(roomName))
		{
			this.rooms.Add(new Room(roomName, type));
		}
		this.rooms.Sort();
	}

	// Token: 0x06001535 RID: 5429 RVA: 0x00082CF4 File Offset: 0x00080EF4
	public void Remove(string roomName)
	{
		int num = this.IndexOf(roomName);
		if (num >= 0)
		{
			this.rooms.RemoveAt(num);
		}
	}

	// Token: 0x06001536 RID: 5430 RVA: 0x00082D1C File Offset: 0x00080F1C
	public int IndexOf(string roomName)
	{
		roomName = roomName.ToLower();
		for (int i = 0; i < this.rooms.Count; i++)
		{
			if (this.rooms[i].lowerName == roomName)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x170000F8 RID: 248
	public Room this[int i]
	{
		get
		{
			return this.rooms[i];
		}
		set
		{
			this.rooms[i] = value;
		}
	}

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x06001539 RID: 5433 RVA: 0x0000F8A2 File Offset: 0x0000DAA2
	public int Count
	{
		get
		{
			return this.rooms.Count;
		}
	}

	// Token: 0x0600153A RID: 5434 RVA: 0x0000F8AF File Offset: 0x0000DAAF
	public List<Room> List()
	{
		return this.rooms;
	}

	// Token: 0x0600153B RID: 5435 RVA: 0x00082D6C File Offset: 0x00080F6C
	public List<string> GetChatRoomNames()
	{
		List<string> list = new List<string>();
		foreach (Room room in this.rooms)
		{
			if (room.type == RoomType.ChatRoom)
			{
				list.Add(room.name);
			}
		}
		return list;
	}

	// Token: 0x04001271 RID: 4721
	private List<Room> rooms = new List<Room>();
}
