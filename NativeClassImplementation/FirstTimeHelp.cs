using System;

// Token: 0x020001C0 RID: 448
public static class FirstTimeHelp
{
	// Token: 0x06000E03 RID: 3587 RVA: 0x0000B1E0 File Offset: 0x000093E0
	public static void arena(bool force)
	{
		FirstTimeHelp.showHelp("Arena", "This is the <color=#ffd033>Arena</color>, where you battle the computer or other players.\r\n\r\nTo start, we recommend playing the <color=#ffd033>Tutorial</color>. Once you're familiar with the basics of the game, <color=#ffd033>Skirmishes</color> against the AI and custom-rules <color=#ffd033>Trials</color> are a good way to improve your skills.\r\n\r\nWhen you're ready to face other players, hop into either <color=#ffd033>Quick Match</color> or <color=#ffd033>Ranked Match</color>. If you seek a true challenge, try your hand at <color=#ffd033>Judgement</color>, where you build a deck from random scrolls before taking it to the battlefield against other players.", App.Config.settings.greeted.arena, force);
	}

	// Token: 0x06000E04 RID: 3588 RVA: 0x0000B206 File Offset: 0x00009406
	public static void watch(bool force)
	{
		FirstTimeHelp.showHelp("Spectate", FirstTimeHelp.TextSpectate, App.Config.settings.greeted.spectate, force);
	}

	// Token: 0x06000E05 RID: 3589 RVA: 0x0000B22C File Offset: 0x0000942C
	public static void deckbuilder(bool force)
	{
		FirstTimeHelp.showHelp("Deckbuilder", "Welcome to the <color=#ffd033>Deckbuilder</color>! This is where you get to compose a custom deck of scrolls from your library. Drag scrolls from your library at the top of the screen to the tabletop to add them to a deck. Then save that deck for use in-game.\r\n\r\nOnly three of each scroll-type are allowed in any one deck, but you can reuse them in as many different saved decks as you please. Custom decks need to include at least 50 scrolls.\r\n\r\n<color=#ffd033>Hint</color>: You can drag an entire stack of scrolls by tapping it and holding for a moment.", App.Config.settings.greeted.deck_builder, force);
	}

	// Token: 0x06000E06 RID: 3590 RVA: 0x0000B252 File Offset: 0x00009452
	public static void crafting(bool force)
	{
		FirstTimeHelp.showHelp("Crafting", "<color=#ffd033>Crafting</color> is an excellent way to use excess scrolls. By merging three or the same scroll together, you craft a higher tier version of that scroll.\r\n\r\nThree regular scrolls become a <color=#ffd033>Tier 2</color>. This adds a statistics tab that records the scroll's performance, and adds a gold border. Upgrading these in turn gives you a <color=#ffd033>Tier 3</color>. It gives a small amount of bonus gold when drawn in matches, and is even shinier than the previous tier.", App.Config.settings.greeted.crafting, force);
	}

	// Token: 0x06000E07 RID: 3591 RVA: 0x0000B278 File Offset: 0x00009478
	public static void store(bool force)
	{
		FirstTimeHelp.showHelp("Store", FirstTimeHelp.TextStore, App.Config.settings.greeted.store, force);
	}

	// Token: 0x06000E08 RID: 3592 RVA: 0x0000B29E File Offset: 0x0000949E
	public static void profile(bool force)
	{
		FirstTimeHelp.showHelp("Profile", "To the left, you'll find statistics of your performance so far and a medal displaying your current <color=#ffd033>rank</color>, if you've earned one. Below that is a list of achievements you've unlocked.\r\n\r\nTo the right, you can change the appearance of your <color=#ffd033>avatar</color>. This is how you're presented in matches. You can get more avatar parts in the store.", App.Config.settings.greeted.profile, force);
	}

	// Token: 0x06000E09 RID: 3593 RVA: 0x0000B2C4 File Offset: 0x000094C4
	private static void showHelp(string header, string text, SvBool hasSeen, bool force)
	{
		if (!force && hasSeen)
		{
			return;
		}
		hasSeen.set(true);
		App.Popups.ShowOk(new OkVoidCallback(), "greeting_" + header, header, text, "Ok");
	}

