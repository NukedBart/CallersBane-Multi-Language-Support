using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020001E5 RID: 485
public class TextField
{
	// Token: 0x06000F2D RID: 3885 RVA: 0x00064DFC File Offset: 0x00062FFC
	public TextField(string initialText, string emptyText, GUIStyle style)
	{
		this.controlName = "textfield_" + RuntimeHelpers.GetHashCode(this);
		this.emptySearchFieldString = (emptyText ?? "Search") + "   ";
		this.style = style;
		this.searchString = (initialText ?? string.Empty);
	}

	// Token: 0x06000F2E RID: 3886 RVA: 0x00064E64 File Offset: 0x00063064
	public void OnGUI_update(Rect rect)
	{
		string empty = this.searchString;
		if (GUI.GetNameOfFocusedControl() == this.controlName)
		{
			if (empty == this.emptySearchFieldString)
			{
				empty = string.Empty;
			}
		}
		else if (empty == string.Empty)
		{
			empty = this.emptySearchFieldString;
		}
		GUI.SetNextControlName(this.controlName);
		string text = empty;
		this.searchString = GUI.TextField(rect, empty, this.style);
		this.changed = (this.searchString != text);
		this.updateFocus();
	}

	// Token: 0x06000F2F RID: 3887 RVA: 0x00064EF8 File Offset: 0x000630F8
	private void updateFocus()
	{
		if (this.focusState == 1)
		{
			this.focusState--;
			int keyboardControl = GUIUtility.keyboardControl;
			TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), keyboardControl);
			if (textEditor != null)
			{
				textEditor.SelectNone();
				textEditor.MoveTextEnd();
				textEditor.selectPos = 99999;
			}
		}
		if (this.focusState == 2)
		{
			this.focusState--;
			GUI.FocusControl(this.controlName);
		}
	}

	// Token: 0x06000F30 RID: 3888 RVA: 0x0000C3A7 File Offset: 0x0000A5A7
	public string text()
	{
		return (!(this.searchString == this.emptySearchFieldString)) ? this.searchString : string.Empty;
	}

	// Token: 0x06000F31 RID: 3889 RVA: 0x0000C3CF File Offset: 0x0000A5CF
	public bool isChanged()
	{
		return this.changed;
	}

	// Token: 0x06000F32 RID: 3890 RVA: 0x0000C3D7 File Offset: 0x0000A5D7
	public bool isFocused()
	{
		return GUI.GetNameOfFocusedControl() == this.controlName;
	}

	// Token: 0x06000F33 RID: 3891 RVA: 0x0000C3E9 File Offset: 0x0000A5E9
	public void takeFocus()
	{
		this.focusState = 2;
	}

	// Token: 0x04000BD2 RID: 3026
	private readonly string controlName;

	// Token: 0x04000BD3 RID: 3027
	private readonly string emptySearchFieldString;

	// Token: 0x04000BD4 RID: 3028
	private string searchString;

	// Token: 0x04000BD5 RID: 3029
	private GUIStyle style;

	// Token: 0x04000BD6 RID: 3030
	private bool changed;

	// Token: 0x04000BD7 RID: 3031
	private int focusState;
}
