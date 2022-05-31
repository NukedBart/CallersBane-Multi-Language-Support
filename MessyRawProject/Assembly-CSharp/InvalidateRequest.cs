using System;

// Token: 0x02000148 RID: 328
public class InvalidateRequest : PostRequest
{
	// Token: 0x06000AA7 RID: 2727 RVA: 0x00008FB4 File Offset: 0x000071B4
	public InvalidateRequest(string accessToken) : base(App.Communicator.getAuthUrlForCall("invalidate"), new InvalidateRequest.Data(accessToken))
	{
	}

	// Token: 0x02000149 RID: 329
	private class Data
	{
		// Token: 0x06000AA8 RID: 2728 RVA: 0x00008FD1 File Offset: 0x000071D1
		public Data(string accessToken)
		{
			this.accessToken = accessToken;
		}

		// Token: 0x04000830 RID: 2096
		public string accessToken;
	}
}
