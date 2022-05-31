using System;

// Token: 0x020002C1 RID: 705
public class Version : IComparable<Version>
{
	// Token: 0x06001298 RID: 4760 RVA: 0x0000DF7B File Offset: 0x0000C17B
	public Version(string version)
	{
		Version.getMajorMinor(version, out this.major, out this.minor, out this.build);
	}

	// Token: 0x06001299 RID: 4761 RVA: 0x0000DF9B File Offset: 0x0000C19B
	public Version() : this(0, 0)
	{
	}

	// Token: 0x0600129A RID: 4762 RVA: 0x0000DFA5 File Offset: 0x0000C1A5
	public Version(int major) : this(major, 0)
	{
	}

	// Token: 0x0600129B RID: 4763 RVA: 0x0000DFAF File Offset: 0x0000C1AF
	public Version(int major, int minor) : this(major, minor, 0)
	{
	}

	// Token: 0x0600129C RID: 4764 RVA: 0x0000DFBA File Offset: 0x0000C1BA
	public Version(int major, int minor, int build)
	{
		this.major = major;
		this.minor = minor;
		this.build = build;
	}

	// Token: 0x0600129D RID: 4765 RVA: 0x0000DFD7 File Offset: 0x0000C1D7
	public bool isLowerThan(Version v)
	{
		return this.CompareTo(v) < 0;
	}

	// Token: 0x0600129E RID: 4766 RVA: 0x00078000 File Offset: 0x00076200
	public int CompareTo(Version v)
	{
		return Mth.firstNonZero(new int[]
		{
			this.major.CompareTo(v.major),
			this.minor.CompareTo(v.minor),
			this.build.CompareTo(v.build)
		});
	}

	// Token: 0x0600129F RID: 4767 RVA: 0x0000DFE3 File Offset: 0x0000C1E3
	public bool isZero()
	{
		return this.getMajor() == 0 && this.getMinor() == 0 && this.getBuild() == 0;
	}

	// Token: 0x060012A0 RID: 4768 RVA: 0x0000E007 File Offset: 0x0000C207
	public int getMajor()
	{
		return this.major;
	}

	// Token: 0x060012A1 RID: 4769 RVA: 0x0000E00F File Offset: 0x0000C20F
	public int getMinor()
	{
		return this.minor;
	}

	// Token: 0x060012A2 RID: 4770 RVA: 0x0000E017 File Offset: 0x0000C217
	public int getBuild()
	{
		return this.build;
	}

	// Token: 0x060012A3 RID: 4771 RVA: 0x00078054 File Offset: 0x00076254
	public override string ToString()
	{
		return string.Concat(new object[]
		{
			this.major,
			".",
			this.minor,
			".",
			this.build
		});
	}

	// Token: 0x060012A4 RID: 4772 RVA: 0x000780A8 File Offset: 0x000762A8
	private static void getMajorMinor(string v, out int major, out int minor, out int build)
	{
		string[] array = v.Split(new char[]
		{
			'.'
		}, 1);
		major = (minor = (build = 0));
		if (array.Length > 0 && !int.TryParse(array[0], ref major))
		{
			Log.error("Couldn't parse Major version number! " + v);
		}
		if (array.Length > 1 && !int.TryParse(array[1], ref minor))
		{
			Log.error("Couldn't parse Minor version number! " + v);
		}
		if (array.Length > 2 && !int.TryParse(array[2], ref build))
		{
			Log.error("Couldn't parse Build version number! " + v);
		}
	}

	// Token: 0x04000F6A RID: 3946
	private int major;

	// Token: 0x04000F6B RID: 3947
	private int minor;

	// Token: 0x04000F6C RID: 3948
	private int build;
}
