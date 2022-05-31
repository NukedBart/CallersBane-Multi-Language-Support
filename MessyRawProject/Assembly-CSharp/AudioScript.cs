using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000034 RID: 52
public class AudioScript : MonoBehaviour
{
	// Token: 0x06000250 RID: 592 RVA: 0x00021908 File Offset: 0x0001FB08
	public void SetVolume(AudioScript.ESoundType type, float volume)
	{
		float num = Mathf.Pow(volume, 2f);
		this.volumesStorage[type] = num;
		if (!this.isMusicPaused)
		{
			this.volumesCurrent[type] = num;
		}
		foreach (AudioScript.Sound sound in this.sounds)
		{
			if (sound.type == type && sound.isActive && !this.isMusicPaused)
			{
				sound.setVolume(num);
			}
		}
		if (type != AudioScript.ESoundType.MUSIC)
		{
			if (type == AudioScript.ESoundType.SFX)
			{
				App.Config.settings.sound.volume.value = num;
			}
		}
		else
		{
			App.Config.settings.music.volume.value = num;
			App.Config.globalSettings.music.volume.value = num;
		}
	}

	// Token: 0x06000251 RID: 593 RVA: 0x00003BCD File Offset: 0x00001DCD
	public float GetVolume(AudioScript.ESoundType type)
	{
		return Mathf.Sqrt(this.volumesCurrent[type]);
	}

	// Token: 0x06000252 RID: 594 RVA: 0x00021A24 File Offset: 0x0001FC24
	public void SetSoundToggle(AudioScript.ESoundToggle toggle, bool state)
	{
		this.soundToggles[toggle] = state;
		if (toggle != AudioScript.ESoundToggle.CHAT)
		{
			if (toggle == AudioScript.ESoundToggle.CHAT_HIGHLIGHT)
			{
				App.Config.settings.sound.chat_highlight.value = state;
			}
		}
		else
		{
			App.Config.settings.sound.chat_message.value = state;
		}
	}

	// Token: 0x06000253 RID: 595 RVA: 0x00003BE0 File Offset: 0x00001DE0
	public bool GetSoundToggle(AudioScript.ESoundToggle toggle)
	{
		return this.soundToggles[toggle];
	}

	// Token: 0x06000254 RID: 596 RVA: 0x00003BEE File Offset: 0x00001DEE
	public void PlaySFX(string soundPath)
	{
		this.PlaySFX(soundPath, false);
	}

	// Token: 0x06000255 RID: 597 RVA: 0x00003BF8 File Offset: 0x00001DF8
	public void PlaySFX(string soundPath, bool loop)
	{
		this.PlaySound(soundPath, AudioScript.ESoundType.SFX, loop, false, 1f);
	}

	// Token: 0x06000256 RID: 598 RVA: 0x00003C0A File Offset: 0x00001E0A
	public void PlaySFX(string soundPath, float pitch)
	{
		this.PlaySound(soundPath, AudioScript.ESoundType.SFX, false, false, 1f).source.pitch = pitch;
	}

	// Token: 0x06000257 RID: 599 RVA: 0x00003C26 File Offset: 0x00001E26
	public void PlaySFX(string soundPath, float volume, float pitch)
	{
		this.PlaySound(soundPath, AudioScript.ESoundType.SFX, false, false, volume).source.pitch = pitch;
	}

	// Token: 0x06000258 RID: 600 RVA: 0x00003C3E File Offset: 0x00001E3E
	public void PlayMusic(string soundPath)
	{
		this.simultaneousTracks.Clear();
		this.PlayExclusiveSound(soundPath, AudioScript.ESoundType.MUSIC, true, true, true, AudioScript.EPostFadeoutBehaviour.STOP);
	}

	// Token: 0x06000259 RID: 601 RVA: 0x00003C58 File Offset: 0x00001E58
	public void PlayMusic(string soundPath, AudioScript.EPostFadeoutBehaviour postFadeoutBehaviour)
	{
		this.simultaneousTracks.Clear();
		this.PlayExclusiveSound(soundPath, AudioScript.ESoundType.MUSIC, true, true, true, postFadeoutBehaviour);
	}

