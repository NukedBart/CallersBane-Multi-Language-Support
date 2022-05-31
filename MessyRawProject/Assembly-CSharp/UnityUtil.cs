using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200045D RID: 1117
public class UnityUtil
{
	// Token: 0x060018DF RID: 6367 RVA: 0x0001212C File Offset: 0x0001032C
	public static void refreshBoxCollider(GameObject g)
	{
		if (!(g.collider is BoxCollider))
		{
			return;
		}
		((BoxCollider)g.collider).size = ((BoxCollider)g.collider).size;
	}

	// Token: 0x060018E0 RID: 6368 RVA: 0x00093598 File Offset: 0x00091798
	public static void traverse(GameObject root, Action<GameObject> callback)
	{
		callback.Invoke(root);
		foreach (object obj in root.transform)
		{
			Transform transform = (Transform)obj;
			UnityUtil.traverse(transform.gameObject, callback);
		}
	}

	// Token: 0x060018E1 RID: 6369 RVA: 0x00093608 File Offset: 0x00091808
	public static void setLayerRecursively(GameObject obj, int newLayer)
	{
		UnityUtil.traverse(obj, delegate(GameObject g)
		{
			g.layer = newLayer;
		});
	}

	// Token: 0x060018E2 RID: 6370 RVA: 0x0001215F File Offset: 0x0001035F
	public static GameObject addChild(GameObject parent, GameObject child)
	{
		child.transform.parent = parent.transform;
		UnityUtil.setLocalIdentity(child.transform);
		return child;
	}

	// Token: 0x060018E3 RID: 6371 RVA: 0x00093634 File Offset: 0x00091834
	public static bool GetKeyDown(KeyCode c, params KeyCode[] whileDown)
	{
		if (!Input.GetKeyDown(c))
		{
			return false;
		}
		foreach (KeyCode keyCode in whileDown)
		{
			if (!Input.GetKey(keyCode))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060018E4 RID: 6372 RVA: 0x0001217E File Offset: 0x0001037E
	public static void setLocalIdentity(Transform t)
	{
		t.localScale = Vector3.one;
		t.localPosition = Vector3.zero;
		t.localEulerAngles = Vector3.zero;
	}

	// Token: 0x060018E5 RID: 6373 RVA: 0x00093678 File Offset: 0x00091878
	public static List<GameObject> allParents(GameObject g)
	{
		List<GameObject> list = new List<GameObject>();
		Transform transform = g.transform;
		while (transform != null && transform.gameObject != null)
		{
			list.Add(transform.gameObject);
			transform = transform.parent;
		}
		return list;
	}

	// Token: 0x060018E6 RID: 6374 RVA: 0x000121A1 File Offset: 0x000103A1
	public static bool hasParent(Transform t, Transform parent)
	{
		while (t != null)
		{
			if (t == parent)
			{
				return true;
			}
			t = t.parent;
		}
		return false;
	}

	// Token: 0x060018E7 RID: 6375 RVA: 0x000936C8 File Offset: 0x000918C8
	public static RaycastHit getRaycastHitFor(params RayInfo[] rayInfos)
	{
		foreach (RayInfo rayInfo in rayInfos)
		{
			RaycastHit result = default(RaycastHit);
			if (Physics.Raycast(rayInfo.ray, ref result, float.PositiveInfinity, rayInfo.layerMask))
			{
				return result;
			}
		}
		return default(RaycastHit);
	}

	// Token: 0x060018E8 RID: 6376 RVA: 0x000121CB File Offset: 0x000103CB
	public static Camera getFirstOrtographicCamera()
	{
		return Enumerable.FirstOrDefault<Camera>(Camera.allCameras, (Camera c) => c.isOrthoGraphic);
	}

	// Token: 0x060018E9 RID: 6377 RVA: 0x000121F4 File Offset: 0x000103F4
	public static Camera getFirstPerspectiveCamera()
	{
		return Enumerable.FirstOrDefault<Camera>(Camera.allCameras, (Camera c) => !c.isOrthoGraphic);
	}
}
