using System;
using System.Collections;
using System.Collections.Generic;
using Gui;
using UnityEngine;

// Token: 0x020000D2 RID: 210
public class BattleModeUI : MonoBehaviour
{
	// Token: 0x06000712 RID: 1810 RVA: 0x0003F834 File Offset: 0x0003DA34
	public void Start()
	{
		this.gui3d = new Gui3D(this.uiCamera);
		this.gui3d.setDefaultMaterial(Shaders.matMilkBurn());
		this.gui3d.setLayer(9);
		Material material = Shaders.matMilkBurn();
		material.mainTexture = ResourceManager.LoadTexture("BattleUI/battlegui_end_bg");
		this.endTurnBG = PrimitiveFactory.createPlane();
		this.endTurnBG.layer = 9;
		this.endTurnBG.transform.localScale = new Vector3((float)Screen.height * 0.15f / 10f * 197f / 238f, 1f, (float)Screen.height * 0.15f / 10f);
		this.endTurnBG.transform.eulerAngles = new Vector3(90f, 180f, 0f);
		this.endTurnBG.name = "End_Turn_BG";
		this.endTurnBG.transform.parent = this.uiCamera.transform;
		this.endTurnBG.transform.localPosition = new Vector3((float)(-(float)Screen.width) * 0.5f + (float)Screen.height * 0.21f, (float)(-(float)Screen.height) * 0.43f, 6f);
		this.endTurnBG.renderer.material = material;
		this.endTurnBG.renderer.enabled = false;
		this.endTurnBG.collider.enabled = false;
		Material material2 = Shaders.matMilkBurn();
		material2.mainTexture = ResourceManager.LoadTexture("BattleUI/battlegui_end");
		this.endTurnButton = PrimitiveFactory.createPlane();
		this.endTurnButton.layer = 9;
		this.endTurnButton.transform.localScale = new Vector3((float)Screen.height * 0.08f / 10f * 110f / 95f, 1f, (float)Screen.height * 0.08f / 10f);
		this.endTurnButton.transform.eulerAngles = new Vector3(90f, 180f, 0f);
		this.endTurnButton.name = "End_Turn";
		this.endTurnButton.transform.parent = this.uiCamera.transform;
		this.endTurnButton.transform.localPosition = new Vector3((float)(-(float)Screen.width) * 0.5f + (float)Screen.height * 0.21f, (float)(-(float)Screen.height) * 0.4325f, 5f);
		this.endTurnButton.renderer.material = material2;
		this.originalEndTurnRotation = this.endTurnButton.transform.rotation;
		this.numberBoxWidth = (float)Screen.height * 0.12f;
		this.numberBoxHeight = (float)Screen.height * 0.04f;
		this.leftHandSize = new NumberBox(this.gui3d, new Rect(this.numberBoxWidth + (float)Screen.height * 0.04f, (float)Screen.height - this.numberBoxHeight * 1.2f - (float)Screen.height * 0.01f, this.numberBoxWidth, this.numberBoxHeight), ResourceManager.LoadTexture("BattleUI/battlegui_handsize"), Color.white, true);
		this.rightHandSize = new NumberBox(this.gui3d, new Rect((float)Screen.width - this.numberBoxWidth * 2f - (float)Screen.height * 0.04f, (float)Screen.height - this.numberBoxHeight * 1.2f - (float)Screen.height * 0.01f, this.numberBoxWidth, this.numberBoxHeight), ResourceManager.LoadTexture("BattleUI/battlegui_handsize"), Color.white, true);
		this.leftStack = new NumberBox(this.gui3d, this.GetResourceRect(0, true), ResourceManager.LoadTexture("BattleUI/deck_and_discard"), Color.white, false);
		this.leftStack.SetAlwaysShowBoth(true);
		this.rightStack = new NumberBox(this.gui3d, this.GetResourceRect(0, false), ResourceManager.LoadTexture("BattleUI/deck_and_discard"), Color.white, true);
		this.rightStack.SetAlwaysShowBoth(true);
		this.endTurnButton.renderer.enabled = false;
		this.endTurnButton.collider.enabled = false;
	}

