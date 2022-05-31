using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200005D RID: 93
public class Blinker : ComponentAttacher<TutorialTicker.Tag>
{
	// Token: 0x060003FA RID: 1018 RVA: 0x00030050 File Offset: 0x0002E250
	public Blinker()
	{
		this.blinkdummy_resourcecounter = PrimitiveFactory.createPlane();
		this.blinkdummy_resourcecounter.name = Blinker.Name_ResourceCounter;
		this.blinkdummy_resourcecounter.transform.localScale = Vector3.zero;
		this.blinkdummy_resourcecounter.transform.position = Camera.main.ScreenToWorldPoint(new Vector3((float)Screen.width * 0.02f, (float)Screen.height * 0.95f, 100f));
		this.blinkdummy_resourcecounter.renderer.enabled = false;
		this.blinkdummy_resourcecounter.renderer.material = Shaders.matMilkBurn();
		this.blinkdummy_resourcecounter.renderer.material.color = Color.white;
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x000049F2 File Offset: 0x00002BF2
	public void setUnits(List<GameObject> units)
	{
		this._unitGameObjects = units;
	}

	// Token: 0x060003FD RID: 1021 RVA: 0x000049FB File Offset: 0x00002BFB
	public void setCards(List<GameObject> cards)
	{
		this._cardGameObjects = cards;
	}

	// Token: 0x060003FE RID: 1022 RVA: 0x00030154 File Offset: 0x0002E354
	protected override ComponentAttacher<TutorialTicker.Tag>.IAttachComponent create(TutorialTicker.Tag tag)
	{
		switch (tag)
		{
		case TutorialTicker.Tag.EndTurn:
			return new ComponentAttacher<TutorialTicker.Tag>.Group(new ComponentAttacher<TutorialTicker.Tag>.IAttachComponent[]
			{
				Blinker.Name("End_Turn"),
				Blinker.Name("End_Turn_BG")
			});
		case TutorialTicker.Tag.SacrificeCards:
			return Blinker.Tag("blinkable_cycle");
		case TutorialTicker.Tag.SacrificeResource:
			return Blinker.Tag("blinkable_resource");
		case TutorialTicker.Tag.Blink_Cost:
			return Blinker.Tag("blinkable_cost");
		case TutorialTicker.Tag.Blink_Attack:
			return Blinker.Tag("blinkable_attack");
		case TutorialTicker.Tag.Blink_Countdown:
			return Blinker.Tag("blinkable_countdown");
		case TutorialTicker.Tag.Blink_Health:
			return Blinker.Tag("blinkable_health");
		case TutorialTicker.Tag.Blink_Unit:
			return new ComponentAttacher<TutorialTicker.Tag>.TagAttachComponent<BlinkLerp>("blinkable_unit").includeOnly(this._unitGameObjects);
		case TutorialTicker.Tag.Blink_ResourceCounter:
			return new ComponentAttacher<TutorialTicker.Tag>.ObjAttachComponent<BlinkMilkBurn>(this.blinkdummy_resourcecounter);
		case TutorialTicker.Tag.Blink_Cards:
			return Blinker.Tag("blinkable_card").includeOnly(this._cardGameObjects);
		case TutorialTicker.Tag.Blink_Cast:
			return Blinker.Tag("blinkable_cast");
		case TutorialTicker.Tag.Blink_Idol:
			return Blinker.Tag("blinkable_idol");
		}
		return null;
	}

	// Token: 0x060003FF RID: 1023 RVA: 0x00004A04 File Offset: 0x00002C04
	private static ComponentAttacher<TutorialTicker.Tag>.IAttachComponent Tag(string tag)
	{
		return new ComponentAttacher<TutorialTicker.Tag>.TagAttachComponent<BlinkMilkBurn>(tag);
	}

	// Token: 0x06000400 RID: 1024 RVA: 0x00004A0C File Offset: 0x00002C0C
	private static ComponentAttacher<TutorialTicker.Tag>.IAttachComponent Name(string name)
	{
		return new ComponentAttacher<TutorialTicker.Tag>.ObjAttachComponent<BlinkMilkBurn>(name);
	}

	// Token: 0x04000289 RID: 649
	private List<TutorialTicker.Tag> _active = new List<TutorialTicker.Tag>();

	// Token: 0x0400028A RID: 650
	private List<ComponentAttacher<TutorialTicker.Tag>.IAttachComponent> _enabled = new List<ComponentAttacher<TutorialTicker.Tag>.IAttachComponent>();

	// Token: 0x0400028B RID: 651
	private List<ComponentAttacher<TutorialTicker.Tag>.IAttachComponent> _disabled = new List<ComponentAttacher<TutorialTicker.Tag>.IAttachComponent>();

	// Token: 0x0400028C RID: 652
	private List<GameObject> _unitGameObjects = new List<GameObject>();

	// Token: 0x0400028D RID: 653
	private List<GameObject> _cardGameObjects = new List<GameObject>();

	// Token: 0x0400028E RID: 654
	private List<TutorialTicker.Tag> _pending = new List<TutorialTicker.Tag>();

	// Token: 0x0400028F RID: 655
	private GameObject blinkdummy_resourcecounter;

	// Token: 0x04000290 RID: 656
	public static readonly string Name_ResourceCounter = "blinkdummy_resourcecounter";
}
