using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000082 RID: 130
public class GameReplayer : AbstractCommListener
{
	// Token: 0x060004F1 RID: 1265 RVA: 0x00035F04 File Offset: 0x00034104
	private void Start()
	{
		App.AssetLoader.Init();
		this.key = FileUtil.readFileContents("replaykey.txt");
		this.comm = App.Communicator;
		this.comm.addListener(this);
		this.gameComm = base.gameObject.AddComponent<NoAuthComm>();
		this.gameComm.addListener(this);
		base.StartCoroutine(this.lobbyConnect(0.2f));
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x00035F74 File Offset: 0x00034174
	private IEnumerator lobbyConnect(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		this.comm.storeUserCredentials("demo-1", "demo-1");
		this.comm.sendLogin();
		this.guiLocks--;
		yield break;
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x00035FA0 File Offset: 0x000341A0
	private void gameConnect()
	{
		this.info = this.parseInfo(this.input);
		if (this.info != null)
		{
			this.gameComm.connect(new IpPort(this.info.serverAddress, this.info.serverPort));
		}
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x000052C7 File Offset: 0x000034C7
	public override void onConnect(OnConnectData data)
	{
		if (data.comm == this.gameComm)
		{
			this.requestGame(this.info, this.info.color);
		}
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x00035FF4 File Offset: 0x000341F4
	private void OnGUI()
	{
		GUI.enabled = (this.guiLocks == 0);
		GUI.Label(new Rect(0f, 50f, 500f, 50f), "gameId[-color]@host[:port]\nexample: 12345@127.0.0.1 or\n12345-black@127.0.0.1:9999");
		this.input = GUI.TextField(new Rect(0f, 0f, 300f, 50f), this.input);
		if (GUI.Button(new Rect(0f, 100f, 300f, 50f), "Run"))
		{
			string text = FileUtil.readFileContents(GameReplayer.fullFilename(this.input));
			if (text != null)
			{
				this.setData(text);
			}
			else
			{
				this.gameConnect();
				this.ready = true;
			}
		}
		GUI.enabled = true;
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x000052F7 File Offset: 0x000034F7
	private void requestGame(GameReplayer.GameInfo nfo, string color)
	{
		this.gameComm.send(new GetGameLogMessage((long)nfo.gameId, color, this.key));
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x000360BC File Offset: 0x000342BC
	private GameReplayer.GameInfo parseInfo(string input)
	{
		GameReplayer.GameInfo gameInfo = new GameReplayer.GameInfo();
		int num = input.IndexOf("@");
		if (num > 0)
		{
			string text = input.Substring(num + 1);
			int num2 = input.IndexOf(":", num + 1);
			if (num2 < 0)
			{
				gameInfo.serverPort = 8081;
				gameInfo.serverAddress = input.Substring(num + 1);
			}
			else
			{
				string text2 = StringUtil.keepCharacters(input.Substring(num2 + 1), new Predicate<char>(char.IsDigit));
				int.TryParse(text2, ref gameInfo.serverPort);
				gameInfo.serverAddress = input.Substring(num + 1, num2 - num - 1);
			}
			int num3 = input.IndexOf("-");
			if (num3 > 0 && num3 < num - 1)
			{
				gameInfo.color = input.Substring(num3 + 1, num - num3 - 1).ToLower();
			}
			else
			{
				num3 = num;
			}
			string text3 = StringUtil.keepCharacters(input.Substring(0, num3), new Predicate<char>(char.IsDigit));
			int.TryParse(text3, ref gameInfo.gameId);
			return gameInfo;
		}
		return null;
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x00005318 File Offset: 0x00003518
	private static string fullFilename(string fn)
	{
		return "c:/tmp/gamelog/" + fn.Replace(':', '_');
	}

	// Token: 0x060004F9 RID: 1273 RVA: 0x000361D0 File Offset: 0x000343D0
	public override void handleMessage(Message msg)
	{
		if (msg is CardTypesMessage)
		{
			this.guiLocks--;
		}
		if (msg is GetGameLogMessage)
		{
			GetGameLogMessage getGameLogMessage = (GetGameLogMessage)msg;
			string fn = (!string.IsNullOrEmpty(this.input)) ? this.input : "tmp";
			try
			{
				FileUtil.writeFileContents(GameReplayer.fullFilename(fn), getGameLogMessage.log);
			}
			catch (Exception)
			{
			}
			this.setData(getGameLogMessage.log);
		}
		if (msg is FailMessage)
		{
			if (((FailMessage)msg).isType(typeof(GetGameLogMessage)) && !this.triedAllColors)
			{
				this.requestGame(this.info, (!(this.info.color == "black")) ? "black" : "white");
			}
			this.triedAllColors = true;
		}
	}

	// Token: 0x060004FA RID: 1274 RVA: 0x000362D0 File Offset: 0x000344D0
	private void setData(string data)
	{
		this.comm.setData(data);
		this.comm.setEnabled(false, false);
		App.SceneValues.battleMode.gameMode = GameMode.Replay;
		App.Communicator.setEnabled(false, false);
		SceneLoader.loadScene("_BattleModeView");
	}

	// Token: 0x04000365 RID: 869
	private Communicator comm;

	// Token: 0x04000366 RID: 870
	private MiniCommunicator gameComm;

	// Token: 0x04000367 RID: 871
	private int guiLocks = 2;

	// Token: 0x04000368 RID: 872
	private string input = string.Empty;

	// Token: 0x04000369 RID: 873
	private string key = string.Empty;

	// Token: 0x0400036A RID: 874
	private GameReplayer.GameInfo info = new GameReplayer.GameInfo();

	// Token: 0x0400036B RID: 875
	private bool ready;

	// Token: 0x0400036C RID: 876
	private bool triedAllColors;

	// Token: 0x02000083 RID: 131
	private class GameInfo
	{
		// Token: 0x060004FC RID: 1276 RVA: 0x00036320 File Offset: 0x00034520
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"addr:",
				this.serverAddress,
				", port:",
				this.serverPort,
				", game:",
				this.gameId,
				", color:",
				this.color
			});
		}

		// Token: 0x0400036D RID: 877
		internal int gameId;

		// Token: 0x0400036E RID: 878
		internal int serverPort;

		// Token: 0x0400036F RID: 879
		internal string serverAddress;

		// Token: 0x04000370 RID: 880
		internal string color = "black";
	}
}
