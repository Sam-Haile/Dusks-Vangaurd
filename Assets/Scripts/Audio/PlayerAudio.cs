using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{

    [SerializeField] private AudioSource _audioSource, _effectsSource;

    public List<AudioClip> audioClips;
    private Dictionary<string, AudioClip> audioDict = new Dictionary<string, AudioClip>();

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        foreach (var clip in audioClips)
        {
            audioDict.Add(clip.name, clip);
        }
    }

    // Play a given audio clip
    public void PlayAudioClip(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }

}
