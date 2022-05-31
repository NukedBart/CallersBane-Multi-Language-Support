using System;
using UnityEngine;

// Token: 0x0200040C RID: 1036
public class TextToTexture
{
	// Token: 0x060016F1 RID: 5873 RVA: 0x0008EC50 File Offset: 0x0008CE50
	public TextToTexture(Font customFont, int fontCountX, int fontCountY, PerCharacterKerning[] perCharacterKerning, bool supportSpecialCharacters)
	{
		this.customFont = customFont;
		this.fontTexture = (Texture2D)customFont.material.mainTexture;
		this.fontCountX = fontCountX;
		this.fontCountY = fontCountY;
		this.kerningValues = this.GetCharacterKerningValuesFromPerCharacterKerning(perCharacterKerning);
		this.supportSpecialCharacters = supportSpecialCharacters;
	}

	// Token: 0x060016F2 RID: 5874 RVA: 0x0008ECA4 File Offset: 0x0008CEA4
	public Texture2D CreateTextToTexture(string text, int textPlacementX, int textPlacementY, int textureSize, float characterSize, float lineSpacing)
	{
		Texture2D result = this.CreatefillTexture2D(Color.clear, textureSize, textureSize);
		int num = this.fontTexture.width / this.fontCountX;
		int num2 = this.fontTexture.height / this.fontCountY;
		int num3 = (int)((float)num * characterSize);
		int num4 = (int)((float)num2 * characterSize);
		float num5 = (float)textPlacementX;
		float num6 = (float)textPlacementY;
		for (int i = 0; i < text.Length; i++)
		{
			char c = text.get_Chars(i);
			bool flag = false;
			if (c.Equals('\\'))
			{
				Log.info("THIS IS IT!");
			}
			if (c == '\\' && this.supportSpecialCharacters)
			{
				Log.info("spec char");
				flag = true;
				if (i + 1 < text.Length)
				{
					i++;
					c = text.get_Chars(i);
					if (c == 'n' || c == 'r')
					{
						num6 -= (float)num4 * lineSpacing;
						num5 = (float)textPlacementX;
					}
					else if (c == 't')
					{
						num5 += (float)num3 * this.GetKerningValue(' ') * 5f;
					}
					else if (c == '\\')
					{
						flag = false;
					}
				}
			}
			if (!flag && this.customFont.HasCharacter(c))
			{
				Vector2 characterGridPosition = this.GetCharacterGridPosition(c);
				characterGridPosition.x *= (float)num;
				characterGridPosition.y *= (float)num2;
				Color[] pixels = this.fontTexture.GetPixels((int)characterGridPosition.x, this.fontTexture.height - (int)characterGridPosition.y - num2, num, num2);
				float kerningValue = this.GetKerningValue(c);
				num5 += (float)num3 * kerningValue;
			}
			else if (!flag)
			{
				Log.info("Letter Not Found:" + c);
			}
		}
		return result;
	}

	// Token: 0x060016F3 RID: 5875 RVA: 0x0008EE80 File Offset: 0x0008D080
	public int CalcTextWidthPlusTrailingBuffer(string text, int decalTextureSize, float characterSize)
	{
		float num = 0f;
		int num2 = (int)((float)(this.fontTexture.width / this.fontCountX) * characterSize);
		for (int i = 0; i < text.Length; i++)
		{
			char c = text.get_Chars(i);
			if (i < text.Length - 1)
			{
				num += (float)num2 * this.GetKerningValue(c);
			}
			else
			{
				num += (float)num2;
			}
		}
		return (int)num;
	}

