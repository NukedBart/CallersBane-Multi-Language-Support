using System;
using UnityEngine;

// Token: 0x02000072 RID: 114
public class TimedTrigger : MonoBehaviour
{
	// Token: 0x0600045C RID: 1116 RVA: 0x000028DF File Offset: 0x00000ADF
	private void Start()
	{
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x00004D6F File Offset: 0x00002F6F
	public void init(float seconds)
	{
		this.time = new TimeInfo(seconds);
	}

	// Token: 0x0600045E RID: 1118 RVA: 0x000028DF File Offset: 0x00000ADF
	protected virtual void trigger()
	{
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x00004D7D File Offset: 0x00002F7D
	protected virtual void Update()
	{
		if (this._triggered)
		{
			return;
		}
		if (this.time.isDone())
		{
			this._triggered = true;
			this.trigger();
		}
	}

	// Token: 0x040002DB RID: 731
	protected TimeInfo time;

	// Token: 0x040002DC RID: 732
	private bool _triggered;
}
