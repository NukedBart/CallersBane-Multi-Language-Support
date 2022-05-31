using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020000A5 RID: 165
public class TutorialTicker : ITutorial
{
	// Token: 0x060005DE RID: 1502 RVA: 0x0003AB14 File Offset: 0x00038D14
	public TutorialTicker(TutorialTicker.Line[] lines)
	{
		this._lines = new List<TutorialTicker.Line>();
		this._current = this._lines.Count;
		this._lines.AddRange(lines);
		this._end = this._lines.Count;
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x00005A8B File Offset: 0x00003C8B
	protected void addLine(TutorialTicker.Line line)
	{
		this._lines.Add(line);
		this._end = this._lines.Count;
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x00005AAA File Offset: 0x00003CAA
	protected bool hasLines()
	{
		return this._lines.Count > 0;
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x0003AB60 File Offset: 0x00038D60
	public string getText()
	{
		string text = this.current().text;
		if (text != string.Empty)
		{
			this.lastText = text;
		}
		return this.lastText;
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x00005ABA File Offset: 0x00003CBA
	public void next()
	{
		if (this._current < this._lines.Count)
		{
			this._current++;
		}
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x00005AE0 File Offset: 0x00003CE0
	public bool isAny(params TutorialTicker.Tag[] tags)
	{
		return this.isAny(this._current, tags);
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x00005AEF File Offset: 0x00003CEF
	public bool isRunning()
	{
		return this._current < this._end;
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x00005AFF File Offset: 0x00003CFF
	public bool isBlocking()
	{
		return this.isEmpty() || this.isAny(new TutorialTicker.Tag[1]);
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x00005B1B File Offset: 0x00003D1B
	public bool shouldZoomCard(Card card)
	{
		return this.isAny(new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.ZoomCard
		});
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x00005B2E File Offset: 0x00003D2E
	public bool allowSacrifice(ResourceType resource)
	{
		if (resource == ResourceType.CARDS)
		{
			return this.isAny(new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.SacrificeCards
			});
		}
		return this.isAny(new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.SacrificeResource
		});
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x00005B5A File Offset: 0x00003D5A
	public bool allowEndTurn()
	{
		return this.isAny(new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.EndTurn
		});
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x00005B6C File Offset: 0x00003D6C
	public bool allowHideCardView()
	{
		return !this.isAfter(TutorialTicker.Tag.HideCardView_Off, TutorialTicker.Tag.HideCardView_On);
	}

	// Token: 0x060005EA RID: 1514 RVA: 0x00005B7B File Offset: 0x00003D7B
	public bool allowMove()
	{
		return !this.isAfter(TutorialTicker.Tag.Move_Off, TutorialTicker.Tag.Move_On);
	}

	// Token: 0x060005EB RID: 1515 RVA: 0x00005B8A File Offset: 0x00003D8A
	public bool allowPlayCard(Card card)
	{
		return this.isAny(new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.CardPlayed,
			TutorialTicker.Tag.SummonUnit,
			TutorialTicker.Tag.AllowCardPlayed
		});
	}

	// Token: 0x060005EC RID: 1516 RVA: 0x0003AB98 File Offset: 0x00038D98
	private bool isEmpty()
	{
		List<TutorialTicker.Tag> tags = this.getTags();
		foreach (TutorialTicker.Tag tag in tags)
		{
			if (tag != TutorialTicker.Tag.HandUpdate)
			{
				string text = tag.ToString();
				if (!text.StartsWith("Blink_"))
				{
					if (!text.StartsWith("No_"))
					{
						if (!text.EndsWith("_On"))
						{
							if (!text.EndsWith("_Off"))
							{
								return false;
							}
						}
					}
				}
			}
		}
		return true;
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x0003AC68 File Offset: 0x00038E68
	private bool isAfter(TutorialTicker.Tag what, TutorialTicker.Tag than)
	{
		int pre = this.getPre(what);
		if (!this.has(pre))
		{
			return false;
		}
		int pre2 = this.getPre(than);
		return pre > pre2;
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x00005BA6 File Offset: 0x00003DA6
	private int getPre(TutorialTicker.Tag value)
	{
		return this.getPre(this._current, value);
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x0003AC98 File Offset: 0x00038E98
	private int getPre(int frame, TutorialTicker.Tag value)
	{
		for (int i = frame; i >= 0; i--)
		{
			if (this.isAny(i, new TutorialTicker.Tag[]
			{
				value
			}))
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060005F0 RID: 1520 RVA: 0x00005BB5 File Offset: 0x00003DB5
	private bool has(int i)
	{
		return i >= 0;
	}

	// Token: 0x060005F1 RID: 1521 RVA: 0x00005BBE File Offset: 0x00003DBE
	private int getPost(TutorialTicker.Tag value)
	{
		return this.getPost(this._current, value);
	}

	// Token: 0x060005F2 RID: 1522 RVA: 0x0003ACD0 File Offset: 0x00038ED0
	private int getPost(int frame, TutorialTicker.Tag value)
	{
		for (int i = frame + 1; i < this._lines.Count; i++)
		{
			if (this.isAny(i, new TutorialTicker.Tag[]
			{
				value
			}))
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060005F3 RID: 1523 RVA: 0x0003AD14 File Offset: 0x00038F14
	private bool isAny(int index, params TutorialTicker.Tag[] ofTags)
	{
		return this.get(index, ofTags) != null;
	}

	// Token: 0x060005F4 RID: 1524 RVA: 0x00005BCD File Offset: 0x00003DCD
	private TutorialTicker.Tag? get(params TutorialTicker.Tag[] ofTags)
	{
		return this.get(this._current, ofTags);
	}

	// Token: 0x060005F5 RID: 1525 RVA: 0x0003AD34 File Offset: 0x00038F34
	private TutorialTicker.Tag? get(int index, params TutorialTicker.Tag[] ofTags)
	{
		if (index < 0 || index >= this._lines.Count)
		{
			return default(TutorialTicker.Tag?);
		}
		TutorialTicker.Tag[] tags = this._lines[index].getTags();
		foreach (TutorialTicker.Tag tag in ofTags)
		{
			foreach (TutorialTicker.Tag tag2 in tags)
			{
				if (tag == tag2)
				{
					return new TutorialTicker.Tag?(tag2);
				}
			}
		}
		return default(TutorialTicker.Tag?);
	}

	// Token: 0x060005F6 RID: 1526 RVA: 0x00005BDC File Offset: 0x00003DDC
	public bool onTileClicked(TilePosition pos, Unit unit)
	{
		return unit != null && this.isAny(new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.ClickUnit
		});
	}

	// Token: 0x060005F7 RID: 1527 RVA: 0x00005B5A File Offset: 0x00003D5A
	public bool onEndTurn()
	{
		return this.isAny(new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.EndTurn
		});
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x000059E4 File Offset: 0x00003BE4
	public bool onTurnBegin(int turnId)
	{
		return false;
	}

	// Token: 0x060005F9 RID: 1529 RVA: 0x00005BFD File Offset: 0x00003DFD
	public bool onCardSacrificed(ResourceType res)
	{
		if (res.isCards())
		{
			return this.isAny(new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.Sacrifice,
				TutorialTicker.Tag.SacrificeCards
			});
		}
		return this.isAny(new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.Sacrifice,
			TutorialTicker.Tag.SacrificeResource
		});
	}

	// Token: 0x060005FA RID: 1530 RVA: 0x00005C37 File Offset: 0x00003E37
	public bool onCardPlayed(Card card)
	{
		return this.isAny(new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.CardPlayed
		});
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x0003ADD0 File Offset: 0x00038FD0
	public bool onCardClicked(BattleMode m, Card card)
	{
		FilterData data = new FilterData(m, card);
		return this.isAny(new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.CardClicked
		}) && this.filter(new TilePosition[]
		{
			new TilePosition()
		}, data).Length > 0;
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x00005C4A File Offset: 0x00003E4A
	public bool onHandUpdate()
	{
		return this.isAny(new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.HandUpdate
		});
	}

	// Token: 0x060005FD RID: 1533 RVA: 0x00005C5D File Offset: 0x00003E5D
	public bool onMoveUnit(Unit unit)
	{
		return this.isAny(new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.MoveUnit
		});
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x00005C70 File Offset: 0x00003E70
	public bool onSummonUnit(Unit unit)
	{
		return this.isAny(new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.SummonUnit
		});
	}

	// Token: 0x060005FF RID: 1535 RVA: 0x00005C83 File Offset: 0x00003E83
	public TilePosition[] filter(TilePosition[] pos, FilterData data)
	{
		return this.current().filter(pos, data);
	}

	// Token: 0x06000600 RID: 1536 RVA: 0x00005C92 File Offset: 0x00003E92
	public List<TutorialTicker.Tag> getTags()
	{
		return Enumerable.ToList<TutorialTicker.Tag>(this.current().getTags());
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x0003AE18 File Offset: 0x00039018
	private TutorialTicker.Line current()
	{
		if (this._current < 0 || this._current >= this._lines.Count)
		{
			return TutorialTicker.p("<Ugh!!>", new TutorialTicker.Tag[0]);
		}
		return this._lines[this._current];
	}

	// Token: 0x06000602 RID: 1538 RVA: 0x00005A65 File Offset: 0x00003C65
	private static TutorialTicker.Line p(string text, params TutorialTicker.Tag[] tags)
	{
		return new TutorialTicker.Line(text, tags);
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x0003AE6C File Offset: 0x0003906C
	protected static TutorialTicker.Line.TileFilter Row(params int[] rows)
	{
		return (TilePosition tile, FilterData d) => Enumerable.Contains<int>(rows, tile.row);
	}

	// Token: 0x06000604 RID: 1540 RVA: 0x0003AE94 File Offset: 0x00039094
	protected static TutorialTicker.Line.TileFilter NotRow(params int[] rows)
	{
		return (TilePosition tile, FilterData d) => !Enumerable.Contains<int>(rows, tile.row);
	}

	// Token: 0x06000605 RID: 1541 RVA: 0x0003AEBC File Offset: 0x000390BC
	protected static TutorialTicker.Line.TileFilter Column(params int[] columns)
	{
		return (TilePosition tile, FilterData d) => Enumerable.Contains<int>(columns, tile.column);
	}

	// Token: 0x06000606 RID: 1542 RVA: 0x0003AEE4 File Offset: 0x000390E4
	protected static TutorialTicker.Line.TileFilter NotColumn(params int[] columns)
	{
		return (TilePosition tile, FilterData d) => !Enumerable.Contains<int>(columns, tile.column);
	}

	// Token: 0x06000607 RID: 1543 RVA: 0x0003AF0C File Offset: 0x0003910C
	protected static TutorialTicker.Line.TileFilter Tile(int row, int column)
	{
		return (TilePosition tile, FilterData d) => tile.row == row && tile.column == column;
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x00005CA4 File Offset: 0x00003EA4
	protected static TutorialTicker.Line.TileFilter None()
	{
		return (TilePosition tile, FilterData d) => false;
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x00005CC3 File Offset: 0x00003EC3
	protected static TutorialTicker.Line.TileFilter OpponentTiles()
	{
		return (TilePosition tile, FilterData d) => !d.game.isPlayer(tile.color);
	}

	// Token: 0x0600060A RID: 1546 RVA: 0x00005CE2 File Offset: 0x00003EE2
	protected static TutorialTicker.Line.TileFilter PlayerTiles()
	{
		return (TilePosition tile, FilterData d) => d.game.isPlayer(tile.color);
	}

	// Token: 0x0600060B RID: 1547 RVA: 0x00005D01 File Offset: 0x00003F01
	protected static TutorialTicker.Line.TileFilter ChangeRow()
	{
		return (TilePosition tile, FilterData d) => d.unit == null || !ActiveAbility.isMoveLike(d.ability) || d.unit.getTilePosition().row != tile.row;
	}

	// Token: 0x0600060C RID: 1548 RVA: 0x00005D20 File Offset: 0x00003F20
	protected static TutorialTicker.Line.TileFilter HasIdol()
	{
		return (TilePosition tile, FilterData d) => d.game.getOpposingIdol(tile).alive();
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x00005D3F File Offset: 0x00003F3F
	protected static TutorialTicker.Line.TileFilter HasIdol3()
	{
		return (TilePosition tile, FilterData d) => d.game.getOpposingIdol(tile).alive() && (tile.row == 0 || d.game.getOpposingIdol(new TilePosition(tile.color, tile.row - 1, tile.column)).alive()) && (tile.row == 4 || d.game.getOpposingIdol(new TilePosition(tile.color, tile.row + 1, tile.column)).alive());
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x00005D5E File Offset: 0x00003F5E
	protected static TutorialTicker.Line.TileFilter HasAttackingEnemy()
	{
		return TutorialTicker.HasAttackingEnemy(1);
	}

	// Token: 0x0600060F RID: 1551 RVA: 0x0003AF3C File Offset: 0x0003913C
	protected static TutorialTicker.Line.TileFilter HasAttackingEnemy(int needed)
	{
		return delegate(TilePosition tile, FilterData d)
		{
			int num = 0;
			TileColor color = tile.color.otherColor();
			TilePosition tilePosition = new TilePosition(color, tile.row, 0);
			for (int i = 0; i < 3; i++)
			{
				tilePosition.column = i;
				Unit unit = d.game.getUnit(tilePosition);
				if (!(unit == null))
				{
					if (unit.getAttackInterval() <= 1)
					{
						if (unit.getAttackPower() > 0)
						{
							num++;
						}
					}
				}
			}
			return num >= needed;
		};
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x0003AF64 File Offset: 0x00039164
	protected static TutorialTicker.Line.TileFilter Unit(string name)
	{
		return (TilePosition tile, FilterData d) => d.unitAt() != null && d.unitAt().getCard().isType(name);
	}

	// Token: 0x06000611 RID: 1553 RVA: 0x0003AF8C File Offset: 0x0003918C
	protected static TutorialTicker.Line.TileFilter IfUnit(string name)
	{
		return (TilePosition tile, FilterData d) => d.unitAt() == null || d.unitAt().getCard().isType(name);
	}

	// Token: 0x06000612 RID: 1554 RVA: 0x00005D66 File Offset: 0x00003F66
	protected static TutorialTicker.Line.TileFilter IfUnitAttack()
	{
		return (TilePosition tile, FilterData d) => d.unitAt() == null || d.unitAt().getAttackInterval() == 0;
	}

	// Token: 0x06000613 RID: 1555 RVA: 0x0003AFB4 File Offset: 0x000391B4
	protected static TutorialTicker.Line.TileFilter Card(string name)
	{
		return (TilePosition tile, FilterData d) => (d.unit != null && d.card == null) || (d.card != null && d.card.isType(name));
	}

	// Token: 0x06000614 RID: 1556 RVA: 0x0003AFDC File Offset: 0x000391DC
	protected static TutorialTicker.Line.TileFilter Cost(int cost)
	{
		return (TilePosition tile, FilterData d) => d.card != null && d.card.getCardType().getCost() == cost;
	}

	// Token: 0x06000615 RID: 1557 RVA: 0x00005D85 File Offset: 0x00003F85
	protected static TutorialTicker.Line.TileFilter RowHasNoAttacker()
	{
		return (TilePosition tile, FilterData d) => !Enumerable.Any<Unit>(d.game.getUnitsFor(tile.color), (Unit u) => u.getTilePosition().row == tile.row && u.getAttackInterval() == 0);
	}

	// Token: 0x04000424 RID: 1060
	private List<TutorialTicker.Line> _lines;

	// Token: 0x04000425 RID: 1061
	private int _current;

	// Token: 0x04000426 RID: 1062
	private int _end;

	// Token: 0x04000427 RID: 1063
	private string lastText;

	// Token: 0x020000A6 RID: 166
	public enum Tag
	{
		// Token: 0x04000431 RID: 1073
		Ok,
		// Token: 0x04000432 RID: 1074
		No_EndTurn,
		// Token: 0x04000433 RID: 1075
		No_PlayCard,
		// Token: 0x04000434 RID: 1076
		No_Sacrifice,
		// Token: 0x04000435 RID: 1077
		No_SacrificeCards,
		// Token: 0x04000436 RID: 1078
		No_SacrificeResource,
		// Token: 0x04000437 RID: 1079
		AllowCardPlayed,
		// Token: 0x04000438 RID: 1080
		ClickUnit,
		// Token: 0x04000439 RID: 1081
		EndTurn,
		// Token: 0x0400043A RID: 1082
		Sacrifice,
		// Token: 0x0400043B RID: 1083
		SacrificeCards,
		// Token: 0x0400043C RID: 1084
		SacrificeResource,
		// Token: 0x0400043D RID: 1085
		MoveUnit,
		// Token: 0x0400043E RID: 1086
		SummonUnit,
		// Token: 0x0400043F RID: 1087
		CardClicked,
		// Token: 0x04000440 RID: 1088
		CardPlayed,
		// Token: 0x04000441 RID: 1089
		HandUpdate,
		// Token: 0x04000442 RID: 1090
		ZoomCard,
		// Token: 0x04000443 RID: 1091
		HideCardView_Off,
		// Token: 0x04000444 RID: 1092
		HideCardView_On,
		// Token: 0x04000445 RID: 1093
		Move_Off,
		// Token: 0x04000446 RID: 1094
		Move_On,
		// Token: 0x04000447 RID: 1095
		Blink_Cost,
		// Token: 0x04000448 RID: 1096
		Blink_Attack,
		// Token: 0x04000449 RID: 1097
		Blink_Countdown,
		// Token: 0x0400044A RID: 1098
		Blink_Health,
		// Token: 0x0400044B RID: 1099
		Blink_Unit,
		// Token: 0x0400044C RID: 1100
		Blink_ResourceCounter,
		// Token: 0x0400044D RID: 1101
		Blink_Cards,
		// Token: 0x0400044E RID: 1102
		Blink_Cast,
		// Token: 0x0400044F RID: 1103
		Blink_Idol
	}

	// Token: 0x020000A7 RID: 167
	public class Line
	{
		// Token: 0x0600061E RID: 1566 RVA: 0x0003B164 File Offset: 0x00039364
		public Line(string text, TutorialTicker.Tag[] tags)
		{
			if (text != null)
			{
				string text2 = "#ffcc66";
				string text3 = "#ffcc66";
				string text4 = "#ffffff";
				text = text.Replace("{{", "<color=" + text2 + ">").Replace("}}", "</color>").Replace("[[", "<color=" + text3 + ">").Replace("]]", "</color>").Replace("((", "<color=" + text4 + ">").Replace("))", "</color>");
				foreach (ResourceType resourceType in CollectionUtil.enumValues<ResourceType>())
				{
					string text5 = StringUtil.capitalize(resourceType.ToString());
					if (resourceType == ResourceType.SPECIAL)
					{
						text5 = "Wild";
					}
					Color color = ColorUtil.Brighten(ColorUtil.Darken(ResourceColor.get(resourceType), 0.5f), 0.2f);
					string text6 = GUIUtil.RtColor(text5, color);
					text = text.Replace(text5, text6);
				}
				if (text != string.Empty)
				{
					text = "<color=#aa9988>" + text + "</color>";
				}
			}
			this.text = text;
			this.add(tags);
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x0003B2E4 File Offset: 0x000394E4
		private bool hasOnly(params TutorialTicker.Tag[] tags)
		{
			foreach (TutorialTicker.Tag tag in this._tags)
			{
				if (!Enumerable.Contains<TutorialTicker.Tag>(tags, tag))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x00005DE0 File Offset: 0x00003FE0
		public TutorialTicker.Line proceed(params TutorialTicker.Tag[] tags)
		{
			this._proceed.AddRange(tags);
			return this;
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x00005DEF File Offset: 0x00003FEF
		public TutorialTicker.Line allow(params TutorialTicker.Tag[] tags)
		{
			this._allow.AddRange(tags);
			return this;
		}

		// Token: 0x06000622 RID: 1570 RVA: 0x00005DFE File Offset: 0x00003FFE
		public TutorialTicker.Line add(params TutorialTicker.Tag[] tags)
		{
			this._tags.AddRange(tags);
			return this;
		}

		// Token: 0x06000623 RID: 1571 RVA: 0x00005E0D File Offset: 0x0000400D
		public TutorialTicker.Line filter(params TutorialTicker.Line.TileFilter[] fl)
		{
			this._filters.AddRange(fl);
			return this;
		}

		// Token: 0x06000624 RID: 1572 RVA: 0x0003B350 File Offset: 0x00039550
		public TutorialTicker.Tag[] getTags()
		{
			List<TutorialTicker.Tag> list = new List<TutorialTicker.Tag>();
			list.AddRange(this._tags);
			list.AddRange(this._allow);
			list.AddRange(this._proceed);
			return list.ToArray();
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x0003B390 File Offset: 0x00039590
		public TilePosition[] filter(TilePosition[] pos, FilterData data)
		{
			return Enumerable.ToArray<TilePosition>(Enumerable.Where<TilePosition>(pos, (TilePosition tile) => this.filter(tile, data.setupFor(tile))));
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x0003B3C8 File Offset: 0x000395C8
		private bool filter(TilePosition pos, FilterData data)
		{
			foreach (TutorialTicker.Line.TileFilter tileFilter in this._filters)
			{
				if (!tileFilter(pos, data))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x04000450 RID: 1104
		public string text;

		// Token: 0x04000451 RID: 1105
		private List<TutorialTicker.Tag> _tags = new List<TutorialTicker.Tag>();

		// Token: 0x04000452 RID: 1106
		private List<TutorialTicker.Tag> _allow = new List<TutorialTicker.Tag>();

		// Token: 0x04000453 RID: 1107
		private List<TutorialTicker.Tag> _proceed = new List<TutorialTicker.Tag>();

		// Token: 0x04000454 RID: 1108
		private List<TutorialTicker.Line.TileFilter> _filters = new List<TutorialTicker.Line.TileFilter>();

		// Token: 0x020000A8 RID: 168
		// (Invoke) Token: 0x06000628 RID: 1576
		public delegate bool TileFilter(TilePosition p, FilterData data);
	}
}
