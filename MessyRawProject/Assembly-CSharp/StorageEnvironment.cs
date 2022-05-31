using System;
using UnityEngine;

// Token: 0x0200037D RID: 893
public class StorageEnvironment
{
	// Token: 0x060013E8 RID: 5096 RVA: 0x0000EBAC File Offset: 0x0000CDAC
	public static string getLocalPathFor(string s)
	{
		return OsSpec.getDownloadDataPath() + s;
	}

	// Token: 0x060013E9 RID: 5097 RVA: 0x0000EBB9 File Offset: 0x0000CDB9
	public static string getRemotePathFor(string s)
	{
		return App.Communicator.GetCardDownloadURL() + s;
	}

	// Token: 0x060013EA RID: 5098 RVA: 0x0000EBCB File Offset: 0x0000CDCB
	public static string getCardImagePath(CardType t)
	{
		return StorageEnvironment.getCardImagePath(t.cardImage);
	}

	// Token: 0x060013EB RID: 5099 RVA: 0x0000EBD8 File Offset: 0x0000CDD8
	public static string getCardImagePath(int id)
	{
		return "cardImages/" + id;
	}

	// Token: 0x060013EC RID: 5100 RVA: 0x0000EBEA File Offset: 0x0000CDEA
	public static string getAnimationBundleZipPath(CardType t)
	{
		return StorageEnvironment.getAnimationBundleZipPath(t.animationBundle);
	}

	// Token: 0x060013ED RID: 5101 RVA: 0x0000EBF7 File Offset: 0x0000CDF7
	public static string getAnimationBundleZipPath(int id)
	{
		return "anim/" + id;
	}

	// Token: 0x060013EE RID: 5102 RVA: 0x0000EC09 File Offset: 0x0000CE09
	public static string getAnimationBundleUnzipPath(CardType t)
	{
		return StorageEnvironment.getAnimationBundleUnzipPath(t.animationBundle);
	}

	// Token: 0x060013EF RID: 5103 RVA: 0x0000EC16 File Offset: 0x0000CE16
	public static string getAnimationBundleUnzipPath(int id)
	{
		return "bundles/" + id;
	}

	// Token: 0x060013F0 RID: 5104 RVA: 0x0000EC28 File Offset: 0x0000CE28
	public static string getPreviewPath(CardType t)
	{
		return StorageEnvironment.getPreviewPath(t.animationPreviewImage);
	}

	// Token: 0x060013F1 RID: 5105 RVA: 0x0000EC35 File Offset: 0x0000CE35
	public static string getPreviewPath(int id)
	{
		return "img/" + id;
	}

	// Token: 0x060013F2 RID: 5106 RVA: 0x0000EC47 File Offset: 0x0000CE47
	public static string getAddonFolder()
	{
		return Application.persistentDataPath + "/addon/";
	}

	// Token: 0x060013F3 RID: 5107 RVA: 0x0000EC58 File Offset: 0x0000CE58
	public static string getAddonPath(string p)
	{
		return StorageEnvironment.getAddonFolder() + p;
	}

	// Token: 0x04001123 RID: 4387
	public const string CardImagePath = "cardImages/";

	// Token: 0x04001124 RID: 4388
	public const string AnimationPreviewPath = "img/";

	// Token: 0x04001125 RID: 4389
	public const string AnimationBundleZipPath = "anim/";

	// Token: 0x04001126 RID: 4390
	public const string AnimationBundleUnzipPath = "bundles/";
}
