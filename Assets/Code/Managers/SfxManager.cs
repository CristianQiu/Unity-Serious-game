using UnityEngine;

/// <summary>
/// The Sfx manager, it stores all the sound effects of the game and has a pool of AudioSources generated
/// at runtime, so it is possible to play several sound effects at the same time, with different settings
/// </summary>
public sealed class SfxManager : Singleton<SfxManager>
{
    #region Public Attributes

    [Header("Pitch")]
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;

    [Header("Clips")]
    public AudioClip clipWordCompleted = null;
    public AudioClip clipWordFailed = null;
    public AudioClip clipKeyTap = null;
    public AudioClip clipSlide = null;
    public AudioClip clipCatchKey = null;
    public AudioClip clipBip = null;
    public AudioClip clipExplosion = null;
    public AudioClip clipSkeletonDeath = null;

    #endregion

    #region Protected Attributes



    #endregion

    #region Private Attributes

    private const int AudioSourcePool = 32;

    private const float MinVolume = 0.0f;
    private const float MaxVolume = 1.0f;
    private const float DefaultVolume = 1.0f;
    private const float DefaultPitch = 1.0f;

    private float currSfxVolume = DefaultVolume;

    private AudioSource[] audioSources = null;

    #endregion

    #region Properties

    public float CurrSfxVolume { get { return currSfxVolume; } }

    #endregion

    #region MonoBehaviour Methods

    // Use this for initialization
    private void Start () 
    {
        Init();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initialization
    /// </summary>
    private void Init()
    {
        InitAudioSourcePool(AudioSourcePool);
    }

    /// <summary>
    /// Initialize the AudioSource pool
    /// </summary>
    /// <param name="quantity"></param>
    private void InitAudioSourcePool(int quantity)
    {
        audioSources = new AudioSource[quantity];

        // add all sources as child gameobjects
        for (int i = 0; i < quantity; i++)
        {
            GameObject newObj = new GameObject("AudioSource " +i);
            newObj.transform.parent = transform;
            newObj.transform.localPosition = Vector3.zero;

            AudioSource currAudioSrc = newObj.AddComponent<AudioSource>();
            audioSources[i] = currAudioSrc;

            // initialize their state to a controlled default state
            InitAudioSourceSettings(currAudioSrc, null, false, false, false, currSfxVolume, DefaultPitch, 0.0f);
        }
    }

    /// <summary>
    /// Initialize an audio source settings
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="clip"></param>
    /// <param name="muted"></param>
    /// <param name="playOnAwake"></param>
    /// <param name="loop"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <param name="spatialBlend"></param>
    private void InitAudioSourceSettings(AudioSource audioSource, AudioClip clip, bool mute, bool playOnAwake, bool loop, float volume, float pitch, float spatialBlend)
    {
        audioSource.clip = clip;
        audioSource.mute = mute;
        audioSource.playOnAwake = playOnAwake;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.spatialBlend = spatialBlend;
        // ...
    }

    #endregion

    #region Methods

    /// <summary>
    /// Find the first AudioSource that is not playing anything
    /// </summary>
    /// <returns></returns>
    private AudioSource FindAudioSrcNotPlaying()
    {
        AudioSource audioSrc = null;

        for (int i = 0; i < audioSources.Length; i++)
        {
            if (!audioSources[i].isPlaying)
            {
                audioSrc = audioSources[i];
                break;
            }
        }

        return audioSrc;
    }

    // TODO: Depending on the game each sound should have a certain amount of AudioSources available, should find a way to deal with it
    // TODO: Maybe we should differenciate between 2D and 3D sound effects and add methods to support both...

    /// <summary>
    /// Play a sound effect clip, and choose wether to randomize pitch or not
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="randomizePitch"></param>
    public void PlaySfx(AudioClip clip, bool randomizePitch)
    {
        AudioSource audioSrc = FindAudioSrcNotPlaying();
        audioSrc.transform.localPosition = Vector3.zero;

        if (audioSrc == null)
            return;

        float pitch = randomizePitch ? Random.Range(minPitch, maxPitch) : DefaultPitch;

        // initialize their state to a controlled default state
        InitAudioSourceSettings(audioSrc, clip, false, false, false, currSfxVolume, pitch, 0.0f);

        // play the sound
        audioSrc.Play();
    }

    /// <summary>
    /// Play a sound effect clip, and choose wether to randomize pitch or not
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="randomizePitch"></param>
    public void PlaySfx(AudioClip clip, bool randomizePitch, Vector3 position)
    {
        AudioSource audioSrc = FindAudioSrcNotPlaying();
        audioSrc.transform.position = position;

        if (audioSrc == null)
            return;

        float pitch = randomizePitch ? Random.Range(minPitch, maxPitch) : DefaultPitch;

        // initialize their state to a controlled default state
        InitAudioSourceSettings(audioSrc, clip, false, false, false, currSfxVolume, pitch, 0.0f);

        // play the sound
        audioSrc.Play();
    }

    /// <summary>
    /// Set the volume of all the sound effects
    /// </summary>
    /// <param name="newVolume"></param>
    public void SetSfxVolume(float newVolume)
    {
        newVolume = Mathf.Clamp(newVolume, MinVolume, MaxVolume);

        currSfxVolume = newVolume;

        for (int i = 0; i < audioSources.Length; i++)
            audioSources[i].volume = currSfxVolume;
    }

    #endregion
}