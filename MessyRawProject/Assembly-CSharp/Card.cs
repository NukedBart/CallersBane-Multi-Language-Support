using System;
using UnityEngine;

// Token: 0x020000EE RID: 238
public class Card
{
	// Token: 0x060007D9 RID: 2009 RVA: 0x00002DDA File Offset: 0x00000FDA
	public Card()
	{
	}

	// Token: 0x060007DA RID: 2010 RVA: 0x00006F40 File Offset: 0x00005140
	public Card(long id, CardType type) : this(id, type, false)
	{
	}

	// Token: 0x060007DB RID: 2011 RVA: 0x00006F4B File Offset: 0x0000514B
	public Card(long id, CardType type, bool tradable)
	{
		this.id = id;
		this.type = type;
		this._typeId = type.id;
		this.tradable = tradable;
	}

	// Token: 0x060007DC RID: 2012 RVA: 0x00004AAC File Offset: 0x00002CAC
	public virtual bool isValid()
	{
		return true;
	}

	// Token: 0x060007DD RID: 2013 RVA: 0x00006F74 File Offset: 0x00005174
	public long getId()
	{
		return this.id;
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x00006F7C File Offset: 0x0000517C
	public string getResourceString()
	{
		return StringUtil.capitalize(this.type.getResource().ToString());
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x00006F98 File Offset: 0x00005198
	public CardType getCardType()
	{
		return this.type;
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x00006FA0 File Offset: 0x000051A0
	public ResourceType getResourceType()
	{
		return this.type.getResource();
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x00006FAD File Offset: 0x000051AD
	public int getCostTotal()
	{
		return this.type.getCost();
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x00006FBA File Offset: 0x000051BA
	public string getName()
	{
		return this.type.name;
	}

	// Token: 0x060007E3 RID: 2019 RVA: 0x00006FC7 File Offset: 0x000051C7
	public string getNameLower()
	{
		return this.type.getNameLower();
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x00006FD4 File Offset: 0x000051D4
	public int getType()
	{
		return this.type.id;
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x00006FE1 File Offset: 0x000051E1
	public string getPieceType()
	{
		return this.type.types.ToString();
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x00006FF3 File Offset: 0x000051F3
	public CardType.Kind getPieceKind()
	{
		return this.type.kind;
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x00007000 File Offset: 0x00005200
	public string getPieceKindText()
	{
		if (this.isToken)
		{
			return "TOKEN " + this.getPieceKind().ToString();
		}
		return this.getPieceKind().ToString();
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x00007038 File Offset: 0x00005238
	public int getRarity()
	{
		return this.type.rarity;
	}

	// Token: 0x060007E9 RID: 2025 RVA: 0x0004387C File Offset: 0x00041A7C
	public string getRarityString()
	{
		switch (this.type.rarity)
		{
		case 0:
			return "Common";
		case 1:
			return "Uncommon";
		case 2:
			return "Rare";
		default:
			return string.Empty;
		}
	}

	// Token: 0x060007EA RID: 2026 RVA: 0x00007045 File Offset: 0x00005245
	public int getHitPoints()
	{
		return this.type.hp;
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x00007052 File Offset: 0x00005252
	public int getAttackPower()
	{
		return this.type.ap;
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x0000705F File Offset: 0x0000525F
	public int getAttackInterval()
	{
		return this.type.ac;
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x0000706C File Offset: 0x0000526C
	public string getDescription()
	{
		return this.type.description;
	}

	// Token: 0x060007EE RID: 2030 RVA: 0x00007079 File Offset: 0x00005279
	public string getDescriptionFlavor()
	{
		return this.type.flavor;
	}

	// Token: 0x060007EF RID: 2031 RVA: 0x00007086 File Offset: 0x00005286
	public string getDescriptionAndAbilitiesLowerCase()
	{
		return this.type.getDescriptionAndAbilitiesLowerCase();
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x00007093 File Offset: 0x00005293
	public KeywordDescription[] getKeywords()
	{
		return this.type.getKeywords();
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x000070A0 File Offset: 0x000052A0
	public ActiveAbility[] getActiveAbilities()
	{
		return this.type.abilities;
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x000438C4 File Offset: 0x00041AC4
	public bool hasTriggeredAbility()
	{
		foreach (ActiveAbility activeAbility in this.getActiveAbilities())
		{
			if (activeAbility.isTriggered())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x000070AD File Offset: 0x000052AD
	public bool isType(string name)
	{
		return name.ToLower() == this.getNameLower();
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x000070C0 File Offset: 0x000052C0
	public PassiveAbility[] getPassiveAbilities()
	{
		return this.type.passiveRules;
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x000070CD File Offset: 0x000052CD
	public string getAttackType()
	{
		throw new NotImplementedException("getAttackType isn't implemented, since it isn't sent from server");
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x000070D9 File Offset: 0x000052D9
	public TargetArea getTargetArea()
	{
		return this.type.targetArea;
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x000070E6 File Offset: 0x000052E6
	public string getCardImage()
	{
		return this.type.cardImage.ToString();
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x000070F8 File Offset: 0x000052F8
	public int getSet()
	{
		return this.type.set;
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x00007105 File Offset: 0x00005305
	public int getTier()
	{
		return Mathf.Clamp(this.level + 1, 1, 3);
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x00007116 File Offset: 0x00005316
	public bool isTier(int t)
	{
		return this.getTier() == t;
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x00007121 File Offset: 0x00005321
	public bool isAtleastTier(int t)
	{
		return this.getTier() >= t;
	}

	// Token: 0x1700006E RID: 110
	// (get) Token: 0x060007FC RID: 2044 RVA: 0x0000712F File Offset: 0x0000532F
	// (set) Token: 0x060007FD RID: 2045 RVA: 0x00007137 File Offset: 0x00005337
	public int typeId
	{
		get
		{
			return this._typeId;
		}
		set
		{
			this._typeId = value;
			this.type = ((this._typeId < 0) ? new CardType() : CardTypeManager.getInstance().get(this._typeId));
		}
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x0000716C File Offset: 0x0000536C
	public void update()
	{
		this.typeId = this._typeId;
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x0000717A File Offset: 0x0000537A
	public T dataAs<T>()
	{
		return (T)((object)this.data);
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x00007187 File Offset: 0x00005387
	public bool hasStats()
	{
		return this.level >= 1;
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x00007195 File Offset: 0x00005395
	public bool isFoiled()
	{
		return this.level >= 2;
	}

	// Token: 0x06000802 RID: 2050 RVA: 0x000071A3 File Offset: 0x000053A3
	public bool upgrade()
	{
		if (this.level >= 4)
		{
			return false;
		}
		this.level++;
		return true;
	}

	// Token: 0x06000803 RID: 2051 RVA: 0x000071C2 File Offset: 0x000053C2
	public override int GetHashCode()
	{
		return (int)(this.id * 31337L) + this.type.id;
	}

	// Token: 0x06000804 RID: 2052 RVA: 0x00043900 File Offset: 0x00041B00
	public override bool Equals(object obj)
	{
		Card card = obj as Card;
		return card != null && card.type.id == this.type.id && card.id == this.id;
	}

	// Token: 0x040005E0 RID: 1504
	public long id;

	// Token: 0x040005E1 RID: 1505
	public bool tradable;

	// Token: 0x040005E2 RID: 1506
	public int level;

	// Token: 0x040005E3 RID: 1507
	public bool isToken;

	// Token: 0x040005E4 RID: 1508
	protected CardType type;

	// Token: 0x040005E5 RID: 1509
	public object data;

	// Token: 0x040005E6 RID: 1510
	private int _typeId;
}
