using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{

    [SerializeField] private AudioSource _effectsSource;

    public List<AudioClip> audioClips;
    private Dictionary<string, AudioClip> audioDict = new Dictionary<string, AudioClip>();

    private void Start()
    {
        _effectsSource = GetComponent<AudioSource>();

        foreach (var clip in audioClips)
        {
            audioDict.Add(clip.name, clip);
        }
    }

    // Play a given audio clip
    public void PlayAudioClip(AudioClip clip)
    {
        _effectsSource.clip = clip;
        _effectsSource.Play();
    }




    public void SelectRandomAudio(string clipNames)
    {
        string[] audioClipNames = clipNames.Split(',');
        List<AudioClip> audioClips = new List<AudioClip>();

        foreach (string clipName in audioClipNames)
        {
            audioDict.TryGetValue(clipName, out AudioClip clip);
            audioClips.Add(clip);
        }

        int index = Random.Range(0, audioClips.Count);

        _effectsSource.PlayOneShot(audioClips[index]);

    }

}
