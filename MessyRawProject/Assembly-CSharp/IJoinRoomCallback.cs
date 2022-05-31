using System;

// Token: 0x02000388 RID: 904
public interface IJoinRoomCallback : ICancelCallback
{
	// Token: 0x06001404 RID: 5124
	void PopupJoinRoom(string popupType, string roomName, bool autoIncrement);
}
