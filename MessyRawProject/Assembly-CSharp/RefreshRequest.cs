using System;

// Token: 0x0200014B RID: 331
public class RefreshRequest : PostRequest
{
	// Token: 0x06000AB2 RID: 2738 RVA: 0x0000903A File Offset: 0x0000723A
	public RefreshRequest(string accessToken) : base(App.Communicator.getAuthUrlForCall("refresh"), new RefreshRequest.RefreshPayload(accessToken, AuthRequest.CLIENT_TOKEN))
	{
	}

	// Token: 0x06000AB3 RID: 2739 RVA: 0x0000905C File Offset: 0x0000725C
	public RefreshResponse getResponse()
	{
		return base.buildResponse<RefreshResponse>();
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x00050614 File Offset: 0x0004E814
	public override string getError()
	{
		int errorCode = base.getErrorCode();
		if (errorCode == 403)
		{
			return "Invalid access token.";
		}
		if (errorCode == 404)
		{
			return "Could not find the auth service. Please try again later.";
		}
		return base.getError();
	}

	// Token: 0x0200014C RID: 332
	private class RefreshPayload
	{
		// Token: 0x06000AB5 RID: 2741 RVA: 0x00009064 File Offset: 0x00007264
		public RefreshPayload(string accessToken, string clientToken)
		{
			this.accessToken = accessToken;
			this.clientToken = clientToken;
		}

		// Token: 0x04000836 RID: 2102
		public string accessToken;

		// Token: 0x04000837 RID: 2103
		public string clientToken;
	}
}
