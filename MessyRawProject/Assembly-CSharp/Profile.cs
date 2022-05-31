using System;
using System.Collections.Generic;
using System.Text;

// Token: 0x02000391 RID: 913
public class Profile
{
	// Token: 0x0600145E RID: 5214 RVA: 0x00002DDA File Offset: 0x00000FDA
	private Profile()
	{
	}

	// Token: 0x06001460 RID: 5216 RVA: 0x0007E818 File Offset: 0x0007CA18
	public static void StartProfile(string tag)
	{
		Profile.ProfilePoint profilePoint;
		Profile.profiles.TryGetValue(tag, ref profilePoint);
		profilePoint.lastRecorded = DateTime.UtcNow;
		Profile.profiles[tag] = profilePoint;
	}

	// Token: 0x06001461 RID: 5217 RVA: 0x0007E84C File Offset: 0x0007CA4C
	public static void EndProfile(string tag)
	{
		if (!Profile.profiles.ContainsKey(tag))
		{
			Log.error("Can only end profiling for a tag which has already been started (tag was " + tag + ")");
			return;
		}
		Profile.ProfilePoint profilePoint = Profile.profiles[tag];
		profilePoint.totalTime += DateTime.UtcNow - profilePoint.lastRecorded;
		profilePoint.totalCalls++;
		Profile.profiles[tag] = profilePoint;
	}

	// Token: 0x06001462 RID: 5218 RVA: 0x0000F039 File Offset: 0x0000D239
	public static void Reset()
	{
		Profile.profiles.Clear();
		Profile.startTime = DateTime.UtcNow;
	}

	// Token: 0x06001463 RID: 5219 RVA: 0x0007E8CC File Offset: 0x0007CACC
	public static void PrintResults()
	{
		TimeSpan timeSpan = DateTime.UtcNow - Profile.startTime;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("============================\n\t\t\t\tProfile results:\n============================\n");
		foreach (KeyValuePair<string, Profile.ProfilePoint> keyValuePair in Profile.profiles)
		{
			double totalSeconds = keyValuePair.Value.totalTime.TotalSeconds;
			int totalCalls = keyValuePair.Value.totalCalls;
			if (totalCalls >= 1)
			{
				stringBuilder.Append("\nProfile ");
				stringBuilder.Append(keyValuePair.Key);
				stringBuilder.Append(" took ");
				stringBuilder.Append(totalSeconds.ToString("F5"));
				stringBuilder.Append(" seconds to complete over ");
				stringBuilder.Append(totalCalls);
				stringBuilder.Append(" iteration");
				if (totalCalls != 1)
				{
					stringBuilder.Append("s");
				}
				stringBuilder.Append(", averaging ");
				stringBuilder.Append((totalSeconds / (double)totalCalls).ToString("F5"));
				stringBuilder.Append(" seconds per call");
			}
		}
		stringBuilder.Append("\n\n============================\n\t\tTotal runtime: ");
		stringBuilder.Append(timeSpan.TotalSeconds.ToString("F3"));
		stringBuilder.Append(" seconds\n============================");
		Log.info(stringBuilder.ToString());
	}

	// Token: 0x0400119E RID: 4510
	private static Dictionary<string, Profile.ProfilePoint> profiles = new Dictionary<string, Profile.ProfilePoint>();

	// Token: 0x0400119F RID: 4511
	private static DateTime startTime = DateTime.UtcNow;

	// Token: 0x02000392 RID: 914
	public struct ProfilePoint
	{
		// Token: 0x040011A0 RID: 4512
		public DateTime lastRecorded;

		// Token: 0x040011A1 RID: 4513
		public TimeSpan totalTime;

		// Token: 0x040011A2 RID: 4514
		public int totalCalls;
	}
}
