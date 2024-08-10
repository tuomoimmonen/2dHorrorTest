using UnityEngine;

public class PlayRandomSound : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip[] sounds;
    [SerializeField] float minPitch = 0.95f;
    [SerializeField] float maxPitch = 1.05f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
    }
}
