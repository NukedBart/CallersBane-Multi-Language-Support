using System;

// Token: 0x02000087 RID: 135
public static class GameTypeExtension
{
	// Token: 0x06000505 RID: 1285 RVA: 0x00005376 File Offset: 0x00003576
	public static bool isTutorial(this GameType t)
	{
		return t == GameType.SP_TUTORIAL;
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x0000537C File Offset: 0x0000357C
	public static bool isSinglePlayer(this GameType t)
	{
		return t.ToString().StartsWith("SP");
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x00005393 File Offset: 0x00003593
	public static bool isMultiplayer(this GameType t)
	{
		return !t.isSinglePlayer();
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x0000539E File Offset: 0x0000359E
	public static bool isMultiplayerChallenge(this GameType t)
	{
		return t == GameType.MP_UNRANKED;
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x000053A4 File Offset: 0x000035A4
	public static bool isLimited(this GameType t)
	{
		return t == GameType.MP_LIMITED;
	}

	// Token: 0x0600050A RID: 1290 RVA: 0x000053AA File Offset: 0x000035AA
	public static bool isMultiplayerQuickmatch(this GameType t)
	{
		return t == GameType.MP_QUICKMATCH;
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x000053B0 File Offset: 0x000035B0
	public static bool isMultiplayerRanked(this GameType t)
	{
		return t == GameType.MP_RANKED;
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x000053B6 File Offset: 0x000035B6
	public static bool isTestingGrounds(this GameType t)
	{
		return t.isMultiplayerQuickmatch();
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x00004AAC File Offset: 0x00002CAC
	public static bool hasWildResources(this GameType t)
	{
		return true;
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x0003641C File Offset: 0x0003461C
	public static string getPrefix(this GameType t, bool capitalize)
	{
		if (t == GameType.None)
		{
			return "[No GameType]";
		}
		if (t == GameType.MP_LIMITED)
		{
			return "Judgement";
		}
		string text = t.ToString().Substring(3);
		if (t == GameType.MP_QUICKMATCH)
		{
			text = "quick";
		}
		return (!capitalize) ? text.ToLower() : StringUtil.capitalize(text);
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x00036478 File Offset: 0x00034678
	public static string getString(this GameType t)
	{
		switch (t)
		{
		case GameType.SP_TUTORIAL:
			return "Tutorial";
		case GameType.SP_QUICKMATCH:
			return "Skirmish";
		case GameType.SP_TOWERMATCH:
			return "Trial";
		case GameType.SP_ADVENTUREMATCH:
			return "Adventure Match";
		case GameType.MP_LIMITED:
			return "Judgement";
		case GameType.MP_RANKED:
			return "Ranked Match";
		case GameType.MP_UNRANKED:
			return "Challenge Match";
		case GameType.MP_QUICKMATCH:
			return "Quick Match";
		case GameType.SP_LABMATCH:
			return "Custom AI Match";
		default:
			return string.Empty;
		}
	}
}