	// Token: 0x0600025A RID: 602 RVA: 0x00021A90 File Offset: 0x0001FC90
	public void StartSimultaneousSilentMusic(string[] soundPaths)
	{
		double num = AudioSettings.dspTime + 1.5;
		Log.info("Scheduling tracks for time: " + num);
		foreach (string text in soundPaths)
		{
			if (!this.simultaneousTracks.Contains(text))
			{
				this.simultaneousTracks.Add(text);
				AudioScript.Sound sound = this.CheckLoadSound(text, AudioScript.ESoundType.MUSIC);
				sound.source.loop = true;
				sound.setVolume(0f);
				sound.source.PlayScheduled(num);
				sound.isActive = false;
			}
		}
	}

	// Token: 0x0600025B RID: 603 RVA: 0x00021B34 File Offset: 0x0001FD34
	public void FadeInSimultaneousTrack(int index)
	{
		if (index < 0 || index >= this.simultaneousTracks.Count)
		{
			Log.error(string.Concat(new object[]
			{
				"Simultaneous track index ",
				index,
				" is out of bounds (",
				this.simultaneousTracks.Count,
				" current tracks)."
			}));
			return;
		}
		if (this.simultaneousTracks.Count == 0)
		{
			Log.error("Failed to fade in simultaneous track");
			return;
		}
		string text = this.simultaneousTracks[index];
		AudioScript.Sound sound = this.CheckLoadSound(text, AudioScript.ESoundType.MUSIC);
		sound.isActive = true;
		foreach (AudioScript.Sound sound2 in this.sounds)
		{
			if (sound2.isActive && sound2.type == AudioScript.ESoundType.MUSIC && sound2.name != text)
			{
				this.SafeStartCoroutine(this.FadeOut(sound2, 5f, true, AudioScript.EPostFadeoutBehaviour.NONE));
			}
		}
		this.SafeStartCoroutine(this.FadeIn(sound, 5f));
	}

	// Token: 0x0600025C RID: 604 RVA: 0x00021C6C File Offset: 0x0001FE6C
	public void StopSound(string soundPath, bool fadeOut)
	{
		foreach (AudioScript.Sound sound in this.sounds)
		{
			if (sound.isActive && sound.name == soundPath)
			{
				this.FadeOutAudio(sound, !fadeOut, AudioScript.EPostFadeoutBehaviour.STOP);
			}
		}
	}

	// Token: 0x0600025D RID: 605 RVA: 0x00021CE8 File Offset: 0x0001FEE8
	public void StopSoundsOfType(AudioScript.ESoundType type, bool fadeOut, AudioScript.EPostFadeoutBehaviour postFadeoutBehaviour)
	{
		foreach (AudioScript.Sound sound in this.sounds)
		{
			if (sound.isActive && sound.type == type)
			{
				this.FadeOutAudio(sound, !fadeOut, postFadeoutBehaviour);
			}
		}
	}

	// Token: 0x0600025E RID: 606 RVA: 0x00021D60 File Offset: 0x0001FF60
	protected void Start()
	{
		this.volumesCurrent = new Dictionary<AudioScript.ESoundType, float>();
		this.volumesStorage = new Dictionary<AudioScript.ESoundType, float>();
		this.soundToggles = new Dictionary<AudioScript.ESoundToggle, bool>();
		foreach (object obj in Enum.GetValues(typeof(AudioScript.ESoundType)))
		{
			this.volumesStorage.Add((AudioScript.ESoundType)((int)obj), 0.5f);
			this.volumesCurrent.Add((AudioScript.ESoundType)((int)obj), 0.5f);
		}
		foreach (object obj2 in Enum.GetValues(typeof(AudioScript.ESoundToggle)))
		{
			this.soundToggles.Add((AudioScript.ESoundToggle)((int)obj2), true);
		}
		this.OnUserSettingsLoaded();
		this.isStarted = true;
	}

	// Token: 0x0600025F RID: 607 RVA: 0x00021E84 File Offset: 0x00020084
	public void OnUserSettingsLoaded()
	{
		if (App.Config.HasLoadedUserSettings())
		{
			this.SetVolume(AudioScript.ESoundType.MUSIC, Mathf.Sqrt(App.Config.settings.music.volume));
			this.SetVolume(AudioScript.ESoundType.SFX, Mathf.Sqrt(App.Config.settings.sound.volume));
			this.SetSoundToggle(AudioScript.ESoundToggle.CHAT, App.Config.settings.sound.chat_message);
			this.SetSoundToggle(AudioScript.ESoundToggle.CHAT_HIGHLIGHT, App.Config.settings.sound.chat_highlight);
		}
		else
		{
			this.SetVolume(AudioScript.ESoundType.MUSIC, Mathf.Sqrt(App.Config.globalSettings.music.volume));
			this.SetVolume(AudioScript.ESoundType.SFX, Mathf.Sqrt(0.5f));
		}
	}

