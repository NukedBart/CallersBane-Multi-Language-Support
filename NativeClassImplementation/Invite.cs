using System;
using UnityEngine;

// Token: 0x020001F0 RID: 496
public class Invite : MonoBehaviour
{
	// Token: 0x06000F95 RID: 3989 RVA: 0x000028DF File Offset: 0x00000ADF
	private void Start()
	{
	}

	// Token: 0x06000F96 RID: 3990 RVA: 0x0000C794 File Offset: 0x0000A994
	public void init(string label, string fullText, Invite.InviteType type)
	{
		this.inviteLabel = label;
		this.fullText = fullText;
		this.inviteType = type;
		this.timeOut = -1f;
		this.init(null);
	}

	// Token: 0x06000F97 RID: 3991 RVA: 0x00067004 File Offset: 0x00065204
	public void init(Message msg)
	{
		this.message = msg;
		if (msg != null)
		{
			if (msg is GameChallengeMessage)
			{
				this.inviterInfo = new InviterInfo(((GameChallengeMessage)msg).from);
				this.inviteLabel = "Challenge!";
			}
			else if (msg is GameMatchMessage)
			{
				this.inviterInfo = null;
				this.gameType = ((GameMatchMessage)msg).gameType;
				this.inviteLabel = "Match Found!";
				this.timeOut = 19f;
			}
			else if (msg is GameMatchCancelledMessage)
			{
				this.inviteLabel = "Match Cancelled";
			}
			else if (msg is GameChallengeResponseMessage && ((GameChallengeResponseMessage)msg).status == GameChallengeResponseMessage.Status.DECLINE)
			{
				this.inviterInfo = new InviterInfo(((GameChallengeResponseMessage)msg).to);
				this.inviteLabel = "Challenge Declined!";
			}
			else if (msg is TradeInviteForwardMessage)
			{
				this.inviterInfo = new InviterInfo(((TradeInviteForwardMessage)msg).inviter);
				this.inviteLabel = "Trade Invite!";
				Log.warning("Trade Invite! " + this.inviterInfo.from.name);
			}
			else if (msg is TradeResponseMessage && ((TradeResponseMessage)msg).status == "DECLINE")
			{
				this.inviterInfo = new InviterInfo(((TradeResponseMessage)msg).to);
				this.inviteLabel = "Trade Declined!";
			}
			else if (msg is StringNotificationMessage)
			{
				this.inviteLabel = ((StringNotificationMessage)msg).header;
				this.fullText = ((StringNotificationMessage)msg).message;
			}
			else if (msg is AchievementUnlockedMessage)
			{
				AchievementUnlockedMessage achievementUnlockedMessage = (AchievementUnlockedMessage)msg;
				this.inviteLabel = "Achievement earned";
				AchievementType achievementType = AchievementTypeManager.getInstance().get(achievementUnlockedMessage.typeId);
				if (achievementType != null)
				{
					if (achievementType.goldReward > 0)
					{
						this.inviteLabel = this.inviteLabel + " - " + GUIUtil.RtColor(achievementType.goldReward + "g", Color.yellow);
					}
					this.inviteLabel = this.inviteLabel + "\n" + achievementType.name;
				}
				else
				{
					this.inviteLabel = this.inviteLabel + "\nID: " + achievementUnlockedMessage.typeId;
				}
				this.textColor = ColorUtil.FromHex24(14802100u);
			}
		}
		this.inviteActive = false;
		this.startTime = Time.time;
		base.transform.position = new Vector3((float)Screen.width, (float)Screen.height * 0.14f, 0f);
	}

	// Token: 0x04000C17 RID: 3095
	public Rect size;

	// Token: 0x04000C18 RID: 3096
	public float startTime;

	// Token: 0x04000C19 RID: 3097
	public string inviteLabel;

	// Token: 0x04000C1A RID: 3098
	public string fullText;

	// Token: 0x04000C1B RID: 3099
	public Invite.InviteType inviteType;

	// Token: 0x04000C1C RID: 3100
	public Message message;

	// Token: 0x04000C1D RID: 3101
	public InviterInfo inviterInfo;

	// Token: 0x04000C1E RID: 3102
	public GameType gameType;

	// Token: 0x04000C1F RID: 3103
	public bool inviteActive;

	// Token: 0x04000C20 RID: 3104
	public GameObject gO;

	// Token: 0x04000C21 RID: 3105
	public float timeOut = 120f;

	// Token: 0x04000C22 RID: 3106
	public Color textColor = Color.white;

	// Token: 0x020001F1 RID: 497
	public enum InviteType
	{
		// Token: 0x04000C24 RID: 3108
		DEFAULT,
		// Token: 0x04000C25 RID: 3109
		FRIEND_NEW_REQUEST,
		// Token: 0x04000C26 RID: 3110
		SOLD_MARKET_SCROLLS
	}
}
