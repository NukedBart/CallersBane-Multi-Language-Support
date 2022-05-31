using System;

// Token: 0x020003B1 RID: 945
public class ChatUser : IComparable
{
	// Token: 0x0600152A RID: 5418 RVA: 0x00082B90 File Offset: 0x00080D90
	public static ChatUser FromPerson(Person p)
	{
		return new ChatUser
		{
			name = p.profile.name,
			profileId = p.profile.id,
			acceptChallenges = true,
			acceptTrades = true,
			adminRole = p.profile.adminRole
		};
	}

	// Token: 0x0600152B RID: 5419 RVA: 0x00082BE8 File Offset: 0x00080DE8
	public static ChatUser FromRoomInfoProfile(RoomInfoProfile p)
	{
		return new ChatUser
		{
			name = p.name,
			profileId = p.profileId,
			acceptChallenges = p.acceptChallenges,
			acceptTrades = p.acceptTrades,
			adminRole = p.adminRole,
			featureType = p.featureType
		};
	}

	// Token: 0x0600152C RID: 5420 RVA: 0x00082C44 File Offset: 0x00080E44
	public int CompareTo(object obj)
	{
		ChatUser chatUser = obj as ChatUser;
		return this.name.CompareTo(chatUser.name);
	}

	// Token: 0x0400125F RID: 4703
	public string name;

	// Token: 0x04001260 RID: 4704
	public int profileId;

	// Token: 0x04001261 RID: 4705
	public AdminRole adminRole;

	// Token: 0x04001262 RID: 4706
	public bool acceptChallenges;

	// Token: 0x04001263 RID: 4707
	public bool acceptTrades;

	// Token: 0x04001264 RID: 4708
	public FeatureType featureType;
}
