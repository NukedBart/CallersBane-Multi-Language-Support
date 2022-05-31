using System;
using System.Collections.Generic;

// Token: 0x020001C1 RID: 449
public class FriendList : AbstractCommListener
{
	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x06000E0C RID: 3596 RVA: 0x0000B329 File Offset: 0x00009529
	public List<Person> Friends
	{
		get
		{
			return this.friends;
		}
	}

	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x06000E0D RID: 3597 RVA: 0x0000B331 File Offset: 0x00009531
	public List<string> Blocked
	{
		get
		{
			return this.blocked;
		}
	}

	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x06000E0E RID: 3598 RVA: 0x0000B339 File Offset: 0x00009539
	public List<Request> Requests
	{
		get
		{
			return this.requests;
		}
	}

	// Token: 0x06000E0F RID: 3599 RVA: 0x00060114 File Offset: 0x0005E314
	public int IncomingRequestCount()
	{
		int num = 0;
		foreach (Request request in this.requests)
		{
			if (request.from.profile.id != App.MyProfile.ProfileInfo.id)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06000E10 RID: 3600 RVA: 0x00060194 File Offset: 0x0005E394
	public int OnlineFriendCount()
	{
		int num = 0;
		foreach (Person person in this.friends)
		{
			if (person.online())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06000E11 RID: 3601 RVA: 0x0000391A File Offset: 0x00001B1A
	private void Start()
	{
		App.Communicator.addListener(this);
	}

	// Token: 0x06000E12 RID: 3602 RVA: 0x000601FC File Offset: 0x0005E3FC
	public override void handleMessage(Message msg)
	{
		if (msg is GetFriendsMessage)
		{
			GetFriendsMessage getFriendsMessage = (GetFriendsMessage)msg;
			this.UpdateFriends(getFriendsMessage.friends);
		}
		if (msg is GetBlockedPersonsMessage)
		{
			GetBlockedPersonsMessage getBlockedPersonsMessage = (GetBlockedPersonsMessage)msg;
			this.blocked.Clear();
			foreach (string text in getBlockedPersonsMessage.blocked)
			{
				this.blocked.Add(text);
			}
		}
		if (msg is FriendUpdateMessage)
		{
			FriendUpdateMessage friendUpdateMessage = (FriendUpdateMessage)msg;
			App.LobbyMenu.FriendUpdated(friendUpdateMessage.friend.profile.name);
			this.UpdateFriends(new Person[]
			{
				friendUpdateMessage.friend
			});
		}
		if (msg is GetFriendRequestsMessage)
		{
			GetFriendRequestsMessage getFriendRequestsMessage = (GetFriendRequestsMessage)msg;
			this.UpdateRequests(getFriendRequestsMessage.requests);
		}
		if (msg is RemoveFriendMessage)
		{
			RemoveFriendMessage removeFriendMessage = (RemoveFriendMessage)msg;
			for (int j = this.friends.Count - 1; j >= 0; j--)
			{
				if (this.friends[j].profile.name == removeFriendMessage.profileName)
				{
					this.friends.RemoveAt(j);
					break;
				}
			}
		}
		if (msg is FriendRequestUpdateMessage)
		{
			FriendRequestUpdateMessage friendRequestUpdateMessage = (FriendRequestUpdateMessage)msg;
			if (friendRequestUpdateMessage.request.request.status == FriendStatus.PENDING && friendRequestUpdateMessage.request.from.profile.id != App.MyProfile.ProfileInfo.id)
			{
				App.InviteManager.addInvite("Friend request from " + friendRequestUpdateMessage.request.from.profile.name, friendRequestUpdateMessage.request.from.profile.name + " wants to be friends. Click the friends icon to accept or decline.", Invite.InviteType.FRIEND_NEW_REQUEST);
			}
			this.UpdateRequests(new Request[]
			{
				friendRequestUpdateMessage.request
			});
		}
	}

	// Token: 0x06000E13 RID: 3603 RVA: 0x0000B341 File Offset: 0x00009541
	public void AcceptRequest(Request fr)
	{
		App.Communicator.send(new AcceptFriendRequestMessage(fr.request.id));
		if (this.requests.Contains(fr))
		{
			this.requests.Remove(fr);
		}
	}

	// Token: 0x06000E14 RID: 3604 RVA: 0x0000B37C File Offset: 0x0000957C
	public void DeclineRequest(Request fr)
	{
		App.Communicator.send(new DeclineFriendRequestMessage(fr.request.id));
		if (this.requests.Contains(fr))
		{
			this.requests.Remove(fr);
		}
	}

	// Token: 0x06000E15 RID: 3605 RVA: 0x0000B3B7 File Offset: 0x000095B7
	public void AddFriend(string username)
	{
		App.Communicator.send(new SendFriendRequestMessage(username));
	}

	// Token: 0x06000E16 RID: 3606 RVA: 0x0000B3CA File Offset: 0x000095CA
	public void RemoveFriend(Person p)
	{
		App.Communicator.send(new RemoveFriendMessage(p.profile.name));
	}

	// Token: 0x06000E17 RID: 3607 RVA: 0x000603FC File Offset: 0x0005E5FC
	public bool IsFriend(string username)
	{
		foreach (Person person in this.friends)
		{
			if (person.profile.name == username)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000E18 RID: 3608 RVA: 0x00060470 File Offset: 0x0005E670
	public bool IsFriendPending(string username)
	{
		foreach (Request request in this.requests)
		{
			if (request.to.profile.name == username)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000E19 RID: 3609 RVA: 0x0000B3E7 File Offset: 0x000095E7
	public bool IsBlocked(string username)
	{
		return this.blocked.Contains(username);
	}

	// Token: 0x06000E1A RID: 3610 RVA: 0x0000B3FD File Offset: 0x000095FD
	public void BlockUser(string username)
	{
		if (!this.blocked.Contains(username))
		{
			this.blocked.Add(username);
		}
		App.Communicator.send(new BlockPersonMessage(username));
	}

	// Token: 0x06000E1B RID: 3611 RVA: 0x0000B42D File Offset: 0x0000962D
	public void UnblockUser(string username)
	{
		if (this.blocked.Contains(username))
		{
			this.blocked.Remove(username);
		}
		App.Communicator.send(new UnblockPersonMessage(username));
	}

	// Token: 0x06000E1C RID: 3612 RVA: 0x000604E8 File Offset: 0x0005E6E8
	private void UpdateFriends(Person[] persons)
	{
		foreach (Person person in persons)
		{
			bool flag = false;
			for (int j = 0; j < this.friends.Count; j++)
			{
				Person person2 = this.friends[j];
				if (person2.IsSameUser(person))
				{
					this.friends[j] = person;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.friends.Add(person);
			}
		}
		this.friends.Sort();
	}

	// Token: 0x06000E1D RID: 3613 RVA: 0x00060580 File Offset: 0x0005E780
	private void UpdateRequests(Request[] newRequests)
	{
		List<int> list = new List<int>();
		foreach (Request request in newRequests)
		{
			bool flag = false;
			for (int j = 0; j < this.requests.Count; j++)
			{
				Request request2 = this.requests[j];
				if (request2.request.id == request.request.id)
				{
					this.requests[j] = request;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.requests.Add(request);
			}
		}
		for (int k = this.requests.Count - 1; k >= 0; k--)
		{
			Request request3 = this.requests[k];
			if (request3.request.status == FriendStatus.ACCEPTED)
			{
				if (request3.from.profile.id == App.MyProfile.ProfileInfo.id)
				{
					App.InviteManager.addInvite("Now friends with " + request3.to.profile.name, request3.to.profile.name + " has accepted your friend request.");
					this.UpdateFriends(new Person[]
					{
						request3.to
					});
					list.Add(request3.to.profile.id);
				}
				else
				{
					this.UpdateFriends(new Person[]
					{
						request3.from
					});
					list.Add(request3.from.profile.id);
				}
				this.requests.RemoveAt(k);
			}
			else if (request3.request.status == FriendStatus.DECLINED)
			{
				if (request3.from.profile.id == App.MyProfile.ProfileInfo.id)
				{
					App.InviteManager.addInvite(request3.to.profile.name + " declined your request", request3.to.profile.name + " has declined your friend request.");
				}
				this.requests.RemoveAt(k);
			}
			else if (request3.request.status == FriendStatus.CANCELED)
			{
				this.requests.RemoveAt(k);
			}
		}
		foreach (int num in list)
		{
			for (int l = this.requests.Count - 1; l >= 0; l--)
			{
				if (this.requests[l].from.profile.id == num || this.requests[l].to.profile.id == num)
				{
					this.requests.RemoveAt(l);
				}
			}
		}
		this.requests.Sort();
	}

	// Token: 0x04000AF6 RID: 2806
	private List<Person> friends = new List<Person>();

	// Token: 0x04000AF7 RID: 2807
	private List<string> blocked = new List<string>();

	// Token: 0x04000AF8 RID: 2808
	private List<Request> requests = new List<Request>();
}
