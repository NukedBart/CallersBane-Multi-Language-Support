using System;
using UnityEngine;

// Token: 0x02000390 RID: 912
public static class PlaneModifier
{
	// Token: 0x0600145D RID: 5213 RVA: 0x0007E790 File Offset: 0x0007C990
	public static void shear(GameObject g, float u, float d)
	{
		Mesh mesh = g.GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		for (int i = 0; i < vertices.Length; i++)
		{
			float x = vertices[i].x;
			Vector3[] array = vertices;
			int num = i;
			array[num].x = array[num].x + ((mesh.uv[i].y >= 0.5f) ? (-u) : (-d));
		}
		mesh.vertices = vertices;
		mesh.RecalculateBounds();
	}
}
