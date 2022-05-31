using System;

// Token: 0x02000339 RID: 825
public class ProfilePageInfoMessage : Message
{
	// Token: 0x06001375 RID: 4981 RVA: 0x0000E744 File Offset: 0x0000C944
	public ProfilePageInfoMessage()
	{
	}

	// Token: 0x06001376 RID: 4982 RVA: 0x0000E763 File Offset: 0x0000C963
	public ProfilePageInfoMessage(int profileId)
	{
		this.profileId = new int?(profileId);
	}

	// Token: 0x06001377 RID: 4983 RVA: 0x0000E78E File Offset: 0x0000C98E
	public int getScrollsTotal()
	{
		return this.scrollsCommon + this.scrollsUncommon + this.scrollsRare;
	}

	// Token: 0x06001378 RID: 4984 RVA: 0x0000E7A4 File Offset: 0x0000C9A4
	public AvatarInfo getAvatar()
	{
		return this.avatar.getAvatarInfo();
	}

	// Token: 0x0400106E RID: 4206
	public int? profileId;

	// Token: 0x0400106F RID: 4207
	[ServerToClient]
	public string name = string.Empty;

	// Token: 0x04001070 RID: 4208
	[ServerToClient]
	public int gold;

	// Token: 0x04001071 RID: 4209
	[ServerToClient]
	public int scrollsCommon;

	// Token: 0x04001072 RID: 4210
	[ServerToClient]
	public int scrollsUncommon;

	// Token: 0x04001073 RID: 4211
	[ServerToClient]
	public int scrollsRare;

	// Token: 0x04001074 RID: 4212
	[ServerToClient]
	public int uniqueTypes;

	// Token: 0x04001075 RID: 4213
	[ServerToClient]
	public int rating;

	// Token: 0x04001076 RID: 4214
	[ServerToClient]
	public int ranking;

	// Token: 0x04001077 RID: 4215
	[ServerToClient]
	public int rank;

	// Token: 0x04001078 RID: 4216
	[ServerToClient]
	public int winsForRank;

	// Token: 0x04001079 RID: 4217
	[ServerToClient]
	public int gamesPlayed;

	// Token: 0x0400107A RID: 4218
	[ServerToClient]
	public int gamesWon;

	// Token: 0x0400107B RID: 4219
	[ServerToClient]
	public int rankedWon;

	// Token: 0x0400107C RID: 4220
	[ServerToClient]
	public int limitedWon;

	// Token: 0x0400107D RID: 4221
	public int[] unlockedAvatarTypes;

	// Token: 0x0400107E RID: 4222
	public short[] unlockedAchievementTypes;

	// Token: 0x0400107F RID: 4223
	public short[] unlockedIdolTypes = new short[0];

	// Token: 0x04001080 RID: 4224
	public IdolTypeDeserializer idols;

	// Token: 0x04001081 RID: 4225
	public string lastGamePlayed;

	// Token: 0x04001082 RID: 4226
	public AvatarInfoDeserializer avatar;
}
