using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x0200045F RID: 1119
public class ValidatedTextfield
{
	// Token: 0x060018EE RID: 6382 RVA: 0x00093720 File Offset: 0x00091920
	public ValidatedTextfield(string name, int minLength, int maxLength, bool numericOnly, string label, Color labelColor)
	{
		this.name = name;
		this.numericOnly = numericOnly;
		this.maxLength = maxLength;
		this.minLength = minLength;
		this.label = label;
		this.labelColor = labelColor;
		this.emptySkin = (GUISkin)ResourceManager.Load("_GUISkins/EmptySkin");
	}

	// Token: 0x060018EF RID: 6383 RVA: 0x0001223E File Offset: 0x0001043E
	public string GetName()
	{
		return this.name;
	}

	// Token: 0x060018F0 RID: 6384 RVA: 0x00012246 File Offset: 0x00010446
	public string GetValue()
	{
		if (string.IsNullOrEmpty(this.value))
		{
			return null;
		}
		return this.value;
	}

	// Token: 0x060018F1 RID: 6385 RVA: 0x00012260 File Offset: 0x00010460
	public void SetValue(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			this.value = string.Empty;
		}
		else
		{
			this.value = value;
		}
	}

	// Token: 0x060018F2 RID: 6386 RVA: 0x0009379C File Offset: 0x0009199C
	public void Draw(Rect rect, float labelWidth)
	{
		GUI.Box(rect, string.Empty);
		GUISkin skin = GUI.skin;
		GUI.skin = this.emptySkin;
		int fontSize = GUI.skin.textField.fontSize;
		GUI.skin.textField.fontSize = 6 + Screen.height / 64;
		Color textColor = GUI.skin.textField.normal.textColor;
		GUI.skin.textField.normal.textColor = new Color(0.2f, 0.2f, 0.2f);
		GUI.skin.settings.cursorColor = new Color(0.2f, 0.2f, 0.2f);
		GUI.SetNextControlName(this.name);
		this.value = GUI.TextField(rect, this.value);
		GUI.skin.textField.normal.textColor = textColor;
		GUI.skin.textField.fontSize = fontSize;
		GUI.skin = skin;
		Texture2D texture2D = ResourceManager.LoadTexture("Store/checkout_checkmark");
		Texture2D texture2D2 = ResourceManager.LoadTexture("Store/checkout_alertbox");
		float num = rect.height * 0.6f;
		float num2 = num * (float)texture2D2.width / (float)texture2D2.height;
		float num3 = (rect.height - num) / 2f;
		Rect rect2;
		rect2..ctor(rect.xMax - num2 - num3, rect.y + num3, num2, num);
		using (Dictionary<string, string>.Enumerator enumerator = this.errorsByType.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				KeyValuePair<string, string> keyValuePair = enumerator.Current;
				GUI.DrawTexture(rect2, texture2D2);
				Vector2 screenMousePos = GUIUtil.getScreenMousePos();
				if (rect2.Contains(screenMousePos))
				{
					Rect rect3;
					rect3..ctor(rect.x + 4f, rect.y + 4f, rect.width - num2 - num3 - 8f, rect.height - 8f);
					GUI.Box(rect3, string.Empty);
					Color textColor2 = GUI.skin.label.normal.textColor;
					GUI.skin.label.normal.textColor = new Color(0.8f, 0.2f, 0f);
					int fontSize2 = GUI.skin.label.fontSize;
					TextAnchor alignment = GUI.skin.label.alignment;
					GUI.skin.label.fontSize = Screen.height / 40;
					GUI.skin.label.alignment = 1;
					float num4 = (float)Screen.height / 400f;
					GUI.Label(new Rect(rect3.x + num4 * 2f, rect3.y + num4, rect3.width - num4 * 4f, rect3.height - num4), keyValuePair.Value);
					GUI.skin.label.fontSize = fontSize2;
					GUI.skin.label.alignment = alignment;
					GUI.skin.label.normal.textColor = textColor2;
				}
			}
		}
		if (this.numericOnly)
		{
			this.Numeric();
		}
		if (this.maxLength > 0)
		{
			this.CapString(this.maxLength);
		}
		if (this.InternalValidity(false) && GUI.GetNameOfFocusedControl() != this.name && this.errorsByType.Count == 0 && this.value.Length >= this.minLength)
		{
			GUI.DrawTexture(rect2, texture2D);
		}
		LabelUtil.HelpLabel(rect, this.label, labelWidth, this.labelColor);
	}

	// Token: 0x060018F3 RID: 6387 RVA: 0x00012284 File Offset: 0x00010484
	private void CapString(int length)
	{
		this.value = this.value.Substring(0, Mathf.Min(length, this.value.Length));
	}

	// Token: 0x060018F4 RID: 6388 RVA: 0x00093B68 File Offset: 0x00091D68
	private void Numeric()
	{
		Regex regex = new Regex("[^0-9]");
		this.value = regex.Replace(this.value, string.Empty);
	}

	// Token: 0x060018F5 RID: 6389 RVA: 0x000122A9 File Offset: 0x000104A9
	public bool AssertValidityOrMark()
	{
		return this.InternalValidity(true) && this.value.Length >= this.minLength;
	}

	// Token: 0x060018F6 RID: 6390 RVA: 0x000122D0 File Offset: 0x000104D0
	public void ClearAllErrors()
	{
		this.errorsByType.Clear();
	}

	// Token: 0x060018F7 RID: 6391 RVA: 0x00093B98 File Offset: 0x00091D98
	private bool InternalValidity(bool checkForErrors)
	{
		if ((this.minLength > 0 && this.value == null) || this.value.Trim().Length < this.minLength)
		{
			if (checkForErrors && !this.errorsByType.ContainsKey("empty"))
			{
				int num = (this.value != null) ? this.value.Trim().Length : 0;
				string text = "Cannot be empty.";
				string text2 = (!this.numericOnly) ? "characters" : "digits";
				if (this.minLength > 1 && this.minLength == this.maxLength)
				{
					text = string.Concat(new object[]
					{
						"Must be exactly ",
						this.minLength,
						" ",
						text2,
						"."
					});
				}
				else if (this.minLength > 1)
				{
					text = this.minLength + "-" + this.maxLength;
				}
				this.errorsByType.Add("empty", text);
			}
		}
		else if (this.errorsByType.ContainsKey("empty"))
		{
			this.errorsByType.Remove("empty");
		}
		return true;
	}

	// Token: 0x0400155D RID: 5469
	private GUISkin emptySkin;

	// Token: 0x0400155E RID: 5470
	private int maxLength = -1;

	// Token: 0x0400155F RID: 5471
	private int minLength = -1;

	// Token: 0x04001560 RID: 5472
	private bool numericOnly;

	// Token: 0x04001561 RID: 5473
	private string name;

	// Token: 0x04001562 RID: 5474
	private string value = string.Empty;

	// Token: 0x04001563 RID: 5475
	private Dictionary<string, string> errorsByType = new Dictionary<string, string>();

	// Token: 0x04001564 RID: 5476
	private string label;

	// Token: 0x04001565 RID: 5477
	private Color labelColor;
}
