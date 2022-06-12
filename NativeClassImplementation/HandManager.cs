using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001E8 RID: 488
public class HandManager : MonoBehaviour, iCardRule
{
	// Token: 0x06000F46 RID: 3910 RVA: 0x0000C4BB File Offset: 0x0000A6BB
	public bool AddSelectedCardConfirm(CardConfirmType confirmType)
	{
		if (this.cardActivator != null)
		{
			this.cardActivator.addConfirm(confirmType);
			if (this.hasSelected())
			{
				this.selectedCard.onSelect();
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000F47 RID: 3911 RVA: 0x0000C4F3 File Offset: 0x0000A6F3
	public void setCardShaderName(string shader)
	{
		this.shaderName = shader;
	}

	// Token: 0x06000F48 RID: 3912 RVA: 0x00065134 File Offset: 0x00063334
	public CardView RayCast(out bool consumedByActivator)
	{
		consumedByActivator = false;
		if (!this.isInited)
		{
			return null;
		}
		if (!GUIUtil.isMouseOnScreen())
		{
			return null;
		}
		if (this.blockCheck.isInputBlocked())
		{
			return null;
		}
		bool flag = this.magnifiedCard != null;
		if (this.cardActivator != null)
		{
			consumedByActivator = this.cardActivator.Raycast();
		}
		RaycastHit raycastHit = default(RaycastHit);
		Ray ray = this.handCamera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, ref raycastHit, this.handCamera.farClipPlane, 1 << Layers.BattleModeUI) && !this.blockCheck.isInputBlocked())
		{
			if (flag && this.magnifiedCard && UnityUtil.hasParent(raycastHit.collider.gameObject.transform, this.magnifiedCard.transform))
			{
				return null;
			}
			if (flag && raycastHit.collider.gameObject == this.blackOverlay)
			{
				return this.magnifiedCard;
			}
			CardView component = raycastHit.collider.GetComponent<CardView>();
			if (component == null && raycastHit.collider.transform.parent != null && raycastHit.collider.transform.parent.parent != null)
			{
				component = raycastHit.collider.transform.parent.parent.gameObject.GetComponent<CardView>();
			}
			if (component != null)
			{
				if (component != this.magnifiedCard)
				{
					this.HoverCard(component.getCardInfo().getId());
				}
				return component;
			}
		}
		this.DehoverCard();
		return null;
	}

	// Token: 0x06000F49 RID: 3913 RVA: 0x0000C4FC File Offset: 0x0000A6FC
	private bool hasSelected()
	{
		return this.selectedCard != null;
	}

	// Token: 0x06000F4A RID: 3914 RVA: 0x0000C50A File Offset: 0x0000A70A
	private bool hasSelected(long cardId)
	{
		return this.hasSelected() && this.selectedCard.getCardId() == cardId;
	}

	// Token: 0x06000F4B RID: 3915 RVA: 0x000652FC File Offset: 0x000634FC
	private void ClearSelection()
	{
		if (!this.hasSelected())
		{
			return;
		}
		if (this.blockCheck.isInputBlocked())
		{
			return;
		}
		CardView card = this.selectedCard;
		this.selectedCard.onDeselect();
		this.selectedCard = null;
		this.handManagerCallback.SpecificCardDeselected(card);
	}

	// Token: 0x06000F4C RID: 3916 RVA: 0x0000C528 File Offset: 0x0000A728
	public void DeselectCard()
	{
		if (this.blockCheck.isInputBlocked())
		{
			return;
		}
		this.ClearSelection();
		this.DestroyMagnified();
		this.CloseCardActivator();
		this.UpdateRenderDepths();
		this.AnimateCardViews(0f);
	}

	// Token: 0x06000F4D RID: 3917 RVA: 0x0006534C File Offset: 0x0006354C
	public void RemoveSelectedCard()
	{
		if (!this.hasSelected())
		{
			return;
		}
		long id = this.selectedCard.getCardInfo().getId();
		for (int i = 0; i < this.currentHand.Count; i++)
		{
			if (this.currentHand[i].getId() == id)
			{
				this.currentHand.RemoveAt(i);
				break;
			}
		}
		this.CleanupHand();
		this.RefreshCardViews();
		this.DeselectCard();
	}

	// Token: 0x06000F4E RID: 3918 RVA: 0x0000C55E File Offset: 0x0000A75E
	public void DehoverCard()
	{
		this.HoverCard(-1L);
	}

	// Token: 0x06000F4F RID: 3919 RVA: 0x000653CC File Offset: 0x000635CC
	public CardView SelectCard(long id, ICardActivatorCallback callbackTarget, ResourceType[] resTypes, bool canSacrificeForCards, bool canUseCard, bool isSpectate, bool scaleUpWild)
	{
		if (this.magnifiedCard != null)
		{
			this.DestroyMagnified();
		}
		if (this.hasSelected(id))
		{
			this.DeselectCard();
			return null;
		}
		if (this.hasSelected())
		{
			this.ClearSelection();
		}
		for (int i = 0; i < this.cardViews.Count; i++)
		{
			if (this.cardViews[i].getCardInfo().id == id && this.selectedCard != this.cardViews[i])
			{
				this.selectedCard = this.cardViews[i];
				this.UpdateRenderDepths();
				this.AnimateCardViews(0f);
			}
		}
		if (!this.hasSelected())
		{
			Log.warning("Trying to select an non-existant card. Did you magnifiy a card that got sacrificed?");
			return null;
		}
		this.CloseCardActivator();
		this.ShowCardActivator(this.selectedCard, callbackTarget, resTypes, canSacrificeForCards, canUseCard, scaleUpWild);
		if (isSpectate)
		{
			this.MagnifySelected();
		}
		return this.selectedCard;
	}

	// Token: 0x06000F50 RID: 3920 RVA: 0x000654D0 File Offset: 0x000636D0
	public void HoverCard(long id)
	{
		if (id == -1L)
		{
			if (this.hoveredCard != null && this.hoveredCard != this.selectedCard)
			{
				base.StartCoroutine(this.MoveCard(this.hoveredCard, this.hoveredCard.getStartPos()));
			}
			this.hoveredCard = null;
			return;
		}
		for (int i = 0; i < this.cardViews.Count; i++)
		{
			if (this.cardViews[i].getCardInfo().id == id)
			{
				if (this.hoveredCard != this.cardViews[i])
				{
					if (this.hoveredCard != null && this.hoveredCard != this.selectedCard)
					{
						base.StartCoroutine(this.MoveCard(this.hoveredCard, this.hoveredCard.getStartPos()));
					}
					this.hoveredCard = this.cardViews[i];
					if (this.hoveredCard != this.selectedCard)
					{
						base.StartCoroutine(this.MoveCard(this.hoveredCard, this.hoveredCard.getStartPos() + this.GetOffset(this.hoveredCard)));
					}
				}
				break;
			}
		}
	}

	// Token: 0x06000F51 RID: 3921 RVA: 0x0000C568 File Offset: 0x0000A768
	public void RaiseCards(bool raise)
	{
		this.RaiseCards((!raise) ? 0f : 0.22f);
	}

	// Token: 0x06000F52 RID: 3922 RVA: 0x00065628 File Offset: 0x00063828
	public void RaiseCards(float height)
	{
		if (height == this.raisedHeight)
		{
			return;
		}
		this.raisedHeight = height;
		this.yHeightOffset = this.handCamera.orthographicSize * height;
		foreach (CardView cardView in this.cardViews)
		{
			base.StartCoroutine(this.MoveCard(cardView, cardView.getStartPos()));
		}
	}

	// Token: 0x06000F53 RID: 3923 RVA: 0x0000C585 File Offset: 0x0000A785
	public CardView GetSelectedCard()
	{
		return this.selectedCard;
	}

	// Token: 0x06000F54 RID: 3924 RVA: 0x000656B8 File Offset: 0x000638B8
	public void MagnifySelected()
	{
		if (!this.hasSelected())
		{
			return;
		}
		this.blackOverlay = PrimitiveFactory.createPlane();
		this.blackOverlay.name = "CardInfoBlackOverlay";
		this.blackOverlay.transform.eulerAngles = new Vector3(90f, 180f, 0f);
		this.blackOverlay.transform.localScale = new Vector3((float)Screen.width, 1f, (float)Screen.height);
		this.blackOverlay.transform.position = new Vector3(0f, 0f, 0.75f);
		this.blackOverlay.transform.parent = this.handCameraTransform;
		Material material = new Material(ResourceManager.LoadShader("Transparent/Diffuse"));
		material.color = new Color(0f, 0f, 0f, 0.5f);
		material.renderQueue = 93990;
		this.blackOverlay.renderer.material = material;
		this.blackOverlay.layer = 9;
		this.magnifiedCard = this.CreateCardObject(this.selectedCard.getCardInfo());
		this.magnifiedCard.enableShowStats();
		this.magnifiedCard.enableShowHelp();
		this.magnifiedCard.setRenderQueue(200);
		this.magnifiedCard.setLayer(Layers.BattleModeUI);
		this.magnifiedCard.setRaycastCamera(this.handCamera);
		this.magnifiedCard.transform.eulerAngles = new Vector3(90f, 180f, 0f);
		this.magnifiedCard.transform.localScale = CardView.CardLocalScale((float)Screen.height * 0.45f);
		this.magnifiedCard.transform.parent = this.handCameraTransform;
		this.magnifiedCard.transform.position = new Vector3(0f, (float)(-(float)Screen.height) * 0.05f, 0.5f);
		this.cardActivator.Hide();
	}

	// Token: 0x06000F55 RID: 3925 RVA: 0x0000C58D File Offset: 0x0000A78D
	public CardView getMagnifiedCard()
	{
		return this.magnifiedCard;
	}

	// Token: 0x06000F56 RID: 3926 RVA: 0x000658BC File Offset: 0x00063ABC
	private void DestroyMagnified()
	{
		if (this.magnifiedCard != null)
		{
			Object.Destroy(this.magnifiedCard.gameObject);
			Object.Destroy(this.blackOverlay);
			if (this.cardActivator)
			{
				this.cardActivator.Show();
			}
		}
	}

	// Token: 0x06000F57 RID: 3927 RVA: 0x0000C595 File Offset: 0x0000A795
	private void Start()
	{
		this.handCamera.orthographicSize = (float)(Screen.height / 2);
		this.hoverOffset = (float)Screen.height * 0.025f;
	}

	// Token: 0x06000F58 RID: 3928 RVA: 0x0000C5BC File Offset: 0x0000A7BC
	public void Init(ITutorialBlockCheck blockCheck, IHandManagerCallback multiplierIncreaseCallback)
	{
		this.blockCheck = blockCheck;
		this.handManagerCallback = multiplierIncreaseCallback;
		this.tooltipGUISkin = (GUISkin)ResourceManager.Load("_GUISkins/Tooltip");
		this.isInited = true;
	}

	// Token: 0x06000F59 RID: 3929 RVA: 0x00065910 File Offset: 0x00063B10
	public void SetHand(Card[] hand, ResourceGroup currentResources, int profileId)
	{
		TimedLog.LogWithTime("HandManager: Entering SetHand");
		this.currentHand.Clear();
		bool flag = false;
		foreach (Card card in hand)
		{
			this.currentHand.Add(card);
			if (this.hasSelected(card.getId()))
			{
				flag = true;
			}
		}
		TimedLog.LogWithTime("HandManager: SetHand: Stored new hand");
		if (!flag)
		{
			this.selectedCard = null;
		}
		this.RefreshHand(currentResources, profileId);
		TimedLog.LogWithTime("HandManager: SetHand: Finished");
	}

	// Token: 0x06000F5A RID: 3930 RVA: 0x0000C5E8 File Offset: 0x0000A7E8
	public int GetHandSize()
	{
		return this.currentHand.Count;
	}

	// Token: 0x06000F5B RID: 3931 RVA: 0x0000C5F5 File Offset: 0x0000A7F5
	public List<CardView> GetCardViewsInHand()
	{
		return this.cardViews;
	}

	// Token: 0x06000F5C RID: 3932 RVA: 0x00065998 File Offset: 0x00063B98
	public bool IsAnyCardMoving()
	{
		foreach (CardView cardView in this.cardViews)
		{
			if (cardView.cardMoving)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000F5D RID: 3933 RVA: 0x0000C5FD File Offset: 0x0000A7FD
	public void RefreshHand(ResourceGroup currentResources, int profileId)
	{
		this.CleanupHand();
		this.RefreshCardViews();
		this.RefreshCardAffordabilities(currentResources);
		this.UpdateRenderDepths();
		this.AnimateCardViews(0.1f);
		this.SetCardOwner(profileId);
	}

	// Token: 0x06000F5E RID: 3934 RVA: 0x00065A00 File Offset: 0x00063C00
	public void RefreshCardAffordabilities(ResourceGroup currentResources)
	{
		TimedLog.LogWithTime("HandManager: Entering RefreshCardAffordabilities");
		CardType cardType = new CardType();
		foreach (Card card in this.currentHand)
		{
			CardView cardViewById = this.GetCardViewById(card.getId());
			bool flag = !this.hasSelected() || this.selectedCard == cardViewById;
			cardType.costDecay = (cardType.costEnergy = (cardType.costGrowth = (cardType.costOrder = 0)));
			int num = cardViewById.getCostOrOverridden();
			if (!flag)
			{
				num += card.getCardType().getTag<int>("increasing_cost", 0);
			}
			switch (card.getResourceType())
			{
			case ResourceType.GROWTH:
				cardType.costGrowth = num;
				break;
			case ResourceType.ENERGY:
				cardType.costEnergy = num;
				break;
			case ResourceType.ORDER:
				cardType.costOrder = num;
				break;
			case ResourceType.DECAY:
				cardType.costDecay = num;
				break;
			}
			if (flag)
			{
				cardViewById.setGrayedOutCost(!currentResources.canAfford(cardType));
			}
			else
			{
				cardViewById.setGrayedOutCost(!this.reductedResources.canAfford(cardType));
			}
		}
	}

	// Token: 0x06000F5F RID: 3935 RVA: 0x0000C62A File Offset: 0x0000A82A
	public void SetReductedResources(ResourceGroup reductedResources)
	{
		this.reductedResources = new ResourceGroup(reductedResources);
	}

	// Token: 0x06000F60 RID: 3936 RVA: 0x00065B70 File Offset: 0x00063D70
	public void SetCardsGrayedOut(bool grayedOut)
	{
		this.cardsGrayedOut = grayedOut;
		foreach (Card card in this.currentHand)
		{
			CardView cardViewById = this.GetCardViewById(card.getId());
			cardViewById.setGrayedOut(this.cardsGrayedOut);
		}
	}

	// Token: 0x06000F61 RID: 3937 RVA: 0x00065BE4 File Offset: 0x00063DE4
	private void CleanupHand()
	{
		TimedLog.LogWithTime("HandManager: Entering CleanupHand");
		List<CardView> list = new List<CardView>();
		foreach (CardView cardView in this.cardViews)
		{
			bool flag = false;
			foreach (Card card in this.currentHand)
			{
				if (card.getId() == cardView.getCardInfo().getId())
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(cardView);
			}
		}
		foreach (CardView cardView2 in list)
		{
			this.cardViews.Remove(cardView2);
			Object.DestroyImmediate(cardView2.gameObject);
		}
	}

	// Token: 0x06000F62 RID: 3938 RVA: 0x00065D14 File Offset: 0x00063F14
	private void SetCardOwner(int profileId)
	{
		foreach (CardView cardView in this.cardViews)
		{
			cardView.setProfileId(profileId);
		}
	}

	// Token: 0x06000F63 RID: 3939 RVA: 0x00065D70 File Offset: 0x00063F70
	private void RefreshCardViews()
	{
		TimedLog.LogWithTime("HandManager: Entering RefreshCardViews");
		int num = 0;
		foreach (Card card in this.currentHand)
		{
			CardView cardViewById = this.GetCardViewById(card.getId());
			if (cardViewById == null)
			{
				try
				{
					CardView cardView = this.CreateCardObject(card);
					Transform transform = cardView.gameObject.transform;
					transform.eulerAngles = new Vector3(90f, 180f, 0f);
					transform.localScale = CardView.CardLocalScale((float)Screen.height * 0.2f);
					transform.parent = this.handCameraTransform;
					transform.position = new Vector3((float)Screen.width * 0.7f, (float)(-(float)Screen.height) * 0.45f, this.handCamera.farClipPlane - 5f - 0.1f * (float)num);
					cardView.setGrayedOut(this.cardsGrayedOut);
					cardView.setLayer(9);
					this.cardViews.Add(cardView);
					if (card.isFoiled())
					{
						float num2 = this.handManagerCallback.IncreaseMultiplier(card.getRarity());
						if (num2 > 0f)
						{
							this.cardTexts.Add(new HandManager.TextObject(cardView, "Gold\n+" + Math.Round((double)(num2 * 100f), 3) + "%", 2.3f));
						}
					}
				}
				catch (Exception ex)
				{
					Log.warning("Error when creating card. ERROR: " + ex);
				}
			}
			num++;
		}
	}

	// Token: 0x06000F64 RID: 3940 RVA: 0x00065F4C File Offset: 0x0006414C
	private Vector3 GetOffset(CardView cardView)
	{
		float num = 0f;
		if (cardView == this.selectedCard)
		{
			num = (float)Screen.height * 0.075f;
		}
		else if (cardView == this.hoveredCard)
		{
			num = this.hoverOffset;
		}
		return new Vector3(0f, num, 0f);
	}

	// Token: 0x06000F65 RID: 3941 RVA: 0x00065FAC File Offset: 0x000641AC
	private void AnimateCardViews(float delay)
	{
		TimedLog.LogWithTime("HandManager: Entering AnimateCardViews");
		int count = this.cardViews.Count;
		if (this.hasSelected())
		{
			for (int i = 0; i < this.cardViews.Count; i++)
			{
				if (this.cardViews[i] == this.selectedCard)
				{
					break;
				}
			}
		}
		float num = 0f;
		if (this.cardViews.Count > 0)
		{
			num = this.cardViews[0].collider.bounds.size.x;
		}
		Vector3 vector;
		vector..ctor((float)(-(float)Screen.width) * 0.5f + (float)Screen.height * 0.43f, (float)(-(float)Screen.height) * 0.44f, 3f);
		Vector3 vector2;
		vector2..ctor((float)Screen.width * 0.5f - (float)Screen.height * 0.43f, (float)(-(float)Screen.height) * 0.44f, 3f);
		float num2 = (vector2 - vector).x / (float)(count - 1);
		float num3 = Mathf.Min(num, num2);
		for (int j = 0; j < this.cardViews.Count; j++)
		{
			CardView cardView = this.cardViews[j];
			float num4 = (vector2.x + vector.x) / 2f + ((float)(-(float)this.cardViews.Count) / 2f + 0.5f + (float)j) * num3;
			float num5 = 0f;
			if (this.hasSelected())
			{
				if (this.selectedCard == cardView)
				{
					num5 = this.hoverOffset;
				}
				else
				{
					num5 = (float)(-(float)Screen.height) * 0.03f;
				}
			}
			Vector3 vector3;
			vector3..ctor(num4, vector.y + num5, cardView.transform.position.z);
			cardView.setStartPos(vector3);
			base.StartCoroutine(this.MoveCardDelayed(cardView, vector3, (float)j * delay));
		}
	}

	// Token: 0x06000F66 RID: 3942 RVA: 0x000661D4 File Offset: 0x000643D4
	private IEnumerator MoveCard(CardView cardView, Vector3 pos)
	{
		if (cardView != null)
		{
			iTween.Stop(cardView.gameObject);
			yield return null;
			if (cardView != null)
			{
				cardView.cardMoving = true;
				iTween.MoveTo(cardView.gameObject, iTween.Hash(new object[]
				{
					"x",
					pos.x,
					"y",
					pos.y + this.yHeightOffset,
					"time",
					0.3f,
					"easetype",
					iTween.EaseType.easeOutExpo,
					"oncompletetarget",
					cardView.gameObject,
					"oncomplete",
					"doneMoving"
				}));
			}
		}
		yield break;
	}

	// Token: 0x06000F67 RID: 3943 RVA: 0x0006620C File Offset: 0x0006440C
	private IEnumerator MoveCardDelayed(CardView cardView, Vector3 pos, float delay)
	{
		if (cardView != null)
		{
			iTween.Stop(cardView.gameObject);
			yield return null;
			if (cardView != null)
			{
				cardView.cardMoving = true;
				iTween.MoveTo(cardView.gameObject, iTween.Hash(new object[]
				{
					"x",
					pos.x,
					"y",
					pos.y + this.yHeightOffset,
					"z",
					pos.z,
					"time",
					0.2f,
					"delay",
					delay,
					"easetype",
					iTween.EaseType.easeInOutExpo,
					"oncompletetarget",
					cardView.gameObject,
					"oncomplete",
					"doneMoving"
				}));
			}
		}
		yield break;
	}

	// Token: 0x06000F68 RID: 3944 RVA: 0x00066254 File Offset: 0x00064454
	private void UpdateRenderDepths()
	{
		TimedLog.LogWithTime("HandManager: Entering UpdateRenderDepths");
		int num = -1;
		for (int i = 0; i < this.cardViews.Count; i++)
		{
			CardView cardView = this.cardViews[i];
			int num2;
			if (cardView == this.selectedCard)
			{
				num = i;
				num2 = this.cardViews.Count * 2;
			}
			else if (num == -1)
			{
				num2 = i;
			}
			else
			{
				num2 = this.cardViews.Count * 2 + (num - i);
			}
			cardView.setRenderQueue(num2);
			cardView.transform.position = new Vector3(cardView.transform.position.x, cardView.transform.position.y, this.handCamera.farClipPlane - 1f - (float)num2);
		}
	}

	// Token: 0x06000F69 RID: 3945 RVA: 0x00066334 File Offset: 0x00064534
	private CardView CreateCardObject(Card cardInfo)
	{
		GameObject gameObject = PrimitiveFactory.createPlane();
		CardView cardView = gameObject.AddComponent<CardView>();
		cardView.overrideCost(this.handManagerCallback.GetCostForCard(cardInfo));
		if (this.shaderName != null)
		{
			cardView.setShader(this.shaderName);
		}
		cardView.name = "Card_" + cardInfo.getName();
		cardView.init(this, cardInfo, 0);
		cardView.setRaycastCamera(this.handCamera);
		cardView.applyHighResTexture();
		return cardView;
	}

	// Token: 0x06000F6A RID: 3946 RVA: 0x000663AC File Offset: 0x000645AC
	private CardView GetCardViewById(long id)
	{
		foreach (CardView cardView in this.cardViews)
		{
			if (cardView.getCardInfo().getId() == id)
			{
				return cardView;
			}
		}
		return null;
	}

	// Token: 0x06000F6B RID: 3947 RVA: 0x0000C638 File Offset: 0x0000A838
	public void ShowCardActivatorForSelected(ICardActivatorCallback callbackTarget, List<ResourceType> resTypes, bool canUseCard, bool scaleUpWild)
	{
		if (!this.hasSelected())
		{
			return;
		}
		this.CloseCardActivator();
		this.ShowCardActivator(this.GetSelectedCard(), callbackTarget, resTypes.ToArray(), resTypes.Count > 0, canUseCard, scaleUpWild);
	}

	// Token: 0x06000F6C RID: 3948 RVA: 0x0006641C File Offset: 0x0006461C
	private void ShowCardActivator(CardView card, ICardActivatorCallback callbackTarget, ResourceType[] resTypes, bool canSacrificeForCards, bool canUseCard, bool scaleUpWild)
	{
		this.cardActivator = new GameObject("CardActivator").AddComponent<CardActivator>();
		this.cardActivator.transform.parent = this.handCameraTransform;
		this.cardActivator.transform.position = new Vector3(card.transform.position.x, (float)(-(float)Screen.height) * 0.21f, card.transform.position.z);
		this.cardActivator.init(this.handCamera, callbackTarget, card, resTypes, canSacrificeForCards, canUseCard, scaleUpWild);
	}

	// Token: 0x06000F6D RID: 3949 RVA: 0x0000C66B File Offset: 0x0000A86B
	private void CloseCardActivator()
	{
		if (this.cardActivator != null)
		{
			this.cardActivator.killActivator();
			this.cardActivator = null;
		}
	}

	// Token: 0x06000F6E RID: 3950 RVA: 0x000664B8 File Offset: 0x000646B8
	private void OnGUI()
	{
		GUI.depth = 3;
		GUI.skin = (GUISkin)ResourceManager.Load("_GUISkins/RegularUI");
		TextAnchor alignment = GUI.skin.label.alignment;
		int fontSize = GUI.skin.label.fontSize;
		GUI.skin.label.alignment = 1;
		GUI.skin.label.fontSize = Screen.height / 40;
		foreach (HandManager.TextObject textObject in this.cardTexts)
		{
			GUI.color = new Color(0f, 0f, 0f, textObject.alpha);
			GUI.Label(new Rect(textObject.rect.x + 2f, textObject.rect.y + 2f, textObject.rect.width, textObject.rect.height), textObject.text);
			GUI.color = new Color(1f, 0.95f, 0.1f, textObject.alpha);
			GUI.Label(textObject.rect, textObject.text);
			GUI.color = Color.white;
		}
		if (this.cardActivator != null && this.cardActivator.getTooltip() != null)
		{
			GUIContent guicontent = new GUIContent(this.cardActivator.getTooltip());
			GUISkin skin = GUI.skin;
			GUI.skin = this.tooltipGUISkin;
			int num = Screen.height / 4;
			int fontSize2 = GUI.skin.label.fontSize;
			GUI.skin.label.fontSize = Screen.height / 45;
			float num2 = this.tooltipGUISkin.label.CalcHeight(guicontent, (float)num);
			Vector2 screenMousePos = GUIUtil.getScreenMousePos();
			GUI.Label(new Rect(screenMousePos.x + (float)(num / 20), screenMousePos.y - (float)(num / 13), (float)num, num2), guicontent);
			this.cardActivator.getTooltip();
			GUI.skin.label.fontSize = fontSize2;
			GUI.skin = skin;
		}
		GUI.skin.label.fontSize = fontSize;
		GUI.skin.label.alignment = alignment;
	}

	// Token: 0x06000F6F RID: 3951 RVA: 0x00066714 File Offset: 0x00064914
	private void Update()
	{
		this.toRemove.Clear();
		foreach (HandManager.TextObject textObject in this.cardTexts)
		{
			if (textObject.cardView != null)
			{
				Vector3 vector = this.handCamera.WorldToScreenPoint(textObject.cardView.transform.position);
				textObject.rect = new Rect(vector.x - (float)Screen.height * 0.15f, (float)Screen.height - vector.y - (float)Screen.height * 0.2325f + textObject.offsetY, (float)Screen.height * 0.3f, (float)Screen.height * 0.07f);
			}
			textObject.timeLeft -= Time.deltaTime;
			textObject.alpha = Mathf.Min(1f, textObject.timeLeft * 10f / textObject.duration);
			textObject.offsetY = -0.05f * (float)Screen.height * (1f - textObject.alpha);
			if (textObject.timeLeft < 0f)
			{
				this.toRemove.Add(textObject);
			}
		}
		foreach (HandManager.TextObject textObject2 in this.toRemove)
		{
			this.cardTexts.Remove(textObject2);
		}
	}

	// Token: 0x06000F70 RID: 3952 RVA: 0x000028DF File Offset: 0x00000ADF
	public void HideCardView()
	{
	}

	// Token: 0x06000F71 RID: 3953 RVA: 0x000028DF File Offset: 0x00000ADF
	public void ActivateTriggeredAbility(string id, TilePosition pos)
	{
	}

	// Token: 0x04000BDF RID: 3039
	public Transform handCameraTransform;

	// Token: 0x04000BE0 RID: 3040
	public Camera handCamera;

	// Token: 0x04000BE1 RID: 3041
	private List<Card> currentHand = new List<Card>();

	// Token: 0x04000BE2 RID: 3042
	private List<CardView> cardViews = new List<CardView>();

	// Token: 0x04000BE3 RID: 3043
	private bool cardsGrayedOut = true;

	// Token: 0x04000BE4 RID: 3044
	private CardView hoveredCard;

	// Token: 0x04000BE5 RID: 3045
	private CardView selectedCard;

	// Token: 0x04000BE6 RID: 3046
	private float hoverOffset;

	// Token: 0x04000BE7 RID: 3047
	private CardActivator cardActivator;

	// Token: 0x04000BE8 RID: 3048
	private CardView magnifiedCard;

	// Token: 0x04000BE9 RID: 3049
	private GameObject blackOverlay;

	// Token: 0x04000BEA RID: 3050
	private ITutorialBlockCheck blockCheck;

	// Token: 0x04000BEB RID: 3051
	private IHandManagerCallback handManagerCallback;

	// Token: 0x04000BEC RID: 3052
	private ResourceGroup reductedResources = new ResourceGroup();

	// Token: 0x04000BED RID: 3053
	private bool isInited;

	// Token: 0x04000BEE RID: 3054
	private GUISkin tooltipGUISkin;

	// Token: 0x04000BEF RID: 3055
	private List<HandManager.TextObject> cardTexts = new List<HandManager.TextObject>();

	// Token: 0x04000BF0 RID: 3056
	private string shaderName;

	// Token: 0x04000BF1 RID: 3057
	private float raisedHeight;

	// Token: 0x04000BF2 RID: 3058
	private float yHeightOffset;

	// Token: 0x04000BF3 RID: 3059
	private List<HandManager.TextObject> toRemove = new List<HandManager.TextObject>();

	// Token: 0x020001E9 RID: 489
	private class TextObject
	{
		// Token: 0x06000F72 RID: 3954 RVA: 0x000668D4 File Offset: 0x00064AD4
		public TextObject(CardView cv, string s, float duration)
		{
			this.cardView = cv;
			this.text = s;
			this.timeLeft = duration;
			this.duration = duration;
		}

		// Token: 0x04000BF4 RID: 3060
		public CardView cardView;

		// Token: 0x04000BF5 RID: 3061
		public string text;

		// Token: 0x04000BF6 RID: 3062
		public float duration;

		// Token: 0x04000BF7 RID: 3063
		public float timeLeft;

		// Token: 0x04000BF8 RID: 3064
		public float offsetY;

		// Token: 0x04000BF9 RID: 3065
		public float alpha;

		// Token: 0x04000BFA RID: 3066
		public Rect rect;
	}
}
