using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000175 RID: 373
public class CommandLine : MonoBehaviour
{
	// Token: 0x06000B80 RID: 2944 RVA: 0x000526F4 File Offset: 0x000508F4
	private void handleArgument(string argName, string argString)
	{
		if (argName != null)
		{
			if (CommandLine.<>f__switch$map14 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(7);
				dictionary.Add("--assetsDir", 0);
				dictionary.Add("--assetIndex", 1);
				dictionary.Add("--username", 2);
				dictionary.Add("--uuid", 3);
				dictionary.Add("--useruuid", 4);
				dictionary.Add("--userProperties", 5);
				dictionary.Add("--accessToken", 6);
				CommandLine.<>f__switch$map14 = dictionary;
			}
			int num;
			if (CommandLine.<>f__switch$map14.TryGetValue(argName, ref num))
			{
				switch (num)
				{
				case 0:
					ResourceAbstraction.assetsDirectory = argString;
					Log.info("Assets directory set to: " + ResourceAbstraction.assetsDirectory);
					break;
				case 1:
					ResourceAbstraction.assetIndexPath = argString;
					Log.info("Asset index path set to: " + ResourceAbstraction.assetIndexPath);
					break;
				case 2:
					this.username = argString;
					break;
				case 3:
					this.uuid = argString;
					break;
				case 4:
					this.useruuid = argString;
					break;
				case 6:
					this.accessToken = argString;
					break;
				}
			}
		}
	}

	// Token: 0x06000B81 RID: 2945 RVA: 0x00052820 File Offset: 0x00050A20
	private void handleArgument(string argName)
	{
		if (argName != null)
		{
			if (CommandLine.<>f__switch$map15 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
				dictionary.Add("-popupwindow", 0);
				CommandLine.<>f__switch$map15 = dictionary;
			}
			int num;
			if (CommandLine.<>f__switch$map15.TryGetValue(argName, ref num))
			{
				if (num == 0)
				{
					App.IsBorderlessWindow = true;
				}
			}
		}
	}

	// Token: 0x06000B82 RID: 2946 RVA: 0x00052880 File Offset: 0x00050A80
	private void Awake()
	{
		if (this.commandLineEnabled && !CommandLine.commandLineRead)
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				foreach (string text in this.arguments)
				{
					if (commandLineArgs[i].Equals(text))
					{
						if (text.StartsWith("--"))
						{
							if (i + 1 < commandLineArgs.Length)
							{
								Log.info("Argument [" + commandLineArgs[i] + "] set to: " + commandLineArgs[i + 1]);
								this.handleArgument(text, commandLineArgs[i + 1]);
							}
							else
							{
								Log.error("Missing " + text + " parameter.");
							}
						}
						else if (text.StartsWith("-"))
						{
							Log.info("Argument [" + text + "] set.");
							this.handleArgument(text);
						}
					}
				}
			}
			if (this.username == null || this.uuid == null || this.accessToken == null || this.useruuid == null)
			{
				Log.info("Missing username, uuid, useruuid or accessToken - skipping automatic sign-in.");
			}
			else
			{
				MiniCommunicator.accessToken = new AuthResponse(this.accessToken, this.useruuid, this.uuid, this.username);
				App.StartedWithLauncher = true;
			}
		}
		CommandLine.commandLineRead = true;
		if (Application.isEditor && App.Debug_UseDefaultAssetPaths())
		{
			ResourceAbstraction.assetsDirectory = Application.persistentDataPath + "/objects";
			ResourceAbstraction.assetIndexPath = Application.persistentDataPath + "/scrolls.json";
			Log.info("Editor - using default assets directory: " + ResourceAbstraction.assetsDirectory);
		}
	}

	// Token: 0x040008C6 RID: 2246
	private bool commandLineEnabled = true;

	// Token: 0x040008C7 RID: 2247
	private string[] arguments = new string[]
	{
		"--assetsDir",
		"--assetIndex",
		"--username",
		"--uuid",
		"--userProperties",
		"--accessToken",
		"-popupwindow",
		"--useruuid"
	};

	// Token: 0x040008C8 RID: 2248
	private string accessToken;

	// Token: 0x040008C9 RID: 2249
	private string uuid;

	// Token: 0x040008CA RID: 2250
	private string username;

	// Token: 0x040008CB RID: 2251
	private string useruuid;

	// Token: 0x040008CC RID: 2252
	private static bool commandLineRead;
}
