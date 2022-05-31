using System;
using UnityEngine;

// Token: 0x020003EE RID: 1006
public class SignOut : MonoBehaviour
{
	// Token: 0x060015FF RID: 5631 RVA: 0x000853A8 File Offset: 0x000835A8
	private void Start()
	{
		GameObject gameObject = Camera.main.gameObject;
		Object.DontDestroyOnLoad(gameObject);
		Object[] array = Object.FindObjectsOfType(typeof(GameObject));
		foreach (Object @object in array)
		{
			if (@object != base.gameObject && @object != gameObject)
			{
				Object.DestroyImmediate(@object);
			}
		}
		SceneLoader.loadScene("_LoginView");
		Object.DestroyImmediate(gameObject);
		Object.Destroy(this);
	}
}
