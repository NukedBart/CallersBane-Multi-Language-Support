using System;

// Token: 0x02000387 RID: 903
public interface IDeckSaveCallback : ICancelCallback, IOkStringCallback, IOkStringCancelCallback
{
	// Token: 0x06001402 RID: 5122
	void PopupExport(string popupType, string choice);

	// Token: 0x06001403 RID: 5123
	void PopupSaveAIDeck(string popupType, string choice);
}
