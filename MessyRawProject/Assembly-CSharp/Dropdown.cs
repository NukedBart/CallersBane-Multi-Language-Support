using System;
using UnityEngine;

// Token: 0x020001BB RID: 443
public class Dropdown : MonoBehaviour, IOverlayClickCallback
{
	// Token: 0x14000004 RID: 4
	// (add) Token: 0x06000DE7 RID: 3559 RVA: 0x0000AF83 File Offset: 0x00009183
	// (remove) Token: 0x06000DE8 RID: 3560 RVA: 0x0000AF9C File Offset: 0x0000919C
	public event Dropdown.DropdownChangedDelegate DropdownChangedEvent;

	// Token: 0x06000DE9 RID: 3561 RVA: 0x0000AFB5 File Offset: 0x000091B5
	public void Init(string[] strings, float maxRows, bool centerAlign, bool firstStringIsSelection, int guiDepth)
	{
		this.strings = strings;
		this.maxRows = maxRows;
		this.centerAlign = centerAlign;
		this.firstStringIsSelection = firstStringIsSelection;
		this.guiDepth = guiDepth;
		this.nothingSelectedError = false;
	}

	// Token: 0x06000DEA RID: 3562 RVA: 0x0000AFE3 File Offset: 0x000091E3
	private void Awake()
	{
		this.SetEnabled(false);
		this.skin = (GUISkin)ResourceManager.Load("_GUISkins/Dropdown");
	}

	// Token: 0x06000DEB RID: 3563 RVA: 0x0000B001 File Offset: 0x00009201
	private void Start()
	{
		this.overlay = base.gameObject.AddComponent<GUIBlackOverlayButton>();
		this.overlay.Init(this, this.guiDepth + 1, true);
		this.overlay.enabled = false;
	}

	// Token: 0x06000DEC RID: 3564 RVA: 0x0000B035 File Offset: 0x00009235
	public void SetRect(Rect r)
	{
		this.r = r;
	}

	// Token: 0x06000DED RID: 3565 RVA: 0x0000B03E File Offset: 0x0000923E
	public Rect GetRect()
	{
		return this.r;
	}

	// Token: 0x06000DEE RID: 3566 RVA: 0x0000B046 File Offset: 0x00009246
	public void SetEnabled(bool enabled)
	{
		if (this.overlay != null && !enabled)
		{
			this.overlay.enabled = false;
		}
		base.enabled = enabled;
	}

	// Token: 0x06000DEF RID: 3567 RVA: 0x0000B072 File Offset: 0x00009272
	public void SetCenterY(bool centerY)
	{
		this.centerY = centerY;
	}

	// Token: 0x06000DF0 RID: 3568 RVA: 0x0005FB34 File Offset: 0x0005DD34
	private void OnGUI()
	{
		if (this.strings == null)
		{
			return;
		}
		if (this.selectedIndex >= this.strings.Length)
		{
			this.selectedIndex = 0;
		}
		GUI.depth = this.guiDepth - 1;
		GUI.skin = this.skin;
		GUIStyle label = GUI.skin.label;
		TextAnchor alignment = (!this.centerAlign) ? 3 : 4;
		GUI.skin.button.alignment = alignment;
		label.alignment = alignment;
		GUIStyle label2 = GUI.skin.label;
		int fontSize = 10 + Screen.height / 80;
		GUI.skin.button.fontSize = fontSize;
		label2.fontSize = fontSize;
		if (!this.isOpen)
		{
			if (this.GUIButton(this.r, string.Empty))
			{
				if (!this.isOpen)
				{
					this.scrollPos = new Vector2(0f, this.r.height * (float)this.selectedIndex);
				}
				this.isOpen = !this.isOpen;
				this.overlay.enabled = this.isOpen;
			}
			Texture2D texture2D = ResourceManager.LoadTexture("Store/checkout_dropdownarrow");
			float num = this.r.height * 0.65f;
			float num2 = num * (float)texture2D.width / (float)texture2D.height;
			float num3 = (this.r.height - num) / 2f;
			string text = (this.strings.Length <= 0) ? string.Empty : this.strings[this.selectedIndex];
			Color textColor = GUI.skin.label.normal.textColor;
			if (this.nothingSelectedError)
			{
				GUI.skin.label.normal.textColor = new Color(0.8f, 0.2f, 0f);
				GUI.DrawTexture(new Rect(this.r.xMax - num2 * 2f - num3 * 2f, this.r.y + num3, num2, num), ResourceManager.LoadTexture("Store/checkout_alertbox"));
			}
			GUI.Label(new Rect(this.r.x, this.r.y, this.r.width - num2, this.r.height), text);
			GUI.skin.label.normal.textColor = textColor;
			GUI.DrawTexture(new Rect(this.r.xMax - num2 - num3, this.r.y + num3, num2, num), texture2D);
		}
		if (this.isOpen)
		{
			float num4 = Mathf.Min(this.r.height * this.maxRows, this.r.height * (float)this.strings.Length + 8f);
			float num5 = (!this.centerY) ? 0f : (this.r.height / 2f - num4 / 2f);
			Rect rect;
			rect..ctor(this.r.x, this.r.y + num5, this.r.width, num4);
			GUI.Box(rect, string.Empty);
			Color color = GUI.color;
			GUI.color = new Color(color.r, color.g, color.b, color.a * 0.5f);
			GUI.Box(new Rect(rect.xMax - 20f, rect.y + 4f, 16f, rect.height - 8f), string.Empty);
			GUI.color = color;
			Rect rect2;
			rect2..ctor(rect.x + 4f, rect.y + 4f, rect.width - 8f, rect.height - 8f);
			this.scrollPos = GUI.BeginScrollView(rect2, this.scrollPos, new Rect(0f, 0f, rect2.width - 20f, (float)this.strings.Length * this.r.height));
			for (int i = 0; i < this.strings.Length; i++)
			{
				if (this.GUIButton(new Rect(0f, 0f + (float)i * this.r.height, rect.width - 25f, this.r.height), this.strings[i]))
				{
					this.overlay.enabled = false;
					this.isOpen = false;
					this.selectedIndex = i;
					if (i != 0)
					{
						this.nothingSelectedError = false;
					}
					if (this.DropdownChangedEvent != null)
					{
						this.DropdownChangedEvent(i, this.strings[i]);
					}
				}
			}
			GUI.EndScrollView();
		}
	}