	// Token: 0x060016F4 RID: 5876 RVA: 0x0008EEF0 File Offset: 0x0008D0F0
	private Color[] changeDimensions(Color[] originalColors, int originalWidth, int originalHeight, int newWidth, int newHeight)
	{
		Color[] array;
		if (originalWidth == newWidth && originalHeight == newHeight)
		{
			array = originalColors;
		}
		else
		{
			array = new Color[newWidth * newHeight];
			Texture2D texture2D = new Texture2D(originalWidth, originalHeight);
			texture2D.SetPixels(originalColors);
			for (int i = 0; i < newHeight; i++)
			{
				for (int j = 0; j < newWidth; j++)
				{
					int num = j + i * newWidth;
					float num2 = (float)j / (float)newWidth;
					float num3 = (float)i / (float)newHeight;
					array[num] = texture2D.GetPixelBilinear(num2, num3);
				}
			}
		}
		return array;
	}

	// Token: 0x060016F5 RID: 5877 RVA: 0x0008EF8C File Offset: 0x0008D18C
	private Texture2D AddPixelsToTextureIfClear(Texture2D texture, Color[] newPixels, int placementX, int placementY, int placementWidth, int placementHeight)
	{
		if (placementX + placementWidth < texture.width)
		{
			Color[] pixels = texture.GetPixels(placementX, placementY, placementWidth, placementHeight);
			for (int i = 0; i < placementHeight; i++)
			{
				for (int j = 0; j < placementWidth; j++)
				{
					int num = j + i * placementWidth;
					if (pixels[num] != Color.clear)
					{
						newPixels[num] = pixels[num];
					}
				}
			}
		}
		else
		{
			Log.info("Letter Falls Outside Bounds of Texture" + (placementX + placementWidth));
		}
		return texture;
	}

	// Token: 0x060016F6 RID: 5878 RVA: 0x0008F038 File Offset: 0x0008D238
	private Vector2 GetCharacterGridPosition(char c)
	{
		int num = (int)(c - ' ');
		return new Vector2((float)(num % this.fontCountX), (float)(num / this.fontCountX));
	}

	// Token: 0x060016F7 RID: 5879 RVA: 0x0008F064 File Offset: 0x0008D264
	private float GetKerningValue(char c)
	{
		try
		{
			if (c == '\n')
			{
				return 0.4f;
			}
			return this.kerningValues[(int)(c - ' ')];
		}
		catch (Exception ex)
		{
			Log.info("ERROR: " + ex);
		}
		return 1f;
	}

	// Token: 0x060016F8 RID: 5880 RVA: 0x0008F0C8 File Offset: 0x0008D2C8
	private Texture2D CreatefillTexture2D(Color color, int textureWidth, int textureHeight)
	{
		Texture2D texture2D = new Texture2D(textureWidth, textureHeight);
		int num = texture2D.width * texture2D.height;
		Color[] array = new Color[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = color;
		}
		texture2D.SetPixels(array);
		return texture2D;
	}

	// Token: 0x060016F9 RID: 5881 RVA: 0x0008F11C File Offset: 0x0008D31C
	private float[] GetCharacterKerningValuesFromPerCharacterKerning(PerCharacterKerning[] perCharacterKerning)
	{
		float[] array = new float[96];
		foreach (PerCharacterKerning perCharacterKerning2 in perCharacterKerning)
		{
			if (perCharacterKerning2.First != string.Empty)
			{
				int @char = (int)perCharacterKerning2.GetChar();
				if (@char >= 0 && @char - 32 < array.Length)
				{
					array[@char - 32] = perCharacterKerning2.GetKerningValue();
				}
			}
		}
		return array;
	}

	// Token: 0x0400146F RID: 5231
	private const int ASCII_START_OFFSET = 32;

	// Token: 0x04001470 RID: 5232
	private Font customFont;

	// Token: 0x04001471 RID: 5233
	private Texture2D fontTexture;

	// Token: 0x04001472 RID: 5234
	private int fontCountX;

	// Token: 0x04001473 RID: 5235
	private int fontCountY;

	// Token: 0x04001474 RID: 5236
	private float[] kerningValues;

	// Token: 0x04001475 RID: 5237
	private bool supportSpecialCharacters;
}