	// Token: 0x06000260 RID: 608 RVA: 0x00021F68 File Offset: 0x00020168
	public void InjectSound(string soundName, AudioClip clip)
	{
		string text = soundName.ToLower();
		for (int i = this.sounds.Count - 1; i >= 0; i--)
		{
			if (this.sounds[i].name.ToLower() == text)
			{
				this.sounds[i].source.clip = clip;
				return;
			}
		}
		this.AddSound(soundName, AudioScript.ESoundType.SFX, clip);
	}

	// Token: 0x06000261 RID: 609 RVA: 0x00021FE0 File Offset: 0x000201E0
	protected virtual AudioScript.Sound CheckLoadSound(string soundName, AudioScript.ESoundType type)
	{
		foreach (AudioScript.Sound sound in this.sounds)
		{
			if (sound.name == soundName)
			{
				return sound;
			}
		}
		return this.AddSound(soundName, type, (AudioClip)ResourceManager.Load(soundName));
	}

	// Token: 0x06000262 RID: 610 RVA: 0x00022060 File Offset: 0x00020260
	private AudioScript.Sound AddSound(string soundName, AudioScript.ESoundType type, AudioClip clip)
	{
		AudioSource audioSource = new GameObject().AddComponent<AudioSource>();
		audioSource.name = soundName;
		audioSource.transform.parent = base.transform;
		audioSource.clip = clip;
		AudioScript.Sound sound = new AudioScript.Sound(soundName, audioSource, type);
		this.sounds.Add(sound);
		return sound;
	}

	// Token: 0x06000263 RID: 611 RVA: 0x00003C72 File Offset: 0x00001E72
	private void OnApplicationFocus(bool focus)
	{
		if (App.Config.settings.music.pause_when_minimized)
		{
			this.PauseMusic(!focus);
		}
	}

	// Token: 0x06000264 RID: 612 RVA: 0x00003C9C File Offset: 0x00001E9C
	private void OnApplicationPause(bool pause)
	{
		if (App.Config.settings.music.pause_when_minimized)
		{
			this.PauseMusic(pause);
		}
	}

	// Token: 0x06000265 RID: 613 RVA: 0x000220B0 File Offset: 0x000202B0
	private void PauseMusic(bool pause)
	{
		if (this.isStarted)
		{
			if (pause)
			{
				this.volumesCurrent[AudioScript.ESoundType.MUSIC] = 0f;
				foreach (AudioScript.Sound sound in this.sounds)
				{
					if (sound.isActive && sound.type == AudioScript.ESoundType.MUSIC)
					{
						this.SafeStartCoroutine(this.FadeOut(sound, 0.7f, false, AudioScript.EPostFadeoutBehaviour.NONE));
					}
				}
			}
			else
			{
				this.volumesCurrent[AudioScript.ESoundType.MUSIC] = this.volumesStorage[AudioScript.ESoundType.MUSIC];
				foreach (AudioScript.Sound sound2 in this.sounds)
				{
					if (sound2.isActive && sound2.type == AudioScript.ESoundType.MUSIC)
					{
						this.SafeStartCoroutine(this.FadeIn(sound2, 1.4f));
					}
				}
			}
			this.isMusicPaused = pause;
		}
	}

	// Token: 0x06000266 RID: 614 RVA: 0x00003CC3 File Offset: 0x00001EC3
	protected AudioScript.Sound PlaySound(string soundPath, AudioScript.ESoundType type, bool loop, bool fadeIn)
	{
		return this.PlaySound(soundPath, type, loop, fadeIn, 1f);
	}

	// Token: 0x06000267 RID: 615 RVA: 0x000221E0 File Offset: 0x000203E0
	protected virtual AudioScript.Sound PlaySound(string soundPath, AudioScript.ESoundType type, bool loop, bool fadeIn, float volume)
	{
		AudioScript.Sound sound = this.CheckLoadSound(soundPath, type);
		if (sound == null)
		{
			throw new ArgumentException("no sound found for: " + soundPath);
		}
		sound.source.pitch = 1f;
		sound.setBaseVolume(volume);
		if (type == AudioScript.ESoundType.SFX || !sound.source.isPlaying)
		{
			sound.source.loop = loop;
			this.FadeInAudio(sound, !fadeIn);
		}
		return sound;
	}

