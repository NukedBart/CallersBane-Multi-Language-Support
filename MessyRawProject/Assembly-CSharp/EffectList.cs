using System;
using System.Collections.Generic;

// Token: 0x02000074 RID: 116
public class EffectList : ListQueue<EffectMessage>
{
	// Token: 0x06000471 RID: 1137 RVA: 0x00031904 File Offset: 0x0002FB04
	public void endGame()
	{
		List<EffectMessage> list = new List<EffectMessage>();
		foreach (EffectMessage effectMessage in this._list)
		{
			if (effectMessage is EMChatEffect)
			{
				list.Add(effectMessage);
			}
		}
		this._list = list;
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x00004E91 File Offset: 0x00003091
	public void onRunEffect(EffectMessage m)
	{
		this._updateSkipEffect(m);
	}

	// Token: 0x06000473 RID: 1139 RVA: 0x00031978 File Offset: 0x0002FB78
	public bool onEffectDone(EffectMessage m)
	{
		return --this._pendingEffectCount <= 0;
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x0003199C File Offset: 0x0002FB9C
	private void _updateSkipEffect(EffectMessage m)
	{
		if (this._decideSkipMessage == null)
		{
			if (m is EMSummonUnit)
			{
				this._decideSkipMessage = m;
			}
		}
		else
		{
			if (this._decideSkipMessage is EMEnchantUnit && (m is EMStatsUpdate || m is EMEnchantUnit || m is EMDelay))
			{
				this._pendingEffectCount++;
				return;
			}
			this._decideSkipMessage = null;
		}
		this._pendingEffectCount = 0;
	}

	// Token: 0x040002DF RID: 735
	private EffectMessage _decideSkipMessage;

	// Token: 0x040002E0 RID: 736
	private int _pendingEffectCount;
}
