using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SemitruckSoundPlayer : MonoBehaviour
{
    [SerializeField]
    private float maxVolume = 1f;

    [SerializeField]
    private float distToHearSound;
    
    [SerializeField]
    private float distToHearMaxSound;

    private static SemitruckSoundPlayer singleton;
    private static AudioSource audioSource;
    private static bool wasSoundSetThisFrame;
    private static float volume;

    void Awake()
    {
        singleton = this;
        audioSource = GetComponent<AudioSource>();
        volume = 0f;
        audioSource.volume = 0f;
        wasSoundSetThisFrame = false;
    }

    void LateUpdate()
    {
        if (wasSoundSetThisFrame)
        {
            wasSoundSetThisFrame = false;
        }
        else
        {
            volume = 0f;
        }

        audioSource.volume = volume;
        volume = 0f;
    }

    public static void SetVolume(Vector3 position)
    {
        if (audioSource == null) return;

        float dist = Vector3.Distance(position, Player.Singleton.transform.position);

        float newVolume;
        try
        {
            newVolume = (singleton.distToHearSound - dist) / Mathf.Abs(singleton.distToHearSound - singleton.distToHearMaxSound);
        }
        catch
        {
            newVolume = 1f;
        }
        newVolume *= singleton.maxVolume;

        if (newVolume > 0f)
            print(newVolume);

        volume = Mathf.Max(volume, newVolume);
        wasSoundSetThisFrame = true;
    }
}