	// Token: 0x06000713 RID: 1811 RVA: 0x0003FC64 File Offset: 0x0003DE64
	public void Init(IBattleModeUICallback callback, ITutorialBlockCheck blockCheck, GameMode gameMode, AvatarInfo leftAvatarInfo, AvatarInfo rightAvatarInfo, Color gradientColor)
	{
		this.callback = callback;
		this.blockCheck = blockCheck;
		this.gameMode = gameMode;
		this.leftAvatar = Avatar.Create(leftAvatarInfo);
		this.rightAvatar = Avatar.Create(rightAvatarInfo);
		this.leftAvatarSize = this.GetAvatarSize(1f);
		this.rightAvatarSize = this.GetAvatarSize(1f);
		this.leftAvatarX = -this.leftAvatarSize.x;
		this.rightAvatarX = (float)Screen.width + this.rightAvatarSize.x;
		this.isInited = true;
		this.gradientColor = gradientColor;
		App.ChatUI.SetLeftRightWidths(0.3f, 0.29700002f);
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x000066F6 File Offset: 0x000048F6
	public Avatar GetLeftAvatar()
	{
		return this.leftAvatar;
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x000066FE File Offset: 0x000048FE
	public Avatar GetRightAvatar()
	{
		return this.rightAvatar;
	}

	// Token: 0x06000716 RID: 1814 RVA: 0x0003FD10 File Offset: 0x0003DF10
	public PlayerAssets GetResources(bool isLeft)
	{
		if (isLeft)
		{
			return new PlayerAssets(this.leftAvailable, this.leftMax, this.leftHandSize.GetFirst());
		}
		return new PlayerAssets(this.rightAvailable, this.rightMax, this.rightHandSize.GetFirst());
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x00006706 File Offset: 0x00004906
	public ResourceGroup GetLeftPlayerResources()
	{
		return this.leftAvailable;
	}

	// Token: 0x06000718 RID: 1816 RVA: 0x0000670E File Offset: 0x0000490E
	public ResourceGroup GetRightPlayerResources()
	{
		return this.rightAvailable;
	}

	// Token: 0x06000719 RID: 1817 RVA: 0x00006716 File Offset: 0x00004916
	public void ShowLeftHandSize()
	{
		this.showLeftHandSize = true;
	}

	// Token: 0x0600071A RID: 1818 RVA: 0x0003FD5C File Offset: 0x0003DF5C
	public void UpdateData(PlayerAssets assets, bool isPlayer, bool leftPlayer)
	{
		if (!isPlayer)
		{
			if (leftPlayer)
			{
				this.leftHandSize.SetFirst(assets.handSize);
			}
			else
			{
				this.rightHandSize.SetFirst(assets.handSize);
			}
		}
		this.UpdateData(assets.availableResources, assets.outputResources, leftPlayer);
	}

	// Token: 0x0600071B RID: 1819 RVA: 0x0000671F File Offset: 0x0000491F
	public void Hurt(int damage, bool leftPlayer)
	{
		base.StartCoroutine(this.AvatarLerp(leftPlayer));
	}

	// Token: 0x0600071C RID: 1820 RVA: 0x0000672F File Offset: 0x0000492F
	public void UpdateStackSize(int library, int graveyard, bool leftPlayer)
	{
		if (leftPlayer)
		{
			this.leftStack.SetBoth(library, graveyard);
		}
		else
		{
			this.rightStack.SetBoth(library, graveyard);
		}
	}

	// Token: 0x0600071D RID: 1821 RVA: 0x0003FDB0 File Offset: 0x0003DFB0
	public void UpdateData(ResourceGroup available, ResourceGroup max, bool leftPlayer)
	{
		if (leftPlayer)
		{
			this.leftAvailable = available;
			this.leftMax = max;
		}
		else
		{
			this.rightAvailable = available;
			this.rightMax = max;
		}
		Dictionary<ResourceType, NumberBox> dictionary = (!leftPlayer) ? this.rightNumberDisplays : this.leftNumberDisplays;
		foreach (ResourceType resourceType in CollectionUtil.enumValues<ResourceType>())
		{
			if (dictionary.ContainsKey(resourceType))
			{
				dictionary[resourceType].SetBoth(available.get(resourceType), max.get(resourceType));
			}
			else if (max.has(resourceType) || available.has(resourceType))
			{
				NumberBox numberBox = new NumberBox(this.gui3d, this.GetResourceRect(dictionary.Count + 1, leftPlayer), ResourceManager.LoadTexture(resourceType.battleIconFilename()), ResourceColor.get(resourceType), !leftPlayer);
				numberBox.resourceCounterGo = GameObject.Find("blinkdummy_resourcecounter");
				numberBox.SetBoth(available.get(resourceType), max.get(resourceType));
				dictionary[resourceType] = numberBox;
				ResourceType? resourceType2 = this.pendingSacrificedFor;
				if (resourceType2 != null && resourceType == this.pendingSacrificedFor.Value)
				{
					this.markSacrificedFor(leftPlayer, resourceType);
				}
			}
		}
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x0003FEEC File Offset: 0x0003E0EC
	private Rect GetResourceRect(int index, bool leftPlayer)
	{
		return new Rect((!leftPlayer) ? ((float)Screen.width - this.numberBoxWidth - (float)Screen.height * 0.02f) : ((float)Screen.height * 0.02f), (float)Screen.height - this.numberBoxHeight * (float)(index + 1) * 1.2f - (float)Screen.height * 0.01f, this.numberBoxWidth, this.numberBoxHeight);
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x0003FF60 File Offset: 0x0003E160
	public Rect GetResourceIconRect(ResourceType resource, bool leftPlayer)
	{
		Dictionary<ResourceType, NumberBox> dictionary = (!leftPlayer) ? this.rightNumberDisplays : this.leftNumberDisplays;
		if (dictionary.ContainsKey(resource))
		{
			return dictionary[resource].GetIconRect();
		}
		Texture2D icon = ResourceManager.LoadTexture(resource.battleIconFilename());
		return new NumberBox(this.gui3d, this.GetResourceRect(dictionary.Count + 1, leftPlayer), icon, Color.white, !leftPlayer).GetIconRect();
	}

	// Token: 0x06000720 RID: 1824 RVA: 0x00006756 File Offset: 0x00004956
	public void MoveInAvatars()
	{
		base.StartCoroutine(this.MoveAvatarsCoroutine((float)(-(float)Screen.height) * 0.16f, (float)Screen.height * 0.16f));
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x0000677E File Offset: 0x0000497E
	public void MoveOutAvatars()
	{
		base.StartCoroutine(this.MoveAvatarsCoroutine((float)Screen.height * 0.16f, (float)(-(float)Screen.height) * 0.16f));
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x0003FFD4 File Offset: 0x0003E1D4
	public void ScaleAvatar(bool enlarge, bool leftPlayer)
	{
		float to = (!enlarge) ? 0.7f : 1.2f;
		base.StartCoroutine(this.ScaleAvatarCoroutine(to, leftPlayer));
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x000067A6 File Offset: 0x000049A6
	public void BeginTurn(bool leftPlayer)
	{
		if (leftPlayer)
		{
			this.ShowEndTurn(true);
		}
		else
		{
			this.ShowEndTurn(false);
		}
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x00040008 File Offset: 0x0003E208
	private IEnumerator MoveAvatarsCoroutine(float fromOffset, float toOffset)
	{
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime * 2f;
			if (t > 1f)
			{
				t = 1f;
			}
			float factor = t * t * (3f - 2f * t);
			this.leftAvatarX = fromOffset + (toOffset - fromOffset) * factor;
			this.rightAvatarX = (float)Screen.width - (fromOffset + (toOffset - fromOffset) * factor);
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x00040040 File Offset: 0x0003E240
	private IEnumerator ScaleAvatarCoroutine(float to, bool leftPlayer)
	{
		float from = ((!leftPlayer) ? this.rightAvatarSize : this.leftAvatarSize).x / this.GetAvatarSize(1f).x;
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime * 2.5f;
			if (t > 1f)
			{
				t = 1f;
			}
			float factor = t * t * (3f - 2f * t);
			Vector2 avatarSize = this.GetAvatarSize(from + (to - from) * factor);
			if (leftPlayer)
			{
				this.leftAvatarSize = avatarSize;
			}
			else
			{
				this.rightAvatarSize = avatarSize;
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x00040078 File Offset: 0x0003E278
	private IEnumerator AvatarLerp(bool leftPlayer)
	{
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime * 4f;
			if (t > 1f)
			{
				t = 1f;
			}
			float realT = (t <= 0.5f) ? t : (1f - t);
			float factor = 4f * (realT - realT * realT);
			if (leftPlayer)
			{
				this.leftAvatarLerp = 0.5f * factor;
			}
			else
			{
				this.rightAvatarLerp = 0.5f * factor;
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x000400A4 File Offset: 0x0003E2A4
	private Vector2 GetAvatarSize(float scale)
	{
		float num = (float)Screen.height * 0.7f;
		float num2 = num * 567f / 991f;
		return new Vector2(num2 * scale, num * scale);
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x000400D8 File Offset: 0x0003E2D8
	private void Raycast()
	{
		Vector3 mousePosition = Input.mousePosition;
		RaycastHit raycastHit = default(RaycastHit);
		Ray ray = this.uiCamera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, ref raycastHit, this.uiCamera.farClipPlane, 512))
		{
			string name = raycastHit.collider.gameObject.name;
			if (name != null)
			{
				if (BattleModeUI.<>f__switch$map13 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
					dictionary.Add("End_Turn", 0);
					BattleModeUI.<>f__switch$map13 = dictionary;
				}
				int num;
				if (BattleModeUI.<>f__switch$map13.TryGetValue(name, ref num))
				{
					if (num == 0)
					{
						if (!this.blockCheck.isInputBlocked() && this.callback.allowEndTurn())
						{
							if (Input.GetMouseButtonDown(0))
							{
								this.endTurnButton.renderer.material.mainTexture = ResourceManager.LoadTexture("BattleUI/battlegui_end_mousedown");
								this.holdingEndTurn = true;
							}
							if (Input.GetMouseButtonUp(0) && this.holdingEndTurn)
							{
								this.endTurnButton.renderer.material.mainTexture = ResourceManager.LoadTexture("BattleUI/battlegui_end");
								this.holdingEndTurn = false;
								this.callback.endturnPressed();
								this.ShowEndTurn(false);
							}
							if (Input.GetMouseButton(0))
							{
							}
							if (!Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonUp(0))
							{
								this.endTurnButton.renderer.material.mainTexture = ResourceManager.LoadTexture("BattleUI/battlegui_end_mouseover");
							}
						}
						goto IL_1B2;
					}
				}
			}
			this.endTurnButton.renderer.material.mainTexture = ResourceManager.LoadTexture("BattleUI/battlegui_end");
			this.holdingEndTurn = false;
			IL_1B2:;
		}
		else
		{
			this.endTurnButton.renderer.material.mainTexture = ResourceManager.LoadTexture("BattleUI/battlegui_end");
			this.holdingEndTurn = false;
		}
		if (Input.GetMouseButtonUp(0))
		{
			this.holdingEndTurn = false;
		}
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x000402D4 File Offset: 0x0003E4D4
	private void Update()
	{
		this.Raycast();
		this.gui3d.frameBegin();
		this.gui3d.setDepth(10f);
		this.gui3d.DrawTexture(new Rect(0f, (float)Screen.height * 0.7f, (float)Screen.width, (float)Screen.height * 0.3f), ResourceManager.LoadTexture("BattleUI/battlegui_gradient_white"));
		this.gui3d.GetLastMaterial().color = this.gradientColor;
		bool flag = false;
		if (flag)
		{
			new ScrollsFrame(new Rect(0f, (float)Screen.height * 0.835f, (float)Screen.height * 0.3f, (float)Screen.height * 0.165f)).SetGui(this.gui3d).AddNinePatch(ScrollsFrame.Border.DARK_DOUBLE_CURVED, NinePatch.Patches.TOP_RIGHT | NinePatch.Patches.CENTER | NinePatch.Patches.BOTTOM_LEFT).AddNinePatch(ScrollsFrame.Border.DARK_DOUBLE_SHARP, NinePatch.Patches.TOP_LEFT | NinePatch.Patches.TOP | NinePatch.Patches.LEFT | NinePatch.Patches.CENTER | NinePatch.Patches.RIGHT | NinePatch.Patches.BOTTOM | NinePatch.Patches.BOTTOM_RIGHT).Draw();
			new ScrollsFrame(new Rect((float)Screen.width - (float)Screen.height * 0.3f, (float)Screen.height * 0.835f, (float)Screen.height * 0.3f, (float)Screen.height * 0.165f)).SetGui(this.gui3d).AddNinePatch(ScrollsFrame.Border.DARK_DOUBLE_CURVED, NinePatch.Patches.TOP_LEFT | NinePatch.Patches.CENTER | NinePatch.Patches.BOTTOM_RIGHT).AddNinePatch(ScrollsFrame.Border.DARK_DOUBLE_SHARP, NinePatch.Patches.TOP | NinePatch.Patches.TOP_RIGHT | NinePatch.Patches.LEFT | NinePatch.Patches.CENTER | NinePatch.Patches.RIGHT | NinePatch.Patches.BOTTOM_LEFT | NinePatch.Patches.BOTTOM).Draw();
		}
		else
		{
			new ScrollsFrame(new Rect(0f, (float)Screen.height * 0.835f, (float)Screen.height * 0.3f, (float)Screen.height * 0.165f)).SetGui(this.gui3d).AddNinePatch(ScrollsFrame.Border.DARK_DOUBLE_CURVED, NinePatch.Patches.CENTER | NinePatch.Patches.BOTTOM_LEFT).AddNinePatch(ScrollsFrame.Border.DARK_DOUBLE_SHARP, NinePatch.Patches.TOP_LEFT | NinePatch.Patches.TOP | NinePatch.Patches.TOP_RIGHT | NinePatch.Patches.LEFT | NinePatch.Patches.CENTER | NinePatch.Patches.RIGHT | NinePatch.Patches.BOTTOM | NinePatch.Patches.BOTTOM_RIGHT).Draw();
			new ScrollsFrame(new Rect((float)Screen.width - (float)Screen.height * 0.3f, (float)Screen.height * 0.835f, (float)Screen.height * 0.3f, (float)Screen.height * 0.165f)).SetGui(this.gui3d).AddNinePatch(ScrollsFrame.Border.DARK_DOUBLE_CURVED, NinePatch.Patches.CENTER | NinePatch.Patches.BOTTOM_RIGHT).AddNinePatch(ScrollsFrame.Border.DARK_DOUBLE_SHARP, NinePatch.Patches.TOP_LEFT | NinePatch.Patches.TOP | NinePatch.Patches.TOP_RIGHT | NinePatch.Patches.LEFT | NinePatch.Patches.CENTER | NinePatch.Patches.RIGHT | NinePatch.Patches.BOTTOM_LEFT | NinePatch.Patches.BOTTOM).Draw();
			new ScrollsFrame(new Rect((float)Screen.height * 0.3f, (float)Screen.height * 0.86f, (float)Screen.width - (float)Screen.height * 0.6f, (float)Screen.height * 0.14f)).SetGui(this.gui3d).AddNinePatch(ScrollsFrame.Border.DARK_DOUBLE_CURVED, NinePatch.Patches.TOP_LEFT | NinePatch.Patches.TOP_RIGHT | NinePatch.Patches.CENTER).AddNinePatch(ScrollsFrame.Border.DARK_DOUBLE_SHARP, NinePatch.Patches.TOP | NinePatch.Patches.LEFT | NinePatch.Patches.CENTER | NinePatch.Patches.RIGHT | NinePatch.Patches.BOTTOM_LEFT | NinePatch.Patches.BOTTOM | NinePatch.Patches.BOTTOM_RIGHT).Draw();
		}
		if (this.isInited)
		{
			Rect rect;
			rect..ctor(this.leftAvatarX - this.leftAvatarSize.x / 2f, (float)Screen.height * 0.78f - this.leftAvatarSize.y / 5.5f, this.leftAvatarSize.x, this.leftAvatarSize.y);
			Color color;
			color..ctor(1f, 1f - this.leftAvatarLerp, 1f - this.leftAvatarLerp);
			this.leftAvatar.draw(this.gui3d, rect, color, false);
			Rect rect2;
			rect2..ctor(this.rightAvatarX - this.rightAvatarSize.x / 2f, (float)Screen.height * 0.78f - this.rightAvatarSize.y / 5.5f, this.rightAvatarSize.x, this.rightAvatarSize.y);
			Color color2;
			color2..ctor(1f, 1f - this.rightAvatarLerp, 1f - this.rightAvatarLerp);
			this.rightAvatar.draw(this.gui3d, rect2, color2, true);
		}
		foreach (NumberBox numberBox in this.leftNumberDisplays.Values)
		{
			numberBox.Draw();
		}
		foreach (NumberBox numberBox2 in this.rightNumberDisplays.Values)
		{
			numberBox2.Draw();
		}
		this.leftStack.Draw();
		this.rightStack.Draw();
		if (this.showLeftHandSize)
		{
			this.leftHandSize.Draw();
		}
		this.rightHandSize.Draw();
		this.gui3d.frameEnd();
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x000067C1 File Offset: 0x000049C1
	private void ShowEndTurn(bool show)
	{
		base.StopCoroutine("FadeInEndTurn");
		base.StopCoroutine("FadeOutEndTurn");
		if (show)
		{
			base.StartCoroutine("FadeInEndTurn");
		}
		else
		{
			base.StartCoroutine("FadeOutEndTurn");
		}
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x0004074C File Offset: 0x0003E94C
	private IEnumerator FadeInEndTurn()
	{
		this.endTurnButton.renderer.enabled = true;
		this.endTurnButton.collider.enabled = true;
		this.endTurnBG.renderer.enabled = true;
		float from = this.endTurnButton.renderer.material.color.a;
		float to = 1f;
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime * 2f;
			if (t > 1f)
			{
				t = 1f;
			}
			float factor = t * t * (3f - 2f * t);
			Color color = new Color(1f, 1f, 1f, from + (to - from) * factor);
			this.endTurnButton.renderer.material.color = color;
			this.endTurnBG.renderer.material.color = color;
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x00040768 File Offset: 0x0003E968
	private IEnumerator FadeOutEndTurn()
	{
		this.endTurnButton.collider.enabled = false;
		Color origColor = this.endTurnButton.renderer.material.color;
		float from = origColor.a;
		float to = 0f;
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime * 2f;
			if (t > 1f)
			{
				t = 1f;
			}
			float factor = t * t * (3f - 2f * t);
			Color color = ColorUtil.GetWithAlpha(origColor, from + (to - from) * factor);
			this.endTurnButton.renderer.material.color = color;
			this.endTurnBG.renderer.material.color = color;
			this.endTurnButton.transform.rotation = this.originalEndTurnRotation;
			this.endTurnButton.transform.Rotate(new Vector3(0f, 1f, 0f), -183.3465f * factor);
			yield return null;
		}
		this.endTurnButton.transform.rotation = this.originalEndTurnRotation;
		this.endTurnButton.renderer.enabled = false;
		this.endTurnBG.renderer.enabled = false;
		yield break;
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x00040784 File Offset: 0x0003E984
	public void markSacrificedFor(bool isLeft, ResourceType resource)
	{
		if (resource.isCards())
		{
			((!isLeft) ? this.rightStack : this.leftStack).SetSelected(true);
			return;
		}
		Dictionary<ResourceType, NumberBox> dictionary = (!isLeft) ? this.rightNumberDisplays : this.leftNumberDisplays;
		bool flag = false;
		foreach (KeyValuePair<ResourceType, NumberBox> keyValuePair in dictionary)
		{
			if (keyValuePair.Key == resource)
			{
				keyValuePair.Value.SetSelected(true);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			this.pendingSacrificedFor = new ResourceType?(resource);
		}
	}

	// Token: 0x0600072E RID: 1838 RVA: 0x00040848 File Offset: 0x0003EA48
	public void clearSacrificedFor(bool isLeft)
	{
		this.pendingSacrificedFor = default(ResourceType?);
		Dictionary<ResourceType, NumberBox> dictionary = (!isLeft) ? this.rightNumberDisplays : this.leftNumberDisplays;
		((!isLeft) ? this.rightStack : this.leftStack).SetSelected(false);
		foreach (NumberBox numberBox in dictionary.Values)
		{
			numberBox.SetSelected(false);
		}
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x000408E8 File Offset: 0x0003EAE8
	public void ResetTempReductions()
	{
		foreach (NumberBox numberBox in this.leftNumberDisplays.Values)
		{
			numberBox.SetTemporaryModifier(0);
		}
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x00040948 File Offset: 0x0003EB48
	public void SetTempReduction(ResourceGroup resourcesAfter)
	{
		foreach (KeyValuePair<ResourceType, int> keyValuePair in resourcesAfter)
		{
			if (this.leftNumberDisplays.ContainsKey(keyValuePair.Key))
			{
				int num = this.GetLeftPlayerResources().get(keyValuePair.Key);
				int value = keyValuePair.Value;
				if (num > value)
				{
					this.leftNumberDisplays[keyValuePair.Key].SetTemporaryModifier(value - num);
				}
			}
		}
	}

	// Token: 0x04000518 RID: 1304
	public const int UI_LAYER = 9;

	// Token: 0x04000519 RID: 1305
	private const float SideHeightPercent = 0.3f;

	// Token: 0x0400051A RID: 1306
	[SerializeField]
	private Camera uiCamera;

	// Token: 0x0400051B RID: 1307
	private Gui3D gui3d;

	// Token: 0x0400051C RID: 1308
	private GameMode gameMode;

	// Token: 0x0400051D RID: 1309
	private Vector2 leftAvatarSize;

	// Token: 0x0400051E RID: 1310
	private Vector2 rightAvatarSize;

	// Token: 0x0400051F RID: 1311
	private float leftAvatarX;

	// Token: 0x04000520 RID: 1312
	private float rightAvatarX;

	// Token: 0x04000521 RID: 1313
	private GameObject endTurnBG;

	// Token: 0x04000522 RID: 1314
	private GameObject endTurnButton;

	// Token: 0x04000523 RID: 1315
	private IBattleModeUICallback callback;

	// Token: 0x04000524 RID: 1316
	private ITutorialBlockCheck blockCheck;

	// Token: 0x04000525 RID: 1317
	private bool holdingEndTurn;

	// Token: 0x04000526 RID: 1318
	private ResourceGroup leftAvailable = new ResourceGroup();

	// Token: 0x04000527 RID: 1319
	private ResourceGroup leftMax = new ResourceGroup();

	// Token: 0x04000528 RID: 1320
	private ResourceGroup rightAvailable = new ResourceGroup();

	// Token: 0x04000529 RID: 1321
	private ResourceGroup rightMax = new ResourceGroup();

	// Token: 0x0400052A RID: 1322
	private Dictionary<ResourceType, NumberBox> leftNumberDisplays = new Dictionary<ResourceType, NumberBox>();

	// Token: 0x0400052B RID: 1323
	private Dictionary<ResourceType, NumberBox> rightNumberDisplays = new Dictionary<ResourceType, NumberBox>();

	// Token: 0x0400052C RID: 1324
	private bool showLeftHandSize;

	// Token: 0x0400052D RID: 1325
	private NumberBox leftHandSize;

	// Token: 0x0400052E RID: 1326
	private NumberBox rightHandSize;

	// Token: 0x0400052F RID: 1327
	private float numberBoxWidth;

	// Token: 0x04000530 RID: 1328
	private float numberBoxHeight;

	// Token: 0x04000531 RID: 1329
	private Avatar leftAvatar;

	// Token: 0x04000532 RID: 1330
	private Avatar rightAvatar;

	// Token: 0x04000533 RID: 1331
	[SerializeField]
	private Color gradientColor = Color.black;

	// Token: 0x04000534 RID: 1332
	private float leftAvatarLerp;

	// Token: 0x04000535 RID: 1333
	private float rightAvatarLerp;

	// Token: 0x04000536 RID: 1334
	private bool isInited;

	// Token: 0x04000537 RID: 1335
	private Quaternion originalEndTurnRotation;

	// Token: 0x04000538 RID: 1336
	private NumberBox leftStack;

	// Token: 0x04000539 RID: 1337
	private NumberBox rightStack;

	// Token: 0x0400053A RID: 1338
	private ResourceType? pendingSacrificedFor;
}