	// Token: 0x06000E0A RID: 3594 RVA: 0x00060068 File Offset: 0x0005E268
	public static void showFirstTimeHelpFor(string scene, bool force)
	{
		if (scene == "_Lobby")
		{
			FirstTimeHelp.arena(force);
		}
		else if (scene == "_Watch")
		{
			FirstTimeHelp.watch(force);
		}
		else if (scene == "_Store")
		{
			FirstTimeHelp.store(force);
		}
		else if (scene == "_Profile")
		{
			FirstTimeHelp.profile(force);
		}
		else if (scene == "_CraftingView")
		{
			FirstTimeHelp.crafting(force);
		}
		else if (scene == "_DeckBuilderView")
		{
			FirstTimeHelp.deckbuilder(force);
		}
	}

	// Token: 0x04000AF0 RID: 2800
	private const string TextArena = "This is the <color=#ffd033>Arena</color>, where you battle the computer or other players.\r\n\r\nTo start, we recommend playing the <color=#ffd033>Tutorial</color>. Once you're familiar with the basics of the game, <color=#ffd033>Skirmishes</color> against the AI and custom-rules <color=#ffd033>Trials</color> are a good way to improve your skills.\r\n\r\nWhen you're ready to face other players, hop into either <color=#ffd033>Quick Match</color> or <color=#ffd033>Ranked Match</color>. If you seek a true challenge, try your hand at <color=#ffd033>Judgement</color>, where you build a deck from random scrolls before taking it to the battlefield against other players.";

	// Token: 0x04000AF1 RID: 2801
	private const string TextDeckbuilder = "Welcome to the <color=#ffd033>Deckbuilder</color>! This is where you get to compose a custom deck of scrolls from your library. Drag scrolls from your library at the top of the screen to the tabletop to add them to a deck. Then save that deck for use in-game.\r\n\r\nOnly three of each scroll-type are allowed in any one deck, but you can reuse them in as many different saved decks as you please. Custom decks need to include at least 50 scrolls.\r\n\r\n<color=#ffd033>Hint</color>: You can drag an entire stack of scrolls by tapping it and holding for a moment.";

	// Token: 0x04000AF2 RID: 2802
	private const string TextCrafting = "<color=#ffd033>Crafting</color> is an excellent way to use excess scrolls. By merging three or the same scroll together, you craft a higher tier version of that scroll.\r\n\r\nThree regular scrolls become a <color=#ffd033>Tier 2</color>. This adds a statistics tab that records the scroll's performance, and adds a gold border. Upgrading these in turn gives you a <color=#ffd033>Tier 3</color>. It gives a small amount of bonus gold when drawn in matches, and is even shinier than the previous tier.";

	// Token: 0x04000AF3 RID: 2803
	private const string TextProfile = "To the left, you'll find statistics of your performance so far and a medal displaying your current <color=#ffd033>rank</color>, if you've earned one. Below that is a list of achievements you've unlocked.\r\n\r\nTo the right, you can change the appearance of your <color=#ffd033>avatar</color>. This is how you're presented in matches. You can get more avatar parts in the store.";

	// Token: 0x04000AF4 RID: 2804
	private static readonly string TextSpectate = I18n.Text("Watch others playing {GAME_NAME} here. It's a great place to get some insight into how the top ranked players perform, or to hang out while you wait for a match. If you don't like the idea of other players spectating your matches, visit the settings menu.");

	// Token: 0x04000AF5 RID: 2805
	private static readonly string TextStore = I18n.Text("When you play {GAME_NAME} matches, you earn <color=#ffd033>Gold</color>. Use your Gold here to get new scrolls. You can buy single random scrolls, various packs, pre-constructed decks or upgrade your avatar.\r\n\r\nThe <color=#ffd033>Just for You</color> section shows a regularly refreshed selection of scrolls available face up, at a higher cost.\r\n\r\nEverything in the Store is available for Gold that you earn while playing. Some items can also be purchased with Shards, which is our real-money equivalent. These options are significantly limited to keep everyone at equal footing.");
}
