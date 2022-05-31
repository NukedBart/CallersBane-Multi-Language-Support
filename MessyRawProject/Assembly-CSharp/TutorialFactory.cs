using System;

// Token: 0x020000A4 RID: 164
public class TutorialFactory : TutorialTicker
{
	// Token: 0x060005D7 RID: 1495 RVA: 0x00005A5C File Offset: 0x00003C5C
	private TutorialFactory() : base(null)
	{
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x00005A65 File Offset: 0x00003C65
	private static TutorialTicker.Line p(string text, params TutorialTicker.Tag[] tags)
	{
		return new TutorialTicker.Line(text, tags);
	}

	// Token: 0x060005D9 RID: 1497 RVA: 0x0003A350 File Offset: 0x00038550
	public static ITutorial getTutorialForDeck(int refId)
	{
		if (refId == 73)
		{
			return new TutorialTicker(TutorialFactory.testTutorial0());
		}
		if (refId == 100001 || refId == 58)
		{
			return new TutorialTicker(TutorialFactory.testTutorial1());
		}
		if (refId == 100002 || refId == 59)
		{
			return new TutorialTicker(TutorialFactory.testTutorial2());
		}
		return new AiTutorialTicker();
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x00005A6E File Offset: 0x00003C6E
	public static bool isScriptedTutorial(ITutorial tutorial)
	{
		return tutorial != null && tutorial.GetType() == typeof(TutorialTicker);
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x0003A3B4 File Offset: 0x000385B4
	private static TutorialTicker.Line[] testTutorial0()
	{
		return new TutorialTicker.Line[]
		{
			TutorialFactory.p(null, new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.HandUpdate
			}),
			TutorialFactory.p(I18n.Text("{{Welcome!}}\nYou win a game of {GAME_NAME} by destroying {{three}} of your opponent's {{idols}}."), new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.Blink_Idol
			}),
			TutorialFactory.p("Seems you're in a good place already! ((Click the hourglass button)) to end your turn and see how this plays out.", new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.Move_Off,
				TutorialTicker.Tag.EndTurn
			})
		};
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x0003A414 File Offset: 0x00038614
	private static TutorialTicker.Line[] testTutorial1()
	{
		TutorialTicker.Line[] array = new TutorialTicker.Line[25];
		array[0] = TutorialFactory.p(null, new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.HandUpdate
		});
		array[1] = TutorialFactory.p("{{Welcome!}}\nAs mentioned in the previous tutorial, the goal is to destroy {{three}} of your opponent's {{idols}}.", new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.Blink_Idol
		});
		array[2] = TutorialFactory.p("Casting scrolls costs resources. To gain your first Growth resource, ((click any scroll)), then sacrifice it by ((clicking the resource icon)) that appears to the left.", new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.CardClicked,
			TutorialTicker.Tag.Blink_Cards
		});
		array[3] = TutorialFactory.p(string.Empty, new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.SacrificeResource,
			TutorialTicker.Tag.Move_Off
		});
		array[4] = TutorialFactory.p("Great! Now you have enough Growth resources to play a creature. ((Select the)) [[Beast Rat]] scroll.", new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.AllowCardPlayed,
			TutorialTicker.Tag.CardClicked,
			TutorialTicker.Tag.Blink_Cards
		}).filter(new TutorialTicker.Line.TileFilter[]
		{
			TutorialTicker.Card("Beast Rat")
		});
		int num = 5;
		string text = "Your resources are shown in the lower left corner of the screen. Summoning the Rat is about to cost you one Growth.";
		TutorialTicker.Tag[] array2 = new TutorialTicker.Tag[3];
		array2[0] = TutorialTicker.Tag.AllowCardPlayed;
		array2[1] = TutorialTicker.Tag.Blink_ResourceCounter;
		array[num] = TutorialFactory.p(text, array2).filter(new TutorialTicker.Line.TileFilter[]
		{
			TutorialTicker.Row(new int[]
			{
				2
			})
		});
		array[6] = TutorialFactory.p("Controlling the middle row is good strategy. ((Click a tile)) to summon your Rat.", new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.SummonUnit
		}).filter(new TutorialTicker.Line.TileFilter[]
		{
			TutorialTicker.Row(new int[]
			{
				2
			})
		});
		array[7] = TutorialFactory.p("You've spent all your resources for this turn. Don't worry, {{you'll get them back next turn}}.", new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.Blink_ResourceCounter
		});
		array[8] = TutorialFactory.p("((Click the)) [[Beast Rat]] to take a closer look.", new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.ClickUnit,
			TutorialTicker.Tag.Blink_Unit
		});
		array[9] = TutorialFactory.p("Each creature has a {{cost}}, along with three primary values: Attack, Countdown, and Health.", new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.Blink_Cost,
			TutorialTicker.Tag.HideCardView_Off
		});
		array[10] = TutorialFactory.p("{{Attack}} shows how much damage it deals to an opponent's unit or idol.", new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.Blink_Attack
		});
		array[11] = TutorialFactory.p("{{Health}} shows how much damage it can take.", new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.Blink_Health
		});
		array[12] = TutorialFactory.p("A creature's {{Countdown}} is decreased by one each turn. When it reaches zero, they'll attack at the end of the turn.", new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.Blink_Countdown
		});
		array[13] = TutorialFactory.p("((Click the hourglass button)) to end your turn.", new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.HideCardView_On,
			TutorialTicker.Tag.EndTurn
		});
		array[14] = TutorialFactory.p(null, new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.HandUpdate
		});
		array[15] = TutorialFactory.p("Splendid! As you may have noticed, {{you draw 1 scroll at the beginning of each turn}}.", new TutorialTicker.Tag[0]);
		array[16] = TutorialFactory.p("Once again, ((sacrifice a scroll for resources)), increasing your resource count to two. Then ((play a 2-cost Kinfolk Ranger)).", new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.Blink_Cards,
			TutorialTicker.Tag.CardClicked
		});
		array[17] = TutorialFactory.p(string.Empty, new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.SacrificeResource
		});
		array[18] = TutorialFactory.p(string.Empty, new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.Blink_Cards,
			TutorialTicker.Tag.SummonUnit
		}).filter(new TutorialTicker.Line.TileFilter[]
		{
			TutorialTicker.Cost(2)
		});
		array[19] = TutorialFactory.p("Great. You'll take down those idols in no time. ((End the turn)).", new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.EndTurn
		});
		array[20] = TutorialFactory.p(null, new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.HandUpdate
		});
		array[21] = TutorialFactory.p("The [[Beast Rat]] is flashing because it's about to attack. When you end your turn, it will hit the nearest opponent unit or idol on its row.", new TutorialTicker.Tag[0]);
		array[22] = TutorialFactory.p("You can {{move}} units {{once per turn}}. Take down another idol by moving your Rat. ((Click it, then click a tile on an another row)).", new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.MoveUnit,
			TutorialTicker.Tag.Move_On,
			TutorialTicker.Tag.Blink_Unit
		}).filter(new TutorialTicker.Line.TileFilter[]
		{
			TutorialTicker.ChangeRow(),
			TutorialTicker.IfUnit("Beast Rat"),
			TutorialTicker.IfUnitAttack()
		});
		array[23] = TutorialFactory.p("Excellent! Your [[Ragged Wolf]] has {{Haste}}, so it can attack straight away. ((Summon it opposite one of your opponent's idols)).", new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.SummonUnit,
			TutorialTicker.Tag.Move_Off,
			TutorialTicker.Tag.Blink_Cards
		}).filter(new TutorialTicker.Line.TileFilter[]
		{
			TutorialTicker.HasIdol(),
			TutorialTicker.RowHasNoAttacker(),
			TutorialTicker.Card("Ragged Wolf")
		});
		array[24] = TutorialFactory.p("Victory is at hand! ((Hit end turn.))", new TutorialTicker.Tag[]
		{
			TutorialTicker.Tag.EndTurn
		});
		return array;
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x0003A770 File Offset: 0x00038970
	private static TutorialTicker.Line[] testTutorial2()
	{
		return new TutorialTicker.Line[]
		{
			TutorialFactory.p("In this tutorial, you will learn about {{spells}} and {{enchantments}}, and how to {{sacrifice for scrolls}}.", new TutorialTicker.Tag[0]),
			TutorialFactory.p("You begin in the middle of a close match. The opponent is threatening both your creatures, and you can't deal enough damage to destroy the opponent's units.", new TutorialTicker.Tag[0]),
			TutorialFactory.p("Unfortunately, the only scroll in your hand is a [[Sister of the Bear]]. Since you only have 3 Growth, you cannot afford to play it. Sacrificing for resources would leave you with no scrolls to play.", new TutorialTicker.Tag[0]),
			TutorialFactory.p("Luckily, instead of sacrificing for resources, you can {{sacrifice a scroll for two new scrolls}}. You can only sacrifice {{once per turn}}.", new TutorialTicker.Tag[0]),
			TutorialFactory.p("((Click the [[Sister of the Bear]])), and then ((click the scroll icon to the right)).", new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.CardClicked,
				TutorialTicker.Tag.Move_Off,
				TutorialTicker.Tag.Blink_Cards
			}).filter(new TutorialTicker.Line.TileFilter[]
			{
				TutorialTicker.Card("Sister of the Bear")
			}),
			TutorialFactory.p(string.Empty, new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.SacrificeCards,
				TutorialTicker.Tag.Move_Off
			}),
			TutorialFactory.p(string.Empty, new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.HandUpdate
			}),
			TutorialFactory.p("You've drawn two {{Enchantment}} scrolls. ((Click the [[Champion Ring]])) to see what it does.", new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.HideCardView_Off,
				TutorialTicker.Tag.CardClicked,
				TutorialTicker.Tag.AllowCardPlayed,
				TutorialTicker.Tag.ZoomCard,
				TutorialTicker.Tag.Blink_Cards
			}).filter(new TutorialTicker.Line.TileFilter[]
			{
				TutorialTicker.Card("Champion Ring"),
				TutorialTicker.IfUnit("Beast Rat")
			}),
			TutorialFactory.p("The [[Champion Ring]] is an enchantment scroll that increases the {{Attack}} of a unit. Playing it on your [[Beast Rat]] will ensure we destroy the opponent's [[Kinfolk Ranger]].", new TutorialTicker.Tag[0]).filter(new TutorialTicker.Line.TileFilter[]
			{
				TutorialTicker.IfUnit("Beast Rat")
			}),
			TutorialFactory.p("It's often a good idea to focus on destroying units instead of idols. ((Play the [[Champion Ring]])) on the [[Beast Rat]], and let's not move it.", new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.CardPlayed,
				TutorialTicker.Tag.Blink_Unit,
				TutorialTicker.Tag.Blink_Cards
			}).filter(new TutorialTicker.Line.TileFilter[]
			{
				TutorialTicker.Card("Champion Ring"),
				TutorialTicker.Unit("Beast Rat")
			}),
			TutorialFactory.p("Now, ((click the [[Binding Root]])) to see what it does.", new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.HideCardView_Off,
				TutorialTicker.Tag.ZoomCard,
				TutorialTicker.Tag.AllowCardPlayed,
				TutorialTicker.Tag.CardClicked,
				TutorialTicker.Tag.Blink_Cards
			}).filter(new TutorialTicker.Line.TileFilter[]
			{
				TutorialTicker.Card("Binding Root")
			}),
			TutorialFactory.p("The [[Binding Root]] can prevent an enemy unit from moving. ((Play it on the opponent's [[Sister of the Bear]])) to pin it to the bottom row.", new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.CardPlayed,
				TutorialTicker.Tag.Blink_Unit
			}).filter(new TutorialTicker.Line.TileFilter[]
			{
				TutorialTicker.Card("Binding Root"),
				TutorialTicker.Unit("Sister of the Bear")
			}),
			TutorialFactory.p("You're out of scrolls and resources for this turn. ((Hit end turn)) to begin your attacks.", new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.HideCardView_On,
				TutorialTicker.Tag.EndTurn
			}),
			TutorialFactory.p(null, new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.HandUpdate
			}),
			TutorialFactory.p("You've drawn a [[Beast Rat]]. Just like last time, let's ((sacrifice for two new scrolls)) and hope we draw something useful.", new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.Blink_Cards,
				TutorialTicker.Tag.CardClicked
			}),
			TutorialFactory.p(string.Empty, new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.SacrificeCards
			}),
			TutorialFactory.p("Great! Now you have the tools to win the match. Let's see how. First, ((click the [[Earthen Testament]])) to see what it does.", new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.HideCardView_Off,
				TutorialTicker.Tag.ZoomCard,
				TutorialTicker.Tag.CardClicked,
				TutorialTicker.Tag.Blink_Cards,
				TutorialTicker.Tag.AllowCardPlayed
			}).filter(new TutorialTicker.Line.TileFilter[]
			{
				TutorialTicker.Card("Earthen Testament")
			}),
			TutorialFactory.p("[[Earthen Testament]] is a {{Spell}}. Spells do many different things. In this case, it will decrease the {{Countdown}} of your enchanted units.", new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.Blink_Cards
			}).filter(new TutorialTicker.Line.TileFilter[]
			{
				TutorialTicker.Card("Earthen Testament")
			}),
			TutorialFactory.p("Playing it now will make your [[Beast Rat]] attack, since you've already {{enchanted it with a [[Champion Ring]]}}. ((Click the Cast button)) above the [[Earthen Testament]].", new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.Blink_Cast,
				TutorialTicker.Tag.CardPlayed
			}).filter(new TutorialTicker.Line.TileFilter[]
			{
				TutorialTicker.Card("Earthen Testament")
			}),
			TutorialFactory.p("Good. Remember, the [[Ragged Wolf]] has {{Haste}}, meaning it will attack the turn it's summoned. ((Play it in front of the weakest opponent idol)), and ((hit end turn)) to destroy your enemy!", new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.HideCardView_On,
				TutorialTicker.Tag.CardClicked,
				TutorialTicker.Tag.Blink_Cards
			}),
			TutorialFactory.p(string.Empty, new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.Blink_Cards,
				TutorialTicker.Tag.SummonUnit
			}).filter(new TutorialTicker.Line.TileFilter[]
			{
				TutorialTicker.Row(new int[]
				{
					3
				})
			}),
			TutorialFactory.p(string.Empty, new TutorialTicker.Tag[]
			{
				TutorialTicker.Tag.EndTurn
			})
		};
	}
}