	// Token: 0x06000DF1 RID: 3569 RVA: 0x0000B07B File Offset: 0x0000927B
	public void SetSkin(GUISkin skin)
	{
		this.skin = skin;
	}

	// Token: 0x06000DF2 RID: 3570 RVA: 0x0000B084 File Offset: 0x00009284
	public bool AssertValidityOrMark()
	{
		if (this.firstStringIsSelection && this.selectedIndex == this.nothingSelectedIndex)
		{
			this.nothingSelectedError = true;
			return false;
		}
		return true;
	}

	// Token: 0x06000DF3 RID: 3571 RVA: 0x0000B0AC File Offset: 0x000092AC
	public Dropdown SetNothingSelectedIndex(int index)
	{
		this.firstStringIsSelection = true;
		this.nothingSelectedIndex = index;
		return this;
	}

	// Token: 0x06000DF4 RID: 3572 RVA: 0x0000B0BD File Offset: 0x000092BD
	public int GetSelectedIndex()
	{
		if (this.strings == null || this.strings.Length == 0)
		{
			return -1;
		}
		return this.selectedIndex;
	}

	// Token: 0x06000DF5 RID: 3573 RVA: 0x0000B0DF File Offset: 0x000092DF
	public void SetSelectedIndex(int index)
	{
		if (this.strings == null || index < 0 || index >= this.strings.Length)
		{
			return;
		}
		this.selectedIndex = index;
	}

	// Token: 0x06000DF6 RID: 3574 RVA: 0x0000B109 File Offset: 0x00009309
	public string GetSelectedItem()
	{
		if (this.strings == null || this.strings.Length == 0)
		{
			return null;
		}
		return this.strings[this.selectedIndex];
	}

	// Token: 0x06000DF7 RID: 3575 RVA: 0x00060014 File Offset: 0x0005E214
	public void SetSelectedItem(string item)
	{
		if (this.strings == null)
		{
			return;
		}
		for (int i = 0; i < this.strings.Length; i++)
		{
			if (this.strings[i] == item)
			{
				this.selectedIndex = i;
				break;
			}
		}
	}

	// Token: 0x06000DF8 RID: 3576 RVA: 0x0000B132 File Offset: 0x00009332
	public void SetSelectedItem(int index)
	{
		this.selectedIndex = index;
	}

	// Token: 0x06000DF9 RID: 3577 RVA: 0x0000B13B File Offset: 0x0000933B
	public void OverlayClicked()
	{
		this.isOpen = false;
		this.overlay.enabled = false;
	}

	// Token: 0x06000DFA RID: 3578 RVA: 0x0000B150 File Offset: 0x00009350
	private bool GUIButton(Rect r, string text)
	{
		if (GUI.Button(r, text))
		{
			App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
			return true;
		}
		return false;
	}

	// Token: 0x04000AD9 RID: 2777
	private GUIBlackOverlayButton overlay;

	// Token: 0x04000ADA RID: 2778
	private string[] strings;

	// Token: 0x04000ADB RID: 2779
	private int selectedIndex;

	// Token: 0x04000ADC RID: 2780
	private int nothingSelectedIndex;

	// Token: 0x04000ADD RID: 2781
	private bool isOpen;

	// Token: 0x04000ADE RID: 2782
	private Vector2 scrollPos;

	// Token: 0x04000ADF RID: 2783
	private Rect r = default(Rect);

	// Token: 0x04000AE0 RID: 2784
	private float maxRows;

	// Token: 0x04000AE1 RID: 2785
	private bool centerAlign;

	// Token: 0x04000AE2 RID: 2786
	private bool centerY;

	// Token: 0x04000AE3 RID: 2787
	private bool firstStringIsSelection;

	// Token: 0x04000AE4 RID: 2788
	private bool nothingSelectedError;

	// Token: 0x04000AE5 RID: 2789
	private int guiDepth;

	// Token: 0x04000AE6 RID: 2790
	private GUISkin skin;

	// Token: 0x020001BC RID: 444
	// (Invoke) Token: 0x06000DFC RID: 3580
	public delegate void DropdownChangedDelegate(int selectedIndex, string selection);
}
