using System;
using UnityEngine;

// Token: 0x0200027A RID: 634
public class InternalEffectMessage : EffectMessage
{
	// Token: 0x06001213 RID: 4627 RVA: 0x000778DC File Offset: 0x00075ADC
	public InternalEffectMessage()
	{
		string text = base.GetType().Name;
		if (text.StartsWith("EM"))
		{
			text = text.Substring(2);
		}
		else
		{
			Debug.LogWarning("[Internal]EffectMessages should start with \"EM\" : " + text);
		}
		this.type = text;
	}
}
