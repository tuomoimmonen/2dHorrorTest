using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] float waitTime = 1.0f;
    Animator animator;

    [Header("Events")]
    public static Action OnLevelFinished;


    private void Start()
    {
        if(MusicManager.instance != null)
        {
            MusicManager.instance.UpdateCurrentTrack();
        }
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadSceneAfterFadeToBlack(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void RestartLevel()
    {
        StartCoroutine(LoadSceneAfterFadeToBlack(SceneManager.GetActiveScene().buildIndex));

    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadSceneAfterFadeToBlack("MainMenu"));
    }

    void FadeToBlack()
    {
        animator.SetTrigger("FadeToBlack");
    }

    IEnumerator LoadSceneAfterFadeToBlack(string sceneName)
    {
        OnLevelFinished?.Invoke();
        Time.timeScale = 1f;
        FadeToBlack();
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator LoadSceneAfterFadeToBlack(int sceneIndex)
    {
        OnLevelFinished?.Invoke();
        Time.timeScale = 1f;
        FadeToBlack();
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame()
    {
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }


}
