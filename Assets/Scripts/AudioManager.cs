using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip successClip;
    [SerializeField] private AudioClip errorClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlaySuccess()
    {
        if (successClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(successClip);
        }
    }

    public void PlayError()
    {
        if (errorClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(errorClip);
        }
    }
}
