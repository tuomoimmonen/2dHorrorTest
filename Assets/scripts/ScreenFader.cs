using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] float waitTime = 1.0f;
    Animator animator;

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
        FadeToBlack();
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator LoadSceneAfterFadeToBlack(int sceneIndex)
    {
        FadeToBlack();
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }



}
