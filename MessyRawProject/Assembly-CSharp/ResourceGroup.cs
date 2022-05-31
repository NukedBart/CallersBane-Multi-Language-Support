using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x02000254 RID: 596
public class ResourceGroup : IEnumerable, IEnumerable<KeyValuePair<ResourceType, int>>
{
	// Token: 0x060011BA RID: 4538 RVA: 0x0000D7DA File Offset: 0x0000B9DA
	public ResourceGroup()
	{
		this._resources = new Dictionary<ResourceType, int>();
	}

	// Token: 0x060011BB RID: 4539 RVA: 0x0000D7ED File Offset: 0x0000B9ED
	public ResourceGroup(ResourceGroup g)
	{
		this._resources = new Dictionary<ResourceType, int>(g._resources);
	}

	// Token: 0x060011BC RID: 4540 RVA: 0x0000D806 File Offset: 0x0000BA06
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this._resources.GetEnumerator();
	}

	// Token: 0x170000EF RID: 239
	// (get) Token: 0x060011BD RID: 4541 RVA: 0x0000D818 File Offset: 0x0000BA18
	// (set) Token: 0x060011BE RID: 4542 RVA: 0x0000D821 File Offset: 0x0000BA21
	public int DECAY
	{
		get
		{
			return this.get(ResourceType.DECAY);
		}
		set
		{
			this.set(ResourceType.DECAY, value);
		}
	}

	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x060011BF RID: 4543 RVA: 0x0000D82B File Offset: 0x0000BA2B
	// (set) Token: 0x060011C0 RID: 4544 RVA: 0x0000D834 File Offset: 0x0000BA34
	public int ORDER
	{
		get
		{
			return this.get(ResourceType.ORDER);
		}
		set
		{
			this.set(ResourceType.ORDER, value);
		}
	}

	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x060011C1 RID: 4545 RVA: 0x0000D83E File Offset: 0x0000BA3E
	// (set) Token: 0x060011C2 RID: 4546 RVA: 0x0000D847 File Offset: 0x0000BA47
	public int ENERGY
	{
		get
		{
			return this.get(ResourceType.ENERGY);
		}
		set
		{
			this.set(ResourceType.ENERGY, value);
		}
	}

	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x060011C3 RID: 4547 RVA: 0x0000D851 File Offset: 0x0000BA51
	// (set) Token: 0x060011C4 RID: 4548 RVA: 0x0000D85A File Offset: 0x0000BA5A
	public int GROWTH
	{
		get
		{
			return this.get(ResourceType.GROWTH);
		}
		set
		{
			this.set(ResourceType.GROWTH, value);
		}
	}

	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x060011C5 RID: 4549 RVA: 0x0000D864 File Offset: 0x0000BA64
	// (set) Token: 0x060011C6 RID: 4550 RVA: 0x0000D86D File Offset: 0x0000BA6D
	public int SPECIAL
	{
		get
		{
			return this.get(ResourceType.SPECIAL);
		}
		set
		{
			this.set(ResourceType.SPECIAL, value);
		}
	}

	// Token: 0x060011C7 RID: 4551 RVA: 0x00077558 File Offset: 0x00075758
	public bool equals(ResourceGroup g)
	{
		foreach (ResourceType t in CollectionUtil.enumValues<ResourceType>())
		{
			if (this.get(t) != g.get(t))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060011C8 RID: 4552 RVA: 0x0007759C File Offset: 0x0007579C
	public void sub(ResourceType rt, int amount)
	{
		if (!rt.isResource())
		{
			Log.warning("Subbing a non-regular ResourceType: " + rt);
		}
		int num = this.get(rt);
		int num2 = amount - num;
		if (num >= amount)
		{
			this.set(rt, num - amount);
		}
		else
		{
			this.set(rt, 0);
			this.SPECIAL -= num2;
		}
	}

	// Token: 0x060011C9 RID: 4553 RVA: 0x00077604 File Offset: 0x00075804
	public int get(ResourceType t)
	{
		int result;
		if (this._resources.TryGetValue(t, ref result))
		{
			return result;
		}
		return 0;
	}

	// Token: 0x060011CA RID: 4554 RVA: 0x0000D877 File Offset: 0x0000BA77
	public void set(ResourceType t, int value)
	{
		this._resources[t] = value;
	}

	// Token: 0x060011CB RID: 4555 RVA: 0x0000D886 File Offset: 0x0000BA86
	public bool has(ResourceType t)
	{
		return this.get(t) > 0;
	}

	// Token: 0x060011CC RID: 4556 RVA: 0x00077628 File Offset: 0x00075828
	public bool canAfford(CardType cardType)
	{
		int special = this.SPECIAL;
		return cardType.costDecay <= this.DECAY + special && cardType.costEnergy <= this.ENERGY + special && cardType.costGrowth <= this.GROWTH + special && cardType.costOrder <= this.ORDER + special;
	}

	// Token: 0x060011CD RID: 4557 RVA: 0x0000D892 File Offset: 0x0000BA92
	public bool canAfford(Card card)
	{
		return this.canAfford(card.getCardType());
	}

	// Token: 0x060011CE RID: 4558 RVA: 0x0007768C File Offset: 0x0007588C
	public override string ToString()
	{
		string text = string.Empty;
		foreach (ResourceType resourceType in CollectionUtil.enumValues<ResourceType>())
		{
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				resourceType,
				": ",
				this.get(resourceType),
				"\n"
			});
		}
		return text;
	}

	// Token: 0x060011CF RID: 4559 RVA: 0x0000D806 File Offset: 0x0000BA06
	public IEnumerator<KeyValuePair<ResourceType, int>> GetEnumerator()
	{
		return this._resources.GetEnumerator();
	}

	// Token: 0x04000E68 RID: 3688
	private Dictionary<ResourceType, int> _resources;
}
