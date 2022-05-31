using System;
using System.Collections.Generic;

// Token: 0x0200027C RID: 636
public class GameInfoMessage : Message
{
	// Token: 0x06001216 RID: 4630 RVA: 0x0000DB20 File Offset: 0x0000BD20
	public string getPlayerName(TileColor color)
	{
		return (!color.isWhite()) ? this.black : this.white;
	}

	// Token: 0x06001217 RID: 4631 RVA: 0x0000DB3E File Offset: 0x0000BD3E
	public int getPlayerProfileId(TileColor color)
	{
		return (!color.isWhite()) ? this.blackAvatar.profileId : this.whiteAvatar.profileId;
	}

	// Token: 0x06001218 RID: 4632 RVA: 0x0000DB66 File Offset: 0x0000BD66
	public AvatarInfo getAvatar(TileColor color)
	{
		return (!color.isWhite()) ? this.blackAvatar.getAvatarInfo() : this.whiteAvatar.getAvatarInfo();
	}

	// Token: 0x06001219 RID: 4633 RVA: 0x00077930 File Offset: 0x00075B30
	public bool isSpectate()
	{
		string text = App.MyProfile.ProfileInfo.name.ToLower();
		return this.getPlayerName(this.color).ToLower() != text;
	}

	// Token: 0x0600121A RID: 4634 RVA: 0x0000DB8E File Offset: 0x0000BD8E
	public bool hasTimer()
	{
		return this.roundTimerSeconds >= 1 && this.roundTimerSeconds <= 1000000;
	}

	// Token: 0x0600121B RID: 4635 RVA: 0x0007796C File Offset: 0x00075B6C
	private static IdolInfo[] getDefaultIdols(TileColor color)
	{
		List<IdolInfo> list = new List<IdolInfo>();
		for (int i = 0; i < 5; i++)
		{
			IdolInfo idolInfo = new IdolInfo();
			idolInfo.color = color;
			idolInfo.position = i;
			idolInfo.hp = (idolInfo.maxHp = 10);
			list.Add(idolInfo);
		}
		return list.ToArray();
	}

	// Token: 0x0600121C RID: 4636 RVA: 0x0000DBAF File Offset: 0x0000BDAF
	public IdolInfo getIdolInfo(TileColor color, int row)
	{
		return (!color.isWhite()) ? this.blackIdols[row] : this.whiteIdols[row];
	}

	// Token: 0x0600121D RID: 4637 RVA: 0x000779C4 File Offset: 0x00075BC4
	public short getIdolType(TileColor color, int row)
	{
		IdolTypeDeserializer idolTypeDeserializer = (!color.isWhite()) ? this.blackIdolTypes : this.whiteIdolTypes;
		return (idolTypeDeserializer != null) ? idolTypeDeserializer.getIdolType(row) : -1;
	}

	// Token: 0x0600121E RID: 4638 RVA: 0x00077A04 File Offset: 0x00075C04
	public bool hasRule(string rule)
	{
		if (this.customSettings == null)
		{
			return false;
		}
		rule = rule.ToLower();
		foreach (string text in this.customSettings)
		{
			if (text.ToLower().Equals(rule))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04000EC5 RID: 3781
	public string white;

	// Token: 0x04000EC6 RID: 3782
	public string black;

	// Token: 0x04000EC7 RID: 3783
	[ServerToClient]
	public GameType gameType;

	// Token: 0x04000EC8 RID: 3784
	[ServerToClient]
	public long gameId;

	// Token: 0x04000EC9 RID: 3785
	[ServerToClient]
	public int port;

	// Token: 0x04000ECA RID: 3786
	[ServerToClient]
	public string nodeId;

	// Token: 0x04000ECB RID: 3787
	public string background;

	// Token: 0x04000ECC RID: 3788
	[ServerToClient]
	public int refId;

	// Token: 0x04000ECD RID: 3789
	[ServerToClient]
	public TileColor color;

	// Token: 0x04000ECE RID: 3790
	public AvatarInfoDeserializer whiteAvatar;

	// Token: 0x04000ECF RID: 3791
	public AvatarInfoDeserializer blackAvatar;

	// Token: 0x04000ED0 RID: 3792
	[ServerToClient]
	public int roundTimerSeconds;

	// Token: 0x04000ED1 RID: 3793
	[ServerToClient]
	public EndPhaseMessage.Phase phase;

	// Token: 0x04000ED2 RID: 3794
	[ServerToClient]
	public int rewardForIdolKill;

	// Token: 0x04000ED3 RID: 3795
	[ServerToClient]
	public IdolInfo[] whiteIdols = GameInfoMessage.getDefaultIdols(TileColor.white);

	// Token: 0x04000ED4 RID: 3796
	[ServerToClient]
	public IdolInfo[] blackIdols = GameInfoMessage.getDefaultIdols(TileColor.black);

	// Token: 0x04000ED5 RID: 3797
	public IdolTypeDeserializer whiteIdolTypes;

	// Token: 0x04000ED6 RID: 3798
	public IdolTypeDeserializer blackIdolTypes;

	// Token: 0x04000ED7 RID: 3799
	public List<string> customSettings = new List<string>();

	// Token: 0x04000ED8 RID: 3800
	[ServerToClient]
	public float maxTierRewardMultiplier;

	// Token: 0x04000ED9 RID: 3801
	[ServerToClient]
	public float[] tierRewardMultiplierDelta;
}
