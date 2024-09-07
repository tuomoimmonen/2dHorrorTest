using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    AudioSource musicSource;
    [SerializeField] AudioClip safeMusic;
    [SerializeField] AudioClip combatMusic;
    [SerializeField] string[] safeRoomScenes = { "MainMenu", "Intro", "Outro" };

    private void Awake()
    {
        if (instance == null)
        {
            musicSource = GetComponent<AudioSource>();
            musicSource.clip = safeMusic;
            musicSource.Play();
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if(this != instance)
            {
                Destroy(gameObject);
            }
        }
    }

    public void UpdateCurrentTrack()
    {
        string curScene = SceneManager.GetActiveScene().name;
        bool inSafeRoom = false;

        foreach (string room in safeRoomScenes)
        {
            if(room == curScene)
            {
                inSafeRoom = true;
                break;
            }
        }
        AudioClip curTrack = combatMusic;

        if (inSafeRoom)
        {
            curTrack = safeMusic;
        }

        if (musicSource.clip != curTrack)
        {
            musicSource.clip = curTrack;
            musicSource.Play();
        }
    }

}
