using System;
using UnityEngine;

// Token: 0x02000141 RID: 321
public class AuthRequest : PostRequest
{
	// Token: 0x06000A96 RID: 2710 RVA: 0x00008E8D File Offset: 0x0000708D
	public AuthRequest(string username, string password) : base(App.Communicator.getAuthUrlForCall("authenticate"), AuthRequest.AuthPayload.CreateWithUsername(username, password, AuthRequest.CLIENT_TOKEN))
	{
	}

	// Token: 0x06000A97 RID: 2711 RVA: 0x00008EB0 File Offset: 0x000070B0
	public AuthRequest(string userId, string password, string clientToken) : base(App.Communicator.getAuthUrlForCall("authenticate"), AuthRequest.AuthPayload.CreateWithUserId(userId, password, userId))
	{
	}

	// Token: 0x06000A99 RID: 2713 RVA: 0x00008EE0 File Offset: 0x000070E0
	public AuthResponse getResponse()
	{
		return base.buildResponse<AuthResponse>();
	}

	// Token: 0x06000A9A RID: 2714 RVA: 0x00050498 File Offset: 0x0004E698
	public override string getError()
	{
		int errorCode = base.getErrorCode();
		if (errorCode == 403)
		{
			return "Invalid username or password.";
		}
		if (errorCode == 404)
		{
			return "Could not find the auth service. Please try again later.";
		}
		return base.getError();
	}

	// Token: 0x0400081F RID: 2079
	public static string CLIENT_TOKEN = Hash.sha256(SystemInfo.deviceUniqueIdentifier);

	// Token: 0x02000142 RID: 322
	private class AuthPayload
	{
		// Token: 0x06000A9B RID: 2715 RVA: 0x00008EE8 File Offset: 0x000070E8
		private AuthPayload(string username, string userId, string password, string clientToken)
		{
			this.username = username;
			this.userId = userId;
			this.password = password;
			this.clientToken = clientToken;
		}

		// Token: 0x06000A9C RID: 2716 RVA: 0x00008F1F File Offset: 0x0000711F
		public static AuthRequest.AuthPayload CreateWithUsername(string username, string password, string clientToken)
		{
			return new AuthRequest.AuthPayload(username, null, password, clientToken);
		}

		// Token: 0x06000A9D RID: 2717 RVA: 0x00008F2A File Offset: 0x0000712A
		public static AuthRequest.AuthPayload CreateWithUserId(string userId, string password, string clientToken)
		{
			return new AuthRequest.AuthPayload(null, userId, password, clientToken);
		}

		// Token: 0x04000820 RID: 2080
		public AuthRequest.AuthPayload.Agent agent = new AuthRequest.AuthPayload.Agent();

		// Token: 0x04000821 RID: 2081
		public string username;

		// Token: 0x04000822 RID: 2082
		public string userId;

		// Token: 0x04000823 RID: 2083
		public string password;

		// Token: 0x04000824 RID: 2084
		public string clientToken;

		// Token: 0x04000825 RID: 2085
		public bool requestUser = true;

		// Token: 0x02000143 RID: 323
		public class Agent
		{
			// Token: 0x04000826 RID: 2086
			public string name = "Scrolls";

			// Token: 0x04000827 RID: 2087
			public int version = 1;
		}
	}
}
