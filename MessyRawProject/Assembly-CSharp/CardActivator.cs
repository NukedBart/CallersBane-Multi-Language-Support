using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000056 RID: 86
public class CardActivator : MonoBehaviour
{
	// Token: 0x060003CA RID: 970 RVA: 0x0002E8AC File Offset: 0x0002CAAC
	public void init(Camera handCamera, ICardActivatorCallback callbackTarget, CardView card, ResourceType[] resTypes, bool canSacrificeForCards, bool canUseCard, bool scaleUpWild)
	{
		this.audioScript = App.AudioScript;
		this.handCamera = handCamera;
		this.callBackTarget = callbackTarget;
		this.card = card;
		this.resTypes = resTypes;
		this.canSacrificeForCards = canSacrificeForCards;
		this.canUseCard = canUseCard;
		this.allowSpecial = callbackTarget.allowSacrifice(ResourceType.SPECIAL);
		this.allowCycle = callbackTarget.allowSacrifice(ResourceType.CARDS);
		this.scaleUpWild = scaleUpWild;
		this.setupIcons();
	}

	// Token: 0x060003CB RID: 971 RVA: 0x000048AC File Offset: 0x00002AAC
	public void Show()
	{
		this.SetEnabledRecursively(base.gameObject, true);
	}

	// Token: 0x060003CC RID: 972 RVA: 0x000048BB File Offset: 0x00002ABB
	public void Hide()
	{
		this.SetEnabledRecursively(base.gameObject, false);
	}

	// Token: 0x060003CD RID: 973 RVA: 0x000048CA File Offset: 0x00002ACA
	public string getTooltip()
	{
		return this.tooltip;
	}

	// Token: 0x060003CE RID: 974 RVA: 0x0002E91C File Offset: 0x0002CB1C
	private void SetEnabledRecursively(GameObject obj, bool enabled)
	{
		if (obj.renderer)
		{
			obj.renderer.enabled = enabled;
		}
		if (obj.collider)
		{
			obj.collider.enabled = enabled;
		}
		EffectPlayer component = obj.GetComponent<EffectPlayer>();
		if (component)
		{
			component.enabled = enabled;
		}
		foreach (object obj2 in obj.GetComponentInChildren<Transform>())
		{
			Transform transform = (Transform)obj2;
			this.SetEnabledRecursively(transform.gameObject, enabled);
		}
	}

