using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200013F RID: 319
public class ColorChanger
{
	// Token: 0x06000A89 RID: 2697 RVA: 0x00008E40 File Offset: 0x00007040
	public void Initialize(GameObject go)
	{
		this.go = go;
		this.StoreColorRecursively(go);
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x00008E50 File Offset: 0x00007050
	public void SetColor(Color c)
	{
		this.color = c;
		this.SetColorRecursively(this.go, c);
	}

	// Token: 0x06000A8B RID: 2699 RVA: 0x00008E66 File Offset: 0x00007066
	public Color GetColor()
	{
		return this.color;
	}

	// Token: 0x06000A8C RID: 2700 RVA: 0x00008E6E File Offset: 0x0000706E
	public void Add(GameObject go)
	{
		this.StoreColorRecursively(go);
	}

	// Token: 0x06000A8D RID: 2701 RVA: 0x00008E77 File Offset: 0x00007077
	public void Remove(GameObject go)
	{
		this.RemoveRecursively(go);
	}

	// Token: 0x06000A8E RID: 2702 RVA: 0x00050228 File Offset: 0x0004E428
	private void StoreColor(GameObject go)
	{
		if (go.renderer && go.renderer.material && go.renderer.material.color != Color.white && !this.originalColors.ContainsKey(go.renderer.material))
		{
			this.originalColors.Add(go.renderer.material, go.renderer.material.color);
		}
	}

	// Token: 0x06000A8F RID: 2703 RVA: 0x000502BC File Offset: 0x0004E4BC
	private void StoreColorRecursively(GameObject go)
	{
		this.StoreColor(go);
		foreach (object obj in go.GetComponentInChildren<Transform>())
		{
			Transform transform = (Transform)obj;
			this.StoreColorRecursively(transform.gameObject);
		}
	}

	// Token: 0x06000A90 RID: 2704 RVA: 0x0005032C File Offset: 0x0004E52C
	private void SetColorRecursively(GameObject go, Color c)
	{
		if (go.renderer && go.renderer.material)
		{
			if (this.originalColors.ContainsKey(go.renderer.material))
			{
				go.renderer.material.color = this.originalColors[go.renderer.material] * c;
			}
			else
			{
				go.renderer.material.color = c;
			}
		}
		foreach (object obj in go.GetComponentInChildren<Transform>())
		{
			Transform transform = (Transform)obj;
			this.SetColorRecursively(transform.gameObject, c);
		}
	}

	// Token: 0x06000A91 RID: 2705 RVA: 0x00050418 File Offset: 0x0004E618
	private void RemoveRecursively(GameObject go)
	{
		this.originalColors.Remove(go.renderer.material);
		foreach (object obj in go.GetComponentInChildren<Transform>())
		{
			Transform transform = (Transform)obj;
			this.RemoveRecursively(transform.gameObject);
		}
	}

	// Token: 0x0400081C RID: 2076
	private Color color = Color.white;

	// Token: 0x0400081D RID: 2077
	private GameObject go;

	// Token: 0x0400081E RID: 2078
	private Dictionary<Material, Color> originalColors = new Dictionary<Material, Color>();
}
