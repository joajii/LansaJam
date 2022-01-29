using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName ="Sound Holder")]
public class SoundHolder : ScriptableObject {
	public AudioClip clip;
	public AudioClip[] additionalClips;
	public AudioMixerGroup mixerGroup;
	public float minVolume = 1;
	public float maxVolume = 1;
	public float minPitch = 1;
	public float maxPitch = 1;
	public bool loop;
}
