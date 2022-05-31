using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000393 RID: 915
public class AchievementList : MonoBehaviour
{
	// Token: 0x06001465 RID: 5221 RVA: 0x0000F079 File Offset: 0x0000D279
	public static void drawAchievementIcon(Rect rect, AchievementType t)
	{
		AchievementList.drawAchievementIcon(rect, t, -1f);
	}

	// Token: 0x06001466 RID: 5222 RVA: 0x0007EA58 File Offset: 0x0007CC58
	public static void drawAchievementIcon(Rect rect, AchievementType t, float glow)
	{
		string icon = t.getIcon();
		Texture2D texture2D = ResourceManager.LoadTexture("Achievement/" + icon);
		if (texture2D == null)
		{
			texture2D = ResourceManager.LoadTexture("Achievement/Misc.png");
		}
		GUI.DrawTexture(rect, ResourceManager.LoadTexture("Achievement/Frame"));
		GUI.DrawTexture(rect, texture2D);
		if (glow > 0f)
		{
			Texture2D texture2D2 = ResourceManager.LoadTexture("Achievement/glow/" + icon);
			if (texture2D2 == null)
			{
				texture2D2 = ResourceManager.LoadTexture("Achievement/glow/Misc.png");
			}
			GUI.color = new Color(1f, 1f, 1f, glow);
			GUI.DrawTexture(rect, texture2D2);
			GUI.color = Color.white;
		}
	}

	// Token: 0x06001467 RID: 5223 RVA: 0x0007EB0C File Offset: 0x0007CD0C
	public void init(Rect rect, float margin, short[] unlockedTypes)
	{
		this.unlockedIds = new List<short>(unlockedTypes);
		this.all = AchievementTypeManager.getInstance().getAll();
		this.margin = margin;
		this.processAchievements();
		this.skin = (GUISkin)ResourceManager.Load("_GUISkins/RegularUI");
		this.achievementListBgStyle = new GUIStyle(this.skin.box);
		this.achievementListBgStyle.normal.background = ResourceManager.LoadTexture("ChatUI/black_box_border");
		this.scrollPosition = Vector2.zero;
		this.setRect(rect);
	}

	// Token: 0x06001468 RID: 5224 RVA: 0x0007EB9C File Offset: 0x0007CD9C
	public void setRect(Rect rect)
	{
		this.rect = rect;
		this.itemRect = new Rect(0f, 0f, rect.width, (float)Screen.height * 0.0875f);
		this.itemRect.width = this.itemRect.width - 20f;
		this.frameRect = GeomUtil.inflate(rect, this.margin, this.margin);
		this.scrollArea = new Rect(0f, 0f, this.itemRect.width, (float)this.achievements.Count * this.itemRect.height);
		this.initedAchievementRects = false;
	}

	// Token: 0x06001469 RID: 5225 RVA: 0x0007EC48 File Offset: 0x0007CE48
	private void setupAchievementRects()
	{
		this.initedAchievementRects = true;
		float num = GUI.skin.label.CalcHeight(new GUIContent("M"), this.itemRect.width);
		Rect rect = this.itemRect;
		for (int i = 0; i < this.achievements.Count; i++)
		{
			AchievementList.Achievement achievement = this.achievements[i];
			rect.y = (float)i * this.itemRect.height;
			achievement.rect = rect;
			Rect iconRect = this.itemRect;
			float num2 = this.itemRect.height * 0.8f;
			float num3 = num2;
			iconRect.height = num3;
			iconRect.width = num3;
			iconRect.y = (this.itemRect.height - num2) * 0.5f;
			iconRect.x = num2 * 0.125f;
			achievement.iconRect = iconRect;
			float num4 = GUI.skin.label.CalcHeight(new GUIContent(achievement.type.description), rect.width);
			if (num4 > num)
			{
				achievement.twoLines = true;
			}
			Rect rect2 = this.itemRect;
			if (!achievement.twoLines)
			{
				rect2.y += (float)Screen.height * 0.01f;
			}
			rect2.xMin = iconRect.width + 2f * achievement.iconRect.x;
			achievement.nameRect = rect2;
			if (achievement.twoLines)
			{
				rect2.y += (float)Screen.height * 0.03f;
			}
			else
			{
				rect2.y += (float)Screen.height * 0.03f;
			}
			achievement.descRect = rect2;
		}
	}

