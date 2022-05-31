using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000183 RID: 387
public class CustomGames : AbstractCommListener, IOkCallback, ICancelCallback, IOkCancelCallback
{
	// Token: 0x06000C03 RID: 3075 RVA: 0x00009D35 File Offset: 0x00007F35
	private void Start()
	{
		this.impl = new CustomGamesImpl();
		App.Communicator.addListener(this);
		App.ChatUI.Show(false);
		base.StartCoroutine(this.fadeInAfterWait(0.2f));
	}

	// Token: 0x06000C04 RID: 3076 RVA: 0x00054E3C File Offset: 0x0005303C
	private IEnumerator fadeInAfterWait(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		App.LobbyMenu.fadeInScene();
		yield break;
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x00009D6B File Offset: 0x00007F6B
	private void OnGUI()
	{
		this.impl.OnGUI();
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x00009D78 File Offset: 0x00007F78
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.impl.OnDestroy();
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x00009D8B File Offset: 0x00007F8B
	public override void handleMessage(Message message)
	{
		this.impl.handleMessage(message);
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x00009D99 File Offset: 0x00007F99
	public void PopupOk(string popupType)
	{
		this.impl.PopupOk(popupType);
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x00009DA7 File Offset: 0x00007FA7
	public void PopupCancel(string popupType)
	{
		this.impl.PopupCancel(popupType);
	}

	// Token: 0x04000940 RID: 2368
	private CustomGamesImpl impl;
}
