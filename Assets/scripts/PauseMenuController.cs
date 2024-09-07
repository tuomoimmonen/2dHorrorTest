using System;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    public static Action OnGamePaused;
    public static Action OnGameUnPaused;

    private void Awake()
    {
        pauseMenu.SetActive(false);
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        pauseMenu?.SetActive(true);
        OnGamePaused?.Invoke();
        Time.timeScale = 0;
    }

    public void UnPauseGame()
    {
        OnGameUnPaused?.Invoke();
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }
}
