using System;
using UnityEngine;

// Token: 0x020001C2 RID: 450
public static class GUITags
{
	// Token: 0x06000E1F RID: 3615 RVA: 0x0000B45E File Offset: 0x0000965E
	public static GUIContent lockTutorial(this GUIContent c)
	{
		return c.addTag(GUITags.AlwaysLock).addTag(GUITags.Tutorial).addTag(GUITags.Tooltip);
	}

	// Token: 0x06000E20 RID: 3616 RVA: 0x0000B47F File Offset: 0x0000967F
	public static GUIContent lockDemo(this GUIContent c)
	{
		return c.addTag(GUITags.Premium).addTag(GUITags.Tooltip);
	}

	// Token: 0x06000E21 RID: 3617 RVA: 0x0000B496 File Offset: 0x00009696
	public static GUIContent lockDemoNoTooltip(this GUIContent c)
	{
		return c.addTag(GUITags.Premium);
	}

	// Token: 0x06000E22 RID: 3618 RVA: 0x0000B4A3 File Offset: 0x000096A3
	public static GUIContent lockDemoHideIfPopup(this GUIContent c)
	{
		return c.addTag(GUITags.Premium).addTag(GUITags.TooltipIfNoPopup);
	}

	// Token: 0x06000E23 RID: 3619 RVA: 0x0000B4BA File Offset: 0x000096BA
	public static GUIContent center(this GUIContent c)
	{
		return c.addTag(GUITags.Center);
	}

	// Token: 0x06000E24 RID: 3620 RVA: 0x0000B4C7 File Offset: 0x000096C7
	public static GUIContent helpArrow(this GUIContent c)
	{
		return c.addTag(GUITags.HelpArrow);
	}

	// Token: 0x06000E25 RID: 3621 RVA: 0x0006092C File Offset: 0x0005EB2C
	private static GUIContent addTag(this GUIContent c, char tag)
	{
		string text = c.tooltip ?? string.Empty;
		c.tooltip = ((!GUITags.hasTags(text)) ? (GUITags.TagSeparator + tag) : (text + tag));
		return c;
	}

	// Token: 0x06000E26 RID: 3622 RVA: 0x00060980 File Offset: 0x0005EB80
	public static void clearTags(this GUIContent c)
	{
		if (c.tooltip == null)
		{
			return;
		}
		int tagsStartIndex = GUITags.getTagsStartIndex(c.tooltip);
		if (tagsStartIndex < 0)
		{
			return;
		}
		c.tooltip = c.tooltip.Substring(0, tagsStartIndex);
	}

	// Token: 0x06000E27 RID: 3623 RVA: 0x000609C0 File Offset: 0x0005EBC0
	public static GUIContent clearedTagsCopy(this GUIContent c)
	{
		GUIContent guicontent = new GUIContent(c);
		guicontent.clearTags();
		return guicontent;
	}

	// Token: 0x06000E28 RID: 3624 RVA: 0x0000B4D4 File Offset: 0x000096D4
	private static int getTagsStartIndex(string s)
	{
		return (s == null) ? -1 : s.IndexOf(GUITags.TagSeparator);
	}

	// Token: 0x06000E29 RID: 3625 RVA: 0x0000B4ED File Offset: 0x000096ED
	private static bool hasTags(string s)
	{
		return s != null && GUITags.getTagsStartIndex(s) >= 0;
	}

	// Token: 0x06000E2A RID: 3626 RVA: 0x000609DC File Offset: 0x0005EBDC
	public static bool hasTag(this GUIContent c, char tag)
	{
		int tagsStartIndex = GUITags.getTagsStartIndex(c.tooltip);
		return tagsStartIndex >= 0 && c.tooltip.IndexOf(tag, tagsStartIndex) >= 0;
	}

	// Token: 0x04000AF9 RID: 2809
	private static readonly string TagSeparator = "_%_";

	// Token: 0x04000AFA RID: 2810
	public static readonly char AlwaysLock = 'L';

	// Token: 0x04000AFB RID: 2811
	public static readonly char DemoLock = 'l';

	// Token: 0x04000AFC RID: 2812
	public static readonly char Premium = 'p';

	// Token: 0x04000AFD RID: 2813
	public static readonly char Tooltip = 't';

	// Token: 0x04000AFE RID: 2814
	public static readonly char TooltipIfNoPopup = 'n';

	// Token: 0x04000AFF RID: 2815
	public static readonly char Center = 'c';

	// Token: 0x04000B00 RID: 2816
	public static readonly char HelpArrow = 'h';

	// Token: 0x04000B01 RID: 2817
	public static readonly char Tutorial = 'u';

	// Token: 0x04000B02 RID: 2818
	public static readonly GUIContent DemoLocked = new GUIContent().lockDemo();

	// Token: 0x04000B03 RID: 2819
	public static readonly GUIContent DemoLockedNoTooltip = new GUIContent().lockDemoNoTooltip();

	// Token: 0x04000B04 RID: 2820
	public static readonly GUIContent TutorialLocked = new GUIContent().lockTutorial();
}
