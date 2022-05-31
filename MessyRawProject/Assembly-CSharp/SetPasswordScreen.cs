using System;
using UnityEngine;

// Token: 0x0200021F RID: 543
internal class SetPasswordScreen : IOkCallback, ICancelCallback, IOkCancelCallback
{
	// Token: 0x0600114F RID: 4431 RVA: 0x00075FB0 File Offset: 0x000741B0
	public SetPasswordScreen()
	{
		this.emptySkin = (GUISkin)ResourceManager.Load("_GUISkins/EmptySkin");
		this.rect = GUIUtil.centeredScreen(0.4f, 0.3f);
		App.Popups.ShowOkCancelRenderer(this, "setPassword", "Change password", this.rect, new Action(this.render), "Ok", "Cancel");
	}

	// Token: 0x06001151 RID: 4433 RVA: 0x00076040 File Offset: 0x00074240
	public void render()
	{
		Rect rect;
		rect..ctor(0.2f, 0.05f, 0.6f, 0.15f);
		GUI.Label(GeomUtil.cropShare(this.rect, GeomUtil.getTranslated(rect, 0f, -0.15f)), "Current password");
		this.oldPassword = Login.drawTextBox(this.emptySkin.button, "spOld", GeomUtil.cropShare(this.rect, rect), this.oldPassword, true);
		rect.y = 0.45f;
		GUI.Label(GeomUtil.cropShare(this.rect, GeomUtil.getTranslated(rect, 0f, -0.15f)), "New password");
		this.newPassword = Login.drawTextBox(this.emptySkin.button, "spNew", GeomUtil.cropShare(this.rect, rect), this.newPassword, true);
		rect.y = 0.75f;
		GUI.Label(GeomUtil.cropShare(this.rect, GeomUtil.getTranslated(rect, 0f, -0.15f)), "Repeat new password");
		this.repeatNewPassword = Login.drawTextBox(this.emptySkin.button, "spRepeatNew", GeomUtil.cropShare(this.rect, rect), this.repeatNewPassword, true);
		if (this.err != null)
		{
			rect.y = 0.95f;
			rect.x = 0f;
			rect.width = 1f;
			GUI.Label(GeomUtil.cropShare(this.rect, GeomUtil.getTranslated(rect, 0f, 0f)), GUIUtil.RtColor(this.err, SetPasswordScreen.errorColor));
		}
	}

	// Token: 0x06001152 RID: 4434 RVA: 0x0000D3BC File Offset: 0x0000B5BC
	public void setError(string error)
	{
		this.err = error;
	}

	// Token: 0x06001153 RID: 4435 RVA: 0x000028DF File Offset: 0x00000ADF
	public void PopupCancel(string popupType)
	{
	}

	// Token: 0x06001154 RID: 4436 RVA: 0x000761D8 File Offset: 0x000743D8
	public void PopupOk(string popupType)
	{
		if (popupType != "setPassword")
		{
			return;
		}
		App.Popups.StayOpen();
		this.err = Login.validatePassword(this.newPassword, this.repeatNewPassword, ">>>><<%FD#)#)");
		if (this.err == null)
		{
			App.Communicator.send(new SetPasswordMessage(this.oldPassword, this.newPassword));
		}
	}

	// Token: 0x04000DBD RID: 3517
	private string oldPassword = string.Empty;

	// Token: 0x04000DBE RID: 3518
	private string newPassword = string.Empty;

	// Token: 0x04000DBF RID: 3519
	private string repeatNewPassword = string.Empty;

	// Token: 0x04000DC0 RID: 3520
	private Rect rect;

	// Token: 0x04000DC1 RID: 3521
	private GUISkin emptySkin;

	// Token: 0x04000DC2 RID: 3522
	private static Color errorColor = ColorUtil.FromHex24(16737877u);

	// Token: 0x04000DC3 RID: 3523
	private string err;
}
