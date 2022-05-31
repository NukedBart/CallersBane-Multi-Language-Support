using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000002 RID: 2
public class iTween : MonoBehaviour
{
	// Token: 0x06000003 RID: 3 RVA: 0x00002164 File Offset: 0x00000364
	public void setLocalTimer(iTween.ITimer timer)
	{
		this.localTimer = timer;
	}

	// Token: 0x06000004 RID: 4 RVA: 0x0000216D File Offset: 0x0000036D
	public static void setSharedTimer(iTween.ITimer timer)
	{
		iTween.sharedTimer = timer;
	}

	// Token: 0x06000005 RID: 5 RVA: 0x00012F4C File Offset: 0x0001114C
	public static void CameraFadeFrom(float amount, float time)
	{
		if (iTween.cameraFade)
		{
			iTween.CameraFadeFrom(iTween.Hash(new object[]
			{
				"amount",
				amount,
				"time",
				time
			}));
		}
		else
		{
			Debug.LogError("iTween Error: You must first add a camera fade object with CameraFadeAdd() before atttempting to use camera fading.");
		}
	}

	// Token: 0x06000006 RID: 6 RVA: 0x00002175 File Offset: 0x00000375
	public static void CameraFadeFrom(Hashtable args)
	{
		if (iTween.cameraFade)
		{
			iTween.ColorFrom(iTween.cameraFade, args);
		}
		else
		{
			Debug.LogError("iTween Error: You must first add a camera fade object with CameraFadeAdd() before atttempting to use camera fading.");
		}
	}

	// Token: 0x06000007 RID: 7 RVA: 0x00012FAC File Offset: 0x000111AC
	public static void CameraFadeTo(float amount, float time)
	{
		if (iTween.cameraFade)
		{
			iTween.CameraFadeTo(iTween.Hash(new object[]
			{
				"amount",
				amount,
				"time",
				time
			}));
		}
		else
		{
			Debug.LogError("iTween Error: You must first add a camera fade object with CameraFadeAdd() before atttempting to use camera fading.");
		}
	}

	// Token: 0x06000008 RID: 8 RVA: 0x000021A0 File Offset: 0x000003A0
	public static void CameraFadeTo(Hashtable args)
	{
		if (iTween.cameraFade)
		{
			iTween.ColorTo(iTween.cameraFade, args);
		}
		else
		{
			Debug.LogError("iTween Error: You must first add a camera fade object with CameraFadeAdd() before atttempting to use camera fading.");
		}
	}

	// Token: 0x06000009 RID: 9 RVA: 0x0001300C File Offset: 0x0001120C
	public static void ValueTo(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		if (!args.Contains("onupdate") || !args.Contains("from") || !args.Contains("to"))
		{
			Debug.LogError("iTween Error: ValueTo() requires an 'onupdate' callback function and a 'from' and 'to' property.  The supplied 'onupdate' callback must accept a single argument that is the same type as the supplied 'from' and 'to' properties!");
			return;
		}
		args["type"] = "value";
		if (args["from"].GetType() == typeof(Vector2))
		{
			args["method"] = "vector2";
		}
		else if (args["from"].GetType() == typeof(Vector3))
		{
			args["method"] = "vector3";
		}
		else if (args["from"].GetType() == typeof(Rect))
		{
			args["method"] = "rect";
		}
		else if (args["from"].GetType() == typeof(float))
		{
			args["method"] = "float";
		}
		else
		{
			if (args["from"].GetType() != typeof(Color))
			{
				Debug.LogError("iTween Error: ValueTo() only works with interpolating Vector3s, Vector2s, floats, ints, Rects and Colors!");
				return;
			}
			args["method"] = "color";
		}
		if (!args.Contains("easetype"))
		{
			args.Add("easetype", iTween.EaseType.linear);
		}
		iTween.Launch(target, args);
	}

	// Token: 0x0600000A RID: 10 RVA: 0x000021CB File Offset: 0x000003CB
	public static void FadeFrom(GameObject target, float alpha, float time)
	{
		iTween.FadeFrom(target, iTween.Hash(new object[]
		{
			"alpha",
			alpha,
			"time",
			time
		}));
	}

	// Token: 0x0600000B RID: 11 RVA: 0x00002200 File Offset: 0x00000400
	public static void FadeFrom(GameObject target, Hashtable args)
	{
		iTween.ColorFrom(target, args);
	}

	// Token: 0x0600000C RID: 12 RVA: 0x00002209 File Offset: 0x00000409
	public static void FadeTo(GameObject target, float alpha, float time)
	{
		iTween.FadeTo(target, iTween.Hash(new object[]
		{
			"alpha",
			alpha,
			"time",
			time
		}));
	}

	// Token: 0x0600000D RID: 13 RVA: 0x0000223E File Offset: 0x0000043E
	public static void FadeTo(GameObject target, Hashtable args)
	{
		iTween.ColorTo(target, args);
	}

	// Token: 0x0600000E RID: 14 RVA: 0x00002247 File Offset: 0x00000447
	public static void ColorFrom(GameObject target, Color color, float time)
	{
		iTween.ColorFrom(target, iTween.Hash(new object[]
		{
			"color",
			color,
			"time",
			time
		}));
	}

