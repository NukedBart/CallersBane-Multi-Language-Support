using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

// Token: 0x02000437 RID: 1079
public static class FileUtil
{
	// Token: 0x060017E4 RID: 6116 RVA: 0x00091668 File Offset: 0x0008F868
	public static void writeFileContents(string filename, string contents)
	{
		using (StreamWriter streamWriter = new StreamWriter(filename))
		{
			streamWriter.Write(contents);
		}
	}

	// Token: 0x060017E5 RID: 6117 RVA: 0x00011287 File Offset: 0x0000F487
	public static bool fileExists(string filename)
	{
		return File.Exists(filename);
	}

	// Token: 0x060017E6 RID: 6118 RVA: 0x000916A8 File Offset: 0x0008F8A8
	public static string readFileContents(string filename)
	{
		string result = null;
		if (FileUtil.fileExists(filename))
		{
			using (FileStream fileStream = new FileStream(filename, 3, 1, 3))
			{
				using (TextReader textReader = new StreamReader(fileStream))
				{
					result = textReader.ReadToEnd();
				}
			}
		}
		return result;
	}

	// Token: 0x060017E7 RID: 6119 RVA: 0x0009171C File Offset: 0x0008F91C
	public static void extractZip(string filename, string directory)
	{
		if (Directory.Exists(directory))
		{
			return;
		}
		FileStream fileStream = null;
		ZipFile zipFile = null;
		try
		{
			fileStream = File.OpenRead(filename);
			zipFile = new ZipFile(fileStream);
			foreach (object obj in zipFile)
			{
				ZipEntry zipEntry = (ZipEntry)obj;
				string name = zipEntry.Name;
				byte[] array = new byte[4096];
				Stream inputStream = zipFile.GetInputStream(zipEntry);
				string text = Path.Combine(directory, name);
				string directoryName = Path.GetDirectoryName(text);
				if (directoryName.Length > 0)
				{
					Directory.CreateDirectory(directoryName);
				}
				using (FileStream fileStream2 = File.Create(text))
				{
					int num = 0;
					long size = zipEntry.Size;
					while ((long)num < size)
					{
						int num2 = inputStream.Read(array, 0, array.Length);
						fileStream2.Write(array, 0, num2);
						num += num2;
					}
				}
			}
		}
		catch (Exception s)
		{
			Log.error(s);
		}
		finally
		{
			if (zipFile != null)
			{
				zipFile.Close();
			}
			if (fileStream != null)
			{
				fileStream.Close();
			}
		}
	}

	// Token: 0x060017E8 RID: 6120 RVA: 0x0001128F File Offset: 0x0000F48F
	public static void saveContents(string fn, byte[] data)
	{
		FileUtil.createFolderIfNecessary(fn);
		File.WriteAllBytes(fn, data);
	}

	// Token: 0x060017E9 RID: 6121 RVA: 0x0001129F File Offset: 0x0000F49F
	public static void saveContents(string fn, string contents)
	{
		FileUtil.createFolderIfNecessary(fn);
		File.WriteAllText(fn, contents);
	}

	// Token: 0x060017EA RID: 6122 RVA: 0x00091884 File Offset: 0x0008FA84
	public static bool createFolderIfNecessary(string fileOrFolderName)
	{
		string directoryName = Path.GetDirectoryName(fileOrFolderName);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
			return true;
		}
		return false;
	}
}
