using System;

// Token: 0x02000053 RID: 83
public class BattleScenarioTestOutput
{
	// Token: 0x060003AF RID: 943 RVA: 0x00004814 File Offset: 0x00002A14
	public BattleScenarioTestOutput(BattleMode m)
	{
		this.m = m;
	}

	// Token: 0x060003B0 RID: 944 RVA: 0x0002E244 File Offset: 0x0002C444
	public override string ToString()
	{
		return string.Concat(new object[]
		{
			"@Test\npublic void test_",
			new Random().Next(16777216),
			"() {\n",
			this.infiniteResources(),
			this.state(),
			this.setResources(),
			"}"
		});
	}

	// Token: 0x060003B1 RID: 945 RVA: 0x0002E2A8 File Offset: 0x0002C4A8
	private string infiniteResources()
	{
		string text = string.Empty;
		foreach (TileColor color in this.colors())
		{
			ResourceGroup resourceGroup = new ResourceGroup();
			ResourceGroup resourceGroup2 = resourceGroup;
			int num = 1000;
			resourceGroup.ORDER = num;
			num = num;
			resourceGroup.GROWTH = num;
			num = num;
			resourceGroup.ENERGY = num;
			resourceGroup2.DECAY = num;
			text += this.giveRes(color, resourceGroup);
		}
		return text;
	}

	// Token: 0x060003B2 RID: 946 RVA: 0x0002E328 File Offset: 0x0002C528
	private string giveRes(TileColor color, ResourceGroup res)
	{
		string text = this.playerFor(color);
		string text2 = string.Empty;
		string text3 = text2;
		text2 = string.Concat(new object[]
		{
			text3,
			"giveRes(",
			text,
			", ",
			res.DECAY,
			", ResourceType.DECAY);\n"
		});
		text3 = text2;
		text2 = string.Concat(new object[]
		{
			text3,
			"giveRes(",
			text,
			", ",
			res.ENERGY,
			", ResourceType.ENERGY);\n"
		});
		text3 = text2;
		text2 = string.Concat(new object[]
		{
			text3,
			"giveRes(",
			text,
			", ",
			res.GROWTH,
			", ResourceType.GROWTH);\n"
		});
		text3 = text2;
		text2 = string.Concat(new object[]
		{
			text3,
			"giveRes(",
			text,
			", ",
			res.ORDER,
			", ResourceType.ORDER);\n"
		});
		return text2 + "\n";
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x0002E440 File Offset: 0x0002C640
	private string setResources()
	{
		string text = string.Empty;
		foreach (TileColor color in this.colors())
		{
			ResourceGroup res = (!this.m.isLeftColor(color)) ? this.m.battleUI.GetRightPlayerResources() : this.m.battleUI.GetLeftPlayerResources();
			text += this.giveRes(color, res);
		}
		return text;
	}

	// Token: 0x060003B4 RID: 948 RVA: 0x00004823 File Offset: 0x00002A23
	private TileColor[] colors()
	{
		return new TileColor[]
		{
			TileColor.white,
			TileColor.black
		};
	}

	// Token: 0x060003B5 RID: 949 RVA: 0x0002E4BC File Offset: 0x0002C6BC
	private string state()
	{
		string text = string.Empty;
		foreach (TileColor color in this.colors())
		{
			for (int j = 0; j < 5; j++)
			{
				for (int k = 0; k < 3; k++)
				{
					Unit unit = this.m.getUnit(color, j, k);
					if (!(unit == null))
					{
						text = text + this.unit(unit) + "\n";
						text = text + this.state(unit) + "\n\n";
					}
				}
			}
		}
		return text;
	}

	// Token: 0x060003B6 RID: 950 RVA: 0x0002E568 File Offset: 0x0002C768
	private string unit(Unit u)
	{
		TilePosition tilePosition = u.getTilePosition();
		return string.Concat(new string[]
		{
			"Unit ",
			this.unitId(u),
			" = addUnit(",
			this.playerFor(tilePosition),
			", \"",
			u.getName(),
			"\", ",
			this.tileTupleFor(tilePosition),
			");"
		});
	}

	// Token: 0x060003B7 RID: 951 RVA: 0x0002E5D8 File Offset: 0x0002C7D8
	private string state(Unit u)
	{
		CardType cardType = u.getCard().getCardType();
		string text = this.unitId(u);
		string text2 = string.Empty;
		string text3 = text2;
		text2 = string.Concat(new object[]
		{
			text3,
			text,
			".setAc(",
			u.getAttackInterval(),
			");\n"
		});
		text3 = text2;
		text2 = string.Concat(new object[]
		{
			text3,
			text,
			".modifyAp(",
			u.getAttackPower(),
			" - ",
			text,
			".ap());\n"
		});
		text3 = text2;
		text2 = string.Concat(new object[]
		{
			text3,
			text,
			".modifyHp(",
			u.getHitPoints(),
			" - ",
			text,
			".hp());\n"
		});
		foreach (EnchantmentInfo enchantmentInfo in u.getBuffs())
		{
			text3 = text2;
			text2 = string.Concat(new string[]
			{
				text3,
				"enchantUnit(",
				this.playerFor(u.getTilePosition()),
				", ",
				text,
				", \"",
				enchantmentInfo.name,
				"\");\n"
			});
		}
		return text2;
	}

	// Token: 0x060003B8 RID: 952 RVA: 0x0000482F File Offset: 0x00002A2F
	private string playerFor(TilePosition t)
	{
		return this.playerFor(t.color);
	}

	// Token: 0x060003B9 RID: 953 RVA: 0x0000483D File Offset: 0x00002A3D
	private string playerFor(TileColor c)
	{
		return (c != TileColor.white) ? "bp" : "wp";
	}

	// Token: 0x060003BA RID: 954 RVA: 0x00004854 File Offset: 0x00002A54
	private string tileTupleFor(TilePosition t)
	{
		return t.row + ", " + t.column;
	}

	// Token: 0x060003BB RID: 955 RVA: 0x0002E754 File Offset: 0x0002C954
	private string unitId(Unit u)
	{
		TilePosition tilePosition = u.getTilePosition();
		return string.Concat(new object[]
		{
			this.playerFor(tilePosition),
			"_",
			tilePosition.row,
			"_",
			tilePosition.column
		});
	}

	// Token: 0x04000261 RID: 609
	private BattleMode m;
}
