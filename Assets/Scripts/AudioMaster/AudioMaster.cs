using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMaster : MonoBehaviour {

	static AudioMaster instance;

	Dictionary<AudioSource, Coroutine> playingAudios = new Dictionary<AudioSource, Coroutine>();

	public AudioMaster Instance {
		get {
			if (instance == null) {
				instance = new GameObject("Audio Master").AddComponent<AudioMaster>();
			}
			return instance;
		}
		protected set {
			instance = value;
		}
	}

	public AudioSource Play(SoundHolder holder) {
		if  (holder == null) {
			Debug.Log("No holder");
			return null;
		}
		AudioClip clip = holder.clip;
		int random = -1;
		if (holder.additionalClips != null && holder.additionalClips.Length > 0) {
			random = Random.Range(-1, holder.additionalClips.Length);
			if (random >= 0)
				clip = holder.additionalClips[random];
		}

		if (clip == null) {
			if (random == -1)
				Debug.Log("No clip in main clip slot");
			else
				Debug.Log($"No clip in additional clip slot { random +1}");
			return null;
		}
		AudioSource audioSource = new GameObject("Sound - " + clip.name).AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.outputAudioMixerGroup = holder.mixerGroup;
		audioSource.loop = holder.loop;
		audioSource.volume = Random.Range(holder.minVolume, holder.maxVolume);
		audioSource.pitch = Random.Range(holder.minPitch, holder.maxPitch);
		audioSource.Play();
		playingAudios.Add(audioSource, StartCoroutine(SoundRoutine(audioSource)));
		return audioSource;
	}

	public void Stop(AudioSource audioSource) {
		if (playingAudios.ContainsKey(audioSource)) {
			audioSource.Stop();
			StopCoroutine(playingAudios[audioSource]);
			playingAudios.Remove(audioSource);
			Destroy(audioSource.gameObject);
		}
	}

	IEnumerator SoundRoutine(AudioSource audioSource) {
		//yield return null; //Might need, not sure
		while (audioSource.isPlaying)
			yield return null;
		playingAudios.Remove(audioSource);
		Destroy(audioSource.gameObject);
	}
}
