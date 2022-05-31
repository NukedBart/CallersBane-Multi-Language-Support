using System;

// Token: 0x0200026E RID: 622
public class EffectMessage
{
	// Token: 0x060011FD RID: 4605 RVA: 0x0007787C File Offset: 0x00075A7C
	public EffectMessage()
	{
		string text = base.GetType().ToString();
		if (text.IndexOf("EM") == 0)
		{
			this.type = text.Substring(2);
		}
		else
		{
			Log.error("This EffectMessage subclass doesn't start with 'EM'! : " + text);
			this.type = "<Unnamed>";
		}
	}

	// Token: 0x060011FF RID: 4607 RVA: 0x000059E4 File Offset: 0x00003BE4
	public virtual bool isAction()
	{
		return false;
	}

	// Token: 0x06001200 RID: 4608 RVA: 0x0000D9C4 File Offset: 0x0000BBC4
	public bool isSameGroup(long groupId)
	{
		return this.groupId != null && this.groupId.Value == groupId;
	}

	// Token: 0x06001201 RID: 4609 RVA: 0x0000D9E7 File Offset: 0x0000BBE7
	public int stampSequenceId()
	{
		if (this._sequenceId != 0)
		{
			Log.error("EffectMessage stamped multiple times!");
		}
		this._sequenceId = ++EffectMessage._runningSequenceId;
		return this._sequenceId;
	}

	// Token: 0x06001202 RID: 4610 RVA: 0x0000DA17 File Offset: 0x0000BC17
	public int sequenceId()
	{
		return this._sequenceId;
	}

	// Token: 0x06001203 RID: 4611 RVA: 0x0000DA1F File Offset: 0x0000BC1F
	public virtual float timeoutSeconds()
	{
		return 6f;
	}

	// Token: 0x06001204 RID: 4612 RVA: 0x0000DA26 File Offset: 0x0000BC26
	public void setRawText(string s)
	{
		this._rawText = s;
	}

	// Token: 0x06001205 RID: 4613 RVA: 0x0000DA2F File Offset: 0x0000BC2F
	public string getRawText()
	{
		return this._rawText;
	}

	// Token: 0x04000EB0 RID: 3760
	private static int _runningSequenceId;

	// Token: 0x04000EB1 RID: 3761
	private int _sequenceId;

	// Token: 0x04000EB2 RID: 3762
	public string type;

	// Token: 0x04000EB3 RID: 3763
	public long? groupId;

	// Token: 0x04000EB4 RID: 3764
	private string _rawText;
}
