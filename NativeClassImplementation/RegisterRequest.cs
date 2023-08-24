using System;
using System.Globalization;

// Token: 0x0200014E RID: 334
public class RegisterRequest : PostRequest
{
	// Token: 0x06000AB7 RID: 2743 RVA: 0x0000907A File Offset: 0x0000727A
	public RegisterRequest(string email, string password, string username, bool acceptsNewsletters, DateTime dateOfBirth) : base(App.Communicator.ApiURL, new RegisterRequest.RegistrationPayload(email, password, username, acceptsNewsletters, dateOfBirth))
	{
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x00050650 File Offset: 0x0004E850
	public override string getError()
	{
		int errorCode = base.getErrorCode();
		if (errorCode == 404)
		{
			return "Could not find the registration service. Please try again later.";
		}
		if (errorCode == 400 || errorCode == 403 || errorCode == 429)
		{
			return base.buildError<RegistrationError>().errorMessage;
		}
		if (errorCode == 500)
		{
			return "Registration failed. Please try again. If it still doesn't work, please contact support at help.mojang.com.";
		}
		return base.getError();
	}

	// Token: 0x0200014F RID: 335
	private class RegistrationPayload
	{
		// Token: 0x06000AB9 RID: 2745 RVA: 0x000506BC File Offset: 0x0004E8BC
		public RegistrationPayload(string email, string password, string username, bool acceptsNewsletters, DateTime dateOfBirth)
		{
			this.email = email;
			this.password = password;
			this.profileName = username;
			this.acceptsNewsletters = acceptsNewsletters;
			this.dateOfBirth = dateOfBirth.ToString("s", CultureInfo.InvariantCulture);
		}

		// Token: 0x0400083A RID: 2106
		public string agent = "scrolls";

		// Token: 0x0400083B RID: 2107
		public string email;

		// Token: 0x0400083C RID: 2108
		public string password;

		// Token: 0x0400083D RID: 2109
		public string profileName;

		// Token: 0x0400083E RID: 2110
		public string dateOfBirth;

		// Token: 0x0400083F RID: 2111
		public bool acceptsNewsletters;
	}
}
