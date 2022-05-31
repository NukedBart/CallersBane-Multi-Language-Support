using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gui
{
	// Token: 0x020001DF RID: 479
	public class MultiClickCheck
	{
		// Token: 0x06000F15 RID: 3861 RVA: 0x0000C247 File Offset: 0x0000A447
		public MultiClickCheck(float doubleClickTimeLimit, int maxClicks)
		{
			this._maxClicks = maxClicks;
			this._dblClickTime = doubleClickTimeLimit;
			this.clear();
		}

		// Token: 0x06000F16 RID: 3862 RVA: 0x0000C26E File Offset: 0x0000A46E
		public float getTimeSinceClick()
		{
			return Time.time - this._lastClickTime;
		}

		// Token: 0x06000F17 RID: 3863 RVA: 0x0000C27C File Offset: 0x0000A47C
		public bool clicked(int numClicks)
		{
			this._lastClickTime = Time.time;
			this._clickTimes.AddFirst(this._lastClickTime);
			this._clickTimes.RemoveLast();
			return this.getItemsInRange() >= numClicks;
		}

		// Token: 0x06000F18 RID: 3864 RVA: 0x00064374 File Offset: 0x00062574
		public void clear()
		{
			this._clickTimes.Clear();
			for (int i = 0; i < this._maxClicks; i++)
			{
				this._clickTimes.AddLast(float.MinValue);
			}
		}

		// Token: 0x06000F19 RID: 3865 RVA: 0x000643B4 File Offset: 0x000625B4
		private int getItemsInRange()
		{
			int num = 0;
			float time = Time.time;
			foreach (float num2 in this._clickTimes)
			{
				float num3 = num2;
				if (time - num3 > this._dblClickTime)
				{
					break;
				}
				num++;
			}
			return num;
		}

		// Token: 0x04000B97 RID: 2967
		private LinkedList<float> _clickTimes = new LinkedList<float>();

		// Token: 0x04000B98 RID: 2968
		private int _maxClicks;

		// Token: 0x04000B99 RID: 2969
		private float _dblClickTime;

		// Token: 0x04000B9A RID: 2970
		private float _lastClickTime;
	}
}
