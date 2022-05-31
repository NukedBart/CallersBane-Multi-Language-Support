using System;
using System.Collections.Generic;

// Token: 0x02000171 RID: 369
internal class JsonMessageSplitter : IServerMessageSplitter
{
	// Token: 0x06000B68 RID: 2920 RVA: 0x000097CC File Offset: 0x000079CC
	public JsonMessageSplitter() : this(0)
	{
	}

	// Token: 0x06000B69 RID: 2921 RVA: 0x000097D5 File Offset: 0x000079D5
	public JsonMessageSplitter(int getAtDepth)
	{
		this._goalDepth = getAtDepth;
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x000097FA File Offset: 0x000079FA
	public void feed(string s)
	{
		this._current += s;
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x0005218C File Offset: 0x0005038C
	public string getNextMessage()
	{
		if (this._current == null)
		{
			return null;
		}
		if (this._current.Length > 0)
		{
			this._parseData();
		}
		LinkedList<string> linkedList = (this._goalDepth != 0) ? this._queuedWithDepth : this._queued;
		if (linkedList.Count == 0)
		{
			return null;
		}
		string value = linkedList.First.Value;
		linkedList.RemoveFirst();
		return value;
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x0000980E File Offset: 0x00007A0E
	public void clear()
	{
		this._current = null;
		this._queued.Clear();
		this._queuedWithDepth.Clear();
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x000521FC File Offset: 0x000503FC
	private void _parseData()
	{
		int num = 0;
		bool flag = false;
		int num2 = 0;
		int num3 = 0;
		char[] array = this._current.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			char c = array[i];
			if (num == 0)
			{
				if (c == '{')
				{
					num = 1;
					num2++;
				}
				else if (!char.IsWhiteSpace(c))
				{
					return;
				}
			}
			else if (num == 1)
			{
				if (c == '"')
				{
					num = 2;
				}
				else if (c == '{')
				{
					num2++;
					if (num2 == this._goalDepth)
					{
						num3 = i;
					}
				}
				else if (c == '}')
				{
					num2--;
					if (this._goalDepth > 0 && num2 == this._goalDepth)
					{
						this._queuedWithDepth.AddLast(this._current.Substring(num3, i + 1 - num3));
					}
					if (num2 == 0)
					{
						this._queued.AddLast(this._current.Substring(0, i + 1));
						this._current = this._current.Substring(i + 1);
						return;
					}
				}
			}
			else if (num == 2 && !flag && c == '"')
			{
				num = 1;
			}
			flag = (!flag && c == '\\');
		}
	}

	// Token: 0x040008B9 RID: 2233
	private LinkedList<string> _queued = new LinkedList<string>();

	// Token: 0x040008BA RID: 2234
	private LinkedList<string> _queuedWithDepth = new LinkedList<string>();

	// Token: 0x040008BB RID: 2235
	private string _current;

	// Token: 0x040008BC RID: 2236
	private int _goalDepth;
}