	// Token: 0x06000268 RID: 616 RVA: 0x00022258 File Offset: 0x00020458
	private AudioScript.Sound PlayExclusiveSound(string soundPath, AudioScript.ESoundType type, bool loop, bool fadeIn, bool fadeOutOthers, AudioScript.EPostFadeoutBehaviour postFadeoutBehaviour)
	{
		foreach (AudioScript.Sound sound in this.sounds)
		{
			if (sound.isActive && sound.type == type && sound.name != soundPath)
			{
				this.FadeOutAudio(sound, !fadeOutOthers, postFadeoutBehaviour);
			}
		}
		return this.PlaySound(soundPath, type, loop, fadeIn);
	}

	// Token: 0x06000269 RID: 617 RVA: 0x000222EC File Offset: 0x000204EC
	private void FadeInAudio(AudioScript.Sound sound, bool immediate)
	{
		if (this.isMusicPaused && sound.type == AudioScript.ESoundType.MUSIC)
		{
			sound.setVolume(0f);
			sound.source.Play();
			sound.isActive = true;
			return;
		}
		if (immediate)
		{
			sound.setVolume(this.volumesCurrent[sound.type]);
			sound.source.Play();
			sound.isActive = true;
			return;
		}
		this.SafeStartCoroutine(this.FadeIn(sound, 0.7f));
	}

	// Token: 0x0600026A RID: 618 RVA: 0x00022370 File Offset: 0x00020570
	private void FadeOutAudio(AudioScript.Sound sound, bool immediate, AudioScript.EPostFadeoutBehaviour postFadeoutBehaviour)
	{
		if (immediate)
		{
			sound.isActive = false;
			sound.setVolume(0f);
			if (postFadeoutBehaviour == AudioScript.EPostFadeoutBehaviour.STOP)
			{
				sound.source.Stop();
			}
			if (postFadeoutBehaviour == AudioScript.EPostFadeoutBehaviour.PAUSE)
			{
				sound.source.Pause();
			}
			return;
		}
		this.SafeStartCoroutine(this.FadeOut(sound, 0.7f, true, postFadeoutBehaviour));
	}

	// Token: 0x0600026B RID: 619 RVA: 0x000223D0 File Offset: 0x000205D0
	private IEnumerator FadeOut(AudioScript.Sound sound, float timer, bool makeInactive, AudioScript.EPostFadeoutBehaviour postFadeoutBehaviour)
	{
		if (!sound.isActive)
		{
			yield break;
		}
		if (makeInactive)
		{
			sound.isActive = false;
		}
		if (this.isInvalid())
		{
			yield break;
		}
		float start = sound.source.volume;
		float end = 0f;
		float t = 0f;
		float timeStarted = Time.time;
		while (t < 1f)
		{
			t = Mathf.Min((Time.time - timeStarted) / timer, 1f);
			if (this.isInvalid())
			{
				yield break;
			}
			sound.source.volume = end + (start - end) * Mathf.Cos(t * 3.1415927f / 2f);
			yield return null;
		}
		if (this.isInvalid())
		{
			yield break;
		}
		sound.source.volume = end;
		if (postFadeoutBehaviour != AudioScript.EPostFadeoutBehaviour.PAUSE)
		{
			if (postFadeoutBehaviour == AudioScript.EPostFadeoutBehaviour.STOP)
			{
				sound.source.Stop();
			}
		}
		else
		{
			sound.source.Pause();
		}
		yield break;
	}

	// Token: 0x0600026C RID: 620 RVA: 0x00022428 File Offset: 0x00020628
	private IEnumerator FadeIn(AudioScript.Sound sound, float timer)
	{
		if (this.isInvalid())
		{
			yield break;
		}
		if (!sound.source.isPlaying)
		{
			sound.setVolume(0f);
			sound.source.Play();
		}
		float start = sound.source.volume;
		float end = sound.getCalculatedVolume(this.volumesStorage[sound.type]);
		float t = 0f;
		float timeStarted = Time.time;
		while (t < 1f)
		{
			t = Mathf.Min((Time.time - timeStarted) / timer, 1f);
			if (this.isInvalid())
			{
				yield break;
			}
			sound.source.volume = start + (end - start) * Mathf.Sin(t * 3.1415927f / 2f);
			yield return null;
		}
		if (this.isInvalid())
		{
			yield break;
		}
		sound.source.volume = end;
		sound.isActive = true;
		yield break;
	}

