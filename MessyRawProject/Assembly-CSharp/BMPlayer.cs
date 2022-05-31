using System;
using System.Collections.Generic;
using Gui;
using UnityEngine;

// Token: 0x0200003B RID: 59
public class BMPlayer
{
	// Token: 0x0600027F RID: 639 RVA: 0x00003D6F File Offset: 0x00001F6F
	public BMPlayer(bool isLeft)
	{
		this._persistentRules = new BMPlayer.PersistentRules(isLeft);
	}

	// Token: 0x06000280 RID: 640 RVA: 0x00003D8E File Offset: 0x00001F8E
	public BMPlayer.Costs costs()
	{
		return this._costs;
	}

	// Token: 0x06000281 RID: 641 RVA: 0x00003D96 File Offset: 0x00001F96
	public BMPlayer.PersistentRules rules()
	{
		return this._persistentRules;
	}

	// Token: 0x04000163 RID: 355
	public string name;

	// Token: 0x04000164 RID: 356
	public int profileId;

	// Token: 0x04000165 RID: 357
	private BMPlayer.Costs _costs = new BMPlayer.Costs();

	// Token: 0x04000166 RID: 358
	private bool _isLeft;

	// Token: 0x04000167 RID: 359
	public BMPlayer.PersistentRules _persistentRules;

	// Token: 0x0200003C RID: 60
	public class Costs
	{
		// Token: 0x06000283 RID: 643 RVA: 0x000227D0 File Offset: 0x000209D0
		public void update(EMCostUpdate m)
		{
			foreach (EMCostUpdate.CostInfo costInfo in m.costs)
			{
				this._costs[costInfo.cardTypeId] = costInfo.cost;
			}
		}

		// Token: 0x06000284 RID: 644 RVA: 0x00003DB1 File Offset: 0x00001FB1
		public int get(Card card)
		{
			return this.get(card.getCardType());
		}

		// Token: 0x06000285 RID: 645 RVA: 0x00003DBF File Offset: 0x00001FBF
		public int get(CardType cardType)
		{
			return CollectionUtil.getOrDefault<int, int>(this._costs, cardType.id, cardType.getCost());
		}

		// Token: 0x04000168 RID: 360
		private Dictionary<int, int> _costs = new Dictionary<int, int>();
	}

	// Token: 0x0200003D RID: 61
	public class PersistentRules
	{
		// Token: 0x06000286 RID: 646 RVA: 0x00003DD8 File Offset: 0x00001FD8
		public PersistentRules(bool isLeft)
		{
			this._isLeft = isLeft;
		}

		// Token: 0x06000287 RID: 647 RVA: 0x00022814 File Offset: 0x00020A14
		public void add(ICollection<EMRuleAdded> rules)
		{
			if (rules == null)
			{
				return;
			}
			foreach (EMRuleAdded rule in rules)
			{
				this.add(rule);
			}
		}

		// Token: 0x06000288 RID: 648 RVA: 0x00022870 File Offset: 0x00020A70
		public void add(EMRuleAdded rule)
		{
			GameObject gameObject = PrimitiveFactory.createPlane(true);
			gameObject.transform.eulerAngles = new Vector3(39f, 90f, 0f);
			gameObject.transform.localScale = new Vector3(0.145f, 0.001f, 0.11f);
			PersistentRuleCardView persistentRuleCardView = gameObject.AddComponent<PersistentRuleCardView>();
			persistentRuleCardView.init(rule.card, rule.color, rule.roundsLeft);
			int num = ~this._rules.BinarySearch(persistentRuleCardView, PersistentRuleCardView.comparer);
			if (num == this._rules.Count)
			{
				this._rules.Add(persistentRuleCardView);
			}
			else
			{
				this._rules.Insert(num, persistentRuleCardView);
			}
			this._updatePositions();
		}

		// Token: 0x06000289 RID: 649 RVA: 0x0002292C File Offset: 0x00020B2C
		public void update(EMRuleUpdate rule)
		{
			int num = this.indexOf(rule.card);
			if (num < 0)
			{
				return;
			}
			this._rules[num].set(rule.roundsLeft);
			this._updatePositions();
		}

		// Token: 0x0600028A RID: 650 RVA: 0x0002296C File Offset: 0x00020B6C
		public void remove(EMRuleRemoved rule)
		{
			int num = this.indexOf(rule.card);
			if (num < 0)
			{
				return;
			}
			this._rules[num].remove(0.6f);
			this._rules.RemoveAt(num);
			this._updatePositions();
		}

		// Token: 0x0600028B RID: 651 RVA: 0x000229B8 File Offset: 0x00020BB8
		private void _updatePositions()
		{
			Gui3D gui3D = new Gui3D(UnityUtil.getFirstOrtographicCamera());
			float num = (float)Screen.height * 0.08f;
			float num2 = num * 1.2673267f;
			Rect rect;
			rect..ctor(0f, (float)Screen.height * 0.085f, num2, num);
			float num3 = num2 * 1.3f;
			float num4 = num2 * 0.8f;
			int num5 = 3;
			float num6 = (float)(num5 - 1) * num3;
			int count = this._rules.Count;
			if (count > 1)
			{
				num3 = Math.Min(num3, num6 / (float)(count - 1));
			}
			for (int i = 0; i < count; i++)
			{
				rect.x = (float)(Screen.width / 2) + (num4 + (float)i * num3) * (float)((!this._isLeft) ? 1 : -1);
				if (this._isLeft)
				{
					rect.x -= num2;
				}
				GameObject gameObject = this._rules[i].gameObject;
				if (gameObject.transform.parent == null)
				{
					gui3D.DrawObject(rect, gameObject);
				}
				Gui3D.PosData position = gui3D.getPosition(rect);
				Gui3D.PosData posData = position;
				posData.pos.z = posData.pos.z + (float)i;
				iTween.MoveTo(gameObject, iTween.Hash(new object[]
				{
					"position",
					position.pos,
					"time",
					0.25f,
					"easetype",
					iTween.EaseType.easeInOutQuint
				}));
			}
		}

		// Token: 0x0600028C RID: 652 RVA: 0x00022B48 File Offset: 0x00020D48
		public void clear()
		{
			foreach (PersistentRuleCardView persistentRuleCardView in this._rules)
			{
				Object.Destroy(persistentRuleCardView.gameObject);
			}
			this._rules.Clear();
		}

		// Token: 0x0600028D RID: 653 RVA: 0x00022BB4 File Offset: 0x00020DB4
		private int indexOf(Card card)
		{
			for (int i = 0; i < this._rules.Count; i++)
			{
				if (this._rules[i].card().getId() == card.getId())
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x04000169 RID: 361
		private List<PersistentRuleCardView> _rules = new List<PersistentRuleCardView>();

		// Token: 0x0400016A RID: 362
		private bool _isLeft;
	}
}
