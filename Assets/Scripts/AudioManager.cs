using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;



public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    static AudioManager current;
    [Header("环境声音")]
    public AudioClip ambientClip;
    public AudioClip musicClip;
    [Header("Robbie音效")]
    public AudioClip[] walkStepClips;
    public AudioClip[] crouchStepClips;
    public AudioClip jumpClip;
    public AudioClip jumpVoiceClip;
    public AudioClip deathVoiceClip;
    public AudioClip orbVoiceClip;
    public AudioClip deathClip;
    [Header("FX音效")]
    public AudioClip deathFXClip;
    public AudioClip orbFXClip;

    AudioSource ambientSource;
    AudioSource musicSource;
    AudioSource fxSource;
    AudioSource playerSouce;
    AudioSource voiceSource;


    private void Awake()
    {
        current = this;
        DontDestroyOnLoad(gameObject);
        ambientSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        fxSource = gameObject.AddComponent<AudioSource>();
        playerSouce = gameObject.AddComponent<AudioSource>();
        voiceSource = gameObject.AddComponent<AudioSource>();

        StartLevelAudio();

    }
    public static void PlayerFootstepAudio()
    {
        int index = Random.Range(0, current.walkStepClips.Length);
        current.playerSouce.clip = current.walkStepClips[index];
        current.playerSouce.Play();

    }
    public static void PlayerCrouchFootstepAudio()
    {
        int index = Random.Range(0, current.crouchStepClips.Length);
        current.playerSouce.clip = current.crouchStepClips[index];
        current.playerSouce.Play();

    }

    public static void PlayerJumpAudio()
    {
        current.playerSouce.clip = current.jumpClip;
        current.playerSouce.Play();
        current.voiceSource.clip = current.jumpVoiceClip;
        current.voiceSource.Play();

    }
    void StartLevelAudio()
    {
        current.ambientSource.clip = current.ambientClip;
        current.ambientSource.loop = true;
        current.ambientSource.Play();

        current.musicSource.clip = current.musicClip;
        current.musicSource.loop = true;
        current.musicSource.Play();
    }
    public static void PlayDeathAudio()
    {
        current.playerSouce.clip = current.deathClip;
        current.playerSouce.Play();
        current.voiceSource.clip = current.deathVoiceClip;
        current.voiceSource.Play();
        current.fxSource.clip = current.deathFXClip;
        current.fxSource.Play();
    }
    public static void orbCollectAudio()
    {
        current.voiceSource.clip = current.orbVoiceClip;
        current.voiceSource.Play();
        current.fxSource.clip = current.orbFXClip;
        current.fxSource.Play();
    }
}