	// Token: 0x0600146A RID: 5226 RVA: 0x0007EE08 File Offset: 0x0007D008
	private void OnGUI()
	{
		GUI.depth = 20;
		GUI.skin = this.skin;
		if (!this.initedAchievementRects)
		{
			this.setupAchievementRects();
			if (this.pendingScrollToAchievement != null)
			{
				this._scrollTo(this.pendingScrollToAchievement.Value);
				this.pendingScrollToAchievement = default(int?);
			}
		}
		new ScrollsFrame(this.frameRect).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.CENTER).Draw();
		this.scrollPosition = GUI.BeginScrollView(this.rect, this.scrollPosition, this.scrollArea);
		Rect rect = this.itemRect;
		GUISkin guiskin = GUI.skin;
		TextAnchor alignment = GUI.skin.label.alignment;
		int num = (int)(this.scrollPosition.y / this.itemRect.height) - 1;
		for (int i = Math.Max(0, num); i < this.achievements.Count; i++)
		{
			AchievementList.Achievement achievement = this.achievements[i];
			if (achievement.rect.yMax >= this.scrollPosition.y)
			{
				GUI.BeginGroup(achievement.rect);
				Rect rect2;
				rect2..ctor(0f, 0f, achievement.rect.width, achievement.rect.height);
				this.renderItem(rect2, achievement);
				GUI.EndGroup();
				if (achievement.rect.y - this.scrollPosition.y > this.rect.height)
				{
					break;
				}
			}
		}
		GUI.skin.label.alignment = alignment;
		GUI.EndScrollView();
		if (this.hovered != null)
		{
			App.Tooltip.setText(this.hovered.name + "\n" + this.hovered.description);
		}
		GUI.skin = guiskin;
	}

	// Token: 0x0600146B RID: 5227 RVA: 0x0007EFF0 File Offset: 0x0007D1F0
	private void renderItem(Rect rect, AchievementList.Achievement a)
	{
		AchievementType type = a.type;
		GUIColorStack guicolorStack = new GUIColorStack().push();
		guicolorStack.push((!a.unlocked) ? new Color(0.6f, 0.6f, 0.6f) : Color.white);
		float num = 6f;
		Rect translated = GeomUtil.getTranslated(GeomUtil.inflate(rect, -num, -num), 0f, -num / 6f);
		GUI.Box(rect, string.Empty, this.achievementListBgStyle);
		float num2 = 0.7f;
		GUI.color = ((!a.unlocked) ? new Color(num2, num2, num2, 1f) : Color.white);
		AchievementList.drawAchievementIcon(a.iconRect, a.type);
		GUI.skin.label.alignment = 0;
		GUI.skin.label.fontSize = Screen.height / 30;
		Color color = (!a.unlocked) ? this.lockedColor : Color.white;
		GUIUtil.drawShadowText(a.nameRect, a.getName(), color, 0, 3);
		GUI.skin.label.fontSize = Screen.height / 42;
		Color color2;
		color2..ctor(0.65f, 0.6f, 0.47f);
		GUIUtil.drawShadowText(a.descRect, type.description, color2, 0, 3);
		int count = a.parts.Count;
		Vector2 screenMousePos = GUIUtil.getScreenMousePos();
		if (count > 1 && a.type.partType != null)
		{
			Rect rect2 = GeomUtil.cropShare(rect, new Rect(1f, 0.5f, 0f, 0f));
			float num3 = (float)Screen.height * 0.015f;
			rect2 = GeomUtil.inflate(rect2, num3, num3);
			for (int i = 0; i < count; i++)
			{
				AchievementList.Achievement achievement = a.parts[count - 1 - i];
				rect2.x -= rect2.width;
				bool unlocked = achievement.unlocked;
				Texture2D texture2D = ResourceManager.LoadTexture("Arena/scroll_browser_button_cb" + ((!unlocked) ? string.Empty : "_checked"));
				GUI.DrawTexture(rect2, texture2D);
			}
		}
		guicolorStack.reset();
	}

	// Token: 0x0600146C RID: 5228 RVA: 0x0000F087 File Offset: 0x0000D287
	public void scrollTo(int achievementId)
	{
		if (!this.initedAchievementRects)
		{
			this.pendingScrollToAchievement = new int?(achievementId);
			return;
		}
		this._scrollTo(achievementId);
	}

	// Token: 0x0600146D RID: 5229 RVA: 0x0007F23C File Offset: 0x0007D43C
	private void _scrollTo(int achievementId)
	{
		for (int i = 0; i < this.achievements.Count; i++)
		{
			AchievementList.Achievement achievement = this.achievements[i];
			AchievementList.Achievement achievement2 = achievement;
			if ((int)achievement2.type.id == achievementId)
			{
				this.scrollPosition = new Vector2(this.scrollPosition.x, achievement.rect.y);
				Log.warning("found!");
				return;
			}
		}
	}

	// Token: 0x0600146E RID: 5230 RVA: 0x0007F2B4 File Offset: 0x0007D4B4
	private AchievementList.Achievement getParent(AchievementList.Achievement child)
	{
		foreach (AchievementList.Achievement achievement in this.achievements)
		{
			if (achievement.type.id == child.type.id)
			{
				break;
			}
			if (achievement.type.group == child.type.group)
			{
				return achievement;
			}
		}
		return null;
	}

	// Token: 0x0600146F RID: 5231 RVA: 0x0007F34C File Offset: 0x0007D54C
	private AchievementList.Achievement addAchievement(AchievementType type)
	{
		if (type == null)
		{
			return null;
		}
		bool unlocked = this.unlockedIds.Contains(type.id);
		AchievementList.Achievement achievement = new AchievementList.Achievement(type, unlocked);
		this.achievements.Add(achievement);
		return achievement;
	}

	// Token: 0x06001470 RID: 5232 RVA: 0x0007F388 File Offset: 0x0007D588
	private void processAchievements()
	{
		this.achievements.Clear();
		AchievementTypeManager instance = AchievementTypeManager.getInstance();
		foreach (short groupId in instance.getGroups())
		{
			if (instance.isFillingAchievement(groupId))
			{
				AchievementType type = instance.getHighestUnlocked(groupId, this.unlockedIds) ?? instance.getLowestLocked(groupId, this.unlockedIds);
				AchievementList.Achievement achievement = this.addAchievement(type);
				instance.getUnlockedCount(groupId, this.unlockedIds, out achievement.unlockedCount, out achievement.totalCount);
				achievement.unlocked = (achievement.unlockedCount == achievement.totalCount);
			}
			else if (instance.isProgressAchievement(groupId))
			{
				this.addAchievement(instance.getHighestUnlocked(groupId, this.unlockedIds));
				this.addAchievement(instance.getLowestLocked(groupId, this.unlockedIds));
			}
			else
			{
				foreach (AchievementType type2 in instance.getGroup(groupId))
				{
					this.addAchievement(type2);
				}
			}
		}
	}

	// Token: 0x040011A3 RID: 4515
	public AchievementType hovered;

	// Token: 0x040011A4 RID: 4516
	private Rect rect;

	// Token: 0x040011A5 RID: 4517
	private Rect frameRect;

	// Token: 0x040011A6 RID: 4518
	private List<AchievementType> all;

	// Token: 0x040011A7 RID: 4519
	private List<AchievementList.Achievement> achievements = new List<AchievementList.Achievement>();

	// Token: 0x040011A8 RID: 4520
	private List<short> unlockedIds;

	// Token: 0x040011A9 RID: 4521
	private Rect itemRect;

	// Token: 0x040011AA RID: 4522
	private bool initedAchievementRects;

	// Token: 0x040011AB RID: 4523
	private int? pendingScrollToAchievement;

	// Token: 0x040011AC RID: 4524
	private Vector2 scrollPosition;

	// Token: 0x040011AD RID: 4525
	private Rect scrollArea;

	// Token: 0x040011AE RID: 4526
	private float margin;

	// Token: 0x040011AF RID: 4527
	private GUISkin skin;

	// Token: 0x040011B0 RID: 4528
	private GUIStyle achievementListBgStyle;

	// Token: 0x040011B1 RID: 4529
	public Color bgColorUnlocked;

	// Token: 0x040011B2 RID: 4530
	public Color bgColor;

	// Token: 0x040011B3 RID: 4531
	public Color lockedColor = ColorUtil.FromInts(133, 129, 119);

	// Token: 0x02000394 RID: 916
	private class Achievement
	{
		// Token: 0x06001471 RID: 5233 RVA: 0x0000F0A8 File Offset: 0x0000D2A8
		public Achievement(AchievementType type, bool unlocked)
		{
			this.type = type;
			this.unlocked = unlocked;
		}

		// Token: 0x06001472 RID: 5234 RVA: 0x0007F4FC File Offset: 0x0007D6FC
		public string getName()
		{
			if (this.unlockedCount == 0)
			{
				if (this.type.id == 96)
				{
					return "Order Starter Deck 0/5";
				}
				if (this.type.id == 101)
				{
					return "Energy Starter Deck 0/3";
				}
				if (this.type.id == 104)
				{
					return "Decay Starter Deck 0/7";
				}
			}
			return this.type.name;
		}

		// Token: 0x040011B4 RID: 4532
		public AchievementType type;

		// Token: 0x040011B5 RID: 4533
		public bool unlocked;

		// Token: 0x040011B6 RID: 4534
		public int unlockedCount;

		// Token: 0x040011B7 RID: 4535
		public int totalCount;

		// Token: 0x040011B8 RID: 4536
		public Rect rect;

		// Token: 0x040011B9 RID: 4537
		public Rect iconRect;

		// Token: 0x040011BA RID: 4538
		public Rect nameRect;

		// Token: 0x040011BB RID: 4539
		public Rect descRect;

		// Token: 0x040011BC RID: 4540
		public bool twoLines;

		// Token: 0x040011BD RID: 4541
		public List<AchievementList.Achievement> parts = new List<AchievementList.Achievement>();
	}
}
