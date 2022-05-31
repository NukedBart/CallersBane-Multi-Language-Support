using System;

// Token: 0x02000114 RID: 276
public class TagSoundReader
{
	// Token: 0x060008B6 RID: 2230 RVA: 0x00007925 File Offset: 0x00005B25
	public TagSoundReader(CardType ct)
	{
		this.ct = ct;
	}

	// Token: 0x060008B7 RID: 2231 RVA: 0x0004569C File Offset: 0x0004389C
	public TagSoundReader.Snd get(string name)
	{
		if (App.useExternalResources())
		{
			this.ct.refreshTagsFromDisk();
		}
		string text = this.ct.getTag<string>(name, null);
		if (text != null)
		{
			text = "Sounds/hyperduck/" + this.trimExtension(text);
		}
		return new TagSoundReader.Snd(text)
		{
			volume = this.ct.getTag<float>(name + "_volume", 1f),
			delay = this.ct.getTag<float>(name + "_delay", 0f),
			minPitch = this.ct.getTag<float>(name + "_pitch_min", 1f),
			maxPitch = this.ct.getTag<float>(name + "_pitch_max", 1f)
		};
	}

	// Token: 0x060008B8 RID: 2232 RVA: 0x00045770 File Offset: 0x00043970
	public TagSoundReader.Snd get(string name, string defaultName)
	{
		TagSoundReader.Snd snd = this.get(name);
		if (snd.name == null)
		{
			snd.name = defaultName;
		}
		return snd;
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x00045798 File Offset: 0x00043998
	private string trimExtension(string fn)
	{
		int num = fn.LastIndexOf('.');
		return (num >= 0) ? fn.Substring(0, num) : fn;
	}

	// Token: 0x04000674 RID: 1652
	private readonly CardType ct;

	// Token: 0x02000115 RID: 277
	public class Snd
	{
		// Token: 0x060008BA RID: 2234 RVA: 0x00007934 File Offset: 0x00005B34
		public Snd(string name)
		{
			this.name = name;
		}

		// Token: 0x060008BB RID: 2235 RVA: 0x00007964 File Offset: 0x00005B64
		public float getPitch()
		{
			return RandomUtil.random(this.minPitch, this.maxPitch);
		}

		// Token: 0x060008BC RID: 2236 RVA: 0x000457C4 File Offset: 0x000439C4
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"name: ",
				this.name,
				", delay: ",
				this.delay,
				", volume: ",
				this.volume,
				", pitch: ",
				this.minPitch,
				"-",
				this.maxPitch,
				" (at: ",
				this.start,
				")"
			});
		}

		// Token: 0x060008BD RID: 2237 RVA: 0x00007977 File Offset: 0x00005B77
		public static implicit operator bool(TagSoundReader.Snd snd)
		{
			return snd.name != null;
		}

		// Token: 0x04000675 RID: 1653
		public string name;

		// Token: 0x04000676 RID: 1654
		public string start;

		// Token: 0x04000677 RID: 1655
		public float volume = 1f;

		// Token: 0x04000678 RID: 1656
		public float delay;

		// Token: 0x04000679 RID: 1657
		public float minPitch = 1f;

		// Token: 0x0400067A RID: 1658
		public float maxPitch = 1f;
	}
}
