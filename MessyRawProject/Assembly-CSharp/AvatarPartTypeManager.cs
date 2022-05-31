using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x020003A1 RID: 929
public class AvatarPartTypeManager
{
	// Token: 0x060014C5 RID: 5317 RVA: 0x00080324 File Offset: 0x0007E524
	public AvatarPartTypeManager()
	{
		foreach (string set in new string[]
		{
			"MALE_1",
			"FEMALE_1"
		})
		{
			this.feed(new AvatarPart(AvatarPartName.HEAD, "head_1", set));
			this.feed(new AvatarPart(AvatarPartName.ARM_BACK, "back_arm_1", set));
			this.feed(new AvatarPart(AvatarPartName.ARM_FRONT, "front_arm_1", set));
			this.feed(new AvatarPart(AvatarPartName.BODY, "body_1", set));
			this.feed(new AvatarPart(AvatarPartName.LEG, "legs_1", set));
		}
	}

	// Token: 0x060014C7 RID: 5319 RVA: 0x0000F46E File Offset: 0x0000D66E
	public static AvatarPartTypeManager getInstance()
	{
		return AvatarPartTypeManager._instance;
	}

	// Token: 0x060014C8 RID: 5320 RVA: 0x0000F475 File Offset: 0x0000D675
	public void reset()
	{
		this._idToType.Clear();
		this._partToType.Clear();
		this._sets.Clear();
	}

	// Token: 0x060014C9 RID: 5321 RVA: 0x000803F8 File Offset: 0x0007E5F8
	public void feed(AvatarPart t)
	{
		if (t == null)
		{
			return;
		}
		if (t.part == AvatarPartName.INVALID)
		{
			return;
		}
		int num = t.filename.LastIndexOf(".");
		if (num > 0)
		{
			t.filename = t.filename.Substring(0, num);
		}
		this._idToType[t.id] = t;
		if (!this._sets.Contains(t.set))
		{
			this._sets.Add(t.set);
		}
		AvatarPartTypeManager.Key key = new AvatarPartTypeManager.Key(t.part, t.set);
		if (!this._partToType.ContainsKey(key))
		{
			this._partToType.Add(key, new List<AvatarPart>());
		}
		this._partToType[key].Add(t);
	}

	// Token: 0x060014CA RID: 5322 RVA: 0x000804C4 File Offset: 0x0007E6C4
	public void feed(AvatarPart[] types)
	{
		foreach (AvatarPart t in types)
		{
			this.feed(t);
		}
	}

