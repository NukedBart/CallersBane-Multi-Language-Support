using System;

// Token: 0x020002D9 RID: 729
public class Person : IComparable
{
	// Token: 0x060012CC RID: 4812 RVA: 0x0000E1AB File Offset: 0x0000C3AB
	public bool IsSameUser(Person other)
	{
		return this.profile.id == other.profile.id;
	}

	// Token: 0x060012CD RID: 4813 RVA: 0x000782C8 File Offset: 0x000764C8
	public int CompareTo(object other)
	{
		if (!(other is Person))
		{
			return 0;
		}
		Person person = (Person)other;
		if (this.online() && !person.online())
		{
			return -1;
		}
		if (!this.online() && person.online())
		{
			return 1;
		}
		return this.profile.name.CompareTo(person.profile.name);
	}

	// Token: 0x060012CE RID: 4814 RVA: 0x00078334 File Offset: 0x00076534
	public bool online()
	{
		OnlineState? onlineState = this.onlineState;
		return onlineState != null;
	}

	// Token: 0x060012CF RID: 4815 RVA: 0x00078350 File Offset: 0x00076550
	public bool isInLobby()
	{
		return this.onlineState == OnlineState.LOBBY;
	}

	// Token: 0x060012D0 RID: 4816 RVA: 0x00078378 File Offset: 0x00076578
	public bool isPlaying()
	{
		return this.onlineState == OnlineState.GAME;
	}

	// Token: 0x060012D1 RID: 4817 RVA: 0x000783A0 File Offset: 0x000765A0
	public bool isSpectating()
	{
		return this.onlineState == OnlineState.SPECTATE;
	}

	// Token: 0x060012D2 RID: 4818 RVA: 0x0000E1C5 File Offset: 0x0000C3C5
	public string prefixString()
	{
		return this.onlineState.prefixString();
	}

	// Token: 0x04000FA0 RID: 4000
	public ProfileInfo profile;

	// Token: 0x04000FA1 RID: 4001
	public OnlineState? onlineState;
}
