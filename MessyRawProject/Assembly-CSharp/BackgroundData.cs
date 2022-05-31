using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000040 RID: 64
public class BackgroundData
{
	// Token: 0x06000291 RID: 657 RVA: 0x00022C04 File Offset: 0x00020E04
	public BackgroundData(string name)
	{
		this.name = name;
		this.idName = BackgroundData.nameToIdName(name);
	}

	// Token: 0x06000292 RID: 658 RVA: 0x00022C6C File Offset: 0x00020E6C
	static BackgroundData()
	{
		BackgroundData.bgs.Add(new BackgroundData("Grasslands").addBackground("BattleMode/Backgrounds/Grasslands1/background").setShadowColor(ColorUtil.FromHex24(1851148u)).setGradientColor(ColorUtil.FromInts(21, 30, 0)));
		BackgroundData.bgs.Add(new BackgroundData("Cave").addBackground("BattleMode/Backgrounds/Cave1/background").setShadowColor(ColorUtil.FromHex24(4006939u)).setGradientColor(ColorUtil.FromInts(30, 14, 18)).addLight(5.44f, -2.86f, Color.yellow, 10).addLight(-5.84f, -2.86f, Color.yellow, 10));
		BackgroundData.bgs.Add(new BackgroundData("Desert").addBackground("BattleMode/Backgrounds/Desert1/background").setShadowColor(ColorUtil.FromHex24(9523010u)).setGradientColor(ColorUtil.FromInts(76, 28, 20)));
		BackgroundData.bgs.Add(new BackgroundData("Castle").addBackground("BattleMode/Backgrounds/Castle1/background").setShadowColor(ColorUtil.FromHex24(7224104u)).setGradientColor(ColorUtil.FromInts(67, 32, 4)));
		BackgroundData.bgs.Add(new BackgroundData("Swamp").addBackground("BattleMode/Backgrounds/Swamp1/background").setShadowColor(ColorUtil.FromHex24(4542812u)).setGradientColor(ColorUtil.FromInts(2, 3, 4)).addLight(4.08f, -2.93f, Color.yellow, 3).addLight(-3.66f, -3.46f, Color.yellow, 3));
		BackgroundData.bgs.Add(new BackgroundData("Deep Forest").addBackground("BattleMode/Backgrounds/DeepForest/background").setShadowColor(ColorUtil.FromHex24(1851148u)).setGradientColor(ColorUtil.FromInts(7, 31, 43)));
		BackgroundData.bgs.Add(new BackgroundData("Grassy Mountain").addBackground("BattleMode/Backgrounds/GrassyMountain/background").setShadowColor(ColorUtil.FromHex24(2631687u)).setGradientColor(ColorUtil.FromInts(1, 33, 49)));
		BackgroundData.bgs.Add(new BackgroundData("Green Meadow").addBackground("BattleMode/Backgrounds/GreenMeadow/background").setShadowColor(ColorUtil.FromHex24(996105u)).setGradientColor(ColorUtil.FromInts(1, 20, 30)));
		BackgroundData.bgs.Add(new BackgroundData("Ice Cave").addBackground("BattleMode/Backgrounds/IceCave/background").setShadowColor(ColorUtil.FromHex24(1388087u)).setGradientColor(ColorUtil.FromInts(6, 21, 54)));
		BackgroundData.bgs.Add(new BackgroundData("Lava Grotto").addBackground("BattleMode/Backgrounds/LavaGrotto/background").setShadowColor(ColorUtil.FromHex24(2500134u)).setGradientColor(ColorUtil.FromInts(9, 9, 8)));
		BackgroundData.bgs.Add(new BackgroundData("Snowy Mountain").addBackground("BattleMode/Backgrounds/SnowyMountain/background").setShadowColor(ColorUtil.FromHex24(5461587u)).setGradientColor(ColorUtil.FromInts(34, 37, 54)));
		BackgroundData.bgs.Add(new BackgroundData("Yellow Meadow").addBackground("BattleMode/Backgrounds/YellowMeadow/background").setShadowColor(ColorUtil.FromHex24(5525011u)).setGradientColor(ColorUtil.FromInts(53, 31, 24)));
	}

