using System;
using UnityEngine;

// Token: 0x020001D4 RID: 468
public class GUIBlackOverlayButton : MonoBehaviour
{
	// Token: 0x06000EB6 RID: 3766 RVA: 0x0000BC0F File Offset: 0x00009E0F
	public void Init(IOverlayClickCallback callback, int guiDepth, bool invisible)
	{
		this.Init(callback, guiDepth, new Rect(-50f, -50f, (float)(Screen.width + 100), (float)(Screen.height + 100)), invisible);
	}

	// Token: 0x06000EB7 RID: 3767 RVA: 0x0000BC3B File Offset: 0x00009E3B
	public void Init(IOverlayClickCallback callback, int guiDepth, Rect area, bool invisible)
	{
		this.callback = callback;
		this.guiDepth = guiDepth;
		this.area = area;
		this.invisible = invisible;
		this.emptyGUISkin = (GUISkin)ResourceManager.Load("_GUISkins/EmptySkin");
	}

	// Token: 0x06000EB8 RID: 3768 RVA: 0x0000BC6F File Offset: 0x00009E6F
	public void SetArea(Rect area)
	{
		this.area = area;
	}

	// Token: 0x06000EB9 RID: 3769 RVA: 0x0000BC78 File Offset: 0x00009E78
	public bool IsMouseOverArea(Vector2 mousePos)
	{
		return base.enabled && this.area.Contains(mousePos);
	}

	// Token: 0x06000EBA RID: 3770 RVA: 0x0000BC93 File Offset: 0x00009E93
	public void SetAlpha(float alpha)
	{
		this.alpha = alpha;
	}

	// Token: 0x06000EBB RID: 3771 RVA: 0x00063470 File Offset: 0x00061670
	private void OnGUI()
	{
		GUI.depth = this.guiDepth;
		GUI.skin = this.emptyGUISkin;
		if (!this.invisible)
		{
			GUI.color = new Color(1f, 1f, 1f, this.alpha);
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), ResourceManager.LoadTexture("Shared/blackFiller"));
			GUI.color = new Color(1f, 1f, 1f, 1f);
		}
		if (GUI.Button(this.area, string.Empty))
		{
			this.callback.OverlayClicked();
		}
	}

	// Token: 0x04000B62 RID: 2914
	private IOverlayClickCallback callback;

	// Token: 0x04000B63 RID: 2915
	private int guiDepth;

	// Token: 0x04000B64 RID: 2916
	private Rect area;

	// Token: 0x04000B65 RID: 2917
	private bool invisible;

	// Token: 0x04000B66 RID: 2918
	private GUISkin emptyGUISkin;

	// Token: 0x04000B67 RID: 2919
	private float alpha = 0.5f;
}
