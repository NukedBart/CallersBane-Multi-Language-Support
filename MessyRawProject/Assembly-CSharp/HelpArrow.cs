using System;
using System.Collections.Generic;
using System.Linq;
using Gui;
using UnityEngine;

// Token: 0x02000069 RID: 105
public class HelpArrow : MonoBehaviour
{
	// Token: 0x06000431 RID: 1073 RVA: 0x00030AF8 File Offset: 0x0002ECF8
	private void Start()
	{
		this.cam = HelpArrow.guessCameraFor(base.gameObject);
		string tag = base.gameObject.tag;
		int renderQueue = 93989;
		if (tag == "blinkable_card")
		{
			this.move = 0.5f;
			this.offset = new Vector2(0f, (float)Screen.height * -0.17f);
		}
		if (base.gameObject.name.StartsWith("End_Turn"))
		{
			this.scale = 1.5f;
		}
		if (base.gameObject.name == Blinker.Name_ResourceCounter)
		{
			this.fixedPosition = new Vector2?(new Vector2((float)Screen.width * 0.05f, (float)Screen.height * 0.92f));
			this.fixedAngle = new float?(1.0995574f);
			this.scale = 1.5f;
			this.move = 0.75f;
		}
		if (tag == "blinkable_attack" || tag == "blinkable_health" || tag == "blinkable_countdown" || tag == "blinkable_cost")
		{
			HelpArrow.<Start>c__AnonStorey8C <Start>c__AnonStorey8C = new HelpArrow.<Start>c__AnonStorey8C();
			HelpArrow.<Start>c__AnonStorey8C <Start>c__AnonStorey8C2 = <Start>c__AnonStorey8C;
			List<string> list = new List<string>();
			list.Add("Card_");
			list.Add("PlayedCard");
			list.Add("Resource_Cost_Number");
			<Start>c__AnonStorey8C2.disabledNames = list;
			if (Enumerable.Any<GameObject>(UnityUtil.allParents(base.gameObject), (GameObject g) => HelpArrow.startsWithAnyOf(g.name, <Start>c__AnonStorey8C.disabledNames)))
			{
				base.enabled = false;
				return;
			}
			if (tag == "blinkable_attack")
			{
				this.fixedAngle = new float?(3.926991f);
			}
			if (tag == "blinkable_countdown")
			{
				this.fixedAngle = new float?(4.712389f);
			}
			if (tag == "blinkable_health")
			{
				this.fixedAngle = new float?(5.4977875f);
			}
			this.move = 0.75f;
			if (Enumerable.Any<GameObject>(UnityUtil.allParents(base.gameObject), (GameObject g) => g.name == "statsboard"))
			{
				this.scale = 0.4f;
			}
		}
		this.pointer = PrimitiveFactory.createTexturedPlane("Tutorial/Tutorial_arrow_noglow", true);
		this.pointer.name = "pointer_" + tag;
		this.pointer.layer = 9;
		this.pointer.renderer.material.renderQueue = renderQueue;
		Camera firstOrtographicCamera = UnityUtil.getFirstOrtographicCamera();
		this.gui = new Gui3D(firstOrtographicCamera);
		this.gui.orientPlane(this.pointer);
		this.gui.setBaseDepth(firstOrtographicCamera.farClipPlane - 5f - 0.1f);
		this.onInit();
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x00030DC8 File Offset: 0x0002EFC8
	private static bool startsWithAnyOf(string e, IEnumerable<string> with)
	{
		foreach (string text in with)
		{
			if (e.StartsWith(text))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000433 RID: 1075 RVA: 0x00030E2C File Offset: 0x0002F02C
	private static Camera guessCameraFor(GameObject g)
	{
		int layer = g.layer;
		if (layer == Layers.BattleModeUI || layer == Layers.BattleModeUI_NoHandManager || layer == Layers.InFrontOfUI)
		{
			return UnityUtil.getFirstOrtographicCamera();
		}
		return UnityUtil.getFirstPerspectiveCamera();
	}

	// Token: 0x06000434 RID: 1076 RVA: 0x00030E6C File Offset: 0x0002F06C
	private void Update()
	{
		if (!base.enabled)
		{
			return;
		}
		if (base.gameObject.transform == null)
		{
			return;
		}
		float num = this.a;
		this.a = Mathf.Min(1f, this.a + Time.deltaTime * 4f);
		if (this.a >= num)
		{
			this.pointer.renderer.material.color = new Color(1f, 1f, 1f, this.a);
		}
		Vector3 vector = this.cam.WorldToScreenPoint(base.gameObject.transform.position);
		Vector2 p = GeomUtil.v3tov2(GUIUtil.getScreenMousePos(vector)) + this.offset;
		if (this.fixedPosition != null)
		{
			p = this.fixedPosition.Value;
		}
		this.setupPosition(p);
		float num2 = 0.08f * Mathf.Sin(5f * Time.time);
		Vector3 localScale = Vector3.one * (0.5f * (this.scale * (10f * (1f + num2))));
		localScale.x *= 1.396648f;
		this.pointer.transform.localScale = localScale;
	}

	// Token: 0x06000435 RID: 1077 RVA: 0x000028DF File Offset: 0x00000ADF
	protected virtual void onInit()
	{
	}

	// Token: 0x06000436 RID: 1078 RVA: 0x00030FC4 File Offset: 0x0002F1C4
	protected virtual void setupPosition(Vector2 p)
	{
		Vector2 vector = new Vector2((float)Screen.width, (float)Screen.height) * 0.5f;
		Vector2 vector2 = p - vector;
		if (this.fixedAngle != null)
		{
			float value = this.fixedAngle.Value;
			vector2 = new Vector2(-Mathf.Cos(value), Mathf.Sin(value)) * vector2.magnitude;
		}
		float num = Mathf.Atan2(vector2.y, vector2.x);
		Vector2 vector3 = vector2.normalized * (this.move * 0.1f * (float)Screen.height);
		this.gui.DrawObject(p.x - vector3.x * this.scale, p.y - vector3.y * this.scale, this.pointer);
		this.pointer.transform.localEulerAngles = new Vector3(90f - 57.29578f * num, 270f, 90f);
	}

	// Token: 0x06000437 RID: 1079 RVA: 0x000310D0 File Offset: 0x0002F2D0
	private void OnDestroy()
	{
		if (this.pointer == null)
		{
			return;
		}
		iTween.ColorTo(this.pointer, iTween.Hash(new object[]
		{
			"color",
			new Color(1f, 1f, 1f, 0f),
			"time",
			0.2f
		}));
		Object.Destroy(this.pointer, 0.25f);
	}

	// Token: 0x040002B7 RID: 695
	protected GameObject pointer;

	// Token: 0x040002B8 RID: 696
	private float a = 1f;

	// Token: 0x040002B9 RID: 697
	protected Gui3D gui;

	// Token: 0x040002BA RID: 698
	private Camera cam;

	// Token: 0x040002BB RID: 699
	private float scale = 1f;

	// Token: 0x040002BC RID: 700
	private float move = 1f;

	// Token: 0x040002BD RID: 701
	private Vector2 offset = Vector2.zero;

	// Token: 0x040002BE RID: 702
	private Vector2? fixedPosition;

	// Token: 0x040002BF RID: 703
	private float? fixedAngle;
}
