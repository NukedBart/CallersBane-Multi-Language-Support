using System;

// Token: 0x020002B5 RID: 693
public class ChatMessageMessage : Message
{
	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x06001280 RID: 4736 RVA: 0x0000DE88 File Offset: 0x0000C088
	// (set) Token: 0x06001281 RID: 4737 RVA: 0x0000DE90 File Offset: 0x0000C090
	public string text
	{
		get
		{
			return this.get();
		}
		set
		{
			this._text = value;
		}
	}

	// Token: 0x06001282 RID: 4738 RVA: 0x0000DE99 File Offset: 0x0000C099
	private string get()
	{
		return this.get(App.Config.settings.preferences.profanity_filter);
	}

	// Token: 0x06001283 RID: 4739 RVA: 0x0000DEBA File Offset: 0x0000C0BA
	private string get(bool censor)
	{
		if (censor && this.censoredText != null)
		{
			return this.censoredText;
		}
		return this._text;
	}

	// Token: 0x04000F5A RID: 3930
	public string from;

	// Token: 0x04000F5B RID: 3931
	public string censoredText;

	// Token: 0x04000F5C RID: 3932
	private string _text;
}
