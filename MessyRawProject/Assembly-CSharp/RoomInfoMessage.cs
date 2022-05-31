using System;

// Token: 0x02000344 RID: 836
public class RoomInfoMessage : RoomMessage
{
	// Token: 0x04001093 RID: 4243
	public bool reset;

	// Token: 0x04001094 RID: 4244
	public RoomInfoProfile[] updated = new RoomInfoProfile[0];

	// Token: 0x04001095 RID: 4245
	public RoomInfoProfile[] removed = new RoomInfoProfile[0];
}
