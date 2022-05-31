using System;
using System.Collections.Generic;

// Token: 0x02000023 RID: 35
internal class CardTypeAssetQuerier
{
	// Token: 0x060001E9 RID: 489 RVA: 0x00003870 File Offset: 0x00001A70
	public CardTypeAssetQuerier(List<CardType> cardTypes)
	{
		this.cardTypes = cardTypes;
	}

	// Token: 0x060001EA RID: 490 RVA: 0x0000387F File Offset: 0x00001A7F
	public bool hasType(AssetType type, int id)
	{
		if (type == AssetType.CardImage)
		{
			return this.hasCardImage(id);
		}
		if (type == AssetType.AnimationBundle)
		{
			return this.hasAnimationBundle(id);
		}
		return type == AssetType.AnimationPreview && this.hasAnimationPreview(id);
	}

	// Token: 0x060001EB RID: 491 RVA: 0x00020838 File Offset: 0x0001EA38
	public bool hasCardImage(int id)
	{
		return this.query((CardType t) => id == t.cardImage);
	}

	// Token: 0x060001EC RID: 492 RVA: 0x00020864 File Offset: 0x0001EA64
	public bool hasAnimationBundle(int id)
	{
		return this.query((CardType t) => id == t.animationBundle);
	}

	// Token: 0x060001ED RID: 493 RVA: 0x00020890 File Offset: 0x0001EA90
	public bool hasAnimationPreview(int id)
	{
		return this.query((CardType t) => id == t.animationPreviewImage);
	}

	// Token: 0x060001EE RID: 494 RVA: 0x000208BC File Offset: 0x0001EABC
	private bool query(CardTypeAssetQuerier.Predicate f)
	{
		foreach (CardType t in this.cardTypes)
		{
			if (f(t))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x040000E9 RID: 233
	private List<CardType> cardTypes;

	// Token: 0x02000024 RID: 36
	// (Invoke) Token: 0x060001F0 RID: 496
	private delegate bool Predicate(CardType t);
}
