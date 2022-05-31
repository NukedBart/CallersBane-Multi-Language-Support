using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

// Token: 0x02000102 RID: 258
public class CardType
{
	// Token: 0x06000850 RID: 2128 RVA: 0x0000748A File Offset: 0x0000568A
	public bool isValid()
	{
		return this.id != 0;
	}

	// Token: 0x1700006F RID: 111
	// (get) Token: 0x06000851 RID: 2129 RVA: 0x00007498 File Offset: 0x00005698
	// (set) Token: 0x06000852 RID: 2130 RVA: 0x000074A0 File Offset: 0x000056A0
	public string subTypesStr
	{
		get
		{
			return this._type;
		}
		set
		{
			this._type = value;
			this.types = CardType.TypeSet.fromTypeString(value);
		}
	}

	// Token: 0x06000853 RID: 2131 RVA: 0x000074B5 File Offset: 0x000056B5
	public ResourceType getResource()
	{
		if (this.costDecay > 0)
		{
			return ResourceType.DECAY;
		}
		if (this.costEnergy > 0)
		{
			return ResourceType.ENERGY;
		}
		if (this.costGrowth > 0)
		{
			return ResourceType.GROWTH;
		}
		if (this.costOrder > 0)
		{
			return ResourceType.ORDER;
		}
		return ResourceType.NONE;
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x000074F0 File Offset: 0x000056F0
	public int getCostInResource(ResourceType res)
	{
		if (res == ResourceType.DECAY)
		{
			return this.costDecay;
		}
		if (res == ResourceType.ENERGY)
		{
			return this.costEnergy;
		}
		if (res == ResourceType.GROWTH)
		{
			return this.costGrowth;
		}
		if (res == ResourceType.ORDER)
		{
			return this.costOrder;
		}
		return 0;
	}

	// Token: 0x06000855 RID: 2133 RVA: 0x0000752A File Offset: 0x0000572A
	public int getCost()
	{
		return this.getCostInResource(this.getResource());
	}

	// Token: 0x06000856 RID: 2134 RVA: 0x00007538 File Offset: 0x00005738
	public TagSoundReader Sound()
	{
		return new TagSoundReader(this);
	}

	// Token: 0x06000857 RID: 2135 RVA: 0x00044470 File Offset: 0x00042670
	public void onLoaded()
	{
		this._nameLower = this.name.ToLower();
		this.description = this.description.Replace("\\n", "\n");
		this.flavor = ((this.flavor == null) ? string.Empty : this.flavor.Replace("\\n", "\n"));
		this.generateDescriptionAndAbilities();
		this.generateKeywords();
	}

	// Token: 0x06000858 RID: 2136 RVA: 0x000444E8 File Offset: 0x000426E8
	private void generateDescriptionAndAbilities()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(this.description);
		stringBuilder.Append('\n');
		foreach (ActiveAbility activeAbility in this.abilities)
		{
			if (activeAbility.id != ActiveAbility.Move)
			{
				stringBuilder.Append(activeAbility.name);
			}
		}
		foreach (PassiveAbility passiveAbility in this.passiveRules)
		{
			stringBuilder.Append(passiveAbility.displayName);
		}
		this._descriptionAndAbilities = stringBuilder.ToString().Replace("\\n", "\n").ToLower();
	}

	// Token: 0x06000859 RID: 2137 RVA: 0x000445A8 File Offset: 0x000427A8
	private void generateKeywords()
	{
		List<KeywordDescription> list = new List<KeywordDescription>();
		foreach (PassiveAbility passiveAbility in this.passiveRules)
		{
			string text = passiveAbility.displayName;
			if (this.id != 318 || !(text.ToLower() == "ranged attack"))
			{
				int num = text.IndexOf(':');
				if (num > 0)
				{
					text = text.Substring(0, num);
				}
				this.addKeywordIfNotAdded(list, KeywordDescription.fromWord(text, passiveAbility.description));
			}
		}
		foreach (KeywordDescription toAdd in Keywords.find(this.description))
		{
			this.addKeywordIfNotAdded(list, toAdd);
		}
		foreach (PassiveAbility passiveAbility2 in this.passiveRules)
		{
			foreach (KeywordDescription toAdd2 in Keywords.find(passiveAbility2.displayName))
			{
				this.addKeywordIfNotAdded(list, toAdd2);
			}
		}
		this._keywords = list.ToArray();
	}

	// Token: 0x0600085A RID: 2138 RVA: 0x00044724 File Offset: 0x00042924
	private void addKeywordIfNotAdded(ICollection<KeywordDescription> collection, KeywordDescription toAdd)
	{
		if (!Enumerable.Any<KeywordDescription>(collection, (KeywordDescription c) => c.keyword == toAdd.keyword))
		{
			collection.Add(toAdd);
		}
	}

