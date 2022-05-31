using System;

// Token: 0x02000347 RID: 839
public class RoomsListMessage : Message
{
	// Token: 0x0400109D RID: 4253
	public RoomsListMessage.RoomWithUsers[] rooms;

	// Token: 0x02000348 RID: 840
	public class RoomWithUsers
	{
		// Token: 0x0400109E RID: 4254
		public RoomsListMessage.Room room;

		// Token: 0x0400109F RID: 4255
		public int numberOfUsers;
	}

	// Token: 0x02000349 RID: 841
	public class Room
	{
		// Token: 0x040010A0 RID: 4256
		public string name;

		// Token: 0x040010A1 RID: 4257
		public bool autoIncrement;
	}
}
