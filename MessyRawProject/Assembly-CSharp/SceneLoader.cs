using System;
using System.Linq;
using UnityEngine;

// Token: 0x020003B6 RID: 950
public class SceneLoader : MonoBehaviour
{
	// Token: 0x14000006 RID: 6
	// (add) Token: 0x0600153E RID: 5438 RVA: 0x0000F8B7 File Offset: 0x0000DAB7
	// (remove) Token: 0x0600153F RID: 5439 RVA: 0x0000F8CE File Offset: 0x0000DACE
	public static event SceneLoader.SceneNotification OnSceneWillLoad;

	// Token: 0x14000007 RID: 7
	// (add) Token: 0x06001540 RID: 5440 RVA: 0x0000F8E5 File Offset: 0x0000DAE5
	// (remove) Token: 0x06001541 RID: 5441 RVA: 0x0000F8FC File Offset: 0x0000DAFC
	public static event SceneLoader.SceneNotification OnSceneWillUnload;

	// Token: 0x14000008 RID: 8
	// (add) Token: 0x06001542 RID: 5442 RVA: 0x0000F913 File Offset: 0x0000DB13
	// (remove) Token: 0x06001543 RID: 5443 RVA: 0x0000F92A File Offset: 0x0000DB2A
	public static event SceneLoader.Scene2Notification OnSceneHasLoaded;

	// Token: 0x06001544 RID: 5444 RVA: 0x00082DE0 File Offset: 0x00080FE0
	private void OnLevelWasLoaded()
	{
		if (SceneLoader.OnSceneHasLoaded != null)
		{
			SceneLoader.OnSceneHasLoaded(SceneLoader.getScene(), SceneLoader.previousScene);
		}
		SceneLoader.previousScene = SceneLoader.getScene();
		FirstTimeHelp.showFirstTimeHelpFor(SceneLoader.getScene(), false);
		App.GlobalMessageHandler.setTimeScale(1f);
		App.Config.flushSettings();
	}

	// Token: 0x06001545 RID: 5445 RVA: 0x00082E3C File Offset: 0x0008103C
	public static void loadScene(string sceneName)
	{
		SceneLoader.ValueFinder<string> valueFinder = new SceneLoader.ValueFinder<string>(SceneLoader.getScene());
		SceneLoader.ValueFinder<string> valueFinder2 = new SceneLoader.ValueFinder<string>(sceneName);
		if (valueFinder.isAnyOf(new string[]
		{
			"_DeckBuilderView",
			"_Store",
			"_CraftingView"
		}) && valueFinder2.isNoneOf(new string[]
		{
			"_BattleModeView",
			"_DeckBuilderView",
			"_Store",
			"_CraftingView"
		}))
		{
			App.AudioScript.PlayMusic("Music/Theme");
		}
		if ((sceneName == "_CraftingView" && SceneLoader.getScene() != "_DeckBuilderView") || (sceneName == "_DeckBuilderView" && SceneLoader.getScene() != "_CraftingView"))
		{
			App.AudioScript.StopSoundsOfType(AudioScript.ESoundType.MUSIC, true, AudioScript.EPostFadeoutBehaviour.PAUSE);
		}
		if (valueFinder2.isAnyOf(new string[]
		{
			"_DeckBuilderView"
		}) && App.SceneValues.deckBuilder == null)
		{
			App.SceneValues.deckBuilder = new SceneValues.SV_DeckBuilder();
		}
		if (SceneLoader.OnSceneWillUnload != null)
		{
			SceneLoader.OnSceneWillUnload(SceneLoader.getScene());
		}
		if (SceneLoader.OnSceneWillLoad != null)
		{
			SceneLoader.OnSceneWillLoad(sceneName);
		}
		Log.warning("Loading scene: " + sceneName);
		Application.LoadLevel(sceneName);
	}

	// Token: 0x06001546 RID: 5446 RVA: 0x00082F94 File Offset: 0x00081194
	public static bool isScene(params string[] scenes)
	{
		string scene = SceneLoader.getScene();
		foreach (string text in scenes)
		{
			if (scene == text)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001547 RID: 5447 RVA: 0x0000F941 File Offset: 0x0000DB41
	public static string getScene()
	{
		return Application.loadedLevelName;
	}

	// Token: 0x04001272 RID: 4722
	public const string Login = "_LoginView";

	// Token: 0x04001273 RID: 4723
	public const string Home = "_HomeScreen";

	// Token: 0x04001274 RID: 4724
	public const string Lobby = "_Lobby";

	// Token: 0x04001275 RID: 4725
	public const string Store = "_Store";

	// Token: 0x04001276 RID: 4726
	public const string Profile = "_Profile";

	// Token: 0x04001277 RID: 4727
	public const string Settings = "_Settings";

	// Token: 0x04001278 RID: 4728
	public const string BattleMode = "_BattleModeView";

	// Token: 0x04001279 RID: 4729
	public const string Watch = "_Watch";

	// Token: 0x0400127A RID: 4730
	public const string DeckBuilder = "_DeckBuilderView";

	// Token: 0x0400127B RID: 4731
	public const string Crafting = "_CraftingView";

	// Token: 0x0400127C RID: 4732
	public const string SelectPrecon = "_SelectPreconstructed";

	// Token: 0x0400127D RID: 4733
	public const string CustomGames = "_CustomGames";

	// Token: 0x0400127E RID: 4734
	private static string previousScene;

	// Token: 0x020003B7 RID: 951
	private class ValueFinder<T>
	{
		// Token: 0x06001548 RID: 5448 RVA: 0x0000F948 File Offset: 0x0000DB48
		public ValueFinder(T value)
		{
			this.value = value;
		}

		// Token: 0x06001549 RID: 5449 RVA: 0x0000F957 File Offset: 0x0000DB57
		public bool isAnyOf(params T[] values)
		{
			return Enumerable.Contains<T>(values, this.value);
		}

		// Token: 0x0600154A RID: 5450 RVA: 0x0000F965 File Offset: 0x0000DB65
		public bool isNoneOf(params T[] values)
		{
			return !this.isAnyOf(values);
		}

		// Token: 0x04001282 RID: 4738
		private T value;
	}

	// Token: 0x020003B8 RID: 952
	// (Invoke) Token: 0x0600154C RID: 5452
	public delegate void SceneNotification(string sceneName);

	// Token: 0x020003B9 RID: 953
	// (Invoke) Token: 0x06001550 RID: 5456
	public delegate void Scene2Notification(string sceneName, string previous);
}