	// Token: 0x0600026D RID: 621 RVA: 0x00003CD5 File Offset: 0x00001ED5
	private bool isInvalid()
	{
		return !this;
	}

	// Token: 0x0600026E RID: 622 RVA: 0x00003CE0 File Offset: 0x00001EE0
	private void SafeStartCoroutine(IEnumerator e)
	{
		if (this.isInvalid())
		{
			return;
		}
		base.StartCoroutine(e);
	}

	// Token: 0x0400012C RID: 300
	public const float DELAY_SIMULTANEOUS_SONG_SWITCH = 1.5f;

	// Token: 0x0400012D RID: 301
	private const float DEFAULT_VOLUME_SFX = 0.5f;

	// Token: 0x0400012E RID: 302
	private const float DEFAULT_VOLUME_MUSIC = 0.5f;

	// Token: 0x0400012F RID: 303
	private const string VOLUME_SFX = "VOLUME_SFX";

	// Token: 0x04000130 RID: 304
	private const string VOLUME_MUSIC = "VOLUME_MUSIC";

	// Token: 0x04000131 RID: 305
	private const string TOGGLE_CHAT = "TOGGLE_CHAT";

	// Token: 0x04000132 RID: 306
	private const string TOGGLE_CHAT_HIGHLIGHT = "TOGGLE_CHAT_HIGHLIGHT";

	// Token: 0x04000133 RID: 307
	protected List<AudioScript.Sound> sounds = new List<AudioScript.Sound>();

	// Token: 0x04000134 RID: 308
	private Dictionary<AudioScript.ESoundType, float> volumesCurrent;

	// Token: 0x04000135 RID: 309
	private Dictionary<AudioScript.ESoundType, float> volumesStorage;

	// Token: 0x04000136 RID: 310
	private Dictionary<AudioScript.ESoundToggle, bool> soundToggles;

	// Token: 0x04000137 RID: 311
	private bool isMusicPaused;

	// Token: 0x04000138 RID: 312
	private bool isStarted;

	// Token: 0x04000139 RID: 313
	private List<string> simultaneousTracks = new List<string>();

	// Token: 0x02000035 RID: 53
	public enum EPostFadeoutBehaviour
	{
		// Token: 0x0400013B RID: 315
		NONE,
		// Token: 0x0400013C RID: 316
		PAUSE,
		// Token: 0x0400013D RID: 317
		STOP
	}

	// Token: 0x02000036 RID: 54
	public enum ESoundType
	{
		// Token: 0x0400013F RID: 319
		MUSIC,
		// Token: 0x04000140 RID: 320
		SFX
	}

	// Token: 0x02000037 RID: 55
	public enum ESoundToggle
	{
		// Token: 0x04000142 RID: 322
		CHAT,
		// Token: 0x04000143 RID: 323
		CHAT_HIGHLIGHT
	}

	// Token: 0x02000038 RID: 56
	protected class Sound
	{
		// Token: 0x0600026F RID: 623 RVA: 0x00003CF6 File Offset: 0x00001EF6
		public Sound(string name, AudioSource source, AudioScript.ESoundType type)
		{
			this.name = name;
			this.source = source;
			this.type = type;
			this.isActive = false;
		}

		// Token: 0x06000270 RID: 624 RVA: 0x00003D25 File Offset: 0x00001F25
		public AudioScript.Sound setBaseVolume(float volume)
		{
			this.baseVolume = volume;
			return this;
		}

		// Token: 0x06000271 RID: 625 RVA: 0x00003D2F File Offset: 0x00001F2F
		public void setVolume(float volume)
		{
			this.source.volume = this.getCalculatedVolume(volume);
		}

		// Token: 0x06000272 RID: 626 RVA: 0x00003D43 File Offset: 0x00001F43
		public float getCalculatedVolume(float volume)
		{
			return volume * this.baseVolume;
		}

		// Token: 0x04000144 RID: 324
		public string name;

		// Token: 0x04000145 RID: 325
		public AudioSource source;

		// Token: 0x04000146 RID: 326
		public AudioScript.ESoundType type;

		// Token: 0x04000147 RID: 327
		public bool isActive;

		// Token: 0x04000148 RID: 328
		private float baseVolume = 1f;
	}
}
