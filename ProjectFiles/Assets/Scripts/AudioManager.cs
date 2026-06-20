using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("SE Clips")]
    public AudioClip hitSound;
    public AudioClip magicHitSound;
    public AudioClip healSound;
    public AudioClip playerHitSound;
    public AudioClip guardHitSound;
    public AudioClip uiChoiceSound;
    public AudioClip uiKillSound;
    public AudioClip uiNextPageSound;
    public AudioClip uiPreviousPageSound;

    [Header("同じSEの最短再生間隔（秒）")]
    public float seSameInterval = 0.05f;

    private AudioSource audioSource;
    private Dictionary<AudioClip, float> lastPlayTimes = new Dictionary<AudioClip, float>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        AudioListener.volume = 0.5f;
    }

    public void PlaySE(AudioClip clip)
    {
        if (clip == null) return;

        float now = Time.time;
        if (lastPlayTimes.TryGetValue(clip, out float lastTime) && now - lastTime < seSameInterval)
            return;

        lastPlayTimes[clip] = now;
        audioSource.PlayOneShot(clip);
    }
}
