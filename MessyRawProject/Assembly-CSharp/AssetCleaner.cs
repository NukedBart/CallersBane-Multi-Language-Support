using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000021 RID: 33
public class AssetCleaner
{
	// Token: 0x060001DC RID: 476 RVA: 0x00003822 File Offset: 0x00001A22
	public AssetCleaner(List<CardType> validCardTypes)
	{
		if (validCardTypes != null && validCardTypes.Count > 0)
		{
			this.q = new CardTypeAssetQuerier(validCardTypes);
		}
	}

	// Token: 0x060001DD RID: 477 RVA: 0x00003848 File Offset: 0x00001A48
	public void cleanAll()
	{
		this.cleanType(AssetType.CardImage);
		this.cleanType(AssetType.AnimationBundle);
		this.cleanType(AssetType.AnimationPreview);
	}

	// Token: 0x060001DE RID: 478 RVA: 0x000204CC File Offset: 0x0001E6CC
	public IEnumerator cleanAllCoroutine()
	{
		yield return new WaitForSeconds(1f);
		this.cleanType(AssetType.CardImage);
		yield return null;
		this.cleanType(AssetType.AnimationBundle);
		yield return null;
		this.cleanType(AssetType.AnimationBundle);
		yield break;
	}

	// Token: 0x060001DF RID: 479 RVA: 0x000204E8 File Offset: 0x0001E6E8
	private void cleanType(AssetType type)
	{
		if (type == AssetType.CardImage)
		{
			this.cleanLocalFolder(type, "cardImages/", false);
		}
		if (type == AssetType.AnimationBundle)
		{
			this.cleanLocalFolder(type, "anim/", false);
			this.cleanLocalFolder(type, "bundles/", true);
		}
		if (type == AssetType.AnimationPreview)
		{
			this.cleanLocalFolder(type, "img/", false);
		}
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x00020540 File Offset: 0x0001E740
	private void cleanLocalFolder(AssetType assetType, string path, bool folders)
	{
		if (this.q == null)
		{
			return;
		}
		string localPathFor = StorageEnvironment.getLocalPathFor(path);
		if (!Directory.Exists(localPathFor))
		{
			return;
		}
		string[] array = (!folders) ? Directory.GetFiles(localPathFor) : Directory.GetDirectories(localPathFor);
		foreach (string text in array)
		{
			int idFromFilename = AssetCleaner.getIdFromFilename(text);
			if (idFromFilename < 0)
			{
				Log.info(string.Concat(new object[]
				{
					"Non-standard filename (type ",
					assetType,
					") : ",
					text
				}));
			}
			else if (!this.q.hasType(assetType, idFromFilename))
			{
				string[] array3 = this.getAdditionalFilenames(text, assetType) ?? new string[0];
				foreach (string text2 in array3)
				{
					try
					{
						File.Delete(text2);
					}
					catch (Exception)
					{
					}
				}
				Log.warning("Removing old graphics (type " + assetType.ToString() + ") : " + text);
				if (folders)
				{
					if (assetType == AssetType.AnimationBundle)
					{
						try
						{
							Directory.Delete(text);
						}
						catch (Exception)
						{
						}
					}
				}
				else
				{
					try
					{
						File.Delete(text);
					}
					catch (Exception)
					{
					}
				}
			}
		}
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x000206C8 File Offset: 0x0001E8C8
	private string[] getAdditionalFilenames(string idPath, AssetType assetType)
	{
		if (assetType == AssetType.AnimationBundle)
		{
			return new string[]
			{
				Path.Combine(idPath, "anims.data"),
				Path.Combine(idPath, "sprites.png"),
				Path.Combine(idPath, "sprites.pos"),
				Path.Combine(idPath, "anims.bytes"),
				Path.Combine(idPath, "spritespos.bytes")
			};
		}
		return null;
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x0002072C File Offset: 0x0001E92C
	private static int getIdFromFilename(string fn)
	{
		int num = fn.LastIndexOf("/");
		int num2 = fn.LastIndexOf(".");
		if (num < 0)
		{
			return -1;
		}
		if (num2 < 0 || num2 < num)
		{
			num2 = fn.Length;
		}
		string text = fn.Substring(num + 1, num2 - num - 1);
		int result;
		if (int.TryParse(text, ref result))
		{
			return result;
		}
		return -1;
	}

	// Token: 0x040000E5 RID: 229
	private CardTypeAssetQuerier q;
}