	// Token: 0x0600085B RID: 2139 RVA: 0x00007540 File Offset: 0x00005740
	public string getDescriptionAndAbilitiesLowerCase()
	{
		return this._descriptionAndAbilities;
	}

	// Token: 0x0600085C RID: 2140 RVA: 0x00007548 File Offset: 0x00005748
	public string getNameLower()
	{
		return this._nameLower;
	}

	// Token: 0x0600085D RID: 2141 RVA: 0x00007550 File Offset: 0x00005750
	public KeywordDescription[] getKeywords()
	{
		return this._keywords;
	}

	// Token: 0x0600085E RID: 2142 RVA: 0x00007558 File Offset: 0x00005758
	public bool useDummyAnimationBundle()
	{
		return this.animationBundle == 0;
	}

	// Token: 0x17000070 RID: 112
	// (get) Token: 0x0600085F RID: 2143 RVA: 0x00007563 File Offset: 0x00005763
	// (set) Token: 0x06000860 RID: 2144 RVA: 0x0000756B File Offset: 0x0000576B
	public Dictionary<string, object> tags
	{
		get
		{
			return this._tags;
		}
		set
		{
			this._tags = value;
			this._unmodifiedTags = value;
			this._tag = new Tags(value);
		}
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x00007587 File Offset: 0x00005787
	public bool hasTag(string tag)
	{
		return this._tag.has(tag);
	}

	// Token: 0x06000862 RID: 2146 RVA: 0x00044764 File Offset: 0x00042964
	public bool hasAnyTag(params string[] tags)
	{
		foreach (string tag in tags)
		{
			if (this._tag.has(tag))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000863 RID: 2147 RVA: 0x00007595 File Offset: 0x00005795
	public bool getTag<T>(string tag, ref T value)
	{
		return this._tag.get<T>(tag, ref value);
	}

	// Token: 0x06000864 RID: 2148 RVA: 0x000075A4 File Offset: 0x000057A4
	public T getTag<T>(string tag, T value)
	{
		return this._tag.get<T>(tag, value);
	}

	// Token: 0x06000865 RID: 2149 RVA: 0x000075B3 File Offset: 0x000057B3
	private Tags getTags()
	{
		return this._tag;
	}

	// Token: 0x06000866 RID: 2150 RVA: 0x000447A0 File Offset: 0x000429A0
	public void refreshTagsFromDisk()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		string filename = StorageEnvironment.getAddonPath("tags/") + this.name + ".txt";
		string text = FileUtil.readFileContents(filename);
		if (text == null)
		{
			this.getTags().setTo(this._unmodifiedTags);
			return;
		}
		Dictionary<string, object> with = SettingsSerializer.ReadAsTags(text);
		CollectionUtil.updateDict<string, object>(dictionary, with);
		if (dictionary.Count > 0)
		{
			this.getTags().setTo(dictionary);
		}
		else
		{
			this.getTags().setTo(this._unmodifiedTags);
		}
	}

	// Token: 0x06000867 RID: 2151 RVA: 0x0004482C File Offset: 0x00042A2C
	public void writeDefaultTagsToDisk()
	{
		string addonPath = StorageEnvironment.getAddonPath("tags/");
		string text = addonPath + this.name + ".txt";
		Directory.CreateDirectory(addonPath);
		if (!File.Exists(text))
		{
			FileUtil.writeFileContents(text, "# This file (and the sounds themselves) are immediately reloaded\r\n# when changed + saved. Restarting the game is not necessary.\r\n#\r\n# Delays are measured in seconds, and can only be positive.\r\n# Filenames don't use the file extension (tries .wav, then .ogg).\r\n#\r\n# sound_attack_start can be one of: init, hit\r\n# The word defines when to start play the sound, where:\r\n#   init    -> actual attack animation starts (after charging).\r\n#   hit     -> unit hits its opponent. This is the default value.\r\n#\r\n# Example:\r\n#sound_attack = filename_override\r\n#sound_attack_start = init\r\n#sound_attack_delay = 0.5\r\n#sound_attack_pitch_min = 1\r\n#sound_attack_pitch_max = 1.1\r\n#sound_attack_volume = 0.7\r\n#\r\n# You can also define filenames, delays or pitch to sounds other\r\n# than the attack sound, using the same variables. The available\r\n# sounds are:\r\n#\r\n#sound_cast = card_played_filename_override\r\n#\r\n#sound_loadunit = smokescreen_unit_loading_filename_override\r\n#\r\n#sound_summon = unit_summoned_filename_override\r\n#\r\n#sound_prepare = unit_gets_ready_to_attack_filename_override\r\n#\r\n#sound_attack = unit_attack_filename_override\r\n#\r\n#sound_impact = projectile_hits_target_filename_override\r\n#\r\n#sound_ability = unit_activates_its_ability_filename_override\r\n");
		}
	}

	// Token: 0x04000616 RID: 1558
	public int id;

	// Token: 0x04000617 RID: 1559
	public string name = "_dummy";

	// Token: 0x04000618 RID: 1560
	public CardType.Kind kind;

	// Token: 0x04000619 RID: 1561
	public CardType.TypeSet types = new CardType.TypeSet();

	// Token: 0x0400061A RID: 1562
	private string _type;

	// Token: 0x0400061B RID: 1563
	public int hp;

	// Token: 0x0400061C RID: 1564
	public int ap;

	// Token: 0x0400061D RID: 1565
	public int ac;

	// Token: 0x0400061E RID: 1566
	public TargetArea targetArea;

	// Token: 0x0400061F RID: 1567
	public int costDecay;

	// Token: 0x04000620 RID: 1568
	public int costOrder;

	// Token: 0x04000621 RID: 1569
	public int costGrowth;

	// Token: 0x04000622 RID: 1570
	public int costEnergy;

	// Token: 0x04000623 RID: 1571
	private string _nameLower;

	// Token: 0x04000624 RID: 1572
	private string _descriptionAndAbilities;

	// Token: 0x04000625 RID: 1573
	private KeywordDescription[] _keywords;

	// Token: 0x04000626 RID: 1574
	public int rarity;

	// Token: 0x04000627 RID: 1575
	public bool available;

	// Token: 0x04000628 RID: 1576
	public int set;

	// Token: 0x04000629 RID: 1577
	public string description = string.Empty;

	// Token: 0x0400062A RID: 1578
	public string flavor = string.Empty;

	// Token: 0x0400062B RID: 1579
	public ActiveAbility[] abilities = new ActiveAbility[0];

	// Token: 0x0400062C RID: 1580
	public PassiveAbility[] passiveRules = new PassiveAbility[0];

	// Token: 0x0400062D RID: 1581
	public int cardImage;

	// Token: 0x0400062E RID: 1582
	public int animationBundle;

	// Token: 0x0400062F RID: 1583
	public int animationPreviewImage;

	// Token: 0x04000630 RID: 1584
	public string animationPreviewInfo;

	// Token: 0x04000631 RID: 1585
	private Dictionary<string, object> _unmodifiedTags = new Dictionary<string, object>();

	// Token: 0x04000632 RID: 1586
	public Dictionary<string, object> _tags = new Dictionary<string, object>();

	// Token: 0x04000633 RID: 1587
	private Tags _tag = new Tags();

	// Token: 0x02000103 RID: 259
	public enum Kind
	{
		// Token: 0x04000635 RID: 1589
		NONE,
		// Token: 0x04000636 RID: 1590
		SPELL,
		// Token: 0x04000637 RID: 1591
		ENCHANTMENT,
		// Token: 0x04000638 RID: 1592
		CREATURE,
		// Token: 0x04000639 RID: 1593
		STRUCTURE,
		// Token: 0x0400063A RID: 1594
		LAST
	}

	// Token: 0x02000104 RID: 260
	public class TypeSet
	{
		// Token: 0x06000868 RID: 2152 RVA: 0x000075BB File Offset: 0x000057BB
		public TypeSet()
		{
		}

		// Token: 0x06000869 RID: 2153 RVA: 0x00044874 File Offset: 0x00042A74
		public TypeSet(string[] types)
		{
			this.name = string.Join(", ", types);
			this.types = Enumerable.ToArray<string>(Enumerable.Select<string, string>(types, (string t) => t.ToLower()));
		}

		// Token: 0x0600086A RID: 2154 RVA: 0x000075DA File Offset: 0x000057DA
		public bool isType(string type)
		{
			return Enumerable.Contains<string>(this.types, type.ToLower());
		}

		// Token: 0x0600086B RID: 2155 RVA: 0x000075ED File Offset: 0x000057ED
		public override string ToString()
		{
			return this.name;
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x000075F5 File Offset: 0x000057F5
		public static CardType.TypeSet fromTypeString(string t)
		{
			return new CardType.TypeSet(t.Split(new char[]
			{
				','
			}));
		}

		// Token: 0x0400063B RID: 1595
		private string[] types = new string[0];

		// Token: 0x0400063C RID: 1596
		private string name = string.Empty;
	}
}