	// Token: 0x0600000F RID: 15 RVA: 0x000131A4 File Offset: 0x000113A4
	public static void ColorFrom(GameObject target, Hashtable args)
	{
		Color color = default(Color);
		Color color2 = default(Color);
		args = iTween.CleanArgs(args);
		if (!args.Contains("includechildren") || (bool)args["includechildren"])
		{
			foreach (object obj in target.transform)
			{
				Transform transform = (Transform)obj;
				Hashtable hashtable = (Hashtable)args.Clone();
				hashtable["ischild"] = true;
				iTween.ColorFrom(transform.gameObject, hashtable);
			}
		}
		if (!args.Contains("easetype"))
		{
			args.Add("easetype", iTween.EaseType.linear);
		}
		if (target.GetComponent(typeof(GUITexture)))
		{
			color = (color2 = target.guiTexture.color);
		}
		else if (target.GetComponent(typeof(GUIText)))
		{
			color = (color2 = target.guiText.material.color);
		}
		else if (target.renderer)
		{
			color = (color2 = target.renderer.material.color);
		}
		else if (target.light)
		{
			color = (color2 = target.light.color);
		}
		if (args.Contains("color"))
		{
			color = (Color)args["color"];
		}
		else
		{
			if (args.Contains("r"))
			{
				color.r = (float)args["r"];
			}
			if (args.Contains("g"))
			{
				color.g = (float)args["g"];
			}
			if (args.Contains("b"))
			{
				color.b = (float)args["b"];
			}
			if (args.Contains("a"))
			{
				color.a = (float)args["a"];
			}
		}
		if (args.Contains("amount"))
		{
			color.a = (float)args["amount"];
			args.Remove("amount");
		}
		else if (args.Contains("alpha"))
		{
			color.a = (float)args["alpha"];
			args.Remove("alpha");
		}
		if (target.GetComponent(typeof(GUITexture)))
		{
			target.guiTexture.color = color;
		}
		else if (target.GetComponent(typeof(GUIText)))
		{
			target.guiText.material.color = color;
		}
		else if (target.renderer)
		{
			target.renderer.material.color = color;
		}
		else if (target.light)
		{
			target.light.color = color;
		}
		args["color"] = color2;
		args["type"] = "color";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x06000010 RID: 16 RVA: 0x0000227C File Offset: 0x0000047C
	public static void ColorTo(GameObject target, Color color, float time)
	{
		iTween.ColorTo(target, iTween.Hash(new object[]
		{
			"color",
			color,
			"time",
			time
		}));
	}

	// Token: 0x06000011 RID: 17 RVA: 0x00013534 File Offset: 0x00011734
	public static void ColorTo(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		if (!args.Contains("includechildren") || (bool)args["includechildren"])
		{
			foreach (object obj in target.transform)
			{
				Transform transform = (Transform)obj;
				Hashtable hashtable = (Hashtable)args.Clone();
				hashtable["ischild"] = true;
				iTween.ColorTo(transform.gameObject, hashtable);
			}
		}
		if (!args.Contains("easetype"))
		{
			args.Add("easetype", iTween.EaseType.linear);
		}
		args["type"] = "color";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x06000012 RID: 18 RVA: 0x00013630 File Offset: 0x00011830
	public static void AudioFrom(GameObject target, float volume, float pitch, float time)
	{
		iTween.AudioFrom(target, iTween.Hash(new object[]
		{
			"volume",
			volume,
			"pitch",
			pitch,
			"time",
			time
		}));
	}

	// Token: 0x06000013 RID: 19 RVA: 0x00013684 File Offset: 0x00011884
	public static void AudioFrom(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		AudioSource audioSource;
		if (args.Contains("audiosource"))
		{
			audioSource = (AudioSource)args["audiosource"];
		}
		else
		{
			if (!target.GetComponent(typeof(AudioSource)))
			{
				Debug.LogError("iTween Error: AudioFrom requires an AudioSource.");
				return;
			}
			audioSource = target.audio;
		}
		Vector2 vector;
		Vector2 vector2;
		vector.x = (vector2.x = audioSource.volume);
		vector.y = (vector2.y = audioSource.pitch);
		if (args.Contains("volume"))
		{
			vector2.x = (float)args["volume"];
		}
		if (args.Contains("pitch"))
		{
			vector2.y = (float)args["pitch"];
		}
		audioSource.volume = vector2.x;
		audioSource.pitch = vector2.y;
		args["volume"] = vector.x;
		args["pitch"] = vector.y;
		if (!args.Contains("easetype"))
		{
			args.Add("easetype", iTween.EaseType.linear);
		}
		args["type"] = "audio";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x06000014 RID: 20 RVA: 0x00013800 File Offset: 0x00011A00
	public static void AudioTo(GameObject target, float volume, float pitch, float time)
	{
		iTween.AudioTo(target, iTween.Hash(new object[]
		{
			"volume",
			volume,
			"pitch",
			pitch,
			"time",
			time
		}));
	}

	// Token: 0x06000015 RID: 21 RVA: 0x00013854 File Offset: 0x00011A54
	public static void AudioTo(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		if (!args.Contains("easetype"))
		{
			args.Add("easetype", iTween.EaseType.linear);
		}
		args["type"] = "audio";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x06000016 RID: 22 RVA: 0x000022B1 File Offset: 0x000004B1
	public static void Stab(GameObject target, AudioClip audioclip, float delay)
	{
		iTween.Stab(target, iTween.Hash(new object[]
		{
			"audioclip",
			audioclip,
			"delay",
			delay
		}));
	}

	// Token: 0x06000017 RID: 23 RVA: 0x000022E1 File Offset: 0x000004E1
	public static void Stab(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "stab";
		iTween.Launch(target, args);
	}

	// Token: 0x06000018 RID: 24 RVA: 0x00002302 File Offset: 0x00000502
	public static void LookFrom(GameObject target, Vector3 looktarget, float time)
	{
		iTween.LookFrom(target, iTween.Hash(new object[]
		{
			"looktarget",
			looktarget,
			"time",
			time
		}));
	}

	// Token: 0x06000019 RID: 25 RVA: 0x000138B4 File Offset: 0x00011AB4
	public static void LookFrom(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		Vector3 eulerAngles = target.transform.eulerAngles;
		if (args["looktarget"].GetType() == typeof(Transform))
		{
			Transform transform = target.transform;
			Transform transform2 = (Transform)args["looktarget"];
			Vector3? vector = (Vector3?)args["up"];
			transform.LookAt(transform2, (vector == null) ? iTween.Defaults.up : vector.Value);
		}
		else if (args["looktarget"].GetType() == typeof(Vector3))
		{
			Transform transform3 = target.transform;
			Vector3 vector2 = (Vector3)args["looktarget"];
			Vector3? vector3 = (Vector3?)args["up"];
			transform3.LookAt(vector2, (vector3 == null) ? iTween.Defaults.up : vector3.Value);
		}
		if (args.Contains("axis"))
		{
			Vector3 eulerAngles2 = target.transform.eulerAngles;
			string text = (string)args["axis"];
			if (text != null)
			{
				if (iTween.<>f__switch$map0 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
					dictionary.Add("x", 0);
					dictionary.Add("y", 1);
					dictionary.Add("z", 2);
					iTween.<>f__switch$map0 = dictionary;
				}
				int num;
				if (iTween.<>f__switch$map0.TryGetValue(text, ref num))
				{
					switch (num)
					{
					case 0:
						eulerAngles2.y = eulerAngles.y;
						eulerAngles2.z = eulerAngles.z;
						break;
					case 1:
						eulerAngles2.x = eulerAngles.x;
						eulerAngles2.z = eulerAngles.z;
						break;
					case 2:
						eulerAngles2.x = eulerAngles.x;
						eulerAngles2.y = eulerAngles.y;
						break;
					}
				}
			}
			target.transform.eulerAngles = eulerAngles2;
		}
		args["rotation"] = eulerAngles;
		args["type"] = "rotate";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x0600001A RID: 26 RVA: 0x00002337 File Offset: 0x00000537
	public static void LookTo(GameObject target, Vector3 looktarget, float time)
	{
		iTween.LookTo(target, iTween.Hash(new object[]
		{
			"looktarget",
			looktarget,
			"time",
			time
		}));
	}

	// Token: 0x0600001B RID: 27 RVA: 0x00013AF4 File Offset: 0x00011CF4
	public static void LookTo(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		if (args.Contains("looktarget") && args["looktarget"].GetType() == typeof(Transform))
		{
			Transform transform = (Transform)args["looktarget"];
			args["position"] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			args["rotation"] = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
		}
		args["type"] = "look";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x0600001C RID: 28 RVA: 0x0000236C File Offset: 0x0000056C
	public static void MoveTo(GameObject target, Vector3 position, float time)
	{
		iTween.MoveTo(target, iTween.Hash(new object[]
		{
			"position",
			position,
			"time",
			time
		}));
	}

	// Token: 0x0600001D RID: 29 RVA: 0x00013BF4 File Offset: 0x00011DF4
	public static void MoveTo(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		if (args.Contains("position") && args["position"].GetType() == typeof(Transform))
		{
			Transform transform = (Transform)args["position"];
			args["position"] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			args["rotation"] = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
			args["scale"] = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
		args["type"] = "move";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x0600001E RID: 30 RVA: 0x000023A1 File Offset: 0x000005A1
	public static void MoveFrom(GameObject target, Vector3 position, float time)
	{
		iTween.MoveFrom(target, iTween.Hash(new object[]
		{
			"position",
			position,
			"time",
			time
		}));
	}

	// Token: 0x0600001F RID: 31 RVA: 0x00013D34 File Offset: 0x00011F34
	public static void MoveFrom(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		bool flag;
		if (args.Contains("islocal"))
		{
			flag = (bool)args["islocal"];
		}
		else
		{
			flag = iTween.Defaults.isLocal;
		}
		if (args.Contains("path"))
		{
			Vector3[] array2;
			if (args["path"].GetType() == typeof(Vector3[]))
			{
				Vector3[] array = (Vector3[])args["path"];
				array2 = new Vector3[array.Length];
				Array.Copy(array, array2, array.Length);
			}
			else
			{
				Transform[] array3 = (Transform[])args["path"];
				array2 = new Vector3[array3.Length];
				for (int i = 0; i < array3.Length; i++)
				{
					array2[i] = array3[i].position;
				}
			}
			if (array2[array2.Length - 1] != target.transform.position)
			{
				Vector3[] array4 = new Vector3[array2.Length + 1];
				Array.Copy(array2, array4, array2.Length);
				if (flag)
				{
					array4[array4.Length - 1] = target.transform.localPosition;
					target.transform.localPosition = array4[0];
				}
				else
				{
					array4[array4.Length - 1] = target.transform.position;
					target.transform.position = array4[0];
				}
				args["path"] = array4;
			}
			else
			{
				if (flag)
				{
					target.transform.localPosition = array2[0];
				}
				else
				{
					target.transform.position = array2[0];
				}
				args["path"] = array2;
			}
		}
		else
		{
			Vector3 vector2;
			Vector3 vector;
			if (flag)
			{
				vector = (vector2 = target.transform.localPosition);
			}
			else
			{
				vector = (vector2 = target.transform.position);
			}
			if (args.Contains("position"))
			{
				if (args["position"].GetType() == typeof(Transform))
				{
					Transform transform = (Transform)args["position"];
					vector = transform.position;
				}
				else if (args["position"].GetType() == typeof(Vector3))
				{
					vector = (Vector3)args["position"];
				}
			}
			else
			{
				if (args.Contains("x"))
				{
					vector.x = (float)args["x"];
				}
				if (args.Contains("y"))
				{
					vector.y = (float)args["y"];
				}
				if (args.Contains("z"))
				{
					vector.z = (float)args["z"];
				}
			}
			if (flag)
			{
				target.transform.localPosition = vector;
			}
			else
			{
				target.transform.position = vector;
			}
			args["position"] = vector2;
		}
		args["type"] = "move";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x06000020 RID: 32 RVA: 0x000023D6 File Offset: 0x000005D6
	public static void MoveAdd(GameObject target, Vector3 amount, float time)
	{
		iTween.MoveAdd(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x06000021 RID: 33 RVA: 0x0000240B File Offset: 0x0000060B
	public static void MoveAdd(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "move";
		args["method"] = "add";
		iTween.Launch(target, args);
	}

	// Token: 0x06000022 RID: 34 RVA: 0x0000243C File Offset: 0x0000063C
	public static void MoveBy(GameObject target, Vector3 amount, float time)
	{
		iTween.MoveBy(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x06000023 RID: 35 RVA: 0x00002471 File Offset: 0x00000671
	public static void MoveBy(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "move";
		args["method"] = "by";
		iTween.Launch(target, args);
	}

	// Token: 0x06000024 RID: 36 RVA: 0x000024A2 File Offset: 0x000006A2
	public static void ScaleTo(GameObject target, Vector3 scale, float time)
	{
		iTween.ScaleTo(target, iTween.Hash(new object[]
		{
			"scale",
			scale,
			"time",
			time
		}));
	}

	// Token: 0x06000025 RID: 37 RVA: 0x000140A0 File Offset: 0x000122A0
	public static void ScaleTo(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		if (args.Contains("scale") && args["scale"].GetType() == typeof(Transform))
		{
			Transform transform = (Transform)args["scale"];
			args["position"] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			args["rotation"] = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
			args["scale"] = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
		args["type"] = "scale";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x06000026 RID: 38 RVA: 0x000024D7 File Offset: 0x000006D7
	public static void ScaleFrom(GameObject target, Vector3 scale, float time)
	{
		iTween.ScaleFrom(target, iTween.Hash(new object[]
		{
			"scale",
			scale,
			"time",
			time
		}));
	}

	// Token: 0x06000027 RID: 39 RVA: 0x000141E0 File Offset: 0x000123E0
	public static void ScaleFrom(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		Vector3 localScale2;
		Vector3 localScale = localScale2 = target.transform.localScale;
		if (args.Contains("scale"))
		{
			if (args["scale"].GetType() == typeof(Transform))
			{
				Transform transform = (Transform)args["scale"];
				localScale = transform.localScale;
			}
			else if (args["scale"].GetType() == typeof(Vector3))
			{
				localScale = (Vector3)args["scale"];
			}
		}
		else
		{
			if (args.Contains("x"))
			{
				localScale.x = (float)args["x"];
			}
			if (args.Contains("y"))
			{
				localScale.y = (float)args["y"];
			}
			if (args.Contains("z"))
			{
				localScale.z = (float)args["z"];
			}
		}
		target.transform.localScale = localScale;
		args["scale"] = localScale2;
		args["type"] = "scale";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x06000028 RID: 40 RVA: 0x0000250C File Offset: 0x0000070C
	public static void ScaleAdd(GameObject target, Vector3 amount, float time)
	{
		iTween.ScaleAdd(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x06000029 RID: 41 RVA: 0x00002541 File Offset: 0x00000741
	public static void ScaleAdd(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "scale";
		args["method"] = "add";
		iTween.Launch(target, args);
	}

	// Token: 0x0600002A RID: 42 RVA: 0x00002572 File Offset: 0x00000772
	public static void ScaleBy(GameObject target, Vector3 amount, float time)
	{
		iTween.ScaleBy(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x0600002B RID: 43 RVA: 0x000025A7 File Offset: 0x000007A7
	public static void ScaleBy(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "scale";
		args["method"] = "by";
		iTween.Launch(target, args);
	}

	// Token: 0x0600002C RID: 44 RVA: 0x000025D8 File Offset: 0x000007D8
	public static void RotateTo(GameObject target, Vector3 rotation, float time)
	{
		iTween.RotateTo(target, iTween.Hash(new object[]
		{
			"rotation",
			rotation,
			"time",
			time
		}));
	}

	// Token: 0x0600002D RID: 45 RVA: 0x00014340 File Offset: 0x00012540
	public static void RotateTo(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		if (args.Contains("rotation") && args["rotation"].GetType() == typeof(Transform))
		{
			Transform transform = (Transform)args["rotation"];
			args["position"] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			args["rotation"] = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
			args["scale"] = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
		args["type"] = "rotate";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x0600002E RID: 46 RVA: 0x0000260D File Offset: 0x0000080D
	public static void RotateFrom(GameObject target, Vector3 rotation, float time)
	{
		iTween.RotateFrom(target, iTween.Hash(new object[]
		{
			"rotation",
			rotation,
			"time",
			time
		}));
	}

	// Token: 0x0600002F RID: 47 RVA: 0x00014480 File Offset: 0x00012680
	public static void RotateFrom(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		bool flag;
		if (args.Contains("islocal"))
		{
			flag = (bool)args["islocal"];
		}
		else
		{
			flag = iTween.Defaults.isLocal;
		}
		Vector3 vector2;
		Vector3 vector;
		if (flag)
		{
			vector = (vector2 = target.transform.localEulerAngles);
		}
		else
		{
			vector = (vector2 = target.transform.eulerAngles);
		}
		if (args.Contains("rotation"))
		{
			if (args["rotation"].GetType() == typeof(Transform))
			{
				Transform transform = (Transform)args["rotation"];
				vector = transform.eulerAngles;
			}
			else if (args["rotation"].GetType() == typeof(Vector3))
			{
				vector = (Vector3)args["rotation"];
			}
		}
		else
		{
			if (args.Contains("x"))
			{
				vector.x = (float)args["x"];
			}
			if (args.Contains("y"))
			{
				vector.y = (float)args["y"];
			}
			if (args.Contains("z"))
			{
				vector.z = (float)args["z"];
			}
		}
		if (flag)
		{
			target.transform.localEulerAngles = vector;
		}
		else
		{
			target.transform.eulerAngles = vector;
		}
		args["rotation"] = vector2;
		args["type"] = "rotate";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x06000030 RID: 48 RVA: 0x00002642 File Offset: 0x00000842
	public static void RotateAdd(GameObject target, Vector3 amount, float time)
	{
		iTween.RotateAdd(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x06000031 RID: 49 RVA: 0x00002677 File Offset: 0x00000877
	public static void RotateAdd(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "rotate";
		args["method"] = "add";
		iTween.Launch(target, args);
	}

	// Token: 0x06000032 RID: 50 RVA: 0x000026A8 File Offset: 0x000008A8
	public static void RotateBy(GameObject target, Vector3 amount, float time)
	{
		iTween.RotateBy(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x06000033 RID: 51 RVA: 0x000026DD File Offset: 0x000008DD
	public static void RotateBy(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "rotate";
		args["method"] = "by";
		iTween.Launch(target, args);
	}

	// Token: 0x06000034 RID: 52 RVA: 0x0000270E File Offset: 0x0000090E
	public static void ShakePosition(GameObject target, Vector3 amount, float time)
	{
		iTween.ShakePosition(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x06000035 RID: 53 RVA: 0x00002743 File Offset: 0x00000943
	public static void ShakePosition(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "shake";
		args["method"] = "position";
		iTween.Launch(target, args);
	}

	// Token: 0x06000036 RID: 54 RVA: 0x00002774 File Offset: 0x00000974
	public static void ShakeScale(GameObject target, Vector3 amount, float time)
	{
		iTween.ShakeScale(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x06000037 RID: 55 RVA: 0x000027A9 File Offset: 0x000009A9
	public static void ShakeScale(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "shake";
		args["method"] = "scale";
		iTween.Launch(target, args);
	}

	// Token: 0x06000038 RID: 56 RVA: 0x000027DA File Offset: 0x000009DA
	public static void ShakeRotation(GameObject target, Vector3 amount, float time)
	{
		iTween.ShakeRotation(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x06000039 RID: 57 RVA: 0x0000280F File Offset: 0x00000A0F
	public static void ShakeRotation(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "shake";
		args["method"] = "rotation";
		iTween.Launch(target, args);
	}

	// Token: 0x0600003A RID: 58 RVA: 0x00002840 File Offset: 0x00000A40
	public static void PunchPosition(GameObject target, Vector3 amount, float time)
	{
		iTween.PunchPosition(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x0600003B RID: 59 RVA: 0x0001463C File Offset: 0x0001283C
	public static void PunchPosition(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "punch";
		args["method"] = "position";
		args["easetype"] = iTween.EaseType.punch;
		iTween.Launch(target, args);
	}

	// Token: 0x0600003C RID: 60 RVA: 0x00002875 File Offset: 0x00000A75
	public static void PunchRotation(GameObject target, Vector3 amount, float time)
	{
		iTween.PunchRotation(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x0600003D RID: 61 RVA: 0x0001468C File Offset: 0x0001288C
	public static void PunchRotation(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "punch";
		args["method"] = "rotation";
		args["easetype"] = iTween.EaseType.punch;
		iTween.Launch(target, args);
	}

	// Token: 0x0600003E RID: 62 RVA: 0x000028AA File Offset: 0x00000AAA
	public static void PunchScale(GameObject target, Vector3 amount, float time)
	{
		iTween.PunchScale(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x0600003F RID: 63 RVA: 0x000146DC File Offset: 0x000128DC
	public static void PunchScale(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "punch";
		args["method"] = "scale";
		args["easetype"] = iTween.EaseType.punch;
		iTween.Launch(target, args);
	}

	// Token: 0x06000040 RID: 64 RVA: 0x0001472C File Offset: 0x0001292C
	private void GenerateTargets()
	{
		string text = this.type;
		if (text != null)
		{
			if (iTween.<>f__switch$mapA == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(10);
				dictionary.Add("value", 0);
				dictionary.Add("color", 1);
				dictionary.Add("audio", 2);
				dictionary.Add("move", 3);
				dictionary.Add("scale", 4);
				dictionary.Add("rotate", 5);
				dictionary.Add("shake", 6);
				dictionary.Add("punch", 7);
				dictionary.Add("look", 8);
				dictionary.Add("stab", 9);
				iTween.<>f__switch$mapA = dictionary;
			}
			int num;
			if (iTween.<>f__switch$mapA.TryGetValue(text, ref num))
			{
				switch (num)
				{
				case 0:
				{
					string text2 = this.method;
					if (text2 != null)
					{
						if (iTween.<>f__switch$map1 == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
							dictionary.Add("float", 0);
							dictionary.Add("vector2", 1);
							dictionary.Add("vector3", 2);
							dictionary.Add("color", 3);
							dictionary.Add("rect", 4);
							iTween.<>f__switch$map1 = dictionary;
						}
						int num2;
						if (iTween.<>f__switch$map1.TryGetValue(text2, ref num2))
						{
							switch (num2)
							{
							case 0:
								this.GenerateFloatTargets();
								this.apply = new iTween.ApplyTween(this.ApplyFloatTargets);
								break;
							case 1:
								this.GenerateVector2Targets();
								this.apply = new iTween.ApplyTween(this.ApplyVector2Targets);
								break;
							case 2:
								this.GenerateVector3Targets();
								this.apply = new iTween.ApplyTween(this.ApplyVector3Targets);
								break;
							case 3:
								this.GenerateColorTargets();
								this.apply = new iTween.ApplyTween(this.ApplyColorTargets);
								break;
							case 4:
								this.GenerateRectTargets();
								this.apply = new iTween.ApplyTween(this.ApplyRectTargets);
								break;
							}
						}
					}
					break;
				}
				case 1:
				{
					string text2 = this.method;
					if (text2 != null)
					{
						if (iTween.<>f__switch$map2 == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
							dictionary.Add("to", 0);
							iTween.<>f__switch$map2 = dictionary;
						}
						int num2;
						if (iTween.<>f__switch$map2.TryGetValue(text2, ref num2))
						{
							if (num2 == 0)
							{
								this.GenerateColorToTargets();
								this.apply = new iTween.ApplyTween(this.ApplyColorToTargets);
							}
						}
					}
					break;
				}
				case 2:
				{
					string text2 = this.method;
					if (text2 != null)
					{
						if (iTween.<>f__switch$map3 == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
							dictionary.Add("to", 0);
							iTween.<>f__switch$map3 = dictionary;
						}
						int num2;
						if (iTween.<>f__switch$map3.TryGetValue(text2, ref num2))
						{
							if (num2 == 0)
							{
								this.GenerateAudioToTargets();
								this.apply = new iTween.ApplyTween(this.ApplyAudioToTargets);
							}
						}
					}
					break;
				}
				case 3:
				{
					string text2 = this.method;
					if (text2 != null)
					{
						if (iTween.<>f__switch$map4 == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
							dictionary.Add("to", 0);
							dictionary.Add("by", 1);
							dictionary.Add("add", 1);
							iTween.<>f__switch$map4 = dictionary;
						}
						int num2;
						if (iTween.<>f__switch$map4.TryGetValue(text2, ref num2))
						{
							if (num2 != 0)
							{
								if (num2 == 1)
								{
									this.GenerateMoveByTargets();
									this.apply = new iTween.ApplyTween(this.ApplyMoveByTargets);
								}
							}
							else if (this.tweenArguments.Contains("path"))
							{
								this.GenerateMoveToPathTargets();
								this.apply = new iTween.ApplyTween(this.ApplyMoveToPathTargets);
							}
							else
							{
								this.GenerateMoveToTargets();
								this.apply = new iTween.ApplyTween(this.ApplyMoveToTargets);
							}
						}
					}
					break;
				}
				case 4:
				{
					string text2 = this.method;
					if (text2 != null)
					{
						if (iTween.<>f__switch$map5 == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
							dictionary.Add("to", 0);
							dictionary.Add("by", 1);
							dictionary.Add("add", 2);
							iTween.<>f__switch$map5 = dictionary;
						}
						int num2;
						if (iTween.<>f__switch$map5.TryGetValue(text2, ref num2))
						{
							switch (num2)
							{
							case 0:
								this.GenerateScaleToTargets();
								this.apply = new iTween.ApplyTween(this.ApplyScaleToTargets);
								break;
							case 1:
								this.GenerateScaleByTargets();
								this.apply = new iTween.ApplyTween(this.ApplyScaleToTargets);
								break;
							case 2:
								this.GenerateScaleAddTargets();
								this.apply = new iTween.ApplyTween(this.ApplyScaleToTargets);
								break;
							}
						}
					}
					break;
				}
				case 5:
				{
					string text2 = this.method;
					if (text2 != null)
					{
						if (iTween.<>f__switch$map6 == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
							dictionary.Add("to", 0);
							dictionary.Add("add", 1);
							dictionary.Add("by", 2);
							iTween.<>f__switch$map6 = dictionary;
						}
						int num2;
						if (iTween.<>f__switch$map6.TryGetValue(text2, ref num2))
						{
							switch (num2)
							{
							case 0:
								this.GenerateRotateToTargets();
								this.apply = new iTween.ApplyTween(this.ApplyRotateToTargets);
								break;
							case 1:
								this.GenerateRotateAddTargets();
								this.apply = new iTween.ApplyTween(this.ApplyRotateAddTargets);
								break;
							case 2:
								this.GenerateRotateByTargets();
								this.apply = new iTween.ApplyTween(this.ApplyRotateAddTargets);
								break;
							}
						}
					}
					break;
				}
				case 6:
				{
					string text2 = this.method;
					if (text2 != null)
					{
						if (iTween.<>f__switch$map7 == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
							dictionary.Add("position", 0);
							dictionary.Add("scale", 1);
							dictionary.Add("rotation", 2);
							iTween.<>f__switch$map7 = dictionary;
						}
						int num2;
						if (iTween.<>f__switch$map7.TryGetValue(text2, ref num2))
						{
							switch (num2)
							{
							case 0:
								this.GenerateShakePositionTargets();
								this.apply = new iTween.ApplyTween(this.ApplyShakePositionTargets);
								break;
							case 1:
								this.GenerateShakeScaleTargets();
								this.apply = new iTween.ApplyTween(this.ApplyShakeScaleTargets);
								break;
							case 2:
								this.GenerateShakeRotationTargets();
								this.apply = new iTween.ApplyTween(this.ApplyShakeRotationTargets);
								break;
							}
						}
					}
					break;
				}
				case 7:
				{
					string text2 = this.method;
					if (text2 != null)
					{
						if (iTween.<>f__switch$map8 == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
							dictionary.Add("position", 0);
							dictionary.Add("rotation", 1);
							dictionary.Add("scale", 2);
							iTween.<>f__switch$map8 = dictionary;
						}
						int num2;
						if (iTween.<>f__switch$map8.TryGetValue(text2, ref num2))
						{
							switch (num2)
							{
							case 0:
								this.GeneratePunchPositionTargets();
								this.apply = new iTween.ApplyTween(this.ApplyPunchPositionTargets);
								break;
							case 1:
								this.GeneratePunchRotationTargets();
								this.apply = new iTween.ApplyTween(this.ApplyPunchRotationTargets);
								break;
							case 2:
								this.GeneratePunchScaleTargets();
								this.apply = new iTween.ApplyTween(this.ApplyPunchScaleTargets);
								break;
							}
						}
					}
					break;
				}
				case 8:
				{
					string text2 = this.method;
					if (text2 != null)
					{
						if (iTween.<>f__switch$map9 == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
							dictionary.Add("to", 0);
							iTween.<>f__switch$map9 = dictionary;
						}
						int num2;
						if (iTween.<>f__switch$map9.TryGetValue(text2, ref num2))
						{
							if (num2 == 0)
							{
								this.GenerateLookToTargets();
								this.apply = new iTween.ApplyTween(this.ApplyLookToTargets);
							}
						}
					}
					break;
				}
				case 9:
					this.GenerateStabTargets();
					this.apply = new iTween.ApplyTween(this.ApplyStabTargets);
					break;
				}
			}
		}
	}

	// Token: 0x06000041 RID: 65 RVA: 0x00014EC8 File Offset: 0x000130C8
	private void GenerateRectTargets()
	{
		this.rects = new Rect[3];
		this.rects[0] = (Rect)this.tweenArguments["from"];
		this.rects[1] = (Rect)this.tweenArguments["to"];
	}

	// Token: 0x06000042 RID: 66 RVA: 0x00014F30 File Offset: 0x00013130
	private void GenerateColorTargets()
	{
		this.colors = new Color[1, 3];
		this.colors[0, 0] = (Color)this.tweenArguments["from"];
		this.colors[0, 1] = (Color)this.tweenArguments["to"];
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00014F90 File Offset: 0x00013190
	private void GenerateVector3Targets()
	{
		this.vector3s = new Vector3[3];
		this.vector3s[0] = (Vector3)this.tweenArguments["from"];
		this.vector3s[1] = (Vector3)this.tweenArguments["to"];
		if (this.tweenArguments.Contains("speed"))
		{
			float num = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = num / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x06000044 RID: 68 RVA: 0x00015054 File Offset: 0x00013254
	private void GenerateVector2Targets()
	{
		this.vector2s = new Vector2[3];
		this.vector2s[0] = (Vector2)this.tweenArguments["from"];
		this.vector2s[1] = (Vector2)this.tweenArguments["to"];
		if (this.tweenArguments.Contains("speed"))
		{
			Vector3 vector;
			vector..ctor(this.vector2s[0].x, this.vector2s[0].y, 0f);
			Vector3 vector2;
			vector2..ctor(this.vector2s[1].x, this.vector2s[1].y, 0f);
			float num = Math.Abs(Vector3.Distance(vector, vector2));
			this.time = num / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x06000045 RID: 69 RVA: 0x00015154 File Offset: 0x00013354
	private void GenerateFloatTargets()
	{
		this.floats = new float[3];
		this.floats[0] = (float)this.tweenArguments["from"];
		this.floats[1] = (float)this.tweenArguments["to"];
		if (this.tweenArguments.Contains("speed"))
		{
			float num = Math.Abs(this.floats[0] - this.floats[1]);
			this.time = num / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x06000046 RID: 70 RVA: 0x000151F0 File Offset: 0x000133F0
	private void GenerateColorToTargets()
	{
		if (base.GetComponent(typeof(GUITexture)))
		{
			this.colors = new Color[1, 3];
			this.colors[0, 0] = (this.colors[0, 1] = base.guiTexture.color);
		}
		else if (base.GetComponent(typeof(GUIText)))
		{
			this.colors = new Color[1, 3];
			this.colors[0, 0] = (this.colors[0, 1] = base.guiText.material.color);
		}
		else if (base.renderer)
		{
			this.colors = new Color[base.renderer.materials.Length, 3];
			for (int i = 0; i < base.renderer.materials.Length; i++)
			{
				this.colors[i, 0] = base.renderer.materials[i].GetColor(this.namedcolorvalue.ToString());
				this.colors[i, 1] = base.renderer.materials[i].GetColor(this.namedcolorvalue.ToString());
			}
		}
		else if (base.light)
		{
			this.colors = new Color[1, 3];
			this.colors[0, 0] = (this.colors[0, 1] = base.light.color);
		}
		else
		{
			this.colors = new Color[1, 3];
		}
		if (this.tweenArguments.Contains("color"))
		{
			for (int j = 0; j < this.colors.GetLength(0); j++)
			{
				this.colors[j, 1] = (Color)this.tweenArguments["color"];
			}
		}
		else
		{
			if (this.tweenArguments.Contains("r"))
			{
				for (int k = 0; k < this.colors.GetLength(0); k++)
				{
					this.colors[k, 1].r = (float)this.tweenArguments["r"];
				}
			}
			if (this.tweenArguments.Contains("g"))
			{
				for (int l = 0; l < this.colors.GetLength(0); l++)
				{
					this.colors[l, 1].g = (float)this.tweenArguments["g"];
				}
			}
			if (this.tweenArguments.Contains("b"))
			{
				for (int m = 0; m < this.colors.GetLength(0); m++)
				{
					this.colors[m, 1].b = (float)this.tweenArguments["b"];
				}
			}
			if (this.tweenArguments.Contains("a"))
			{
				for (int n = 0; n < this.colors.GetLength(0); n++)
				{
					this.colors[n, 1].a = (float)this.tweenArguments["a"];
				}
			}
		}
		if (this.tweenArguments.Contains("amount"))
		{
			for (int num = 0; num < this.colors.GetLength(0); num++)
			{
				this.colors[num, 1].a = (float)this.tweenArguments["amount"];
			}
		}
		else if (this.tweenArguments.Contains("alpha"))
		{
			for (int num2 = 0; num2 < this.colors.GetLength(0); num2++)
			{
				this.colors[num2, 1].a = (float)this.tweenArguments["alpha"];
			}
		}
	}

	// Token: 0x06000047 RID: 71 RVA: 0x0001563C File Offset: 0x0001383C
	private void GenerateAudioToTargets()
	{
		this.vector2s = new Vector2[3];
		if (this.tweenArguments.Contains("audiosource"))
		{
			this.audioSource = (AudioSource)this.tweenArguments["audiosource"];
		}
		else if (base.GetComponent(typeof(AudioSource)))
		{
			this.audioSource = base.audio;
		}
		else
		{
			Debug.LogError("iTween Error: AudioTo requires an AudioSource.");
			this.Dispose();
		}
		this.vector2s[0] = (this.vector2s[1] = new Vector2(this.audioSource.volume, this.audioSource.pitch));
		if (this.tweenArguments.Contains("volume"))
		{
			this.vector2s[1].x = (float)this.tweenArguments["volume"];
		}
		if (this.tweenArguments.Contains("pitch"))
		{
			this.vector2s[1].y = (float)this.tweenArguments["pitch"];
		}
	}

	// Token: 0x06000048 RID: 72 RVA: 0x0001577C File Offset: 0x0001397C
	private void GenerateStabTargets()
	{
		if (this.tweenArguments.Contains("audiosource"))
		{
			this.audioSource = (AudioSource)this.tweenArguments["audiosource"];
		}
		else if (base.GetComponent(typeof(AudioSource)))
		{
			this.audioSource = base.audio;
		}
		else
		{
			base.gameObject.AddComponent(typeof(AudioSource));
			this.audioSource = base.audio;
			this.audioSource.playOnAwake = false;
		}
		this.audioSource.clip = (AudioClip)this.tweenArguments["audioclip"];
		if (this.tweenArguments.Contains("pitch"))
		{
			this.audioSource.pitch = (float)this.tweenArguments["pitch"];
		}
		if (this.tweenArguments.Contains("volume"))
		{
			this.audioSource.volume = (float)this.tweenArguments["volume"];
		}
		this.time = this.audioSource.clip.length / this.audioSource.pitch;
	}

	// Token: 0x06000049 RID: 73 RVA: 0x000158C4 File Offset: 0x00013AC4
	private void GenerateLookToTargets()
	{
		this.vector3s = new Vector3[3];
		this.vector3s[0] = base.transform.eulerAngles;
		if (this.tweenArguments.Contains("looktarget"))
		{
			if (this.tweenArguments["looktarget"].GetType() == typeof(Transform))
			{
				Transform transform = base.transform;
				Transform transform2 = (Transform)this.tweenArguments["looktarget"];
				Vector3? vector = (Vector3?)this.tweenArguments["up"];
				transform.LookAt(transform2, (vector == null) ? iTween.Defaults.up : vector.Value);
			}
			else if (this.tweenArguments["looktarget"].GetType() == typeof(Vector3))
			{
				Transform transform3 = base.transform;
				Vector3 vector2 = (Vector3)this.tweenArguments["looktarget"];
				Vector3? vector3 = (Vector3?)this.tweenArguments["up"];
				transform3.LookAt(vector2, (vector3 == null) ? iTween.Defaults.up : vector3.Value);
			}
		}
		else
		{
			Debug.LogError("iTween Error: LookTo needs a 'looktarget' property!");
			this.Dispose();
		}
		this.vector3s[1] = base.transform.eulerAngles;
		base.transform.eulerAngles = this.vector3s[0];
		if (this.tweenArguments.Contains("axis"))
		{
			string text = (string)this.tweenArguments["axis"];
			if (text != null)
			{
				if (iTween.<>f__switch$mapB == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
					dictionary.Add("x", 0);
					dictionary.Add("y", 1);
					dictionary.Add("z", 2);
					iTween.<>f__switch$mapB = dictionary;
				}
				int num;
				if (iTween.<>f__switch$mapB.TryGetValue(text, ref num))
				{
					switch (num)
					{
					case 0:
						this.vector3s[1].y = this.vector3s[0].y;
						this.vector3s[1].z = this.vector3s[0].z;
						break;
					case 1:
						this.vector3s[1].x = this.vector3s[0].x;
						this.vector3s[1].z = this.vector3s[0].z;
						break;
					case 2:
						this.vector3s[1].x = this.vector3s[0].x;
						this.vector3s[1].y = this.vector3s[0].y;
						break;
					}
				}
			}
		}
		this.vector3s[1] = new Vector3(this.clerp(this.vector3s[0].x, this.vector3s[1].x, 1f), this.clerp(this.vector3s[0].y, this.vector3s[1].y, 1f), this.clerp(this.vector3s[0].z, this.vector3s[1].z, 1f));
		if (this.tweenArguments.Contains("speed"))
		{
			float num2 = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = num2 / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x0600004A RID: 74 RVA: 0x00015CC0 File Offset: 0x00013EC0
	private void GenerateMoveToPathTargets()
	{
		Vector3[] array2;
		if (this.tweenArguments["path"].GetType() == typeof(Vector3[]))
		{
			Vector3[] array = (Vector3[])this.tweenArguments["path"];
			if (array.Length == 1)
			{
				Debug.LogError("iTween Error: Attempting a path movement with MoveTo requires an array of more than 1 entry!");
				this.Dispose();
			}
			array2 = new Vector3[array.Length];
			Array.Copy(array, array2, array.Length);
		}
		else
		{
			Transform[] array3 = (Transform[])this.tweenArguments["path"];
			if (array3.Length == 1)
			{
				Debug.LogError("iTween Error: Attempting a path movement with MoveTo requires an array of more than 1 entry!");
				this.Dispose();
			}
			array2 = new Vector3[array3.Length];
			for (int i = 0; i < array3.Length; i++)
			{
				array2[i] = array3[i].position;
			}
		}
		bool flag;
		int num;
		if (base.transform.position != array2[0])
		{
			if (!this.tweenArguments.Contains("movetopath") || (bool)this.tweenArguments["movetopath"])
			{
				flag = true;
				num = 3;
			}
			else
			{
				flag = false;
				num = 2;
			}
		}
		else
		{
			flag = false;
			num = 2;
		}
		this.vector3s = new Vector3[array2.Length + num];
		if (flag)
		{
			this.vector3s[1] = base.transform.position;
			num = 2;
		}
		else
		{
			num = 1;
		}
		Array.Copy(array2, 0, this.vector3s, num, array2.Length);
		this.vector3s[0] = this.vector3s[1] + (this.vector3s[1] - this.vector3s[2]);
		this.vector3s[this.vector3s.Length - 1] = this.vector3s[this.vector3s.Length - 2] + (this.vector3s[this.vector3s.Length - 2] - this.vector3s[this.vector3s.Length - 3]);
		if (this.vector3s[1] == this.vector3s[this.vector3s.Length - 2])
		{
			Vector3[] array4 = new Vector3[this.vector3s.Length];
			Array.Copy(this.vector3s, array4, this.vector3s.Length);
			array4[0] = array4[array4.Length - 3];
			array4[array4.Length - 1] = array4[2];
			this.vector3s = new Vector3[array4.Length];
			Array.Copy(array4, this.vector3s, array4.Length);
		}
		this.path = new iTween.CRSpline(this.vector3s);
		if (this.tweenArguments.Contains("speed"))
		{
			float num2 = iTween.PathLength(this.vector3s);
			this.time = num2 / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x0600004B RID: 75 RVA: 0x00016020 File Offset: 0x00014220
	private void GenerateMoveToTargets()
	{
		this.vector3s = new Vector3[3];
		if (this.isLocal)
		{
			this.vector3s[0] = (this.vector3s[1] = base.transform.localPosition);
		}
		else
		{
			this.vector3s[0] = (this.vector3s[1] = base.transform.position);
		}
		if (this.tweenArguments.Contains("position"))
		{
			if (this.tweenArguments["position"].GetType() == typeof(Transform))
			{
				Transform transform = (Transform)this.tweenArguments["position"];
				this.vector3s[1] = transform.position;
			}
			else if (this.tweenArguments["position"].GetType() == typeof(Vector3))
			{
				this.vector3s[1] = (Vector3)this.tweenArguments["position"];
			}
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
		if (this.tweenArguments.Contains("orienttopath") && (bool)this.tweenArguments["orienttopath"])
		{
			this.tweenArguments["looktarget"] = this.vector3s[1];
		}
		if (this.tweenArguments.Contains("speed"))
		{
			float num = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = num / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x0600004C RID: 76 RVA: 0x000162C8 File Offset: 0x000144C8
	private void GenerateMoveByTargets()
	{
		this.vector3s = new Vector3[6];
		this.vector3s[4] = base.transform.eulerAngles;
		this.vector3s[0] = (this.vector3s[1] = (this.vector3s[3] = base.transform.position));
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] = this.vector3s[0] + (Vector3)this.tweenArguments["amount"];
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = this.vector3s[0].x + (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = this.vector3s[0].y + (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = this.vector3s[0].z + (float)this.tweenArguments["z"];
			}
		}
		base.transform.Translate(this.vector3s[1], this.space);
		this.vector3s[5] = base.transform.position;
		base.transform.position = this.vector3s[0];
		if (this.tweenArguments.Contains("orienttopath") && (bool)this.tweenArguments["orienttopath"])
		{
			this.tweenArguments["looktarget"] = this.vector3s[1];
		}
		if (this.tweenArguments.Contains("speed"))
		{
			float num = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = num / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x0600004D RID: 77 RVA: 0x0001658C File Offset: 0x0001478C
	private void GenerateScaleToTargets()
	{
		this.vector3s = new Vector3[3];
		this.vector3s[0] = (this.vector3s[1] = base.transform.localScale);
		if (this.tweenArguments.Contains("scale"))
		{
			if (this.tweenArguments["scale"].GetType() == typeof(Transform))
			{
				Transform transform = (Transform)this.tweenArguments["scale"];
				this.vector3s[1] = transform.localScale;
			}
			else if (this.tweenArguments["scale"].GetType() == typeof(Vector3))
			{
				this.vector3s[1] = (Vector3)this.tweenArguments["scale"];
			}
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
		if (this.tweenArguments.Contains("speed"))
		{
			float num = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = num / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x0600004E RID: 78 RVA: 0x000167A0 File Offset: 0x000149A0
	private void GenerateScaleByTargets()
	{
		this.vector3s = new Vector3[3];
		this.vector3s[0] = (this.vector3s[1] = base.transform.localScale);
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] = Vector3.Scale(this.vector3s[1], (Vector3)this.tweenArguments["amount"]);
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				Vector3[] array = this.vector3s;
				int num = 1;
				array[num].x = array[num].x * (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				Vector3[] array2 = this.vector3s;
				int num2 = 1;
				array2[num2].y = array2[num2].y * (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				Vector3[] array3 = this.vector3s;
				int num3 = 1;
				array3[num3].z = array3[num3].z * (float)this.tweenArguments["z"];
			}
		}
		if (this.tweenArguments.Contains("speed"))
		{
			float num4 = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = num4 / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x0600004F RID: 79 RVA: 0x00016964 File Offset: 0x00014B64
	private void GenerateScaleAddTargets()
	{
		this.vector3s = new Vector3[3];
		this.vector3s[0] = (this.vector3s[1] = base.transform.localScale);
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] += (Vector3)this.tweenArguments["amount"];
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				Vector3[] array = this.vector3s;
				int num = 1;
				array[num].x = array[num].x + (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				Vector3[] array2 = this.vector3s;
				int num2 = 1;
				array2[num2].y = array2[num2].y + (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				Vector3[] array3 = this.vector3s;
				int num3 = 1;
				array3[num3].z = array3[num3].z + (float)this.tweenArguments["z"];
			}
		}
		if (this.tweenArguments.Contains("speed"))
		{
			float num4 = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = num4 / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x06000050 RID: 80 RVA: 0x00016B20 File Offset: 0x00014D20
	private void GenerateRotateToTargets()
	{
		this.vector3s = new Vector3[3];
		if (this.isLocal)
		{
			this.vector3s[0] = (this.vector3s[1] = base.transform.localEulerAngles);
		}
		else
		{
			this.vector3s[0] = (this.vector3s[1] = base.transform.eulerAngles);
		}
		if (this.tweenArguments.Contains("rotation"))
		{
			if (this.tweenArguments["rotation"].GetType() == typeof(Transform))
			{
				Transform transform = (Transform)this.tweenArguments["rotation"];
				this.vector3s[1] = transform.eulerAngles;
			}
			else if (this.tweenArguments["rotation"].GetType() == typeof(Vector3))
			{
				this.vector3s[1] = (Vector3)this.tweenArguments["rotation"];
			}
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
		this.vector3s[1] = new Vector3(this.clerp(this.vector3s[0].x, this.vector3s[1].x, 1f), this.clerp(this.vector3s[0].y, this.vector3s[1].y, 1f), this.clerp(this.vector3s[0].z, this.vector3s[1].z, 1f));
		if (this.tweenArguments.Contains("speed"))
		{
			float num = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = num / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x06000051 RID: 81 RVA: 0x00016E10 File Offset: 0x00015010
	private void GenerateRotateAddTargets()
	{
		this.vector3s = new Vector3[5];
		this.vector3s[0] = (this.vector3s[1] = (this.vector3s[3] = base.transform.eulerAngles));
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] += (Vector3)this.tweenArguments["amount"];
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				Vector3[] array = this.vector3s;
				int num = 1;
				array[num].x = array[num].x + (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				Vector3[] array2 = this.vector3s;
				int num2 = 1;
				array2[num2].y = array2[num2].y + (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				Vector3[] array3 = this.vector3s;
				int num3 = 1;
				array3[num3].z = array3[num3].z + (float)this.tweenArguments["z"];
			}
		}
		if (this.tweenArguments.Contains("speed"))
		{
			float num4 = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = num4 / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x06000052 RID: 82 RVA: 0x00016FE0 File Offset: 0x000151E0
	private void GenerateRotateByTargets()
	{
		this.vector3s = new Vector3[4];
		this.vector3s[0] = (this.vector3s[1] = (this.vector3s[3] = base.transform.eulerAngles));
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] += Vector3.Scale((Vector3)this.tweenArguments["amount"], new Vector3(360f, 360f, 360f));
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				Vector3[] array = this.vector3s;
				int num = 1;
				array[num].x = array[num].x + 360f * (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				Vector3[] array2 = this.vector3s;
				int num2 = 1;
				array2[num2].y = array2[num2].y + 360f * (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				Vector3[] array3 = this.vector3s;
				int num3 = 1;
				array3[num3].z = array3[num3].z + 360f * (float)this.tweenArguments["z"];
			}
		}
		if (this.tweenArguments.Contains("speed"))
		{
			float num4 = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = num4 / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x06000053 RID: 83 RVA: 0x000171D8 File Offset: 0x000153D8
	private void GenerateShakePositionTargets()
	{
		this.vector3s = new Vector3[4];
		this.vector3s[3] = base.transform.eulerAngles;
		this.vector3s[0] = base.transform.position;
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] = (Vector3)this.tweenArguments["amount"];
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
	}

	// Token: 0x06000054 RID: 84 RVA: 0x0001731C File Offset: 0x0001551C
	private void GenerateShakeScaleTargets()
	{
		this.vector3s = new Vector3[3];
		this.vector3s[0] = base.transform.localScale;
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] = (Vector3)this.tweenArguments["amount"];
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
	}

	// Token: 0x06000055 RID: 85 RVA: 0x00017444 File Offset: 0x00015644
	private void GenerateShakeRotationTargets()
	{
		this.vector3s = new Vector3[3];
		this.vector3s[0] = base.transform.eulerAngles;
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] = (Vector3)this.tweenArguments["amount"];
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
	}

	// Token: 0x06000056 RID: 86 RVA: 0x0001756C File Offset: 0x0001576C
	private void GeneratePunchPositionTargets()
	{
		this.vector3s = new Vector3[5];
		this.vector3s[4] = base.transform.eulerAngles;
		this.vector3s[0] = base.transform.position;
		this.vector3s[1] = (this.vector3s[3] = Vector3.zero);
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] = (Vector3)this.tweenArguments["amount"];
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
	}

	// Token: 0x06000057 RID: 87 RVA: 0x000176D8 File Offset: 0x000158D8
	private void GeneratePunchRotationTargets()
	{
		this.vector3s = new Vector3[4];
		this.vector3s[0] = base.transform.eulerAngles;
		this.vector3s[1] = (this.vector3s[3] = Vector3.zero);
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] = (Vector3)this.tweenArguments["amount"];
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
	}

	// Token: 0x06000058 RID: 88 RVA: 0x00017828 File Offset: 0x00015A28
	private void GeneratePunchScaleTargets()
	{
		this.vector3s = new Vector3[3];
		this.vector3s[0] = base.transform.localScale;
		this.vector3s[1] = Vector3.zero;
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] = (Vector3)this.tweenArguments["amount"];
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
	}

	// Token: 0x06000059 RID: 89 RVA: 0x00017964 File Offset: 0x00015B64
	private void ApplyRectTargets()
	{
		this.rects[2].x = this.ease(this.rects[0].x, this.rects[1].x, this.percentage);
		this.rects[2].y = this.ease(this.rects[0].y, this.rects[1].y, this.percentage);
		this.rects[2].width = this.ease(this.rects[0].width, this.rects[1].width, this.percentage);
		this.rects[2].height = this.ease(this.rects[0].height, this.rects[1].height, this.percentage);
		this.tweenArguments["onupdateparams"] = this.rects[2];
		if (this.percentage == 1f)
		{
			this.tweenArguments["onupdateparams"] = this.rects[1];
		}
	}

	// Token: 0x0600005A RID: 90 RVA: 0x00017AE0 File Offset: 0x00015CE0
	private void ApplyColorTargets()
	{
		this.colors[0, 2].r = this.ease(this.colors[0, 0].r, this.colors[0, 1].r, this.percentage);
		this.colors[0, 2].g = this.ease(this.colors[0, 0].g, this.colors[0, 1].g, this.percentage);
		this.colors[0, 2].b = this.ease(this.colors[0, 0].b, this.colors[0, 1].b, this.percentage);
		this.colors[0, 2].a = this.ease(this.colors[0, 0].a, this.colors[0, 1].a, this.percentage);
		this.tweenArguments["onupdateparams"] = this.colors[0, 2];
		if (this.percentage == 1f)
		{
			this.tweenArguments["onupdateparams"] = this.colors[0, 1];
		}
	}

	// Token: 0x0600005B RID: 91 RVA: 0x00017C60 File Offset: 0x00015E60
	private void ApplyVector3Targets()
	{
		this.vector3s[2].x = this.ease(this.vector3s[0].x, this.vector3s[1].x, this.percentage);
		this.vector3s[2].y = this.ease(this.vector3s[0].y, this.vector3s[1].y, this.percentage);
		this.vector3s[2].z = this.ease(this.vector3s[0].z, this.vector3s[1].z, this.percentage);
		this.tweenArguments["onupdateparams"] = this.vector3s[2];
		if (this.percentage == 1f)
		{
			this.tweenArguments["onupdateparams"] = this.vector3s[1];
		}
	}

	// Token: 0x0600005C RID: 92 RVA: 0x00017D98 File Offset: 0x00015F98
	private void ApplyVector2Targets()
	{
		this.vector2s[2].x = this.ease(this.vector2s[0].x, this.vector2s[1].x, this.percentage);
		this.vector2s[2].y = this.ease(this.vector2s[0].y, this.vector2s[1].y, this.percentage);
		this.tweenArguments["onupdateparams"] = this.vector2s[2];
		if (this.percentage == 1f)
		{
			this.tweenArguments["onupdateparams"] = this.vector2s[1];
		}
	}

	// Token: 0x0600005D RID: 93 RVA: 0x00017E8C File Offset: 0x0001608C
	private void ApplyFloatTargets()
	{
		this.floats[2] = this.ease(this.floats[0], this.floats[1], this.percentage);
		this.tweenArguments["onupdateparams"] = this.floats[2];
		if (this.percentage == 1f)
		{
			this.tweenArguments["onupdateparams"] = this.floats[1];
		}
	}

	// Token: 0x0600005E RID: 94 RVA: 0x00017F0C File Offset: 0x0001610C
	private void ApplyColorToTargets()
	{
		for (int i = 0; i < this.colors.GetLength(0); i++)
		{
			this.colors[i, 2].r = this.ease(this.colors[i, 0].r, this.colors[i, 1].r, this.percentage);
			this.colors[i, 2].g = this.ease(this.colors[i, 0].g, this.colors[i, 1].g, this.percentage);
			this.colors[i, 2].b = this.ease(this.colors[i, 0].b, this.colors[i, 1].b, this.percentage);
			this.colors[i, 2].a = this.ease(this.colors[i, 0].a, this.colors[i, 1].a, this.percentage);
		}
		if (base.GetComponent(typeof(GUITexture)))
		{
			base.guiTexture.color = this.colors[0, 2];
		}
		else if (base.GetComponent(typeof(GUIText)))
		{
			base.guiText.material.color = this.colors[0, 2];
		}
		else if (base.renderer)
		{
			for (int j = 0; j < this.colors.GetLength(0); j++)
			{
				base.renderer.materials[j].SetColor(this.namedcolorvalue.ToString(), this.colors[j, 2]);
			}
		}
		else if (base.light)
		{
			base.light.color = this.colors[0, 2];
		}
		if (this.percentage == 1f)
		{
			if (base.GetComponent(typeof(GUITexture)))
			{
				base.guiTexture.color = this.colors[0, 1];
			}
			else if (base.GetComponent(typeof(GUIText)))
			{
				base.guiText.material.color = this.colors[0, 1];
			}
			else if (base.renderer)
			{
				for (int k = 0; k < this.colors.GetLength(0); k++)
				{
					base.renderer.materials[k].SetColor(this.namedcolorvalue.ToString(), this.colors[k, 1]);
				}
			}
			else if (base.light)
			{
				base.light.color = this.colors[0, 1];
			}
		}
	}

	// Token: 0x0600005F RID: 95 RVA: 0x0001825C File Offset: 0x0001645C
	private void ApplyAudioToTargets()
	{
		this.vector2s[2].x = this.ease(this.vector2s[0].x, this.vector2s[1].x, this.percentage);
		this.vector2s[2].y = this.ease(this.vector2s[0].y, this.vector2s[1].y, this.percentage);
		this.audioSource.volume = this.vector2s[2].x;
		this.audioSource.pitch = this.vector2s[2].y;
		if (this.percentage == 1f)
		{
			this.audioSource.volume = this.vector2s[1].x;
			this.audioSource.pitch = this.vector2s[1].y;
		}
	}

	// Token: 0x06000060 RID: 96 RVA: 0x000028DF File Offset: 0x00000ADF
	private void ApplyStabTargets()
	{
	}

	// Token: 0x06000061 RID: 97 RVA: 0x00018374 File Offset: 0x00016574
	private void ApplyMoveToPathTargets()
	{
		this.preUpdate = base.transform.position;
		float num = this.ease(0f, 1f, this.percentage);
		if (this.isLocal)
		{
			base.transform.localPosition = this.path.Interp(Mathf.Clamp(num, 0f, 1f));
		}
		else
		{
			base.transform.position = this.path.Interp(Mathf.Clamp(num, 0f, 1f));
		}
		if (this.tweenArguments.Contains("orienttopath") && (bool)this.tweenArguments["orienttopath"])
		{
			float num2;
			if (this.tweenArguments.Contains("lookahead"))
			{
				num2 = (float)this.tweenArguments["lookahead"];
			}
			else
			{
				num2 = iTween.Defaults.lookAhead;
			}
			float num3 = this.ease(0f, 1f, Mathf.Min(1f, this.percentage + num2));
			this.tweenArguments["looktarget"] = this.path.Interp(Mathf.Clamp(num3, 0f, 1f));
		}
		this.postUpdate = base.transform.position;
		if (this.physics)
		{
			base.transform.position = this.preUpdate;
			base.rigidbody.MovePosition(this.postUpdate);
		}
	}

	// Token: 0x06000062 RID: 98 RVA: 0x00018508 File Offset: 0x00016708
	private void ApplyMoveToTargets()
	{
		this.preUpdate = base.transform.position;
		this.vector3s[2].x = this.ease(this.vector3s[0].x, this.vector3s[1].x, this.percentage);
		this.vector3s[2].y = this.ease(this.vector3s[0].y, this.vector3s[1].y, this.percentage);
		this.vector3s[2].z = this.ease(this.vector3s[0].z, this.vector3s[1].z, this.percentage);
		if (this.isLocal)
		{
			base.transform.localPosition = this.vector3s[2];
		}
		else
		{
			base.transform.position = this.vector3s[2];
		}
		if (this.percentage == 1f)
		{
			if (this.isLocal)
			{
				base.transform.localPosition = this.vector3s[1];
			}
			else
			{
				base.transform.position = this.vector3s[1];
			}
		}
		this.postUpdate = base.transform.position;
		if (this.physics)
		{
			base.transform.position = this.preUpdate;
			base.rigidbody.MovePosition(this.postUpdate);
		}
	}

	// Token: 0x06000063 RID: 99 RVA: 0x000186D0 File Offset: 0x000168D0
	private void ApplyMoveByTargets()
	{
		this.preUpdate = base.transform.position;
		Vector3 eulerAngles = default(Vector3);
		if (this.tweenArguments.Contains("looktarget"))
		{
			eulerAngles = base.transform.eulerAngles;
			base.transform.eulerAngles = this.vector3s[4];
		}
		this.vector3s[2].x = this.ease(this.vector3s[0].x, this.vector3s[1].x, this.percentage);
		this.vector3s[2].y = this.ease(this.vector3s[0].y, this.vector3s[1].y, this.percentage);
		this.vector3s[2].z = this.ease(this.vector3s[0].z, this.vector3s[1].z, this.percentage);
		base.transform.Translate(this.vector3s[2] - this.vector3s[3], this.space);
		this.vector3s[3] = this.vector3s[2];
		if (this.tweenArguments.Contains("looktarget"))
		{
			base.transform.eulerAngles = eulerAngles;
		}
		this.postUpdate = base.transform.position;
		if (this.physics)
		{
			base.transform.position = this.preUpdate;
			base.rigidbody.MovePosition(this.postUpdate);
		}
	}

	// Token: 0x06000064 RID: 100 RVA: 0x000188B8 File Offset: 0x00016AB8
	private void ApplyScaleToTargets()
	{
		this.vector3s[2].x = this.ease(this.vector3s[0].x, this.vector3s[1].x, this.percentage);
		this.vector3s[2].y = this.ease(this.vector3s[0].y, this.vector3s[1].y, this.percentage);
		this.vector3s[2].z = this.ease(this.vector3s[0].z, this.vector3s[1].z, this.percentage);
		base.transform.localScale = this.vector3s[2];
		if (this.percentage == 1f)
		{
			base.transform.localScale = this.vector3s[1];
		}
	}

	// Token: 0x06000065 RID: 101 RVA: 0x000189DC File Offset: 0x00016BDC
	private void ApplyLookToTargets()
	{
		this.vector3s[2].x = this.ease(this.vector3s[0].x, this.vector3s[1].x, this.percentage);
		this.vector3s[2].y = this.ease(this.vector3s[0].y, this.vector3s[1].y, this.percentage);
		this.vector3s[2].z = this.ease(this.vector3s[0].z, this.vector3s[1].z, this.percentage);
		if (this.isLocal)
		{
			base.transform.localRotation = Quaternion.Euler(this.vector3s[2]);
		}
		else
		{
			base.transform.rotation = Quaternion.Euler(this.vector3s[2]);
		}
	}

	// Token: 0x06000066 RID: 102 RVA: 0x00018B08 File Offset: 0x00016D08
	private void ApplyRotateToTargets()
	{
		this.preUpdate = base.transform.eulerAngles;
		this.vector3s[2].x = this.ease(this.vector3s[0].x, this.vector3s[1].x, this.percentage);
		this.vector3s[2].y = this.ease(this.vector3s[0].y, this.vector3s[1].y, this.percentage);
		this.vector3s[2].z = this.ease(this.vector3s[0].z, this.vector3s[1].z, this.percentage);
		if (this.isLocal)
		{
			base.transform.localRotation = Quaternion.Euler(this.vector3s[2]);
		}
		else
		{
			base.transform.rotation = Quaternion.Euler(this.vector3s[2]);
		}
		if (this.percentage == 1f)
		{
			if (this.isLocal)
			{
				base.transform.localRotation = Quaternion.Euler(this.vector3s[1]);
			}
			else
			{
				base.transform.rotation = Quaternion.Euler(this.vector3s[1]);
			}
		}
		this.postUpdate = base.transform.eulerAngles;
		if (this.physics)
		{
			base.transform.eulerAngles = this.preUpdate;
			base.rigidbody.MoveRotation(Quaternion.Euler(this.postUpdate));
		}
	}

	// Token: 0x06000067 RID: 103 RVA: 0x00018CEC File Offset: 0x00016EEC
	private void ApplyRotateAddTargets()
	{
		this.preUpdate = base.transform.eulerAngles;
		this.vector3s[2].x = this.ease(this.vector3s[0].x, this.vector3s[1].x, this.percentage);
		this.vector3s[2].y = this.ease(this.vector3s[0].y, this.vector3s[1].y, this.percentage);
		this.vector3s[2].z = this.ease(this.vector3s[0].z, this.vector3s[1].z, this.percentage);
		base.transform.Rotate(this.vector3s[2] - this.vector3s[3], this.space);
		this.vector3s[3] = this.vector3s[2];
		this.postUpdate = base.transform.eulerAngles;
		if (this.physics)
		{
			base.transform.eulerAngles = this.preUpdate;
			base.rigidbody.MoveRotation(Quaternion.Euler(this.postUpdate));
		}
	}

	// Token: 0x06000068 RID: 104 RVA: 0x00018E74 File Offset: 0x00017074
	private void ApplyShakePositionTargets()
	{
		this.preUpdate = base.transform.position;
		Vector3 eulerAngles = default(Vector3);
		if (this.tweenArguments.Contains("looktarget"))
		{
			eulerAngles = base.transform.eulerAngles;
			base.transform.eulerAngles = this.vector3s[3];
		}
		if (this.percentage == 0f)
		{
			base.transform.Translate(this.vector3s[1], this.space);
		}
		base.transform.position = this.vector3s[0];
		float num = 1f - this.percentage;
		this.vector3s[2].x = Random.Range(-this.vector3s[1].x * num, this.vector3s[1].x * num);
		this.vector3s[2].y = Random.Range(-this.vector3s[1].y * num, this.vector3s[1].y * num);
		this.vector3s[2].z = Random.Range(-this.vector3s[1].z * num, this.vector3s[1].z * num);
		base.transform.Translate(this.vector3s[2], this.space);
		if (this.tweenArguments.Contains("looktarget"))
		{
			base.transform.eulerAngles = eulerAngles;
		}
		this.postUpdate = base.transform.position;
		if (this.physics)
		{
			base.transform.position = this.preUpdate;
			base.rigidbody.MovePosition(this.postUpdate);
		}
	}

	// Token: 0x06000069 RID: 105 RVA: 0x0001906C File Offset: 0x0001726C
	private void ApplyShakeScaleTargets()
	{
		if (this.percentage == 0f)
		{
			base.transform.localScale = this.vector3s[1];
		}
		base.transform.localScale = this.vector3s[0];
		float num = 1f - this.percentage;
		this.vector3s[2].x = Random.Range(-this.vector3s[1].x * num, this.vector3s[1].x * num);
		this.vector3s[2].y = Random.Range(-this.vector3s[1].y * num, this.vector3s[1].y * num);
		this.vector3s[2].z = Random.Range(-this.vector3s[1].z * num, this.vector3s[1].z * num);
		base.transform.localScale += this.vector3s[2];
	}

	// Token: 0x0600006A RID: 106 RVA: 0x000191AC File Offset: 0x000173AC
	private void ApplyShakeRotationTargets()
	{
		this.preUpdate = base.transform.eulerAngles;
		if (this.percentage == 0f)
		{
			base.transform.Rotate(this.vector3s[1], this.space);
		}
		base.transform.eulerAngles = this.vector3s[0];
		float num = 1f - this.percentage;
		this.vector3s[2].x = Random.Range(-this.vector3s[1].x * num, this.vector3s[1].x * num);
		this.vector3s[2].y = Random.Range(-this.vector3s[1].y * num, this.vector3s[1].y * num);
		this.vector3s[2].z = Random.Range(-this.vector3s[1].z * num, this.vector3s[1].z * num);
		base.transform.Rotate(this.vector3s[2], this.space);
		this.postUpdate = base.transform.eulerAngles;
		if (this.physics)
		{
			base.transform.eulerAngles = this.preUpdate;
			base.rigidbody.MoveRotation(Quaternion.Euler(this.postUpdate));
		}
	}

	// Token: 0x0600006B RID: 107 RVA: 0x00019344 File Offset: 0x00017544
	private void ApplyPunchPositionTargets()
	{
		this.preUpdate = base.transform.position;
		Vector3 eulerAngles = default(Vector3);
		if (this.tweenArguments.Contains("looktarget"))
		{
			eulerAngles = base.transform.eulerAngles;
			base.transform.eulerAngles = this.vector3s[4];
		}
		if (this.vector3s[1].x > 0f)
		{
			this.vector3s[2].x = this.punch(this.vector3s[1].x, this.percentage);
		}
		else if (this.vector3s[1].x < 0f)
		{
			this.vector3s[2].x = -this.punch(Mathf.Abs(this.vector3s[1].x), this.percentage);
		}
		if (this.vector3s[1].y > 0f)
		{
			this.vector3s[2].y = this.punch(this.vector3s[1].y, this.percentage);
		}
		else if (this.vector3s[1].y < 0f)
		{
			this.vector3s[2].y = -this.punch(Mathf.Abs(this.vector3s[1].y), this.percentage);
		}
		if (this.vector3s[1].z > 0f)
		{
			this.vector3s[2].z = this.punch(this.vector3s[1].z, this.percentage);
		}
		else if (this.vector3s[1].z < 0f)
		{
			this.vector3s[2].z = -this.punch(Mathf.Abs(this.vector3s[1].z), this.percentage);
		}
		base.transform.Translate(this.vector3s[2] - this.vector3s[3], this.space);
		this.vector3s[3] = this.vector3s[2];
		if (this.tweenArguments.Contains("looktarget"))
		{
			base.transform.eulerAngles = eulerAngles;
		}
		this.postUpdate = base.transform.position;
		if (this.physics)
		{
			base.transform.position = this.preUpdate;
			base.rigidbody.MovePosition(this.postUpdate);
		}
	}

	// Token: 0x0600006C RID: 108 RVA: 0x00019638 File Offset: 0x00017838
	private void ApplyPunchRotationTargets()
	{
		this.preUpdate = base.transform.eulerAngles;
		if (this.vector3s[1].x > 0f)
		{
			this.vector3s[2].x = this.punch(this.vector3s[1].x, this.percentage);
		}
		else if (this.vector3s[1].x < 0f)
		{
			this.vector3s[2].x = -this.punch(Mathf.Abs(this.vector3s[1].x), this.percentage);
		}
		if (this.vector3s[1].y > 0f)
		{
			this.vector3s[2].y = this.punch(this.vector3s[1].y, this.percentage);
		}
		else if (this.vector3s[1].y < 0f)
		{
			this.vector3s[2].y = -this.punch(Mathf.Abs(this.vector3s[1].y), this.percentage);
		}
		if (this.vector3s[1].z > 0f)
		{
			this.vector3s[2].z = this.punch(this.vector3s[1].z, this.percentage);
		}
		else if (this.vector3s[1].z < 0f)
		{
			this.vector3s[2].z = -this.punch(Mathf.Abs(this.vector3s[1].z), this.percentage);
		}
		base.transform.Rotate(this.vector3s[2] - this.vector3s[3], this.space);
		this.vector3s[3] = this.vector3s[2];
		this.postUpdate = base.transform.eulerAngles;
		if (this.physics)
		{
			base.transform.eulerAngles = this.preUpdate;
			base.rigidbody.MoveRotation(Quaternion.Euler(this.postUpdate));
		}
	}

	// Token: 0x0600006D RID: 109 RVA: 0x000198CC File Offset: 0x00017ACC
	private void ApplyPunchScaleTargets()
	{
		if (this.vector3s[1].x > 0f)
		{
			this.vector3s[2].x = this.punch(this.vector3s[1].x, this.percentage);
		}
		else if (this.vector3s[1].x < 0f)
		{
			this.vector3s[2].x = -this.punch(Mathf.Abs(this.vector3s[1].x), this.percentage);
		}
		if (this.vector3s[1].y > 0f)
		{
			this.vector3s[2].y = this.punch(this.vector3s[1].y, this.percentage);
		}
		else if (this.vector3s[1].y < 0f)
		{
			this.vector3s[2].y = -this.punch(Mathf.Abs(this.vector3s[1].y), this.percentage);
		}
		if (this.vector3s[1].z > 0f)
		{
			this.vector3s[2].z = this.punch(this.vector3s[1].z, this.percentage);
		}
		else if (this.vector3s[1].z < 0f)
		{
			this.vector3s[2].z = -this.punch(Mathf.Abs(this.vector3s[1].z), this.percentage);
		}
		base.transform.localScale = this.vector3s[0] + this.vector3s[2];
	}

	// Token: 0x0600006E RID: 110 RVA: 0x00019AE4 File Offset: 0x00017CE4
	private IEnumerator TweenDelay()
	{
		this.delayStarted = this.timer().getTime();
		while (this.timer().getTime() - this.delayStarted < this.delay)
		{
			yield return null;
		}
		if (this.wasPaused)
		{
			this.wasPaused = false;
			this.TweenStart();
		}
		yield break;
	}

	// Token: 0x0600006F RID: 111 RVA: 0x00019B00 File Offset: 0x00017D00
	private void TweenStart()
	{
		this.CallBack("onstart");
		if (!this.loop)
		{
			this.ConflictCheck();
			this.GenerateTargets();
		}
		if (this.type == "stab")
		{
			this.audioSource.PlayOneShot(this.audioSource.clip);
		}
		if (this.type == "move" || this.type == "scale" || this.type == "rotate" || this.type == "punch" || this.type == "shake" || this.type == "curve" || this.type == "look")
		{
			this.EnableKinematic();
		}
		this.isRunning = true;
	}

	// Token: 0x06000070 RID: 112 RVA: 0x00019BFC File Offset: 0x00017DFC
	private IEnumerator TweenRestart()
	{
		if (this.delay > 0f)
		{
			this.delayStarted = this.timer().getTime();
			while (this.timer().getTime() - this.delayStarted < this.delay)
			{
				yield return null;
			}
		}
		this.loop = true;
		this.TweenStart();
		yield break;
	}

	// Token: 0x06000071 RID: 113 RVA: 0x000028E1 File Offset: 0x00000AE1
	private void TweenUpdate()
	{
		this.apply();
		this.CallBack("onupdate");
		this.UpdatePercentage();
	}

	// Token: 0x06000072 RID: 114 RVA: 0x00019C18 File Offset: 0x00017E18
	private void TweenComplete()
	{
		this.isRunning = false;
		if (this.percentage > 0.5f)
		{
			this.percentage = 1f;
		}
		else
		{
			this.percentage = 0f;
		}
		this.apply();
		if (this.type == "value")
		{
			this.CallBack("onupdate");
		}
		if (this.loopType == iTween.LoopType.none)
		{
			this.Dispose();
		}
		else
		{
			this.TweenLoop();
		}
		this.CallBack("oncomplete");
	}

	// Token: 0x06000073 RID: 115 RVA: 0x00019CAC File Offset: 0x00017EAC
	private void TweenLoop()
	{
		this.DisableKinematic();
		iTween.LoopType loopType = this.loopType;
		if (loopType != iTween.LoopType.loop)
		{
			if (loopType == iTween.LoopType.pingPong)
			{
				this.reverse = !this.reverse;
				this.runningTime = 0f;
				base.StartCoroutine("TweenRestart");
			}
		}
		else
		{
			this.percentage = 0f;
			this.runningTime = 0f;
			this.apply();
			base.StartCoroutine("TweenRestart");
		}
	}

	// Token: 0x06000074 RID: 116 RVA: 0x00019D38 File Offset: 0x00017F38
	public static Vector3 Vector3Update(Vector3 currentValue, Vector3 targetValue, float speed)
	{
		Vector3 vector = targetValue - currentValue;
		currentValue += vector * speed * iTween.sharedTimer.getDeltaFrameTime();
		return currentValue;
	}

	// Token: 0x06000075 RID: 117 RVA: 0x00019D6C File Offset: 0x00017F6C
	public static Vector2 Vector2Update(Vector2 currentValue, Vector2 targetValue, float speed)
	{
		Vector2 vector = targetValue - currentValue;
		currentValue += vector * speed * iTween.sharedTimer.getDeltaFrameTime();
		return currentValue;
	}

	// Token: 0x06000076 RID: 118 RVA: 0x00019DA0 File Offset: 0x00017FA0
	public static float FloatUpdate(float currentValue, float targetValue, float speed)
	{
		float num = targetValue - currentValue;
		currentValue += num * speed * iTween.sharedTimer.getDeltaFrameTime();
		return currentValue;
	}

	// Token: 0x06000077 RID: 119 RVA: 0x000028FF File Offset: 0x00000AFF
	public static void FadeUpdate(GameObject target, Hashtable args)
	{
		args["a"] = args["alpha"];
		iTween.ColorUpdate(target, args);
	}

	// Token: 0x06000078 RID: 120 RVA: 0x0000291E File Offset: 0x00000B1E
	public static void FadeUpdate(GameObject target, float alpha, float time)
	{
		iTween.FadeUpdate(target, iTween.Hash(new object[]
		{
			"alpha",
			alpha,
			"time",
			time
		}));
	}

	// Token: 0x06000079 RID: 121 RVA: 0x00019DC4 File Offset: 0x00017FC4
	public static void ColorUpdate(GameObject target, Hashtable args)
	{
		iTween.CleanArgs(args);
		Color[] array = new Color[4];
		if (!args.Contains("includechildren") || (bool)args["includechildren"])
		{
			foreach (object obj in target.transform)
			{
				Transform transform = (Transform)obj;
				iTween.ColorUpdate(transform.gameObject, args);
			}
		}
		float num;
		if (args.Contains("time"))
		{
			num = (float)args["time"];
			num *= iTween.Defaults.updateTimePercentage;
		}
		else
		{
			num = iTween.Defaults.updateTime;
		}
		if (target.GetComponent(typeof(GUITexture)))
		{
			array[0] = (array[1] = target.guiTexture.color);
		}
		else if (target.GetComponent(typeof(GUIText)))
		{
			array[0] = (array[1] = target.guiText.material.color);
		}
		else if (target.renderer)
		{
			array[0] = (array[1] = target.renderer.material.color);
		}
		else if (target.light)
		{
			array[0] = (array[1] = target.light.color);
		}
		if (args.Contains("color"))
		{
			array[1] = (Color)args["color"];
		}
		else
		{
			if (args.Contains("r"))
			{
				array[1].r = (float)args["r"];
			}
			if (args.Contains("g"))
			{
				array[1].g = (float)args["g"];
			}
			if (args.Contains("b"))
			{
				array[1].b = (float)args["b"];
			}
			if (args.Contains("a"))
			{
				array[1].a = (float)args["a"];
			}
		}
		array[3].r = Mathf.SmoothDamp(array[0].r, array[1].r, ref array[2].r, num);
		array[3].g = Mathf.SmoothDamp(array[0].g, array[1].g, ref array[2].g, num);
		array[3].b = Mathf.SmoothDamp(array[0].b, array[1].b, ref array[2].b, num);
		array[3].a = Mathf.SmoothDamp(array[0].a, array[1].a, ref array[2].a, num);
		if (target.GetComponent(typeof(GUITexture)))
		{
			target.guiTexture.color = array[3];
		}
		else if (target.GetComponent(typeof(GUIText)))
		{
			target.guiText.material.color = array[3];
		}
		else if (target.renderer)
		{
			target.renderer.material.color = array[3];
		}
		else if (target.light)
		{
			target.light.color = array[3];
		}
	}

	// Token: 0x0600007A RID: 122 RVA: 0x00002953 File Offset: 0x00000B53
	public static void ColorUpdate(GameObject target, Color color, float time)
	{
		iTween.ColorUpdate(target, iTween.Hash(new object[]
		{
			"color",
			color,
			"time",
			time
		}));
	}

	// Token: 0x0600007B RID: 123 RVA: 0x0001A228 File Offset: 0x00018428
	public static void AudioUpdate(GameObject target, Hashtable args)
	{
		iTween.CleanArgs(args);
		Vector2[] array = new Vector2[4];
		float num;
		if (args.Contains("time"))
		{
			num = (float)args["time"];
			num *= iTween.Defaults.updateTimePercentage;
		}
		else
		{
			num = iTween.Defaults.updateTime;
		}
		AudioSource audioSource;
		if (args.Contains("audiosource"))
		{
			audioSource = (AudioSource)args["audiosource"];
		}
		else
		{
			if (!target.GetComponent(typeof(AudioSource)))
			{
				Debug.LogError("iTween Error: AudioUpdate requires an AudioSource.");
				return;
			}
			audioSource = target.audio;
		}
		array[0] = (array[1] = new Vector2(audioSource.volume, audioSource.pitch));
		if (args.Contains("volume"))
		{
			array[1].x = (float)args["volume"];
		}
		if (args.Contains("pitch"))
		{
			array[1].y = (float)args["pitch"];
		}
		array[3].x = Mathf.SmoothDampAngle(array[0].x, array[1].x, ref array[2].x, num);
		array[3].y = Mathf.SmoothDampAngle(array[0].y, array[1].y, ref array[2].y, num);
		audioSource.volume = array[3].x;
		audioSource.pitch = array[3].y;
	}

	// Token: 0x0600007C RID: 124 RVA: 0x0001A3E4 File Offset: 0x000185E4
	public static void AudioUpdate(GameObject target, float volume, float pitch, float time)
	{
		iTween.AudioUpdate(target, iTween.Hash(new object[]
		{
			"volume",
			volume,
			"pitch",
			pitch,
			"time",
			time
		}));
	}

	// Token: 0x0600007D RID: 125 RVA: 0x0001A438 File Offset: 0x00018638
	public static void RotateUpdate(GameObject target, Hashtable args)
	{
		iTween.CleanArgs(args);
		Vector3[] array = new Vector3[4];
		Vector3 eulerAngles = target.transform.eulerAngles;
		float num;
		if (args.Contains("time"))
		{
			num = (float)args["time"];
			num *= iTween.Defaults.updateTimePercentage;
		}
		else
		{
			num = iTween.Defaults.updateTime;
		}
		bool flag;
		if (args.Contains("islocal"))
		{
			flag = (bool)args["islocal"];
		}
		else
		{
			flag = iTween.Defaults.isLocal;
		}
		if (flag)
		{
			array[0] = target.transform.localEulerAngles;
		}
		else
		{
			array[0] = target.transform.eulerAngles;
		}
		if (args.Contains("rotation"))
		{
			if (args["rotation"].GetType() == typeof(Transform))
			{
				Transform transform = (Transform)args["rotation"];
				array[1] = transform.eulerAngles;
			}
			else if (args["rotation"].GetType() == typeof(Vector3))
			{
				array[1] = (Vector3)args["rotation"];
			}
		}
		array[3].x = Mathf.SmoothDampAngle(array[0].x, array[1].x, ref array[2].x, num);
		array[3].y = Mathf.SmoothDampAngle(array[0].y, array[1].y, ref array[2].y, num);
		array[3].z = Mathf.SmoothDampAngle(array[0].z, array[1].z, ref array[2].z, num);
		if (flag)
		{
			target.transform.localEulerAngles = array[3];
		}
		else
		{
			target.transform.eulerAngles = array[3];
		}
		if (target.rigidbody != null)
		{
			Vector3 eulerAngles2 = target.transform.eulerAngles;
			target.transform.eulerAngles = eulerAngles;
			target.rigidbody.MoveRotation(Quaternion.Euler(eulerAngles2));
		}
	}

	// Token: 0x0600007E RID: 126 RVA: 0x00002988 File Offset: 0x00000B88
	public static void RotateUpdate(GameObject target, Vector3 rotation, float time)
	{
		iTween.RotateUpdate(target, iTween.Hash(new object[]
		{
			"rotation",
			rotation,
			"time",
			time
		}));
	}

	// Token: 0x0600007F RID: 127 RVA: 0x0001A6A4 File Offset: 0x000188A4
	public static void ScaleUpdate(GameObject target, Hashtable args)
	{
		iTween.CleanArgs(args);
		Vector3[] array = new Vector3[4];
		float num;
		if (args.Contains("time"))
		{
			num = (float)args["time"];
			num *= iTween.Defaults.updateTimePercentage;
		}
		else
		{
			num = iTween.Defaults.updateTime;
		}
		array[0] = (array[1] = target.transform.localScale);
		if (args.Contains("scale"))
		{
			if (args["scale"].GetType() == typeof(Transform))
			{
				Transform transform = (Transform)args["scale"];
				array[1] = transform.localScale;
			}
			else if (args["scale"].GetType() == typeof(Vector3))
			{
				array[1] = (Vector3)args["scale"];
			}
		}
		else
		{
			if (args.Contains("x"))
			{
				array[1].x = (float)args["x"];
			}
			if (args.Contains("y"))
			{
				array[1].y = (float)args["y"];
			}
			if (args.Contains("z"))
			{
				array[1].z = (float)args["z"];
			}
		}
		array[3].x = Mathf.SmoothDamp(array[0].x, array[1].x, ref array[2].x, num);
		array[3].y = Mathf.SmoothDamp(array[0].y, array[1].y, ref array[2].y, num);
		array[3].z = Mathf.SmoothDamp(array[0].z, array[1].z, ref array[2].z, num);
		target.transform.localScale = array[3];
	}

	// Token: 0x06000080 RID: 128 RVA: 0x000029BD File Offset: 0x00000BBD
	public static void ScaleUpdate(GameObject target, Vector3 scale, float time)
	{
		iTween.ScaleUpdate(target, iTween.Hash(new object[]
		{
			"scale",
			scale,
			"time",
			time
		}));
	}

	// Token: 0x06000081 RID: 129 RVA: 0x0001A8F0 File Offset: 0x00018AF0
	public static void MoveUpdate(GameObject target, Hashtable args)
	{
		iTween.CleanArgs(args);
		Vector3[] array = new Vector3[4];
		Vector3 position = target.transform.position;
		float num;
		if (args.Contains("time"))
		{
			num = (float)args["time"];
			num *= iTween.Defaults.updateTimePercentage;
		}
		else
		{
			num = iTween.Defaults.updateTime;
		}
		bool flag;
		if (args.Contains("islocal"))
		{
			flag = (bool)args["islocal"];
		}
		else
		{
			flag = iTween.Defaults.isLocal;
		}
		if (flag)
		{
			array[0] = (array[1] = target.transform.localPosition);
		}
		else
		{
			array[0] = (array[1] = target.transform.position);
		}
		if (args.Contains("position"))
		{
			if (args["position"].GetType() == typeof(Transform))
			{
				Transform transform = (Transform)args["position"];
				array[1] = transform.position;
			}
			else if (args["position"].GetType() == typeof(Vector3))
			{
				array[1] = (Vector3)args["position"];
			}
		}
		else
		{
			if (args.Contains("x"))
			{
				array[1].x = (float)args["x"];
			}
			if (args.Contains("y"))
			{
				array[1].y = (float)args["y"];
			}
			if (args.Contains("z"))
			{
				array[1].z = (float)args["z"];
			}
		}
		array[3].x = Mathf.SmoothDamp(array[0].x, array[1].x, ref array[2].x, num);
		array[3].y = Mathf.SmoothDamp(array[0].y, array[1].y, ref array[2].y, num);
		array[3].z = Mathf.SmoothDamp(array[0].z, array[1].z, ref array[2].z, num);
		if (args.Contains("orienttopath") && (bool)args["orienttopath"])
		{
			args["looktarget"] = array[3];
		}
		if (args.Contains("looktarget"))
		{
			iTween.LookUpdate(target, args);
		}
		if (flag)
		{
			target.transform.localPosition = array[3];
		}
		else
		{
			target.transform.position = array[3];
		}
		if (target.rigidbody != null)
		{
			Vector3 position2 = target.transform.position;
			target.transform.position = position;
			target.rigidbody.MovePosition(position2);
		}
	}

	// Token: 0x06000082 RID: 130 RVA: 0x000029F2 File Offset: 0x00000BF2
	public static void MoveUpdate(GameObject target, Vector3 position, float time)
	{
		iTween.MoveUpdate(target, iTween.Hash(new object[]
		{
			"position",
			position,
			"time",
			time
		}));
	}

	// Token: 0x06000083 RID: 131 RVA: 0x0001AC5C File Offset: 0x00018E5C
	public static void LookUpdate(GameObject target, Hashtable args)
	{
		iTween.CleanArgs(args);
		Vector3[] array = new Vector3[5];
		float num;
		if (args.Contains("looktime"))
		{
			num = (float)args["looktime"];
			num *= iTween.Defaults.updateTimePercentage;
		}
		else if (args.Contains("time"))
		{
			num = (float)args["time"] / 2f;
			num *= iTween.Defaults.updateTimePercentage;
		}
		else
		{
			num = iTween.Defaults.updateTime;
		}
		array[0] = target.transform.eulerAngles;
		if (args.Contains("looktarget"))
		{
			if (args["looktarget"].GetType() == typeof(Transform))
			{
				Transform transform = target.transform;
				Transform transform2 = (Transform)args["looktarget"];
				Vector3? vector = (Vector3?)args["up"];
				transform.LookAt(transform2, (vector == null) ? iTween.Defaults.up : vector.Value);
			}
			else if (args["looktarget"].GetType() == typeof(Vector3))
			{
				Transform transform3 = target.transform;
				Vector3 vector2 = (Vector3)args["looktarget"];
				Vector3? vector3 = (Vector3?)args["up"];
				transform3.LookAt(vector2, (vector3 == null) ? iTween.Defaults.up : vector3.Value);
			}
			array[1] = target.transform.eulerAngles;
			target.transform.eulerAngles = array[0];
			array[3].x = Mathf.SmoothDampAngle(array[0].x, array[1].x, ref array[2].x, num);
			array[3].y = Mathf.SmoothDampAngle(array[0].y, array[1].y, ref array[2].y, num);
			array[3].z = Mathf.SmoothDampAngle(array[0].z, array[1].z, ref array[2].z, num);
			target.transform.eulerAngles = array[3];
			if (args.Contains("axis"))
			{
				array[4] = target.transform.eulerAngles;
				string text = (string)args["axis"];
				if (text != null)
				{
					if (iTween.<>f__switch$mapC == null)
					{
						Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
						dictionary.Add("x", 0);
						dictionary.Add("y", 1);
						dictionary.Add("z", 2);
						iTween.<>f__switch$mapC = dictionary;
					}
					int num2;
					if (iTween.<>f__switch$mapC.TryGetValue(text, ref num2))
					{
						switch (num2)
						{
						case 0:
							array[4].y = array[0].y;
							array[4].z = array[0].z;
							break;
						case 1:
							array[4].x = array[0].x;
							array[4].z = array[0].z;
							break;
						case 2:
							array[4].x = array[0].x;
							array[4].y = array[0].y;
							break;
						}
					}
				}
				target.transform.eulerAngles = array[4];
			}
			return;
		}
		Debug.LogError("iTween Error: LookUpdate needs a 'looktarget' property!");
	}

	// Token: 0x06000084 RID: 132 RVA: 0x00002A27 File Offset: 0x00000C27
	public static void LookUpdate(GameObject target, Vector3 looktarget, float time)
	{
		iTween.LookUpdate(target, iTween.Hash(new object[]
		{
			"looktarget",
			looktarget,
			"time",
			time
		}));
	}

	// Token: 0x06000085 RID: 133 RVA: 0x0001B034 File Offset: 0x00019234
	public static float PathLength(Transform[] path)
	{
		Vector3[] array = new Vector3[path.Length];
		float num = 0f;
		for (int i = 0; i < path.Length; i++)
		{
			array[i] = path[i].position;
		}
		Vector3[] pts = iTween.PathControlPointGenerator(array);
		Vector3 vector = iTween.Interp(pts, 0f);
		int num2 = path.Length * 20;
		for (int j = 1; j <= num2; j++)
		{
			float t = (float)j / (float)num2;
			Vector3 vector2 = iTween.Interp(pts, t);
			num += Vector3.Distance(vector, vector2);
			vector = vector2;
		}
		return num;
	}

	// Token: 0x06000086 RID: 134 RVA: 0x0001B0D0 File Offset: 0x000192D0
	public static float PathLength(Vector3[] path)
	{
		float num = 0f;
		Vector3[] pts = iTween.PathControlPointGenerator(path);
		Vector3 vector = iTween.Interp(pts, 0f);
		int num2 = path.Length * 20;
		for (int i = 1; i <= num2; i++)
		{
			float t = (float)i / (float)num2;
			Vector3 vector2 = iTween.Interp(pts, t);
			num += Vector3.Distance(vector, vector2);
			vector = vector2;
		}
		return num;
	}

	// Token: 0x06000087 RID: 135 RVA: 0x0001B134 File Offset: 0x00019334
	public static Texture2D CameraTexture(Color color)
	{
		Texture2D texture2D = new Texture2D(Screen.width, Screen.height, 5, false);
		Color[] array = new Color[Screen.width * Screen.height];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = color;
		}
		texture2D.SetPixels(array);
		texture2D.Apply();
		return texture2D;
	}

	// Token: 0x06000088 RID: 136 RVA: 0x00002A5C File Offset: 0x00000C5C
	public static void PutOnPath(GameObject target, Vector3[] path, float percent)
	{
		target.transform.position = iTween.Interp(iTween.PathControlPointGenerator(path), percent);
	}

	// Token: 0x06000089 RID: 137 RVA: 0x00002A75 File Offset: 0x00000C75
	public static void PutOnPath(Transform target, Vector3[] path, float percent)
	{
		target.position = iTween.Interp(iTween.PathControlPointGenerator(path), percent);
	}

	// Token: 0x0600008A RID: 138 RVA: 0x0001B194 File Offset: 0x00019394
	public static void PutOnPath(GameObject target, Transform[] path, float percent)
	{
		Vector3[] array = new Vector3[path.Length];
		for (int i = 0; i < path.Length; i++)
		{
			array[i] = path[i].position;
		}
		target.transform.position = iTween.Interp(iTween.PathControlPointGenerator(array), percent);
	}

	// Token: 0x0600008B RID: 139 RVA: 0x0001B1EC File Offset: 0x000193EC
	public static void PutOnPath(Transform target, Transform[] path, float percent)
	{
		Vector3[] array = new Vector3[path.Length];
		for (int i = 0; i < path.Length; i++)
		{
			array[i] = path[i].position;
		}
		target.position = iTween.Interp(iTween.PathControlPointGenerator(array), percent);
	}

	// Token: 0x0600008C RID: 140 RVA: 0x0001B23C File Offset: 0x0001943C
	public static Vector3 PointOnPath(Transform[] path, float percent)
	{
		Vector3[] array = new Vector3[path.Length];
		for (int i = 0; i < path.Length; i++)
		{
			array[i] = path[i].position;
		}
		return iTween.Interp(iTween.PathControlPointGenerator(array), percent);
	}

	// Token: 0x0600008D RID: 141 RVA: 0x00002A89 File Offset: 0x00000C89
	public static void DrawLine(Vector3[] line)
	{
		if (line.Length > 0)
		{
			iTween.DrawLineHelper(line, iTween.Defaults.color, "gizmos");
		}
	}

	// Token: 0x0600008E RID: 142 RVA: 0x00002AA4 File Offset: 0x00000CA4
	public static void DrawLine(Vector3[] line, Color color)
	{
		if (line.Length > 0)
		{
			iTween.DrawLineHelper(line, color, "gizmos");
		}
	}

	// Token: 0x0600008F RID: 143 RVA: 0x0001B288 File Offset: 0x00019488
	public static void DrawLine(Transform[] line)
	{
		if (line.Length > 0)
		{
			Vector3[] array = new Vector3[line.Length];
			for (int i = 0; i < line.Length; i++)
			{
				array[i] = line[i].position;
			}
			iTween.DrawLineHelper(array, iTween.Defaults.color, "gizmos");
		}
	}

	// Token: 0x06000090 RID: 144 RVA: 0x0001B2E0 File Offset: 0x000194E0
	public static void DrawLine(Transform[] line, Color color)
	{
		if (line.Length > 0)
		{
			Vector3[] array = new Vector3[line.Length];
			for (int i = 0; i < line.Length; i++)
			{
				array[i] = line[i].position;
			}
			iTween.DrawLineHelper(array, color, "gizmos");
		}
	}

	// Token: 0x06000091 RID: 145 RVA: 0x00002A89 File Offset: 0x00000C89
	public static void DrawLineGizmos(Vector3[] line)
	{
		if (line.Length > 0)
		{
			iTween.DrawLineHelper(line, iTween.Defaults.color, "gizmos");
		}
	}

	// Token: 0x06000092 RID: 146 RVA: 0x00002AA4 File Offset: 0x00000CA4
	public static void DrawLineGizmos(Vector3[] line, Color color)
	{
		if (line.Length > 0)
		{
			iTween.DrawLineHelper(line, color, "gizmos");
		}
	}

	// Token: 0x06000093 RID: 147 RVA: 0x0001B288 File Offset: 0x00019488
	public static void DrawLineGizmos(Transform[] line)
	{
		if (line.Length > 0)
		{
			Vector3[] array = new Vector3[line.Length];
			for (int i = 0; i < line.Length; i++)
			{
				array[i] = line[i].position;
			}
			iTween.DrawLineHelper(array, iTween.Defaults.color, "gizmos");
		}
	}

	// Token: 0x06000094 RID: 148 RVA: 0x0001B2E0 File Offset: 0x000194E0
	public static void DrawLineGizmos(Transform[] line, Color color)
	{
		if (line.Length > 0)
		{
			Vector3[] array = new Vector3[line.Length];
			for (int i = 0; i < line.Length; i++)
			{
				array[i] = line[i].position;
			}
			iTween.DrawLineHelper(array, color, "gizmos");
		}
	}

	// Token: 0x06000095 RID: 149 RVA: 0x00002ABB File Offset: 0x00000CBB
	public static void DrawLineHandles(Vector3[] line)
	{
		if (line.Length > 0)
		{
			iTween.DrawLineHelper(line, iTween.Defaults.color, "handles");
		}
	}

	// Token: 0x06000096 RID: 150 RVA: 0x00002AD6 File Offset: 0x00000CD6
	public static void DrawLineHandles(Vector3[] line, Color color)
	{
		if (line.Length > 0)
		{
			iTween.DrawLineHelper(line, color, "handles");
		}
	}

	// Token: 0x06000097 RID: 151 RVA: 0x0001B334 File Offset: 0x00019534
	public static void DrawLineHandles(Transform[] line)
	{
		if (line.Length > 0)
		{
			Vector3[] array = new Vector3[line.Length];
			for (int i = 0; i < line.Length; i++)
			{
				array[i] = line[i].position;
			}
			iTween.DrawLineHelper(array, iTween.Defaults.color, "handles");
		}
	}

	// Token: 0x06000098 RID: 152 RVA: 0x0001B38C File Offset: 0x0001958C
	public static void DrawLineHandles(Transform[] line, Color color)
	{
		if (line.Length > 0)
		{
			Vector3[] array = new Vector3[line.Length];
			for (int i = 0; i < line.Length; i++)
			{
				array[i] = line[i].position;
			}
			iTween.DrawLineHelper(array, color, "handles");
		}
	}

	// Token: 0x06000099 RID: 153 RVA: 0x00002AED File Offset: 0x00000CED
	public static Vector3 PointOnPath(Vector3[] path, float percent)
	{
		return iTween.Interp(iTween.PathControlPointGenerator(path), percent);
	}

	// Token: 0x0600009A RID: 154 RVA: 0x00002AFB File Offset: 0x00000CFB
	public static void DrawPath(Vector3[] path)
	{
		if (path.Length > 0)
		{
			iTween.DrawPathHelper(path, iTween.Defaults.color, "gizmos");
		}
	}

	// Token: 0x0600009B RID: 155 RVA: 0x00002B16 File Offset: 0x00000D16
	public static void DrawPath(Vector3[] path, Color color)
	{
		if (path.Length > 0)
		{
			iTween.DrawPathHelper(path, color, "gizmos");
		}
	}

	// Token: 0x0600009C RID: 156 RVA: 0x0001B3E0 File Offset: 0x000195E0
	public static void DrawPath(Transform[] path)
	{
		if (path.Length > 0)
		{
			Vector3[] array = new Vector3[path.Length];
			for (int i = 0; i < path.Length; i++)
			{
				array[i] = path[i].position;
			}
			iTween.DrawPathHelper(array, iTween.Defaults.color, "gizmos");
		}
	}

	// Token: 0x0600009D RID: 157 RVA: 0x0001B438 File Offset: 0x00019638
	public static void DrawPath(Transform[] path, Color color)
	{
		if (path.Length > 0)
		{
			Vector3[] array = new Vector3[path.Length];
			for (int i = 0; i < path.Length; i++)
			{
				array[i] = path[i].position;
			}
			iTween.DrawPathHelper(array, color, "gizmos");
		}
	}

	// Token: 0x0600009E RID: 158 RVA: 0x00002AFB File Offset: 0x00000CFB
	public static void DrawPathGizmos(Vector3[] path)
	{
		if (path.Length > 0)
		{
			iTween.DrawPathHelper(path, iTween.Defaults.color, "gizmos");
		}
	}

	// Token: 0x0600009F RID: 159 RVA: 0x00002B16 File Offset: 0x00000D16
	public static void DrawPathGizmos(Vector3[] path, Color color)
	{
		if (path.Length > 0)
		{
			iTween.DrawPathHelper(path, color, "gizmos");
		}
	}

	// Token: 0x060000A0 RID: 160 RVA: 0x0001B3E0 File Offset: 0x000195E0
	public static void DrawPathGizmos(Transform[] path)
	{
		if (path.Length > 0)
		{
			Vector3[] array = new Vector3[path.Length];
			for (int i = 0; i < path.Length; i++)
			{
				array[i] = path[i].position;
			}
			iTween.DrawPathHelper(array, iTween.Defaults.color, "gizmos");
		}
	}

	// Token: 0x060000A1 RID: 161 RVA: 0x0001B438 File Offset: 0x00019638
	public static void DrawPathGizmos(Transform[] path, Color color)
	{
		if (path.Length > 0)
		{
			Vector3[] array = new Vector3[path.Length];
			for (int i = 0; i < path.Length; i++)
			{
				array[i] = path[i].position;
			}
			iTween.DrawPathHelper(array, color, "gizmos");
		}
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x00002B2D File Offset: 0x00000D2D
	public static void DrawPathHandles(Vector3[] path)
	{
		if (path.Length > 0)
		{
			iTween.DrawPathHelper(path, iTween.Defaults.color, "handles");
		}
	}

	// Token: 0x060000A3 RID: 163 RVA: 0x00002B48 File Offset: 0x00000D48
	public static void DrawPathHandles(Vector3[] path, Color color)
	{
		if (path.Length > 0)
		{
			iTween.DrawPathHelper(path, color, "handles");
		}
	}

	// Token: 0x060000A4 RID: 164 RVA: 0x0001B48C File Offset: 0x0001968C
	public static void DrawPathHandles(Transform[] path)
	{
		if (path.Length > 0)
		{
			Vector3[] array = new Vector3[path.Length];
			for (int i = 0; i < path.Length; i++)
			{
				array[i] = path[i].position;
			}
			iTween.DrawPathHelper(array, iTween.Defaults.color, "handles");
		}
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x0001B4E4 File Offset: 0x000196E4
	public static void DrawPathHandles(Transform[] path, Color color)
	{
		if (path.Length > 0)
		{
			Vector3[] array = new Vector3[path.Length];
			for (int i = 0; i < path.Length; i++)
			{
				array[i] = path[i].position;
			}
			iTween.DrawPathHelper(array, color, "handles");
		}
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x0001B538 File Offset: 0x00019738
	public static void CameraFadeDepth(int depth)
	{
		if (iTween.cameraFade)
		{
			iTween.cameraFade.transform.position = new Vector3(iTween.cameraFade.transform.position.x, iTween.cameraFade.transform.position.y, (float)depth);
		}
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x00002B5F File Offset: 0x00000D5F
	public static void CameraFadeDestroy()
	{
		if (iTween.cameraFade)
		{
			Object.Destroy(iTween.cameraFade);
		}
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x00002B7A File Offset: 0x00000D7A
	public static void CameraFadeSwap(Texture2D texture)
	{
		if (iTween.cameraFade)
		{
			iTween.cameraFade.guiTexture.texture = texture;
		}
	}

	// Token: 0x060000A9 RID: 169 RVA: 0x0001B598 File Offset: 0x00019798
	public static GameObject CameraFadeAdd(Texture2D texture, int depth)
	{
		if (iTween.cameraFade)
		{
			return null;
		}
		iTween.cameraFade = new GameObject("iTween Camera Fade");
		iTween.cameraFade.transform.position = new Vector3(0.5f, 0.5f, (float)depth);
		iTween.cameraFade.AddComponent("GUITexture");
		iTween.cameraFade.guiTexture.texture = texture;
		iTween.cameraFade.guiTexture.color = new Color(0.5f, 0.5f, 0.5f, 0f);
		return iTween.cameraFade;
	}

	// Token: 0x060000AA RID: 170 RVA: 0x0001B634 File Offset: 0x00019834
	public static GameObject CameraFadeAdd(Texture2D texture)
	{
		if (iTween.cameraFade)
		{
			return null;
		}
		iTween.cameraFade = new GameObject("iTween Camera Fade");
		iTween.cameraFade.transform.position = new Vector3(0.5f, 0.5f, (float)iTween.Defaults.cameraFadeDepth);
		iTween.cameraFade.AddComponent("GUITexture");
		iTween.cameraFade.guiTexture.texture = texture;
		iTween.cameraFade.guiTexture.color = new Color(0.5f, 0.5f, 0.5f, 0f);
		return iTween.cameraFade;
	}

	// Token: 0x060000AB RID: 171 RVA: 0x0001B6D4 File Offset: 0x000198D4
	public static GameObject CameraFadeAdd()
	{
		if (iTween.cameraFade)
		{
			return null;
		}
		iTween.cameraFade = new GameObject("iTween Camera Fade");
		iTween.cameraFade.transform.position = new Vector3(0.5f, 0.5f, (float)iTween.Defaults.cameraFadeDepth);
		iTween.cameraFade.AddComponent("GUITexture");
		iTween.cameraFade.guiTexture.texture = iTween.CameraTexture(Color.black);
		iTween.cameraFade.guiTexture.color = new Color(0.5f, 0.5f, 0.5f, 0f);
		return iTween.cameraFade;
	}

	// Token: 0x060000AC RID: 172 RVA: 0x0001B77C File Offset: 0x0001997C
	public static void Resume(GameObject target)
	{
		Component[] components = target.GetComponents(typeof(iTween));
		foreach (iTween iTween in components)
		{
			iTween.enabled = true;
		}
	}

	// Token: 0x060000AD RID: 173 RVA: 0x0001B7C0 File Offset: 0x000199C0
	public static void Resume(GameObject target, bool includechildren)
	{
		iTween.Resume(target);
		if (includechildren)
		{
			foreach (object obj in target.transform)
			{
				Transform transform = (Transform)obj;
				iTween.Resume(transform.gameObject, true);
			}
		}
	}

	// Token: 0x060000AE RID: 174 RVA: 0x0001B834 File Offset: 0x00019A34
	public static void Resume(GameObject target, string type)
	{
		Component[] components = target.GetComponents(typeof(iTween));
		foreach (iTween iTween in components)
		{
			string text = iTween.type + iTween.method;
			text = text.Substring(0, type.Length);
			if (text.ToLower() == type.ToLower())
			{
				iTween.enabled = true;
			}
		}
	}

	// Token: 0x060000AF RID: 175 RVA: 0x0001B8B4 File Offset: 0x00019AB4
	public static void Resume(GameObject target, string type, bool includechildren)
	{
		Component[] components = target.GetComponents(typeof(iTween));
		foreach (iTween iTween in components)
		{
			string text = iTween.type + iTween.method;
			text = text.Substring(0, type.Length);
			if (text.ToLower() == type.ToLower())
			{
				iTween.enabled = true;
			}
		}
		if (includechildren)
		{
			foreach (object obj in target.transform)
			{
				Transform transform = (Transform)obj;
				iTween.Resume(transform.gameObject, type, true);
			}
		}
	}

	// Token: 0x060000B0 RID: 176 RVA: 0x0001B9A0 File Offset: 0x00019BA0
	public static void Resume()
	{
		for (int i = 0; i < iTween.tweens.Count; i++)
		{
			Hashtable hashtable = (Hashtable)iTween.tweens[i];
			GameObject target = (GameObject)hashtable["target"];
			iTween.Resume(target);
		}
	}

	// Token: 0x060000B1 RID: 177 RVA: 0x0001B9F0 File Offset: 0x00019BF0
	public static void Resume(string type)
	{
		ArrayList arrayList = new ArrayList();
		for (int i = 0; i < iTween.tweens.Count; i++)
		{
			Hashtable hashtable = (Hashtable)iTween.tweens[i];
			GameObject gameObject = (GameObject)hashtable["target"];
			arrayList.Insert(arrayList.Count, gameObject);
		}
		for (int j = 0; j < arrayList.Count; j++)
		{
			iTween.Resume((GameObject)arrayList[j], type);
		}
	}

	// Token: 0x060000B2 RID: 178 RVA: 0x0001BA7C File Offset: 0x00019C7C
	public static void Pause(GameObject target)
	{
		Component[] components = target.GetComponents(typeof(iTween));
		foreach (iTween iTween in components)
		{
			if (iTween.delay > 0f)
			{
				iTween.delay -= iTween.timer().getTime() - iTween.delayStarted;
				iTween.StopCoroutine("TweenDelay");
			}
			iTween.isPaused = true;
			iTween.enabled = false;
		}
	}

	// Token: 0x060000B3 RID: 179 RVA: 0x0001BB04 File Offset: 0x00019D04
	public static void Pause(GameObject target, bool includechildren)
	{
		iTween.Pause(target);
		if (includechildren)
		{
			foreach (object obj in target.transform)
			{
				Transform transform = (Transform)obj;
				iTween.Pause(transform.gameObject, true);
			}
		}
	}

	// Token: 0x060000B4 RID: 180 RVA: 0x0001BB78 File Offset: 0x00019D78
	public static void Pause(GameObject target, string type)
	{
		Component[] components = target.GetComponents(typeof(iTween));
		foreach (iTween iTween in components)
		{
			string text = iTween.type + iTween.method;
			text = text.Substring(0, type.Length);
			if (text.ToLower() == type.ToLower())
			{
				if (iTween.delay > 0f)
				{
					iTween.delay -= iTween.timer().getTime() - iTween.delayStarted;
					iTween.StopCoroutine("TweenDelay");
				}
				iTween.isPaused = true;
				iTween.enabled = false;
			}
		}
	}

	// Token: 0x060000B5 RID: 181 RVA: 0x0001BC38 File Offset: 0x00019E38
	public static void Pause(GameObject target, string type, bool includechildren)
	{
		Component[] components = target.GetComponents(typeof(iTween));
		foreach (iTween iTween in components)
		{
			string text = iTween.type + iTween.method;
			text = text.Substring(0, type.Length);
			if (text.ToLower() == type.ToLower())
			{
				if (iTween.delay > 0f)
				{
					iTween.delay -= iTween.timer().getTime() - iTween.delayStarted;
					iTween.StopCoroutine("TweenDelay");
				}
				iTween.isPaused = true;
				iTween.enabled = false;
			}
		}
		if (includechildren)
		{
			foreach (object obj in target.transform)
			{
				Transform transform = (Transform)obj;
				iTween.Pause(transform.gameObject, type, true);
			}
		}
	}

	// Token: 0x060000B6 RID: 182 RVA: 0x0001BD64 File Offset: 0x00019F64
	public static void Pause()
	{
		for (int i = 0; i < iTween.tweens.Count; i++)
		{
			Hashtable hashtable = (Hashtable)iTween.tweens[i];
			GameObject target = (GameObject)hashtable["target"];
			iTween.Pause(target);
		}
	}

	// Token: 0x060000B7 RID: 183 RVA: 0x0001BDB4 File Offset: 0x00019FB4
	public static void Pause(string type)
	{
		ArrayList arrayList = new ArrayList();
		for (int i = 0; i < iTween.tweens.Count; i++)
		{
			Hashtable hashtable = (Hashtable)iTween.tweens[i];
			GameObject gameObject = (GameObject)hashtable["target"];
			arrayList.Insert(arrayList.Count, gameObject);
		}
		for (int j = 0; j < arrayList.Count; j++)
		{
			iTween.Pause((GameObject)arrayList[j], type);
		}
	}

	// Token: 0x060000B8 RID: 184 RVA: 0x00002B9B File Offset: 0x00000D9B
	public static int Count()
	{
		return iTween.tweens.Count;
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x0001BE40 File Offset: 0x0001A040
	public static int Count(string type)
	{
		int num = 0;
		for (int i = 0; i < iTween.tweens.Count; i++)
		{
			Hashtable hashtable = (Hashtable)iTween.tweens[i];
			string text = (string)hashtable["type"] + (string)hashtable["method"];
			text = text.Substring(0, type.Length);
			if (text.ToLower() == type.ToLower())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060000BA RID: 186 RVA: 0x0001BECC File Offset: 0x0001A0CC
	public static int Count(GameObject target)
	{
		Component[] components = target.GetComponents(typeof(iTween));
		return components.Length;
	}

	// Token: 0x060000BB RID: 187 RVA: 0x0001BEF0 File Offset: 0x0001A0F0
	public static int Count(GameObject target, string type)
	{
		int num = 0;
		Component[] components = target.GetComponents(typeof(iTween));
		foreach (iTween iTween in components)
		{
			string text = iTween.type + iTween.method;
			text = text.Substring(0, type.Length);
			if (text.ToLower() == type.ToLower())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060000BC RID: 188 RVA: 0x0001BF74 File Offset: 0x0001A174
	public static void Stop()
	{
		for (int i = 0; i < iTween.tweens.Count; i++)
		{
			Hashtable hashtable = (Hashtable)iTween.tweens[i];
			GameObject target = (GameObject)hashtable["target"];
			iTween.Stop(target);
		}
		iTween.tweens.Clear();
	}

	// Token: 0x060000BD RID: 189 RVA: 0x0001BFD0 File Offset: 0x0001A1D0
	public static void Stop(string type)
	{
		ArrayList arrayList = new ArrayList();
		for (int i = 0; i < iTween.tweens.Count; i++)
		{
			Hashtable hashtable = (Hashtable)iTween.tweens[i];
			GameObject gameObject = (GameObject)hashtable["target"];
			arrayList.Insert(arrayList.Count, gameObject);
		}
		for (int j = 0; j < arrayList.Count; j++)
		{
			iTween.Stop((GameObject)arrayList[j], type);
		}
	}

	// Token: 0x060000BE RID: 190 RVA: 0x0001C05C File Offset: 0x0001A25C
	public static void Stop(GameObject target)
	{
		Component[] components = target.GetComponents(typeof(iTween));
		foreach (iTween iTween in components)
		{
			iTween.Dispose();
		}
	}

	// Token: 0x060000BF RID: 191 RVA: 0x0001C0A0 File Offset: 0x0001A2A0
	public static void Stop(GameObject target, bool includechildren)
	{
		iTween.Stop(target);
		if (includechildren)
		{
			foreach (object obj in target.transform)
			{
				Transform transform = (Transform)obj;
				iTween.Stop(transform.gameObject, true);
			}
		}
	}

	// Token: 0x060000C0 RID: 192 RVA: 0x0001C114 File Offset: 0x0001A314
	public static void Stop(GameObject target, string type)
	{
		Component[] components = target.GetComponents(typeof(iTween));
		foreach (iTween iTween in components)
		{
			string text = iTween.type + iTween.method;
			text = text.Substring(0, type.Length);
			if (text.ToLower() == type.ToLower())
			{
				iTween.Dispose();
			}
		}
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x0001C194 File Offset: 0x0001A394
	public static void Stop(GameObject target, string type, bool includechildren)
	{
		Component[] components = target.GetComponents(typeof(iTween));
		foreach (iTween iTween in components)
		{
			string text = iTween.type + iTween.method;
			text = text.Substring(0, type.Length);
			if (text.ToLower() == type.ToLower())
			{
				iTween.Dispose();
			}
		}
		if (includechildren)
		{
			foreach (object obj in target.transform)
			{
				Transform transform = (Transform)obj;
				iTween.Stop(transform.gameObject, type, true);
			}
		}
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x0001C27C File Offset: 0x0001A47C
	public static Hashtable Hash(params object[] args)
	{
		Hashtable hashtable = new Hashtable(args.Length / 2);
		if (args.Length % 2 != 0)
		{
			Debug.LogError("Tween Error: Hash requires an even number of arguments!");
			return null;
		}
		for (int i = 0; i < args.Length - 1; i += 2)
		{
			hashtable.Add(args[i], args[i + 1]);
		}
		return hashtable;
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x00002BA7 File Offset: 0x00000DA7
	private void Awake()
	{
		this.RetrieveArgs();
		this.lastRealTime = iTween.realTimeTimer.getTime();
	}

	// Token: 0x060000C4 RID: 196 RVA: 0x0001C2D0 File Offset: 0x0001A4D0
	private IEnumerator Start()
	{
		if (this.delay > 0f)
		{
			yield return base.StartCoroutine("TweenDelay");
		}
		this.TweenStart();
		yield break;
	}

	// Token: 0x060000C5 RID: 197 RVA: 0x00002BBF File Offset: 0x00000DBF
	private iTween.ITimer timer()
	{
		return this.localTimer ?? iTween.sharedTimer;
	}

	// Token: 0x060000C6 RID: 198 RVA: 0x0001C2EC File Offset: 0x0001A4EC
	private void Update()
	{
		if (this.isRunning && !this.physics)
		{
			if (!this.reverse)
			{
				if (this.percentage < 1f)
				{
					this.TweenUpdate();
				}
				else
				{
					this.TweenComplete();
				}
			}
			else if (this.percentage > 0f)
			{
				this.TweenUpdate();
			}
			else
			{
				this.TweenComplete();
			}
		}
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x0001C364 File Offset: 0x0001A564
	private void FixedUpdate()
	{
		if (this.isRunning && this.physics)
		{
			if (!this.reverse)
			{
				if (this.percentage < 1f)
				{
					this.TweenUpdate();
				}
				else
				{
					this.TweenComplete();
				}
			}
			else if (this.percentage > 0f)
			{
				this.TweenUpdate();
			}
			else
			{
				this.TweenComplete();
			}
		}
	}

	// Token: 0x060000C8 RID: 200 RVA: 0x0001C3DC File Offset: 0x0001A5DC
	private void LateUpdate()
	{
		if (this.tweenArguments.Contains("looktarget") && this.isRunning && (this.type == "move" || this.type == "shake" || this.type == "punch"))
		{
			iTween.LookUpdate(base.gameObject, this.tweenArguments);
		}
	}

	// Token: 0x060000C9 RID: 201 RVA: 0x0001C45C File Offset: 0x0001A65C
	private void OnEnable()
	{
		if (this.isRunning)
		{
			this.EnableKinematic();
		}
		if (this.isPaused)
		{
			this.isPaused = false;
			if (this.delay > 0f)
			{
				this.wasPaused = true;
				this.ResumeDelay();
			}
		}
	}

	// Token: 0x060000CA RID: 202 RVA: 0x00002BD3 File Offset: 0x00000DD3
	private void OnDisable()
	{
		this.DisableKinematic();
	}

	// Token: 0x060000CB RID: 203 RVA: 0x0001C4AC File Offset: 0x0001A6AC
	private static void DrawLineHelper(Vector3[] line, Color color, string method)
	{
		Gizmos.color = color;
		for (int i = 0; i < line.Length - 1; i++)
		{
			if (method == "gizmos")
			{
				Gizmos.DrawLine(line[i], line[i + 1]);
			}
			else if (method == "handles")
			{
				Debug.LogError("iTween Error: Drawing a line with Handles is temporarily disabled because of compatability issues with Unity 2.6!");
			}
		}
	}

	// Token: 0x060000CC RID: 204 RVA: 0x0001C524 File Offset: 0x0001A724
	private static void DrawPathHelper(Vector3[] path, Color color, string method)
	{
		Vector3[] pts = iTween.PathControlPointGenerator(path);
		Vector3 vector = iTween.Interp(pts, 0f);
		Gizmos.color = color;
		int num = path.Length * 20;
		for (int i = 1; i <= num; i++)
		{
			float t = (float)i / (float)num;
			Vector3 vector2 = iTween.Interp(pts, t);
			if (method == "gizmos")
			{
				Gizmos.DrawLine(vector2, vector);
			}
			else if (method == "handles")
			{
				Debug.LogError("iTween Error: Drawing a path with Handles is temporarily disabled because of compatability issues with Unity 2.6!");
			}
			vector = vector2;
		}
	}

	// Token: 0x060000CD RID: 205 RVA: 0x0001C5B0 File Offset: 0x0001A7B0
	private static Vector3[] PathControlPointGenerator(Vector3[] path)
	{
		int num = 2;
		Vector3[] array = new Vector3[path.Length + num];
		Array.Copy(path, 0, array, 1, path.Length);
		array[0] = array[1] + (array[1] - array[2]);
		array[array.Length - 1] = array[array.Length - 2] + (array[array.Length - 2] - array[array.Length - 3]);
		if (array[1] == array[array.Length - 2])
		{
			Vector3[] array2 = new Vector3[array.Length];
			Array.Copy(array, array2, array.Length);
			array2[0] = array2[array2.Length - 3];
			array2[array2.Length - 1] = array2[2];
			array = new Vector3[array2.Length];
			Array.Copy(array2, array, array2.Length);
		}
		return array;
	}

	// Token: 0x060000CE RID: 206 RVA: 0x0001C6E4 File Offset: 0x0001A8E4
	private static Vector3 Interp(Vector3[] pts, float t)
	{
		int num = pts.Length - 3;
		int num2 = Mathf.Min(Mathf.FloorToInt(t * (float)num), num - 1);
		float num3 = t * (float)num - (float)num2;
		Vector3 vector = pts[num2];
		Vector3 vector2 = pts[num2 + 1];
		Vector3 vector3 = pts[num2 + 2];
		Vector3 vector4 = pts[num2 + 3];
		return 0.5f * ((-vector + 3f * vector2 - 3f * vector3 + vector4) * (num3 * num3 * num3) + (2f * vector - 5f * vector2 + 4f * vector3 - vector4) * (num3 * num3) + (-vector + vector3) * num3 + 2f * vector2);
	}

	// Token: 0x060000CF RID: 207 RVA: 0x0001C7FC File Offset: 0x0001A9FC
	private static void Launch(GameObject target, Hashtable args)
	{
		if (!args.Contains("id"))
		{
			args["id"] = iTween.GenerateID();
		}
		if (!args.Contains("target"))
		{
			args["target"] = target;
		}
		iTween.tweens.Insert(0, args);
		target.AddComponent("iTween");
	}

	// Token: 0x060000D0 RID: 208 RVA: 0x0001C860 File Offset: 0x0001AA60
	private static Hashtable CleanArgs(Hashtable args)
	{
		Hashtable hashtable = new Hashtable(args.Count);
		Hashtable hashtable2 = new Hashtable(args.Count);
		foreach (object obj in args)
		{
			DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
			hashtable.Add(dictionaryEntry.Key, dictionaryEntry.Value);
		}
		foreach (object obj2 in hashtable)
		{
			DictionaryEntry dictionaryEntry2 = (DictionaryEntry)obj2;
			if (dictionaryEntry2.Value.GetType() == typeof(int))
			{
				int num = (int)dictionaryEntry2.Value;
				float num2 = (float)num;
				args[dictionaryEntry2.Key] = num2;
			}
			if (dictionaryEntry2.Value.GetType() == typeof(double))
			{
				double num3 = (double)dictionaryEntry2.Value;
				float num4 = (float)num3;
				args[dictionaryEntry2.Key] = num4;
			}
		}
		foreach (object obj3 in args)
		{
			DictionaryEntry dictionaryEntry3 = (DictionaryEntry)obj3;
			hashtable2.Add(dictionaryEntry3.Key.ToString().ToLower(), dictionaryEntry3.Value);
		}
		args = hashtable2;
		return args;
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x0001CA28 File Offset: 0x0001AC28
	private static string GenerateID()
	{
		int num = 15;
		char[] array = new char[]
		{
			'a',
			'b',
			'c',
			'd',
			'e',
			'f',
			'g',
			'h',
			'i',
			'j',
			'k',
			'l',
			'm',
			'n',
			'o',
			'p',
			'q',
			'r',
			's',
			't',
			'u',
			'v',
			'w',
			'x',
			'y',
			'z',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F',
			'G',
			'H',
			'I',
			'J',
			'K',
			'L',
			'M',
			'N',
			'O',
			'P',
			'Q',
			'R',
			'S',
			'T',
			'U',
			'V',
			'W',
			'X',
			'Y',
			'Z',
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8'
		};
		int num2 = array.Length - 1;
		string text = string.Empty;
		for (int i = 0; i < num; i++)
		{
			text += array[(int)Mathf.Floor((float)Random.Range(0, num2))];
		}
		return text;
	}

	// Token: 0x060000D2 RID: 210 RVA: 0x0001CA8C File Offset: 0x0001AC8C
	private void RetrieveArgs()
	{
		foreach (object obj in iTween.tweens)
		{
			Hashtable hashtable = (Hashtable)obj;
			if ((GameObject)hashtable["target"] == base.gameObject)
			{
				this.tweenArguments = hashtable;
				break;
			}
		}
		this.id = (string)this.tweenArguments["id"];
		this.type = (string)this.tweenArguments["type"];
		this.method = (string)this.tweenArguments["method"];
		if (this.tweenArguments.Contains("time"))
		{
			this.time = (float)this.tweenArguments["time"];
		}
		else
		{
			this.time = iTween.Defaults.time;
		}
		if (base.rigidbody != null)
		{
			this.physics = true;
		}
		if (this.tweenArguments.Contains("delay"))
		{
			this.delay = (float)this.tweenArguments["delay"];
		}
		else
		{
			this.delay = iTween.Defaults.delay;
		}
		if (this.tweenArguments.Contains("namedcolorvalue"))
		{
			if (this.tweenArguments["namedcolorvalue"].GetType() == typeof(iTween.NamedValueColor))
			{
				this.namedcolorvalue = (iTween.NamedValueColor)((int)this.tweenArguments["namedcolorvalue"]);
			}
			else
			{
				try
				{
					this.namedcolorvalue = (iTween.NamedValueColor)((int)Enum.Parse(typeof(iTween.NamedValueColor), (string)this.tweenArguments["namedcolorvalue"], true));
				}
				catch
				{
					Debug.LogWarning("iTween: Unsupported namedcolorvalue supplied! Default will be used.");
					this.namedcolorvalue = iTween.NamedValueColor._Color;
				}
			}
		}
		else
		{
			this.namedcolorvalue = iTween.Defaults.namedColorValue;
		}
		if (this.tweenArguments.Contains("looptype"))
		{
			if (this.tweenArguments["looptype"].GetType() == typeof(iTween.LoopType))
			{
				this.loopType = (iTween.LoopType)((int)this.tweenArguments["looptype"]);
			}
			else
			{
				try
				{
					this.loopType = (iTween.LoopType)((int)Enum.Parse(typeof(iTween.LoopType), (string)this.tweenArguments["looptype"], true));
				}
				catch
				{
					Debug.LogWarning("iTween: Unsupported loopType supplied! Default will be used.");
					this.loopType = iTween.LoopType.none;
				}
			}
		}
		else
		{
			this.loopType = iTween.LoopType.none;
		}
		if (this.tweenArguments.Contains("easetype"))
		{
			if (this.tweenArguments["easetype"].GetType() == typeof(iTween.EaseType))
			{
				this.easeType = (iTween.EaseType)((int)this.tweenArguments["easetype"]);
			}
			else
			{
				try
				{
					this.easeType = (iTween.EaseType)((int)Enum.Parse(typeof(iTween.EaseType), (string)this.tweenArguments["easetype"], true));
				}
				catch
				{
					Debug.LogWarning("iTween: Unsupported easeType supplied! Default will be used.");
					this.easeType = iTween.Defaults.easeType;
				}
			}
		}
		else
		{
			this.easeType = iTween.Defaults.easeType;
		}
		if (this.tweenArguments.Contains("space"))
		{
			if (this.tweenArguments["space"].GetType() == typeof(Space))
			{
				this.space = (int)this.tweenArguments["space"];
			}
			else
			{
				try
				{
					this.space = (int)Enum.Parse(typeof(Space), (string)this.tweenArguments["space"], true);
				}
				catch
				{
					Debug.LogWarning("iTween: Unsupported space supplied! Default will be used.");
					this.space = iTween.Defaults.space;
				}
			}
		}
		else
		{
			this.space = iTween.Defaults.space;
		}
		if (this.tweenArguments.Contains("islocal"))
		{
			this.isLocal = (bool)this.tweenArguments["islocal"];
		}
		else
		{
			this.isLocal = iTween.Defaults.isLocal;
		}
		if (this.tweenArguments.Contains("ignoretimescale"))
		{
			this.useRealTime = (bool)this.tweenArguments["ignoretimescale"];
		}
		else
		{
			this.useRealTime = iTween.Defaults.useRealTime;
		}
		this.GetEasingFunction();
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x0001CF8C File Offset: 0x0001B18C
	private void GetEasingFunction()
	{
		switch (this.easeType)
		{
		case iTween.EaseType.easeInQuad:
			this.ease = new iTween.EasingFunction(this.easeInQuad);
			break;
		case iTween.EaseType.easeOutQuad:
			this.ease = new iTween.EasingFunction(this.easeOutQuad);
			break;
		case iTween.EaseType.easeInOutQuad:
			this.ease = new iTween.EasingFunction(this.easeInOutQuad);
			break;
		case iTween.EaseType.easeInCubic:
			this.ease = new iTween.EasingFunction(this.easeInCubic);
			break;
		case iTween.EaseType.easeOutCubic:
			this.ease = new iTween.EasingFunction(this.easeOutCubic);
			break;
		case iTween.EaseType.easeInOutCubic:
			this.ease = new iTween.EasingFunction(this.easeInOutCubic);
			break;
		case iTween.EaseType.easeInQuart:
			this.ease = new iTween.EasingFunction(this.easeInQuart);
			break;
		case iTween.EaseType.easeOutQuart:
			this.ease = new iTween.EasingFunction(this.easeOutQuart);
			break;
		case iTween.EaseType.easeInOutQuart:
			this.ease = new iTween.EasingFunction(this.easeInOutQuart);
			break;
		case iTween.EaseType.easeInQuint:
			this.ease = new iTween.EasingFunction(this.easeInQuint);
			break;
		case iTween.EaseType.easeOutQuint:
			this.ease = new iTween.EasingFunction(this.easeOutQuint);
			break;
		case iTween.EaseType.easeInOutQuint:
			this.ease = new iTween.EasingFunction(this.easeInOutQuint);
			break;
		case iTween.EaseType.easeInSine:
			this.ease = new iTween.EasingFunction(this.easeInSine);
			break;
		case iTween.EaseType.easeOutSine:
			this.ease = new iTween.EasingFunction(this.easeOutSine);
			break;
		case iTween.EaseType.easeInOutSine:
			this.ease = new iTween.EasingFunction(this.easeInOutSine);
			break;
		case iTween.EaseType.easeInExpo:
			this.ease = new iTween.EasingFunction(this.easeInExpo);
			break;
		case iTween.EaseType.easeOutExpo:
			this.ease = new iTween.EasingFunction(this.easeOutExpo);
			break;
		case iTween.EaseType.easeInOutExpo:
			this.ease = new iTween.EasingFunction(this.easeInOutExpo);
			break;
		case iTween.EaseType.easeInCirc:
			this.ease = new iTween.EasingFunction(this.easeInCirc);
			break;
		case iTween.EaseType.easeOutCirc:
			this.ease = new iTween.EasingFunction(this.easeOutCirc);
			break;
		case iTween.EaseType.easeInOutCirc:
			this.ease = new iTween.EasingFunction(this.easeInOutCirc);
			break;
		case iTween.EaseType.linear:
			this.ease = new iTween.EasingFunction(this.linear);
			break;
		case iTween.EaseType.spring:
			this.ease = new iTween.EasingFunction(this.spring);
			break;
		case iTween.EaseType.bounce:
			this.ease = new iTween.EasingFunction(this.bounce);
			break;
		case iTween.EaseType.easeInBack:
			this.ease = new iTween.EasingFunction(this.easeInBack);
			break;
		case iTween.EaseType.easeOutBack:
			this.ease = new iTween.EasingFunction(this.easeOutBack);
			break;
		case iTween.EaseType.easeInOutBack:
			this.ease = new iTween.EasingFunction(this.easeInOutBack);
			break;
		case iTween.EaseType.elastic:
			this.ease = new iTween.EasingFunction(this.elastic);
			break;
		}
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x0001D2A0 File Offset: 0x0001B4A0
	private void UpdatePercentage()
	{
		if (this.useRealTime)
		{
			this.runningTime += iTween.realTimeTimer.getTime() - this.lastRealTime;
		}
		else
		{
			this.runningTime += this.timer().getDeltaFrameTime();
		}
		if (this.reverse)
		{
			this.percentage = 1f - this.runningTime / this.time;
		}
		else
		{
			this.percentage = this.runningTime / this.time;
		}
		this.lastRealTime = iTween.realTimeTimer.getTime();
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x0001D340 File Offset: 0x0001B540
	private void CallBack(string callbackType)
	{
		if (this.tweenArguments.Contains(callbackType) && !this.tweenArguments.Contains("ischild"))
		{
			GameObject gameObject;
			if (this.tweenArguments.Contains(callbackType + "target"))
			{
				gameObject = (GameObject)this.tweenArguments[callbackType + "target"];
			}
			else
			{
				gameObject = base.gameObject;
			}
			if (this.tweenArguments[callbackType].GetType() == typeof(string))
			{
				gameObject.SendMessage((string)this.tweenArguments[callbackType], this.tweenArguments[callbackType + "params"], 1);
			}
			else
			{
				Debug.LogError("iTween Error: Callback method references must be passed as a String!");
				Object.Destroy(this);
			}
		}
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x0001D41C File Offset: 0x0001B61C
	private void Dispose()
	{
		for (int i = 0; i < iTween.tweens.Count; i++)
		{
			Hashtable hashtable = (Hashtable)iTween.tweens[i];
			if ((string)hashtable["id"] == this.id)
			{
				iTween.tweens.RemoveAt(i);
				break;
			}
		}
		Object.Destroy(this);
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x0001D48C File Offset: 0x0001B68C
	private void ConflictCheck()
	{
		Component[] components = base.GetComponents(typeof(iTween));
		foreach (iTween iTween in components)
		{
			if (iTween.type == "value")
			{
				return;
			}
			if (iTween.isRunning && iTween.type == this.type)
			{
				if (iTween.tweenArguments.Count != this.tweenArguments.Count)
				{
					iTween.Dispose();
					return;
				}
				foreach (object obj in this.tweenArguments)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					if (!iTween.tweenArguments.Contains(dictionaryEntry.Key))
					{
						iTween.Dispose();
						return;
					}
					if (!iTween.tweenArguments[dictionaryEntry.Key].Equals(this.tweenArguments[dictionaryEntry.Key]) && (string)dictionaryEntry.Key != "id")
					{
						iTween.Dispose();
						return;
					}
				}
				this.Dispose();
			}
		}
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x0001D5F8 File Offset: 0x0001B7F8
	private void EnableKinematic()
	{
		if (base.gameObject.GetComponent(typeof(Rigidbody)) && !base.rigidbody.isKinematic)
		{
			this.kinematic = true;
			base.rigidbody.isKinematic = true;
		}
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x00002BDB File Offset: 0x00000DDB
	private void DisableKinematic()
	{
		if (this.kinematic && base.rigidbody.isKinematic)
		{
			this.kinematic = false;
			base.rigidbody.isKinematic = false;
		}
	}

	// Token: 0x060000DA RID: 218 RVA: 0x00002C0B File Offset: 0x00000E0B
	private void ResumeDelay()
	{
		base.StartCoroutine("TweenDelay");
	}

	// Token: 0x060000DB RID: 219 RVA: 0x00002C19 File Offset: 0x00000E19
	private float linear(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, value);
	}

	// Token: 0x060000DC RID: 220 RVA: 0x0001D648 File Offset: 0x0001B848
	private float clerp(float start, float end, float value)
	{
		float num = 0f;
		float num2 = 360f;
		float num3 = Mathf.Abs((num2 - num) / 2f);
		float result;
		if (end - start < -num3)
		{
			float num4 = (num2 - start + end) * value;
			result = start + num4;
		}
		else if (end - start > num3)
		{
			float num4 = -(num2 - end + start) * value;
			result = start + num4;
		}
		else
		{
			result = start + (end - start) * value;
		}
		return result;
	}

	// Token: 0x060000DD RID: 221 RVA: 0x0001D6C0 File Offset: 0x0001B8C0
	private float spring(float start, float end, float value)
	{
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * 3.1415927f * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + 1.2f * (1f - value));
		return start + (end - start) * value;
	}

	// Token: 0x060000DE RID: 222 RVA: 0x00002C23 File Offset: 0x00000E23
	private float easeInQuad(float start, float end, float value)
	{
		end -= start;
		return end * value * value + start;
	}

	// Token: 0x060000DF RID: 223 RVA: 0x00002C31 File Offset: 0x00000E31
	private float easeOutQuad(float start, float end, float value)
	{
		end -= start;
		return -end * value * (value - 2f) + start;
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x0001D724 File Offset: 0x0001B924
	private float easeInOutQuad(float start, float end, float value)
	{
		value /= 0.5f;
		end -= start;
		if (value < 1f)
		{
			return end / 2f * value * value + start;
		}
		value -= 1f;
		return -end / 2f * (value * (value - 2f) - 1f) + start;
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x00002C46 File Offset: 0x00000E46
	private float easeInCubic(float start, float end, float value)
	{
		end -= start;
		return end * value * value * value + start;
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x00002C56 File Offset: 0x00000E56
	private float easeOutCubic(float start, float end, float value)
	{
		value -= 1f;
		end -= start;
		return end * (value * value * value + 1f) + start;
	}

	// Token: 0x060000E3 RID: 227 RVA: 0x0001D77C File Offset: 0x0001B97C
	private float easeInOutCubic(float start, float end, float value)
	{
		value /= 0.5f;
		end -= start;
		if (value < 1f)
		{
			return end / 2f * value * value * value + start;
		}
		value -= 2f;
		return end / 2f * (value * value * value + 2f) + start;
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x00002C75 File Offset: 0x00000E75
	private float easeInQuart(float start, float end, float value)
	{
		end -= start;
		return end * value * value * value * value + start;
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x00002C87 File Offset: 0x00000E87
	private float easeOutQuart(float start, float end, float value)
	{
		value -= 1f;
		end -= start;
		return -end * (value * value * value * value - 1f) + start;
	}

	// Token: 0x060000E6 RID: 230 RVA: 0x0001D7D0 File Offset: 0x0001B9D0
	private float easeInOutQuart(float start, float end, float value)
	{
		value /= 0.5f;
		end -= start;
		if (value < 1f)
		{
			return end / 2f * value * value * value * value + start;
		}
		value -= 2f;
		return -end / 2f * (value * value * value * value - 2f) + start;
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x00002CA9 File Offset: 0x00000EA9
	private float easeInQuint(float start, float end, float value)
	{
		end -= start;
		return end * value * value * value * value * value + start;
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x00002CBD File Offset: 0x00000EBD
	private float easeOutQuint(float start, float end, float value)
	{
		value -= 1f;
		end -= start;
		return end * (value * value * value * value * value + 1f) + start;
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x0001D82C File Offset: 0x0001BA2C
	private float easeInOutQuint(float start, float end, float value)
	{
		value /= 0.5f;
		end -= start;
		if (value < 1f)
		{
			return end / 2f * value * value * value * value * value + start;
		}
		value -= 2f;
		return end / 2f * (value * value * value * value * value + 2f) + start;
	}

	// Token: 0x060000EA RID: 234 RVA: 0x00002CE0 File Offset: 0x00000EE0
	private float easeInSine(float start, float end, float value)
	{
		end -= start;
		return -end * Mathf.Cos(value / 1f * 1.5707964f) + end + start;
	}

	// Token: 0x060000EB RID: 235 RVA: 0x00002D00 File Offset: 0x00000F00
	private float easeOutSine(float start, float end, float value)
	{
		end -= start;
		return end * Mathf.Sin(value / 1f * 1.5707964f) + start;
	}

	// Token: 0x060000EC RID: 236 RVA: 0x00002D1D File Offset: 0x00000F1D
	private float easeInOutSine(float start, float end, float value)
	{
		end -= start;
		return -end / 2f * (Mathf.Cos(3.1415927f * value / 1f) - 1f) + start;
	}

	// Token: 0x060000ED RID: 237 RVA: 0x00002D47 File Offset: 0x00000F47
	private float easeInExpo(float start, float end, float value)
	{
		end -= start;
		return end * Mathf.Pow(2f, 10f * (value / 1f - 1f)) + start;
	}

	// Token: 0x060000EE RID: 238 RVA: 0x00002D6F File Offset: 0x00000F6F
	private float easeOutExpo(float start, float end, float value)
	{
		end -= start;
		return end * (-Mathf.Pow(2f, -10f * value / 1f) + 1f) + start;
	}

	// Token: 0x060000EF RID: 239 RVA: 0x0001D888 File Offset: 0x0001BA88
	private float easeInOutExpo(float start, float end, float value)
	{
		value /= 0.5f;
		end -= start;
		if (value < 1f)
		{
			return end / 2f * Mathf.Pow(2f, 10f * (value - 1f)) + start;
		}
		value -= 1f;
		return end / 2f * (-Mathf.Pow(2f, -10f * value) + 2f) + start;
	}

	// Token: 0x060000F0 RID: 240 RVA: 0x00002D98 File Offset: 0x00000F98
	private float easeInCirc(float start, float end, float value)
	{
		end -= start;
		return -end * (Mathf.Sqrt(1f - value * value) - 1f) + start;
	}

	// Token: 0x060000F1 RID: 241 RVA: 0x00002DB8 File Offset: 0x00000FB8
	private float easeOutCirc(float start, float end, float value)
	{
		value -= 1f;
		end -= start;
		return end * Mathf.Sqrt(1f - value * value) + start;
	}

	// Token: 0x060000F2 RID: 242 RVA: 0x0001D8FC File Offset: 0x0001BAFC
	private float easeInOutCirc(float start, float end, float value)
	{
		value /= 0.5f;
		end -= start;
		if (value < 1f)
		{
			return -end / 2f * (Mathf.Sqrt(1f - value * value) - 1f) + start;
		}
		value -= 2f;
		return end / 2f * (Mathf.Sqrt(1f - value * value) + 1f) + start;
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x0001D96C File Offset: 0x0001BB6C
	private float bounce(float start, float end, float value)
	{
		value /= 1f;
		end -= start;
		if (value < 0.36363637f)
		{
			return end * (7.5625f * value * value) + start;
		}
		if (value < 0.72727275f)
		{
			value -= 0.54545456f;
			return end * (7.5625f * value * value + 0.75f) + start;
		}
		if ((double)value < 0.9090909090909091)
		{
			value -= 0.8181818f;
			return end * (7.5625f * value * value + 0.9375f) + start;
		}
		value -= 0.95454544f;
		return end * (7.5625f * value * value + 0.984375f) + start;
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x0001DA14 File Offset: 0x0001BC14
	private float easeInBack(float start, float end, float value)
	{
		end -= start;
		value /= 1f;
		float num = 1.70158f;
		return end * value * value * ((num + 1f) * value - num) + start;
	}

	// Token: 0x060000F5 RID: 245 RVA: 0x0001DA48 File Offset: 0x0001BC48
	private float easeOutBack(float start, float end, float value)
	{
		float num = 1.70158f;
		end -= start;
		value = value / 1f - 1f;
		return end * (value * value * ((num + 1f) * value + num) + 1f) + start;
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x0001DA88 File Offset: 0x0001BC88
	private float easeInOutBack(float start, float end, float value)
	{
		float num = 1.70158f;
		end -= start;
		value /= 0.5f;
		if (value < 1f)
		{
			num *= 1.525f;
			return end / 2f * (value * value * ((num + 1f) * value - num)) + start;
		}
		value -= 2f;
		num *= 1.525f;
		return end / 2f * (value * value * ((num + 1f) * value + num) + 2f) + start;
	}

	// Token: 0x060000F7 RID: 247 RVA: 0x0001DB08 File Offset: 0x0001BD08
	private float punch(float amplitude, float value)
	{
		if (value == 0f)
		{
			return 0f;
		}
		if (value == 1f)
		{
			return 0f;
		}
		float num = 0.3f;
		float num2 = num / 6.2831855f * Mathf.Asin(0f);
		return amplitude * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * 1f - num2) * 6.2831855f / num);
	}

	// Token: 0x060000F8 RID: 248 RVA: 0x0001DB80 File Offset: 0x0001BD80
	private float elastic(float start, float end, float value)
	{
		end -= start;
		float num = 1f;
		float num2 = num * 0.3f;
		float num3 = 0f;
		if (value == 0f)
		{
			return start;
		}
		if ((value /= num) == 1f)
		{
			return start + end;
		}
		float num4;
		if (num3 == 0f || num3 < Mathf.Abs(end))
		{
			num3 = end;
			num4 = num2 / 4f;
		}
		else
		{
			num4 = num2 / 6.2831855f * Mathf.Asin(end / num3);
		}
		return num3 * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * num - num4) * 6.2831855f / num2) + end + start;
	}

	// Token: 0x04000001 RID: 1
	public static ArrayList tweens = new ArrayList();

	// Token: 0x04000002 RID: 2
	private static GameObject cameraFade;

	// Token: 0x04000003 RID: 3
	public string id;

	// Token: 0x04000004 RID: 4
	public string type;

	// Token: 0x04000005 RID: 5
	public string method;

	// Token: 0x04000006 RID: 6
	public iTween.EaseType easeType;

	// Token: 0x04000007 RID: 7
	public float time;

	// Token: 0x04000008 RID: 8
	public float delay;

	// Token: 0x04000009 RID: 9
	public iTween.LoopType loopType;

	// Token: 0x0400000A RID: 10
	public bool isRunning;

	// Token: 0x0400000B RID: 11
	public bool isPaused;

	// Token: 0x0400000C RID: 12
	private float runningTime;

	// Token: 0x0400000D RID: 13
	private float percentage;

	// Token: 0x0400000E RID: 14
	private float delayStarted;

	// Token: 0x0400000F RID: 15
	private bool kinematic;

	// Token: 0x04000010 RID: 16
	private bool isLocal;

	// Token: 0x04000011 RID: 17
	private bool loop;

	// Token: 0x04000012 RID: 18
	private bool reverse;

	// Token: 0x04000013 RID: 19
	private bool wasPaused;

	// Token: 0x04000014 RID: 20
	private bool physics;

	// Token: 0x04000015 RID: 21
	private Hashtable tweenArguments;

	// Token: 0x04000016 RID: 22
	private Space space;

	// Token: 0x04000017 RID: 23
	private iTween.EasingFunction ease;

	// Token: 0x04000018 RID: 24
	private iTween.ApplyTween apply;

	// Token: 0x04000019 RID: 25
	private AudioSource audioSource;

	// Token: 0x0400001A RID: 26
	private Vector3[] vector3s;

	// Token: 0x0400001B RID: 27
	private Vector2[] vector2s;

	// Token: 0x0400001C RID: 28
	private Color[,] colors;

	// Token: 0x0400001D RID: 29
	private float[] floats;

	// Token: 0x0400001E RID: 30
	private Rect[] rects;

	// Token: 0x0400001F RID: 31
	private iTween.CRSpline path;

	// Token: 0x04000020 RID: 32
	private Vector3 preUpdate;

	// Token: 0x04000021 RID: 33
	private Vector3 postUpdate;

	// Token: 0x04000022 RID: 34
	private iTween.NamedValueColor namedcolorvalue;

	// Token: 0x04000023 RID: 35
	private float lastRealTime;

	// Token: 0x04000024 RID: 36
	private bool useRealTime;

	// Token: 0x04000025 RID: 37
	private static iTween.ITimer sharedTimer = new iTween.UnityTimer();

	// Token: 0x04000026 RID: 38
	private static iTween.ITimer realTimeTimer = new iTween.UnityRealTimeTimer();

	// Token: 0x04000027 RID: 39
	private iTween.ITimer localTimer;

	// Token: 0x02000003 RID: 3
	public interface ITimer
	{
		// Token: 0x060000F9 RID: 249
		float getTime();

		// Token: 0x060000FA RID: 250
		float getDeltaFrameTime();
	}

	// Token: 0x02000004 RID: 4
	public class UnityTimer : iTween.ITimer
	{
		// Token: 0x060000FC RID: 252 RVA: 0x00002DE2 File Offset: 0x00000FE2
		public float getTime()
		{
			return Time.time;
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00002DE9 File Offset: 0x00000FE9
		public float getDeltaFrameTime()
		{
			return Time.deltaTime;
		}
	}

	// Token: 0x02000005 RID: 5
	public class UnityRealTimeTimer : iTween.ITimer
	{
		// Token: 0x060000FF RID: 255 RVA: 0x00002DF0 File Offset: 0x00000FF0
		public float getTime()
		{
			return Time.realtimeSinceStartup;
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00002DF7 File Offset: 0x00000FF7
		public float getDeltaFrameTime()
		{
			throw new NotImplementedException("deltaTime is not implemented");
		}
	}

	// Token: 0x02000006 RID: 6
	public enum EaseType
	{
		// Token: 0x04000036 RID: 54
		easeInQuad,
		// Token: 0x04000037 RID: 55
		easeOutQuad,
		// Token: 0x04000038 RID: 56
		easeInOutQuad,
		// Token: 0x04000039 RID: 57
		easeInCubic,
		// Token: 0x0400003A RID: 58
		easeOutCubic,
		// Token: 0x0400003B RID: 59
		easeInOutCubic,
		// Token: 0x0400003C RID: 60
		easeInQuart,
		// Token: 0x0400003D RID: 61
		easeOutQuart,
		// Token: 0x0400003E RID: 62
		easeInOutQuart,
		// Token: 0x0400003F RID: 63
		easeInQuint,
		// Token: 0x04000040 RID: 64
		easeOutQuint,
		// Token: 0x04000041 RID: 65
		easeInOutQuint,
		// Token: 0x04000042 RID: 66
		easeInSine,
		// Token: 0x04000043 RID: 67
		easeOutSine,
		// Token: 0x04000044 RID: 68
		easeInOutSine,
		// Token: 0x04000045 RID: 69
		easeInExpo,
		// Token: 0x04000046 RID: 70
		easeOutExpo,
		// Token: 0x04000047 RID: 71
		easeInOutExpo,
		// Token: 0x04000048 RID: 72
		easeInCirc,
		// Token: 0x04000049 RID: 73
		easeOutCirc,
		// Token: 0x0400004A RID: 74
		easeInOutCirc,
		// Token: 0x0400004B RID: 75
		linear,
		// Token: 0x0400004C RID: 76
		spring,
		// Token: 0x0400004D RID: 77
		bounce,
		// Token: 0x0400004E RID: 78
		easeInBack,
		// Token: 0x0400004F RID: 79
		easeOutBack,
		// Token: 0x04000050 RID: 80
		easeInOutBack,
		// Token: 0x04000051 RID: 81
		elastic,
		// Token: 0x04000052 RID: 82
		punch
	}

	// Token: 0x02000007 RID: 7
	public enum LoopType
	{
		// Token: 0x04000054 RID: 84
		none,
		// Token: 0x04000055 RID: 85
		loop,
		// Token: 0x04000056 RID: 86
		pingPong
	}

	// Token: 0x02000008 RID: 8
	public enum NamedValueColor
	{
		// Token: 0x04000058 RID: 88
		_Color,
		// Token: 0x04000059 RID: 89
		_SpecColor,
		// Token: 0x0400005A RID: 90
		_Emission,
		// Token: 0x0400005B RID: 91
		_ReflectColor
	}

	// Token: 0x02000009 RID: 9
	public static class Defaults
	{
		// Token: 0x0400005C RID: 92
		public static float time = 1f;

		// Token: 0x0400005D RID: 93
		public static float delay = 0f;

		// Token: 0x0400005E RID: 94
		public static iTween.NamedValueColor namedColorValue = iTween.NamedValueColor._Color;

		// Token: 0x0400005F RID: 95
		public static iTween.LoopType loopType = iTween.LoopType.none;

		// Token: 0x04000060 RID: 96
		public static iTween.EaseType easeType = iTween.EaseType.easeOutExpo;

		// Token: 0x04000061 RID: 97
		public static float lookSpeed = 3f;

		// Token: 0x04000062 RID: 98
		public static bool isLocal = false;

		// Token: 0x04000063 RID: 99
		public static Space space = 1;

		// Token: 0x04000064 RID: 100
		public static bool orientToPath = false;

		// Token: 0x04000065 RID: 101
		public static Color color = Color.white;

		// Token: 0x04000066 RID: 102
		public static float updateTimePercentage = 0.05f;

		// Token: 0x04000067 RID: 103
		public static float updateTime = 1f * iTween.Defaults.updateTimePercentage;

		// Token: 0x04000068 RID: 104
		public static int cameraFadeDepth = 999999;

		// Token: 0x04000069 RID: 105
		public static float lookAhead = 0.05f;

		// Token: 0x0400006A RID: 106
		public static bool useRealTime = false;

		// Token: 0x0400006B RID: 107
		public static Vector3 up = Vector3.up;
	}

	// Token: 0x0200000A RID: 10
	private class CRSpline
	{
		// Token: 0x06000102 RID: 258 RVA: 0x00002E03 File Offset: 0x00001003
		public CRSpline(params Vector3[] pts)
		{
			this.pts = new Vector3[pts.Length];
			Array.Copy(pts, this.pts, pts.Length);
		}

		// Token: 0x06000103 RID: 259 RVA: 0x0001DCC8 File Offset: 0x0001BEC8
		public Vector3 Interp(float t)
		{
			int num = this.pts.Length - 3;
			int num2 = Mathf.Min(Mathf.FloorToInt(t * (float)num), num - 1);
			float num3 = t * (float)num - (float)num2;
			Vector3 vector = this.pts[num2];
			Vector3 vector2 = this.pts[num2 + 1];
			Vector3 vector3 = this.pts[num2 + 2];
			Vector3 vector4 = this.pts[num2 + 3];
			return 0.5f * ((-vector + 3f * vector2 - 3f * vector3 + vector4) * (num3 * num3 * num3) + (2f * vector - 5f * vector2 + 4f * vector3 - vector4) * (num3 * num3) + (-vector + vector3) * num3 + 2f * vector2);
		}

		// Token: 0x0400006C RID: 108
		public Vector3[] pts;
	}

	// Token: 0x0200000B RID: 11
	// (Invoke) Token: 0x06000105 RID: 261
	private delegate float EasingFunction(float start, float end, float value);

	// Token: 0x0200000C RID: 12
	// (Invoke) Token: 0x06000109 RID: 265
	private delegate void ApplyTween();
}
