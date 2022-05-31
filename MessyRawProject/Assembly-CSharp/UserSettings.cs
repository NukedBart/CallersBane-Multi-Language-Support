using System;

// Token: 0x020003D1 RID: 977
public class UserSettings
{
	// Token: 0x060015A7 RID: 5543 RVA: 0x0000FC9A File Offset: 0x0000DE9A
	public bool showTutorialArrows()
	{
		return this.isFirstStart;
	}

	// Token: 0x060015A8 RID: 5544 RVA: 0x000848F8 File Offset: 0x00082AF8
	public void onLoaded()
	{
		UserSettings.First_Time_Help_Texts first_Time_Help_Texts = this.greeted;
		this.isFirstStart = (!first_Time_Help_Texts.arena && !first_Time_Help_Texts.spectate && !first_Time_Help_Texts.deck_builder && !first_Time_Help_Texts.crafting && !first_Time_Help_Texts.store && !first_Time_Help_Texts.profile);
	}

	// Token: 0x040012EF RID: 4847
	public UserSettings.Battle battle = new UserSettings.Battle();

	// Token: 0x040012F0 RID: 4848
	public UserSettings.DeckBuilder deck_builder = new UserSettings.DeckBuilder();

	// Token: 0x040012F1 RID: 4849
	public UserSettings.Sound sound = new UserSettings.Sound();

	// Token: 0x040012F2 RID: 4850
	public UserSettings.Music music = new UserSettings.Music();

	// Token: 0x040012F3 RID: 4851
	public UserSettings.Chat chat = new UserSettings.Chat();

	// Token: 0x040012F4 RID: 4852
	public UserSettings.Preferences preferences = new UserSettings.Preferences();

	// Token: 0x040012F5 RID: 4853
	public SvBool spectate_show_unstarted = new SvBool(true);

	// Token: 0x040012F6 RID: 4854
	public SvString first_version = new SvString(SharedConstants.getGameVersion().ToString());

	// Token: 0x040012F7 RID: 4855
	private bool isFirstStart;

	// Token: 0x040012F8 RID: 4856
	public UserSettings.CardList cardlist = new UserSettings.CardList();

	// Token: 0x040012F9 RID: 4857
	public UserSettings.First_Time_Help_Texts greeted = new UserSettings.First_Time_Help_Texts();

	// Token: 0x020003D2 RID: 978
	public class Battle : ISettingsGroup
	{
		// Token: 0x060015AA RID: 5546 RVA: 0x00084974 File Offset: 0x00082B74
		public BackgroundData getBackground()
		{
			int backgroundIdFor = BackgroundData.getBackgroundIdFor(this.background);
			return (backgroundIdFor >= 0) ? BackgroundData.getBackground((long)backgroundIdFor) : null;
		}

		// Token: 0x040012FA RID: 4858
		public SvBool show_unit_stats = new SvBool(true);

		// Token: 0x040012FB RID: 4859
		public SvString background = new SvString();
	}

	// Token: 0x020003D3 RID: 979
	public class DeckBuilder : ISettingsGroup
	{
		// Token: 0x040012FC RID: 4860
		public SvEnum<DeckBuilder2.SortMode> sort_order = new SvEnum<DeckBuilder2.SortMode>(DeckBuilder2.SortMode.Name);
	}

	// Token: 0x020003D4 RID: 980
	public class Sound : ISettingsGroup
	{
		// Token: 0x040012FD RID: 4861
		public SvFloat volume = new SvFloat(0.25f).Unit();

		// Token: 0x040012FE RID: 4862
		public SvBool chat_message = new SvBool(true);

		// Token: 0x040012FF RID: 4863
		public SvBool chat_highlight = new SvBool(true);
	}

	// Token: 0x020003D5 RID: 981
	public class Music : ISettingsGroup
	{
		// Token: 0x04001300 RID: 4864
		public SvFloat volume = new SvFloat(0.25f).Unit();

		// Token: 0x04001301 RID: 4865
		public SvBool pause_when_minimized = new SvBool(true);
	}

	// Token: 0x020003D6 RID: 982
	public class Chat : ISettingsGroup
	{
		// Token: 0x04001302 RID: 4866
		public SvList<string> rooms = new SvList<string>();
	}

	// Token: 0x020003D7 RID: 983
	public class Preferences : ISettingsGroup
	{
		// Token: 0x04001303 RID: 4867
		public SvBool tier_visuals = new SvBool(true);

		// Token: 0x04001304 RID: 4868
		public SvBool profanity_filter = new SvBool(true);
	}

	// Token: 0x020003D8 RID: 984
	public class CardList : ISettingsGroup
	{
		// Token: 0x04001305 RID: 4869
		public SvBool marketplace_buy = new SvBool(true);

		// Token: 0x04001306 RID: 4870
		public SvBool marketplace_sold = new SvBool(true);

		// Token: 0x04001307 RID: 4871
		public SvBool marketplace_library = new SvBool(true);

		// Token: 0x04001308 RID: 4872
		public SvBool store_sell = new SvBool(true);

		// Token: 0x04001309 RID: 4873
		public SvBool trade_p1 = new SvBool(true);

		// Token: 0x0400130A RID: 4874
		public SvBool trade_p2 = new SvBool(true);
	}

	// Token: 0x020003D9 RID: 985
	public class First_Time_Help_Texts : ISettingsGroup
	{
		// Token: 0x0400130B RID: 4875
		public SvBool arena = new SvBool(false);

		// Token: 0x0400130C RID: 4876
		public SvBool spectate = new SvBool(false);

		// Token: 0x0400130D RID: 4877
		public SvBool deck_builder = new SvBool(false);

		// Token: 0x0400130E RID: 4878
		public SvBool crafting = new SvBool(false);

		// Token: 0x0400130F RID: 4879
		public SvBool store = new SvBool(false);

		// Token: 0x04001310 RID: 4880
		public SvBool profile = new SvBool(false);
	}
}
