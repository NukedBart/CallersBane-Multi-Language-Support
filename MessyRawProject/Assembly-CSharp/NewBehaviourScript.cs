using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000010 RID: 16
public class NewBehaviourScript : MonoBehaviour
{
	// Token: 0x0600011F RID: 287 RVA: 0x0001DFFC File Offset: 0x0001C1FC
	private void Start()
	{
		Material sharedMaterial = this.createMaterial();
		foreach (Object @object in Object.FindObjectsOfType(typeof(GameObject)))
		{
			GameObject gameObject = (GameObject)@object;
			gameObject.renderer.sharedMaterial = sharedMaterial;
			Log.warning("creating: " + RuntimeHelpers.GetHashCode(gameObject.renderer.sharedMaterial));
		}
	}

	// Token: 0x06000120 RID: 288 RVA: 0x0001E074 File Offset: 0x0001C274
	private void Update()
	{
		foreach (Object @object in Object.FindObjectsOfType(typeof(GameObject)))
		{
			GameObject gameObject = (GameObject)@object;
			Log.warning("list: " + RuntimeHelpers.GetHashCode(gameObject.renderer.sharedMaterial));
		}
	}

	// Token: 0x06000121 RID: 289 RVA: 0x00002E62 File Offset: 0x00001062
	private Material createMaterial()
	{
		return new Material(Shader.Find("Diffuse"));
	}
}
