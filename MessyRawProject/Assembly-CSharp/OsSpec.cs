using System;
using System.IO;
using UnityEngine;

// Token: 0x0200037B RID: 891
public class OsSpec
{
	// Token: 0x060013DE RID: 5086 RVA: 0x0000EB67 File Offset: 0x0000CD67
	public static string getDownloadDataPath()
	{
		return Application.persistentDataPath + "/download-data/";
	}

	// Token: 0x060013DF RID: 5087 RVA: 0x000790A8 File Offset: 0x000772A8
	public static string getApplicationPath()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath);
		directoryInfo = directoryInfo.Parent;
		string result = directoryInfo.ToString();
		if (OsSpec.getOS() == OSType.OSX)
		{
			directoryInfo = directoryInfo.Parent;
			result = directoryInfo.ToString();
		}
		return result;
	}

	// Token: 0x060013E0 RID: 5088 RVA: 0x000790E8 File Offset: 0x000772E8
	public static string getInstalledPath()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath);
		directoryInfo = directoryInfo.Parent.Parent;
		string result = directoryInfo.ToString();
		if (OsSpec.getOS() == OSType.OSX)
		{
			directoryInfo = directoryInfo.Parent;
			result = directoryInfo.ToString();
		}
		return result;
	}

	// Token: 0x060013E1 RID: 5089 RVA: 0x0000EB78 File Offset: 0x0000CD78
	public static string getOSSuffix()
	{
		return OsSpec.getOSSuffix(OsSpec.getOS());
	}

	// Token: 0x060013E2 RID: 5090 RVA: 0x00079130 File Offset: 0x00077330
	public static string getOSSuffix(OSType os)
	{
		if (os == OSType.Windows)
		{
			return "Win";
		}
		if (os == OSType.OSX)
		{
			return "OSX";
		}
		if (os == OSType.Android)
		{
			return "Android";
		}
		if (os == OSType.iOS)
		{
			return "iOS";
		}
		if (os == OSType.Linux)
		{
			return "Linux";
		}
		return "unknown";
	}

	// Token: 0x060013E3 RID: 5091 RVA: 0x00079184 File Offset: 0x00077384
	public static OSType getOS()
	{
		string text = Application.platform.ToString().ToLower();
		if (text.Contains("windows"))
		{
			return OSType.Windows;
		}
		if (text.Contains("osx"))
		{
			return OSType.OSX;
		}
		if (text.Contains("android"))
		{
			return OSType.Android;
		}
		if (text.Contains("ios"))
		{
			return OSType.iOS;
		}
		if (text.Contains("linux"))
		{
			return OSType.Linux;
		}
		return OSType.Unknown;
	}

	// Token: 0x060013E4 RID: 5092 RVA: 0x0000EB84 File Offset: 0x0000CD84
	public static bool isMobile()
	{
		return OsSpec.isAndroid() || OsSpec.isIos();
	}

	// Token: 0x060013E5 RID: 5093 RVA: 0x0000EB98 File Offset: 0x0000CD98
	public static bool isAndroid()
	{
		return OsSpec.getOS() == OSType.Android;
	}

	// Token: 0x060013E6 RID: 5094 RVA: 0x0000EBA2 File Offset: 0x0000CDA2
	public static bool isIos()
	{
		return OsSpec.getOS() == OSType.iOS;
	}
}
