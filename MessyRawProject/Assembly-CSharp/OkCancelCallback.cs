using System;

// Token: 0x0200038B RID: 907
public class OkCancelCallback : IOkCallback, ICancelCallback, IOkCancelCallback
{
	// Token: 0x0600140A RID: 5130 RVA: 0x0000EC97 File Offset: 0x0000CE97
	public OkCancelCallback(Action<string> ok, Action<string> cancel)
	{
		this.ok = ok;
		this.cancel = cancel;
	}

	// Token: 0x0600140B RID: 5131 RVA: 0x0000ECAD File Offset: 0x0000CEAD
	public void PopupOk(string popupType)
	{
		if (this.ok != null)
		{
			this.ok.Invoke(popupType);
		}
	}

	// Token: 0x0600140C RID: 5132 RVA: 0x0000ECC6 File Offset: 0x0000CEC6
	public void PopupCancel(string popupType)
	{
		if (this.cancel != null)
		{
			this.cancel.Invoke(popupType);
		}
	}

	// Token: 0x0400112A RID: 4394
	private Action<string> ok;

	// Token: 0x0400112B RID: 4395
	private Action<string> cancel;
}