	// Token: 0x060003CF RID: 975 RVA: 0x000048D2 File Offset: 0x00002AD2
	private void Start()
	{
		base.gameObject.layer = 9;
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x0002E9D8 File Offset: 0x0002CBD8
	private GameObject getIcon(string s)
	{
		foreach (GameObject gameObject in this.allIcons)
		{
			if (gameObject.name == s)
			{
				return gameObject;
			}
		}
		return null;
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x000048E1 File Offset: 0x00002AE1
	private bool hasIcon(string s)
	{
		return this.getIcon(s) != null;
	}

	// Token: 0x060003D2 RID: 978 RVA: 0x0002EA48 File Offset: 0x0002CC48
	public bool Raycast()
	{
		this.tooltip = null;
		if (this.iconClickedBool)
		{
			return false;
		}
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit raycastHit = default(RaycastHit);
			Ray ray = this.handCamera.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, ref raycastHit, this.handCamera.farClipPlane, 512))
			{
				if (this.hasIcon(raycastHit.collider.gameObject.name))
				{
					this.iconClicked(raycastHit.collider.gameObject.name);
					return true;
				}
				foreach (GameObject gameObject in this.allIcons)
				{
					Object.Destroy(gameObject);
				}
				this.allIcons.Clear();
				Object.Destroy(base.gameObject);
				return false;
			}
		}
		else
		{
			RaycastHit raycastHit2 = default(RaycastHit);
			Ray ray2 = this.handCamera.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray2, ref raycastHit2, this.handCamera.farClipPlane, 512))
			{
				Vector3 vector = this.GetIconScale(raycastHit2.collider.gameObject.name) * 1.1f;
				GameObject icon = this.getIcon(raycastHit2.collider.gameObject.name);
				if (icon != null)
				{
					this.tooltip = this.updateTooltip(icon.name);
					bool large = true;
					if ((!this.allowSpecial && icon.name == "special") || (!this.allowCycle && icon.name == "CARDS" && this.tooltip != null))
					{
						large = false;
					}
					this.ScaleIcon(icon, large);
					this.lastIcon = this.currentIcon;
					this.currentIcon = icon;
				}
				if (this.currentIcon != this.lastIcon && this.lastIcon != null)
				{
					this.ScaleIcon(this.lastIcon, false);
					this.lastIcon = null;
				}
				if (raycastHit2.collider.gameObject != this.currentIcon && this.currentIcon != null)
				{
					this.ScaleIcon(this.currentIcon, false);
					this.currentIcon = null;
					this.lastIcon = null;
				}
				return false;
			}
			foreach (GameObject icon2 in this.allIcons)
			{
				this.ScaleIcon(icon2, false);
			}
		}
		return false;
	}

	// Token: 0x060003D3 RID: 979 RVA: 0x0002ED24 File Offset: 0x0002CF24
	private string updateTooltip(string icon)
	{
		ResourceType? resourceType = CardActivator.resourceFromName(icon);
		if (resourceType == null)
		{
			return null;
		}
		return this.callBackTarget.getResourceTooltip(resourceType.Value);
	}

	// Token: 0x060003D4 RID: 980 RVA: 0x0002ED58 File Offset: 0x0002CF58
	private void ScaleIcon(GameObject icon, bool large)
	{
		if (icon != null)
		{
			Vector3 vector;
			if (icon.name == "play")
			{
				vector = this.GetIconScale(icon.name) * ((!large) ? 1f : 1.15f);
			}
			else
			{
				vector = this.GetIconScale(icon.name) * ((!large) ? 1f : 1.25f);
			}
			iTween.ScaleTo(icon, iTween.Hash(new object[]
			{
				"x",
				vector.x,
				"y",
				vector.y,
				"z",
				vector.z,
				"time",
				0.2f,
				"delay",
				0
			}));
		}
	}

	// Token: 0x060003D5 RID: 981 RVA: 0x0002EE58 File Offset: 0x0002D058
	private static ResourceType? resourceFromName(string name)
	{
		name = name.ToUpper();
		if (!Enum.IsDefined(typeof(ResourceType), name))
		{
			return default(ResourceType?);
		}
		return (ResourceType?)Enum.Parse(typeof(ResourceType), name, true);
	}

	// Token: 0x060003D6 RID: 982 RVA: 0x0002EEA4 File Offset: 0x0002D0A4
	public Vector3 GetIconScale(string iconName)
	{
		float num = (float)Screen.height / 9f;
		if (this.scaleUpWild)
		{
			ResourceType? resourceType = CardActivator.resourceFromName(iconName);
			if (resourceType != null && resourceType.Value.isResource())
			{
				num *= 0.6f;
			}
		}
		if (iconName == "bg" || CardActivator.resourceFromName(iconName) != null)
		{
			return new Vector3(0.045625f, 1f, 0.045f) * num;
		}
		if (iconName != null)
		{
			if (CardActivator.<>f__switch$map12 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("play", 0);
				dictionary.Add("magnify", 1);
				CardActivator.<>f__switch$map12 = dictionary;
			}
			int num2;
			if (CardActivator.<>f__switch$map12.TryGetValue(iconName, ref num2))
			{
				if (num2 == 0)
				{
					return new Vector3(13.5f, 1f, 9.9f) / 110f * num;
				}
				if (num2 == 1)
				{
					return new Vector3(0.08f, 1f, 0.07f) * num;
				}
			}
		}
		return new Vector3(1f, 1f, 1f) * num;
	}

	// Token: 0x060003D7 RID: 983 RVA: 0x0002EFF0 File Offset: 0x0002D1F0
	public void addConfirm(CardConfirmType confirmType)
	{
		if (!this.playIcon)
		{
			this.playIcon = this.createIcon(null, "play", new Vector3(base.transform.position.x, base.transform.position.y + (float)Screen.height * 0.085f, 1f));
			this.playIcon.renderer.enabled = false;
			this.playIcon.tag = "blinkable_cast";
			this.allIcons.Add(this.playIcon);
			Color color = ResourceColor.get(this.card.getCardType().getResource());
			if (confirmType == CardConfirmType.Cast)
			{
				GameObject gameObject = this.setupCastButton(this.playIcon.transform, "cast_button_runes_back", -2f);
				gameObject.renderer.material.color = color;
				this.setupCastButton(this.playIcon.transform, "cast_button_runes_front", -1f);
			}
			else
			{
				Vector3 vector;
				vector..ctor(0f, -0.04f * (float)Screen.height, 0f);
				this.playIcon.transform.localPosition = this.playIcon.transform.localPosition + vector;
				GameObject gameObject2 = this.setupCastButton(this.playIcon.transform, "cast_button_runes_back", -2f);
				gameObject2.renderer.material.color = Color.black;
				this.setupCastButton(this.playIcon.transform, "cast_button_runes_front", -1f);
			}
		}
	}

	// Token: 0x060003D8 RID: 984 RVA: 0x0002F18C File Offset: 0x0002D38C
	private GameObject setupCastButton(Transform parent, string name, float zOffset)
	{
		GameObject gameObject = new GameObject("castButton-" + name);
		gameObject.AddComponent<MeshRenderer>();
		gameObject.transform.parent = parent;
		UnityUtil.setLocalIdentity(gameObject.transform);
		gameObject.transform.Translate(0f, zOffset, 0f);
		gameObject.transform.localEulerAngles = new Vector3(90f, 180f, 0f);
		Vector3 baseScale = Vector3.one * 2f;
		EffectPlayer effectPlayer = gameObject.AddComponent<EffectPlayer>();
		effectPlayer.init("battlegui-cardglow_cast_target", 1, DefaultIEffectCallback.instance(), 91991, baseScale, true, string.Empty, 0);
		effectPlayer.layer = 9;
		effectPlayer.getAnimPlayer().setAnimationId(name);
		return gameObject;
	}

	// Token: 0x060003D9 RID: 985 RVA: 0x0002F248 File Offset: 0x0002D448
	private void setupIcons()
	{
		this.iconMat = Shaders.matMilkBurn();
		this.iconMat.renderQueue = 96500;
		if (this.canUseCard)
		{
			List<Vector3> list = new List<Vector3>();
			int num = this.resTypes.Length + ((!this.canSacrificeForCards) ? 0 : 1);
			float num2 = (float)Screen.height * 0.063f;
			float num3 = 0f;
			for (int i = 0; i < num; i++)
			{
				float num4 = ((float)(-(float)num) / 2f + 0.5f + (float)i) * num2;
				if (i < this.resTypes.Length && this.resTypes[i] == ResourceType.SPECIAL)
				{
					num3 += 0.2f * num2;
				}
				list.Add(new Vector3(base.transform.position.x + num4 + num3, base.transform.position.y, 1f));
			}
			for (int j = 0; j < list.Count; j++)
			{
				list[j] += new Vector3(-num3 / 2f, 0f, 0f);
			}
			Color color;
			color..ctor(0.35f, 0.35f, 0.35f, 1f);
			for (int k = 0; k < this.resTypes.Length; k++)
			{
				ResourceType resourceType = this.resTypes[k];
				string name = resourceType.ToString().ToLower();
				GameObject gameObject = this.createIcon(resourceType.battleIconFilename(), name, list[k]);
				this.allIcons.Add(gameObject);
				GameObject gameObject2 = this.createBgIcon(new Vector3(list[k].x, list[k].y, list[k].z + 1f));
				this.bgObjects.Add(gameObject2);
				if (!this.callBackTarget.allowSacrifice(resourceType))
				{
					gameObject.renderer.material.color = color;
					gameObject2.renderer.material.color = color;
				}
				GameObject gameObject3 = gameObject2;
				string tag = "blinkable_resource";
				gameObject.tag = tag;
				gameObject3.tag = tag;
			}
			if (this.canSacrificeForCards)
			{
				this.cycleIcon = this.createIcon(ResourceType.CARDS.battleIconFilename(), "CARDS", list[this.resTypes.Length]);
				GameObject gameObject4 = this.createBgIcon(new Vector3(list[this.resTypes.Length].x, list[this.resTypes.Length].y, list[this.resTypes.Length].z + 1f));
				if (!this.callBackTarget.allowSacrifice(ResourceType.CARDS))
				{
					this.cycleIcon.renderer.material.color = color;
					gameObject4.renderer.material.color = color;
				}
				GameObject gameObject5 = gameObject4;
				string tag = "blinkable_cycle";
				this.cycleIcon.tag = tag;
				gameObject5.tag = tag;
				this.allIcons.Add(this.cycleIcon);
				this.bgObjects.Add(gameObject4);
			}
		}
		this.magnifyIcon = this.createIcon("Icons/icon_magnify_small", "magnify", new Vector3(base.transform.position.x, base.transform.position.y - (float)Screen.height * 0.255f, 1f));
		this.allIcons.Add(this.magnifyIcon);
	}

	// Token: 0x060003DA RID: 986 RVA: 0x0002F610 File Offset: 0x0002D810
	private GameObject createIcon(string fn, string name, Vector3 pos)
	{
		GameObject gameObject = PrimitiveFactory.createPlane();
		gameObject.transform.localScale = this.GetIconScale(name);
		gameObject.transform.position = pos;
		gameObject.renderer.material = this.iconMat;
		if (fn != null)
		{
			gameObject.renderer.material.mainTexture = ResourceManager.LoadTexture(fn);
		}
		gameObject.transform.eulerAngles = new Vector3(90f, 180f, 0f);
		gameObject.transform.parent = base.transform;
		gameObject.name = name;
		gameObject.layer = 9;
		return gameObject;
	}

	// Token: 0x060003DB RID: 987 RVA: 0x000048F0 File Offset: 0x00002AF0
	private GameObject createBgIcon(Vector3 pos)
	{
		return this.createIcon("BattleUI/battlegui_resource_bg", "bg", pos);
	}

	// Token: 0x060003DC RID: 988 RVA: 0x0002F6B0 File Offset: 0x0002D8B0
	private void iconClicked(string icon)
	{
		ResourceType? resourceType = CardActivator.resourceFromName(icon);
		if (resourceType != null && !this.callBackTarget.allowSacrifice(resourceType.Value))
		{
			return;
		}
		this.audioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
		if (icon == "magnify")
		{
			this.callBackTarget.magnifyCard(this.card);
			return;
		}
		if (icon == "play" && !this.callBackTarget.allowPlayCard())
		{
			return;
		}
		this.iconClickedBool = true;
		this.sacIcon = icon;
		float num = 0f;
		this.selectedIcon = icon;
		float num2 = 1.3f;
		float num3 = (float)Screen.height * 0.007f;
		Vector2 vector;
		vector..ctor(num3 * num2, num3);
		GameObject icon2 = this.getIcon(icon);
		foreach (GameObject gameObject in this.allIcons)
		{
			if (gameObject != icon2)
			{
				iTween.ScaleTo(gameObject, iTween.Hash(new object[]
				{
					"x",
					0,
					"z",
					0,
					"time",
					0.9f,
					"delay",
					num += 0.1f,
					"oncompletetarget",
					base.gameObject,
					"oncomplete",
					"tweenComplete",
					"oncompleteparams",
					gameObject
				}));
			}
		}
		if (icon == "play")
		{
			this.callBackTarget.confirmPlayCard(this.card);
			iTween.ScaleTo(this.playIcon, iTween.Hash(new object[]
			{
				"x",
				0,
				"y",
				0,
				"z",
				0,
				"time",
				0.5f,
				"delay",
				0.3f,
				"oncompletetarget",
				base.gameObject,
				"oncomplete",
				"tweenComplete",
				"oncompleteparams",
				this.playIcon
			}));
		}
		else if (icon == "CARDS")
		{
			this.callBackTarget.sacrificeCard(this.card, ResourceType.CARDS);
			this.resourceTweenComplete(ResourceType.CARDS);
			iTween.ScaleTo(this.cycleIcon, iTween.Hash(new object[]
			{
				"x",
				0,
				"y",
				0,
				"z",
				0,
				"time",
				0.5f,
				"delay",
				0.3f,
				"oncompletetarget",
				base.gameObject,
				"oncomplete",
				"tweenComplete",
				"oncompleteparams",
				this.cycleIcon
			}));
		}
		else
		{
			ResourceType value = CardActivator.resourceFromName(icon).Value;
			this.callBackTarget.sacrificeCard(this.card, value);
			GameObject gameObject2 = GameObject.Find(this.selectedIcon);
			this.cleanBlinkable(gameObject2);
			Vector2 center = this.callBackTarget.getSacrificeDestRect(value).center;
			this.callBackTarget.glowResourceIcon(value, gameObject2.transform.position);
			iTween.MoveTo(gameObject2, iTween.Hash(new object[]
			{
				"x",
				(float)(-(float)Screen.width / 2) + (float)Screen.height * 0.07f,
				"y",
				(float)(-(float)Screen.height) * 0.4f,
				"time",
				0.5f,
				"delay",
				0,
				"oncompletetarget",
				base.gameObject,
				"oncomplete",
				"resourceTweenComplete",
				"oncompleteparams",
				value
			}));
			iTween.ScaleTo(gameObject2, iTween.Hash(new object[]
			{
				"x",
				vector.x,
				"z",
				vector.y,
				"time",
				0.3f,
				"delay",
				0
			}));
			iTween.FadeTo(gameObject2, 0f, 0.3f);
		}
		foreach (GameObject target in this.bgObjects)
		{
			iTween.FadeTo(target, 0f, 0.3f);
		}
	}

	// Token: 0x060003DD RID: 989 RVA: 0x0002FC04 File Offset: 0x0002DE04
	public void tweenComplete(GameObject obj)
	{
		this.iconsDestroyed++;
		if (this.iconsDestroyed == 1 && this.canUseCard)
		{
			Object.Destroy(obj);
		}
		if (this.iconsDestroyed < this.resTypes.Length + 1)
		{
			Object.Destroy(obj);
		}
		if (this.iconsDestroyed == this.resTypes.Length + 1)
		{
			Object.Destroy(obj);
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060003DE RID: 990 RVA: 0x0002FC80 File Offset: 0x0002DE80
	public void killActivator()
	{
		if (!this.iconClickedBool)
		{
			foreach (GameObject gameObject in this.allIcons)
			{
				Object.Destroy(gameObject);
			}
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060003DF RID: 991 RVA: 0x00004903 File Offset: 0x00002B03
	private void resourceTweenComplete(ResourceType resourceType)
	{
		this.callBackTarget.resourceTweenComplete(resourceType);
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x0002FCF0 File Offset: 0x0002DEF0
	private void cleanBlinkable(GameObject obj)
	{
		Blink component = obj.GetComponent<Blink>();
		if (component != null)
		{
			Object.Destroy(component);
		}
		obj.tag = null;
	}

	// Token: 0x04000265 RID: 613
	private GameObject playIcon;

	// Token: 0x04000266 RID: 614
	private GameObject magnifyIcon;

	// Token: 0x04000267 RID: 615
	private GameObject cycleIcon;

	// Token: 0x04000268 RID: 616
	private List<GameObject> allIcons = new List<GameObject>();

	// Token: 0x04000269 RID: 617
	private ICardActivatorCallback callBackTarget;

	// Token: 0x0400026A RID: 618
	private ResourceType[] resTypes;

	// Token: 0x0400026B RID: 619
	private CardView card;

	// Token: 0x0400026C RID: 620
	private GameObject currentIcon;

	// Token: 0x0400026D RID: 621
	private GameObject lastIcon;

	// Token: 0x0400026E RID: 622
	private Vector3 lastIconScale;

	// Token: 0x0400026F RID: 623
	private bool iconClickedBool;

	// Token: 0x04000270 RID: 624
	private bool canSacrificeForCards;

	// Token: 0x04000271 RID: 625
	private bool canUseCard;

	// Token: 0x04000272 RID: 626
	private string selectedIcon;

	// Token: 0x04000273 RID: 627
	private AudioScript audioScript;

	// Token: 0x04000274 RID: 628
	private Camera handCamera;

	// Token: 0x04000275 RID: 629
	private Material iconMat;

	// Token: 0x04000276 RID: 630
	private List<GameObject> bgObjects = new List<GameObject>();

	// Token: 0x04000277 RID: 631
	private string tooltip;

	// Token: 0x04000278 RID: 632
	private bool allowSpecial;

	// Token: 0x04000279 RID: 633
	private bool allowCycle;

	// Token: 0x0400027A RID: 634
	private bool scaleUpWild;

	// Token: 0x0400027B RID: 635
	private string sacIcon;

	// Token: 0x0400027C RID: 636
	private int iconsDestroyed;
}
