using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200005E RID: 94
public abstract class ComponentAttacher<Y>
{
	// Token: 0x06000402 RID: 1026 RVA: 0x00030288 File Offset: 0x0002E488
	public void update(List<Y> newActiveTags)
	{
		this._enabled.Clear();
		this._disabled.Clear();
		foreach (Y tag in newActiveTags)
		{
			this._add(tag, true);
		}
		foreach (Y y in this._active)
		{
			if (newActiveTags.IndexOf(y) < 0)
			{
				this._add(y, false);
			}
		}
		this._active.Clear();
		this._active.AddRange(newActiveTags);
		this.Update();
	}

	// Token: 0x06000403 RID: 1027 RVA: 0x00030368 File Offset: 0x0002E568
	private void _add(Y tag, bool enable)
	{
		ComponentAttacher<Y>.IAttachComponent attachComponent = this.create(tag);
		if (attachComponent == null)
		{
			return;
		}
		attachComponent.enable = enable;
		this._add(attachComponent);
	}

	// Token: 0x06000404 RID: 1028 RVA: 0x00004A3D File Offset: 0x00002C3D
	private void _add(ComponentAttacher<Y>.IAttachComponent tt)
	{
		if (tt.enable)
		{
			this._enabled.Add(tt);
		}
		else
		{
			this._disabled.Add(tt);
		}
	}

	// Token: 0x06000405 RID: 1029
	protected abstract ComponentAttacher<Y>.IAttachComponent create(Y tag);

	// Token: 0x06000406 RID: 1030 RVA: 0x00030394 File Offset: 0x0002E594
	public void Update()
	{
		foreach (ComponentAttacher<Y>.IAttachComponent attachComponent in this._enabled)
		{
			attachComponent.execute();
		}
		foreach (ComponentAttacher<Y>.IAttachComponent attachComponent2 in this._disabled)
		{
			attachComponent2.execute();
		}
	}

	// Token: 0x04000291 RID: 657
	private List<Y> _active = new List<Y>();

	// Token: 0x04000292 RID: 658
	private List<ComponentAttacher<Y>.IAttachComponent> _enabled = new List<ComponentAttacher<Y>.IAttachComponent>();

	// Token: 0x04000293 RID: 659
	private List<ComponentAttacher<Y>.IAttachComponent> _disabled = new List<ComponentAttacher<Y>.IAttachComponent>();

	// Token: 0x0200005F RID: 95
	protected abstract class IAttachComponent
	{
		// Token: 0x06000408 RID: 1032
		public abstract void execute();

		// Token: 0x06000409 RID: 1033 RVA: 0x00030438 File Offset: 0x0002E638
		protected void updateComponent<T>(GameObject g, bool enable) where T : Component
		{
			if (g == null)
			{
				return;
			}
			enable = (enable && this.includeFunc.Invoke(g));
			T component = g.GetComponent<T>();
			if (enable && component != null)
			{
				return;
			}
			if (enable)
			{
				g.AddComponent<T>();
				g.AddComponent(this.componentType);
			}
			else if (component != null)
			{
				Object.Destroy(component);
				Object.Destroy(g.GetComponent(this.componentType));
			}
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x000304D4 File Offset: 0x0002E6D4
		public ComponentAttacher<Y>.IAttachComponent includeOnly(List<GameObject> gs)
		{
			this.includeFunc = ((GameObject g) => gs.Contains(g));
			return this;
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x00030504 File Offset: 0x0002E704
		public ComponentAttacher<Y>.IAttachComponent excludeOnly(List<GameObject> gs)
		{
			this.includeFunc = ((GameObject g) => !gs.Contains(g));
			return this;
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x00004AA2 File Offset: 0x00002CA2
		public ComponentAttacher<Y>.IAttachComponent setComponentType(Type componentType)
		{
			this.componentType = componentType;
			return this;
		}

		// Token: 0x04000294 RID: 660
		public bool enable;

		// Token: 0x04000295 RID: 661
		private Func<GameObject, bool> includeFunc = (GameObject g) => true;

		// Token: 0x04000296 RID: 662
		private Type componentType = typeof(HelpArrow);
	}

	// Token: 0x02000062 RID: 98
	protected class TagAttachComponent<T> : ComponentAttacher<Y>.IAttachComponent where T : Behaviour
	{
		// Token: 0x06000412 RID: 1042 RVA: 0x00004ACE File Offset: 0x00002CCE
		public TagAttachComponent(string tag)
		{
			this.tag = tag;
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x00030534 File Offset: 0x0002E734
		public override void execute()
		{
			foreach (GameObject g in GameObject.FindGameObjectsWithTag(this.tag))
			{
				base.updateComponent<T>(g, this.enable);
			}
		}

		// Token: 0x0400029A RID: 666
		private string tag;
	}

	// Token: 0x02000063 RID: 99
	protected class ObjAttachComponent<T> : ComponentAttacher<Y>.IAttachComponent where T : Behaviour
	{
		// Token: 0x06000414 RID: 1044 RVA: 0x00004ADD File Offset: 0x00002CDD
		public ObjAttachComponent(GameObject g)
		{
			this.g = g;
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x00004AEC File Offset: 0x00002CEC
		public ObjAttachComponent(string name)
		{
			this.name = name;
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x00030574 File Offset: 0x0002E774
		public override void execute()
		{
			if (this.g == null && this.name != null)
			{
				this.g = GameObject.Find(this.name);
			}
			if (this.g != null)
			{
				base.updateComponent<T>(this.g, this.enable);
			}
		}

		// Token: 0x0400029B RID: 667
		private GameObject g;

		// Token: 0x0400029C RID: 668
		private string name;
	}

	// Token: 0x02000064 RID: 100
	protected class Group : ComponentAttacher<Y>.IAttachComponent
	{
		// Token: 0x06000417 RID: 1047 RVA: 0x00004AFB File Offset: 0x00002CFB
		public Group(params ComponentAttacher<Y>.IAttachComponent[] attachers)
		{
			this.attachers = attachers;
		}

		// Token: 0x06000418 RID: 1048 RVA: 0x000305D4 File Offset: 0x0002E7D4
		public override void execute()
		{
			foreach (ComponentAttacher<Y>.IAttachComponent attachComponent in this.attachers)
			{
				attachComponent.enable = this.enable;
				attachComponent.execute();
			}
		}

		// Token: 0x0400029D RID: 669
		private ComponentAttacher<Y>.IAttachComponent[] attachers;
	}
}