	// Token: 0x060014CB RID: 5323 RVA: 0x000804F4 File Offset: 0x0007E6F4
	public AvatarPart get(int id)
	{
		AvatarPart result;
		if (!this._idToType.TryGetValue(id, ref result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x060014CC RID: 5324 RVA: 0x00080518 File Offset: 0x0007E718
	public AvatarPart get(AvatarPartName type, string set)
	{
		AvatarPartTypeManager.Key key = new AvatarPartTypeManager.Key(type, set);
		if (this._partToType.ContainsKey(key))
		{
			return this._partToType[key][0];
		}
		return this._getFailSafePart(type, set);
	}

	// Token: 0x060014CD RID: 5325 RVA: 0x0008055C File Offset: 0x0007E75C
	public AvatarPart get(AvatarInfo.Part part)
	{
		AvatarPart avatarPart = this.get(part.id);
		if (avatarPart != null)
		{
			return avatarPart;
		}
		Log.warning("AvatarPartTypeManager::get. Couldn't find AvatarPartType with id " + part.id);
		return this._getFailSafePart(part.part, part.set);
	}

	// Token: 0x060014CE RID: 5326 RVA: 0x0000F498 File Offset: 0x0000D698
	private AvatarPart _getFailSafePart(AvatarPartName type, string set)
	{
		if (set != "MALE_1" && set != "FEMALE_1")
		{
			set = AvatarPartTypeManager.getDefaultSet();
		}
		return this.get(type, set);
	}

	// Token: 0x060014CF RID: 5327 RVA: 0x000805AC File Offset: 0x0007E7AC
	public AvatarPart[] getPartsOfType(AvatarPartName type, string set)
	{
		AvatarPartTypeManager.Key key = new AvatarPartTypeManager.Key(type, set);
		List<AvatarPart> list;
		if (this._partToType.TryGetValue(key, ref list))
		{
			List<AvatarPart> list2 = new List<AvatarPart>();
			foreach (AvatarPart avatarPart in list)
			{
				AvatarPart avatarPart2 = this.get(avatarPart.id);
				if (avatarPart2 != null && avatarPart2.id >= 0)
				{
					list2.Add(avatarPart);
				}
			}
			return list2.ToArray();
		}
		Log.warning(string.Concat(new object[]
		{
			"AvatarPartTypeManager::get. Couldn't find AvatarPartTypes with name ",
			type,
			" from set ",
			set
		}));
		if (type != AvatarPartName.INVALID)
		{
			return new AvatarPart[]
			{
				this._getFailSafePart(type, set)
			};
		}
		return null;
	}

	// Token: 0x060014D0 RID: 5328 RVA: 0x00080694 File Offset: 0x0007E894
	public int getFrontArmIdForBackArm(int id)
	{
		AvatarPart avatarPart = this.get(id);
		if (avatarPart.part != AvatarPartName.ARM_BACK)
		{
			avatarPart = this.get(AvatarPartName.ARM_BACK, avatarPart.set);
		}
		string suffix = avatarPart.getSuffix();
		foreach (AvatarPart avatarPart2 in this.getPartsOfType(AvatarPartName.ARM_FRONT, avatarPart.set))
		{
			if (avatarPart2.getSuffix() == suffix)
			{
				return avatarPart2.id;
			}
		}
		return this.get(AvatarPartName.ARM_FRONT, avatarPart.set).id;
	}

	// Token: 0x060014D1 RID: 5329 RVA: 0x0000F4C9 File Offset: 0x0000D6C9
	public int size()
	{
		return this._idToType.Count;
	}

	// Token: 0x060014D2 RID: 5330 RVA: 0x0000F4D6 File Offset: 0x0000D6D6
	public static bool isHeadFrontMost(string set)
	{
		return set == "FEMALE_1";
	}

	// Token: 0x060014D3 RID: 5331 RVA: 0x0000F4E3 File Offset: 0x0000D6E3
	public string[] getSets()
	{
		return this._sets.ToArray();
	}

	// Token: 0x060014D4 RID: 5332 RVA: 0x00080720 File Offset: 0x0007E920
	public string[] getPublicSets()
	{
		List<string> list = new List<string>(this.getSets());
		return Enumerable.ToArray<string>(Enumerable.Where<string>(list, (string x) => !this._hiddenSets.Contains(x)));
	}

	// Token: 0x060014D5 RID: 5333 RVA: 0x00080750 File Offset: 0x0007E950
	public bool hasSet(string set)
	{
		foreach (string text in this._sets)
		{
			if (text == set)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060014D6 RID: 5334 RVA: 0x0000F4F0 File Offset: 0x0000D6F0
	public void hideSet(string set)
	{
		if (!this._hiddenSets.Contains(set))
		{
			this._hiddenSets.Add(set);
		}
	}

	// Token: 0x060014D7 RID: 5335 RVA: 0x0000F50F File Offset: 0x0000D70F
	public string getSetName(AvatarInfo avatar)
	{
		return AvatarPartTypeManager.getSetName(this.getSet_bestGuess(avatar));
	}

	// Token: 0x060014D8 RID: 5336 RVA: 0x0000F51D File Offset: 0x0000D71D
	public static string getSetName(AvatarPart part)
	{
		return AvatarPartTypeManager.getSetName(part.set);
	}

	// Token: 0x060014D9 RID: 5337 RVA: 0x000807BC File Offset: 0x0007E9BC
	public static string getSetName(string set)
	{
		string text = set.ToUpper();
		if (text == "MALE_1")
		{
			return "Male";
		}
		if (text == "FEMALE_1")
		{
			return "Female";
		}
		return string.Empty;
	}

	// Token: 0x060014DA RID: 5338 RVA: 0x00080804 File Offset: 0x0007EA04
	public string getSet_bestGuess(AvatarInfo av)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		int[] array = new int[]
		{
			av.armBack.id,
			av.armFront.id,
			av.body.id,
			av.leg.id,
			av.head.id
		};
		foreach (int id in array)
		{
			AvatarPart avatarPart = this.get(id);
			if (avatarPart != null)
			{
				if (!dictionary.ContainsKey(avatarPart.set))
				{
					dictionary.Add(avatarPart.set, 1);
				}
				else
				{
					Dictionary<string, int> dictionary3;
					Dictionary<string, int> dictionary2 = dictionary3 = dictionary;
					string set;
					string text = set = avatarPart.set;
					int num = dictionary3[set];
					dictionary2[text] = num + 1;
				}
			}
		}
		string result = null;
		int num2 = -1;
		foreach (string text2 in dictionary.Keys)
		{
			int num3 = dictionary[text2];
			if (num3 > num2)
			{
				num2 = num3;
				result = text2;
			}
		}
		return result;
	}

	// Token: 0x060014DB RID: 5339 RVA: 0x0000F52A File Offset: 0x0000D72A
	public static string getDefaultSet()
	{
		return "MALE_1";
	}

	// Token: 0x060014DC RID: 5340 RVA: 0x0008094C File Offset: 0x0007EB4C
	public bool isUnlocked(int id)
	{
		AvatarPart avatarPart = this.get(id);
		return avatarPart != null && (avatarPart.type == AvatarPartRarity.COMMON || this._unlocked.Contains(id));
	}

	// Token: 0x060014DD RID: 5341 RVA: 0x0000F531 File Offset: 0x0000D731
	public void unlock(int id)
	{
		if (!this.isUnlocked(id))
		{
			this._unlocked.Add(id);
		}
	}

	// Token: 0x040011F4 RID: 4596
	private List<int> _unlocked = new List<int>();

	// Token: 0x040011F5 RID: 4597
	private Dictionary<int, AvatarPart> _idToType = new Dictionary<int, AvatarPart>();

	// Token: 0x040011F6 RID: 4598
	private Dictionary<AvatarPartTypeManager.Key, List<AvatarPart>> _partToType = new Dictionary<AvatarPartTypeManager.Key, List<AvatarPart>>();

	// Token: 0x040011F7 RID: 4599
	private List<string> _sets = new List<string>();

	// Token: 0x040011F8 RID: 4600
	private List<string> _hiddenSets = new List<string>();

	// Token: 0x040011F9 RID: 4601
	private static AvatarPartTypeManager _instance = new AvatarPartTypeManager();

	// Token: 0x020003A2 RID: 930
	private class Key : IEquatable<AvatarPartTypeManager.Key>
	{
		// Token: 0x060014DF RID: 5343 RVA: 0x0000F55C File Offset: 0x0000D75C
		public Key(AvatarPartName type, string set)
		{
			this.type = type;
			this.set = set;
		}

		// Token: 0x060014E0 RID: 5344 RVA: 0x0000F572 File Offset: 0x0000D772
		public bool Equals(AvatarPartTypeManager.Key other)
		{
			return this.type == other.type && this.set == other.set;
		}

		// Token: 0x060014E1 RID: 5345 RVA: 0x0000F599 File Offset: 0x0000D799
		public override bool Equals(object obj)
		{
			return obj is AvatarPartTypeManager.Key && this.Equals((AvatarPartTypeManager.Key)obj);
		}

		// Token: 0x060014E2 RID: 5346 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
		public override int GetHashCode()
		{
			return (int)(this.type + ((this.set != null) ? this.set.GetHashCode() : 0));
		}

		// Token: 0x040011FA RID: 4602
		public AvatarPartName type;

		// Token: 0x040011FB RID: 4603
		public string set;
	}
}
