using System;
using System.Globalization;
using System.IO;
using Irrelevant.Assets;
using UnityEngine;

// Token: 0x020000CC RID: 204
public class PreviewImage
{
	// Token: 0x06000700 RID: 1792 RVA: 0x0003F28C File Offset: 0x0003D48C
	private static Texture2D loadPreview(CardType ct, out bool isRealUnitArt)
	{
		string id = "preview_" + ct.id;
		Texture2D texture2D = ResourceManager.instance.tryGetTexture2D(id);
		if (texture2D == null)
		{
			texture2D = new Texture2D(8, 8, 3, false);
			string localPathFor = StorageEnvironment.getLocalPathFor(StorageEnvironment.getPreviewPath(ct));
			if (!File.Exists(localPathFor))
			{
				isRealUnitArt = false;
				return PreviewImage.loadCardImageAsPreview(ct);
			}
			byte[] array = File.ReadAllBytes(localPathFor);
			if (!texture2D.LoadImage(array))
			{
				isRealUnitArt = false;
				return PreviewImage.loadCardImageAsPreview(ct);
			}
			texture2D.wrapMode = 1;
			ResourceManager.instance.assignTexture2D(id, texture2D);
		}
		isRealUnitArt = true;
		return texture2D;
	}

	// Token: 0x06000701 RID: 1793 RVA: 0x0003F328 File Offset: 0x0003D528
	private static Texture2D loadCardImageAsPreview(CardType ct)
	{
		Texture2D texture2D = App.AssetLoader.LoadCardImage(ct.cardImage.ToString());
		texture2D.wrapMode = 1;
		return texture2D;
	}

	// Token: 0x06000702 RID: 1794 RVA: 0x0003F354 File Offset: 0x0003D554
	public static UnitAnimDescription createPreviewAnimation(CardType ct)
	{
		bool flag;
		Texture2D tex = PreviewImage.loadPreview(ct, out flag);
		ScalePosModifier scalePosModifier = PreviewImage.getPreviewPositionFor(ct);
		if (!flag)
		{
			scalePosModifier = new ScalePosModifier(scalePosModifier.scale, -300f, 0f);
		}
		return UnitAnimDescriptionFactory.fromImageAndRect(tex, scalePosModifier.offset.x, scalePosModifier.offset.y, scalePosModifier.scale);
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x0003F3B0 File Offset: 0x0003D5B0
	private static ScalePosModifier getPreviewPositionFor(CardType ct)
	{
		if (ct.animationPreviewInfo == null)
		{
			return new ScalePosModifier(4f, -200f, 260f);
		}
		string[] array = ct.animationPreviewInfo.Split(new char[]
		{
			','
		});
		float num = 1f / float.Parse(array[2], CultureInfo.InvariantCulture);
		float x = -num * float.Parse(array[0], CultureInfo.InvariantCulture);
		float y = -num * float.Parse(array[1], CultureInfo.InvariantCulture);
		return new ScalePosModifier(num, x, y);
	}
}
