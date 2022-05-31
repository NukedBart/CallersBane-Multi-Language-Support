using System;
using UnityEngine;

// Token: 0x02000457 RID: 1111
public static class SystemUtil
{
	// Token: 0x060018C7 RID: 6343 RVA: 0x00011FDE File Offset: 0x000101DE
	public static string getStackTrace()
	{
		return SystemUtil.getStackTrace(string.Empty);
	}

	// Token: 0x060018C8 RID: 6344 RVA: 0x0009332C File Offset: 0x0009152C
	public static string getStackTrace(string message)
	{
		string result;
		try
		{
			throw new Exception(message);
		}
		catch (Exception ex)
		{
			string text = (ex.Message.Length <= 0) ? string.Empty : ex.Message;
			result = text + ex.StackTrace;
		}
		return result;
	}

	// Token: 0x060018C9 RID: 6345 RVA: 0x00093390 File Offset: 0x00091590
	public static void copyToClipboard(string data)
	{
		TextEditor textEditor = new TextEditor();
		textEditor.content = new GUIContent(data);
		textEditor.SelectAll();
		textEditor.Copy();
	}
}
