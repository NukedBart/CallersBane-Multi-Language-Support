using System;
using System.Collections.Generic;
using Animation.Serialization;
using UnityEngine;

namespace Irrelevant.Assets
{
	// Token: 0x02000017 RID: 23
	internal class GraphicsUtils
	{
		// Token: 0x06000166 RID: 358 RVA: 0x0001EAB4 File Offset: 0x0001CCB4
		public static Mesh createQuad(float w, float h, float up, float vp, float us, float vs)
		{
			Vector3[] array = new Vector3[4];
			Vector3[] array2 = new Vector3[4];
			Vector2[] array3 = new Vector2[4];
			int[] array4 = new int[6];
			array[0] = new Vector3(0f, h, 0f);
			array[1] = new Vector3(w, h, 0f);
			array[2] = new Vector3(0f, 0f, 0f);
			array[3] = new Vector3(w, 0f, 0f);
			for (int i = 0; i < array2.Length; i++)
			{
				Vector3 forward = Vector3.forward;
				array2[i] = forward;
			}
			array3[2] = new Vector2(up, 1f - vp);
			array3[3] = new Vector2(up + us, 1f - vp);
			array3[0] = new Vector2(up, 1f - (vp + vs));
			array3[1] = new Vector2(up + us, 1f - (vp + vs));
			array4[0] = 0;
			array4[1] = 2;
			array4[2] = 3;
			array4[3] = 0;
			array4[4] = 3;
			array4[5] = 1;
			Mesh mesh = new Mesh();
			mesh.vertices = array;
			mesh.triangles = array4;
			mesh.uv = array3;
			mesh.normals = array2;
			mesh.RecalculateBounds();
			return mesh;
		}

		// Token: 0x06000167 RID: 359 RVA: 0x0001EC34 File Offset: 0x0001CE34
		private static void apply(ref Matrix4x4 m, PD_AnimFramePart part)
		{
			m.m00 = part.a;
			m.m10 = part.b;
			m.m01 = part.c;
			m.m11 = part.d;
			m.m03 = part.tx;
			m.m13 = part.ty;
		}

		// Token: 0x06000168 RID: 360 RVA: 0x0001EC8C File Offset: 0x0001CE8C
		public static Mesh combineMeshes(List<PD_AnimFramePart> parts)
		{
			int count = parts.Count;
			int num = count * 4;
			Mesh mesh = new Mesh();
			Vector3[] array = new Vector3[num];
			Vector3[] array2 = new Vector3[num];
			Vector2[] array3 = new Vector2[num];
			Color[] array4 = new Color[num];
			int[] array5 = new int[count * 6];
			Matrix4x4 matrix4x = default(Matrix4x4);
			matrix4x.m33 = 1f;
			for (int i = 0; i < count; i++)
			{
				int num2 = 4 * i;
				int num3 = 6 * i;
				PD_AnimFramePart pd_AnimFramePart = parts[i];
				Color color = (pd_AnimFramePart.colorBits != (long)((ulong)-1)) ? ColorUtil.FromHex32((uint)pd_AnimFramePart.colorBits) : Color.white;
				GraphicsUtils.apply(ref matrix4x, pd_AnimFramePart);
				for (int j = 0; j < 4; j++)
				{
					Mesh mesh2 = (Mesh)pd_AnimFramePart.mesh;
					Vector3 vector = mesh2.vertices[j];
					array[num2 + j] = matrix4x.MultiplyPoint(vector);
					array3[num2 + j] = mesh2.uv[j];
					array4[num2 + j] = color;
					array2[num2 + j] = mesh2.normals[j];
				}
				array5[num3] = num2;
				array5[num3 + 1] = num2 + 2;
				array5[num3 + 2] = num2 + 3;
				array5[num3 + 3] = num2;
				array5[num3 + 4] = num2 + 3;
				array5[num3 + 5] = num2 + 1;
			}
			mesh.vertices = array;
			mesh.uv = array3;
			mesh.colors = array4;
			mesh.normals = array2;
			mesh.triangles = array5;
			return mesh;
		}

		// Token: 0x06000169 RID: 361 RVA: 0x0001EE54 File Offset: 0x0001D054
		public static Matrix4x4 multMatrix(Matrix4x4 m, Matrix4x4 o)
		{
			Matrix4x4 result = default(Matrix4x4);
			result.m00 = m.m00 * o.m00 + m.m01 * o.m10 + m.m02 * o.m20 + m.m03 * o.m30;
			result.m01 = m.m00 * o.m01 + m.m01 * o.m11 + m.m02 * o.m21 + m.m03 * o.m31;
			result.m02 = m.m00 * o.m02 + m.m01 * o.m12 + m.m02 * o.m22 + m.m03 * o.m32;
			result.m03 = m.m00 * o.m03 + m.m01 * o.m13 + m.m02 * o.m23 + m.m03 * o.m33;
			result.m10 = m.m10 * o.m00 + m.m11 * o.m10 + m.m12 * o.m20 + m.m13 * o.m30;
			result.m11 = m.m10 * o.m01 + m.m11 * o.m11 + m.m12 * o.m21 + m.m13 * o.m31;
			result.m12 = m.m10 * o.m02 + m.m11 * o.m12 + m.m12 * o.m22 + m.m13 * o.m32;
			result.m13 = m.m10 * o.m03 + m.m11 * o.m13 + m.m12 * o.m23 + m.m13 * o.m33;
			result.m20 = m.m20 * o.m00 + m.m21 * o.m10 + m.m22 * o.m20 + m.m23 * o.m30;
			result.m21 = m.m20 * o.m01 + m.m21 * o.m11 + m.m22 * o.m21 + m.m23 * o.m31;
			result.m22 = m.m20 * o.m02 + m.m21 * o.m12 + m.m22 * o.m22 + m.m23 * o.m32;
			result.m23 = m.m20 * o.m03 + m.m21 * o.m13 + m.m22 * o.m23 + m.m23 * o.m33;
			result.m30 = m.m30 * o.m00 + m.m31 * o.m10 + m.m32 * o.m20 + m.m33 * o.m30;
			result.m31 = m.m30 * o.m01 + m.m31 * o.m11 + m.m32 * o.m21 + m.m33 * o.m31;
			result.m32 = m.m30 * o.m02 + m.m31 * o.m12 + m.m32 * o.m22 + m.m33 * o.m32;
			result.m33 = m.m30 * o.m03 + m.m31 * o.m13 + m.m32 * o.m23 + m.m33 * o.m33;
			return result;
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000323F File Offset: 0x0000143F
		public static Matrix4x4 multMatrix(Matrix4x4 m, Transform t)
		{
			return GraphicsUtils.multMatrix(m, Matrix4x4.TRS(t.position, t.rotation, new Vector3(1f, 1f, 1f)));
		}

		// Token: 0x0600016B RID: 363 RVA: 0x0000326C File Offset: 0x0000146C
		public static Matrix4x4 multMatrix(Matrix4x4 m, Vector3 pos, Quaternion rot, Vector3 scale)
		{
			return GraphicsUtils.multMatrix(m, Matrix4x4.TRS(pos, rot, scale));
		}
	}
}
