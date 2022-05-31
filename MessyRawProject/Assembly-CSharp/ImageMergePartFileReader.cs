using System;
using System.Collections.Generic;
using System.Linq;
using JsonFx.Json;
using UnityEngine;

// Token: 0x02000442 RID: 1090
internal class ImageMergePartFileReader
{
	// Token: 0x06001846 RID: 6214 RVA: 0x00011859 File Offset: 0x0000FA59
	public static ImageMergePartFileReader.Desc readFile(string filename)
	{
		return ImageMergePartFileReader._read(FileUtil.readFileContents(filename));
	}

	// Token: 0x06001847 RID: 6215 RVA: 0x000926E4 File Offset: 0x000908E4
	public static ImageMergePartFileReader.Desc readAsset(string assetFilename)
	{
		Object @object = ResourceManager.Load(assetFilename);
		return (!(@object != null)) ? null : ImageMergePartFileReader._read(@object.ToString());
	}

	// Token: 0x06001848 RID: 6216 RVA: 0x00092718 File Offset: 0x00090918
	private static ImageMergePartFileReader.Desc _read(string s)
	{
		if (s == null)
		{
			return null;
		}
		ImageMergePartFileReader.Desc desc = new JsonReader().Read<ImageMergePartFileReader.Desc>(s);
		if (desc == null)
		{
			return null;
		}
		return desc;
	}

	// Token: 0x02000443 RID: 1091
	public class Desc
	{
		// Token: 0x06001849 RID: 6217 RVA: 0x00002DDA File Offset: 0x00000FDA
		private Desc()
		{
		}

		// Token: 0x0600184A RID: 6218 RVA: 0x00092744 File Offset: 0x00090944
		public List<ImageMergePartFileReader.Layer> getRandomLayers(int count)
		{
			List<ImageMergePartFileReader.Layer> list = new List<ImageMergePartFileReader.Layer>();
			list.Add(RandomUtil.choice<ImageMergePartFileReader.Layer>(Enumerable.ToList<ImageMergePartFileReader.Layer>(Enumerable.Where<ImageMergePartFileReader.Layer>(this.layers, (ImageMergePartFileReader.Layer x) => x.isBackground != 0))));
			List<ImageMergePartFileReader.Layer> list2 = RandomUtil.sample<ImageMergePartFileReader.Layer>(Enumerable.ToList<ImageMergePartFileReader.Layer>(Enumerable.Where<ImageMergePartFileReader.Layer>(this.layers, (ImageMergePartFileReader.Layer x) => x.isBackground == 0)), count);
			list2.Sort((ImageMergePartFileReader.Layer a, ImageMergePartFileReader.Layer b) => b.layer - a.layer);
			list.AddRange(Enumerable.ToList<ImageMergePartFileReader.Layer>(list2));
			return list;
		}

		// Token: 0x0400151D RID: 5405
		public ImageMergePartFileReader.Layer[] layers;
	}

	// Token: 0x02000444 RID: 1092
	public class Layer
	{
		// Token: 0x0600184F RID: 6223 RVA: 0x000927F4 File Offset: 0x000909F4
		public static List<ImageMergerComponent.Pos> getPositions(IEnumerable<ImageMergePartFileReader.Layer> layers, string path)
		{
			List<ImageMergerComponent.Pos> list = new List<ImageMergerComponent.Pos>();
			foreach (ImageMergePartFileReader.Layer layer in layers)
			{
				list.AddRange(layer.getPositions(path));
			}
			return list;
		}

		// Token: 0x06001850 RID: 6224 RVA: 0x00092854 File Offset: 0x00090A54
		public ImageMergerComponent.Pos[] getPositions(string path)
		{
			ImageMergerComponent.Pos[] array = new ImageMergerComponent.Pos[this.parts.Length];
			for (int i = 0; i < this.parts.Length; i++)
			{
				ImageMergePartFileReader.Part part = this.parts[i];
				Rect rect = (this.isBackground == 0) ? new Rect(part.x / (float)this.width, part.y / (float)this.height, part.w / (float)this.width, part.h / (float)this.height) : new Rect(0f, 0f, 1f, 1f);
				ImageMergerComponent.Pos pos = new ImageMergerComponent.Pos(path + part.filename).setRect(rect);
				pos.layerWidth = (float)this.width;
				pos.layerHeight = (float)this.height;
				array[i] = pos;
			}
			return array;
		}

		// Token: 0x04001521 RID: 5409
		public int width;

		// Token: 0x04001522 RID: 5410
		public int height;

		// Token: 0x04001523 RID: 5411
		public int layer;

		// Token: 0x04001524 RID: 5412
		public int isBackground;

		// Token: 0x04001525 RID: 5413
		public ImageMergePartFileReader.Part[] parts;
	}

	// Token: 0x02000445 RID: 1093
	public class Part
	{
		// Token: 0x04001526 RID: 5414
		public string filename;

		// Token: 0x04001527 RID: 5415
		public float x;

		// Token: 0x04001528 RID: 5416
		public float y;

		// Token: 0x04001529 RID: 5417
		public float w;

		// Token: 0x0400152A RID: 5418
		public float h;
	}
}
