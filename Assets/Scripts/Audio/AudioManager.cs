using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using static BattleSystem;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton instance

    [SerializeField] private AudioSource _audioSource, _effectsSource;

    public List<AudioClip> audioClips;
    private Dictionary<string, AudioClip> audioDict = new Dictionary<string, AudioClip>();

    private void OnEnable()
    {
        PlayerCollision.OnBattleTriggered += PlayBattleMusic;
        OnBattleState += SetBattleMusic;
        OnEnemyAction += HandleEnemyAction;

    }

    private void OnDisable()
    {
        PlayerCollision.OnBattleTriggered -= PlayBattleMusic;
        OnBattleState -= SetBattleMusic;
        OnEnemyAction -= HandleEnemyAction;
    }

    void Awake()
    {
        // Implementing a simple Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep the audio manager across scenes
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach (var clip in audioClips)
        {
            audioDict.Add(clip.name, clip);
        }
    }

    public void PlaySound(string clipName)
    {
        if (audioDict.TryGetValue(clipName, out AudioClip clip))
        {
            _audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Clip not found: " + clipName);
        }
    }

    public void StopSound(string clipName)
    {
        if (audioDict.TryGetValue(clipName, out AudioClip clip))
        {
            _audioSource.Stop();
        }
        else
        {
            Debug.LogWarning("Clip not found: " + clipName);
        }
    }

    public void PlayBattleMusic()
    {
        if (audioDict.TryGetValue("BattleTheme", out AudioClip clip))
        {
            Debug.Log("Playing song " + clip.name);
            _audioSource.PlayOneShot(clip);
        }
    }



    private void SetBattleMusic(BattleState actionType)
    {
        switch (actionType)
        {
            case BattleState.WON:
                Debug.Log("Stop battle music");
                StopSound("BattleTheme");
                break;

            default: 
                break;
        }

    }

    // Play sound effects depending on the enemies action,
    // probably should move this to its own script like the players for each enemies unique sound effects
    private void HandleEnemyAction(BattleActionType actionType, Unit enemy)
    {
        switch (actionType)
        {
            case BattleActionType.Start:
                break;
            case BattleActionType.Attack:
                break;
            case BattleActionType.Gaurd:
                break;
            case BattleActionType.Arcane:
                break;
            case BattleActionType.Die:
                break;
            case BattleActionType.Damaged:
                break;
            default:
                break;
        }
    }

}
