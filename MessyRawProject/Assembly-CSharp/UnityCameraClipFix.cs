using System;
using UnityEngine;

// Token: 0x0200045C RID: 1116
public class UnityCameraClipFix : MonoBehaviour
{
	// Token: 0x060018DC RID: 6364 RVA: 0x00012115 File Offset: 0x00010315
	public void init(Camera camera, Rect clipRect, bool isScreenSpace)
	{
		this._camera = camera;
		this._clipRect = clipRect;
		this._isScreenSpace = isScreenSpace;
	}

	// Token: 0x060018DD RID: 6365 RVA: 0x00093534 File Offset: 0x00091734
	private void Update()
	{
		if (--this._frames >= 0)
		{
			if (this._isScreenSpace)
			{
				GUIUtil.setScissorScreenRect(this._camera, this._clipRect);
			}
			else
			{
				GUIUtil.setScissorRect(this._camera, this._clipRect);
			}
		}
		else
		{
			Object.Destroy(this);
		}
	}

	// Token: 0x04001556 RID: 5462
	private int _frames = 5;

	// Token: 0x04001557 RID: 5463
	private Camera _camera;

	// Token: 0x04001558 RID: 5464
	private Rect _clipRect;

	// Token: 0x04001559 RID: 5465
	private bool _isScreenSpace;
}