	// Token: 0x06000293 RID: 659 RVA: 0x00003E5B File Offset: 0x0000205B
	public static string[] getAllNames()
	{
		return Enumerable.ToArray<string>(Enumerable.Select<BackgroundData, string>(BackgroundData.bgs, (BackgroundData b) => b.name));
	}

	// Token: 0x06000294 RID: 660 RVA: 0x00003E89 File Offset: 0x00002089
	public List<WorldImage> getImages()
	{
		return this.images;
	}

	// Token: 0x06000295 RID: 661 RVA: 0x00003E91 File Offset: 0x00002091
	public List<LightInfo> getLights()
	{
		return this.lights;
	}

	// Token: 0x06000296 RID: 662 RVA: 0x00003E99 File Offset: 0x00002099
	private static string nameToIdName(string name)
	{
		return name.Replace(" ", string.Empty).ToLower().Trim();
	}

	// Token: 0x06000297 RID: 663 RVA: 0x00003EB5 File Offset: 0x000020B5
	private BackgroundData setShadowColor(Color color)
	{
		this.shadowColor = color;
		return this;
	}

	// Token: 0x06000298 RID: 664 RVA: 0x00003EBF File Offset: 0x000020BF
	private BackgroundData addBackground(string filename)
	{
		this.images.Add(new WorldImage(filename, new Vector3(-0.84f, -2.15f, 0f), new Vector3(1.765f, 1f, 1.0612f)));
		return this;
	}

	// Token: 0x06000299 RID: 665 RVA: 0x00003EFB File Offset: 0x000020FB
	private BackgroundData addLight(float x, float y, Color color, int range)
	{
		this.lights.Add(new LightInfo(new Vector3(y, 0.75f, x), color, range));
		return this;
	}

	// Token: 0x0600029A RID: 666 RVA: 0x00003F1D File Offset: 0x0000211D
	public BackgroundData setGradientColor(Color color)
	{
		this.gradientColor = color;
		return this;
	}

	// Token: 0x0600029B RID: 667 RVA: 0x00003F27 File Offset: 0x00002127
	public static BackgroundData getBackground(long background)
	{
		return BackgroundData.bgs[BackgroundData.getBackgroundIdFor(background)];
	}

	// Token: 0x0600029C RID: 668 RVA: 0x00003F39 File Offset: 0x00002139
	public static int getBackgroundIdFor(long n)
	{
		return (int)(n % (long)BackgroundData.bgs.Count);
	}

	// Token: 0x0600029D RID: 669 RVA: 0x00022FAC File Offset: 0x000211AC
	public static int getBackgroundIdFor(string name)
	{
		if (!string.IsNullOrEmpty(name))
		{
			string text = BackgroundData.nameToIdName(name);
			for (int i = 0; i < BackgroundData.bgs.Count; i++)
			{
				if (BackgroundData.bgs[i].idName == text)
				{
					return i;
				}
			}
		}
		return -1;
	}

	// Token: 0x04000172 RID: 370
	public List<WorldImage> images = new List<WorldImage>();

	// Token: 0x04000173 RID: 371
	public List<LightInfo> lights = new List<LightInfo>();

	// Token: 0x04000174 RID: 372
	public Color shadowColor = new Color(0.2f, 0.2f, 0.2f, 1f);

	// Token: 0x04000175 RID: 373
	public Color gradientColor = Color.black;

	// Token: 0x04000176 RID: 374
	public string name;

	// Token: 0x04000177 RID: 375
	private string idName;

	// Token: 0x04000178 RID: 376
	private static List<BackgroundData> bgs = new List<BackgroundData>();
}
