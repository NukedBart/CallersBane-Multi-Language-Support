using System;
using UnityEngine;

// Token: 0x0200038F RID: 911
public class PrimitiveFactory
{
	// Token: 0x06001451 RID: 5201 RVA: 0x0000EFC0 File Offset: 0x0000D1C0
	public static GameObject createPlane()
	{
		return PrimitiveFactory.createPlane(true);
	}

	// Token: 0x06001452 RID: 5202 RVA: 0x0007E58C File Offset: 0x0007C78C
	public static GameObject createPlane(bool createBoxCollider)
	{
		if (PrimitiveFactory._planeMesh == null)
		{
			PrimitiveFactory.init();
		}
		GameObject gameObject = new GameObject();
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.sharedMesh = PrimitiveFactory._planeMesh;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		Renderer renderer = meshRenderer;
		bool castShadows = false;
		meshRenderer.castShadows = castShadows;
		renderer.castShadows = castShadows;
		if (createBoxCollider)
		{
			gameObject.AddComponent<BoxCollider>();
		}
		return gameObject;
	}

	// Token: 0x06001453 RID: 5203 RVA: 0x0000EFC8 File Offset: 0x0000D1C8
	public static void init()
	{
		PrimitiveFactory._planeMesh = PrimitiveFactory._createMesh();
	}

	// Token: 0x06001454 RID: 5204 RVA: 0x0007E5EC File Offset: 0x0007C7EC
	public static Mesh _createMesh()
	{
		Mesh mesh = new Mesh();
		float num = 5f;
		float num2 = -5f;
		float num3 = -5f;
		float num4 = 5f;
		mesh.vertices = new Vector3[]
		{
			new Vector3(num + PrimitiveFactory._mx(), PrimitiveFactory._mz(), num4 + PrimitiveFactory._my()),
			new Vector3(num + PrimitiveFactory._mx(), PrimitiveFactory._mz(), num3 + PrimitiveFactory._my()),
			new Vector3(num2 + PrimitiveFactory._mx(), PrimitiveFactory._mz(), num4 + PrimitiveFactory._my()),
			new Vector3(num2 + PrimitiveFactory._mx(), PrimitiveFactory._mz(), num3 + PrimitiveFactory._my())
		};
		float num5 = 1E-05f;
		float num6 = 1f - num5;
		mesh.uv = new Vector2[]
		{
			new Vector2(num5, num5),
			new Vector2(num5, num6),
			new Vector2(num6, num5),
			new Vector2(num6, num6)
		};
		mesh.triangles = new int[]
		{
			0,
			1,
			2,
			3,
			2,
			1
		};
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		return mesh;
	}

	// Token: 0x06001455 RID: 5205 RVA: 0x0000EFD4 File Offset: 0x0000D1D4
	public static float _mx()
	{
		return 0f;
	}

	// Token: 0x06001456 RID: 5206 RVA: 0x0000EFDB File Offset: 0x0000D1DB
	public static float _my()
	{
		return PrimitiveFactory._mx();
	}

	// Token: 0x06001457 RID: 5207 RVA: 0x0000EFD4 File Offset: 0x0000D1D4
	public static float _mz()
	{
		return 0f;
	}

	// Token: 0x06001458 RID: 5208 RVA: 0x0000EFE2 File Offset: 0x0000D1E2
	public static GameObject createTexturedPlane(string texFn, Material material)
	{
		return PrimitiveFactory.createTexturedPlane(ResourceManager.LoadTexture(texFn), material);
	}

	// Token: 0x06001459 RID: 5209 RVA: 0x0007E750 File Offset: 0x0007C950
	public static GameObject createTexturedPlane(Texture tex, Material material)
	{
		GameObject gameObject = PrimitiveFactory.createPlane(false);
		if (material != null)
		{
			gameObject.renderer.material = material;
		}
		gameObject.renderer.material.mainTexture = tex;
		return gameObject;
	}

	// Token: 0x0600145A RID: 5210 RVA: 0x0000EFF0 File Offset: 0x0000D1F0
	private static Material getMaterial(bool alpha)
	{
		return (!alpha) ? PrimitiveFactory._opaqueMaterial : PrimitiveFactory._alphaMaterial;
	}

	// Token: 0x0600145B RID: 5211 RVA: 0x0000F007 File Offset: 0x0000D207
	public static GameObject createTexturedPlane(string fn, bool alpha)
	{
		return PrimitiveFactory.createTexturedPlane(fn, PrimitiveFactory.getMaterial(alpha));
	}

	// Token: 0x0600145C RID: 5212 RVA: 0x0000F015 File Offset: 0x0000D215
	public static GameObject createTexturedPlane(Texture tex, bool alpha)
	{
		return PrimitiveFactory.createTexturedPlane(tex, PrimitiveFactory.getMaterial(alpha));
	}

	// Token: 0x0400119B RID: 4507
	private static Mesh _planeMesh;

	// Token: 0x0400119C RID: 4508
	private static Material _alphaMaterial = new Material(ResourceManager.LoadShader("Scrolls/Unlit/Transparent"));

	// Token: 0x0400119D RID: 4509
	private static Material _opaqueMaterial = new Material(ResourceManager.LoadShader("Scrolls/Unlit/Color"));
}
