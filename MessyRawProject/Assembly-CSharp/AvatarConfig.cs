using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x0200039C RID: 924
public class AvatarConfig
{
	// Token: 0x060014AE RID: 5294 RVA: 0x00002DDA File Offset: 0x00000FDA
	private AvatarConfig()
	{
	}

	// Token: 0x060014AF RID: 5295 RVA: 0x0000F39F File Offset: 0x0000D59F
	public AvatarConfigPart getConfigPart(int index)
	{
		return this._parts[index];
	}

	// Token: 0x060014B0 RID: 5296 RVA: 0x00080024 File Offset: 0x0007E224
	public static AvatarConfig createConfig(string set, bool includeLocked)
	{
		AvatarPartTypeManager instance = AvatarPartTypeManager.getInstance();
		if (!instance.hasSet(set))
		{
			set = AvatarPartTypeManager.getDefaultSet();
		}
		AvatarConfig avatarConfig = new AvatarConfig();
		avatarConfig.set = set;
		avatarConfig.head = AvatarConfig._config(set, AvatarPartName.HEAD, includeLocked);
		avatarConfig.body = AvatarConfig._config(set, AvatarPartName.BODY, includeLocked);
		avatarConfig.leg = AvatarConfig._config(set, AvatarPartName.LEG, includeLocked);
		avatarConfig.arm = AvatarConfig._config(set, AvatarPartName.ARM_BACK, includeLocked);
		avatarConfig.arm2 = AvatarConfig._config(set, AvatarPartName.ARM_FRONT, includeLocked);
		avatarConfig._parts = new AvatarConfigPart[]
		{
			avatarConfig.arm,
			avatarConfig.leg,
			avatarConfig.body,
			avatarConfig.head,
			avatarConfig.arm2
		};
		return avatarConfig;
	}

	// Token: 0x060014B1 RID: 5297 RVA: 0x000800D8 File Offset: 0x0007E2D8
	public static AvatarConfig loadConfig(string set, int[] ids, bool includeLocked)
	{
		if (ids.Length != 5)
		{
			return null;
		}
		AvatarConfig avatarConfig = AvatarConfig.createConfig(set, includeLocked);
		for (int i = 0; i < 5; i++)
		{
			avatarConfig.getConfigPart(i).setId(ids[i]);
		}
		return avatarConfig;
	}

	// Token: 0x060014B2 RID: 5298 RVA: 0x0008011C File Offset: 0x0007E31C
	public static AvatarConfig loadConfig(AvatarInfo av, bool includeLocked)
	{
		return AvatarConfig.loadConfig(AvatarPartTypeManager.getInstance().getSet_bestGuess(av), new int[]
		{
			av.armBack.id,
			av.leg.id,
			av.body.id,
			av.head.id,
			av.armFront.id
		}, includeLocked);
	}

	// Token: 0x060014B3 RID: 5299 RVA: 0x00080188 File Offset: 0x0007E388
	private static AvatarConfigPart _config(string set, AvatarPartName type, bool includeLocked)
	{
		AvatarPartTypeManager m = AvatarPartTypeManager.getInstance();
		List<AvatarPart> list = new List<AvatarPart>();
		foreach (string text in new string[]
		{
			set
		})
		{
			list.AddRange(Enumerable.Where<AvatarPart>(m.getPartsOfType(type, text), (AvatarPart p) => includeLocked || m.isUnlocked(p.id)));
		}
		AvatarPart[] array2 = list.ToArray();
		int[] array3 = new int[array2.Length];
		for (int j = 0; j < array3.Length; j++)
		{
			array3[j] = array2[j].id;
		}
		return new AvatarConfigPart(array3);
	}

	// Token: 0x040011E8 RID: 4584
	public AvatarConfigPart head;

	// Token: 0x040011E9 RID: 4585
	public AvatarConfigPart leg;

	// Token: 0x040011EA RID: 4586
	public AvatarConfigPart arm;

	// Token: 0x040011EB RID: 4587
	public AvatarConfigPart body;

	// Token: 0x040011EC RID: 4588
	public AvatarConfigPart arm2;

	// Token: 0x040011ED RID: 4589
	public string set;

	// Token: 0x040011EE RID: 4590
	private AvatarConfigPart[] _parts;
}
