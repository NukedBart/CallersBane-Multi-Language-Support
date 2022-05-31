using System;
using System.Collections;
using System.Globalization;
using System.IO;
using UnityEngine;

// Token: 0x020003C5 RID: 965
public class ScreenCapture : MonoBehaviour
{
	// Token: 0x0600156F RID: 5487 RVA: 0x0000FAFC File Offset: 0x0000DCFC
	private void Awake()
	{
		this.regularUI = (GUISkin)ResourceManager.Load("_GUISkins/RegularUI");
	}

	// Token: 0x06001570 RID: 5488 RVA: 0x0000FB13 File Offset: 0x0000DD13
	private void LateUpdate()
	{
		if (this.screenshotTime > 0f)
		{
			this.screenshotTime -= Time.deltaTime;
		}
		if (Input.GetKeyDown(286))
		{
			base.StartCoroutine(this.CaptureScreenSoon());
		}
	}

	// Token: 0x06001571 RID: 5489 RVA: 0x00083068 File Offset: 0x00081268
	private IEnumerator CaptureScreenSoon()
	{
		this.screenshotTime = -1f;
		yield return null;
		FileUtil.createFolderIfNecessary(Application.persistentDataPath + "/Screenshots/");
		int number = 1;
		string dtString = DateTime.Now.ToString("dd-MM-yy", DateTimeFormatInfo.InvariantInfo);
		this.lastPath = this.getPathForDate(dtString, 1);
		while (File.Exists(this.lastPath))
		{
			number++;
			this.lastPath = this.getPathForDate(dtString, number);
		}
		Log.info("Saved screenshot to: " + this.lastPath);
		Application.CaptureScreenshot(this.lastPath);
		base.StartCoroutine(this.StartFlashSoon());
		yield break;
	}

	// Token: 0x06001572 RID: 5490 RVA: 0x00083084 File Offset: 0x00081284
	private IEnumerator StartFlashSoon()
	{
		yield return null;
		yield return null;
		this.screenshotTime = 4f;
		yield break;
	}

	// Token: 0x06001573 RID: 5491 RVA: 0x000830A0 File Offset: 0x000812A0
	private void OnGUI()
	{
		if (Event.current != null && OsSpec.getOS() == OSType.Windows && Event.current.type == 5 && Event.current.keyCode == 317)
		{
			base.StartCoroutine(this.CaptureScreenSoon());
		}
		if (this.screenshotTime > 0f)
		{
			GUI.depth = 1;
			GUI.skin = this.regularUI;
			Color color = GUI.color;
			float num = 4f - this.screenshotTime;
			float num2 = 0f;
			if (num < 0.15f)
			{
				num2 = num / 0.15f;
			}
			else if (num < 0.3f)
			{
				num2 = 1f - (num - 0.15f) / 0.15f;
			}
			if (num < 0.3f)
			{
				GUI.color = new Color(1f, 1f, 1f, num2 * 0.5f);
				GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), ResourceManager.LoadTexture("ChatUI/white"));
			}
			float num3 = 1f;
			if (this.screenshotTime < 0.5f)
			{
				num3 = this.screenshotTime / 0.5f;
			}
			GUI.color = new Color(1f, 1f, 1f, num3);
			GUI.Label(new Rect(10f, 10f, (float)Screen.width, 40f), "Saved screenshot to: " + this.lastPath);
			GUI.color = color;
		}
	}

	// Token: 0x06001574 RID: 5492 RVA: 0x0000FB53 File Offset: 0x0000DD53
	private string getPathForDate(string dtString, int number)
	{
		return string.Concat(new object[]
		{
			Application.persistentDataPath,
			"/Screenshots/Screen_",
			dtString,
			"_",
			number,
			".png"
		});
	}

	// Token: 0x040012A9 RID: 4777
	private const float SCREENSHOT_DISPLAY_TIME = 4f;

	// Token: 0x040012AA RID: 4778
	private const float SCREENSHOT_FADE_DURATION = 0.5f;

	// Token: 0x040012AB RID: 4779
	private const float FLASH_FADE_DURATION = 0.15f;

	// Token: 0x040012AC RID: 4780
	private float screenshotTime;

	// Token: 0x040012AD RID: 4781
	private string lastPath;

	// Token: 0x040012AE RID: 4782
	private GUISkin regularUI;
}
