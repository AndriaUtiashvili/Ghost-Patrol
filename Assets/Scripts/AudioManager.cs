using UnityEngine;

public enum GameSound
{
    Blaster,
    GhostHit,
    GhostDefeated,
    PlayerDamage,
    GhostEscaped,
    Flashlight,
    WeaponUpgrade,
    WaveCompleted,
    BossArrival,
    Victory,
    GameOver,
    MenuClick
}

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Gameplay Sounds")]
    [SerializeField] private AudioClip blaster;
    [SerializeField] private AudioClip ghostHit;
    [SerializeField] private AudioClip ghostDefeated;
    [SerializeField] private AudioClip playerDamage;
    [SerializeField] private AudioClip ghostEscaped;
    [SerializeField] private AudioClip flashlight;

    [Header("Progress Sounds")]
    [SerializeField] private AudioClip weaponUpgrade;
    [SerializeField] private AudioClip waveCompleted;
    [SerializeField] private AudioClip bossArrival;
    [SerializeField] private AudioClip victory;
    [SerializeField] private AudioClip gameOver;

    [Header("Menu")]
    [SerializeField] private AudioClip menuClick;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)]
    private float masterVolume = 0.7f;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;
    }

    public void Play(GameSound sound)
    {
        AudioClip clip = null;
        float volume = 1f;

        switch (sound)
        {
            case GameSound.Blaster:
                clip = blaster;
                volume = 0.35f;
                break;

            case GameSound.GhostHit:
                clip = ghostHit;
                volume = 0.45f;
                break;

            case GameSound.GhostDefeated:
                clip = ghostDefeated;
                volume = 0.6f;
                break;

            case GameSound.PlayerDamage:
                clip = playerDamage;
                volume = 0.8f;
                break;

            case GameSound.GhostEscaped:
                clip = ghostEscaped;
                volume = 0.7f;
                break;

            case GameSound.Flashlight:
                clip = flashlight;
                volume = 0.7f;
                break;

            case GameSound.WeaponUpgrade:
                clip = weaponUpgrade;
                break;

            case GameSound.WaveCompleted:
                clip = waveCompleted;
                break;

            case GameSound.BossArrival:
                clip = bossArrival;
                break;

            case GameSound.Victory:
                clip = victory;
                break;

            case GameSound.GameOver:
                clip = gameOver;
                break;

            case GameSound.MenuClick:
                clip = menuClick;
                volume = 0.7f;
                break;
        }

        if (clip != null)
        {
            audioSource.PlayOneShot(
                clip,
                volume * masterVolume
            );
        }
    }

    [ContextMenu("Preview Blaster Sound")]
    private void PreviewBlasterSound()
    {
        if (Application.isPlaying)
        {
            Play(GameSound.Blaster);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}