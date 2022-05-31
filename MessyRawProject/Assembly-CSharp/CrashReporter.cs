using System;
using System.Text;
using UnityEngine;

// Token: 0x02000162 RID: 354
public class CrashReporter
{
	// Token: 0x06000B0A RID: 2826 RVA: 0x0005105C File Offset: 0x0004F25C
	private static void appendGetSurroundings(StringBuilder sb, string s, int p, string id, int before, int after)
	{
		if (p < 0)
		{
			return;
		}
		int num = Math.Max(p - before, 0);
		int num2 = Math.Min(p + after, s.Length);
		sb.Append(string.Concat(new object[]
		{
			"\n### ",
			id,
			" ### CharIndex: ",
			num,
			" >>>\n"
		}));
		sb.Append(s.Substring(num, num2 - num));
		sb.Append(string.Concat(new object[]
		{
			"\n### ",
			id,
			" ### CharIndex: ",
			num2,
			" <<<\n"
		}));
	}

	// Token: 0x06000B0B RID: 2827 RVA: 0x0005110C File Offset: 0x0004F30C
	public static string partitionLog(string log, string[] aroundWords)
	{
		string text = log.ToLower();
		StringBuilder stringBuilder = new StringBuilder();
		CrashReporter.appendGetSurroundings(stringBuilder, log, log.Length, "End of Log (30 kB)", 30000, 0);
		foreach (string text2 in aroundWords)
		{
			int num = text.IndexOf(text2.ToLower());
			if (num >= 0)
			{
				CrashReporter.appendGetSurroundings(stringBuilder, log, num, text2, 8000, 10000);
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06000B0C RID: 2828 RVA: 0x00051194 File Offset: 0x0004F394
	public static ReportBugMessage sendBugReport(string userMessage)
	{
		string systemInfo = string.Concat(new object[]
		{
			"DEBUG DATA\n Unity Player Version: ",
			Application.unityVersion,
			"\n Graphics ",
			SystemInfo.graphicsDeviceName,
			"\n Graphics Memory Size ",
			SystemInfo.graphicsMemorySize,
			"\n Graphics Device Version ",
			SystemInfo.graphicsDeviceVendor,
			"\n Graphics Device Vendor ",
			SystemInfo.graphicsDeviceVendor,
			"\n Support Shadows ",
			SystemInfo.supportsShadows,
			"\n Support Image Effects ",
			SystemInfo.supportsImageEffects,
			"\n Support Render Textures ",
			SystemInfo.supportsRenderTextures
		});
		string filename = Application.persistentDataPath + "/output_log.txt";
		string text = FileUtil.readFileContents(filename);
		if (text == null)
		{
			text = "null";
		}
		text = CrashReporter.partitionLog(text, new string[]
		{
			"error",
			"exception"
		});
		return new ReportBugMessage(userMessage, systemInfo, text);
	}

	// Token: 0x06000B0D RID: 2829 RVA: 0x00051294 File Offset: 0x0004F494
	public static ReportCrashMessage sendCrashReport()
	{
		string data = string.Concat(new object[]
		{
			"DEBUG DATA\n Unity Player Version: ",
			Application.unityVersion,
			"\n Graphics ",
			SystemInfo.graphicsDeviceName,
			"\n Graphics Memory Size ",
			SystemInfo.graphicsMemorySize,
			"\n Graphics Device Version ",
			SystemInfo.graphicsDeviceVendor,
			"\n Graphics Device Vendor ",
			SystemInfo.graphicsDeviceVendor,
			"\n Support Shadows ",
			SystemInfo.supportsShadows,
			"\n Support Image Effects ",
			SystemInfo.supportsImageEffects,
			"\n Support Render Textures ",
			SystemInfo.supportsRenderTextures
		});
		return new ReportCrashMessage(data);
	}
}
